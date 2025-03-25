using AuctionSniper.App.ViewModels;
using AuctionSniper.XMPP;
using Microsoft.Extensions.Configuration;
using XmppDotNet;

namespace AuctionSniper.App
{
    public partial class MainPage : ContentPage
    {
        private readonly IConfiguration configuration;
        private SniperTranslator? messageTranslator;
        private readonly Client xmppClient;

        public MainPage(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            this.configuration = configuration;
            xmppClient = serviceProvider.GetRequiredService<Client>();

            xmppClient.ClientHasBinded += XmppClient_ClientHasBinded;
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
                messageTranslator = new SniperTranslator(new Core.AuctionSniper((MainPageViewModel)BindingContext));

                string server = configuration.GetSection($"xmppSettings:server").Get<string>() ?? throw new Exception("xmppSettings:server section of settings file could not be loaded.");
                ClientUser sniperUser = configuration.GetSection($"xmppSettings:sniper").Get<ClientUser>() ?? throw new Exception("xmppSettings:sniper section of settings file could not be loaded.");

#if ANDROID
                await xmppClient.CreateWithLogAsync(sniperUser.Username, sniperUser.Password, server, true, messageTranslator);
#else
                await xmppClient.CreateWithLogAsync(sniperUser.Username, sniperUser.Password, server, messageTranslator);
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
