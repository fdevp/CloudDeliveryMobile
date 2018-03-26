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

namespace CloudDeliveryMobile.Android.Components.FloatingWidget
{
    public interface IFloatingWidgetActivity
    {
        FloatingWidgetService FloatingWidgetBinder { get; set; }

        FloatingWidgetConnection FloatingWidgetConnection { get; set; }
        
        bool FloatingWidetIsBound { get; set; }

        void CreateFloatingWidget();
    }
}