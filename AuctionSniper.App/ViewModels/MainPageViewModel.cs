using AuctionSniper.Core;

namespace AuctionSniper.App.ViewModels
{
    public class MainPageViewModel : ViewModelBase, SniperListener
    {
        private string? sniperBidStatus;
        public string? SniperBidStatus
        {
            get { return sniperBidStatus; }
            set
            {
                if (sniperBidStatus != value)
                {
                    sniperBidStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public void SniperBidding()
        {
            SniperBidStatus = "Bidding";
        }

        public void SniperLost()
        {
            SniperBidStatus = "Lost";
        }

        public void SniperWinning()
        {
            SniperBidStatus = "Winning";
        }
    }
}
