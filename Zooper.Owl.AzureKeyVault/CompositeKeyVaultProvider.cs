using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace Zooper.Owl.AzureKeyVault;

public class CompositeKeyVaultProvider : ConfigurationProvider
{
	private readonly SecretClient[] _secretClients;

	public CompositeKeyVaultProvider(IEnumerable<string> keyVaultUris)
	{
		_secretClients = keyVaultUris.Select(uri => new SecretClient(new(uri), new DefaultAzureCredential())).ToArray();
	}

	public override void Load()
	{
		// This method can be used to load secrets initially, if needed.
	}

	public override bool TryGet(
		string key,
		out string? value)
	{
		value = GetSecretAsync(key).GetAwaiter().GetResult();
		return value != null;
	}

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