using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Resources;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.Platform;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels.SalePoint
{
    public class SalepointNewOrderViewModel : BaseViewModel
    {
        public OrderEditModel Model { get; set; }

        public string FullLocationName
        {
            get
            {
                return string.Concat(Model.DestinationCity, ", ", Model.DestinationAddress);
            }
        }

        public IMvxAsyncCommand CreateOrder
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    string asd = JsonConvert.SerializeObject(this.Model);
                });
            }
        }

        public IMvxAsyncCommand CloseFragment
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    await this.navigationService.Close(this);
                });
            }
        }

        public IMvxCommand RetrySearch
        {
            get
            {
                return new MvxCommand(() =>
                {
                    this.GeocoderStarted = false;
                    this.GeocoderFinished = false;
                    this.GeocoderInProgress = false;
                    this.AddressFound = false;
                });
            }
        }

        public bool GeocoderSuccess
        {
            get
            {
                return GeocoderFinished && AddressFound;
            }
        }

        public bool GeocoderFailed
        {
            get
            {
                return GeocoderFinished && !AddressFound;
            }
        }

        public bool AddressFound
        {
            get
            {
                return this.addressFound;
            }
            set
            {
                this.addressFound = value;
                RaisePropertyChanged(() => this.AddressFound);
                RaisePropertyChanged(() => this.GeocoderSuccess);
                RaisePropertyChanged(() => this.GeocoderFailed);
            }
        }

        public bool GeocoderStarted
        {
            get
            {
                return this.geocoderStarted;
            }
            set
            {
                this.geocoderStarted = value;
                RaisePropertyChanged(() => this.GeocoderStarted);
            }
        }

        public bool GeocoderFinished
        {
            get
            {
                return this.geocoderFinished;
            }
            set
            {
                this.geocoderFinished = value;
                RaisePropertyChanged(() => this.GeocoderFinished);
            }
        }

        public bool GeocoderInProgress
        {
            get
            {
                return this.geocoderInProgress;
            }
            set
            {
                this.geocoderInProgress = value;
                RaisePropertyChanged(() => this.GeocoderInProgress);
            }
        }

        public SalepointNewOrderViewModel(IMvxNavigationService navigationService, ISalepointOrdersService salepointOrdersService)
        {
            this.salepointOrdersService = salepointOrdersService;
            this.navigationService = navigationService;

            this.Model = new OrderEditModel();
            this.Model.DestinationCity = ConstantValues.default_city;

            LoadStreetsList();
        }


        private bool addressFound = false;
        private bool geocoderInProgress = false;
        private bool geocoderStarted = false;
        private bool geocoderFinished = false;
        private string apartament;

        private ISalepointOrdersService salepointOrdersService;
        private IMvxNavigationService navigationService;




        //street auto complete
        public string DestinationAdddress
        {
            get
            {
                return this.Model.DestinationAddress;
            }
            set
            {
                this.Model.DestinationAddress = value;
                this.RaisePropertyChanged(() => this.DestinationAdddress);
            }
        }


        private string _currentStreetHint;
        public string CurrentStreetHint
        {
            get
            { return _currentStreetHint; }
            set
            {
                if (value == "")
                {
                    _currentStreetHint = null;
                    ClearStreetSuggestions();
                    return;
                }
                else
                {
                    _currentStreetHint = value;
                }

                if (this.Model.DestinationCity != ConstantValues.default_city)
                    return;

                if (_currentStreetHint.Trim().Length < 2)
                {
                    ClearStreetSuggestions();
                    return;
                }

                var list = CompleteStreetsList.Where(i => (i ?? "").ToUpper().Contains(_currentStreetHint.ToUpper()));
                if (list.Count() > 0)
                {
                    StreetSuggestions = list.ToList();
                }
                else
                {
                    ClearStreetSuggestions();
                }
            }
        }

        private List<string> _streetSuggestions = new List<string>();
        public List<string> StreetSuggestions
        {
            get
            {
                if (_streetSuggestions == null)
                {
                    _streetSuggestions = new List<string>();
                }
                return _streetSuggestions;
            }
            set { _streetSuggestions = value; RaisePropertyChanged(()=> StreetSuggestions); }
        }

        private List<string> CompleteStreetsList;

        private async void LoadStreetsList()
        {
            CompleteStreetsList = await this.salepointOrdersService.StreetsList();
        }

        private void ClearStreetSuggestions()
        {
            StreetSuggestions = new List<string>();
        }

       
    }
}
