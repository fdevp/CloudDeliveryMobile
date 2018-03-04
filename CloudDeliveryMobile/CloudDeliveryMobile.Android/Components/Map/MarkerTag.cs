using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CloudDeliveryMobile.Android.Components.Geolocation
{
    public class MarkerTag : Java.Lang.Object
    {
        public MarkerType Type { get; set; }

        public int? SalepointId { get; set; }

        public int? OrderId { get; set; }
    }
}