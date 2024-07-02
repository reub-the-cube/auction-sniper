using Xunit;

namespace E2ETests
{
    [CollectionDefinition("Windows test collection")]
    public class WindowsCollectionDefinition : ICollectionFixture<PlatformTestFixture>
    {
    }
}
