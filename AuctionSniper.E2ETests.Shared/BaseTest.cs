using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using Xunit;

namespace E2ETests;

[Collection(PlatformTestFixture.TestCollectionName)]
public abstract class BaseTest : IDisposable
{
	protected AppiumDriver App => PlatformTestFixture.App;

	public void Dispose()
	{
		PlatformTestFixture.RelaunchApp();
	}

	// This could also be an extension method to AppiumDriver if you prefer
	protected AppiumElement FindUIElement(string id)
	{
		if (App is WindowsDriver)
		{
			return App.FindElement(MobileBy.AccessibilityId(id));
		}

		return App.FindElement(MobileBy.Id(id));
	}
}