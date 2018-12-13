/*
 * 描述：建筑类型
 * 作者：刘旭涛
 * 创建时间：2018/11/29 0:14:15
 * 版本：v0.1
 */
using Assets._02.Scripts.zhxUIScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Structure : ISerializable {
    
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
    
    public struct ButtonAction {
        public string Title;
        public Action<Structure> Action;

        public ButtonAction(string title, Action<Structure> action) {
            Title = title;
            Action = action;
        }
    }

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
            CallOnProgressChange(0, ConstructionManager.StructureSettings[ID].WorkAll, value);
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
                    HasTriggered = true;
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
    /// <summary>
    /// ID
    /// </summary>
    public int ID { get; private set; }
    /// <summary>
    /// 是否已触发过OnStart
    /// </summary>
    protected bool HasTriggered { get; private set; }

    public event Action<Structure> OnStateChange;
    public event Action<float, float, float> OnProgressChange;

    private Coroutine BuildingCoroutine { get; set; }

    private State _facilityState = State.NONE;
    private float _workNow;
    
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
    /// 返回上下文菜单按钮项。
    /// </summary>
    public virtual List<ButtonAction> GetButtonActions() {
        return new List<ButtonAction> {
            new ButtonAction("拆除", (structure) => structure.FacilityState = State.REMOVING)
        };
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
        return PublicMethod.CanConsumeItems(ConstructionManager.StructureSettings[ID].BuildCosts);
    }

    public Structure(int id) {
        ID = id;
        BuildCostRatios = new float[ConstructionManager.StructureSettings[ID].BuildCosts.Length];
        CostReturnRatios = new float[ConstructionManager.StructureSettings[ID].BuildCosts.Length];
        for (int i = 0; i < ConstructionManager.StructureSettings[ID].BuildCosts.Length; i++) {
            BuildCostRatios[i] = 1;
        }
    }
    
    protected Structure(SerializationInfo info, StreamingContext context) {
        ID = info.GetInt32("ID");
        HasTriggered = info.GetBoolean("HasTriggered");
        WorkSpeedRatio = (float)info.GetValue("WorkSpeedRatio", typeof(float));
        WorkNow = (float)info.GetValue("WorkNow", typeof(float));
        Position = new Vector3((float)info.GetValue("PositionX", typeof(float)), (float)info.GetValue("PositionY", typeof(float)), (float)info.GetValue("PositionZ", typeof(float)));
        BuildCostRatios = (float[])info.GetValue("BuildCostRatios", typeof(float[]));
        CostReturnRatios = (float[])info.GetValue("CostReturnRatios", typeof(float[]));
        FacilityState = (State)info.GetValue("FacilityState", typeof(State));
    }

    public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue("ID", ID);
        info.AddValue("HasTriggered", HasTriggered);
        info.AddValue("WorkSpeedRatio", WorkSpeedRatio);
        info.AddValue("WorkNow", WorkNow);
        info.AddValue("FacilityState", FacilityState);
        info.AddValue("PositionX", Position.x);
        info.AddValue("PositionY", Position.y);
        info.AddValue("PositionZ", Position.z);
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
        PublicMethod.ConsumeItems(ConstructionManager.StructureSettings[ID].BuildCosts);
    }

    private void ReturnCosts(bool isCancel) {
        // TODO
        Debug.Log("TODO: RETURN COST.");
    }

    private IEnumerator RunBuilding() {
        while (WorkNow < ConstructionManager.StructureSettings[ID].WorkAll) {
            WorkNow += Time.deltaTime * WorkSpeedRatio;
            yield return 1;
        }
        WorkNow = 0;
        FacilityState = State.WORKING;
    }
}
