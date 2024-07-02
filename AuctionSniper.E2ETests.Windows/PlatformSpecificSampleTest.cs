using NUnit.Framework;

namespace E2ETests;

public class PlatformSpecificSampleTest : BaseTest
{
	[Test]
	public void SampleTest()
	{
		App.GetScreenshot().SaveAsFile($"{nameof(SampleTest)}.png");
	}
}