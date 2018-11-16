/*
 * 描述：
 * 作者：����
 * 创建时间：2018/11/6 23:29:41
 * 版本：v0.1
 */
/*
 * 描述：物品栏类——所有管理物品的对象由其生成（入资源采集队伍的物品记录）
 * 作者：张皓翔
 * 创建时间：2018/11/1 14:39:45
 * 版本：v0.1
 */
/*
 * 描述：物品栏（后台控制区）
 * 作者：张皓翔
 * 创建时间：2018/11/1 14:30:00
 * 版本：v0.1
 * 
 * 注释：该区需要用前台控制区调用并在前台控制显示，并完成增删排的对接，其他具有存储物品集合性质的对象可用该类生存（如探险队背包）
 *       增加物品的情况：1.增加一单位物品，需要查找同ID物品进行堆叠，若堆叠已满则新建一个Item
 *                      2.增加若干单位物品，查找同ID物品进行堆叠，若堆叠不下，则查找下一个可堆叠物品，直至剩余物品单独形成一个堆叠
 *                      3.在物品拖拽的时候，原来的物品是要Remove的，不然会在同一个Inventory拖拽下出现物品多出来的BUG
 */

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._02.Scripts.zhxUIScripts
{
    public class Inventory
    {
        //  封装字段  -----------------------------
        private float _max_size;
        private float _curr_size;
        private List<Item> _items;
        private InventoryCtrl controller;
        //  属性  ---------------------------------
        public float maxSize
        {
            get
            {
                return _max_size;
            }
        }
        public float currSize
        {
            get
            {
                return _curr_size;
            }
        }
        public List<Item> items
        {
            get
            {
                return _items;
            }
        }

        //  构造  ---------------------------------
        public Inventory(float maxsize, InventoryCtrl _controler = null)
        {
            
            _max_size = maxsize;
            _curr_size = 0f;
            _items = new List<Item>();
            controller = _controler;
        }
        //  方法  ---------------------------------

        public void SetMaxSize(float size)
        {
            _max_size = size;
        }

        public void PushItemWithNoGrid(Item item)
        {
            items.Add(item);
        }


        public int PushItem(Item item)                      //增加物品、自动堆叠并返回放不下的该物品数
        {
            
            int itemId = item.id;
            int itemPileNum = item.currPileNum;
            float restSize = _max_size - _curr_size;
            int restNum = 0;
            int allowNum = itemPileNum;
            int index;
            Item mappingItem = null;
            List<int> existIndex = new List<int>();
            List<GameObject> itemGridInst = controller.itemGridInst;
            if (restSize < item.currPileNum * item.size)    //剩余空间放不下物品
            {
                allowNum = (int)(restSize / item.size);
                restNum = itemPileNum - allowNum;
            }
            _curr_size += item.size * allowNum;
            if(allowNum == 0)                               //一个物品都放不下就直接原路返回
            {
                return restNum;
            }

            for(int i=0; i<itemGridInst.Count; ++i)         //找出所有同物品所在实例Grid的下标
            {
                if(itemGridInst[i].GetComponent<ItemGridCtrl>().item.id == item.id)
                {
                    existIndex.Add(i);
                }
            }

            for(int i=0; i<existIndex.Count; ++i)
            {
                index = existIndex[i];   
                if(items[index].maxPileNum - items[index].currPileNum >= allowNum)
                {
                    items[index].currPileNum += allowNum;
                    allowNum = 0;   //意为不用新建Grid存放多余物品
                    break;
                }
                else
                {
                    allowNum -= (items[index].maxPileNum - items[index].currPileNum);
                    items[index].currPileNum = items[index].maxPileNum;
                }
            }
            if(allowNum != 0)
            {
                mappingItem = item.Clone();                //复制的时候除了ID其他信息都丢失了，有待处理。
                mappingItem.currPileNum = allowNum;
                _items.Add(mappingItem);
                if (controller != null)
                {
                    controller.AddGrid(mappingItem);                 //为前台添加物品
                }
            }
            controller.ReFreshShowGrid();
            controller.ReFreshMaxSize();
            return restNum;
        }

        public int PushItemToLast(Item item)//已同步
        {
            float restSize = maxSize - currSize;
            int allowNum = item.currPileNum;
            int restNum = 0;
            if(restSize < item.size * item.currPileNum)
            {
                allowNum = (int)(restSize / item.size);
                restNum = item.currPileNum - allowNum;
            }
            if(allowNum == 0)
            {
                return item.currPileNum;
            }
            Item mappingItem;
            mappingItem = item.Clone();
           
            mappingItem.currPileNum = allowNum;
            _items.Add(mappingItem);
            _curr_size += allowNum * item.size;
            
            if (controller != null)
            {
                controller.AddGrid(mappingItem);                 //为前台添加物品
            }
            controller.ReFreshMaxSize();
            return restNum;
        }



        public void PopItem(Item item)                          //弹出所有物体
        {
            if (!_items.Remove(item))
            {
                Debug.Log("不存在该物品，弹出失败！");
            }
            else
            {
                _curr_size -= item.size * item.currPileNum;
            }
        }

        public void PopItem(Item item, int num)                 //弹出部分物体
        {
            item.currPileNum -= num;
            _curr_size -= item.size * num;
        }
    }
}
