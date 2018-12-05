/*
 * 描述：ListViewController的测试脚本，绑定到到测试的ListViewController游戏对象上。
 * 作者：项叶盛
 * 创建时间：2018/11/30 18:33:35
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections.Generic;
using WorldMap.UI;
using UnityEngine.UI;
using WorldMap.Model;

namespace TestWorldMap.UI
{
    public class TestListViewController : MonoBehaviour
    {
        private HeroListView listViewController;
        private const int textCnt = 3;
        void Awake()
        {
            listViewController = gameObject.GetComponent<HeroListView>();
            if (listViewController == null)
                listViewController = gameObject.AddComponent<HeroListView>();
            listViewController.Datas = new List<Person>();
        }
        void Start()
        {
            TestAppendItem();
        }

        void Update()
        {

        }
        private void TestAppendItem()
        {
            listViewController.AddItem(NPC.Random().PersonInfo);
            listViewController.AddItem(NPC.Random().PersonInfo);
            listViewController.AddItem(NPC.Random().PersonInfo);
            listViewController.AddItem(NPC.Random().PersonInfo);
        }
    }
}
