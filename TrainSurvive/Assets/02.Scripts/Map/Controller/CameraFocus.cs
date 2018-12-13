/*
 * 描述：控制摄像机跟踪目标，
 *          该脚本应该绑定到摄像机对象上。
 * 作者：项叶盛
 * 创建时间：2018/10/31 1:54:52
 * 版本：v0.1
 */
using UnityEngine;

using TTT.Utility;

namespace WorldMap
{
    public class CameraFocus : MonoBehaviour, ICameraFocus
    {
        public float smoothTime = 0.3F;
        private float xVelocity = 0.0F;
        private float yVelocity = 0.0F;

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
        private bool ifFocused(Vector3 foscus)
        {
            //忽略y轴
            return MathTool.ApproximatelyInView(MathTool.IgnoreZ
                (transform.position), MathTool.IgnoreZ
                (foscus));
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
                if (isFirmFocus)
                    targetPosition = targetTransform.position;
                if (!ifFocused(targetPosition))
                {
                    position.x = Mathf.SmoothDamp(position.x,
                            targetPosition.x, ref xVelocity, smoothTime,Mathf.Infinity,1.0F);
                    position.y = Mathf.SmoothDamp(position.y,
                        targetPosition.y, ref yVelocity, smoothTime, Mathf.Infinity, 1.0F);
                    transform.position = position;
                    //Debug.Log("move camera to " + transform.position + " from " + position);
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