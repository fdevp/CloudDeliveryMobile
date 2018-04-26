using CloudDeliveryMobile.Models.Orders;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ApiInterfaces
{
    public interface ICarrierOrdersApi
    {
        [Put("/api/orders/accept/{orderId}")]
        Task AcceptOrder(int orderId);

        [Get("/api/orders/acceptedlist")]
        Task<List<OrderCarrier>> AcceptedOrdersList();

        [Put("/api/orders/delivered/{orderId}")]
        Task DeliverOrder(int orderId);

        [Get("/api/orders/details/{orderId}")]
        Task<OrderDetails> OrderDetails(int orderId);

        [Get("/api/orders/pendinglist")]
        Task<List<OrderCarrier>> PendingOrdersList();

        [Put("/api/orders/pickup/{orderId}")]
        Task PickupOrder(int orderId);
    }
}
