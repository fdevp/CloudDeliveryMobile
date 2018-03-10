using CloudDeliveryMobile.Models.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Services
{
    public interface IOrdersService
    {
        event EventHandler PendingOrdersUpdated;

        event EventHandler AcceptedOrdersUpdated;

        List<Order> PendingOrders { get; }
        List<Order> AcceptedOrders { get; }

        Task<List<Order>> GetAcceptedOrders();

        Task<List<Order>> GetPendingOrders();

        Task<OrderDetails> Details(int orderId);

        Task Accept(Order order);

        Task Pickup(int orderId);

        Task Delivered(int orderId);
    }
}
