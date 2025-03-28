namespace AuctionSniper.Core
{
    public enum SniperState
    {
        Joining,
        Bidding,
        Winning,
        Lost,
        Won
    }

    public static class SniperStateExtensions
    {
        public static SniperState WhenAuctionClosed(this SniperState activeState)
        {
            return activeState == SniperState.Winning ? SniperState.Won : SniperState.Lost;
        }
    }
}
