using CloudDeliveryMobile.Models.Account;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Resources;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        public SignInViewModel(ISessionProvider sessionProvider, IStorageProvider storageProvider, IMvxNavigationService navigationService)
        {
            this.sessionProvider = sessionProvider;
            this.storageProvider = storageProvider;
            this.navigationService = navigationService;
        }

        public async Task SignIn()
        {
            await this.sessionProvider.SignIn(this.model).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    this.InProgress = false;
                    return;
                }

                //sign in success
                this.navigationService.Navigate<RootViewModel>().ContinueWith(x =>
                {
                    this.model.Password = string.Empty;
                    this.InProgress = false;
                });

            });
        }

        private LoginModel model = new LoginModel();
        private ISessionProvider sessionProvider;
        private IStorageProvider storageProvider;
        private IMvxNavigationService navigationService;
    }
}
