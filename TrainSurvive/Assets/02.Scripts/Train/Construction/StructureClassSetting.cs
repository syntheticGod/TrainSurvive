/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/11 20:39:45
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "策划/建筑分类")]
public class StructureClassSetting : ScriptableObject {
    [Tooltip("建筑类型")]
    public string[] Classes;
    
}
