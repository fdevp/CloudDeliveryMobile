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
    public class PendingOrdersIconValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<OrderCarrier> pendingOrders = value as List<OrderCarrier>;
            if (pendingOrders == null || pendingOrders.Count == 0)
                return "@drawable/marker_bw";
            else
                return "@drawable/marker";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
