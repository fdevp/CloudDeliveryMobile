using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Enums.Events;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Resources;
using Newtonsoft.Json;
using Plugin.LocalNotifications;

namespace CloudDeliveryMobile.Services.Implementations
{
    public class SalepointOrdersService : ISalepointOrdersService
    {
        public SalepointOrdersService(IHttpProvider httpProvider, IStorageProvider storageProvider, INotificationsProvider notificationsProvider)
        {
            this.httpProvider = httpProvider;
            this.storageProvider = storageProvider;
            this.notificationsProvider = notificationsProvider;

            this.notificationsProvider.SalepointOrderAcceptedEvent += (sender, order) => this.OrderAccepted(order);
            this.notificationsProvider.SalepointOrderDeliveredEvent += (sender, orderId) => this.OrderDelivered(orderId);
            this.notificationsProvider.SalepointOrderPickedUpEvent += (sender, orderId) => this.OrderPickedUp(orderId);
        }

        public event EventHandler<ServiceEvent<SalepointAddedOrdersEvents>> AddedOrdersUpdated;

        public event EventHandler<ServiceEvent<SalepointInProgressOrdersEvents>> InProgressOrdersUpdated;

        public event EventHandler FinishedOrdersUpdated;

        public List<OrderSalepoint> AddedOrders { get; private set; }

        public List<OrderSalepoint> InProgressOrders { get; private set; }

        public List<OrderFinishedListItem> FinishedOrders { get; private set; }

        public async Task<OrderSalepoint> Add(OrderEditModel order)
        {
            string response = await this.httpProvider.PostAsync(httpProvider.AbsoluteUri(OrdersApiResources.Add), order);
            OrderSalepoint newOrder = JsonConvert.DeserializeObject<OrderSalepoint>(response);

            this.AddedOrders.Add(newOrder);

            this.AddedOrdersUpdated?.Invoke(this, new ServiceEvent<SalepointAddedOrdersEvents>(SalepointAddedOrdersEvents.AddedOrder, newOrder));

            return newOrder;
        }

        public async Task Cancel(OrderSalepoint order)
        {
            string resource = string.Concat(OrdersApiResources.Cancel, "/", order.Id);
            await this.httpProvider.PutAsync(httpProvider.AbsoluteUri(resource));

            order.Status = OrderStatus.Cancelled;
            OrderFinishedListItem finished = (OrderFinishedListItem)order;
            finished.CancellationTime = DateTime.Now;

            this.FinishedOrders.Add(finished);
            this.FinishedOrdersUpdated.Invoke(this, null);

            if (this.AddedOrders.Contains(order))
            {
                this.AddedOrders.Remove(order);
                this.AddedOrdersUpdated?.Invoke(this, new ServiceEvent<SalepointAddedOrdersEvents>(SalepointAddedOrdersEvents.RemovedOrder, order));
            }

            if (this.InProgressOrders.Contains(order))
            {
                this.InProgressOrders.Remove(order);
                this.InProgressOrdersUpdated?.Invoke(this, new ServiceEvent<SalepointInProgressOrdersEvents>(SalepointInProgressOrdersEvents.RemovedOrder, order));
            }
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


            this.AddedOrdersUpdated?.Invoke(this, new ServiceEvent<SalepointAddedOrdersEvents>(SalepointAddedOrdersEvents.AddedList));

            return this.AddedOrders;
        }

        public async Task<List<OrderSalepoint>> GetInProgressOrders()
        {
            string response = await this.httpProvider.GetAsync(httpProvider.AbsoluteUri(OrdersApiResources.InProgressOrders));
            this.InProgressOrders = JsonConvert.DeserializeObject<List<OrderSalepoint>>(response);

            this.InProgressOrdersUpdated?.Invoke(this, new ServiceEvent<SalepointInProgressOrdersEvents>(SalepointInProgressOrdersEvents.AddedList));

            return this.AddedOrders;
        }

        public async Task<List<OrderFinishedListItem>> GetFinishedOrders()
        {
            string response = await this.httpProvider.GetAsync(httpProvider.AbsoluteUri(OrdersApiResources.FinishedOrders));
            this.FinishedOrders = JsonConvert.DeserializeObject<List<OrderFinishedListItem>>(response);

            this.FinishedOrdersUpdated?.Invoke(this, null);

            return this.FinishedOrders;
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

        public void CleanData()
        {
            this.AddedOrders = null;
            this.InProgressOrders = null;

            this.AddedOrdersUpdated = null;
            this.FinishedOrdersUpdated = null;
            this.InProgressOrdersUpdated = null;

        }

        public void OrderAccepted(OrderSalepoint order)
        {
            if (this.InProgressOrders.All(x => x.Id != order.Id))
            {
                this.InProgressOrders.Add(order);
                this.InProgressOrdersUpdated?.Invoke(this, new ServiceEvent<SalepointInProgressOrdersEvents>(SalepointInProgressOrdersEvents.AddedOrder, order));
            }

            var addedOrderToRemove = this.AddedOrders.Where(x => x.Id == order.Id).FirstOrDefault();

            if (addedOrderToRemove != null)
            {
                this.AddedOrders.Remove(addedOrderToRemove);
                this.AddedOrdersUpdated?.Invoke(this, new ServiceEvent<SalepointAddedOrdersEvents>(SalepointAddedOrdersEvents.RemovedOrder, addedOrderToRemove));
            }

            CrossLocalNotifications.Current.Show("Zaakceptowano zamówienie", String.Concat("Zaakceptował: ", order.CarrierName, "\n", "Do miejsca: ", order.DestinationAddress), order.Id);
        }

        public void OrderPickedUp(int orderId)
        {
            var orderPickedUp = this.InProgressOrders.Where(x => x.Id == orderId).FirstOrDefault();
            if (orderPickedUp == null)
                return;

            orderPickedUp.PickUpTime = DateTime.Now;
            orderPickedUp.Status = OrderStatus.InDelivery;
            this.InProgressOrdersUpdated?.Invoke(this, new ServiceEvent<SalepointInProgressOrdersEvents>(SalepointInProgressOrdersEvents.PickedOrder, orderPickedUp));
        }

        public void OrderDelivered(int orderId)
        {
            var orderDelivered = this.InProgressOrders.Where(x => x.Id == orderId).FirstOrDefault();
            if (orderDelivered == null)
                orderDelivered = AddedOrders.Where(x => x.Id == orderId).FirstOrDefault();

            if (orderDelivered == null)
                return;

            orderDelivered.Status = OrderStatus.Delivered;

            OrderFinishedListItem finished = (OrderFinishedListItem)orderDelivered;
            finished.DeliveredTime = DateTime.Now;

            this.FinishedOrders.Add(finished);
            this.FinishedOrdersUpdated?.Invoke(this, null);

            this.InProgressOrders.Remove(orderDelivered);
            this.InProgressOrdersUpdated?.Invoke(this, new ServiceEvent<SalepointInProgressOrdersEvents>(SalepointInProgressOrdersEvents.RemovedOrder, orderDelivered));

            CrossLocalNotifications.Current.Show("Dostarczono zamówienie", string.Concat("Do miejsca: ", orderDelivered.DestinationAddress), orderDelivered.Id);
        }

        private List<string> streets;

        private IHttpProvider httpProvider;
        private IStorageProvider storageProvider;
        private INotificationsProvider notificationsProvider;
    }
}
