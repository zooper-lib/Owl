using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace Zooper.Owl.HashicorpVault.Extensions;

/// <summary>
/// A configuration provider that enables retrieval of secrets from a Hashicorp Vault instance.
/// </summary>
/// <remarks>
/// This provider supports recursive loading of secrets from the Vault, including nested paths.
/// Secrets are loaded using the KV2 secrets engine and are transformed into configuration keys
/// by replacing path separators with colons.
/// </remarks>
public class HashicorpVaultConfigurationProvider(HashicorpVaultConfigurationSource source) : ConfigurationProvider
{
	/// <summary>
	/// Loads the configuration values from the Hashicorp Vault.
	/// </summary>
	/// <remarks>
	/// This method initializes a Vault client using the provided source configuration
	/// and recursively loads all secrets from the specified mount point.
	/// </remarks>
	public override void Load()
	{
		var client = new VaultClient(
			new(
				source.VaultUrl,
				new TokenAuthMethodInfo(source.Token)
			)
		);

		LoadRecursive(
				client,
				mountPoint: source.MountPoint
			)
			.GetAwaiter()
			.GetResult();
	}

	/// <summary>
	/// Recursively loads secrets from the Hashicorp Vault.
	/// </summary>
	/// <param name="client">The Vault client instance.</param>
	/// <param name="path">The current path to load secrets from. If null, starts from the root.</param>
	/// <param name="mountPoint">The mount point where secrets are stored. Defaults to "kv".</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	/// <remarks>
	/// This method recursively traverses the Vault's secret structure:
	/// 1. Lists all secrets at the current path
	/// 2. For each item:
	///    - If it's a folder (ends with '/'), recursively loads its contents
	///    - If it's a secret, reads its values and adds them to the configuration
	/// Secrets are transformed into configuration keys by replacing path separators with colons.
	/// </remarks>
	private async Task LoadRecursive(
		IVaultClient client,
		string? path = null,
		string mountPoint = "kv")
	{
		try
		{
			// List secrets at the current path
			var secrets = path == null
				? await client.V1.Secrets.KeyValue.V2.ReadSecretPathsAsync(
					"/",
					mountPoint
				)
				: await client.V1.Secrets.KeyValue.V2.ReadSecretPathsAsync(
					path,
					mountPoint
				);

			if (secrets?.Data?.Keys != null)
				foreach (var key in secrets.Data.Keys)
				{
					var newPath = string.IsNullOrEmpty(path) ? key : $"{path}{key}";

					// If key is a folder, list secrets in the folder
					if (key.EndsWith("/"))
					{
						await LoadRecursive(
							client,
							newPath,
							mountPoint
						);
					}
					else
					{
						// If key is a secret, read the secret
						var secret = await client.V1.Secrets.KeyValue.V2.ReadSecretAsync(
							newPath,
							mountPoint: mountPoint
						);

						foreach (var kv in secret.Data.Data)
						{
							var configurationKey = $"{newPath.Replace("/", ":")}:{kv.Key}";
							Data[configurationKey] = kv.Value?.ToString();
						}
					}
				}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred while reading from Vault: {ex.Message}");
		}
	}
}