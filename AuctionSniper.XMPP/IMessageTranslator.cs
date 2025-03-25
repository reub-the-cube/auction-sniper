using XmppDotNet.Xmpp.Client;

namespace AuctionSniper.XMPP
{
    public interface IMessageTranslator
    {
        void ProcessMessage(Message message);
    }
}
