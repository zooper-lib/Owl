using System.Collections.Generic;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Zooper.Owl.AzureKeyVault.Extensions;

/// <summary>
/// Provides extension methods for configuring Azure Key Vault integration in .NET applications.
/// </summary>
[UsedImplicitly]
public static class AzureKeyVaultExtensions
{
	/// <summary>
	/// Configures Azure Key Vault integration using provided credentials.
	/// </summary>
	/// <param name="builder">The host builder to configure.</param>
	/// <param name="tenantId">The Azure AD tenant ID.</param>
	/// <param name="url">The Azure Key Vault URL.</param>
	/// <param name="clientId">The Azure AD client ID.</param>
	/// <param name="clientSecret">The Azure AD client secret.</param>
	/// <returns>The configured host builder.</returns>
	public static IHostBuilder AddAzureKeyVault(
		this IHostBuilder builder,
		string tenantId,
		string url,
		string clientId,
		string clientSecret)
	{
		return builder.ConfigureAppConfiguration(configurationBuilder =>
			{
				configurationBuilder.AddAzureKeyVault(
					tenantId,
					url,
					clientId,
					clientSecret
				);
			}
		);
	}

	/// <summary>
	/// Adds Azure Key Vault configuration to the configuration builder using client credentials.
	/// </summary>
	/// <param name="builder">The configuration builder.</param>
	/// <param name="tenantId">The Azure AD tenant ID.</param>
	/// <param name="url">The Azure Key Vault URL.</param>
	/// <param name="clientId">The Azure AD client ID.</param>
	/// <param name="clientSecret">The Azure AD client secret.</param>
	/// <returns>The configured configuration builder.</returns>
	/// <remarks>
	/// This method creates a SecretClient using the provided credentials and configures it to replace "--" with ":" in secret names.
	/// </remarks>
	public static IConfigurationBuilder AddAzureKeyVault(
		this IConfigurationBuilder builder,
		string tenantId,
		string url,
		string clientId,
		string clientSecret)
	{
		var credentials = new ClientSecretCredential(
			tenantId,
			clientId,
			clientSecret
		);
		var client = new SecretClient(
			new(url),
			credentials
		);

		// * The KeyVaultSecretManager is used to replace "--" with ":" from the secret name.
		builder.AddAzureKeyVault(
			client,
			new KeyVaultSecretManager()
		);

		return builder;
	}

	/// <summary>
	/// Adds configuration support for multiple Azure Key Vaults.
	/// </summary>
	/// <param name="builder">The configuration builder.</param>
	/// <param name="keyVaultUris">A collection of Azure Key Vault URIs to search for secrets.</param>
	/// <returns>The configured configuration builder.</returns>
	/// <remarks>
	/// This method enables fallback behavior where secrets are searched across multiple vaults in sequence.
	/// </remarks>
	public static IConfigurationBuilder AddCompositeKeyVault(
		this IConfigurationBuilder builder,
		IEnumerable<string> keyVaultUris)
	{
		return builder.Add(new CompositeKeyVaultSource(keyVaultUris));
	}
}