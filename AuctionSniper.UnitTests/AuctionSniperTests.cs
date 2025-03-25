using Moq;

namespace AuctionSniper.UnitTests
{
    public class AuctionSniperTests
    {
        [Fact]
        public void ReportsLostWhenAuctionCloses()
        {
            var sniperListener = new Mock<Core.SniperListener>();
            var auctionSniper = new Core.AuctionSniper(sniperListener.Object);

            auctionSniper.AuctionClosed();

            sniperListener.Verify(v => v.SniperLost(), Times.Once());
        }
    }
}
