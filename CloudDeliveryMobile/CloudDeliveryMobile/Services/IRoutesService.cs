using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Enums.Events;
using CloudDeliveryMobile.Models.Routes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Services
{
    public interface IRoutesService
    {
        RouteDetails ActiveRoute { get; }

        event EventHandler<ServiceEvent<CarrierRouteEvents>> ActiveRouteUpdated;

        event EventHandler FinishedRoutesUpdated;

        List<RouteListItem> FinishedRoutes { get; }

        Task<RouteDetails> Add(List<RouteEditModel> model);

        Task<RouteDetails> ActiveRouteDetails(bool refresh = false);

        Task FinishActiveRoute();

        Task<List<RouteListItem>> GetFinishedRoutes();

        Task<RouteDetails> Details(int routeId);

        Task PassPoint(RoutePoint point);

        void CleanData();
    }
}
