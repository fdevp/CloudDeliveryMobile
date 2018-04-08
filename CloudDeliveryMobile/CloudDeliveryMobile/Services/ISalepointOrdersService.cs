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

        event EventHandler FinishedOrdersUpdated;

        List<OrderSalepoint> AddedOrders { get; }

        List<OrderSalepoint> InProgressOrders { get;}

        List<OrderFinishedListItem> FinishedOrders { get; }

        Task<List<OrderSalepoint>> GetAddedOrders();

        Task<List<OrderSalepoint>> GetInProgressOrders();

        Task<List<OrderFinishedListItem>> GetFinishedOrders();

        Task<OrderDetails> Details(int orderId);

        Task<OrderSalepoint> Add(OrderEditModel order);

        Task<List<string>> StreetsList();

        Task Cancel(OrderSalepoint order);

        void ClearData();
    }
}
