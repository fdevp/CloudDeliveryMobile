namespace CloudDeliveryMobile.Models.Orders
{
    public class ApproximateTrace
    {
        public DurationDistance CarrierToSalePoint { get; set; } = new DurationDistance();

        public DurationDistance SalePointToEndpoint { get; set; } = new DurationDistance();
    }
}
