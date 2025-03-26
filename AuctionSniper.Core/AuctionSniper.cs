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
            switch (priceSource)
            {
                case AuctionEventEnums.PriceSource.FromSniper:
                    sniperListener.SniperWinning();
                    break;
                case AuctionEventEnums.PriceSource.FromOtherBidder:
                    auction.Bid(price + increment);
                    sniperListener.SniperBidding();
                    break;
            }
        }
    }
}
