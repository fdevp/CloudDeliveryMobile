using CloudDeliveryMobile.Models.Error;
using CloudDeliveryMobile.Providers;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;

namespace CloudDeliveryMobile.ViewModels
{
    public class BaseViewModel : MvxViewModel
    {
        public string ErrorMessage
        {
            get
            {
                return this.error.Message;
            }
            set
            {
                this.error.Message = value;
                RaisePropertyChanged(() => this.ErrorMessage);
            }
        }

        public bool ErrorOccured
        {
            get
            {
                return this.error.Occured;
            }
            set
            {
                this.error.Occured = value;
                RaisePropertyChanged(() => this.ErrorOccured);
                RaisePropertyChanged(() => ShowContent);
            }
        }

        public bool InProgress
        {
            get { return _inProgress; }
            set
            {
                _inProgress = value;
                RaisePropertyChanged(() => InProgress);
                RaisePropertyChanged(() => ShowContent);
            }
        }

        public bool ShowContent
        {
            get
            {
                return !this.InProgress && !this.ErrorOccured;
            }
        }

        protected bool _inProgress;
        protected Error error = new Error();
    }

    public class BaseViewModel<TParameter> : MvxViewModel<TParameter>
    {
        public string ErrorMessage
        {
            get
            {
                return this.error.Message;
            }
            set
            {
                this.error.Message = value;
                RaisePropertyChanged(() => this.ErrorMessage);
                RaisePropertyChanged(() => ShowContent);
            }
        }

        public bool ErrorOccured
        {
            get
            {
                return this.error.Occured;
            }
            set
            {
                this.error.Occured = value;
                RaisePropertyChanged(() => this.ErrorOccured);
                RaisePropertyChanged(() => ShowContent);
            }
        }

        public bool ShowContent
        {
            get
            {
                return !this.InProgress && !this.ErrorOccured;
            }
        }


        public bool InProgress
        {
            get { return _inProgress; }
            set
            {
                _inProgress = value;
                RaisePropertyChanged(() => InProgress);
                RaisePropertyChanged(() => ShowContent);
            }
        }

        public override void Prepare(TParameter parameter)
        {

        }

        protected bool _inProgress;
        protected Error error = new Error();
    }
}
