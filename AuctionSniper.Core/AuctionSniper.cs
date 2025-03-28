using AuctionSniper.XMPP;

namespace AuctionSniper.Core
{
    public class AuctionSniper(Auction auction, ISniperListener sniperListener, string itemId) : IAuctionEventListener
    {
        private bool isWinning = false;
        private SniperSnapshot snapshot = new(itemId, 0, 0, SniperState.Joining);

        public void AuctionClosed()
        {
            if (isWinning)
            {
                snapshot = new(snapshot.ItemId, snapshot.Price, snapshot.Bid, SniperState.Won);
                sniperListener.SniperSnapshotChanged(snapshot);
            }
            else
            {
                snapshot = new(snapshot.ItemId, snapshot.Price, snapshot.Bid, SniperState.Lost);
                sniperListener.SniperSnapshotChanged(snapshot);
            }
        }

        public void CurrentPrice(int price, int increment, AuctionEventEnums.PriceSource priceSource)
        {
            isWinning = priceSource == AuctionEventEnums.PriceSource.FromSniper;

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

            sniperListener.SniperSnapshotChanged(snapshot);
        }
    }
}
