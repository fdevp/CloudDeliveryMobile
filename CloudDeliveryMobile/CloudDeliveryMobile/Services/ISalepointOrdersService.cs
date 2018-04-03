using CloudDeliveryMobile.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Services
{
    public interface ISalepointOrdersService
    {
        event EventHandler AddedOrdersUpdated;

        event EventHandler InProgressOrdersUpdated;

        List<OrderSalepoint> AddedOrders { get; }

        List<OrderSalepoint> InProgressOrders { get;}

        Task<List<OrderSalepoint>> GetAddedOrders();

        Task<List<OrderSalepoint>> GetInProgressOrders();

        Task<OrderDetails> Details(int orderId);

        Task<OrderSalepoint> Add(OrderEditModel order);

        Task<List<string>> StreetsList();

        Task Cancel(OrderSalepoint order);
    }
}
