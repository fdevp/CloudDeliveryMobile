using CloudDeliveryMobile.Models.Routes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Services
{
    public interface IRoutesService
    {
        RouteDetails ActiveRoute { get; }

        event EventHandler ActiveRouteUpdated;

        Task<RouteDetails> Add(List<RouteEditModel> model);

        Task<RouteDetails> ActiveRouteDetails();

        Task FinishActiveRoute();

        Task<RouteDetails> Details(int routeId);

        Task PassPoint(RoutePoint point);

        void ClearData();
    }
}
