using CloudDeliveryMobile.Providers;
using MvvmCross.Core.Navigation;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels
{
    public class TokenSignInViewModel : BaseViewModel
    {
        public TokenSignInViewModel(ISessionProvider sessionProvider, IMvxNavigationService navigationService)
        {
            this.sessionProvider = sessionProvider;
            this.navigationService = navigationService;
        }

        public override async void Start()
        {

            await this.sessionProvider.CheckToken().ContinueWith(async t =>
            {
                //login by token failed
                if (t.Exception != null)
                {
                    await this.navigationService.Navigate<SignInViewModel>().ContinueWith(async task => await this.navigationService.Close(this));
                    return;
                }


                //sign in by token success
                await this.GoToRoot().ContinueWith(async x =>
                {
                    await this.navigationService.Close(this);
                });
            });
        }

        private async Task GoToRoot()
        {
            //sign in success
            if (this.sessionProvider.HasCarrierRole())
            {
                await this.navigationService.Navigate<RootCarrierViewModel>();
            }
            else if (this.sessionProvider.HasSalePointRole())
            {
                await this.navigationService.Navigate<RootSalePointViewModel>();
            }
        }


        private ISessionProvider sessionProvider;
        private IMvxNavigationService navigationService;
    }
}
