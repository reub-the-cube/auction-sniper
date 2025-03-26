using static AuctionSniper.XMPP.AuctionEventEnums;

namespace AuctionSniper.XMPP
{
    public interface IAuctionEventListener
    {
        public void AuctionClosed();
        public void CurrentPrice(int price, int increment, PriceSource priceSource);
    }
}
