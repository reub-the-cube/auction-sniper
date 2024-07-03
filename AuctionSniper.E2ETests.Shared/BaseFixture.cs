using Microsoft.Extensions.Configuration;

namespace E2ETests
{
    public class BaseFixture
    {
        private static IConfiguration? configuration;
        public static IConfiguration Configuration => configuration ?? throw new NullReferenceException("Configuration is null");

        public BaseFixture()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testsettings.json", false)
                .AddJsonFile("testsettings.dev.json", true)
                .Build();
        }
    }
}
