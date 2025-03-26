using AuctionSniper.XMPP;

namespace AuctionSniper.Core
{
    public class AuctionSniper(Auction auction, SniperListener sniperListener) : IAuctionEventListener
    {
        private bool isWinning = false;

        public void AuctionClosed()
        {
            if (isWinning)
            {
                sniperListener.SniperWon();
            }
            else
            {
                sniperListener.SniperLost();
            }
        }

        public void CurrentPrice(int price, int increment, AuctionEventEnums.PriceSource priceSource)
        {
            isWinning = priceSource == AuctionEventEnums.PriceSource.FromSniper;

            if (isWinning)
            {
                sniperListener.SniperWinning();
            }
            else
            {
                auction.Bid(price + increment);
                sniperListener.SniperBidding();
            }
        }
    }
}
