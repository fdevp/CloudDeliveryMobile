using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.ViewModels.Carrier;
using MvvmCross.Platform.Converters;
using System;
using System.Globalization;

namespace CloudDeliveryMobile.Converters
{
    public class SalepointAddressValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var order =  value as Order;

            if(order != null)
                return String.Concat(order.SalepointCity, ", ", order.SalepointAddress);

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
