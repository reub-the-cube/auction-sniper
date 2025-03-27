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

        public void SniperSnapshotChanged(SniperSnapshot sniperSnapshot)
        {
            AuctionId = sniperSnapshot.ItemId;
            BidStatus = sniperSnapshot.State.ToString();
            CurrentPrice = sniperSnapshot.Price;
            LastBid = sniperSnapshot.Bid;
        }

        public void SniperLost()
        {
            BidStatus = "Lost";
        }

        public void SniperWon()
        {
            BidStatus = "Won";
        }
    }
}
