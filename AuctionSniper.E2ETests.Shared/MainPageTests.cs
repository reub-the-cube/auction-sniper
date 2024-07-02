using FluentAssertions;
using OpenQA.Selenium.DevTools.V123.Storage;
using Xunit;

// You will have to make sure that all the namespaces match
// between the different platform specific projects and the shared
// code files. This has to do with how we initialize the AppiumDriver
// through the AppiumSetup.cs files.
namespace E2ETests;

// This is an example of tests that do not need anything platform specific
public class MainPageTests : BaseTest
{
	private FakeAuctionServer auction = new FakeAuctionServer("item-54321");

    [Fact]
	public void AppLaunches()
	{
		App.GetScreenshot().SaveAsFile($"{nameof(AppLaunches)}.png");
	}

	[Fact]
	public async Task SniperJoinsAuctionUntilAuctionCloses()
	{
		auction.StartSellingItem();
		RequestToJoin(auction);

		auction.HasReceivedRequestToJoinFromSniper();
		auction.AnnounceClosed();

		GetSniperBiddingStatus().Should().Be("Lost");
	}

	private void RequestToJoin(FakeAuctionServer auction)
	{
        var element = FindUIElement("JoinAuction");
		element.Click();
    }

	private string GetSniperBiddingStatus()
	{
		var element = FindUIElement("SniperStatus");
		return element.Text;
	}
}