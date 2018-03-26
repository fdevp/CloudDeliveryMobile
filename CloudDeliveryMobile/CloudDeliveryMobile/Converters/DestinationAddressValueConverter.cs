using CloudDeliveryMobile.Models.Orders;
using MvvmCross.Platform.Converters;
using System;
using System.Globalization;

namespace CloudDeliveryMobile.Converters
{
    public class DestinationAddressValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var orderListItem = value as Order;
            if(orderListItem != null)
                return String.Concat(orderListItem.DestinationCity, ", ", orderListItem.DestinationAddress);
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
