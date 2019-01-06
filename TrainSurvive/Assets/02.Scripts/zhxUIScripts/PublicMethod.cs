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
            for (int i = 0; i < length; ++i)
            {
                result += ((char)Random.Range(65, 81)).ToString();

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
            for (int i = 0; i < fullCountNum; ++i)
            {
                temp = new Material(id);
                temp.currPileNum = temp.maxPileNum;
                items.Add(temp);
            }
            if (num % temp.maxPileNum == 0)
            {
                return items;
            }
            temp = new Material(id);
            temp.currPileNum = num % temp.maxPileNum;
            items.Add(temp);
            return items;
        }

        private static List<Item> GenerateSpecial(int id, int num)
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
            if (id >= 0 && id <= 199)
                items.Add(GenerateWeapon(id));
            else if (id >= 200 && id <= 699)
            {
                items = GenerateMaterial(id, num);
            }
            else if (id >= 700 && id <= 999)
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
        /// <summary>
        /// 判断仓库中指定ID的物体的数量是否大于number。
        /// </summary>
        /// <param name="itemID">物体ID</param>
        /// <param name="number">数量</param>
        /// <returns></returns>
        private static bool CanConsumeItem(int itemID, int number)
        {
            return World.getInstance().storage.GetNumberByID(itemID) >= number;
        }
        /// <summary>
        /// 移出仓库中指定ID的物品
        /// </summary>
        /// <param name="itemID">物品ID</param>
        /// <param name="number">数量</param>
        /// <returns>
        /// TRUE：删除成功
        /// FALSE：仓库物品不足够，取消删除
        /// </returns>
        private static bool ConsumeItem(int itemID, int number)
        {
            return World.getInstance().storage.RemoveItem(itemID, number);
        }
        /// <summary>
        /// 从仓库中一起移除物品列表中所有的物品
        /// </summary>
        /// <param name="consumeList">物品列表</param>
        /// <returns>
        /// TRUE：删除成功
        /// FALSE：其中一个物品不够充足，取消全部删除
        /// </returns>
        public static bool ConsumeItems(ItemData[] consumeList)    //测试成功
        {
            if (!IfHaveEnoughItems(consumeList))
                return false;
            foreach(ItemData item in consumeList)
                ConsumeItem(item.ID, item.Number);
            return true;
        }
        /// <summary>
        /// 判断仓库中是否有充足的物品
        /// </summary>
        /// <param name="consumeList">物品列表</param>
        /// <returns>
        /// TRUE：有足够的物品
        /// FALSE：没有足够的物品
        /// </returns>
        public static bool IfHaveEnoughItems(ItemData[] consumeList)    //测试成功
        {
            for (int i = 0; i < consumeList.Length; ++i)
            {
                if (!CanConsumeItem(consumeList[i].ID, consumeList[i].Number))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 往仓库中添加指定物品
        /// </summary>
        /// <param name="item">物品</param>
        public static void AppendAItem(ItemData item)
        {
            World.getInstance().storage.AddItem(item);
        }
        /// <summary>
        /// 往仓库中添加指定物品
        /// 无限仓库
        /// </summary>
        /// <param name="appendList">物品列表</param>
        /// <returns>
        /// </returns>
        public static void AppendItemsInBackEnd(ItemData[] appendList)
        {
            foreach (ItemData itemData in appendList)
            {
                World.getInstance().storage.AddItem(itemData);
            }
        }
    }
}
