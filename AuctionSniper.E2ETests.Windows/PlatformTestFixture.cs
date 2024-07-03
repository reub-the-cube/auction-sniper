using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

namespace E2ETests;

public class PlatformTestFixture : BaseFixture, IDisposable
{
	private static AppiumDriver? driver;

	public static AppiumDriver App => driver ?? throw new NullReferenceException("AppiumDriver is null");
    public const string TestCollectionName = "Windows test collection";
    public const int DefaultDelay = 500;

    public PlatformTestFixture() : base()
    {
        // If you started an Appium server manually, make sure to comment out the next line
        // This line starts a local Appium server for you as part of the test run
        AppiumServerHelper.StartAppiumLocalServer();

        var windowsOptions = new AppiumOptions
		{
			// Specify windows as the driver, typically don't need to change this
			AutomationName = "windows",
			// Always Windows for Windows
			PlatformName = "Windows",
            // The identifier of the deployed application to test
			App = "com.madetechbookclub.auctionsniper_gfz32p546wxaw!App"
        };

		// Note there are many more options that you can use to influence the app under test according to your needs

		driver = new WindowsDriver(windowsOptions);
    }

    public void Dispose()
    {
        driver?.Quit();

        // If an Appium server was started locally above, make sure we clean it up here
        AppiumServerHelper.DisposeAppiumLocalServer();
    }
}