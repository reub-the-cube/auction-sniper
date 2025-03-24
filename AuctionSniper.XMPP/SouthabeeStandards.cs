namespace AuctionSniper.XMPP
{
    public static class SouthabeeStandards
    {
        public const string BID_REQUEST = "SOLVersion: 1.1; Command: BID; Price: {0};";
        public const string CLOSE_REQUEST = "SOLVersion: 1.1; Command: CLOSE;";
        public const string JOIN_REQUEST = "SOLVersion: 1.1; Command: JOIN;";
        public const string REPORT_PRICE_EVENT = "SOLVersion: 1.1; Event: PRICE; CurrentPrice: {0}; Increment: {1}; Bidder: {2};";

        public static bool IsBidCommand(string messageBody)
        {
            return messageBody.Contains("Command: BID;");
        }
    }
}
