using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Zooper.Owl.AppSettings.Extensions;

public static class AppConfigurationExtensions
{
	private const string AppSettingsName = "appsettings";

	/// <summary>
	/// Configures the app settings for the application.
	/// </summary>
	/// <param name="builder">The builder</param>
	/// <param name="appSettingsName">A custom name for the AppSettings. Don't set to use default "appsettings"</param>
	/// <returns></returns>
	public static IHostBuilder ConfigureAppSettings(
		this IHostBuilder builder,
		string appSettingsName = AppSettingsName)
	{
		return builder.ConfigureAppConfiguration(
			(
				_,
				configurationBuilder) =>
			{
				var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

				configurationBuilder.AddJsonFile(
						$"{appSettingsName}.json",
						true,
						true
					)
					.AddJsonFile(
						$"{appSettingsName}.{environment}.json",
						true
					);
			}
		);
	}
}
