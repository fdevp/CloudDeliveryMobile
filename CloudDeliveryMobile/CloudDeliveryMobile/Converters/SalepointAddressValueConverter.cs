using CloudDeliveryMobile.Models.Routes;
using MvvmCross.Platform.Converters;
using System;
using System.Globalization;
using CloudDeliveryMobile.Models.Routes.Edit;

namespace CloudDeliveryMobile.Converters
{
    public class SalepointAddressValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var routePoint = (RoutePointEditModel)value;
            return String.Concat(routePoint.Order.SalepointCity, ", ", routePoint.Order.SalepointAddress);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
