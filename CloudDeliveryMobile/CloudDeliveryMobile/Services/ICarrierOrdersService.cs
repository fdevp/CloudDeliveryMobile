using CloudDeliveryMobile.Models.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Services
{
    public interface ICarrierOrdersService
    {
        event EventHandler PendingOrdersUpdated;

        event EventHandler AcceptedOrdersUpdated;

        List<OrderCarrier> PendingOrders { get; }
        List<OrderCarrier> AcceptedOrders { get; }

        Task<List<OrderCarrier>> GetAcceptedOrders();

        Task<List<OrderCarrier>> GetPendingOrders();

        Task<OrderDetails> Details(int orderId);

        Task Accept(OrderCarrier order);

        Task Pickup(OrderRoute order);

        Task Delivered(OrderRoute order);
    }
}
