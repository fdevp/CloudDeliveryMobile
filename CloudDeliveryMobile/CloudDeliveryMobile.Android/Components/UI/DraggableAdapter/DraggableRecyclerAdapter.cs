using System.Collections.Specialized;
using Android.Support.V7.Widget;
using Android.Views;
using CloudDeliveryMobile.Android.Components.UI.DraggableAdapter;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Droid.Support.V7.RecyclerView.ItemTemplates;
using MvvmCross.Platform.Droid.WeakSubscription;
using XamarinItemTouchHelper;

namespace CloudDeliveryMobile.Android.Components.UI
{

    public class DraggableRecyclerAdapter : MvxRecyclerAdapter, IItemDragListener, IItemTouchHelperAdapter
    {
        private IItemDragStartListener mDragListener;
        private IMvxAndroidBindingContext bindingContext;

        public DraggableRecyclerAdapter(IMvxAndroidBindingContext bindingContext, IItemDragStartListener mDragListener, IMvxTemplateSelector itemTemplateSelector) : base(bindingContext)
        {
            ItemTemplateSelector = itemTemplateSelector;
            this.mDragListener = mDragListener;
            this.bindingContext = bindingContext;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            base.OnCreateViewHolder(parent, viewType);
            var itemBindingContext = new MvxAndroidBindingContext(parent.Context, bindingContext.LayoutInflaterHolder);
            var holder = new EditPointViewHolder(InflateViewForHolder(parent, viewType, itemBindingContext), itemBindingContext);
            return holder;
        }


        protected override void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsSourceCollectionChanged(sender, e);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            base.OnBindViewHolder(holder, position);
            var editPointHolder = (EditPointViewHolder)holder;
            editPointHolder.handleView.SetOnTouchListener(new TouchListenerHelper(holder, mDragListener));
        }


        public bool OnItemMove(int fromPosition, int toPosition)
        {
            mDragListener.OnItemMove(fromPosition, toPosition);
            NotifyItemMoved(fromPosition, toPosition);
            return true;
        }

        public void OnItemDismiss(int position)
        {
            NotifyItemRemoved(position);
        }
    }
}