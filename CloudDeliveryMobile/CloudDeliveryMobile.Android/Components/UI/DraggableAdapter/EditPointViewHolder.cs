using Android.Graphics;
using Android.Views;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using XamarinItemTouchHelper;

namespace CloudDeliveryMobile.Android.Components.UI.DraggableAdapter
{
    public class EditPointViewHolder : MvxRecyclerViewHolder, IItemTouchHelperViewHolder
    {

        public View handleView;
        public View itemView;

        public EditPointViewHolder(View itemView, IMvxAndroidBindingContext bindingContext) : base(itemView, bindingContext)
        {
            this.itemView = itemView;
            this.handleView = itemView.FindViewById<View>(Resource.Id.edit_point_handle);
        }

        public void OnItemClear()
        {
            itemView.SetBackgroundColor(Color.White);
        }

        public void OnItemSelected()
        {
            itemView.SetBackgroundColor(Color.LightGray);
        }
    }
}