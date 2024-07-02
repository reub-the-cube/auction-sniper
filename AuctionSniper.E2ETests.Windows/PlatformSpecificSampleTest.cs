using Xunit;

namespace E2ETests;

public class PlatformSpecificSampleTest : BaseTest
{
	[Fact]
	public void SampleTest()
	{
		App.GetScreenshot().SaveAsFile($"{nameof(SampleTest)}.png");
	}
}