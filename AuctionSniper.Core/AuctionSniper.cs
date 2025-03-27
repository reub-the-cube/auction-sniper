using AuctionSniper.XMPP;

namespace AuctionSniper.Core
{
    public class AuctionSniper(Auction auction, ISniperListener sniperListener, string itemId) : IAuctionEventListener
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
                int bid = price + increment;
                auction.Bid(bid);
                sniperListener.SniperBidding(new SniperState(itemId, price, bid));
            }
        }
    }
}
