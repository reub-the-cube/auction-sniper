using Microsoft.Extensions.Configuration;

namespace E2ETests
{
    public class FakeAuctionServer
    {
        private readonly string auctionId;
        private readonly string itemId;
        public string AuctionId => auctionId;
        public string ItemId => itemId;

        public FakeAuctionServer(string itemId)
        {
            auctionId = $"auction-{itemId}";
            this.itemId = itemId;
        }

        public void StartSellingItem()
        {

        }

        public void HasReceivedRequestToJoinFromSniper()
        {

        }

        public void AnnounceClosed()
        {
            XMPPAccount auctionItemUser = BaseFixture.Configuration.GetSection($"{auctionId}").Get<XMPPAccount>() ?? throw new Exception($"Section with name {auctionId} of test settings file could not be loaded.");
            Console.WriteLine($"{auctionItemUser.Username}| {auctionItemUser.Password}");
        }
    }
}
