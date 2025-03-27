namespace AuctionSniper.Core
{
    public interface ISniperListener
    {
        void SniperBidding(SniperState sniperState);

        void SniperLost();

        void SniperWinning();

        void SniperWon();
    }
}
