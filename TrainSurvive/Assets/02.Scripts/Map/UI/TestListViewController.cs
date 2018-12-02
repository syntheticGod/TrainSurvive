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

namespace TestWorldMap.UI
{
    public class TestListViewController : MonoBehaviour
    {
        private ListViewController listViewController;
        private List<string> texts;
        private const int textCnt = 3;
        void Awake()
        {
            listViewController = gameObject.GetComponent<ListViewController>();
            if (listViewController == null)
                listViewController = gameObject.AddComponent<ListViewController>();
            listViewController.onItemView = OnItemView;
            texts = new List<string>();
            for(int i = 0; i < textCnt; ++i)
            {
                texts.Add("hello" + i);
            }
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
            for(int i = 0; i < textCnt;i++)
                listViewController.AppendItem();
        }
        private void OnItemView(ListViewItem item, int index)
        {
            item.GetComponentInChildren<Text>().text = texts[index];
        }
    }
}
