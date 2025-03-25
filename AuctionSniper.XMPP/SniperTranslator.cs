using XmppDotNet.Xmpp.Client;

namespace AuctionSniper.XMPP
{
    public class SniperTranslator(IAuctionEventListener auctionEventListener) : IMessageTranslator
    {
        private readonly IAuctionEventListener _auctionEventListener = auctionEventListener;

        public void ProcessMessage(Message message)
        {
            _auctionEventListener.AuctionClosed();
        }
    }
}
