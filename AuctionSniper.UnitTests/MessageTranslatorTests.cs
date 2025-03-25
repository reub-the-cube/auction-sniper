using AuctionSniper.XMPP;
using Moq;
using Shouldly;
using XmppDotNet.Xmpp.Client;

namespace AuctionSniper.UnitTests
{
    public class MessageTranslatorTests
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
    }
}
