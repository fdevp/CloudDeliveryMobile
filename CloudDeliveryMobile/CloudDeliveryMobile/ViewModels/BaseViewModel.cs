using CloudDeliveryMobile.Models.Error;
using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels
{
    public class BaseViewModel : MvxViewModel
    {
        public Error Error { get; set; } = new Error();

        public bool InProgress
        {
            get { return _inProgress; }
            set { _inProgress = value; RaisePropertyChanged(() => InProgress); }
        }

        protected bool _inProgress;
    }

    public class BaseViewModel<TParameter> : MvxViewModel<TParameter>
    {
        public Error Error { get; set; } = new Error();

        public bool InProgress
        {
            get { return _inProgress; }
            set { _inProgress = value; RaisePropertyChanged(() => InProgress); }
        }

        protected bool _inProgress;

        public override void Prepare(TParameter parameter)
        {
            
        }
    }
}
