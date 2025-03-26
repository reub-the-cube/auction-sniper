using AuctionSniper.App.ViewModels;

namespace AuctionSniper.App.Models
{
    public class Sniper : ViewModelBase
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
    }
}
