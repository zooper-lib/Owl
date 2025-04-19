using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Zooper.Owl.AzureKeyVault;

/// <summary>
/// Represents a source of configuration values from multiple Azure Key Vaults.
/// </summary>
/// <remarks>
/// This class implements IConfigurationSource to provide a way to build a CompositeKeyVaultProvider
/// that can retrieve secrets from multiple Azure Key Vaults.
/// </remarks>
public class CompositeKeyVaultSource(IEnumerable<string> keyVaultUris) : IConfigurationSource
{
	/// <summary>
	/// Builds the configuration provider for the source.
	/// </summary>
	/// <param name="builder">The configuration builder.</param>
	/// <returns>A new instance of <see cref="CompositeKeyVaultProvider"/> configured with the provided key vault URIs.</returns>
	public IConfigurationProvider Build(IConfigurationBuilder builder)
	{
		return new CompositeKeyVaultProvider(keyVaultUris);
	}
}
