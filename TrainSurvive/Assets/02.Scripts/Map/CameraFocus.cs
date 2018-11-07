/*
 * 描述：控制摄像机跟踪目标
 * 作者：项叶盛
 * 创建时间：2018/10/31 1:54:52
 * 版本：v0.1
 */
using UnityEngine;
namespace worldname
{
    public class CameraFocus : MonoBehaviour, ICameraFocus
    {
        public float smoothTime = 0.3F;
        private float xVelocity = 0.0F;
        private float zVelocity = 0.0F;

        //开关摄像机焦聚
        private bool enableFocus = false;
        //焦聚过程中，是否继续焦聚（软焦聚会用到）
        private bool isContinueFocus = true;
        //是否是硬焦聚（硬焦聚会用到）
        private bool isFirmFocus;
        //焦聚目标 只能用于焦距一次
        private Vector3 targetPosition;
        //焦距目标的Tansform，用于锁定
        private Transform targetTransform;
        //软焦聚，摄像机在到达目的地后停止焦聚
        public void focusOnce(Transform t)
        {
            enableFocus = true;
            isContinueFocus = true;
            isFirmFocus = false;
            targetPosition = t.position;
        }
        //硬焦聚，摄像机一直跟踪着
        public void focusLock(Transform t)
        {
            enableFocus = true;
            isContinueFocus = true;
            isFirmFocus = true;
            targetTransform = t;
        }
        private bool ifFocused(Transform t)
        {
            return MathUtilsByXYS.ifCloselyXZ(transform.position,
                t.position);
        }

        void Start()
        {

        }
        void Update()
        {
            //摄像机焦聚
            if (enableFocus && isContinueFocus)
            {
               
                Vector3 position = transform.position;
                if (!MathUtilsByXYS.ifCloselyXZ(ref position, ref targetPosition))
                {
                    if (isFirmFocus)
                    {
                        transform.position = MathUtilsByXYS.goStraightSmoothlyXZ(
                            position, targetTransform.position, smoothTime,
                            ref xVelocity, ref zVelocity);
                    }
                    else
                    {
                        MathUtilsByXYS.goStraightSmoothlyXZ(ref position,
                                targetPosition, smoothTime,
                                ref xVelocity, ref zVelocity);
                        transform.position = position;
                    }
                }
                else
                {
                    //只有软焦聚时才会在最后停止
                    isContinueFocus = isFirmFocus;
                }   
            }
        }
    }
}