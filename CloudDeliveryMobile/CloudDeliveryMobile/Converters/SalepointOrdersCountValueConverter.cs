using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.ViewModels.Carrier;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Converters
{
    public class SalepointOrdersCountValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            try
            {
                int salepointId = (int)parameter;
                var points = (MvxObservableCollection<RoutePointEditListItem>)value;
                int ordersCount = points.Count(x => x.Order.SalepointId == salepointId && x.Type == RoutePointType.EndPoint);
                return string.Concat("Zamówień: ", ordersCount);
            }catch(Exception e)
            {
                return string.Empty;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
