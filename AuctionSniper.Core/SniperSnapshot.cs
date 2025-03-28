using System.Reflection.Metadata.Ecma335;

namespace AuctionSniper.Core
{
    public record SniperSnapshot(string ItemId, int Price, int Bid, SniperState State) 
    {
        public SniperSnapshot AuctionClosed() => new(ItemId, Price, Bid, GetAuctionClosedState());

        public SniperSnapshot Bidding(int newPrice, int newBid) => new(ItemId, newPrice, newBid, SniperState.Bidding);

        public SniperSnapshot Winning(int newPrice) => new(ItemId, newPrice, Bid, SniperState.Winning);

        private SniperState GetAuctionClosedState() => State == SniperState.Winning ? SniperState.Won : SniperState.Lost;
    }
}
