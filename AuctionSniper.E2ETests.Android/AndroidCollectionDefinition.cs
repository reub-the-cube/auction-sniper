using Xunit;

namespace E2ETests
{
    [CollectionDefinition("Android test collection")]
    public class AndroidCollectionDefinition : ICollectionFixture<PlatformTestFixture>
    {
    }
}
