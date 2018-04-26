using CloudDeliveryMobile.Models.Orders;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ApiInterfaces
{
    public interface ISalepointOrdersApi
    {
        [Post("/api/orders/add")]
        Task<OrderSalepoint> AddOrder([Body] OrderEditModel order);

        [Get("/api/orders/AddedList")]
        Task<List<OrderSalepoint>> AddedOrdersList();

        [Put("/api/orders/cancel/{orderId}")]
        Task CancelOrder(int orderId);
     
        [Get("/api/orders/details/{orderId}")]
        Task<OrderDetails> OrderDetails(int orderId);

        [Get("/api/orders/FinishedList")]
        Task<List<OrderFinishedListItem>> FinishedOrdersList();

        [Get("/api/orders/InProgressList")]
        Task<List<OrderSalepoint>> InProgressOrdersList();

        [Get("/content/streets.json")]
        Task<List<string>> Streets();
    }
}
