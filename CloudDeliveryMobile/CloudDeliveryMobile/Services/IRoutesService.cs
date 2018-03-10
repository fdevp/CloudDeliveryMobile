using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.Models.Routes.Edit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Services
{
    public interface IRoutesService
    {
        RouteDetails ActiveRoute { get; }

        event EventHandler ActiveRouteUpdated;

        Task<RouteDetails> Add(List<RoutePointEditModel> model);

        Task<RouteDetails> ActiveRouteDetails();

        void FinishActiveRoute();

        Task<RouteDetails> Details(int routeId);

        Task PassPoint(RoutePoint point);
    }
}
