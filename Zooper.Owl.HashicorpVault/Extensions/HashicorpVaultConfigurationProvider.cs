using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace Zooper.Owl.HashicorpVault.Extensions;

public class HashicorpVaultConfigurationProvider(HashicorpVaultConfigurationSource source) : ConfigurationProvider
{
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