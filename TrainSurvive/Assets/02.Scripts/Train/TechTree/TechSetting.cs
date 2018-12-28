/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/10 17:05:37
 * 版本：v0.1
 */
using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "策划/科技")]
public class TechSetting : ScriptableObject {

    [Serializable]
    public class TechCompleteAction : SerializableAction<Tech> { }

    [Tooltip("ID")]
    public int ID;
    [Tooltip("依赖关系，使用ID表示依赖的科技。")]
    public int[] Dependencies;
    [Tooltip("名称")]
    public string Name;
    [Tooltip("描述")]
    public string Description;
    [Tooltip("总工作量")]
    public float TotalWorks;
    [Tooltip("额外功能。")]
    public TechCompleteAction OnCompleted;
}
