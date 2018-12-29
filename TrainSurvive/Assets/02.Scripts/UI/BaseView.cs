/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/26 21:41:50
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TTT.Utility;

namespace TTT.UI
{
    public abstract class BaseView : MonoBehaviour
    {
        Image backgroud;
        private void Awake()
        {
            backgroud = CompTool.ForceGetComponent<Image>(this);
            CreateView();
        }
        public void SetBackgroundColor(Color color)
        {
            backgroud.color = color;
        }
        /// <summary>
        /// 视图内容
        /// </summary>
        public abstract void CreateView();
    }
}
