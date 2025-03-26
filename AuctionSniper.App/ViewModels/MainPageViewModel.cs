using AuctionSniper.App.Models;
using System.Collections.ObjectModel;

namespace AuctionSniper.App.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public ObservableCollection<Sniper> Snipers { get; private set; }

        public MainPageViewModel()
        {
            var source = new List<Sniper>
            {
                new() { BidStatus = "Unjoined"}
            };
            Snipers = [.. source];
        }
    }
}
