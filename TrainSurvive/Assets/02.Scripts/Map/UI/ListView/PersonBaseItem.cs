/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/5 5:01:55
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using TTT.Utility;

namespace WorldMap.UI
{
    public class PersonBaseItem : BaseItem
    {
        private Text text;
        protected override void CreateModel()
        {
            text = ViewTool.CreateText("Name");
        }
        protected override void InitModel()
        {
            ViewTool.SetParent(text, this);
        }
        protected override void PlaceModel()
        {
            ViewTool.FullFillRectTransform(text, Vector2.zero, Vector2.zero);
        }
        public void ShowPerson(Person person)
        {
            text.text = person.name;
        }
    }
}
