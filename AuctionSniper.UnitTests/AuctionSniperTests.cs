using AuctionSniper.Core;
using Moq;

namespace AuctionSniper.UnitTests
{
    public class AuctionSniperTests
    {
        private readonly Mock<Auction> auction = new();
        private Mock<SniperListener> sniperListener = new();
        private readonly Core.AuctionSniper auctionSniper;

        public AuctionSniperTests()
        {
            auctionSniper = new Core.AuctionSniper(auction.Object, sniperListener.Object);
        }

        [Fact]
        public void ReportsLostWhenAuctionClosesImmediately()
        {
            auctionSniper.AuctionClosed();

            sniperListener.Verify(v => v.SniperLost(), Times.Once());
        }

        [Fact]
        public void ReportsLostIfAuctionClosesWhenBidding()
        {
            var sequence = new MockSequence();
            sniperListener = new Mock<SniperListener>(MockBehavior.Strict);
            sniperListener.InSequence(sequence).Setup(s => s.SniperBidding());
            sniperListener.InSequence(sequence).Setup(s => s.SniperLost());

            auctionSniper.CurrentPrice(123, 45, XMPP.AuctionEventEnums.PriceSource.FromOtherBidder);
            auctionSniper.AuctionClosed();

            sniperListener.Verify();
        }

        [Fact]
        public void BidsHigherAndReportsBiddingWhenNewPriceArrives()
        {
            auctionSniper.CurrentPrice(1001, 25, XMPP.AuctionEventEnums.PriceSource.FromOtherBidder);

            sniperListener.Verify(v => v.SniperBidding(), Times.AtLeastOnce());
            auction.Verify(v => v.Bid(1001 + 25), Times.Once());
        }

        [Fact]
        public void ReportsWinningWhenCurrentPriceComesFromSniper()
        {
            auctionSniper.CurrentPrice(123, 45, XMPP.AuctionEventEnums.PriceSource.FromSniper);

            sniperListener.Verify(v => v.SniperWinning(), Times.AtLeastOnce());
        }
    }
}
