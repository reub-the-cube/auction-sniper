namespace AuctionSniper.Core
{
    public interface SniperListener
    {
        void SniperBidding();

        void SniperLost();

        void SniperWinning();

        void SniperWon();
    }
}
