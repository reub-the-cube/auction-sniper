using AuctionSniper.XMPP;
using Shouldly;
using XmppDotNet.Xmpp.Client;

namespace AuctionSniper.UnitTests
{
    public class MessageTranslatorTests
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
}
