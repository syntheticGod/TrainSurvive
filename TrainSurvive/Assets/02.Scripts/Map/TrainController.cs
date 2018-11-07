/*
 * 描述：玩家控制列车移动的控制器。
 * 作者：项叶盛
 * 创建时间：2018/10/31 0:29:36
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace worldname
{
    public class TrainController : MonoBehaviour
    {
        public int initPositionX = 50, initPositionZ = 50;
        //列车移动一格所花的时间，现实中的时间。单位：秒
        public int timePerStepTrain = 1800;//0.5小时
        public float smoothTime = 0.3F;

        //地图信息
        private Vector2 blockSize;            //readonly
        private Vector2 mapOrigin;          //readonly
        private Vector2 mapOriginUnit;    //readonly

        //移动相关
        private Vector3 targetPosition;
        //private List<Vector2> tartgetPositions;
        private bool enableMove;
        private bool isMoving;
        private float xVelocity = 0.0F;
        private float zVelocity = 0.0F;

        //传入的游戏对象或脚本
        public GameObject mainCamera;
        private IMapForTrain iMapForTrain;
        private ICameraFocus cameraFocus;

        void Start()
        {
            iMapForTrain = transform.GetComponentInParent<IMapForTrain>();
            transform.position = new Vector3(initPositionX, transform.position.y,
                initPositionZ);
            cameraFocus = mainCamera.GetComponent<ICameraFocus>();
            cameraFocus.focusLock(transform);
            //FOR TEST : 新建一个测试用的Map
            iMapForTrain = new MapForTest();
            //防止Block大小过小，导致除法错误
            blockSize = iMapForTrain.getBlockSize();
            if (blockSize.x < 0.1 || blockSize.y < 0.1)
            {
                throw new System.Exception("块大小设置的过小");
            }
            mapOrigin = iMapForTrain.getMapOrigin();
            mapOriginUnit = mapOrigin / blockSize + new Vector2(0.5F, 0.5F);
            targetPosition = new Vector3(0, transform.position.y, 0);

        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                Ray ray = mainCamera.GetComponent<Camera>()
                    .ScreenPointToRay(Input.mousePosition);
                //因为摄像机的Projection 为 Orthographic，所以Ray的方向都是平行的
                Debug.Log("origin of ray:" + ray.origin + " dire:" + ray.direction);
                Debug.Log("mouse position " + Input.mousePosition);

                Vector2 clickedPosition = new Vector2
                {
                    x = ray.origin.x,
                    y = ray.origin.z
                };
                Vector2Int clickBlockIndex = whereTheClickedBlock(clickedPosition);
                //判断点击处是否是铁轨
                if (iMapForTrain.ifRail(clickBlockIndex))
                {
                    moveTrainTo(calBlockCenterByIndex(clickBlockIndex));
                }
            }
            //列车移动判断
            if (enableMove && isMoving)
            {
                Vector3 position = transform.position;
                if (!MathUtilsByXYS.ifCloselyXZ(ref position, ref targetPosition))
                {
                    MathUtilsByXYS.goStraightSmoothlyXZ(ref position,
                     targetPosition, smoothTime, ref xVelocity,
                     ref zVelocity);
                    transform.position = position;
                }
                else
                {
                    isMoving = false;
                }
            }
        }
        private Vector2Int whereTheClickedBlock(Vector2 clickedPosition)
        {
            //因为块中心位于原点坐标处，所以要减去blockSize/2
            //(clickedPosition - worldMap.getMapOrigin() - blockSize / 2) / blockSize;
            //公式优化如下
            //clicckedPosition/blockSize - ((mapOrigin + blockSize/2)/blockSize)
            Vector2 index2F = clickedPosition / blockSize - mapOriginUnit;
            return new Vector2Int(Mathf.CeilToInt(index2F.x), Mathf.CeilToInt(index2F.y));
        }
        private Vector2 calBlockCenterByIndex(Vector2Int index2d)
        {
            //索引 * 块大小 = 原点到块中心的向量
            //再加上 原块的中心坐标 就是世界坐标
            return index2d * blockSize + mapOrigin;
        }
        private void moveTrainTo(Vector2 position2d)
        {
            targetPosition.x = position2d.x;
            targetPosition.z = position2d.y;
            enableMove = true;
            isMoving = true;
        }
    }
}
