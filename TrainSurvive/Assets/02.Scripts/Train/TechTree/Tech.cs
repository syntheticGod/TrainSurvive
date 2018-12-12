/*
 * 描述：科技功能，依赖于TimeController，可序列化存档
 * 作者：刘旭涛
 * 创建时间：2018/11/25 23:50:05
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Tech : ISerializable {
    
    public enum State {
        LOCKED,
        UNLOCKED,
        COMPLETED,
        WORKING
    }
    
    private bool IsCompleted { get; set; }
    private bool IsWorking { get; set; }
    private Coroutine ResearchCoroutine { get; set; }

    /// <summary>
    /// 工作速度系数
    /// </summary>
    public float WorkSpeedRatio { get; set; } = 1;
    /// <summary>
    /// 当前工作量
    /// </summary>
    public float CurrentWorks { get; private set; }
    /// <summary>
    /// 当前科技状态
    /// </summary>
    public State TechState {
        get {
            if (World.getInstance().techUnlock <= 0 && !IsCompleted) {
                return State.LOCKED;
            }
            for (int i = 0; i < TechTreeManager.TechSettings[ID].Dependencies.Length; i++) {
                if (TechTreeManager.Instance.Techs[TechTreeManager.TechSettings[ID].Dependencies[i]].TechState != State.COMPLETED) {
                    return State.LOCKED;
                }
            }
            if (IsCompleted) {
                return State.COMPLETED;
            }
            if (IsWorking) {
                return State.WORKING;
            }
            return State.UNLOCKED;
        }
    }
    /// <summary>
    /// ID
    /// </summary>
    public int ID { get; private set; }
    
    /// <summary>
    /// 开始研究
    /// </summary>
    public void StartWorking() {
        StopWorking();
        if (!IsCompleted) {
            IsWorking = true;
            ResearchCoroutine = TimeController.getInstance().StartCoroutine(RunResearch());
        }
    }

    /// <summary>
    /// 暂停研究
    /// </summary>
    public void StopWorking() {
        IsWorking = false;
        if (ResearchCoroutine != null) {
            TimeController.getInstance().StopCoroutine(ResearchCoroutine);
            ResearchCoroutine = null;
        }
    }

    public Tech(int id) {
        ID = id;
        if (TechTreeManager.TechSettings[id].TotalWorks == 0) {
            IsCompleted = true;
            TechTreeManager.TechSettings[ID].OnCompleted.Invoke(this);
        }
    }

    /// <summary>
    /// 序列化恢复时，根据序列化前的状态自动启动研究过程。
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected Tech(SerializationInfo info, StreamingContext context) {
        ID = info.GetInt32("ID");
        IsCompleted = info.GetBoolean("IsCompleted");
        IsWorking = info.GetBoolean("IsWorking");
        WorkSpeedRatio = (float)info.GetValue("WorkSpeedRatio", typeof(float));
        CurrentWorks = (float)info.GetValue("CurrentWorks", typeof(float));
        if (IsWorking) {
            StartWorking();
        }
    }

    /// <summary>
    /// 自定义序列化
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue("IsCompleted", IsCompleted);
        info.AddValue("IsWorking", IsWorking);
        info.AddValue("WorkSpeedRatio", WorkSpeedRatio);
        info.AddValue("CurrentWorks", CurrentWorks);
        info.AddValue("ID", ID);
    }

    private IEnumerator RunResearch() {
        while (CurrentWorks < TechTreeManager.TechSettings[ID].TotalWorks) {
            CurrentWorks += Time.deltaTime * WorkSpeedRatio;
            yield return 1;
        }
        IsCompleted = true;
        TechTreeManager.TechSettings[ID].OnCompleted.Invoke(this);
    }
}
