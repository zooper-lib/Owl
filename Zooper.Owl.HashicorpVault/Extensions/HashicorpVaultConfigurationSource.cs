using Microsoft.Extensions.Configuration;

namespace Zooper.Owl.HashicorpVault.Extensions;

/// <summary>
/// Represents a source of configuration values from a Hashicorp Vault instance.
/// </summary>
/// <remarks>
/// This class implements IConfigurationSource to provide a way to build a HashicorpVaultConfigurationProvider
/// that can retrieve secrets from a Hashicorp Vault instance.
/// </remarks>
public class HashicorpVaultConfigurationSource(
	string vaultUrl,
	string token,
	string mountPoint) : IConfigurationSource
{
	/// <summary>
	/// Gets the URL of the Hashicorp Vault instance.
	/// </summary>
	public string VaultUrl { get; private set; } = vaultUrl;

	/// <summary>
	/// Gets the authentication token used to access the Hashicorp Vault.
	/// </summary>
	public string Token { get; private set; } = token;

	/// <summary>
	/// Gets the mount point where secrets are stored in the Hashicorp Vault.
	/// </summary>
	public string MountPoint { get; private set; } = mountPoint;

	/// <summary>
	/// Builds the configuration provider for the source.
	/// </summary>
	/// <param name="builder">The configuration builder.</param>
	/// <returns>A new instance of <see cref="HashicorpVaultConfigurationProvider"/> configured with the provided vault settings.</returns>
	public IConfigurationProvider Build(IConfigurationBuilder builder)
	{
		return new HashicorpVaultConfigurationProvider(this);
	}
}
