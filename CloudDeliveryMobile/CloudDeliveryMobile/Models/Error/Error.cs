namespace CloudDeliveryMobile.Models.Error
{
    public class Error
    {
        public bool Occured { get; set; } = false;

        public ErrorType Type { get; set; }

        public string Message { get; set; }
    }
}
