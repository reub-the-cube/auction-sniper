using AuctionSniper.App.Interfaces;
using AuctionSniper.App.Models;
using AuctionSniper.XMPP;
using Microsoft.Extensions.Configuration;
using System.Collections.Specialized;

namespace AuctionSniper.App.ViewModels
{
    public class Portfolio : List<Sniper>, INotifyCollectionChanged, IPortfolio
    {
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public Portfolio(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;

            Add(new() { BidStatus = "Unjoined" });
        }

        public async Task JoinAnAuction(string itemId)
        {
            try
            {
                (string server, ClientUser sniperUser) = GetXMPPConfigurationDetails();
                Client xmppClient = _serviceProvider.GetRequiredService<Client>();
                Auction auction = new(xmppClient, itemId);


                Core.AuctionSniper auctionSniper = new(auction, this[0], itemId);
                IMessageTranslator messageTranslator = new SniperTranslator(auctionSniper, sniperUser.Username);

                xmppClient.ClientHasBinded += (object? sender, EventArgs e) => {
                    auction.Join().ContinueWith(result =>
                    {
                        this[0].BidStatus = "Joining";
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
            string server = _configuration.GetSection($"xmppSettings:server").Get<string>() ?? throw new Exception("xmppSettings:server section of settings file could not be loaded.");
            ClientUser sniperUser = _configuration.GetSection($"xmppSettings:sniper").Get<ClientUser>() ?? throw new Exception("xmppSettings:sniper section of settings file could not be loaded.");

            return (server, sniperUser);
        }
    }
}
