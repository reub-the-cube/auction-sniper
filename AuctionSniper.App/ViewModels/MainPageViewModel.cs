using AuctionSniper.XMPP;

namespace AuctionSniper.App.ViewModels
{
    public class MainPageViewModel : ViewModelBase, IAuctionEventListener
    {
        string sniperBidStatus;
        public string SniperBidStatus
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

        public void AuctionClosed()
        {
            SniperBidStatus = "Lost";
        }
    }
}
