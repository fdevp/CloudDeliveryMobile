using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Models.Account;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Resources;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels
{
    public class SignInViewModel : BaseViewModel
    {
        public string UserName
        {
            get { return model.Username; }
            set { model.Username = value; RaisePropertyChanged(() => UserName); }
        }

        public string Password
        {
            get { return model.Password; }
            set { model.Password = value; RaisePropertyChanged(() => Password); }
        }

        public IMvxAsyncCommand SignInCommand
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    this.InProgress = true;
                    this.storageProvider.Insert(DataKeys.LastUsername, model.Username);
                    await this.SignIn();
                });
            }
        }

        public bool TokenInProgress
        {
            get
            {
                return this.tokenInProgres;
            }
            set
            {
                this.tokenInProgres = value;
                RaisePropertyChanged(() => this.TokenInProgress);
            }
        }

        public string ClientId => ConstantValues.google_auth_client_id;

        public SignInViewModel(ISessionProvider sessionProvider, IStorageProvider storageProvider, IMvxNavigationService navigationService, IDeviceProvider deviceProvider)
        {
            this.sessionProvider = sessionProvider;
            this.storageProvider = storageProvider;
            this.navigationService = navigationService;
            this.deviceProvider = deviceProvider;
        }

        public async Task SignIn()
        {
            //form validation
            if (this.UserName.Length < 3 || this.Password.Length < 3)
            {
                this.ErrorOccured = true;
                this.ErrorMessage = "Podano nieprawidłowe dane";
                return;
            }
            this.ClearError();
            this.model.Device = $"{this.deviceProvider.DeviceName()}({this.deviceProvider.DeviceID()})";
            await sessionProvider.CredentialsSignIn(this.model).ContinueWith(async t => await HandleSignInResult(t));

        }

        public async Task GoogleSignIn(string authorizationCode)
        {
            ClearError();
            this.InProgress = true;
            await sessionProvider.GoogleSignIn(authorizationCode, this.deviceProvider.DeviceName()).ContinueWith(async t => await HandleSignInResult(t));
        }

        public override async void Start()
        {
            //set root viewmodel
            this.deviceProvider.RootViewModel = this;

            //try sign in by token
            this.TokenInProgress = true;

            await this.sessionProvider.RefreshTokenSignIn().ContinueWith(async t =>
            {
                //sign in by token failed
                if (t.Exception != null)
                {
                    this.TokenInProgress = false;
                    this.ErrorOccured = true;
                    if (t.Exception.InnerException.GetType() == typeof(InvalidTokenException))
                    {
                        this.ErrorMessage = t.Exception.InnerException.Message;
                    }
                    else if (t.Exception.InnerException.GetType() == typeof(HttpRequestException))
                    {
                        this.ErrorMessage = "Problem z połączeniem z serwerem.";
                    }
                    return;
                }


                //sign in by token success
                if (t.Result)
                {
                    await this.GoToRootAndClose();
                    return;
                }

                this.TokenInProgress = false;
            });


            //try get latest username
            try
            {
                this.model.Username = this.storageProvider.Select(DataKeys.LastUsername);
            }
            catch (Exception e) { }
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            base.ViewDestroy(viewFinishing);

            this.TokenInProgress = false;
            this.InProgress = false;
        }

        private async Task HandleSignInResult(Task<bool> result)
        {
            this.Password = string.Empty;

            //sign in failed
            if (result.Exception != null)
            {
                this.InProgress = false;
                this.ErrorOccured = true;
                if (result.Exception.InnerException.GetType() == typeof(SignInException))
                {
                    this.ErrorMessage = result.Exception.InnerException.Message;
                }
                else if (result.Exception.InnerException.GetType() == typeof(HttpRequestException))
                {
                    this.ErrorMessage = "Problem z połączeniem z serwerem.";
                }
                else
                {
                    this.ErrorMessage = "Wystąpił nieznany błąd.";
                }

                return;
            }

            //sign in success
            await this.GoToRootAndClose();
        }

        private void ClearError()
        {
            this.ErrorOccured = false;
            this.ErrorMessage = string.Empty;
        }

        private async Task GoToRootAndClose()
        {
            await this.GoToRoot().ContinueWith(async x =>
            {
                await this.navigationService.Close(this);
            });
        }

        private async Task GoToRoot()
        {
            //sign in success
            if (this.sessionProvider.HasCarrierRole())
            {
                await this.navigationService.Navigate<CarrierRootViewModel>();
            }
            else if (this.sessionProvider.HasSalePointRole())
            {
                await this.navigationService.Navigate<SalePointRootViewModel>();
            }
        }

        private bool tokenInProgres = false;
        private LoginModel model = new LoginModel { Username = "pointpunkt", Password = "Admin1!" };
        private ISessionProvider sessionProvider;
        private IStorageProvider storageProvider;
        private IDeviceProvider deviceProvider;
        private IMvxNavigationService navigationService;

    }
}
