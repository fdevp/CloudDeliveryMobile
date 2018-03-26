using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.ViewModels.Carrier;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.Converters;

namespace CloudDeliveryMobile.Converters
{
    public class RouteSPRemoveButtonValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int salepointId = (int)parameter;
                var points = value as MvxObservableCollection<RoutePointEditListItem>;
                if(points != null)
                    return points.Count(x => x.Type == RoutePointType.SalePoint && x.Order.SalepointId == salepointId) > 1;
                return string.Empty;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
