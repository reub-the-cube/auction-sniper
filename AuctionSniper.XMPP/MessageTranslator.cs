using XmppDotNet.Xmpp.Client;

namespace AuctionSniper.XMPP
{
    public class MessageTranslator(IAuctionEventListener auctionEventListener)
    {
        private readonly IAuctionEventListener _auctionEventListener = auctionEventListener;

        public void ProcessMessage(Message message)
        {
            _auctionEventListener.AuctionClosed();
        }
    }
}
