using Acr.UserDialogs;
using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Resources;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.Platform;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels.SalePoint
{
    public class SalepointNewOrderViewModel : BaseViewModel
    {
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

                var list = CompleteStreetsList.Where(i => (i ?? "").ToUpper()
                                                                   .Contains(_currentStreetHint.ToUpper()));
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
            set { _streetSuggestions = value; RaisePropertyChanged(() => StreetSuggestions); }
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
        //


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
                    this.InProgress = true;
                    try
                    {
                        await this.salepointOrdersService.Add(this.Model);
                        this.dialogsService.Toast("Dodano nowe zamówienie.", TimeSpan.FromSeconds(5));
                        await this.navigationService.Close(this);
                        return;
                    }
                    catch (ApiException e) // server error
                    {
                        this.ErrorOccured = true;
                        this.ErrorMessage = e.Message;
                    }
                    catch (HttpRequestException httpException) //no connection
                    {
                        this.ErrorOccured = true;
                        this.ErrorMessage = "Problem z połączeniem z serwerem.";
                    }
                    catch (Exception unknownException)
                    {
                        this.ErrorOccured = true;
                        this.ErrorMessage = "Wystąpił nieznany błąd.";
                    }
                    finally
                    {
                        this.InProgress = false;
                    }

                    this.dialogsService.Toast(string.Concat("Błąd, ", this.ErrorMessage), TimeSpan.FromSeconds(5));

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

        public SalepointNewOrderViewModel(IMvxNavigationService navigationService, ISalepointOrdersService salepointOrdersService, IUserDialogs dialogsService)
        {
            this.salepointOrdersService = salepointOrdersService;
            this.navigationService = navigationService;
            this.dialogsService = dialogsService;

            this.Model = new OrderEditModel();
            this.Model.DestinationCity = ConstantValues.default_city;
            this.Model.DestinationAddress = string.Empty;

            LoadStreetsList();
        }


        private bool addressFound = false;
        private bool geocoderInProgress = false;
        private bool geocoderStarted = false;
        private bool geocoderFinished = false;
        private string apartament;

        private IUserDialogs dialogsService;
        private ISalepointOrdersService salepointOrdersService;
        private IMvxNavigationService navigationService;

    }
}
