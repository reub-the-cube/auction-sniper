using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reactive.Linq;
using XmppDotNet;
using XmppDotNet.Extensions.Client.Disco;
using XmppDotNet.Extensions.Client.Message;
using XmppDotNet.Extensions.Client.Presence;
using XmppDotNet.Extensions.Client.Roster;
using XmppDotNet.Transport.Socket;
using XmppDotNet.Xmpp;
using XmppDotNet.Xmpp.Client;

namespace AuctionSniper.XMPP
{
    public class Client(ILogger<Client> logger)
    {
        private XmppClient? xmppClient;

        public async Task CreateWithLogAsync(string username, string password, string server, MessageListener messageListener)
        {
            xmppClient = new XmppClient(
                conf =>
                {
                    conf.UseSocketTransport();
                    conf.AutoReconnect = true;
                }
            )
            {
                Jid = $"{username}@{server}",
                Password = $"{password}"
            };

            xmppClient.StateChanged
                .Subscribe(ss =>
                {
                    // handle the message here
                    logger.LogInformation(ss.ToString());
                });

            xmppClient
                .XmppXElementReceived
                .Where(el => el is Message)
                .Subscribe(el =>
                {
                    // handle the message here
                    logger.LogInformation(el.ToString());
                    messageListener.ProcessMessage((Message)el);
                });

            xmppClient
                .StateChanged
                .Where(s => s == SessionState.Binded)
                .Subscribe(async ss =>
                {
                    //var roster = await xmppClient.RequestRosterAsync();
                    logger.LogInformation(ss.ToString());

                    await xmppClient.SendPresenceAsync(Show.Chat, "free for chat");
                });

            await xmppClient.ConnectAsync();
        }

        public Jid CreateJidFromLocalUsername(string username)
        {
            return $"{username}@{xmppClient?.Jid.Domain}";
        }

        public async Task SendMessageAsync(Jid to, string message)
        {
            await xmppClient.SendChatMessageAsync(to, message);
        }
    }
}
