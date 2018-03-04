using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Enums;
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

        public event EventHandler AcceptedOrdersUpdated;

        public List<Order> PendingOrders { get; private set; }

        public List<Order> AcceptedOrders { get; private set; }

        public async Task Accept(Order order)
        {
            string resource = string.Concat(OrdersApiResources.Accept, "/", order.Id);
            string id = await this.httpProvider.PutAsync(httpProvider.AbsoluteUri(resource));

            order.Id = int.Parse(id);
            this.AcceptedOrders.Add(order);

            if (this.AcceptedOrdersUpdated != null)
                this.AcceptedOrdersUpdated.Invoke(this, null);

            this.PendingOrders.Remove(order);
            if (this.PendingOrdersUpdated != null)
                this.PendingOrdersUpdated.Invoke(this, null);
        }

        public async Task Delivered(int orderId)
        {
            string resource = string.Concat(OrdersApiResources.Delivered, "/", orderId);
            await this.httpProvider.PutAsync(httpProvider.AbsoluteUri(resource));
        }

        public async Task Pickup(int orderId)
        {
            string resource = string.Concat(OrdersApiResources.Pickup, "/", orderId);
            await this.httpProvider.PutAsync(httpProvider.AbsoluteUri(resource));
        }

        public async Task<List<Order>> GetAcceptedOrders()
        {
            Dictionary<string, string> filters = new Dictionary<string, string>();
            filters.Add("Status", OrderStatus.Accepted.ToString());

            string response = await this.httpProvider.GetAsync(httpProvider.AbsoluteUri(OrdersApiResources.List), filters);
            this.AcceptedOrders = JsonConvert.DeserializeObject<List<Order>>(response);

            if (this.AcceptedOrdersUpdated != null)
                this.AcceptedOrdersUpdated.Invoke(this, null);

            return this.AcceptedOrders;
        }

        public async Task<List<Order>> GetPendingOrders()
        {

            string response = await this.httpProvider.GetAsync(httpProvider.AbsoluteUri(OrdersApiResources.PendingOrders));
            this.PendingOrders = JsonConvert.DeserializeObject<List<Order>>(response, new JsonSerializerSettings { FloatParseHandling = FloatParseHandling.Double });

            if (this.PendingOrdersUpdated != null)
                this.PendingOrdersUpdated.Invoke(this, null);

            return this.PendingOrders;
        }

        public async Task<OrderDetails> Details(int orderId)
        {
            string resource = string.Concat(OrdersApiResources.Details, "/", orderId);
            string response = await this.httpProvider.GetAsync(httpProvider.AbsoluteUri(resource));
            OrderDetails order = JsonConvert.DeserializeObject<OrderDetails>(response);
            return order;
        }

        private IHttpProvider httpProvider;
    }
}
