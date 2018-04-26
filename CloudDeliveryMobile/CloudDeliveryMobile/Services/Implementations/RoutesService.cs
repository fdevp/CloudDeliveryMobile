using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudDeliveryMobile.ApiInterfaces;
using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Enums.Events;
using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Resources;
using Newtonsoft.Json;
using Plugin.LocalNotifications;
using Refit;

namespace CloudDeliveryMobile.Services.Implementations
{
    public class RoutesService : IRoutesService
    {
        public RouteDetails ActiveRoute { get; private set; }

        public List<RouteListItem> FinishedRoutes { get; private set; }

        public event EventHandler<ServiceEvent<CarrierRouteEvents>> ActiveRouteUpdated;

        public event EventHandler FinishedRoutesUpdated;

        public RoutesService(IRoutesApi routesApi, INotificationsProvider notificationsProvider, ISessionProvider sessionProvider)
        {
            this.routesApi = routesApi;
            this.notificationsProvider = notificationsProvider;
            this.sessionProvider = sessionProvider;

            this.notificationsProvider.CarrierOrderCancelledEvent += (sender, orderId) => this.CancelOrderInRoute(orderId);
        }

        public async Task<RouteDetails> ActiveRouteDetails(bool refresh = false)
        {
            if (this.ActiveRoute != null && !refresh)
                return this.ActiveRoute;

            this.ActiveRoute = await this.routesApi.ActiveRouteDetails();

            if (this.ActiveRoute != null)
                this.ActiveRouteUpdated?.Invoke(this, new ServiceEvent<CarrierRouteEvents>(CarrierRouteEvents.AddedRoute));

            return this.ActiveRoute;
        }

        public async Task<RouteDetails> Add(List<RouteEditModel> model)
        {
            this.ActiveRoute = await this.routesApi.Add(model);

            this.ActiveRouteUpdated?.Invoke(this, new ServiceEvent<CarrierRouteEvents>(CarrierRouteEvents.AddedRoute));
            return this.ActiveRoute;
        }

        public async Task<RouteDetails> Details(int routeId)
        {

            return await RestService.For<IRoutesApi>(this.sessionProvider.HttpClient).Details(routeId);
        }

        public async Task<List<RouteListItem>> GetFinishedRoutes()
        {
            this.FinishedRoutes = await this.routesApi.FinishedRoutesList();
            this.FinishedRoutesUpdated?.Invoke(this, null);
            return this.FinishedRoutes;
        }

        public async Task FinishActiveRoute()
        {
            if (this.ActiveRoute == null)
                throw new NullReferenceException("Brak aktywnej trasy");
            await this.routesApi.FinishRoute(this.ActiveRoute.Id);
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
            await this.routesApi.PassPoint(point.Id);
            
            point.PassedTime = DateTime.Now;

            if (point.Type == RoutePointType.EndPoint && point.Order.Status == OrderStatus.InDelivery)
            {
                point.Order.Status = OrderStatus.Delivered;
                point.Order.DeliveredTime = DateTime.Now;
            }

            this.ActiveRouteUpdated?.Invoke(this, new ServiceEvent<CarrierRouteEvents>(CarrierRouteEvents.PassedPoint, point));
        }

        private void CancelOrderInRoute(int orderId)
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

        private IRoutesApi routesApi;
        private INotificationsProvider notificationsProvider;
        private ISessionProvider sessionProvider;

    }
}
