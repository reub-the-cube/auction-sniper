using AuctionSniper.XMPP;

namespace AuctionSniper.Core
{
    public class AuctionSniper(SniperListener sniperListener): IAuctionEventListener
    {
        public void AuctionClosed()
        {
            sniperListener.SniperLost();
        }

        public void CurrentPrice(int price, int increment)
        {
            throw new NotImplementedException();
        }
    }
}
