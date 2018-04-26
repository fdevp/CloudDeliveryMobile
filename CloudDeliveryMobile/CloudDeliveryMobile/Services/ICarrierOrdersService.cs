using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Enums.Events;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Models.Routes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Services
{
    public interface ICarrierOrdersService
    {

        /// <summary>
        /// 
        /// </summary>
        event EventHandler<ServiceEvent<CarrierOrdersEvents>> PendingOrdersUpdated;


        /// <summary>
        /// 
        /// </summary>
        event EventHandler<ServiceEvent<CarrierOrdersEvents>> AcceptedOrdersUpdated;


        /// <summary>
        /// 
        /// </summary>
        List<OrderCarrier> PendingOrders { get; }

        /// <summary>
        /// 
        /// </summary>
        List<OrderCarrier> AcceptedOrders { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<OrderCarrier>> GetAcceptedOrders();


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<OrderCarrier>> GetPendingOrders();


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
        Task Accept(OrderCarrier order);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task Pickup(OrderRoute order);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task Delivered(OrderRoute order);


        /// <summary>
        /// 
        /// </summary>
        void ClearData();


        /// <summary>
        /// 
        /// </summary>
        void ClearAcceptedOrders();
    }
}
