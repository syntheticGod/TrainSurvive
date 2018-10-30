/*
 * 描述：列车上的设施
 * 作者：刘旭涛
 * 创建时间：2018/10/29 22:08:00
 * 版本：v0.1
 */
using UnityEngine;

public abstract class Facility : MonoBehaviour {

    [Header("Base Feature")]
    [Tooltip("总建造工作量。")]
    public uint WorkAll;
    [Tooltip("可以支持该设施的建筑平台类型。")]
    public LayerMask[] RequireLayers;

    /// <summary>
    /// 设施名称
    /// </summary>
    public abstract string Name { get; }

    protected virtual void Awake() {
        if (gameObject.layer != LayerMask.NameToLayer("Facility")) {
            Debug.LogError("该Facility对象的layer必须设置为Facility.", this);
        }
    }

    public virtual void OnPlaced() {

    }
}
