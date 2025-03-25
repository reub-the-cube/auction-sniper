namespace AuctionSniper.Core
{
    public interface Auction
    {
        Task Bid(int amount);
    }
}
