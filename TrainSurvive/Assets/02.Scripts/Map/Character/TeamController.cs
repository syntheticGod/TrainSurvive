/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/10 16:06:56
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WorldMap
{
    public class TeamController : MonoBehaviour, OnClickListener
    {
        //小队数据结构
        private Team team;
        //默认一小队人数
        public int defaultPeople = 10;
        private void Init(int people)
        {
            team = new Team(people);
            ButtonHandler.Instance.AddListeners(this);
        }
        void Start()
        {
            Init(defaultPeople);
        }

        void Update()
        {

        }
        public void OnClick(BUTTON_ID id)
        {
            if (!ButtonHandler.IsTeam(id))
                return;
        }
    }
}