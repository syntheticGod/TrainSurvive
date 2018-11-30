/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/27 1:38:59
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace WorldMap.UI
{
    public class ListLayoutGroup : GridLayoutGroup
    {
        private List<Item> items;
        protected override void Awake()
        {
            base.Awake();
            base.startAxis = Axis.Horizontal;
            base.constraint = Constraint.FixedRowCount;
            base.m_ConstraintCount = 1;
            base.spacing = new Vector2(6, 6);
        }
        public void Init(List<Person> persons, GameObject personProfile, TeamOutPrepareDialog dialog)
        {
            if(items == null)
                items = new List<Item>();
            int i;
            for(i = 0; i < persons.Count; ++i)
            {
                Item item ;
                if (i >= items.Count)
                {
                    item = Instantiate(personProfile).AddComponent<Item>();
                    Append(item);
                }
                else
                    item = items[i];

                item.SetDialog(dialog);
                item.Person = persons[i];
            }
            int removeStart = i;
            int removeCount = items.Count  - removeStart;
            for (; i < items.Count; ++i)
            {
                Destroy(items[i]);
            }
            items.RemoveRange(removeStart, removeCount);
        }
        public void Append(Item item)
        {
            items.Add(item);
            item.transform.SetParent(transform, false);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;
            item.transform.localEulerAngles = Vector3.zero;
        }
        public bool Detach(Item item)
        {
            if (!items.Contains(item))
                return false;
            items.Remove(item);
            return true;
        }
        public List<Person> GetData()
        {
            List<Person> ret = new List<Person>();
            foreach(Item item in items)
            {
                ret.Add(item.Person);
            }
            return ret;
        }
    }
    public class Item : MonoBehaviour, IPointerClickHandler
    {
        private TeamOutPrepareDialog dialog;
        public Person Person { get; set; }
        private Text text;
        void Awake()
        {
            text = transform.GetComponent<Text>();
        }
        void Start()
        {
            text.text = Person.name;
        }
        public void SetDialog(TeamOutPrepareDialog dialog)
        {
            this.dialog = dialog;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                dialog.OnItemClick(this);
            }
        }
    }
}