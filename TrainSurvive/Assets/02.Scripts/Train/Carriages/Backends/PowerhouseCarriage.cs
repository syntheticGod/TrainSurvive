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

    #region 私有属性

    #endregion

    #region 严禁调用的隐藏变量
    #endregion

    #region 序列化组
    public PowerhouseCarriage() : base() { }
    public PowerhouseCarriage(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
    #endregion

    #region 公有函数
    #endregion

    #region 私有函数
    protected override void OnUpgradedSuccess(int id) {
        base.OnUpgradedSuccess(id);
    }
    #endregion
}
