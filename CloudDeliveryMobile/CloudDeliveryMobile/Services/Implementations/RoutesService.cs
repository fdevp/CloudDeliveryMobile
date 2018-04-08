using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Resources;
using Newtonsoft.Json;

namespace CloudDeliveryMobile.Services.Implementations
{
    public class RoutesService : IRoutesService
    {
        public RouteDetails ActiveRoute { get; private set; }

        public event EventHandler ActiveRouteUpdated;

        public RoutesService(IHttpProvider httpProvider, IStorageProvider storageProvider)
        {
            this.httpProvider = httpProvider;
            this.storageProvider = storageProvider;
        }

        public async Task<RouteDetails> ActiveRouteDetails()
        {
            string response = await this.httpProvider.GetAsync(httpProvider.AbsoluteUri(RoutesApiResources.ActiveRouteDetails));
            this.ActiveRoute = JsonConvert.DeserializeObject<RouteDetails>(response);

            this.ActiveRouteUpdated?.Invoke(this, null);
            return this.ActiveRoute;
        }

        public async Task<RouteDetails> Add(List<RouteEditModel> model)
        {
            string response = await this.httpProvider.PostAsync(httpProvider.AbsoluteUri(RoutesApiResources.Add), model);
            this.ActiveRoute = JsonConvert.DeserializeObject<RouteDetails>(response);

            this.ActiveRouteUpdated?.Invoke(this, null);
            return this.ActiveRoute;
        }

        public async Task<RouteDetails> Details(int routeId)
        {
            string resource = string.Concat(RoutesApiResources.Details, "/", routeId);
            string response = await this.httpProvider.GetAsync(httpProvider.AbsoluteUri(resource));
            RouteDetails order = JsonConvert.DeserializeObject<RouteDetails>(response);
            return order;
        }

        public async Task FinishActiveRoute()
        {
            string resource = string.Concat(RoutesApiResources.Finish, "/", ActiveRoute.Id);
            await this.httpProvider.PutAsync(httpProvider.AbsoluteUri(resource));
            this.ActiveRoute = null;
            this.ActiveRouteUpdated?.Invoke(this, null);
        }

        public void ClearData()
        {
            this.ActiveRoute = null;
        }
        public async Task PassPoint(RoutePoint point)
        {
            string resource = string.Concat(RoutesApiResources.PassPoint, "/", point.Id);
            await this.httpProvider.PutAsync(httpProvider.AbsoluteUri(resource));
            point.PassedTime = DateTime.Now;
        }

        private IHttpProvider httpProvider;
        private IStorageProvider storageProvider;
    }
}
