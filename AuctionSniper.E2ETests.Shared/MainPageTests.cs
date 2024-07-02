using FluentAssertions;
using Xunit;

// You will have to make sure that all the namespaces match
// between the different platform specific projects and the shared
// code files. This has to do with how we initialize the AppiumDriver
// through the AppiumSetup.cs files.
namespace E2ETests;

// This is an example of tests that do not need anything platform specific
public class MainPageTests : BaseTest
{
    [Fact]
	public void AppLaunches()
	{
		App.GetScreenshot().SaveAsFile($"{nameof(AppLaunches)}.png");
	}

	[Fact]
	public async Task ClickCounterTest()
	{
		// Arrange
		// Find elements with the value of the AutomationId property
		var element = FindUIElement("CounterBtn");

		// Act
		element.Click();
		await Task.Delay(500); // Wait for the click to register and show up on the screenshot

		// Assert
		App.GetScreenshot().SaveAsFile($"{nameof(ClickCounterTest)}.png");
		element.Text.Should().Be("Clicked 1 time");
	}
}