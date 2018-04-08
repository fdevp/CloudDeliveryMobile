using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Resources;
using Newtonsoft.Json;

namespace CloudDeliveryMobile.Services.Implementations
{
    public class CarrierOrdersService : ICarrierOrdersService
    {
        public CarrierOrdersService(IHttpProvider httpProvider)
        {
            this.httpProvider = httpProvider;
        }

        public event EventHandler PendingOrdersUpdated;

        public event EventHandler AcceptedOrdersUpdated;

        public List<OrderCarrier> PendingOrders { get; private set; }

        public List<OrderCarrier> AcceptedOrders { get; private set; }

        public async Task Accept(OrderCarrier order)
        {
            string resource = string.Concat(OrdersApiResources.Accept, "/", order.Id);
            string id = await this.httpProvider.PutAsync(httpProvider.AbsoluteUri(resource));

            order.Id = int.Parse(id);
            this.AcceptedOrders.Add(order);
            this.AcceptedOrdersUpdated?.Invoke(this, null);

            this.PendingOrders.Remove(order);
            this.PendingOrdersUpdated?.Invoke(this, null);
        }

        public async Task Delivered(OrderRoute order)
        {
            string resource = string.Concat(OrdersApiResources.Delivered, "/", order.Id);
            await this.httpProvider.PutAsync(httpProvider.AbsoluteUri(resource));
            order.DeliveredTime = DateTime.Now;
            order.Status = OrderStatus.Delivered;
        }

        public async Task Pickup(OrderRoute order)
        {
            string resource = string.Concat(OrdersApiResources.Pickup, "/", order.Id);
            await this.httpProvider.PutAsync(httpProvider.AbsoluteUri(resource));
            order.PickUpTime = DateTime.Now;
            order.Status = OrderStatus.InDelivery;
        }

        public async Task<List<OrderCarrier>> GetAcceptedOrders()
        {
            string response = await this.httpProvider.GetAsync(httpProvider.AbsoluteUri(OrdersApiResources.AcceptedOrders));
            this.AcceptedOrders = JsonConvert.DeserializeObject<List<OrderCarrier>>(response);

            this.AcceptedOrdersUpdated?.Invoke(this, null);

            return this.AcceptedOrders;
        }

        public async Task<List<OrderCarrier>> GetPendingOrders()
        {

            string response = await this.httpProvider.GetAsync(httpProvider.AbsoluteUri(OrdersApiResources.PendingOrders));
            this.PendingOrders = JsonConvert.DeserializeObject<List<OrderCarrier>>(response);

            this.PendingOrdersUpdated?.Invoke(this, null);

            return this.PendingOrders;
        }

        public async Task<OrderDetails> Details(int orderId)
        {
            string resource = string.Concat(OrdersApiResources.Details, "/", orderId);
            string response = await this.httpProvider.GetAsync(httpProvider.AbsoluteUri(resource));
            OrderDetails order = JsonConvert.DeserializeObject<OrderDetails>(response);
            return order;
        }

        public void ClearData()
        {
            this.AcceptedOrders = null;
            this.PendingOrders = null;
        }

        private IHttpProvider httpProvider;
    }
}
