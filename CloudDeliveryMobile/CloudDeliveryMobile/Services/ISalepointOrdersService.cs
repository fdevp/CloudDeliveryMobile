using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Enums.Events;
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
        /// <summary>
        /// 
        /// </summary>
        event EventHandler<ServiceEvent<SalepointAddedOrdersEvents>> AddedOrdersUpdated;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler<ServiceEvent<SalepointInProgressOrdersEvents>> InProgressOrdersUpdated;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler FinishedOrdersUpdated;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        void OrderAccepted(OrderSalepoint order);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId"></param>
        void OrderPickedUp(int orderId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId"></param>
        void OrderDelivered(int orderId);


        /// <summary>
        /// 
        /// </summary>
        List<OrderSalepoint> AddedOrders { get; }


        /// <summary>
        /// 
        /// </summary>
        List<OrderSalepoint> InProgressOrders { get;}


        /// <summary>
        /// 
        /// </summary>
        List<OrderFinishedListItem> FinishedOrders { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<OrderSalepoint>> GetAddedOrders();


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<OrderSalepoint>> GetInProgressOrders();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<OrderFinishedListItem>> GetFinishedOrders();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<OrderDetails> Details(int orderId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<OrderSalepoint> Add(OrderEditModel order);


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<string>> StreetsList();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task Cancel(OrderSalepoint order);
    
        /// <summary>
        /// 
        /// </summary>
        void ClearData();
    }
}
