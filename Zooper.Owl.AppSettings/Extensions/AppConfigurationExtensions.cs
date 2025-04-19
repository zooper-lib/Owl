using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Zooper.Owl.AppSettings.Extensions;

public static class AppConfigurationExtensions
{
	private const string AppSettingsName = "appsettings";

	// ReSharper disable once CommentTypo
	/// <summary>
	///     Configures the app settings for the application.
	/// </summary>
	/// <param name="builder">The builder</param>
	/// <param name="appSettingsName">A custom name for the AppSettings. Leave null to use default "appsettings"</param>
	/// <returns></returns>
	public static IHostBuilder ConfigureAppSettings(
		this IHostBuilder builder,
		string? appSettingsName = null)
	{
		var appSettings = appSettingsName ?? AppSettingsName;

		return builder.ConfigureAppConfiguration(
			(
				_,
				configurationBuilder) =>
			{
				var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

				configurationBuilder.AddJsonFile(
						$"{appSettings}.json",
						true,
						true
					)
					.AddJsonFile(
						$"{appSettings}.{environment}.json",
						true
					);
			}
		);
	}
}
