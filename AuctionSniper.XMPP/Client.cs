using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using System.Reactive.Linq;
using XmppDotNet;
using XmppDotNet.Extensions.Client.Message;
using XmppDotNet.Extensions.Client.Presence;
using XmppDotNet.Transport.Socket;
using XmppDotNet.Xmpp;
using XmppDotNet.Xmpp.Client;

namespace AuctionSniper.XMPP
{
    public class Client
    {
        public event EventHandler ClientHasBinded;

        private readonly ILogger<Client> logger;
        private XmppClient? xmppClient;

        public Client(ILogger<Client> logger)
        {
            this.logger = logger;

            ClientHasBinded = delegate { };
        }

        public async Task CreateWithLogAsync(string username, string password, string server, bool acceptAllCertificates, MessageListener messageListener)
        {
            xmppClient = new XmppClient(
                conf =>
                {
                    if (acceptAllCertificates)
                    {
                        conf.UseSocketTransport().WithCertificateValidator(new AlwaysAcceptCertificateValidator());
                    }
                    else
                    {
                        conf.UseSocketTransport();
                    }
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
                    messageListener.ProcessMessage(this, (Message)el);
                });

            xmppClient
                .StateChanged
                .Where(s => s == SessionState.Binded)
                .Subscribe(async ss =>
                {
                    logger.LogInformation(ss.ToString());
                    await xmppClient.SendPresenceAsync(Show.Chat, "free for chat");
                    ClientHasBinded.Invoke(this, EventArgs.Empty);
                });

            await xmppClient.ConnectAsync();
        }
        public async Task CreateWithLogAsync(string username, string password, string server, MessageListener messageListener)
        {
            await CreateWithLogAsync(username, password, server, false, messageListener);
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
