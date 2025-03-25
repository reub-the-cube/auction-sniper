using AuctionSniper.Core;
using Moq;

namespace AuctionSniper.UnitTests
{
    public class AuctionSniperTests
    {
        [Fact]
        public void ReportsLostWhenAuctionCloses()
        {
            var sniperListener = new Mock<Core.SniperListener>();
            var auctionSniper = new Core.AuctionSniper(null, sniperListener.Object);

            auctionSniper.AuctionClosed();

            sniperListener.Verify(v => v.SniperLost(), Times.Once());
        }

        [Fact]
        public void BidsHigherAndReportsBiddingWhenNewPriceArrives()
        {
            var sniperListener = new Mock<SniperListener>();
            var auction = new Mock<Auction>();
            var auctionSniper = new Core.AuctionSniper(auction.Object, sniperListener.Object);

            auctionSniper.CurrentPrice(1001, 25);

            sniperListener.Verify(v => v.SniperBidding(), Times.AtLeastOnce());
            auction.Verify(v => v.Bid(1001 + 25), Times.Once());
        }
    }
}
