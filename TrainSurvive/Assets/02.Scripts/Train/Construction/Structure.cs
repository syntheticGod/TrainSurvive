/*
 * 描述：建筑类型
 * 作者：刘旭涛
 * 创建时间：2018/11/29 0:14:15
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public abstract class Structure : ISerializable {

    [Serializable]
    public struct Cost {
        /// <summary>
        /// 物品ID
        /// </summary>
        public int ItemID;
        /// <summary>
        /// 耗材量
        /// </summary>
        public float Value;
    }

    public class FixedInfo {
        /// <summary>
        /// 可以支持该设施的建筑平台的Layers。
        /// </summary>
        public LayerMask RequiredLayers {
            get {
                return LayerMask.GetMask(RequiredLayerNames);
            }
        }
        /// <summary>
        /// 可以支持该设施的建筑平台Layer名称。
        /// </summary>
        public string[] RequiredLayerNames { get; protected set; } = { "TrainGround" };
        /// <summary>
        /// 设施名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 设施描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 总建造工作量，实际工作量为总工作量 * 工作量系数
        /// </summary>
        public float WorkAll { get; set; }
        /// <summary>
        /// 建造耗材
        /// </summary>
        public Cost[] BuildCosts { get; set; }
        /// <summary>
        /// Sprite
        /// </summary>
        public Sprite Sprite { get; set; }
    }

    public enum State {
        /// <summary>
        /// 待建造
        /// </summary>
        NONE,
        /// <summary>
        /// 取消建造
        /// </summary>
        CANCLE,
        /// <summary>
        /// 建造中
        /// </summary>
        BUILDING,
        /// <summary>
        /// 工作中
        /// </summary>
        WORKING,
        /// <summary>
        /// 拆除中
        /// </summary>
        REMOVING
    }

    /// <summary>
    /// 通用信息
    /// </summary>
    public abstract FixedInfo Info { get; }

    /// <summary>
    /// 总建造工作量系数
    /// </summary>
    public float WorkRatio { get; set; } = 1;
    /// <summary>
    /// 当前建造工作量。
    /// </summary>
    public float WorkNow { get; set; }
    /// <summary>
    /// 设施状态。
    /// </summary>
    public State FacilityState {
        get {
            return _facilityState;
        }
        set {
            if (_facilityState == value)
                return;
            _facilityState = value;
            switch (value) {
                case State.BUILDING:
                    BuildingCoroutine = TimeController.getInstance().StartCoroutine(RunBuilding());
                    break;
                case State.WORKING:
                    OnStart();
                    break;
                case State.REMOVING:
                    OnRemoving();
                    break;
                case State.CANCLE:
                    if (BuildingCoroutine != null)
                        TimeController.getInstance().StopCoroutine(BuildingCoroutine);
                    ReturnCosts(true);
                    break;
            }
        }
    }
    /// <summary>
    /// 设施安置位置。
    /// </summary>
    public Vector3 Position { get; protected set; }
    /// <summary>
    /// 建造耗材比例。
    /// </summary>
    public float[] BuildCostRatios { get; protected set; }
    /// <summary>
    /// 建造耗材返还比例。
    /// </summary>
    public float[] CostReturnRatios { get; protected set; }

    private Coroutine BuildingCoroutine { get; set; }

    private State _facilityState = State.NONE;

    /// <summary>
    /// 当设施进入启动状态的回调。
    /// </summary>
    protected virtual void OnStart() { }

    /// <summary>
    /// 请求移除物体时的回调。
    /// </summary>
    protected virtual void OnRemoving() {
        ReturnCosts(false);
    }

    /// <summary>
    /// 放置物体后的回调，如果override的话务必base.OnPlaced()一下。
    /// </summary>
    /// <param name="position">安置位置</param>
    /// <returns>true放置成功，false放置失败。</returns>
    public bool Place(Vector3 position) {
        if (IsCostsAvailable()) {
            CostItems();
            Position = position;
            FacilityState = State.BUILDING;
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// 判断当前建材是否充足。
    /// </summary>
    /// <returns></returns>
    public bool IsCostsAvailable() {
        // TODO
        return true;
    }

    public Structure() {
        BuildCostRatios = new float[Info.BuildCosts.Length];
        CostReturnRatios = new float[Info.BuildCosts.Length];
        for (int i = 0; i < Info.BuildCosts.Length; i++) {
            BuildCostRatios[i] = 1;
        }
    }

    public Structure Instantiate() {
        return GetType().GetConstructor(new Type[] { }).Invoke(new object[] { }) as Structure;
    }

    protected Structure(SerializationInfo info, StreamingContext context) {
        WorkRatio = (float)info.GetValue("WorkRatio", typeof(float));
        WorkNow = (float)info.GetValue("WorkNow", typeof(float));
        Position = (Vector3)info.GetValue("Position", typeof(Vector3));
        BuildCostRatios = (float[])info.GetValue("BuildCostRatios", typeof(float[]));
        CostReturnRatios = (float[])info.GetValue("CostReturnRatios", typeof(float[]));
        FacilityState = (State)info.GetValue("FacilityState", typeof(State));
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue("WorkRatio", WorkRatio);
        info.AddValue("WorkNow", WorkNow);
        info.AddValue("FacilityState", FacilityState);
        info.AddValue("Position", Position);
        info.AddValue("BuildCostRatios", BuildCostRatios);
        info.AddValue("CostReturnRatios", CostReturnRatios);
    }

    private void CostItems() {
        // TODO
        Debug.Log("TODO: COST!");
    }

    private void ReturnCosts(bool isCancel) {
        // TODO
        Debug.Log("TODO: RETURN COST.");
    }

    private IEnumerator RunBuilding() {
        while (WorkNow < Info.WorkAll * WorkRatio) {
            WorkNow += Time.deltaTime;
            yield return 1;
        }
        FacilityState = State.WORKING;
    }
}
