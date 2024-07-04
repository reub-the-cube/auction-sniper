// See https://aka.ms/new-console-template for more information
using AuctionSniper.XMPP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using XmppDotNet;

IServiceProvider serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole())
    .AddSingleton<Client>()
    .BuildServiceProvider();

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("testsettings.dev.json", true)
    .Build();

// Get the Client service object from the container
Client clientService = serviceProvider.GetRequiredService<Client>();

ClientUser auctionItemUser = GetUserProfile();
string xmppServer = GetXmppServer();
string recipientUsername = GetRecipientUsername();

await clientService.CreateWithLogAsync(auctionItemUser.Username, auctionItemUser.Password, xmppServer);

var nextAction = Console.ReadLine();
await HandleActionAsync(nextAction);

ClientUser GetUserProfile()
{
    ClientUser? auctionItemUser = configuration.GetSection($"xmppSettings:sniper").Get<ClientUser>();
    if (auctionItemUser?.Username == null)
    {
        Console.WriteLine("User details not for in test settings section 'xmppSettings:sniper'.");
        Console.WriteLine("Please enter the username:");
        string username = Console.ReadLine() ?? "no user entered";
        Console.WriteLine("Please enter the password:");
        string password = Console.ReadLine() ?? "no password entered";
        auctionItemUser = new() { Username = username, Password = password };
    }

    return auctionItemUser;
}

string GetXmppServer()
{
    string? server = configuration.GetSection($"xmppSettings:server").Get<string>();
    if (server == null)
    {
        Console.WriteLine("Server details not for in test settings section 'xmppSettings:server'.");
        Console.WriteLine("Please enter the server:");
        server = Console.ReadLine() ?? "no server entered";
    }

    return server;
}

string GetRecipientUsername()
{
    string? recipient = configuration.GetSection($"xmppSettings:auction-item-54321:Username").Get<string>();
    if (recipient == null)
    {
        Console.WriteLine("Server details not for in test settings section 'xmppSettings:auction-item-54321:Username'.");
        Console.WriteLine("Please enter the recipient for any communication:");
        recipient = Console.ReadLine() ?? "no recipient entered";
    }

    return recipient;
}

async Task HandleActionAsync(string? action)
{
    switch (action)
    {
        case "message":
            await clientService.SendMessageAsync(new Jid($"{recipientUsername}@{xmppServer}"), "This is a test");
            Console.WriteLine("Message sent");
            break;
        case "roster":
            Console.WriteLine("Next time I'll get the roster");
            break;
        case "exit":
        case "quit":
            return;
        default:
            Console.WriteLine("Unhandled command: quitting process");
            return;
    }

    var nextAction = Console.ReadLine();
    await HandleActionAsync(nextAction);
}