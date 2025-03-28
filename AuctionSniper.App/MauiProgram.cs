using AuctionSniper.App.Interfaces;
using AuctionSniper.App.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace AuctionSniper.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            var appConfig = GetAppSettingsConfiguration("AuctionSniper.App.appsettings.json") ?? throw new FileNotFoundException("Could not load app settings.");
            builder.Configuration.AddConfiguration(appConfig);

            var devConfig = GetAppSettingsConfiguration("AuctionSniper.App.appsettings.dev.json");
            if (devConfig != null)
            {
                builder.Configuration.AddConfiguration(devConfig);
            }

            builder.Services
                .AddTransient<MainPage>()
                .AddTransient<MainPageViewModel>()
                .AddTransient<XMPP.Client>()
                .AddTransient<IPortfolio, Portfolio>();
                
            return builder.Build();
        }

        private static IConfigurationRoot? GetAppSettingsConfiguration(string resourceStreamName)
        {
            IConfigurationRoot? appConfig = null;
            var assembly = Assembly.GetExecutingAssembly();

            using var appSettings = assembly.GetManifestResourceStream(resourceStreamName);
            if (appSettings != null)
            {
                appConfig = new ConfigurationBuilder()
                            .AddJsonStream(appSettings)
                            .Build();
            }

            return appConfig;
        }
    }
}
