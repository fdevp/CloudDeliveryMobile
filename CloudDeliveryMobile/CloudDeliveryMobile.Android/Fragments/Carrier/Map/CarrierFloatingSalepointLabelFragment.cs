using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;

namespace CloudDeliveryMobile.Android.Fragments.Carrier.Map
{
    public class CarrierFloatingSalepointLabelFragment : MvxFragment
    {

        public CarrierFloatingSalepointLabelFragment(Action closeButtonClickCallback)
        {
            this.closeButtonClickCallback = closeButtonClickCallback;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = base.OnCreateView(inflater, container, savedInstanceState);
            view = inflater.Inflate(this.FragmentId, container, false);

            var closeButton = view.FindViewById<ImageButton>(Resource.Id.carrier_floating_salepoint_label_close);
            closeButton.Click += CloseButtonClick;

            this.salepointNameTextView = view.FindViewById<TextView>(Resource.Id.carrier_floating_salepoint_label_name);
            this.salepointNameTextView.Text = this.salepointName;
            return view;
        }


        public override void OnResume()
        {
            base.OnResume();
            if (this.salepointNameTextView != null)
                this.salepointNameTextView.Text = this.salepointName;
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            this.closeButtonClickCallback();
        }

        public string SalepointName
        {
            set
            {
                this.salepointName = value;
                
            }
        }

        private string salepointName;
        private TextView salepointNameTextView;
        private Action closeButtonClickCallback;

        private int FragmentId { get; } = Resource.Layout.carrier_floating_salepoint_label;
    }
}