using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CloudDeliveryMobile.Models;
using MvvmCross.Platform.Converters;

namespace CloudDeliveryMobile.Android.Components.Converters
{
    public class GeoPositionToLatLngConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GeoPosition geopos = (GeoPosition)value;
            return new LatLng(geopos.lat, geopos.lng);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            LatLng geopos = (LatLng)value;
            return new GeoPosition() { lat = geopos.Latitude, lng = geopos.Longitude };
        }
    }
}