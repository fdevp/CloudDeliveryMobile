using System;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using XamarinItemTouchHelper;
using static Android.Support.V7.Widget.RecyclerView;

namespace CloudDeliveryMobile.Android.Components.UI.DraggableAdapter
{
    public class RecyclerItemDragCallback : ItemTouchHelper.Callback
    {
        IItemDragListener mAdapter;

        public RecyclerItemDragCallback(IItemDragListener adapterListener) {
            this.mAdapter = adapterListener;
        }

        public override int GetMovementFlags(RecyclerView recyclerView, ViewHolder viewHolder)
        {
            int dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down;
            int swipeFlags = 0;
            return MakeMovementFlags(dragFlags, swipeFlags);
        }

        public override bool OnMove(RecyclerView recyclerView, ViewHolder source, ViewHolder target)
        {
            return mAdapter.OnItemMove(source.AdapterPosition, target.AdapterPosition);
        }

        public override void OnSelectedChanged(RecyclerView.ViewHolder viewHolder, int actionState)
        {
            // We only want the active item to change
            if (actionState != ItemTouchHelper.ActionStateIdle)
            {
                if (viewHolder is IItemTouchHelperViewHolder)
                {
                    // Let the view holder know that this item is being moved or dragged
                    IItemTouchHelperViewHolder itemViewHolder = (IItemTouchHelperViewHolder)viewHolder;
                    itemViewHolder.OnItemSelected();
                }
            }

            base.OnSelectedChanged(viewHolder, actionState);
        }

        public override void ClearView(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            base.ClearView(recyclerView, viewHolder);

            if (viewHolder is IItemTouchHelperViewHolder)
            {
                // Tell the view holder it's time to restore the idle state
                IItemTouchHelperViewHolder itemViewHolder = (IItemTouchHelperViewHolder)viewHolder;
                itemViewHolder.OnItemClear();
            }
        }


        public override void OnSwiped(ViewHolder viewHolder, int direction)
        {
            throw new NotImplementedException();
        }
    }

}