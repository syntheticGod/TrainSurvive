using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pControl : MonoBehaviour
{
    public int state; //0静止 1移动 2攻击 3等待攻击
    public int upOrDown; //跳跃状态细节 0静止 1上升 2下降
    public int outOrIn;  //撞击状态细节 0静止 1向外撞 2回内
    public int movDirect = 1; //前进方向 1前进 -1后退
    public float posInitX = -7; //初始x
    public float posInitY = -3; //初始y
    public float posX;
    public float posY;
    public float posRecX;
    public float posRecY;
    public float posDeltaX;
    public float spdMov = 2;    //移动速度
    public float spdJmp = 5;    //跳跃速度
    public float spdHitOut = 6; //撞出速度
    public float spdHitBac = 3; //回弹速度
    public float widHit = 0.5f; //最大攻击位移
    public float heiJmp = 0.5f; //最大跳跃高度
    public float timeHitNow = 0;
    public float timeHitMax = 1; //攻击间隔
    public float spdRotate = 90; //攻击旋转速度
    public bool flagWait = false;   //用于【将其他状态平滑转换到静止状态】的标志位 
    public bool flagAttack = false; //用于【将其他状态平滑转换到攻击状态】的标志位 
    public bool flagMove = false;
    // Start is called before the first frame update
    void Start()
    {

        Reset();

    }

    private void Reset()
    {
        flagWait = false;
        flagAttack = false;
        flagMove = false;
        state = 0;
        upOrDown = 0;
        outOrIn = 0;
        this.transform.position = new Vector3(posInitX, posInitY, 0);
        posX = posInitX;
        posY = posInitY;
        posDeltaX = 0;
        movDirect = 1;
    }

    // Update is called once per frame
    void Update()
    {
        posX = this.transform.position.x;
        posY = this.transform.position.y;
        if (Input.GetKeyDown("q"))
        {
            flagWait = true;
            flagMove = false;
            flagAttack = false;
        }
        if (Input.GetKeyDown("w"))
        {
            flagMove = true;
            flagWait = false;
            flagAttack = false;
        }
        if (Input.GetKeyDown("e"))
        {
            flagAttack = true;
            flagMove = false;
            flagWait = false;
        }
        if (Input.GetKeyDown("r"))
        {
            Reset();
        }
        if (Input.GetKeyDown("t"))
        {
            movDirect = -movDirect;
            this.transform.Rotate(0, -180, 0);
        }

        //正处于移动状态
        if (state == 1)
        {
            posX += spdMov * Time.deltaTime * movDirect;
            if (upOrDown == 0)
                upOrDown = 1;
            //上升过程
            else if (upOrDown == 1)
            {
                posY += spdJmp * Time.deltaTime;
                //到顶了
                if (posY >= posInitY + heiJmp)
                {
                    posY = posInitY + heiJmp;    //溢出修正
                    upOrDown = 2;
                }
            }
            //下落过程
            else if (upOrDown == 2)
            {
                posY -= spdJmp * Time.deltaTime;
                //落地了
                if (posY <= posInitY)
                {
                    upOrDown = 1;
                    posY = posInitY;    //溢出修正
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
                if ( movDirect*posDeltaX>=widHit)
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

        //状态转化控制 必须已从移动和攻击回复完毕
        if (posY <= posInitY && posDeltaX * movDirect <= 0)
        {
            if (flagWait)
            {
                state = 0;
                flagWait = false;
            }
            if (flagAttack)
            {
                state = 2;
                flagAttack = false;
                posRecX = posX;
                posDeltaX = 0;
            }
            if (flagMove)
            {
                state = 1;
                flagMove = false;
            }
        }


        if (posDeltaX * movDirect <= 0)
            this.transform.position = new Vector3(posX, posY, 0);
        else this.transform.position = new Vector3(posRecX + posDeltaX, posY, 0);

    }
}
