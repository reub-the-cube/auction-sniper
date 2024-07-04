using XmppDotNet.Xmpp.Client;

namespace AuctionSniper.XMPP
{
    public class MessageListener
    {
        private List<Message> joinMessages = [];

        public void ProcessMessage(Message message)
        {
            if (message.Body.Equals(SouthabeeStandards.JOIN_REQUEST))
            {
                joinMessages.Add(message);
            }
        }

        public bool HasReceivedJoinMessage()
        {
            return joinMessages.Any();
        }

        public string SenderOfFirstJoinMessage()
        {
            return joinMessages.First().From.Local;
        }
    }
}
