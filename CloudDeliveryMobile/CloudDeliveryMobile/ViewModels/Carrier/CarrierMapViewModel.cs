using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Providers;
using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CloudDeliveryMobile.ViewModels.Carrier
{
    public class CarrierMapViewModel : BaseViewModel
    {
        public GeoPosition CurrentPosition
        {
            get
            {
                return this.currentPosition;
            }
        }

        public void UpdateCurrentPosition(double lat, double lng)
        {
            this.currentPosition.lat = lat;
            this.currentPosition.lng = lng;
            RaisePropertyChanged(() => this.CurrentPosition);
        }

        public GeoPosition BasePosition
        {
            get
            {
                if (this.basePosition != null)
                    return this.basePosition;

                this.basePosition = new GeoPosition { lat = Double.Parse(Resources.ConstantValues.map_base_lat, CultureInfo.InvariantCulture), lng = Double.Parse(Resources.ConstantValues.map_base_lng, CultureInfo.InvariantCulture) };

                return this.basePosition;
            }
        }

        public float BaseZoom
        {
            get
            {
                if (this.baseZoom.HasValue)
                    return this.baseZoom.Value;

                this.baseZoom = float.Parse(Resources.ConstantValues.map_base_zoom, CultureInfo.InvariantCulture);
                return this.baseZoom.Value;
            }
        }


        public CarrierMapViewModel(IDeviceProvider deviceProvider)
        {
            this.deviceProvider = deviceProvider;
        }

        private IDeviceProvider deviceProvider;
        private float? baseZoom;
        private GeoPosition basePosition;
        private GeoPosition currentPosition = new GeoPosition();
    }
}
