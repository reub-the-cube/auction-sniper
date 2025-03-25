using AuctionSniper.Core;
using Moq;

namespace AuctionSniper.UnitTests
{
    public class AuctionSniperTests
    {
        private readonly Mock<Auction> auction = new();
        private readonly Mock<SniperListener> sniperListener = new();
        private readonly Core.AuctionSniper auctionSniper;

        public AuctionSniperTests()
        {
            auctionSniper = new Core.AuctionSniper(auction.Object, sniperListener.Object);
        }

        [Fact]
        public void ReportsLostWhenAuctionCloses()
        {
            auctionSniper.AuctionClosed();

            sniperListener.Verify(v => v.SniperLost(), Times.Once());
        }

        [Fact]
        public void BidsHigherAndReportsBiddingWhenNewPriceArrives()
        {
            auctionSniper.CurrentPrice(1001, 25);

            sniperListener.Verify(v => v.SniperBidding(), Times.AtLeastOnce());
            auction.Verify(v => v.Bid(1001 + 25), Times.Once());
        }
    }
}
