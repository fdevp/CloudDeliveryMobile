using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CloudDeliveryMobile.Android.Components
{
    public class GeoPositionCallback : LocationCallback
    {
        public GeoPositionCallback(Activity activity, GoogleMap gmap, Marker posMarker)
        {
            this.activity = activity;
            this.gmap = gmap;
            this.posMarker = posMarker;
        }

        public override void OnLocationResult(LocationResult result)
        {
            base.OnLocationResult(result);
            this.activity.RunOnUiThread(() => {
                this.posMarker.Position = new LatLng(result.LastLocation.Latitude, result.LastLocation.Longitude);
                this.gmap.AnimateCamera(CameraUpdateFactory.NewLatLng(this.posMarker.Position));
            });
            
        }

        private Activity activity;
        private GoogleMap gmap;
        private Marker posMarker;
    }
}