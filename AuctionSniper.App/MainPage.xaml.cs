using AuctionSniper.App.ViewModels;
using AuctionSniper.XMPP;
using Microsoft.Extensions.Configuration;
using XmppDotNet;

namespace AuctionSniper.App
{
    public partial class MainPage : ContentPage
    {
        private readonly IConfiguration configuration;
        private readonly MessageListener messageListener = new();
        private readonly Client xmppClient;

        public MainPage(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            this.configuration = configuration;
            xmppClient = serviceProvider.GetRequiredService<Client>();

            messageListener.CloseMessageReceived += MessageListener_CloseMessageReceived;
            xmppClient.ClientHasBinded += XmppClient_ClientHasBinded;
        }

        private void MessageListener_CloseMessageReceived(object? sender, EventArgs e)
        {
            MainPageViewModel bindingContext = (MainPageViewModel)BindingContext;

            // Assume it's a close message from the auction for now.
            bindingContext.SniperBidStatus = "Lost";
        }

        private void XmppClient_ClientHasBinded(object? sender, EventArgs e)
        {
            // send a join message to the user for the auction's item
            Jid auctionUser = xmppClient.CreateJidFromLocalUsername($"auction-{ItemId.Text}");
            xmppClient.SendMessageAsync(auctionUser, SouthabeeStandards.JOIN_REQUEST).ContinueWith(result =>
            {
                MainPageViewModel bindingContext = (MainPageViewModel)BindingContext;
                bindingContext.SniperBidStatus = "Joining";
            });
        }

        private async void OnJoinClicked(object sender, EventArgs e)
        {
            try
            {
                MainPageViewModel bindingContext = (MainPageViewModel)BindingContext;

                var xmppConfiguration = configuration.GetSection("Settings:XMPPConfiguration").Get<XMPPConfiguration>() ?? throw new Exception("XMPPConfiguration section of settings file could not be loaded.");
#if ANDROID
                await xmppClient.CreateWithLogAsync(xmppConfiguration.SniperUsername, xmppConfiguration.SniperPassword, xmppConfiguration.Server, true, messageListener);
#else
                await xmppClient.CreateWithLogAsync(xmppConfiguration.SniperUsername, xmppConfiguration.SniperPassword, xmppConfiguration.Server, messageListener);
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
