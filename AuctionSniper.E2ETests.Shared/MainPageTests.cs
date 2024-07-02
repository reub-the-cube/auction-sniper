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
	private static readonly string AUCTION_ID = "item-54321";

	private FakeAuctionServer auction = new FakeAuctionServer(AUCTION_ID);

    [Fact]
	public void AppLaunches()
	{
		App.GetScreenshot().SaveAsFile($"{nameof(AppLaunches)}.png");
	}

	[Fact]
	public async Task SniperJoinsAuctionUntilAuctionCloses()
	{
		auction.StartSellingItem();
		await StartBiddingIn(auction);
		SniperBiddingStatus().Should().Be("Joining");

		auction.HasReceivedRequestToJoinFromSniper();
		auction.AnnounceClosed();

		SniperBiddingStatus().Should().Be("Lost");
	}

	private async Task StartBiddingIn(FakeAuctionServer auction)
	{
		var auctionIdText = FindUIElement("AuctionId");
		auctionIdText.SendKeys(auction.AuctionId);

        var joinAuctionButton = FindUIElement("JoinAuction");
		joinAuctionButton.Click();

        await Task.Delay(250);
    }

    private string SniperBiddingStatus()
	{
		var element = FindUIElement("SniperStatus");
		return element.Text;
	}
}