using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Orders;
using MvvmCross.Platform.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Converters
{
    public class AllowCancelOrderValueConveter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var order = value as Order;
            if (order.Status == OrderStatus.Accepted || order.Status == OrderStatus.Added)
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
