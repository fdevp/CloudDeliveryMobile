using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudDeliveryMobile.ApiInterfaces;
using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Enums.Events;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Resources;
using Newtonsoft.Json;
using Plugin.LocalNotifications;

namespace CloudDeliveryMobile.Services.Implementations
{
    public class CarrierOrdersService : ICarrierOrdersService
    {
        public CarrierOrdersService(ICarrierOrdersApi carrierOrdersApi, INotificationsProvider notificationsProvider)
        {
            this.carrierOrdersApi = carrierOrdersApi;
            this.notificationsProvider = notificationsProvider;

            this.notificationsProvider.CarrierOrderAddedEvent += (sender, order) => AddPendingOrder(order);
            this.notificationsProvider.CarrierOrderAcceptedEvent += (sender, id) => RemovePendingOrder(id);
            this.notificationsProvider.CarrierOrderCancelledEvent += (sender, id) => RemovePendingOrder(id);
        }

        public event EventHandler<ServiceEvent<CarrierOrdersEvents>> PendingOrdersUpdated;

        public event EventHandler<ServiceEvent<CarrierOrdersEvents>> AcceptedOrdersUpdated;

        public List<OrderCarrier> PendingOrders { get; private set; }

        public List<OrderCarrier> AcceptedOrders { get; private set; }

        public async Task Accept(OrderCarrier order)
        {
            await this.carrierOrdersApi.AcceptOrder(order.Id);

            this.AcceptedOrders.Add(order);
            this.AcceptedOrdersUpdated?.Invoke(this, new ServiceEvent<CarrierOrdersEvents>(CarrierOrdersEvents.AddedOrder,order));

            this.PendingOrders.Remove(order);
            this.PendingOrdersUpdated?.Invoke(this, new ServiceEvent<CarrierOrdersEvents>(CarrierOrdersEvents.RemovedOrder, order));
        }

        public async Task Delivered(OrderRoute order)
        {
            await this.carrierOrdersApi.DeliverOrder(order.Id);
            order.DeliveredTime = DateTime.Now;
            order.Status = OrderStatus.Delivered;
        }

        public async Task Pickup(OrderRoute order)
        {
            await this.carrierOrdersApi.PickupOrder(order.Id);
            order.PickUpTime = DateTime.Now;
            order.Status = OrderStatus.InDelivery;
        }

        public async Task<List<OrderCarrier>> GetAcceptedOrders()
        {
            this.AcceptedOrders = await this.carrierOrdersApi.AcceptedOrdersList();

            this.AcceptedOrdersUpdated?.Invoke(this, new ServiceEvent<CarrierOrdersEvents>(CarrierOrdersEvents.AddedList));

            return this.AcceptedOrders;
        }

        public async Task<List<OrderCarrier>> GetPendingOrders()
        {
            this.PendingOrders = await this.carrierOrdersApi.PendingOrdersList();

            this.PendingOrdersUpdated?.Invoke(this, new ServiceEvent<CarrierOrdersEvents>(CarrierOrdersEvents.AddedList));

            return this.PendingOrders;
        }

        public async Task<OrderDetails> Details(int orderId)
        {
            OrderDetails order = await this.carrierOrdersApi.OrderDetails(orderId);
            return order;
        }

        public void CleanData()
        {
            this.AcceptedOrders = null;
            this.PendingOrders = null;
            this.AcceptedOrdersUpdated = null;
            this.PendingOrdersUpdated = null;
        }

        public void CleanAcceptedOrders()
        {
            this.AcceptedOrders = null;
            this.AcceptedOrdersUpdated?.Invoke(this, new ServiceEvent<CarrierOrdersEvents>(CarrierOrdersEvents.RemovedList));
        }

        private void AddPendingOrder(OrderCarrier order)
        {
            if (this.PendingOrders != null && this.PendingOrders.All(x => x.Id != order.Id))
            {
                this.PendingOrders.Add(order);
                this.PendingOrdersUpdated?.Invoke(this, new ServiceEvent<CarrierOrdersEvents>(CarrierOrdersEvents.AddedOrder, order));
            }

            CrossLocalNotifications.Current.Show("Dodano zamówienie", String.Concat("Od: ", order.SalepointName, "\n", "Do miejsca: ", order.DestinationAddress), order.Id);

        }

        private void RemovePendingOrder(int orderId)
        {
            if (this.PendingOrders == null)
                return;

            var orderToRemove = this.PendingOrders.Where(x => x.Id == orderId).FirstOrDefault();
            if (orderToRemove != null)
            {
                this.PendingOrders.Remove(orderToRemove);
                this.PendingOrdersUpdated?.Invoke(this, new ServiceEvent<CarrierOrdersEvents>(CarrierOrdersEvents.RemovedOrder, orderToRemove));
            }

        }

        private ICarrierOrdersApi carrierOrdersApi;
        private INotificationsProvider notificationsProvider;
    }
}
