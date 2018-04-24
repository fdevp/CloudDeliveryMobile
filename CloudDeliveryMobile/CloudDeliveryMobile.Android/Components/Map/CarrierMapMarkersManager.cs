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
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Enums.Events;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.ViewModels.Carrier;
using MvvmCross.Platform.Core;

namespace CloudDeliveryMobile.Android.Components.Map
{
    public class CarrierMapMarkersManager : IDisposable
    {
        private Dictionary<int, Marker> salepointsMarkers = new Dictionary<int, Marker>();
        private Dictionary<int, Marker> ordersMarkers = new Dictionary<int, Marker>();
        private Dictionary<int, Marker> routePointsMarkers = new Dictionary<int, Marker>();

        private GoogleMap map;
        private CarrierMapViewModel viewModel;
        private Activity activity;


        public void HandleSelectedSalepointMarkers(object sender, MvxValueEventArgs<int?> e)
        {
            CleanOrdersMarkers();

            if (e.Value.HasValue)
                SetOrdersMarkers();
        }

        public void HandleOrdersMarkers(object sender, MvxValueEventArgs<ServiceEvent<CarrierOrdersEvents>> e)
        {
            //if active route mode -> 
            if (this.viewModel.ActiveRouteMode)
                return;
            switch (e.Value.Type)
            {
                case CarrierOrdersEvents.AddedList:
                    CleanPendingMarkers();
                    SetPendingMarkers();
                    break;
                case CarrierOrdersEvents.AddedOrder:
                    OrderCarrier order = (OrderCarrier)e.Value.Resource;

                    //if selected salepoint has added order
                    if (this.viewModel.SelectedSalepointId == order.SalepointId)
                    {
                        SetOrderMarker(order);
                        break;
                    }

                    //if not selected salepoint has added first order
                    if (!this.salepointsMarkers.ContainsKey(order.SalepointId))
                        SetSalepointMarker(order);

                    break;
                case CarrierOrdersEvents.RemovedOrder:
                    OrderCarrier orderToRemove = (OrderCarrier)e.Value.Resource;
                    CleanMarker(MarkerType.Order, orderToRemove.Id);

                    //if removed order was last order of salepoint
                    if (this.viewModel.PendingOrders.All(x => x.SalepointId != orderToRemove.SalepointId))
                        CleanMarker(MarkerType.Salepoint, orderToRemove.SalepointId);

                    break;
            }
        }

        public void HandleRouteMarkers(object sender, MvxValueEventArgs<ServiceEvent<CarrierRouteEvents>> e)
        {
            switch (e.Value.Type)
            {
                case CarrierRouteEvents.AddedRoute:
                    CleanPendingMarkers();
                    SetRouteMarkers();
                    break;
                case CarrierRouteEvents.FinishedRoute:
                    CleanRoutePointsMarkers();
                    break;
                case CarrierRouteEvents.PassedPoint:
                    //remove point
                    RoutePoint routePoint = (RoutePoint)e.Value.Resource;
                    CleanMarker(MarkerType.ActiveRoutePoint, routePoint.Id);

                    //get next active
                    RoutePoint nextActivePoint = this.viewModel.ActiveRoute.Points.Where(x => x.PassedTime == null && x.Order.Status != OrderStatus.Cancelled).FirstOrDefault();
                    if (nextActivePoint == null)
                        return;
                    //remove next point
                    CleanMarker(MarkerType.PendingRoutePoint, nextActivePoint.Id);

                    //add next point as active point
                    if (nextActivePoint.Type == RoutePointType.EndPoint)
                        SetRouteOrderMarker(nextActivePoint, true);
                    else
                        SetRouteSalepointMarker(nextActivePoint, true);

                    break;
                case CarrierRouteEvents.CancelledPoint:
                    RoutePoint point = (RoutePoint)e.Value.Resource;
                    this.CleanMarker(MarkerType.PendingRoutePoint, point.Id);
                    break;
            }
        }

        public CarrierMapMarkersManager(Activity activity, GoogleMap map, CarrierMapViewModel viewModel)
        {
            this.activity = activity;
            this.map = map;
            this.viewModel = viewModel;

            if (this.viewModel.DataInitialised)
            {
                if (this.viewModel.ActiveRouteMode)
                    SetRouteMarkers();
                else
                    SetPendingMarkers();
            }

        }

        //markers collection actions
        private void SetRouteMarkers()
        {
            if (this.viewModel.ActiveRoute == null)
                return;

            //filter passed
            List<RoutePoint> points = this.viewModel.ActiveRoute.Points.Where(x => x.PassedTime == null).ToList();

            //filter cancelled
            points = points.Where(x => !(x.Type == RoutePointType.EndPoint && x.Order.Status == OrderStatus.Cancelled)).ToList();

            int activeRoutePointId = points.OrderBy(x => x.Index).FirstOrDefault().Id;

            foreach (RoutePoint item in points)
            {
                if (item.Type == RoutePointType.SalePoint)
                    SetRouteSalepointMarker(item, item.Id == activeRoutePointId);
                else
                    SetRouteOrderMarker(item, item.Id == activeRoutePointId);
            }

        }

        private void SetPendingMarkers()
        {
            if (this.viewModel.PendingOrders == null)
                return;

            //add new salepoint markers
            foreach (var item in this.viewModel.PendingOrders)
            {
                if (!this.salepointsMarkers.ContainsKey(item.SalepointId))
                    SetSalepointMarker(item);
            }

            //if active salepoint has not orders
            if (this.viewModel.SelectedSalepointId.HasValue && !this.salepointsMarkers.ContainsKey(this.viewModel.SelectedSalepointId.Value))
                this.viewModel.SelectedSalepointId = null;

            //update selected salepoint orders 
            if (this.viewModel.SelectedSalepointId.HasValue)
                SetOrdersMarkers();
        }

        private void SetOrdersMarkers()
        {
            var salepointOrders = this.viewModel.PendingOrders.Where(x => x.SalepointId == this.viewModel.SelectedSalepointId &&
                                                                          x.EndLatLng != null).ToList();

            foreach (var order in salepointOrders)
            {
                SetOrderMarker(order);
            }

        }

        private void CleanPendingMarkers()
        {
            foreach (var item in salepointsMarkers)
            {
                RemoveMarkerOnUIThread(item.Value);
            }

            salepointsMarkers.Clear();

            CleanOrdersMarkers();
        }

        private void CleanOrdersMarkers()
        {
            foreach (var item in ordersMarkers)
            {
                RemoveMarkerOnUIThread(item.Value);
            }

            ordersMarkers.Clear();
        }

        private void CleanRoutePointsMarkers()
        {
            foreach (var item in routePointsMarkers)
            {
                RemoveMarkerOnUIThread(item.Value);
            }

            routePointsMarkers.Clear();
        }


        //single marker actions
        private void SetSalepointMarker(OrderCarrier order)
        {
            if (order.SalepointLatLng == null || this.salepointsMarkers.ContainsKey(order.SalepointId))
                return;

            MarkerOptions options = new MarkerOptions();
            MarkerTag tag = new MarkerTag { OrderId = order.Id, SalepointId = order.SalepointId };

            tag.Type = MarkerType.Salepoint;

            BitmapDescriptor icon = BitmapDescriptorFactory.FromResource(Resource.Drawable.salepoint_marker);
            options.SetIcon(icon);

            string title = string.Concat(order.SalepointCity, ", ", order.SalepointAddress);
            options.SetTitle(title);
            options.SetPosition(new LatLng(order.SalepointLatLng.lat, order.SalepointLatLng.lng));

            this.activity.RunOnUiThread(() =>
            {
                Marker marker = this.map.AddMarker(options);
                marker.Tag = tag;
                this.salepointsMarkers.Add(order.SalepointId, marker);
            });
        }

        private void SetOrderMarker(OrderCarrier order)
        {
            if (order.EndLatLng == null || this.ordersMarkers.ContainsKey(order.Id))
                return;

            MarkerOptions options = new MarkerOptions();
            MarkerTag tag = new MarkerTag { OrderId = order.Id, SalepointId = order.SalepointId };

            tag.Type = MarkerType.Order;

            BitmapDescriptor icon = BitmapDescriptorFactory.FromResource(Resource.Drawable.endpoint_marker);
            options.SetIcon(icon);

            string title = string.Concat(order.DestinationCity, ", ", order.DestinationAddress);
            options.SetTitle(title);
            options.SetPosition(new LatLng(order.EndLatLng.lat, order.EndLatLng.lng));

            this.activity.RunOnUiThread(() =>
            {
                Marker marker = this.map.AddMarker(options);
                marker.Tag = tag;
                this.ordersMarkers.Add(order.Id, marker);
            });
        }

        private void SetRouteOrderMarker(RoutePoint point, bool active)
        {
            if (point.Order.EndLatLng == null || this.routePointsMarkers.ContainsKey(point.Id))
                return;

            MarkerOptions options = new MarkerOptions();
            MarkerTag tag = new MarkerTag { PointId = point.Id, OrderId = point.OrderId, SalepointId = point.Order.SalepointId };


            options.SetTitle(string.Concat(point.Order.DestinationCity, ", ", point.Order.DestinationAddress));
            options.SetPosition(new LatLng(point.Order.EndLatLng.lat, point.Order.EndLatLng.lng));

            BitmapDescriptor icon;
            if (active)
            {
                tag.Type = MarkerType.ActiveRoutePoint;
                icon = BitmapDescriptorFactory.FromResource(Resource.Drawable.endpoint_marker);
            }
            else
            {
                tag.Type = MarkerType.PendingRoutePoint;
                icon = BitmapDescriptorFactory.FromResource(Resource.Drawable.marker_bw);
            }

            options.SetIcon(icon);

            this.activity.RunOnUiThread(() =>
            {
                Marker marker = this.map.AddMarker(options);
                marker.Tag = tag;
                this.routePointsMarkers.Add(point.Id, marker);
            });
        }

        private void SetRouteSalepointMarker(RoutePoint point, bool active)
        {
            if (point.Order.SalepointLatLng == null || this.routePointsMarkers.ContainsKey(point.Id))
                return;

            MarkerOptions options = new MarkerOptions();
            MarkerTag tag = new MarkerTag { PointId = point.Id, OrderId = point.OrderId, SalepointId = point.Order.SalepointId };


            options.SetTitle(string.Concat(point.Order.SalepointCity, ", ", point.Order.SalepointAddress));
            options.SetPosition(new LatLng(point.Order.SalepointLatLng.lat, point.Order.SalepointLatLng.lng));

            BitmapDescriptor icon;
            if (active)
            {
                tag.Type = MarkerType.ActiveRoutePoint;
                icon = BitmapDescriptorFactory.FromResource(Resource.Drawable.salepoint_marker);
            }
            else
            {
                tag.Type = MarkerType.PendingRoutePoint;
                icon = BitmapDescriptorFactory.FromResource(Resource.Drawable.marker_bw);
            }

            options.SetIcon(icon);


            this.activity.RunOnUiThread(() =>
            {
                Marker marker = this.map.AddMarker(options);
                marker.Tag = tag;
                this.routePointsMarkers.Add(point.Id, marker);
            });
        }

        private void CleanMarker(MarkerType type, int dictKey)
        {
            switch (type)
            {
                case MarkerType.Salepoint:
                    if (this.salepointsMarkers.ContainsKey(dictKey))
                    {
                        RemoveMarkerOnUIThread(this.salepointsMarkers[dictKey]);
                        this.salepointsMarkers.Remove(dictKey);
                    }
                    break;
                case MarkerType.Order:
                    if (this.ordersMarkers.ContainsKey(dictKey))
                    {
                        RemoveMarkerOnUIThread(this.ordersMarkers[dictKey]);
                        this.ordersMarkers.Remove(dictKey);
                    }
                    break;
                case MarkerType.PendingRoutePoint:
                case MarkerType.ActiveRoutePoint:
                    if (this.routePointsMarkers.ContainsKey(dictKey))
                    {
                        RemoveMarkerOnUIThread(this.routePointsMarkers[dictKey]);
                        this.routePointsMarkers.Remove(dictKey);
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
            this.salepointsMarkers.Clear();
            this.ordersMarkers.Clear();
            this.routePointsMarkers.Clear();
        }
    }
}