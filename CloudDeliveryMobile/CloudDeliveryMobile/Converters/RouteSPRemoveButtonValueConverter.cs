using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Routes.Edit;
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
                var points = (MvxObservableCollection<RoutePointEditModel>)value;
                return points.Count(x => x.Type == RoutePointType.SalePoint && x.Order.SalepointId == salepointId) > 1;
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
