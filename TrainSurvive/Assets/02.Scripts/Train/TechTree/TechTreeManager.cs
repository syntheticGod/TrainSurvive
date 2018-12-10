/*
 * 描述：科技树
 * 作者：刘旭涛
 * 创建时间：2018/11/25 21:57:47
 * 版本：v0.1
 */
using UnityEngine;

public class TechTreeManager : MonoBehaviour {

    public static TechTreeManager Instance { get; private set; }
    
    /// <summary>
    /// 登记科技列表，与world同步。
    /// </summary>
    public static Tech[] Techs {
        get {
            if (World.getInstance().techArray == null) {
                World.getInstance().techArray = _techs;
            }
            return World.getInstance().techArray;
        }
    }
    private static Tech[] _techs = {
        /* 0 */ new TrainBasicTech(),
        /* 1 */ new TrainExpand0Tech(),
        /* 2 */ new Science0Tech(),
        /* 3 */ new Carpentry0Tech(),
        /* 4 */ new Carpentry1Tech(),
        /* 5 */ new Carpentry2Tech()
    };
    
    /// <summary>
    /// 当前研究项
    /// </summary>
    public int CurrentWorking {
        get {
            return _currentWorking;
        }
        set {
            if (_currentWorking >= 0 && Techs.Length > _currentWorking) {
                Techs[_currentWorking].StopWorking();
            }
            if (value >= 0 && Techs.Length > value) {
                Techs[value].StartWorking();
            }
            _currentWorking = value;
        }
    }
    private int _currentWorking = -1;
    
    private void Awake() {
        Instance = this;

        // 初始化默认启用科技
        if (Techs[0].TechState != Tech.State.COMPLETED)
            Techs[0].StartWorking();

        for (int i = 0; i < Techs.Length; i++) {
            if (Techs[i].TechState == Tech.State.WORKING) {
                CurrentWorking = i;
            }
        }
    }
    
    private void OnDestroy() {
        Instance = null;
    }
}
