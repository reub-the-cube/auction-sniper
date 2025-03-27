namespace AuctionSniper.Core
{
    public interface ISniperListener
    {
        void SniperSnapshotChanged(SniperSnapshot sniperSnapshot);

        void SniperLost();

        void SniperWon();
    }
}
