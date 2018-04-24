using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CloudDeliveryMobile.Android.Components.Geolocation;
using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Enums.Events;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.ViewModels.Carrier;
using CloudDeliveryMobile.ViewModels.SalePoint.Map;
using MvvmCross.Platform.Core;

namespace CloudDeliveryMobile.Android.Components.Map
{
    class SalepointMapMarkersManager : IDisposable
    {
        private Dictionary<int, Marker> addedOrdersMarkers = new Dictionary<int, Marker>();
        private Dictionary<int, Marker> inprogressOrdersMarkers = new Dictionary<int, Marker>();

        private GoogleMap map;
        private SalepointMapViewModel viewModel;
        private Activity activity;

        public void HandleAddedOrdersMarkers(object sender, MvxValueEventArgs<ServiceEvent<SalepointAddedOrdersEvents>> e)
        {
            switch (e.Value.Type)
            {
                case SalepointAddedOrdersEvents.AddedList:
                    CleanAddedOrdersMarkers();
                    SetAddedOrdersMarkers();
                    break;
                case SalepointAddedOrdersEvents.AddedOrder:
                    OrderSalepoint order = (OrderSalepoint)e.Value.Resource;
                    SetAddedOrdersMarker(order);
                    break;
                case SalepointAddedOrdersEvents.RemovedOrder:
                    OrderSalepoint orderToRemove = (OrderSalepoint)e.Value.Resource;
                    CleanMarker(MarkerType.AddedOrder, orderToRemove.Id);
                    break;
            }
        }

        public void HandleInProgressOrdersMarkers(object sender, MvxValueEventArgs<ServiceEvent<SalepointInProgressOrdersEvents>> e)
        {
            switch (e.Value.Type)
            {
                case SalepointInProgressOrdersEvents.AddedList:
                    CleanInProgressOrdersMarkers();
                    SetInProgressOrdersMarkers();
                    break;
                case SalepointInProgressOrdersEvents.AddedOrder:
                    OrderSalepoint order = (OrderSalepoint)e.Value.Resource;
                    SetInProgressOrdersMarker(order);
                    break;
                case SalepointInProgressOrdersEvents.RemovedOrder:
                    OrderSalepoint orderToRemove = (OrderSalepoint)e.Value.Resource;
                    CleanMarker(MarkerType.InProgressOrder, orderToRemove.Id);
                    break;
            }
        }

        public SalepointMapMarkersManager(Activity activity, GoogleMap map, SalepointMapViewModel viewModel)
        {
            this.activity = activity;
            this.map = map;
            this.viewModel = viewModel;

            SetAddedOrdersMarkers();
            SetInProgressOrdersMarkers();
        }


        private void SetAddedOrdersMarkers()
        {
            if (this.viewModel.AddedOrders == null)
                return;

            foreach (var item in this.viewModel.AddedOrders)
                SetAddedOrdersMarker(item);
        }

        private void SetInProgressOrdersMarkers()
        {
            if (this.viewModel.InProgressOrders == null)
                return;

            foreach (var item in this.viewModel.InProgressOrders)
                SetInProgressOrdersMarker(item);
        }

        private void CleanAddedOrdersMarkers()
        {
            foreach (var item in addedOrdersMarkers)
            {
                RemoveMarkerOnUIThread(item.Value);
            }

            addedOrdersMarkers.Clear();
        }

        private void CleanInProgressOrdersMarkers()
        {
            foreach (var item in inprogressOrdersMarkers)
            {
                RemoveMarkerOnUIThread(item.Value);
            }

            inprogressOrdersMarkers.Clear();
        }

        private void SetAddedOrdersMarker(OrderSalepoint order)
        {
            if (order.EndLatLng == null || this.addedOrdersMarkers.ContainsKey(order.Id))
                return;

            MarkerOptions options = new MarkerOptions();
            MarkerTag tag = new MarkerTag { OrderId = order.Id };

            tag.Type = MarkerType.AddedOrder;

            BitmapDescriptor icon = BitmapDescriptorFactory.FromResource(Resource.Drawable.marker_bw);
            options.SetIcon(icon);

            string title = string.Concat(order.DestinationCity, ", ", order.DestinationAddress);
            options.SetTitle(title);
            options.SetPosition(new LatLng(order.EndLatLng.lat, order.EndLatLng.lng));

            this.activity.RunOnUiThread(() =>
            {
                Marker marker = this.map.AddMarker(options);
                marker.Tag = tag;
                this.addedOrdersMarkers.Add(order.Id, marker);
            });
        }

        private void SetInProgressOrdersMarker(OrderSalepoint order)
        {
            if (order.EndLatLng == null || this.inprogressOrdersMarkers.ContainsKey(order.Id))
                return;

            MarkerOptions options = new MarkerOptions();
            MarkerTag tag = new MarkerTag { OrderId = order.Id };

            tag.Type = MarkerType.InProgressOrder;

            BitmapDescriptor icon = BitmapDescriptorFactory.FromResource(Resource.Drawable.marker);
            options.SetIcon(icon);

            string title = string.Concat(order.DestinationCity, ", ", order.DestinationAddress);
            options.SetTitle(title);
            options.SetPosition(new LatLng(order.EndLatLng.lat, order.EndLatLng.lng));

            this.activity.RunOnUiThread(() =>
            {
                Marker marker = this.map.AddMarker(options);
                marker.Tag = tag;
                inprogressOrdersMarkers.Add(order.Id, marker);
            });
        }

        private void CleanMarker(MarkerType type, int dictKey)
        {
            switch (type)
            {
                case MarkerType.AddedOrder:
                    if (this.addedOrdersMarkers.ContainsKey(dictKey))
                    {
                        this.RemoveMarkerOnUIThread(this.addedOrdersMarkers[dictKey]);
                        this.addedOrdersMarkers.Remove(dictKey);
                    }
                    break;
                case MarkerType.InProgressOrder:
                    if (this.inprogressOrdersMarkers.ContainsKey(dictKey))
                    {
                        this.RemoveMarkerOnUIThread(this.inprogressOrdersMarkers[dictKey]);
                        this.inprogressOrdersMarkers.Remove(dictKey);
                    }
                    break;
            }
        }

        private void RemoveMarkerOnUIThread(Marker marker)
        {
            this.activity.RunOnUiThread(() =>
            {
                marker.Remove();
            });
            
        }

        public void Dispose()
        {
            this.addedOrdersMarkers.Clear();
            this.inprogressOrdersMarkers.Clear();
        }
    }
}