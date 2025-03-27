using AuctionSniper.App.ViewModels;
using AuctionSniper.Core;

namespace AuctionSniper.App.Models
{
    public class Sniper : ViewModelBase, ISniperListener
    {
        private string? auctionId;
        public string? AuctionId
        {
            get { return auctionId; }
            set
            {
                if (auctionId != value)
                {
                    auctionId = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? bidStatus;
        public string? BidStatus {
            get { return bidStatus; }
            set
            {
                if (bidStatus != value)
                {
                    bidStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        private int? currentPrice;
        public int? CurrentPrice
        {
            get { return currentPrice; }
            set
            {
                if (currentPrice != value)
                {
                    currentPrice = value;
                    OnPropertyChanged();
                }
            }
        }

        private int? lastBid;
        public int? LastBid
        {
            get { return lastBid; }
            set
            {
                if (lastBid != value)
                {
                    lastBid = value;
                    OnPropertyChanged();
                }
            }
        }

        public void SniperBidding(SniperState sniperState)
        {
            AuctionId = sniperState.ItemId;
            BidStatus = "Bidding";
            CurrentPrice = sniperState.Price;
            LastBid = sniperState.Bid;
        }

        public void SniperLost()
        {
            BidStatus = "Lost";
        }

        public void SniperWinning()
        {
            BidStatus = "Winning";
        }

        public void SniperWon()
        {
            BidStatus = "Won";
        }
    }
}
