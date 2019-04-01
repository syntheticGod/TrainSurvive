/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/2/11 13:13:01
 * 版本：v0.7
 */
using Assets._02.Scripts.zhxUIScripts;
using System;
using System.Collections;
using System.Runtime.Serialization;
using TTT.Item;
using UnityEngine;

[Serializable]
public class ComposeStructure : CarriageStructure {

    [Serializable]
    public struct ComposeFormula {
        public int[] FromComponent1;
        public int[] FromComponent2;
        public int ToWeapon;
    }

    public ComposeStructure(string name, bool initialEnabled) : base(name, initialEnabled) { }

    protected ComposeStructure(SerializationInfo info, StreamingContext context) : base(info, context) {
        Materials = (ComponentData[])info.GetValue("Materials", typeof(ComponentData[]));
        _processTime = info.GetSingle("_processTime");
        ProcessSpeedRatio = info.GetSingle("ProcessSpeedRatio");
        Progress = info.GetSingle("Progress");
        _formulas = (ComposeFormula[])info.GetValue("_formulas", typeof(ComposeFormula[]));
        SelectedFormula = info.GetInt32("SelectedFormula");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("Materials", Materials);
        info.AddValue("_processTime", _processTime);
        info.AddValue("ProcessSpeedRatio", ProcessSpeedRatio);
        info.AddValue("Progress", Progress);
        info.AddValue("_formulas", _formulas);
        info.AddValue("SelectedFormula", SelectedFormula);
    }

    public ComponentData[] Materials { get; set; } = new ComponentData[2];

    /// <summary>
    /// 处理时间
    /// </summary>
    public float ProcessTime {
        get {
            return _processTime;
        }
    }
    /// <summary>
    /// 配方
    /// </summary>
    public ComposeFormula[] Formulas {
        get {
            return _formulas;
        }
    }
    /// <summary>
    /// 处理速度比例
    /// </summary>
    public float ProcessSpeedRatio { get; set; } = 1;
    /// <summary>
    /// 进度
    /// </summary>
    public float Progress { get; set; }
    /// <summary>
    /// 已选中的配方
    /// </summary>
    public int SelectedFormula { get; set; } = -1;

    public event Action OnFinished;
    public event Action<float, float> OnProgressUpdate;

    [StructurePublicField(Tooltip = "处理时间")]
    private float _processTime;
    [StructurePublicField(Tooltip = "配方")]
    private ComposeFormula[] _formulas;

    public override void OnStart() {
        base.OnStart();

        if (Progress > 0) {
            Forge();
        }
    }

    public override void OnUpgraded(CarriageResearchSetting upgrade) {
        base.OnUpgraded(upgrade);
        string[] parameters = upgrade.Parameter.Split('|');
        if (parameters.Length != 1) {
            Debug.LogError("第" + upgrade.ID + "号升级所需参数为([float]ProcessSpeedRatio)");
            return;
        }

        if (parameters[0].Length > 0) {
            float value = float.Parse(parameters[0]);
            ProcessSpeedRatio = value;
        }
    }

    public bool Forge() {
        for (int i = 0; i < Materials.Length; i++) {
            if (Materials[i] != null && SelectedFormula != -1) {
                TimeController.getInstance().StartCoroutine(Run());
                return true;
            }
        }
        return false;
    }

    private IEnumerator Run() {
        UpdateState("Running", true);
        while (Progress < ProcessTime) {
            Progress += Time.deltaTime * ProcessSpeedRatio;
            OnProgressUpdate?.Invoke(Progress, ProcessTime);
            yield return 1;
        }
        UpdateState("Running", false);
        Progress = 0;
        OnProgressUpdate?.Invoke(Progress, ProcessTime);
        ItemData item = new WeaponData(Formulas[SelectedFormula].ToWeapon, Materials[0], Materials[1], true);
        PublicMethod.AppendItemsInBackEnd(new ItemData[] { item });
        for (int i = 0; i < Materials.Length; i++) {
            Materials[i] = null;
        }
        SelectedFormula = -1;
        OnFinished?.Invoke();
    }
}
