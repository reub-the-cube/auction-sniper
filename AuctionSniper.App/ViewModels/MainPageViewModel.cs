using AuctionSniper.App.Interfaces;

namespace AuctionSniper.App.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public IPortfolio Snipers { get; private set; }

        public MainPageViewModel(IPortfolio portfolio)
        {
            Snipers = portfolio;
        }
    }
}
