namespace AuctionSniper.App.ViewModels
{
    public class MainPageViewModel : ViewModelBase
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
    }
}
