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
public abstract class Tech : ISerializable {
    
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
    /// 工作量系数
    /// </summary>
    public float WorkRatio { get; set; } = 1;
    /// <summary>
    /// 当前工作量
    /// </summary>
    public float CurrentWorks { get; private set; }
    /// <summary>
    /// 当前科技状态
    /// </summary>
    public State TechState {
        get {
            for (int i = 0; i < Dependencies.Length; i++) {
                if (TechTree.Techs[Dependencies[i]].TechState != State.COMPLETED) {
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
    /// 依赖关系，使用ID表示依赖的科技。ID定义见<see cref="TechTree.Techs"/>下标。
    /// </summary>
    public abstract int[] Dependencies { get; }
    /// <summary>
    /// 名称
    /// </summary>
    public abstract string Name { get; }
    /// <summary>
    /// 描述
    /// </summary>
    public abstract string Description { get; }
    /// <summary>
    /// 总工作量
    /// </summary>
    public abstract float TotalWorks { get; }

    /// <summary>
    /// 完成研究
    /// </summary>
    public abstract void OnCompleted();

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

    public Tech() { }

    /// <summary>
    /// 序列化恢复时，根据序列化前的状态自动启动研究过程。
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected Tech(SerializationInfo info, StreamingContext context) {
        IsCompleted = info.GetBoolean("IsCompleted");
        IsWorking = info.GetBoolean("IsWorking");
        WorkRatio = (float)info.GetValue("WorkRatio", typeof(float));
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
        info.AddValue("WorkRatio", WorkRatio);
        info.AddValue("CurrentWorks", CurrentWorks);
    }

    private IEnumerator RunResearch() {
        while (CurrentWorks < TotalWorks * WorkRatio) {
            CurrentWorks += Time.deltaTime;
            yield return 1;
        }
        IsCompleted = true;
        OnCompleted();
    }
}
