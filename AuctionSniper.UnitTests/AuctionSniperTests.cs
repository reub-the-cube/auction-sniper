using AuctionSniper.Core;
using Moq;

namespace AuctionSniper.UnitTests
{
    public class AuctionSniperTests
    {
        private const string ITEM_ID = "TEST_ITEM_123";
        private readonly Mock<Auction> auction = new();
        private readonly Mock<ISniperListener> sniperListener = new(MockBehavior.Strict);
        private readonly Core.AuctionSniper auctionSniper;

        public AuctionSniperTests()
        {
            auctionSniper = new Core.AuctionSniper(auction.Object, sniperListener.Object, ITEM_ID);
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
            sniperListener.InSequence(sequence).Setup(s => s.SniperBidding(It.IsAny<SniperState>()));
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
            int price = 1001;
            int increment = 25;
            int bid = price + increment;
            sniperListener.Setup(s => s.SniperBidding(new SniperState(ITEM_ID, price, bid)));

            auctionSniper.CurrentPrice(price, increment, XMPP.AuctionEventEnums.PriceSource.FromOtherBidder);

            sniperListener.Verify(v => v.SniperBidding(It.IsAny<SniperState>()), Times.AtLeastOnce());
            auction.Verify(v => v.Bid(bid), Times.Once());
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
