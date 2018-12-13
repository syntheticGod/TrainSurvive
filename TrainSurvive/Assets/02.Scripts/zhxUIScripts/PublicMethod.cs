/*
 * 描述：
 * 作者：����
 * 创建时间：2018/11/6 23:29:41
 * 版本：v0.1
 */
/*
 * 描述：公有方法、一般放置常用的过程处理函数
 * 作者：张皓翔
 * 创建时间：2018/10/31 20:21:23
 * 版本：v0.1
 */

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

namespace Assets._02.Scripts.zhxUIScripts
{
    public static class PublicMethod
    {
        public static string GenerateRdString(int length)                   //生成某个长度的随机字符串
        {
            string result = "";   
            for(int i=0; i<length; ++i)
            {
                result += ((char)Random.Range(65,81)).ToString();
                
            }
            return result;
        }

        public static void Useless()                                        //回调函数测试占位变量
        {
            Debug.Log("Test");
        }

        private static Item GenerateWeapon(int id)
        {
            Item result = new Weapon(id);
            return result;
        }

        private static List<Item> GenerateMaterial(int id, int num)
        {
            List<Item> items = new List<Item>();
            Item temp = new Material(id);
            int fullCountNum = num / temp.maxPileNum;
            for(int i=0; i<fullCountNum; ++i)
            {
                temp = new Material(id);
                temp.currPileNum = temp.maxPileNum;
                items.Add(temp);
            }
            if(num % temp.maxPileNum == 0)
            {
                return items;
            }
            temp = new Material(id);
            temp.currPileNum = num % temp.maxPileNum;
            items.Add(temp);
            return items;
        }

        private static List<Item> GenerateSpecial(int id,int num)
        {
            List<Item> items = new List<Item>();
            Item temp = new SpecialItem(id);
            int fullCountNum = num / temp.maxPileNum;
            for (int i = 0; i < fullCountNum; ++i)
            {
                temp = new SpecialItem(id);
                temp.currPileNum = temp.maxPileNum;
                items.Add(temp);
            }
            if (num % temp.maxPileNum == 0)
            {
                return items;
            }
            temp = new SpecialItem(id);
            temp.currPileNum = num % temp.maxPileNum;
            items.Add(temp);
            return items;
        }

        public static Item[] GenerateItem(int id, int num = 1)                  //当传入num大于堆叠数量时，会将多个实例装入数组返回
        {
            List<Item> items = new List<Item>();
            if(id >= 0 && id <= 199)
                items.Add(GenerateWeapon(id));
            else if(id >= 200 && id <= 699)
            {
                items = GenerateMaterial(id, num);
            }
            else if(id >= 700 && id <= 999)
            {
                items = GenerateSpecial(id, num);
            }
            else
            {
                Debug.Log("id参数非法");
                return null;
            }
            //List<Item> items = new List<Item>();
            //Item temp = new Material(id);
            //int FullCountNum = num / temp.maxPileNum;
            //for(int i=0; i<FullCountNum; ++i)
            //{
            //    temp = new Material(id);
            //    temp.currPileNum = temp.maxPileNum;
            //    items.Add(temp);
            //}
            //temp = new Material(id);
            //int left = num % temp.maxPileNum;
            //temp.currPileNum = left;
            //items.Add(temp);
            //return items.ToArray();
            return items.ToArray();
        }

        private static bool CanConsumeItem(ItemData itemData)
        {
            int totalNum = 0;
            List<ItemData> itemDataInTrain = World.getInstance().itemDataInTrain;
            for (int i=0; i< itemDataInTrain.Count; ++i)
            {   //算出目标物品在仓库内共有几个
                if(itemDataInTrain[i].id == itemData.id)
                {
                    totalNum += itemDataInTrain[i].num;
                }
            }
            if(totalNum >= itemData.num)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void ConsumeItem(ItemData itemData)
        {
            List<ItemData> itemDataInTrain = World.getInstance().itemDataInTrain;
            List<int> removeIndex = new List<int>();
            int alreadyConsumeNum = 0;
            for(int i=itemDataInTrain.Count-1; i>=0; --i)
            {
                if(itemDataInTrain[i].id == itemData.id)
                {
                    if(itemData.num - alreadyConsumeNum <= itemDataInTrain[i].num)
                    {
                        itemDataInTrain[i].num -= (itemData.num - alreadyConsumeNum);
                        if(itemDataInTrain[i].num == 0)
                        {
                            removeIndex.Add(i);
                        }
                        break;
                    }
                    else
                    {
                        removeIndex.Add(i);
                        alreadyConsumeNum += itemDataInTrain[i].num;
                    }
                }
            }
            foreach (int index in removeIndex)
            {
                itemDataInTrain.RemoveAt(index);
            }
        }

        public static bool ConsumeItems(ItemData[] consumeList)    //测试成功
        {
            if (!CanConsumeItems(consumeList)) {              //不能和消耗函数结合一起进行判断，因为一旦消耗掉前面的部分，后部分若不够则无法撤回
                return false;
            }
            for(int i=0; i< consumeList.Length; ++i)
            {
                ConsumeItem(consumeList[i]);
            }
            return true;
        }

        public static bool CanConsumeItems(ItemData[] consumeList)    //测试成功
        {
            for (int i = 0; i < consumeList.Length; ++i)
            {
                if (!CanConsumeItem(consumeList[i])) {
                    return false;
                }
            }
            return true;
        }

        private static void appendAItem(ItemData item)
        {
            List<int> indexes = new List<int>();
            int max = World.getInstance().itemDataInTrain.Count;
            for (int i=0; i<max; ++i)
            {
                if (World.getInstance().itemDataInTrain[i].id == item.id)
                    indexes.Add(i);
            }
         
            for (int i = 0; i<indexes.Count; ++i)
            {
                int index = indexes[i];
                Item aimItem = World.getInstance().itemDataInTrain[index].item;
                
                if (aimItem.maxPileNum - aimItem.currPileNum >= item.num)
                {
                    World.getInstance().itemDataInTrain[index].num += item.num;
                    return;
                }
                else
                {
                    World.getInstance().itemDataInTrain[index].num = aimItem.maxPileNum;
                    item.num -= aimItem.maxPileNum - aimItem.currPileNum;
                }
            }
            World.getInstance().itemDataInTrain.Add(item);  
        }

        public static bool AppendItemsInBackEnd(ItemData[] appendList)
        {
            float appendSize = 0f;
            foreach(ItemData itemData in appendList)
            {
                Item[] tempItem = GenerateItem(itemData.id, itemData.num);
                for(int i=0; i<tempItem.Length; ++i)
                {
                    appendSize += tempItem[i].size;
                }
            }
            if(appendSize > World.getInstance().trainInventoryMaxSize - World.getInstance().trainInventoryCurSize)
            {
                return false;
            }
            else
            {
                foreach (ItemData itemData in appendList)
                {
                    Item[] tempItem = GenerateItem(itemData.id, itemData.num);
                    for (int i = 0; i < tempItem.Length; ++i)
                    {
                        World.getInstance().trainInventoryCurSize += tempItem[i].size * tempItem[i].currPileNum;
                        appendAItem(itemData);
                    }
                }
            }
            
            return true;
        }
    }
}
