/*
 * 描述：创建列车、小队等可以移动的角色
 * 作者：项叶盛
 * 创建时间：2018/11/7 17:16:42
 * 版本：v0.1
 */
using UnityEngine;
using WorldMap.Model;

namespace WorldMap
{
    public class CharacterBuild : MonoBehaviour
    {
        //public int InitIndexForXTrain = 50;
        //public int InitIndexForZTrain = 46;
        //列车的最大速度
        public float maxSpeedForTrain = 1.0F;
        //探险队的最大速度
        public float maxSpeedForTeam = 0.7F;

        private Vector2Int initIndexForTrain;
        public GameObject mapBuild;
        public GameObject trainPrefab;
        public GameObject teamPrefab;

        private GameObject trainObject;
        private GameObject teamObject;
        private GameObject characterObject;
        private Train train;
        private Team team;
        private MapGenerate mapGenerate;
        private WorldForMap world;
        public void Init(Vector2Int initIndex)
        {
            initIndexForTrain = initIndex;
        }
        private void Awake()
        {
            Debug.Log("CharacterBuild Awake");
            mapGenerate = GameObject.Find("MapBuild").GetComponent<MapGenerate>();
            FillMoreData();
            CreateModel();
        }
        void Start()
        {
            Debug.Log("CharacterBuild Start");
        }

        void Update()
        {
//测试代码，使得能够在Inspector中修改的值马上生效
#if DEBUG
            if(null != train)
                train.MaxSpeed = maxSpeedForTrain;
            if (null != team)
                team.MaxSpeed = maxSpeedForTeam;
#endif
        }
        private void CreateModel()
        {
            characterObject = new GameObject("Character");
            IMapForTrainTemp mapForTrainTemp = mapBuild.GetComponent<MapGenerate>();
            
            //设置静态数据
            StaticResource sr = StaticResource.Instance();
            sr.BlockSize = mapForTrainTemp.GetBlockSize();
            Debug.Assert(sr.BlockSize.x > 0.1 && sr.BlockSize.y > 0.1, "块大小设置的过小");
            sr.MapOrigin = mapForTrainTemp.GetMapOrigin();
            sr.MapOriginUnit = sr.MapOrigin / sr.BlockSize - new Vector2(0.5F, 0.5F);

            //列车
            train = Train.Instance;
            trainObject = Instantiate(trainPrefab);
            train.Init(true, maxSpeedForTrain, initIndexForTrain, trainObject.GetComponent<TrainController>());
            trainObject.transform.parent = characterObject.transform;

            //探险队
            team = Team.Instance;
            teamObject = Instantiate(teamPrefab);
            team.Init(teamObject.GetComponent<TeamController>());
            teamObject.SetActive(false);
            teamObject.transform.parent = characterObject.transform;
        }

        /// <summary>
        /// 填充更多的数据，例如城镇的信息
        /// </summary>
        private void FillMoreData()
        {
            world = WorldForMap.Instance;
            //TEST：加载存储生成数据
            if (mapGenerate.isCreateMap)
            {
                world.RandomTownsInfo(Map.GetIntanstance().towns);
                world.SaveGame();
            }
            world.PrepareData();
        }
    }
}