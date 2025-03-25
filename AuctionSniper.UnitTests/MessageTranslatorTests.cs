using AuctionSniper.XMPP;
using Moq;
using Shouldly;
using XmppDotNet.Xmpp.Client;

namespace AuctionSniper.UnitTests
{
    public class MessageListenerTests
    {
        private bool closeEventRaised = false;

        [Fact]
        public void NotifiesAuctionClosedWhenCloseMessageReceived()
        {
            var message = new Message(string.Empty, SouthabeeStandards.CLOSE_REQUEST);
            var translator = new MessageListener();
            translator.CloseMessageReceived += CloseMessageReceived;

            translator.ProcessMessage(null, message);

            closeEventRaised.ShouldBe(true);
        }

        private void CloseMessageReceived(object? sender, EventArgs e)
        {
            closeEventRaised = true;
        }
    }

    public class MessageTranslatorTests
    {
        [Fact]
        public void NotifiesAuctionClosedWhenCloseMessageReceived()
        {
            var message = new Message(string.Empty, SouthabeeStandards.CLOSE_REQUEST);
            var auctionEventListener = new Mock<IAuctionEventListener>();
            var translator = new MessageTranslator(auctionEventListener.Object);

            translator.ProcessMessage(message);

            auctionEventListener.Verify(v => v.AuctionClosed(), Times.Once());
        }
    }
}
