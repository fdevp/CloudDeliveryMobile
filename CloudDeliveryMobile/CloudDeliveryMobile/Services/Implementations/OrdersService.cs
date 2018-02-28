using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Resources;
using Newtonsoft.Json;

namespace CloudDeliveryMobile.Services.Implementations
{
    public class OrdersService : IOrdersService
    {
        public OrdersService(IHttpProvider httpProvider)
        {
            this.httpProvider = httpProvider;
        }

        public event EventHandler PendingOrdersUpdated;

        public List<OrderListItem> PendingOrders { get; private set; }


        public async void Accept(int orderId)
        {
            string uri = string.Concat(OrdersApiResources.Accept, "/", orderId);
            await this.httpProvider.PutAsync(uri);
        }

        public async void Delivered(int orderId)
        {
            string uri = string.Concat(OrdersApiResources.Delivered, "/", orderId);
            await this.httpProvider.PutAsync(uri);
        }


        public async void Pickup(int orderId)
        {
            string uri = string.Concat(OrdersApiResources.Pickup, "/", orderId);
            await this.httpProvider.PutAsync(uri);
        }

        public async Task<List<OrderListItem>> GetPendingOrders()
        {
            string response = await this.httpProvider.GetAsync(OrdersApiResources.PendingOrders);
            this.PendingOrders = JsonConvert.DeserializeObject<List<OrderListItem>>(response, new JsonSerializerSettings { FloatParseHandling = FloatParseHandling.Double });

            if(this.PendingOrdersUpdated != null)
                this.PendingOrdersUpdated.Invoke(this, null);

            return this.PendingOrders;
        }

        public async Task<RouteRoot> GetTrace(int orderId)
        {
            string uri = string.Concat(OrdersApiResources.GetTrace, "/", orderId);
            string response = await this.httpProvider.GetAsync(uri);
            RouteRoot trace = JsonConvert.DeserializeObject<RouteRoot>(response);
            return trace;
        }

        public async Task<RouteRoot> SetTrace(int orderId, GeoPosition currentPosition)
        {
            string uri = string.Concat(OrdersApiResources.GetTrace, "/", orderId);
            string response = await this.httpProvider.PostAsync(uri, currentPosition);
            RouteRoot trace = JsonConvert.DeserializeObject<RouteRoot>(response);
            return trace;
        }

        public async Task<ApproximateTrace> ApproximateTrace(int orderId, GeoPosition currentPosition)
        {
            string response = await this.httpProvider.GetAsync(OrdersApiResources.ApproximateTrace, currentPosition.toDictionary());
            ApproximateTrace trace = JsonConvert.DeserializeObject<ApproximateTrace>(response);
            return trace;
        }

        public async Task<OrderDetails> Details(int orderId)
        {
            string uri = string.Concat(OrdersApiResources.Details, "/", orderId);
            string response = await this.httpProvider.GetAsync(uri);
            OrderDetails order = JsonConvert.DeserializeObject<OrderDetails>(response);
            return order;
        }

        private IHttpProvider httpProvider;
    }
}
