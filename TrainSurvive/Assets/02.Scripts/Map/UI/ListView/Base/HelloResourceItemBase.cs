/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/14 21:48:22
 * 版本：v0.1
 */

namespace WorldMap.UI
{
    public class HelloResourceItemBase : AssetsItemView
    {
        protected override void InitModel()
        {
            base.InitModel();
            SetMarkLevel(4);
            SetTargetByName("Weapon_img");
            SetNumber(100);
        }
    }
}
