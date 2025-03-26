using AuctionSniper.App.ViewModels;
using AuctionSniper.Core;

namespace AuctionSniper.App.Models
{
    public class Sniper : ViewModelBase, SniperListener
    {
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

        public void SniperBidding()
        {
            BidStatus = "Bidding";
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
