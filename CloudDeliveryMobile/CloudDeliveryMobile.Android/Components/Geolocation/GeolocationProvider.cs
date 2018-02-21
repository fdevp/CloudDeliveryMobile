﻿using System;
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
        public GeolocationProvider(Activity activity)
        {
            this.activity = activity;
            this.client = new FusedLocationProviderClient(Application.Context);
        }

        public async void StartWatcher(int accuracy, int interval, int fastestInterval, LocationCallback callback)
        {
            if (this.Running)
                return;

            this.callback = callback;

            var locationRequest = new LocationRequest()
                                 .SetInterval(interval)
                                 .SetFastestInterval(fastestInterval)
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
            await client.RequestLocationUpdatesAsync(locationRequest, this.callback).ContinueWith(t =>
            {
                this.Running = true;
            });

        }

        public async void StopWatcher()
        {
            await client.RemoveLocationUpdatesAsync(this.callback).ContinueWith(t =>
            {
                this.Running = false;
            });
        }

        public async Task<Location> GetLocation()
        {
            return await this.client.GetLastLocationAsync();
        }


        private Activity activity;

        private FusedLocationProviderClient client;
        private LocationCallback callback;
    }
}