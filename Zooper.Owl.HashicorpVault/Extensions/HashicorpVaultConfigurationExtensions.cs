using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

// ReSharper disable IdentifierTypo

namespace Zooper.Owl.HashicorpVault.Extensions;

/// <summary>
/// Provides extension methods for configuring Hashicorp Vault in ASP.NET Core applications.
/// </summary>
public static class HashicorpVaultConfigurationExtensions
{
	/// <summary>
	/// Adds Hashicorp Vault as a configuration source to the host builder.
	/// </summary>
	/// <param name="builder">The IHostBuilder to configure.</param>
	/// <param name="uri">The URI of the Hashicorp Vault server.</param>
	/// <param name="vaultToken">The authentication token for accessing the Vault.</param>
	/// <param name="mountPoint">The mount point in the Vault where secrets are stored.</param>
	/// <returns>The IHostBuilder for chaining.</returns>
	public static IHostBuilder AddHashicorpVault(
		this IHostBuilder builder,
		string uri,
		string vaultToken,
		string mountPoint)
	{
		return builder.ConfigureAppConfiguration(
			(_, config) =>
			{
				config.AddHashicorpVault(uri, vaultToken, mountPoint);
			}
		);
	}

	/// <summary>
	/// Adds Hashicorp Vault as a configuration source to the configuration builder.
	/// </summary>
	/// <param name="builder">The IConfigurationBuilder to configure.</param>
	/// <param name="uri">The URI of the Hashicorp Vault server.</param>
	/// <param name="vaultToken">The authentication token for accessing the Vault.</param>
	/// <param name="mountPoint">The mount point in the Vault where secrets are stored.</param>
	/// <returns>The IConfigurationBuilder for chaining.</returns>
	public static IConfigurationBuilder AddHashicorpVault(
		this IConfigurationBuilder builder,
		string uri,
		string vaultToken,
		string mountPoint)
	{
		builder.Add(
			new HashicorpVaultConfigurationSource(
				uri,
				vaultToken,
				mountPoint
			)
		);

		return builder;
	}
}
