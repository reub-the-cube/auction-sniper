using System.Linq;
using XmppDotNet.Xmpp.Client;

namespace AuctionSniper.XMPP
{
    public class SniperTranslator(IAuctionEventListener auctionEventListener, string sniperId) : IMessageTranslator
    {
        private readonly IAuctionEventListener _auctionEventListener = auctionEventListener;

        public void ProcessMessage(Message message)
        {
            var elements = GetKeyValuePairs(message);

            elements.TryGetValue("Event", out var messageType);

            switch (messageType)
            {
                case "CLOSE":
                    _auctionEventListener.AuctionClosed();
                    break;
                case "PRICE":
                    int currentPrice = int.Parse(elements["CurrentPrice"]);
                    int increment = int.Parse(elements["Increment"]);
                    _auctionEventListener.CurrentPrice(currentPrice, increment, AuctionEventEnums.PriceSource.FromOtherBidder);
                    break;
            }
        }

        private static Dictionary<string, string> GetKeyValuePairs(Message message)
        {
            return message.Body
                .Split(";", StringSplitOptions.RemoveEmptyEntries)
                .Select(e =>
                    {
                        var splitElement = e.Split(":", StringSplitOptions.TrimEntries);
                        return new KeyValuePair<string, string>(splitElement[0], splitElement[1]);
                    })
                .ToDictionary(k => k.Key, v => v.Value);
        }
    }
}
