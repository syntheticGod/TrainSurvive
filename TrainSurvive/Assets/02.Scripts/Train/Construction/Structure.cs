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
using UnityEngine.Events;

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

    public struct ButtonAction {
        public string Name { get; set; }
        public Action<Structure> Action { get; set; }
    }

    public class FixedInfo {
        /// <summary>
        /// 可以支持该设施的建筑平台的Layers名称。
        /// </summary>
        public string[] RequiredLayerNames { get; set; } = { "TrainGround" };
        /// <summary>
        /// 物体放置方向，默认向下
        /// </summary>
        public Vector2 LayerOrientation { get; set; } = Vector2.down;
        /// <summary>
        /// 物体所属Layer名称。
        /// </summary>
        public string LayerName { get; set; } = "Facility";
        /// <summary>
        /// 物体所属Layer。
        /// </summary>
        public LayerMask Layer {
            get {
                return LayerMask.NameToLayer(LayerName);
            }
        }
        /// <summary>
        /// 可以支持该设施的建筑平台的Layers。
        /// </summary>
        public LayerMask RequiredLayers {
            get {
                return LayerMask.GetMask(RequiredLayerNames);
            }
        }
        /// <summary>
        /// 设施名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 设施描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 总建造工作量
        /// </summary>
        public float WorkAll { get; set; }
        /// <summary>
        /// 建造耗材
        /// </summary>
        public Cost[] BuildCosts { get; set; }
        /// <summary>
        /// Sprite path.
        /// </summary>
        public string SpritePath { get; set; }
        /// <summary>
        /// Sprite
        /// </summary>
        public Sprite Sprite {
            get {
                return ResourceLoader.GetResource<Sprite>(SpritePath);
            }
        }
        /// <summary>
        /// 所属类型
        /// </summary>
        public int Class { get; set; }
        /// <summary>
        /// 决定了Onstart方法是每次载入都调用还是只有建筑完成时会调用一次
        /// </summary>
        public bool IsOnceFunction { get; set; }
        /// <summary>
        /// 菜单按钮名称和事件
        /// </summary>
        public ButtonAction[] Actions { get; set; }
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
            CallOnStateChange();
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

    public event Action<Structure> OnStateChange;
    public event Action<float, float, float> OnProgressChange;

    private Coroutine BuildingCoroutine { get; set; }

    private State _facilityState = State.NONE;
    private float _workNow;

    public virtual ButtonAction[] GetActions() {
        return Info.Actions;
    }

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
    /// 放置物体。
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
        WorkSpeedRatio = (float)info.GetValue("WorkSpeedRatio", typeof(float));
        WorkNow = (float)info.GetValue("WorkNow", typeof(float));
        Position = (Vector3)info.GetValue("Position", typeof(Vector3));
        BuildCostRatios = (float[])info.GetValue("BuildCostRatios", typeof(float[]));
        CostReturnRatios = (float[])info.GetValue("CostReturnRatios", typeof(float[]));
        State state = (State)info.GetValue("FacilityState", typeof(State));
        if (state != State.WORKING || !Info.IsOnceFunction) {
            FacilityState = state;
        } else {
            _facilityState = state;
        }
    }

    public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue("WorkSpeedRatio", WorkSpeedRatio);
        info.AddValue("WorkNow", WorkNow);
        info.AddValue("FacilityState", FacilityState);
        info.AddValue("Position", Position);
        info.AddValue("BuildCostRatios", BuildCostRatios);
        info.AddValue("CostReturnRatios", CostReturnRatios);
    }

    protected void CallOnStateChange() {
        OnStateChange?.Invoke(this);
    }
    protected void CallOnProgressChange(float min, float max, float value) {
        OnProgressChange?.Invoke(min, max, value);
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
        while (WorkNow < Info.WorkAll) {
            WorkNow += Time.deltaTime * WorkSpeedRatio;
            yield return 1;
        }
        WorkNow = 0;
        FacilityState = State.WORKING;
    }
}
