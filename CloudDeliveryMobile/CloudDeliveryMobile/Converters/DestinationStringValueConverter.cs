using CloudDeliveryMobile.Models.Orders;
using MvvmCross.Platform.Converters;
using System;
using System.Globalization;

namespace CloudDeliveryMobile.Converters
{
    public class DestinationStringValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var orderListItem = (Order)value;
            return String.Concat(orderListItem.DestinationCity, ", ", orderListItem.DestinationAddress);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
