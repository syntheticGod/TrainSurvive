/*
 * 描述：ListViewController的测试脚本，绑定到到测试的ListViewController游戏对象上。
 * 作者：项叶盛
 * 创建时间：2018/11/30 18:33:35
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using WorldMap.UI;
using UnityEngine.UI;

namespace TestWorldMap.UI
{
    public class TestListViewController : MonoBehaviour
    {
        private ListViewController listViewController;
        void Awake()
        {
            listViewController = gameObject.GetComponent<ListViewController>();
            if (listViewController == null)
                listViewController = gameObject.AddComponent<ListViewController>();
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
            listViewController.AppendItem().GetComponent<Text>().text = "hello1";
            listViewController.AppendItem().GetComponent<Text>().text = "hello2";
            listViewController.AppendItem();
        }
    }
}
