using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Resources;
using Newtonsoft.Json;

namespace CloudDeliveryMobile.Services.Implementations
{
    public class SalepointOrdersService : ISalepointOrdersService
    {
        public SalepointOrdersService(IHttpProvider httpProvider, IStorageProvider storageProvider)
        {
            this.httpProvider = httpProvider;
            this.storageProvider = storageProvider;
        }

        public event EventHandler AddedOrdersUpdated;

        public event EventHandler InProgressOrdersUpdated;

        public List<OrderSalepoint> AddedOrders { get; private set; }

        public List<OrderSalepoint> InProgressOrders { get; private set; }

        public async Task<OrderSalepoint> Add(OrderEditModel order)
        {
            string response = await this.httpProvider.PostAsync(OrdersApiResources.Add, order);
            OrderSalepoint newOrder = JsonConvert.DeserializeObject<OrderSalepoint>(response);
            this.AddedOrders.Add(newOrder);

            if (this.AddedOrdersUpdated != null)
                this.AddedOrdersUpdated.Invoke(this, null);

            return newOrder;
        }

        public async Task Cancel(OrderSalepoint order)
        {
            string resource = string.Concat(OrdersApiResources.Cancel, "/", order.Id);
            await this.httpProvider.PutAsync(httpProvider.AbsoluteUri(resource));
            order.Status = OrderStatus.Cancelled;
            this.AddedOrders.Remove(order);

            if (this.AddedOrdersUpdated != null)
                this.AddedOrdersUpdated.Invoke(this, null);
        }

        public async Task<OrderDetails> Details(int orderId)
        {
            string resource = string.Concat(OrdersApiResources.Details, "/", orderId);
            string response = await this.httpProvider.GetAsync(httpProvider.AbsoluteUri(resource));
            OrderDetails order = JsonConvert.DeserializeObject<OrderDetails>(response);
            return order;
        }

        public async Task<List<OrderSalepoint>> GetAddedOrders()
        {
            string response = await this.httpProvider.GetAsync(httpProvider.AbsoluteUri(OrdersApiResources.AddedOrders));
            this.AddedOrders = JsonConvert.DeserializeObject<List<OrderSalepoint>>(response);

            if (this.AddedOrdersUpdated != null)
                this.AddedOrdersUpdated.Invoke(this, null);

            return this.AddedOrders;
        }

        public async Task<List<OrderSalepoint>> GetInProgressOrders()
        {
            string response = await this.httpProvider.GetAsync(httpProvider.AbsoluteUri(OrdersApiResources.InProgressOrders));
            this.InProgressOrders = JsonConvert.DeserializeObject<List<OrderSalepoint>>(response);

            if (this.InProgressOrdersUpdated != null)
                this.InProgressOrdersUpdated.Invoke(this, null);

            return this.AddedOrders;
        }

        public async Task<List<string>> StreetsList()
        {
            if (this.streets != null)
                return this.streets;

            try
            {
                string streetsJson = this.storageProvider.Select(DataKeys.Streets);
                this.streets = JsonConvert.DeserializeObject<List<string>>(streetsJson);
                return this.streets;
            }
            catch (Exception e) { }

            try
            {
                string streetsJson = await this.httpProvider.GetAsync(OrdersApiResources.Streets);
                this.storageProvider.Insert(DataKeys.Streets, streetsJson);
                this.streets = JsonConvert.DeserializeObject<List<string>>(streetsJson);
                return this.streets;
            }
            catch (Exception e) { }

            return new List<string>();
        }

        private List<string> streets;

        private IHttpProvider httpProvider;
        private IStorageProvider storageProvider;
    }
}
