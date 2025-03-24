using XmppDotNet.Xmpp.Client;

namespace AuctionSniper.XMPP
{
    public class MessageListener
    {
        public event EventHandler CloseMessageReceived;

        private List<Message> bidMessages = [];
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
            else if (SouthabeeStandards.IsBidCommand(message.Body))
            {
                bidMessages.Add(message);
            }
        }

        public bool HasReceivedBidMessageFrom(string message, string from)
        {
            return bidMessages
                .Where(m => m.From.Local == from && m.Body == message)
                .Any();
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
