using AuctionSniper.XMPP;
using Moq;
using XmppDotNet.Xmpp.Client;
using static AuctionSniper.XMPP.AuctionEventEnums;

namespace AuctionSniper.UnitTests
{
    public class SniperTranslatorTests
    {
        private const string SNIPER_ID = "test_sniper";

        [Fact]
        public void NotifiesAuctionClosedWhenCloseMessageReceived()
        {
            var message = new Message(string.Empty, SouthabeeStandards.CLOSE_REQUEST);
            var auctionEventListener = new Mock<IAuctionEventListener>();
            var translator = new SniperTranslator(auctionEventListener.Object, SNIPER_ID);

            translator.ProcessMessage(message);

            auctionEventListener.Verify(v => v.AuctionClosed(), Times.Once());
        }

        [Fact]
        public void NotifiesBidDetailsWhenCurrentPriceMessageReceivedFromOtherBidder()
        {
            var message = new Message(string.Empty, string.Format(SouthabeeStandards.REPORT_PRICE_EVENT, 192, 7, "Someone else"));
            var auctionEventListener = new Mock<IAuctionEventListener>();
            var translator = new SniperTranslator(auctionEventListener.Object, SNIPER_ID);

            translator.ProcessMessage(message);

            auctionEventListener.Verify(v => v.CurrentPrice(192, 7, PriceSource.FromOtherBidder), Times.Once());
        }

        [Fact]
        public void NotifiesBidDetailsWhenCurrentPriceMessageReceivedFromSniper()
        {
            var message = new Message(string.Empty, string.Format(SouthabeeStandards.REPORT_PRICE_EVENT, 234, 5, SNIPER_ID));
            var auctionEventListener = new Mock<IAuctionEventListener>();
            var translator = new SniperTranslator(auctionEventListener.Object, SNIPER_ID);

            translator.ProcessMessage(message);

            auctionEventListener.Verify(v => v.CurrentPrice(234, 5, PriceSource.FromSniper), Times.Once());
        }
    }
}
