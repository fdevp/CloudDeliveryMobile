using CloudDeliveryMobile.Providers;
using MvvmCross.Core.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            await this.sessionProvider.CheckToken().ContinueWith(t =>
            {
                //login by token failed
                if (t.Exception != null)
                {
                    this.navigationService.Navigate<SignInViewModel>();
                    this.navigationService.Close(this);
                    return;
                }

                //sign in by token success
                this.navigationService.Navigate<RootViewModel>();

            });
        }

        private ISessionProvider sessionProvider;
        private IMvxNavigationService navigationService;
    }
}
