using AuctionSniper.App.ViewModels;
using AuctionSniper.XMPP;
using Microsoft.Extensions.Configuration;
using XmppDotNet;

namespace AuctionSniper.App
{
    public partial class MainPage : ContentPage
    {
        private readonly IConfiguration configuration;
        private readonly IServiceProvider serviceProvider;

        public MainPage(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            this.configuration = configuration;
            this.serviceProvider = serviceProvider;
        }

        private async void OnJoinClicked(object sender, EventArgs e)
        {
            try
            {
                ((MainPageViewModel)BindingContext).Snipers[0].BidStatus = "Unjoined";

                (string server, ClientUser sniperUser) = GetXMPPConfigurationDetails();

                Client xmppClient = serviceProvider.GetRequiredService<Client>();
                Auction auction = new(xmppClient, ItemId.Text);
                Core.AuctionSniper sniper = new(auction, ((MainPageViewModel)BindingContext).Snipers[0]);
                IMessageTranslator messageTranslator = new SniperTranslator(sniper, sniperUser.Username);

                xmppClient.ClientHasBinded += (object? sender, EventArgs e) => {
                    auction.Join().ContinueWith(result =>
                    {
                        MainPageViewModel bindingContext = (MainPageViewModel)BindingContext;
                        bindingContext.Snipers[0].BidStatus = "Joining";
                    });
                };

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

        private (string server, ClientUser sniper) GetXMPPConfigurationDetails()
        {
            string server = configuration.GetSection($"xmppSettings:server").Get<string>() ?? throw new Exception("xmppSettings:server section of settings file could not be loaded.");
            ClientUser sniperUser = configuration.GetSection($"xmppSettings:sniper").Get<ClientUser>() ?? throw new Exception("xmppSettings:sniper section of settings file could not be loaded.");

            return (server, sniperUser);
        }
    }
}