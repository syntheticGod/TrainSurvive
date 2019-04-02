using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  移动动画与攻击动画的程序实现相关
//  版本：实装版 1.0
//  日期：4.2
//  比起测试版，去掉了posX与移速相关，只看posY位移与posDeltaX
namespace WorldBattle
{
    public class animAchieve : MonoBehaviour
    {
        public int state; //0静止 1移动 2攻击 3等待攻击
        public int upOrDown; //跳跃状态细节 0静止 1上升 2下降
        public int outOrIn;  //撞击状态细节 0静止 1向外撞 2回内
        public int movDirect = 1; //前进方向 1前进 -1后退
        public float posRootX;      //parent的x
        public float posY = 0;
        public float posDeltaX = 0;
        public float spdJmp = 5;    //跳跃速度
        public float spdHitOut; //撞出速度
        public float spdHitBac;     //回弹速度
        public float widHit=1;        //最大攻击位移
        public float heiJmp = 0.5f; //最大跳跃高度
        public float timeHitNow = 0;
        public float timeHitMax = 1; //攻击间隔
        public float timeHitting;    //攻击动画所需时间=前摇时间
        public float spdRotate = 90; //攻击旋转速度
        public bool flagWait = false;   //用于【将其他状态平滑转换到静止状态】的标志位 
        public bool flagAttack = false; //用于【将其他状态平滑转换到攻击状态】的标志位 
        public bool flagMove = false;
        public BattleActor parentComponentAI;
        // Start is called before the first frame update
        void Start()
        {
            Reset();
        }

        //获取角色的逻辑参数,即时更新本地
        private void updateLogicParameter()
        {
            //先捕捉parent的AI组件，可能有更好的代码写法
            if (GetComponentInParent<PersonAI>())
                parentComponentAI = GetComponentInParent<PersonAI>();
            else if (GetComponentInParent<EnemyAI>())
                parentComponentAI = GetComponentInParent<EnemyAI>();
            else if (GetComponentInParent<type1AI>())
                parentComponentAI = GetComponentInParent<type1AI>();
            else if (GetComponentInParent<type2AI>())
                parentComponentAI = GetComponentInParent<type2AI>();
            else if (GetComponentInParent<type3AI>())
                parentComponentAI = GetComponentInParent<type3AI>();
            else if (GetComponentInParent<type4AI>())
                parentComponentAI = GetComponentInParent<type4AI>();
            else Debug.Log("无法捕捉AI组件！");


            if (parentComponentAI)
            {

                //更新攻击间隔
                timeHitMax = parentComponentAI.atkNeedTime;
                //更新攻击出击所需时间（等同于旧版的攻击动画播放时间，这里分配撞出时间：回弹时间=1:3
                timeHitting = 0.4f * timeHitMax * 1 / 4;
                spdHitOut = widHit / timeHitting;
                spdHitBac = spdHitOut / 3;

                //更新面向
                movDirect = (GetComponentInParent<Transform>().rotation.y == 180) ? -1 : 1;

                //即时切换状态;
                //Debug.Log("判断状态切换");
                switch (parentComponentAI.subStateController.curActionState)
                {

                    case BattleActor.ActionStateEnum.MOTION:
                        if (state != 1)
                        {
                            Debug.Log("切换到移动动画");
                            flagMove = true;
                            flagWait = false;
                            flagAttack = false;
                        }
                        break;
                    case BattleActor.ActionStateEnum.ATTACK:
                        if (state < 2)
                        {
                            Debug.Log("切换到攻击动画");
                            flagAttack = true;
                            flagMove = false;
                            flagWait = false;
                        }
                        break;
                    case BattleActor.ActionStateEnum.REST:
                        if (state == 0)
                        {
                            Debug.Log("切换到静止动画");
                            flagWait = true;
                            flagAttack = false;
                            flagMove = false;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        private void Reset()
        {
            flagWait = false;
            flagAttack = false;
            flagMove = false;
            state = 0;
            upOrDown = 0;
            outOrIn = 0;
            this.transform.position = new Vector3(0, 0, 0);
            posY = 0;
            posDeltaX = 0;
            movDirect = 1;
        }

        // Update is called once per frame
        void Update()
        {
            updateLogicParameter();
            posRootX = 0;
            posRootX = GetComponentInParent<Transform>().position.x;
            //posY = this.transform.position.y;
            //---=-=-=-==-=
        

            //正处于移动状态
            if (state == 1)
            {
                // 不需要自己移动了，跟随parent移动
                // posX += spdMov * Time.deltaTime * movDirect;
                if (upOrDown == 0)
                    upOrDown = 1;
                //上升过程
                else if (upOrDown == 1)
                {
                    posY += spdJmp * Time.deltaTime;
                    //到顶了
                    if (posY >=  heiJmp)
                    {
                        posY = heiJmp;    //溢出修正
                        upOrDown = 2;
                    }
                }
                //下落过程
                else if (upOrDown == 2)
                {
                    posY -= spdJmp * Time.deltaTime;
                    //落地了
                    if (posY <= 0)
                    {
                        upOrDown = 1;
                        posY = 0;    //溢出修正
                    }
                }
            }

            //攻击状态
            if (state == 2)
            {
                if (outOrIn == 0)
                    outOrIn = 1;
                //撞出过程
                else if (outOrIn == 1)
                {
                    Debug.Log("outing");
                    posDeltaX += spdHitOut * Time.deltaTime * movDirect;
                    //到顶了
                    if (movDirect * posDeltaX >= widHit)
                    {
                        Debug.Log("outMax");
                        posDeltaX = widHit * movDirect;    //溢出修正
                        outOrIn = 2;
                    }
                }
                //回复过程
                else if (outOrIn == 2)
                {
                    Debug.Log("backing");
                    posDeltaX -= spdHitBac * Time.deltaTime * movDirect;
                    //回来了
                    if (posDeltaX * movDirect <= 0)
                    {
                        Debug.Log("backMax");
                        outOrIn = 1;
                        posDeltaX = 0;      //溢出修正
                        state = 3;
                        timeHitNow = 0;
                    }
                }
            }

            //攻击等待状态
            if (state == 3)
            {
                timeHitNow += Time.deltaTime;
                if (timeHitNow >= timeHitMax)
                {
                    state = 2;
                    timeHitNow -= timeHitMax;
                }
            }

            //完成下落动作
            //用于移动时还没落地就被打断切换成了其他状态
            if(state!=1 && posY > 0)
            {
                posY -= spdJmp * Time.deltaTime;
                //落地了
                if (posY <= 0)
                {
                    upOrDown = 1;
                    posY = 0;    //溢出修正
                }
            }

            //完成回弹动作
            //用于攻击时还没回弹就被打断切换成了其他状态
            if (state < 2 && posDeltaX * movDirect > 0)
            {
                Debug.Log("backing");
                posDeltaX -= spdHitBac * Time.deltaTime * movDirect;
                //回来了
                if (posDeltaX * movDirect <= 0)
                {
                    Debug.Log("backMax");
                    outOrIn = 1;
                    posDeltaX = 0;      //溢出修正
                    timeHitNow = 0;
                }
            }

            //状态转化控制 必须已从移动和攻击回复完毕 现在不需要了
            //if (posY <= posInitY && posDeltaX * movDirect <= 0)

            if (flagWait)
            {
                state = 0;
                flagWait = false;
            }
            if (flagAttack)
            {
                state = 2;
                flagAttack = false;
                posDeltaX = 0;
            }
            if (flagMove)
            {
                state = 1;
                flagMove = false;
            }


            this.transform.localPosition = new Vector3(posDeltaX, posY, 0);

        }
    }
}