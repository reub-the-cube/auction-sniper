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
            sniperListener.Setup(s => s.SniperSnapshotChanged(new(ITEM_ID, 0, 0, SniperState.Lost)));

            auctionSniper.AuctionClosed();

            sniperListener.Verify(v => v.SniperSnapshotChanged(It.IsAny<SniperSnapshot>()), Times.Once());
        }

        [Fact]
        public void ReportsLostIfAuctionClosesWhenBidding()
        {
            var sequence = new MockSequence();
            sniperListener.InSequence(sequence).Setup(s => s.SniperSnapshotChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Bidding)));
            sniperListener.InSequence(sequence).Setup(s => s.SniperSnapshotChanged(new(ITEM_ID, 123, 168, SniperState.Lost)));

            auctionSniper.CurrentPrice(123, 45, XMPP.AuctionEventEnums.PriceSource.FromOtherBidder);
            auctionSniper.AuctionClosed();

            sniperListener.Verify(v => v.SniperSnapshotChanged(It.IsAny<SniperSnapshot>()), Times.AtLeast(2));
        }

        [Fact]
        public void ReportsWonIfAuctionClosesWhenWinning()
        {
            var sequence = new MockSequence();
            sniperListener.InSequence(sequence).Setup(s => s.SniperSnapshotChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Bidding)));
            sniperListener.InSequence(sequence).Setup(s => s.SniperSnapshotChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Winning)));
            sniperListener.InSequence(sequence).Setup(s => s.SniperSnapshotChanged(new(ITEM_ID, 123, 123, SniperState.Won)));

            auctionSniper.CurrentPrice(111, 12, XMPP.AuctionEventEnums.PriceSource.FromOtherBidder);
            auctionSniper.CurrentPrice(123, 45, XMPP.AuctionEventEnums.PriceSource.FromSniper);
            auctionSniper.AuctionClosed();

            sniperListener.Verify(v => v.SniperSnapshotChanged(It.IsAny<SniperSnapshot>()), Times.AtLeast(3));
        }

        [Fact]
        public void BidsHigherAndReportsBiddingWhenNewPriceArrives()
        {
            int price = 1001;
            int increment = 25;
            int bid = price + increment;
            sniperListener.Setup(s => s.SniperSnapshotChanged(new SniperSnapshot(ITEM_ID, price, bid, SniperState.Bidding)));

            auctionSniper.CurrentPrice(price, increment, XMPP.AuctionEventEnums.PriceSource.FromOtherBidder);

            sniperListener.Verify(v => v.SniperSnapshotChanged(It.IsAny<SniperSnapshot>()), Times.AtLeastOnce());
            auction.Verify(v => v.Bid(bid), Times.Once());
        }

        [Fact]
        public void ReportsWinningWhenCurrentPriceComesFromSniper()
        {
            var sequence = new MockSequence();
            sniperListener.InSequence(sequence).Setup(s => s.SniperSnapshotChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Bidding)));
            sniperListener.InSequence(sequence).Setup(s => s.SniperSnapshotChanged(new(ITEM_ID, 123, 123, SniperState.Winning)));

            auctionSniper.CurrentPrice(111, 12, XMPP.AuctionEventEnums.PriceSource.FromOtherBidder);
            auctionSniper.CurrentPrice(123, 45, XMPP.AuctionEventEnums.PriceSource.FromSniper);

            sniperListener.Verify(v => v.SniperSnapshotChanged(It.IsAny<SniperSnapshot>()), Times.AtLeast(2));
        }
    }
}
