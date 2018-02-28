using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Services
{
    public interface IOrdersService
    {
        event EventHandler PendingOrdersUpdated;
        
        List<OrderListItem> PendingOrders { get; }

        Task<ApproximateTrace> ApproximateTrace(int orderId, GeoPosition currentPosition);

        Task<List<OrderListItem>> GetPendingOrders();

        Task<OrderDetails> Details(int orderId);

        void Accept(int orderId);

        void Pickup(int orderId);

        void Delivered(int orderId);

        Task<RouteRoot> SetTrace(int orderId, GeoPosition currentPosition);

        Task<RouteRoot> GetTrace(int orderId);
    }
}
