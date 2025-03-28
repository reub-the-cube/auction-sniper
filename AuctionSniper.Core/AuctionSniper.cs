using AuctionSniper.XMPP;

namespace AuctionSniper.Core
{
    public class AuctionSniper(Auction auction, ISniperListener sniperListener, string itemId) : IAuctionEventListener
    {
        private SniperSnapshot snapshot = new(itemId, 0, 0, SniperState.Joining);

        public void AuctionClosed()
        {
            snapshot = snapshot.AuctionClosed();
            NotifyListenerOfChange();
        }

        public void CurrentPrice(int price, int increment, AuctionEventEnums.PriceSource priceSource)
        {
            var isWinning = priceSource == AuctionEventEnums.PriceSource.FromSniper;

            if (isWinning)
            {
                snapshot = snapshot.Winning(price);
            }
            else
            {
                int bid = price + increment;
                auction.Bid(bid);
                snapshot = snapshot.Bidding(price, bid);
            }

            NotifyListenerOfChange();
        }

        private void NotifyListenerOfChange()
        {
            sniperListener.SniperSnapshotChanged(snapshot);
        }
    }
}
