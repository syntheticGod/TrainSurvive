/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/2/3 11:38:11
 * 版本：v0.7
 */
using Assets._02.Scripts.zhxUIScripts;
using System;
using System.Collections;
using System.Runtime.Serialization;
using TTT.Item;
using TTT.Resource;
using UnityEngine;

public class ForgeStructure : CarriageStructure {

    public ForgeStructure(string name, bool initialEnabled) : base(name, initialEnabled) { }

    protected ForgeStructure(SerializationInfo info, StreamingContext context) : base(info, context) {
        Materials = (ItemData[])info.GetValue("Materials", typeof(ItemData[]));
        UnlockMaterials = (bool[])info.GetValue("UnlockMaterials", typeof(bool[]));
        _processTime = info.GetSingle("_processTime");
        ProcessSpeedRatio = info.GetSingle("ProcessSpeedRatio");
        _componentIDs = (int[])info.GetValue("_componentIDs", typeof(int[]));
        Progress = info.GetSingle("Progress");
        SelectedComponentID = info.GetInt32("SelectedComponentID");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("Materials", Materials);
        info.AddValue("UnlockMaterials", UnlockMaterials);
        info.AddValue("_processTime", _processTime);
        info.AddValue("ProcessSpeedRatio", ProcessSpeedRatio);
        info.AddValue("_componentIDs", _componentIDs);
        info.AddValue("Progress", Progress);
        info.AddValue("SelectedComponentID", SelectedComponentID);
    }

    public ItemData[] Materials { get; set; } = new ItemData[6];
    public bool[] UnlockMaterials { get; set; } = { true, true, true, false, false, false };

    /// <summary>
    /// 处理时间
    /// </summary>
    public float ProcessTime {
        get {
            return _processTime;
        }
    }
    /// <summary>
    /// 部件ID
    /// </summary>
    public int[] ComponentIDs {
        get {
            return _componentIDs;
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
    /// 选中的组件ID
    /// </summary>
    public int SelectedComponentID { get; set; } = -1;

    public event Action OnFinished;
    public event Action<float, float> OnProgressUpdate;

    [StructurePublicField(Tooltip = "处理时间")]
    private float _processTime;
    [StructurePublicField(Tooltip = "部件ID")]
    private int[] _componentIDs;

    public override void OnStart() {
        base.OnStart();

        if (Progress > 0) {
            Forge();
        }
    }

    public override void OnUpgraded(CarriageResearchSetting upgrade) {
        base.OnUpgraded(upgrade);
        string[] parameters = upgrade.Parameter.Split('|');
        if (parameters.Length != 2) {
            Debug.LogError("第" + upgrade.ID + "号升级所需参数为([int]UnlockMaterials|[float]ProcessTime)");
            return;
        }

        if (parameters[0].Length > 0) {
            int count = int.Parse(parameters[0]);
            for (int i = 0; i < count && i < UnlockMaterials.Length; i++) {
                UnlockMaterials[i] = true;
            }
            for (int i = count; i < UnlockMaterials.Length; i++) {
                UnlockMaterials[i] = false;
                Materials[i] = null;
            }
        }
        if (parameters[1].Length > 0) {
            float value = float.Parse(parameters[1]);
            ProcessSpeedRatio = value;
        }
    }

    public float GetResultRarity() {
        float rarities = 0;
        foreach (ItemData item in Materials) {
            if (item != null) {
                rarities += (int)item.Rarity;
            }
        }
        return 0.5f + rarities / 100;
    }

    public bool Forge() {
        for (int i = 0; i < Materials.Length; i++) {
            if (Materials[i] != null && UnlockMaterials[i] && SelectedComponentID != -1) {
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
        ItemData item = new ComponentData(SelectedComponentID, 1, GetResultRarity());
        PublicMethod.AppendItemsInBackEnd(new ItemData[] { item });
        for (int i = 0; i < Materials.Length; i++) {
            Materials[i] = null;
        }
        OnFinished?.Invoke();
    }
}
