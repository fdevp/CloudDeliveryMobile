using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Models
{
    public class ServiceEvent<TEventType>
    {
        public ServiceEvent(TEventType eventType, object resource = null)
        {
            this.Type = eventType;
            this.Resource = resource;
        }

        public TEventType Type { get; set; }
        public object Resource { get; set; }
    }
}
