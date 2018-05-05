using Android.OS;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Views;
using CloudDeliveryMobile.Android.Components.UI;
using CloudDeliveryMobile.Android.Components.UI.DraggableAdapter;
using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.ViewModels.Carrier.SideView;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Binding.Droid.Views;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Droid.Views.Attributes;
using MvvmCross.Platform.Droid.WeakSubscription;
using System;

namespace CloudDeliveryMobile.Android.Fragments.Carrier.SideView
{
    [MvxFragmentPresentation(fragmentContentId: Resource.Id.carrier_side_view_content, IsCacheableFragment = true)]
    public class CarrierSideRouteEditFragment : MvxFragment<CarrierSideRouteEditViewModel>, IItemDragStartListener
    {
      
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View ignore = base.OnCreateView(inflater, container, savedInstanceState);
            View view = this.BindingInflate(FragmentId, null);

            this.recyclerView = view.FindViewById<MvxRecyclerView>(Resource.Id.edit_route_points_list);
            this.recyclerView.ItemTemplateSelector = new EditRouteItemTemplateSelector();

            //set new draggable adapter
            var draggableAdapter = new DraggableRecyclerAdapter((IMvxAndroidBindingContext)this.BindingContext, this, this.recyclerView.Adapter.ItemTemplateSelector);
            this.recyclerView.Adapter = draggableAdapter;

            //set touch helper
            var touchCallback = new RecyclerItemDragCallback(draggableAdapter);
            this.mItemTouchHelper = new ItemTouchHelper(touchCallback);
            mItemTouchHelper.AttachToRecyclerView(recyclerView);

            return view;
        }

        public void OnStartDrag(RecyclerView.ViewHolder viewHolder)
        {
            mItemTouchHelper.StartDrag(viewHolder);
        }

        public bool OnItemMove(int fromPosition, int toPosition)
        {
            this.ViewModel.MovePoint.Execute(new RouteMoveEditPoint { SourceIndex = fromPosition, DestinationIndex = toPosition });
            return true;
        }

        private ItemTouchHelper mItemTouchHelper;
        private MvxRecyclerView recyclerView;
        private int FragmentId { get; } = Resource.Layout.carrier_side_route_edit;
    }


}