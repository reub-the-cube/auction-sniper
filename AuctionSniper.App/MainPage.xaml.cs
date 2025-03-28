using AuctionSniper.App.ViewModels;

namespace AuctionSniper.App
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel mainPageViewModel)
        {
            InitializeComponent();

            BindingContext = mainPageViewModel;
        }

        private async void OnJoinClicked(object sender, EventArgs e)
        {
            await ((MainPageViewModel)BindingContext).Snipers.JoinAnAuction(ItemId.Text);
        }
    }
}