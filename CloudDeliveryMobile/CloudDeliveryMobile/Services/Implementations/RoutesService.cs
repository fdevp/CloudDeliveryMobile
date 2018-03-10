﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.Models.Routes.Edit;
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
            if (this.ActiveRouteUpdated != null)
                this.ActiveRouteUpdated.Invoke(this, null);
            return this.ActiveRoute;
        }

        
        public async Task<RouteDetails> Add(List<RoutePointEditModel> model)
        {
            string response = await this.httpProvider.PostAsync(httpProvider.AbsoluteUri(RoutesApiResources.Add), model);
            this.ActiveRoute = JsonConvert.DeserializeObject<RouteDetails>(response);
            if (this.ActiveRouteUpdated != null)
                this.ActiveRouteUpdated.Invoke(this, null);
            return this.ActiveRoute;
        }
        


        public async Task<RouteDetails> Details(int routeId)
        {
            string resource = string.Concat(RoutesApiResources.Details, "/", routeId);
            string response = await this.httpProvider.GetAsync(httpProvider.AbsoluteUri(resource));
            RouteDetails order = JsonConvert.DeserializeObject<RouteDetails>(response);
            return order;
        }

        public async void FinishActiveRoute()
        {
            string resource = string.Concat(RoutesApiResources.Finish, "/", ActiveRoute.Id);
            await this.httpProvider.PutAsync(httpProvider.AbsoluteUri(resource));
            this.ActiveRoute = null;
            if (this.ActiveRouteUpdated != null)
                this.ActiveRouteUpdated.Invoke(this, null);
        }

        public async Task PassPoint(RoutePoint point)
        {
            string resource = string.Concat(RoutesApiResources.Details, "/", point);
            await this.httpProvider.PutAsync(httpProvider.AbsoluteUri(resource));
            point.PassedTime = DateTime.Now;
        }

        private IHttpProvider httpProvider;
        private IStorageProvider storageProvider;
    }
}
