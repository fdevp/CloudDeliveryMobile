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
using CloudDeliveryMobile.Android.Activities;

namespace CloudDeliveryMobile.Android.Components.FloatingWidget
{
    public class FloatingWidgetConnection : Java.Lang.Object, IServiceConnection
    {
        IFloatingWidgetActivity activity;

        private EventHandler serviceConnected;

        public event EventHandler ServiceConnected
        {
            add
            {
                if (serviceConnected == null || !serviceConnected.GetInvocationList().Contains(value))
                {
                    serviceConnected += value;
                }
            }
            remove
            {
                serviceConnected -= value;
            }
        }

        public FloatingWidgetConnection(IFloatingWidgetActivity activity)
        {
            this.activity = activity;
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            var serviceBinder = service as FloatingWidgetBinder;
            if (serviceBinder != null)
            {
                activity.FloatingWidgetBinder = serviceBinder.GetService();
                activity.FloatingWidetIsBound = true;

                if (serviceConnected != null)
                    serviceConnected.Invoke(this, null);
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            activity.FloatingWidetIsBound = false;
        }
    }
}