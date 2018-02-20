using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace CloudDeliveryMobile.Android.Components
{
    public class GeolocationProvider
    {
        public bool Running { get; private set; } = false;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval">in miliseconds</param>
        /// <param name="fastestinterval">in miliseconds</param>
        public GeolocationProvider(Activity activity, int interval, int fastestInterval, LocationCallback callback)
        {
            this.activity = activity;
            this.interval = interval;
            this.fastestInterval = fastestInterval;
            this.callback = callback;

            this.client = new FusedLocationProviderClient(Application.Context);
        }

        public async void StartWatcher(int accuracy)
        {
            if (this.Running)
                return;

            var locationRequest = new LocationRequest()
                                 .SetInterval(this.interval)
                                 .SetFastestInterval(this.fastestInterval)
                                 .SetPriority(accuracy);



            //location settings dialog
            GoogleApiClient
               googleApiClient = new GoogleApiClient.Builder(Application.Context)
                   .AddApi(LocationServices.API)
                   .Build();

            googleApiClient.Connect();

            LocationSettingsRequest.Builder locationSettingsRequestBuilder = new LocationSettingsRequest.Builder();
            locationSettingsRequestBuilder.AddLocationRequest(locationRequest);
            locationSettingsRequestBuilder.SetAlwaysShow(false);

            LocationSettingsResult locationSettingsResult = await LocationServices.SettingsApi
                                                                                  .CheckLocationSettingsAsync(googleApiClient,
                                                                                                              locationSettingsRequestBuilder.Build());

            if (locationSettingsResult.Status.StatusCode == LocationSettingsStatusCodes.ResolutionRequired)
            {
                locationSettingsResult.Status.StartResolutionForResult(activity, 0);
            }


            //
            await client.RequestLocationUpdatesAsync(locationRequest, callback).ContinueWith(t =>
            {
                this.Running = true;
            });

        }

        public async void StopWatcher()
        {
            await client.RemoveLocationUpdatesAsync(callback).ContinueWith(t =>
            {
                this.Running = false;
            });
        }

        public async Task<Location> GetLocation()
        {
            return await this.client.GetLastLocationAsync();
        }

        private LocationCallback callback;
        private int interval;
        private int fastestInterval;
        private Activity activity;
        private Location lastLocation;

        private FusedLocationProviderClient client;
        private LocationRequest locationRequest;
        
    }
}