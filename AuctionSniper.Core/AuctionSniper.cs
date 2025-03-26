using AuctionSniper.XMPP;

namespace AuctionSniper.Core
{
    public class AuctionSniper(Auction auction, SniperListener sniperListener) : IAuctionEventListener
    {
        public void AuctionClosed()
        {
            sniperListener.SniperLost();
        }

        public void CurrentPrice(int price, int increment, AuctionEventEnums.PriceSource priceSource)
        {
            auction.Bid(price + increment);
            sniperListener.SniperBidding();
        }
    }
}
