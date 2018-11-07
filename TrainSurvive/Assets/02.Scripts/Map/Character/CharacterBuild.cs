/*
 * 描述：创建列车、小队等可以移动的角色
 * 作者：项叶盛
 * 创建时间：2018/11/7 17:16:42
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WorldMap
{
    public class CharacterBuild : MonoBehaviour
    {
        public float InitPositionForTrainX = 50F;
        public float InitPositionForTrainZ = 50F;
        public GameObject mapBuild;
        public GameObject trainPrefab;

        private GameObject trainObject;
        private GameObject characterObject;
        private TrainController trainController;
        void Start()
        {
            createModel();
        }

        void Update()
        {

        }
        private void createModel()
        {
            characterObject = new GameObject("Character");
            trainObject = Instantiate(trainPrefab);
            trainController = trainObject.GetComponent<TrainController>();
            trainController.init(mapBuild, new Vector2(InitPositionForTrainX, InitPositionForTrainZ));
            trainObject.transform.parent = characterObject.transform;
        }
    }
}