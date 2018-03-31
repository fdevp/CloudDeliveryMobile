using CloudDeliveryMobile.Models.Account;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Resources;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using System;
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

        public bool TokenInProgress { get; set; } = false;

        public SignInViewModel(ISessionProvider sessionProvider, IStorageProvider storageProvider, IMvxNavigationService navigationService, IDeviceProvider deviceProvider)
        {
            this.sessionProvider = sessionProvider;
            this.storageProvider = storageProvider;
            this.navigationService = navigationService;
            this.deviceProvider = deviceProvider;
        }

        public async Task SignIn()
        {
            await this.sessionProvider.SignIn(this.model).ContinueWith(async t =>
            {
                this.Password = string.Empty;
                this.InProgress = false;

                if (t.Exception != null)
                {
                    return;
                }

                await this.GoToRoot().ContinueWith(async x =>
                {
                    await this.navigationService.Close(this);
                });
            });
        }

        public override async void Start()
        {
            //set root viewmodel
            this.deviceProvider.RootViewModel = this;

            //try sign in by token
            this.TokenInProgress = true;
            await this.sessionProvider.CheckToken().ContinueWith(async t =>
            {
                //sign in by token failed
                if (t.Exception != null)
                {
                    this.TokenInProgress = false;
                    return;
                }


                //sign in by token success
                await this.GoToRoot().ContinueWith(async x =>
                {
                    await this.navigationService.Close(this);
                    return;
                });
            });


            //try get latest username
            try
            {
                this.model.Username = this.storageProvider.Select(DataKeys.LastUsername);
            }
            catch (Exception e)
            {

            }
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

        private LoginModel model = new LoginModel { Username = "pointPunkt", Password = "Admin1!" };
        private ISessionProvider sessionProvider;
        private IStorageProvider storageProvider;
        private IDeviceProvider deviceProvider;
        private IMvxNavigationService navigationService;
    }
}
