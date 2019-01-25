/*
 * 描述：物品被成功拖出的回调接口
 * 作者：项叶盛
 * 创建时间：2019/1/6 23:35:23
 * 版本：v0.7
 */
namespace TTT.UI
{
    public interface IDropMessageReceiver
    {
        void CallBackDragOut(DragableAssetsItemView item);
    }
}