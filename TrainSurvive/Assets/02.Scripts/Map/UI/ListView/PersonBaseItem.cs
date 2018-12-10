/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/5 5:01:55
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace WorldMap.UI
{
    public class PersonBaseItem : BaseItem
    {
        private Text text;
        protected override void CreateModel()
        {
            text = Utility.CreateText("Name");
        }
        protected override void InitModel()
        {
            Utility.SetParent(text, this);
        }
        protected override void PlaceModel()
        {
            Utility.FullFillRectTransform(text, Vector2.zero, Vector2.zero);
        }
        public void ShowPerson(Person person)
        {
            text.text = person.name;
        }
    }
}
