using Android.App;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;

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