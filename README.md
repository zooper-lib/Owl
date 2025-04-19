# Zooper.Owl

<img src="icon.png" alt="Zooper.Owl Logo" width="120" align="right"/>

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Zooper.Owl is a modular .NET library for secure configuration management that provides seamless integration with Azure Key Vault, Hashicorp Vault, and enhanced application settings. It enables developers to easily retrieve and manage secrets across different platforms with a consistent API.

## Key Features

- **Azure Key Vault Integration**: Retrieve secrets from Azure Key Vault with automatic authentication
- **Hashicorp Vault Integration**: Connect to Hashicorp Vault with recursive secret loading support
- **Enhanced Application Settings**: Smart handling of environment-specific configurations
- **Composite Providers**: Access multiple secret stores with intelligent fallback mechanisms
- **Minimal Dependencies**: Lightweight implementation with focused dependencies
- **Easy Configuration**: Simple extension methods for registration in dependency injection
- **Fully Documented API**: Comprehensive XML documentation for all public members

## Installation

Choose the package that suits your needs:

```bash
# For Azure Key Vault integration
dotnet add package Zooper.Owl.AzureKeyVault

# For Hashicorp Vault integration
dotnet add package Zooper.Owl.HashicorpVault

# For enhanced application settings
dotnet add package Zooper.Owl.AppSettings
```

## Quick Start

### Azure Key Vault Integration

```csharp
// In Program.cs or Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    // Configure Azure Key Vault with settings from configuration
    builder.AddAzureKeyVault();

    // Or with explicit credentials
    builder.AddAzureKeyVault(
        tenantId: "your-tenant-id",
        url: "https://your-vault.vault.azure.net/",
        clientId: "your-client-id",
        clientSecret: "your-client-secret"
    );

    // For multiple vaults with fallback behavior
    builder.AddCompositeKeyVault(new[] {
        "https://primary-vault.vault.azure.net/",
        "https://backup-vault.vault.azure.net/"
    });
}

// Then use IConfiguration to access your secrets
public class MyService
{
    private readonly IConfiguration _configuration;

    public MyService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void DoSomething()
    {
        var mySecret = _configuration["SecretName"];
    }
}
```

### Hashicorp Vault Integration

```csharp
// In Program.cs or Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    // Configure Hashicorp Vault
    builder.AddHashicorpVault(
        uri: "https://vault.example.com:8200",
        vaultToken: "your-vault-token",
        mountPoint: "secret"
    );
}

// Then use IConfiguration to access your secrets
// Vault path structure like "secret/data/myapp/database/password"
// becomes "myapp:database:password" in configuration
```

### Enhanced Application Settings

```csharp
// In Program.cs or Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    // Configure with default appsettings.json and appsettings.{Environment}.json
    builder.ConfigureAppSettings();

    // Or with custom settings file name
    builder.ConfigureAppSettings("mysettings");
}
```

## Core Concepts

### Composite Key Vault Provider

Accesses multiple Azure Key Vaults with automatic fallback:

```csharp
// If a secret isn't found in the first vault, it tries the second, and so on
var vaultUris = new[] {
    "https://primary-vault.vault.azure.net/",
    "https://secondary-vault.vault.azure.net/"
};
var provider = new CompositeKeyVaultProvider(vaultUris);
```

### Recursive Hashicorp Vault Loading

Automatically traverses the Vault's structure, loading all secrets:

```csharp
// Loads all secrets from all paths and subpaths
// Transforms them into configuration values with appropriate key names
services.AddHashicorpVault(
    uri: "https://vault.example.com:8200",
    vaultToken: "your-token",
    mountPoint: "secret"
);
```

### Environment-Specific Configuration

Automatically loads the right configuration for your environment:

```csharp
// Loads appsettings.json, then overlays appsettings.Development.json
// if ASPNETCORE_ENVIRONMENT is Development
builder.ConfigureAppSettings();
```

## Best Practices

### Security Recommendations

1. **Use Managed Identities**: When possible, use Azure Managed Identities instead of client credentials
2. **Rotate Credentials**: Regularly rotate your vault access tokens and credentials
3. **Limit Scope**: Apply the principle of least privilege to vault access policies
4. **Environment Variables**: Consider using environment variables for sensitive connection details

### Configuration Organization

1. **Logical Grouping**: Organize secrets and configurations into logical groups
2. **Naming Conventions**: Use consistent naming patterns for configuration keys
3. **Fallback Strategy**: Set up a clear fallback strategy for configurations across environments

## Advanced Usage

### Combining Multiple Sources

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Set up multiple configuration sources in priority order
    builder.ConfigureAppSettings()
           .AddAzureKeyVault()
           .AddHashicorpVault(uri, token, mountPoint);
}
```

### Custom Authentication

```csharp
// For Azure Key Vault with custom credential provider
var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
{
    ExcludeEnvironmentCredential = true,
    ExcludeManagedIdentityCredential = false
});
// Use with Azure Key Vault client
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
