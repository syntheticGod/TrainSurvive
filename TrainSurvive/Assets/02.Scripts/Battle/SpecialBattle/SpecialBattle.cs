/*
 * 描述：
 * 作者：NONE
 * 创建时间：2019/1/29 17:59:28
 * 版本：v0.7
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class SpecialBattle  {
    public int id;
    public string name;
    public List<ValueTuple<int, int>> monsterList = new List<ValueTuple<int, int>>();//前者为id，后者为数量
    public List<ValueTuple<int, int>> rewardList = new List<ValueTuple<int, int>>();//前者为id，后者为数量
    public int expNum;
    public bool is_talk_battle;
    /// <summary>
    /// 区块坐标X
    /// </summary>
    public string posX;//为now代表动态获取当前位置
    /// <summary>
    /// 区块坐标Y
    /// </summary>
    public string posY;//为now代表动态获取当前位置
}
