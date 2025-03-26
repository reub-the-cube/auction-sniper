using AuctionSniper.App.Models;
using AuctionSniper.Core;
using System.Collections.ObjectModel;

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
                    Snipers[0].BidStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Sniper> Snipers { get; private set; }

        public MainPageViewModel()
        {
            var source = new List<Sniper>
            {
                new() { BidStatus = "Unjoined"}
            };
            Snipers = [.. source];
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

        public void SniperWon()
        {
            SniperBidStatus = "Won";
        }
    }
}
