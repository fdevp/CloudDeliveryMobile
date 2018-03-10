using XamarinItemTouchHelper;

namespace CloudDeliveryMobile.Android.Components.UI.DraggableAdapter
{
    public interface IItemDragStartListener : IItemDragListener, IOnStartDragListener
    {
    }

    public interface IItemDragListener
    {
        bool OnItemMove(int fromPosition, int toPosition);
    }

}