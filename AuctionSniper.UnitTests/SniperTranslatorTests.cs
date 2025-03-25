using AuctionSniper.XMPP;
using Moq;
using XmppDotNet.Xmpp.Client;

namespace AuctionSniper.UnitTests
{
    public class SniperTranslatorTests
    {
        [Fact]
        public void NotifiesAuctionClosedWhenCloseMessageReceived()
        {
            var message = new Message(string.Empty, SouthabeeStandards.CLOSE_REQUEST);
            var auctionEventListener = new Mock<IAuctionEventListener>();
            var translator = new SniperTranslator(auctionEventListener.Object);

            translator.ProcessMessage(message);

            auctionEventListener.Verify(v => v.AuctionClosed(), Times.Once());
        }

        [Fact]
        public void NotifiesBidDetailsWhenCurrentPriceMessageReceived()
        {
            var message = new Message(string.Empty, string.Format(SouthabeeStandards.REPORT_PRICE_EVENT, 192, 7, "Someone else"));
            var auctionEventListener = new Mock<IAuctionEventListener>();
            var translator = new SniperTranslator(auctionEventListener.Object);

            translator.ProcessMessage(message);

            auctionEventListener.Verify(v => v.CurrentPrice(192, 7), Times.Once());
        }
    }
}
