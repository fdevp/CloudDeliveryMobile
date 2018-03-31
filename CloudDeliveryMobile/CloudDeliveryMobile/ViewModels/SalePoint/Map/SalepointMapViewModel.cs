﻿using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.ViewModels.SalePoint.SideView;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels.SalePoint.Map
{
    public class SalepointMapViewModel : BaseViewModel
    {
        public SalepointSideViewViewModel SideView { get; set; }

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

        public SalepointMapViewModel(IMvxNavigationService navigationService)
        {
            this.navigationService = navigationService;
            this.SideView = Mvx.IocConstruct<SalepointSideViewViewModel>();
        }

        public MvxAsyncCommand InitSideView
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    if (this.sideViewInitialised)
                        return;

                    this.sideViewInitialised = true;
                    await this.navigationService.Navigate(this.SideView);
                });

            }
        }


        private bool sideViewInitialised = false;
        private float? baseZoom;
        private GeoPosition basePosition;
        private IMvxNavigationService navigationService;
    }



}
