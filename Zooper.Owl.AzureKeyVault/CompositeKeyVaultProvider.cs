using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace Zooper.Owl.AzureKeyVault;

/// <summary>
/// A configuration provider that enables retrieval of secrets from multiple Azure Key Vaults.
/// Implements a fallback mechanism where secrets are searched across multiple vaults in sequence.
/// </summary>
public class CompositeKeyVaultProvider : ConfigurationProvider
{
	private readonly SecretClient[] _secretClients;

	/// <summary>
	/// Initializes a new instance of the <see cref="CompositeKeyVaultProvider"/> class.
	/// </summary>
	/// <param name="keyVaultUris">A collection of Azure Key Vault URIs to search for secrets.</param>
	/// <remarks>
	/// Each URI should be in the format: https://{vault-name}.vault.azure.net/
	/// </remarks>
	public CompositeKeyVaultProvider(IEnumerable<string> keyVaultUris)
	{
		_secretClients = keyVaultUris.Select(uri => new SecretClient(new(uri), new DefaultAzureCredential())).ToArray();
	}

	/// <summary>
	/// Loads the configuration values from the key vaults.
	/// </summary>
	/// <remarks>
	/// This method can be overridden to implement custom loading behavior if needed.
	/// </remarks>
	public override void Load()
	{
		// This method can be used to load secrets initially, if needed.
	}

	/// <summary>
	/// Attempts to retrieve a secret value from the configured key vaults.
	/// </summary>
	/// <param name="key">The name of the secret to retrieve.</param>
	/// <param name="value">When this method returns, contains the secret value if found; otherwise, null.</param>
	/// <returns>true if the secret was found in any of the configured vaults; otherwise, false.</returns>
	public override bool TryGet(
		string key,
		out string? value)
	{
		value = GetSecretAsync(key).GetAwaiter().GetResult();
		return value != null;
	}

	/// <summary>
	/// Asynchronously retrieves a secret from the configured key vaults.
	/// </summary>
	/// <param name="secretName">The name of the secret to retrieve.</param>
	/// <returns>
	/// A task that represents the asynchronous operation. The task result contains the secret value if found;
	/// otherwise, null if the secret was not found in any of the configured vaults.
	/// </returns>
	/// <remarks>
	/// This method searches through each configured key vault in sequence until the secret is found.
	/// If a 404 (Not Found) error occurs, it continues searching in the next vault.
	/// </remarks>
	private async Task<string?> GetSecretAsync(string secretName)
	{
		foreach (var client in _secretClients)
		{
			try
			{
				KeyVaultSecret secret = await client.GetSecretAsync(secretName);

				if (secret != null)
				{
					return secret.Value;
				}
			}
			catch (Azure.RequestFailedException ex) when (ex.Status == 404)
			{
				// Secret not found in this vault, try the next one
			}
		}

		// The secret was not found in any of the key vaults.
		//throw new KeyNotFoundException($"Secret '{secretName}' not found in any of the key vaults.");
		return null;
	}
}