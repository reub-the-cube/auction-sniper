using AuctionSniper.App.ViewModels;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Reactive.Linq;
using XmppDotNet;
using XmppDotNet.Extensions.Client.Message;
using XmppDotNet.Extensions.Client.Presence;
using XmppDotNet.Transport.Socket;
using XmppDotNet.Xmpp;
using XmppDotNet.Xmpp.Base;

namespace AuctionSniper.App
{
    public partial class MainPage : ContentPage
    {
        private readonly IConfiguration configuration;

        public MainPage(IConfiguration configuration)
        {
            InitializeComponent();

            this.configuration = configuration;
        }

        private async void OnJoinClicked(object sender, EventArgs e)
        {
            try
            {
                MainPageViewModel bindingContext = (MainPageViewModel)BindingContext;

                var xmppConfiguration = configuration.GetSection("Settings:XMPPConfiguration").Get<XMPPConfiguration>() ?? throw new Exception("XMPPConfiguration section of settings file could not be loaded.");

                var xmppClient = new XmppClient(
                    conf =>
                    {
#if ANDROID
                        conf.UseSocketTransport().WithCertificateValidator(new AlwaysAcceptCertificateValidator());
#else
                        conf.UseSocketTransport();
#endif
                        conf.AutoReconnect = true;
                    }
                )
                {
                    Jid = $"{xmppConfiguration.SniperUsername}@{xmppConfiguration.Server}",
                    Password = $"{xmppConfiguration.SniperPassword}"
                };

                await xmppClient.ConnectAsync();

                xmppClient
                    .StateChanged
                    .Where(s => s == SessionState.Binded)
                    .Subscribe(async v =>
                    {
                        // send our online presence to the server
                        await xmppClient.SendPresenceAsync(Show.Chat, "free for chat");

                        // send a join message to the user for the auction's item
                        string joinMessageBody = "SOLVersion: 1.1; Command: JOIN;";
                        await xmppClient.SendChatMessageAsync($"auction-{ItemId.Text}@{xmppConfiguration.Server}", joinMessageBody);
                        bindingContext.SniperBidStatus = "Joining";
                    });

                xmppClient
                    .XmppXElementReceived
                    .Where(el => el is Message)
                    .Subscribe(el =>
                    {
                        // Assume it's a close message from the auction for now.
                        bindingContext.SniperBidStatus = "Lost";
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
