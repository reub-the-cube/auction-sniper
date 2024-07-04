using XmppDotNet.Xmpp.Client;

namespace AuctionSniper.XMPP
{
    public class MessageListener
    {
        public event EventHandler CloseMessageReceived;

        private List<Message> joinMessages = [];

        public MessageListener()
        {
            CloseMessageReceived = delegate { };
        }

        public void ProcessMessage(object? sender, Message message)
        {
            if (message.Body.Equals(SouthabeeStandards.JOIN_REQUEST))
            {
                joinMessages.Add(message);
            }
            else if (message.Body.Equals(SouthabeeStandards.CLOSE_REQUEST))
            {
                CloseMessageReceived?.Invoke(sender, EventArgs.Empty);
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
