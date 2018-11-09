using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class person 
{
    public string name = "11";
    public bool ifOuting = false;
    public bool sex = Random.value > 0.5f;
    public uint weaponID = 0;

    const float basicAtk = 100;
    const float basicAts = 1;
    const float basicCrc = 0;
    const float basicCrd = 1.6f;
    const float basicSpd = 1;
    const float basicHit = 1;
    const float basicHpMax = 100;
    const float basicApMax = 100;
    const float basicApDelta = 5;

    public int[] abi = new int[6]; //1体力 2力量 3敏捷 4技巧 5智力
    public float hp;
    public float hpMax;
    public float ap;
    public float apMax;
    public float apDelta;
    public float valAtk;
    public float valAts;
    public float valSpd;
    public float valCrc;
    public float valCrd;
    public float valHit;
    
    public void randAbi()
    {
        for (int i = 1; i <= 5; i++)
        {
            abi[i] = Random.Range(0, 10);
            updateVal();
        }

    }
    public void updateVal()
    {
        hpMax = basicHpMax * (float)(1 + abi[1] * 0.05); //hp 体力
        apMax = basicApMax * (float)(1 + abi[5] * 0.05); //sp 智力
        apDelta = basicApDelta * (float)(1 + abi[5] * 0.05); 
        valAtk = basicAtk * (float)(1 + abi[2] * 0.05); //atk 力量

        /******公式待定****/
    }
        
       
}
