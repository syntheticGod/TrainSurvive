/*
 * 描述：车厢升级配置
 * 作者：刘旭涛
 * 创建时间：2019/1/28 19:48:41
 * 版本：v0.7
 */
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "策划/车厢升级配置")]
public class CarriageResearchSetting : ScriptableObject {
    
    [Tooltip("ID")]
    public int ID;
    [Tooltip("依赖ID")]
    public int Dependency;
    [Tooltip("名称")]
    public string Name;
    [Tooltip("描述")]
    public string Description;
    [Tooltip("耗材")]
    public ItemData[] Costs;
    [Space]
    [Tooltip("处理升级的设施名称。为空的话则将升级运算交给CarriageBackend（及其子类）处理。")]
    public string StructureName;
    [Tooltip("升级运算需要的参数")]
    public string Parameter;
    [Tooltip("是否解锁同名的UI，若StructureName为空则无效。")]
    public bool UnlockUI;
    [Tooltip("是否解锁设施，若StructureName为空则无效。")]
    public bool UnlockStructure;
    [Tooltip("更新同名设施贴图至某一级，-1表示不更新，若StructureName为空则无效。")]
    public int SpriteLevel = -1;
}
