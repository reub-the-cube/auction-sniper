﻿using AuctionSniper.XMPP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XmppDotNet;

namespace E2ETests
{
    public class FakeAuctionServer
    {
        private readonly string auctionId;
        private readonly string itemId;
        private readonly MessageListener messageListener = new();
        private readonly Client xmppClient;

        public string AuctionId => auctionId;
        public string ItemId => itemId;

        public FakeAuctionServer(string itemId)
        {
            auctionId = $"auction-{itemId}";
            this.itemId = itemId;

            xmppClient = BaseFixture.ServiceProvider.GetRequiredService<Client>();
        }

        public async Task StartSellingItem()
        {
            ClientUser auctionItemUser = BaseFixture.Configuration.GetSection($"xmppSettings:{auctionId}").Get<ClientUser>() ?? throw new Exception($"Section with name xmppSettings:{auctionId} of test settings file could not be loaded.");
            string xmppServer = BaseFixture.Configuration.GetSection($"xmppSettings:server").Get<string>() ?? throw new Exception($"Section with name xmppSettings:server of test settings file could not be loaded.");

            await xmppClient.CreateWithLogAsync(auctionItemUser.Username, auctionItemUser.Password, xmppServer, messageListener);
        }

        public bool HasBeenJoined()
        {
            return messageListener.HasReceivedJoinMessage();
        }

        public async Task AnnounceClosed()
        {
            string username = messageListener.SenderOfFirstJoinMessage();
            Jid to = xmppClient.CreateJidFromLocalUsername(username);

            // Send close message to whoever joined.
            await xmppClient.SendMessageAsync(to, SouthabeeStandards.CLOSE_REQUEST);
        }
    }
}
