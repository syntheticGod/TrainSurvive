/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/1/28 20:01:44
 * 版本：v0.7
 */
using System.Runtime.Serialization;
using UnityEngine;

public class PowerhouseCarriage : CarriageBackend {

    #region 公有属性
    public override string Name { get; } = "动力室";
    #endregion
    
    #region 序列化组
    public PowerhouseCarriage() : base() { }
    public PowerhouseCarriage(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
    #endregion
    
    #region 私有函数
    protected override void OnUpgradedSuccess(int id) {
        base.OnUpgradedSuccess(id);
        if (201 <= id && id <= 205) {
            UpdateConversionRate(float.Parse(ResearchSettings[id].Parameter));
        } else if(206 <= id && id <= 210) {
            UpdateMaxEnergy(int.Parse(ResearchSettings[id].Parameter));
        } else if(211 <= id && id <= 215) {
            UpdateSolorEnergy(int.Parse(ResearchSettings[id].Parameter));
        }
    }
    private void UpdateConversionRate(float rate) {
        Item_EnergyStructure structure = Structures["焚烧炉"] as Item_EnergyStructure;
        structure.ConversionRateRatio = rate / structure.ConversionRate;
    }
    private void UpdateMaxEnergy(int max) {
        // TODO
    }
    private void UpdateSolorEnergy(int count) {
        // TODO
    }
    #endregion
}
