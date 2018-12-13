/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/12 16:12:36
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "策划/车厢")]
public class CarriageSetting : ScriptableObject {
    [Tooltip("ID")]
    public int ID;
    [Tooltip("车厢名字")]
    public string Name;
    [Tooltip("车厢描述")]
    public string Description;
    [Tooltip("总建造工作量")]
    public float WorkAll;
    [Tooltip("车厢大小")]
    public Vector2 Size;
    [Tooltip("Prefab")]
    public GameObject Prefab;
    [Tooltip("要求科技")]
    public int RequiredTech;
    [Tooltip("建造耗材")]
    public ItemData[] BuildCosts;

    public bool HasUnlocked() {
        return TechTreeManager.Instance.Techs[RequiredTech].TechState == Tech.State.COMPLETED;
    }
}
