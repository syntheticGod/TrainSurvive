/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/19 11:35:16
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using TTT.Utility;
using UnityEngine.UI;
using WorldMap.Model;
using System.Text;

namespace WorldMap.Controller
{
    public class ControllerMonitor : MonoBehaviour
    {
        Text coordinateText;
        private void Awake()
        {
            //右上角的坐标信息
            Image mapInfoBG = ViewTool.CreateImage("MapInfo");
            mapInfoBG.color = Color.white;
            ViewTool.SetParent(mapInfoBG, GameObject.Find("Canvas").transform);
            ViewTool.RightTop(mapInfoBG, new Vector2(1.0f, 1.0f), new Vector2(200f, 80f), new Vector2(-200f, 0f));
            coordinateText = ViewTool.CreateText("Coordinate");
            ViewTool.SetParent(coordinateText, mapInfoBG);
            ViewTool.FullFillRectTransform(coordinateText);
        }
        void Start()
        {
            if (WorldForMap.Instance.IfTeamOuting)
            {
                ControllerManager.Instance.UnfocusController("Train", "Character");
                ControllerManager.Instance.ShowController("Team", "Character");
            }
            else
            {
                ControllerManager.Instance.FocusController("Train", "Character");
                ControllerManager.Instance.HideController("Team", "Character");
            }
        }
        private void OnEnable()
        {
            Train.Instance.OnPassBlockCenter += TrainPassBlockCenterCallBack;
            Team.Instance.OnPassBlockCenter += TeamPassBlockCenterCallBack;
        }
        private void OnDisable()
        {
            Train.Instance.OnPassBlockCenter -= TrainPassBlockCenterCallBack;
            Team.Instance.OnPassBlockCenter -= TeamPassBlockCenterCallBack;
        }
        public void TrainPassBlockCenterCallBack(Vector2Int mapPosition)
        {
            coordinateText.text = string.Format("列车：({0:D},{1:D})", Train.Instance.MapPosTrain.x, Train.Instance.MapPosTrain.y);
        }
        public void TeamPassBlockCenterCallBack(Vector2Int mapPosition)
        {
            coordinateText.text = string.Format("探险队：({0:D},{1:D})", Team.Instance.MapPosTeam.x, Team.Instance.MapPosTeam.y);
        }
    }
}