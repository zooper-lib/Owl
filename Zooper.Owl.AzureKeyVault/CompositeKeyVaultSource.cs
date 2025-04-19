using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Zooper.Owl.AzureKeyVault;

public class CompositeKeyVaultSource(IEnumerable<string> keyVaultUris) : IConfigurationSource
{
	public IConfigurationProvider Build(IConfigurationBuilder builder)
	{
		return new CompositeKeyVaultProvider(keyVaultUris);
	}
}
