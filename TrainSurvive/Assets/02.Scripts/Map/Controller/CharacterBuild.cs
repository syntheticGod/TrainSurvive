/*
 * 描述：创建列车、小队等可以移动的角色
 * 作者：项叶盛
 * 创建时间：2018/11/7 17:16:42
 * 版本：v0.1
 */
using UnityEngine;

using WorldMap.Model;
using WorldMap.Controller;

using TTT.Resource;
using TTT.Utility;
using TTT.Team;

namespace WorldMap
{
    public class CharacterBuild : MonoBehaviour
    {
        //列车的最大速度
        public float maxSpeedForTrain = 1.0F;
        //探险队的最大速度
        public float maxSpeedForTeam = 0.7F;

        public GameObject mapBuild;

        private GameObject characterObject;
        private void Awake()
        {
            Debug.Log("CharacterBuild Awake");
            FillMoreData();
            CreateModel();
        }
        void Start()
        {
            Debug.Log("CharacterBuild Start");
            Train.Instance.Init();
            Team.Instance.Init();
        }
        /// <summary>
        /// 填充更多的数据，例如城镇的信息
        /// </summary>
        private void FillMoreData()
        {
            MapGenerate mapGenerate = GameObject.Find("MapBuild").GetComponent<MapGenerate>();
            if (mapGenerate.isCreateMap)
            {
                //设置人物出生地点为第一个城镇
                World.getInstance().PMarker.TrainMapPos = Map.GetInstance().towns[0, 0].position;
                //初始化城镇数据
                World.getInstance().Towns.Init(Map.GetInstance().towns);
                World.getInstance().Npcs.Init();
                World.getInstance().Dialogues.InitOnce();
                //初始化档案时随机生成3个人物
                World.getInstance().numIn = 3;
                for (int i = 0; i < 3; i++)
                {
                    Person person = Person.RandomPerson(1000 + i);
                    //默认全部出战，直到上限
                    if (i < PersonSet.MAX_NUMBER_FIGHER)
                        person.ifReadyForFighting = true;
                    World.getInstance().Persons.Add(person);
                }
                World.getInstance().save();
            }
            Debug.Log(World.getInstance().Npcs.ToString());
        }
        private void CreateModel()
        {
            characterObject = new GameObject("Character");
            MapGenerate map = mapBuild.GetComponent<MapGenerate>();

            //设置静态数据
            Debug.Assert(StaticResource.BlockSize.x > 0.1 && StaticResource.BlockSize.y > 0.1, "块大小设置的过小");

            //列车
            Train.Instance.Config(World.getInstance().PMarker.TrainMapPos, movable: true, maxSpeed: maxSpeedForTrain);
            GOTool.ForceGetComponentInChildren<TrainController>(characterObject, "Train", true).PathOfTransform = "/Character/Train";
            //探险队
            Team.Instance.Config(World.getInstance().PMarker.TeamMapPos);
            GOTool.ForceGetComponentInChildren<TeamController>(characterObject, "Team", false).PathOfTransform = "/Character/Team";
        }
    }
}