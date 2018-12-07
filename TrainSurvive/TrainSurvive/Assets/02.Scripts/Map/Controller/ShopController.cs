/*
 * 描述：商店界面控制类
 * 作者：项叶盛
 * 创建时间：2018/12/1 12:30:05
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections.Generic;
using WorldMap.UI;
using Assets._02.Scripts.zhxUIScripts;

namespace WorldMap
{
    public class ShopController : MonoBehaviour
    {
        private ListViewController goodsInShopLV;
        private ListViewController goodsInPackLV;
        private List<Model.Good> goodsInShop;
        private List<Model.Good> goodsInPack;
        //private InventoryCtrl goodsInShopIC;
        //private InventoryCtrl goodsInPackIC;
        private WorldForMap world;
        void Awake()
        {
            ListViewController[] ls = GetComponentsInChildren<ListViewController>();
            goodsInShopLV = ls[0];
            goodsInShopLV.SetContent(typeof(GoodsItem));
            goodsInPackLV = ls[1];
            goodsInPackLV.SetContent(typeof(GoodsItem));
            goodsInShopLV.onItemView = delegate (ListViewItem item, int index)
            {
                Model.Good data = goodsInShop[index];
                GoodsItem view = item.GetComponentInChildren<GoodsItem>();
                view.SetTargetByName("Weapon_img");
                view.SetNumber(data.item.currPileNum);
                view.SetPrice(data.Price);
            };
            goodsInPackLV.onItemView = delegate (ListViewItem item, int index)
            {

            };
            goodsInPack = new List<Model.Good>();
        }
        void Start()
        {
        }
        void Update()
        { }
        public void Init()
        {
            world = WorldForMap.Instance;
        }
        public void Show(Model.Town town)
        {
            if (gameObject.activeInHierarchy)
            {
                Debug.Log("商店已显示");
                return;
            }
            gameObject.SetActive(true);
            goodsInShop = town.Goods;
            goodsInShopLV.RemoveAllItem();
            for (int i = 0; i < goodsInShop.Count; ++i)
                goodsInShopLV.AppendItem();
            ////探险队访问商店
            //if (world.IfTeamOuting)
            //{
            //}
            ////列车访问商店
            //else
            //{

            //}
        }
        private void Hide()
        {
            if (gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
            }
        }
        public void OnBtnClick(int id)
        {
            switch (id)
            {
                case 0://购买
                    break;
                case 1://取消
                    Hide();
                    break;
                default:
                    break;
            }
        }
    }
}