using AuctionSniper.XMPP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace E2ETests
{
    public class BaseFixture
    {
        private static IConfiguration? configuration;
        private static IServiceProvider? serviceProvider;
        public static IConfiguration Configuration => configuration ?? throw new NullReferenceException("Configuration is null");
        public static IServiceProvider ServiceProvider => serviceProvider ?? throw new NullReferenceException("Service provider is null");

        public BaseFixture()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testsettings.json", false)
                .AddJsonFile("testsettings.dev.json", true)
                .Build();

            serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddDebug())
                .AddSingleton<Client>()
                .BuildServiceProvider();
        }
    }
}
