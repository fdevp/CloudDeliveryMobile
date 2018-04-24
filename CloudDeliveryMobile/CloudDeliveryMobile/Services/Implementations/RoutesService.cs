using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Enums.Events;
using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Resources;
using Newtonsoft.Json;
using Plugin.LocalNotifications;

namespace CloudDeliveryMobile.Services.Implementations
{
    public class RoutesService : IRoutesService
    {
        public RouteDetails ActiveRoute { get; private set; }

        public List<RouteListItem> FinishedRoutes { get; private set; }

        public event EventHandler<ServiceEvent<CarrierRouteEvents>> ActiveRouteUpdated;

        public event EventHandler FinishedRoutesUpdated;

        public RoutesService(IHttpProvider httpProvider, INotificationsProvider notificationsProvider)
        {
            this.httpProvider = httpProvider;
            this.notificationsProvider = notificationsProvider;

            this.notificationsProvider.CarrierOrderCancelledEvent += (sender, orderId) => this.CancelOrderFromRoute(orderId);
        }

        public async Task<RouteDetails> ActiveRouteDetails(bool refresh = false)
        {
            if (this.ActiveRoute != null && !refresh)
                return this.ActiveRoute;

            string response = await this.httpProvider.GetAsync(httpProvider.AbsoluteUri(RoutesApiResources.ActiveRouteDetails));
            this.ActiveRoute = JsonConvert.DeserializeObject<RouteDetails>(response);

            if (this.ActiveRoute != null)
                this.ActiveRouteUpdated?.Invoke(this, new ServiceEvent<CarrierRouteEvents>(CarrierRouteEvents.AddedRoute));

            return this.ActiveRoute;
        }

        public async Task<RouteDetails> Add(List<RouteEditModel> model)
        {
            string response = await this.httpProvider.PostAsync(httpProvider.AbsoluteUri(RoutesApiResources.Add), model);

            this.ActiveRoute = JsonConvert.DeserializeObject<RouteDetails>(response);

            this.ActiveRouteUpdated?.Invoke(this, new ServiceEvent<CarrierRouteEvents>(CarrierRouteEvents.AddedRoute));
            return this.ActiveRoute;
        }

        public async Task<RouteDetails> Details(int routeId)
        {
            string resource = string.Concat(RoutesApiResources.Details, "/", routeId);
            string response = await this.httpProvider.GetAsync(httpProvider.AbsoluteUri(resource));
            RouteDetails order = JsonConvert.DeserializeObject<RouteDetails>(response);
            return order;
        }

        public async Task<List<RouteListItem>> GetFinishedRoutes()
        {
            string response = await this.httpProvider.GetAsync(httpProvider.AbsoluteUri(RoutesApiResources.List));
            this.FinishedRoutes = JsonConvert.DeserializeObject<List<RouteListItem>>(response);
            this.FinishedRoutesUpdated?.Invoke(this, null);
            return this.FinishedRoutes;
        }

        public async Task FinishActiveRoute()
        {
            string resource = string.Concat(RoutesApiResources.Finish, "/", ActiveRoute.Id);
            await this.httpProvider.PutAsync(httpProvider.AbsoluteUri(resource));
            this.ActiveRoute = null;
            this.ActiveRouteUpdated?.Invoke(this, new ServiceEvent<CarrierRouteEvents>(CarrierRouteEvents.FinishedRoute));
        }

        public void CleanData()
        {
            this.ActiveRoute = null;
            this.ActiveRouteUpdated = null;
            this.FinishedRoutesUpdated = null;
        }

        public async Task PassPoint(RoutePoint point)
        {
            string resource = string.Concat(RoutesApiResources.PassPoint, "/", point.Id);
            await this.httpProvider.PutAsync(httpProvider.AbsoluteUri(resource));
            point.PassedTime = DateTime.Now;

            if (point.Type == RoutePointType.EndPoint && point.Order.Status == OrderStatus.InDelivery)
            {
                point.Order.Status = OrderStatus.Delivered;
                point.Order.DeliveredTime = DateTime.Now;
            }

            this.ActiveRouteUpdated?.Invoke(this, new ServiceEvent<CarrierRouteEvents>(CarrierRouteEvents.PassedPoint, point));
        }

        private void CancelOrderFromRoute(int orderId)
        {
            if (this.ActiveRoute == null)
                return;

            var point = ActiveRoute.Points.Where(x => x.OrderId == orderId).FirstOrDefault();
            if (point != null)
            {
                point.Order.Status = OrderStatus.Cancelled;
                this.ActiveRouteUpdated.Invoke(this, new ServiceEvent<CarrierRouteEvents>(CarrierRouteEvents.CancelledPoint, point));
                CrossLocalNotifications.Current.Show("Anulowano zamówienie z trasy", String.Concat("Do miejsca: ", point.Order.DestinationAddress), orderId);
            }

        }

        private IHttpProvider httpProvider;
        private INotificationsProvider notificationsProvider;
    }
}
