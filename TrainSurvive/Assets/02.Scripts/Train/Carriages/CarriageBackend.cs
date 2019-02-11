/*
 * 描述：列车后端
 * 作者：刘旭涛
 * 创建时间：2019/1/28 20:02:10
 * 版本：v0.7
 */
using Assets._02.Scripts.zhxUIScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class CarriageBackend : ISerializable {
    
    #region 公有属性
    public string Name { get; set; }
    public Dictionary<int, CarriageResearchSetting> ResearchSettings {
        get {
            if (_researchSettings == null) {
                CarriageResearchSetting[] settings = ResourceLoader.GetResources<CarriageResearchSetting>("Carriages/" + Name + "/Research");
                _researchSettings = new Dictionary<int, CarriageResearchSetting>();
                foreach (CarriageResearchSetting setting in settings) {
                    _researchSettings.Add(setting.ID, setting);
                }
            }
            return _researchSettings;
        }
    }
    public SortedSet<int> UpgradedID {
        get {
            if (_upgradedID == null) {
                _upgradedID = new SortedSet<int>();
            }
            return _upgradedID;
        }
        private set {
            _upgradedID = value;
        }
    }
    public Dictionary<string, CarriageStructureSetting> StructureSettings {
        get {
            if (_structureSettings == null) {
                CarriageStructureSetting[] settings = ResourceLoader.GetResources<CarriageStructureSetting>("Carriages/" + Name + "/Structure");
                _structureSettings = new Dictionary<string, CarriageStructureSetting>();
                foreach (CarriageStructureSetting setting in settings) {
                    _structureSettings.Add(setting.Name, setting);
                }
            }
            return _structureSettings;
        }
    }
    public SerializableDictionary<string, CarriageStructure> Structures {
        get {
            if (_structures == null) {
                _structures = new SerializableDictionary<string, CarriageStructure>();
                foreach (CarriageStructureSetting setting in StructureSettings.Values) {
                    _structures.Add(setting.Name, setting.Instantiate());
                }
            }
            return _structures;
        }
        private set {
            _structures = value;
        }
    }
    #endregion

    #region 事件
    public event Action<int> OnUpgraded;
    public event Action<string, string, object> OnStructureStateChanged;
    #endregion

    #region 私有属性
    #endregion

    #region 严禁访问
    private Dictionary<int, CarriageResearchSetting> _researchSettings;
    private SortedSet<int> _upgradedID;
    private Dictionary<string, CarriageStructureSetting> _structureSettings;
    private SerializableDictionary<string, CarriageStructure> _structures;
    #endregion

    #region 序列化组
    public CarriageBackend(string name) {
        UpgradedID = new SortedSet<int>();
        Name = name;
        Start();
    }
    protected CarriageBackend(SerializationInfo info, StreamingContext context) {
        UpgradedID = (SortedSet<int>)info.GetValue("UpgradedID", typeof(SortedSet<int>));
        Structures = (SerializableDictionary<string, CarriageStructure>)info.GetValue("Structures", typeof(SerializableDictionary<string, CarriageStructure>));
        Name = info.GetString("Name");
        Start();
    }
    public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue("UpgradedID", UpgradedID);
        info.AddValue("Structures", Structures);
        info.AddValue("Name", Name);
    }
    #endregion

    #region 公有函数
    /// <summary>
    /// 研究一个升级
    /// </summary>
    public void Research(int id) {
        if (IsResearchCostsAvailable(id).Count == 0) {
            PublicMethod.ConsumeItems(ResearchSettings[id].Costs);
            UpgradedID.Add(id);
            OnUpgradedSuccess(id);
        }
    }
    /// <summary>
    /// 判断当前研究耗材是否充足
    /// </summary>
    /// <returns></returns>
    public List<ItemData> IsResearchCostsAvailable(int id) {
        return PublicMethod.CheckItems(ResearchSettings[id].Costs);
    }
    public void CallOnStructureStateChanged(string name, string state, object value) {
        OnStructureStateChanged?.Invoke(name, state, value);
    }
    #endregion

    #region 私有函数
    private void OnUpgradedSuccess(int id) {
        OnUpgraded?.Invoke(id);
        CarriageResearchSetting setting = ResearchSettings[id];
        if (setting.StructureName.Length > 0) {
            CarriageStructure structure = Structures[setting.StructureName];
            if (setting.UnlockStructure) {
                structure.Enabled = true;
                structure.OnStart();
            }
            Structures[setting.StructureName].OnUpgraded(setting);
        } else {
            Upgrade(setting);
        }
    }
    protected virtual void Upgrade(CarriageResearchSetting setting) {
        switch (setting.ID) {
            case 501: {
                CarriageStructure forgeStructure = Structures["锻造台"];
                CarriageStructure composeStructure = Structures["组装台"];
                forgeStructure.Enabled = true;
                forgeStructure.OnStart();
                composeStructure.Enabled = true;
                composeStructure.OnStart();
            }break;
        }
    }
    /// <summary>
    /// 开始工作！
    /// </summary>
    private void Start() {
        foreach (CarriageStructure structure in Structures.Values) {
            structure.CarriageBackend = this;
            if (structure.Enabled) {
                structure.OnStart();
            }
        }
    }
    #endregion
}
