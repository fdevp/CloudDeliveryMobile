using CloudDeliveryMobile.Models.Routes;
using MvvmCross.Platform.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Converters
{
    public class PointIsPassedValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var point = (RoutePoint)value;
            bool active = (bool)parameter;
            return !active && point.PassedTime.HasValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
