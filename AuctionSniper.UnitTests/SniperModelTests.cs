using AuctionSniper.App.Models;
using Shouldly;

namespace AuctionSniper.UnitTests
{
    public class SniperModelTests
    {
        [Fact]
        public void SetsSniperValuesInColumnsAndRaisesEvent()
        {
            var sniper = new Sniper();
            var propertyChanged = false;

            sniper.PropertyChanged += (object? sender, System.ComponentModel.PropertyChangedEventArgs e) =>
            {
                propertyChanged = true;
            };

            sniper.SniperSnapshotChanged(new Core.SniperSnapshot("item id", 555, 666, Core.SniperState.Bidding));

            sniper.AuctionId.ShouldBe("item id");
            sniper.BidStatus.ShouldBe("Bidding");
            sniper.CurrentPrice.ShouldBe(555);
            sniper.LastBid.ShouldBe(666);
            propertyChanged.ShouldBeTrue();
        }
    }
}
