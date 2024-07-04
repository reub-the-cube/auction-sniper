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
	private static readonly string AUCTION_ID = "item-54321";

    private readonly FakeAuctionServer auction = new(AUCTION_ID);

    [Fact]
	public void AppLaunches()
	{
		App.GetScreenshot().SaveAsFile($"{nameof(AppLaunches)}.png");
	}

	[Fact]
	public async Task SniperJoinsAuctionUntilAuctionCloses()
	{
		// Act
		await auction.StartSellingItem();

		// Act
		await StartBiddingIn(auction);

		// Assert
		SniperBiddingStatus().Should().Be("Joining");
		auction.HasBeenJoined().Should().Be(true);

		// Act
		await auction.AnnounceClosed();

		// Assert
		await Task.Delay(PlatformTestFixture.DefaultDelay);
		SniperBiddingStatus().Should().Be("Lost");
	}

	private async Task StartBiddingIn(FakeAuctionServer auction)
	{
		var auctionIdText = FindUIElement("ItemId");
		auctionIdText.SendKeys(auction.ItemId);

        var joinAuctionButton = FindUIElement("JoinAuction");
		joinAuctionButton.Click();

        await Task.Delay(PlatformTestFixture.DefaultDelay);
    }

    private string SniperBiddingStatus()
	{
		var element = FindUIElement("SniperBidStatus");
		return element.Text;
	}
}