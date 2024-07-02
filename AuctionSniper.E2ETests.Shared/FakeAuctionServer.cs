namespace E2ETests
{
    public class FakeAuctionServer
    {
        private string auctionId;
        public string AuctionId => auctionId;

        public FakeAuctionServer(string auctionId) => this.auctionId = auctionId;

        public void StartSellingItem()
        {

        }

        public void HasReceivedRequestToJoinFromSniper()
        {

        }

        public void AnnounceClosed()
        {

        }
    }
}
