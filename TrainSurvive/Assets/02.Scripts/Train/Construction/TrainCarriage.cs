/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/5 18:28:01
 * 版本：v0.1
 */
using Assets._02.Scripts.zhxUIScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public abstract class TrainCarriage : ISerializable {

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
        /// 车厢名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 车厢描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 总建造工作量
        /// </summary>
        public float WorkAll { get; set; }
        /// <summary>
        /// 建造耗材
        /// </summary>
        public ItemData[] BuildCosts { get; set; }
        /// <summary>
        /// 车厢大小
        /// </summary>
        public Vector2 Size { get; set; }
        /// <summary>
        /// Prefab path.
        /// </summary>
        public string PrefabPath { get; set; }
        /// <summary>
        /// GameObject
        /// </summary>
        public GameObject Object {
            get {
                return ResourceLoader.GetResource<GameObject>(PrefabPath);
            }
        }
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
        /// 建造完成
        /// </summary>
        IDLE
    }

    /// <summary>
    /// 通用信息
    /// </summary>
    public abstract FixedInfo Info { get; }
    /// <summary>
    /// 车厢位置
    /// </summary>
    public Vector3 Position { get; set; }
    /// <summary>
    /// 建造工作速度系数
    /// </summary>
    public float WorkSpeedRatio { get; set; } = 1;
    /// <summary>
    /// 当前建造工作量。
    /// </summary>
    public float WorkNow {
        get { return _workNow; }
        set {
            _workNow = value;
            CallOnProgressChange(0, Info.WorkAll, value);
        }
    }
    /// <summary>
    /// 车厢状态。
    /// </summary>
    public State CarriageState {
        get {
            return _carriageState;
        }
        set {
            if (_carriageState == value)
                return;
            _carriageState = value;
            switch (value) {
                case State.BUILDING:
                    BuildingCoroutine = TimeController.getInstance().StartCoroutine(RunBuilding());
                    break;
                case State.IDLE:
                    OnCompleted();
                    break;
                case State.CANCLE:
                    if (BuildingCoroutine != null)
                        TimeController.getInstance().StopCoroutine(BuildingCoroutine);
                    ReturnCosts();
                    break;
            }
            CallOnStateChange();
        }
    }
    /// <summary>
    /// 建造耗材比例。
    /// </summary>
    public float[] BuildCostRatios { get; protected set; }

    public event Action<TrainCarriage> OnStateChange;
    public event Action<float, float, float> OnProgressChange;

    private Coroutine BuildingCoroutine { get; set; }

    private float _workNow;
    private State _carriageState;

    /// <summary>
    /// 放置物体。
    /// </summary>
    /// <param name="position">安置位置</param>
    /// <returns>true放置成功，false放置失败。</returns>
    public bool Place(Vector3 position, bool initial) {
        if (initial) {
            Position = position;
            CarriageState = State.IDLE;
            return true;
        }
        if (IsCostsAvailable()) {
            CostItems();
            Position = position;
            CarriageState = State.BUILDING;
            return true;
        } else {
            return false;
        }
    }

    public TrainCarriage Instantiate() {
        return GetType().GetConstructor(new Type[] { }).Invoke(new object[] { }) as TrainCarriage;
    }

    public virtual void OnCompleted() { }

    public TrainCarriage() {
        BuildCostRatios = new float[Info.BuildCosts.Length];
        for (int i = 0; i < Info.BuildCosts.Length; i++) {
            BuildCostRatios[i] = 1;
        }
    }

    public TrainCarriage(SerializationInfo info, StreamingContext context) {
        WorkSpeedRatio = (float)info.GetValue("WorkSpeedRatio", typeof(float));
        WorkNow = (float)info.GetValue("WorkNow", typeof(float));
        Position = new Vector3((float)info.GetValue("PositionX", typeof(float)), (float)info.GetValue("PositionY", typeof(float)), (float)info.GetValue("PositionZ", typeof(float)));
        BuildCostRatios = (float[])info.GetValue("BuildCostRatios", typeof(float[]));
        State state = (State)info.GetValue("CarriageState", typeof(State));
        if (state != State.IDLE) {
            CarriageState = state;
        } else {
            _carriageState = state;
        }
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue("WorkSpeedRatio", WorkSpeedRatio);
        info.AddValue("WorkNow", WorkNow);
        info.AddValue("CarriageState", CarriageState);
        info.AddValue("PositionX", Position.x);
        info.AddValue("PositionY", Position.y);
        info.AddValue("PositionZ", Position.z);
        info.AddValue("BuildCostRatios", BuildCostRatios);
    }

    private void CostItems() {
        PublicMethod.ConsumeItems(Info.BuildCosts);
    }

    private void ReturnCosts() {
        // TODO
        Debug.Log("TODO: RETURN COST.");
    }

    /// <summary>
    /// 判断当前建材是否充足。
    /// </summary>
    /// <returns></returns>
    public bool IsCostsAvailable() {
        // TODO
        return true;
    }

    protected void CallOnStateChange() {
        OnStateChange?.Invoke(this);
    }
    protected void CallOnProgressChange(float min, float max, float value) {
        OnProgressChange?.Invoke(min, max, value);
    }

    private IEnumerator RunBuilding() {
        while (WorkNow < Info.WorkAll) {
            WorkNow += Time.deltaTime * WorkSpeedRatio;
            yield return 1;
        }
        WorkNow = 0;
        CarriageState = State.IDLE;
    }
}
