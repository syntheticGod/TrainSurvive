/*
 * 描述：科技树
 * 作者：刘旭涛
 * 创建时间：2018/11/25 21:57:47
 * 版本：v0.1
 */
using System;
using UnityEngine;

public class TechTreeManager : MonoBehaviour {

    public static TechTreeManager Instance { get; private set; }
    
    /// <summary>
    /// 登记科技列表，与world同步。
    /// </summary>
    public Tech[] Techs {
        get {
            if (World.getInstance().techArray == null) {
                World.getInstance().techArray = new Tech[TechSettings.Length];
                for (int i = 0; i < TechSettings.Length; i++) {
                    if (TechSettings[i] != null) {
                        World.getInstance().techArray[i] = new Tech(i);
                    }
                }
            }
            return World.getInstance().techArray;
        }
    }
    
    public static TechSetting[] TechSettings {
        get {
            if (_techSettings == null) {
                TechSetting[] techSettings = ResourceLoader.GetResources<TechSetting>("Techs", false);
                int max = -1;
                for (int i = 0; i < techSettings.Length; i++) {
                    max = Mathf.Max(techSettings[i].ID, max);
                }
                _techSettings = new TechSetting[max + 1];
                for (int i = 0; i < techSettings.Length; i++) {
                    _techSettings[techSettings[i].ID] = techSettings[i];
                }
            }
            return _techSettings;
        }
    }
    
    /// <summary>
    /// 当前研究项
    /// </summary>
    public int CurrentWorking {
        get {
            return _currentWorking;
        }
        set {
            if (_currentWorking >= 0 && Techs[_currentWorking] != null) {
                Techs[_currentWorking].StopWorking();
            }
            if (value >= 0 && Techs[value] != null) {
                Techs[value].StartWorking();
            }
            _currentWorking = value;
        }
    }

    private int _currentWorking = -1;
    private static TechSetting[] _techSettings;

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
