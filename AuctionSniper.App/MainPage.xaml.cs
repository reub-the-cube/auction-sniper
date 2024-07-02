using Microsoft.Extensions.Configuration;
using XmppDotNet;
using XmppDotNet.Extensions.Client.Message;
using XmppDotNet.Transport.Socket;

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

                string joinMessageBody = "SOLVersion: 1.1; Command: JOIN;";
                string auctionJid = new Jid($"auction-{AuctionId.Text}");

                await xmppClient.SendChatMessageAsync(auctionJid, joinMessageBody);

                SniperBidStatus.Text = "Joining";
            }
            catch (Exception ex)
            {
                SniperBidStatus.Text = ex.ToString();
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
