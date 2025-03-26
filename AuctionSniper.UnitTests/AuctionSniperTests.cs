using AuctionSniper.Core;
using Moq;

namespace AuctionSniper.UnitTests
{
    public class AuctionSniperTests
    {
        private readonly Mock<Auction> auction = new();
        private readonly Mock<SniperListener> sniperListener = new(MockBehavior.Strict);
        private readonly Core.AuctionSniper auctionSniper;

        public AuctionSniperTests()
        {
            auctionSniper = new Core.AuctionSniper(auction.Object, sniperListener.Object);
        }

        [Fact]
        public void ReportsLostWhenAuctionClosesImmediately()
        {
            sniperListener.Setup(s => s.SniperLost());

            auctionSniper.AuctionClosed();

            sniperListener.Verify(v => v.SniperLost(), Times.Once());
        }

        [Fact]
        public void ReportsLostIfAuctionClosesWhenBidding()
        {
            var sequence = new MockSequence();
            sniperListener.InSequence(sequence).Setup(s => s.SniperBidding());
            sniperListener.InSequence(sequence).Setup(s => s.SniperLost());

            auctionSniper.CurrentPrice(123, 45, XMPP.AuctionEventEnums.PriceSource.FromOtherBidder);
            auctionSniper.AuctionClosed();

            sniperListener.Verify();
        }

        [Fact]
        public void ReportsWonIfAuctionClosesWhenWinning()
        {
            var sequence = new MockSequence();
            sniperListener.InSequence(sequence).Setup(s => s.SniperWinning());
            sniperListener.InSequence(sequence).Setup(s => s.SniperWon());

            auctionSniper.CurrentPrice(123, 45, XMPP.AuctionEventEnums.PriceSource.FromSniper);
            auctionSniper.AuctionClosed();

            sniperListener.Verify(v => v.SniperWon(), Times.AtLeastOnce());
        }

        [Fact]
        public void BidsHigherAndReportsBiddingWhenNewPriceArrives()
        {
            sniperListener.Setup(s => s.SniperBidding());

            auctionSniper.CurrentPrice(1001, 25, XMPP.AuctionEventEnums.PriceSource.FromOtherBidder);

            sniperListener.Verify(v => v.SniperBidding(), Times.AtLeastOnce());
            auction.Verify(v => v.Bid(1001 + 25), Times.Once());
        }

        [Fact]
        public void ReportsWinningWhenCurrentPriceComesFromSniper()
        {
            sniperListener.Setup(s => s.SniperWinning());

            auctionSniper.CurrentPrice(123, 45, XMPP.AuctionEventEnums.PriceSource.FromSniper);

            sniperListener.Verify(v => v.SniperWinning(), Times.AtLeastOnce());
        }
    }
}
