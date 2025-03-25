using AuctionSniper.XMPP;
using XmppDotNet;

namespace AuctionSniper.App
{
    public class Auction(Client xmppClient, string itemId) : Core.Auction
    {
        public async Task Bid(int amount) => await SendMessageAsync(string.Format(SouthabeeStandards.BID_REQUEST, amount));

        public async Task Join() => await SendMessageAsync(SouthabeeStandards.JOIN_REQUEST);

        private async Task SendMessageAsync(string message)
        {
            Jid auctionUser = xmppClient.CreateJidFromLocalUsername($"auction-{itemId}");
            await xmppClient.SendMessageAsync(auctionUser, message);
        }
    }
}
