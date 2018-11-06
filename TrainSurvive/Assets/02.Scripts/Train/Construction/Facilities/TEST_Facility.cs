/*
 * 描述：用于测试的建筑设施
 * 作者：刘旭涛
 * 创建时间：2018/10/29 22:11:02
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;

public class TEST_Facility : Facility {
    public override string Name { get; } = "Bili";
    public override float WorkAll { get; } = 5;
    public override Cost[] Costs { get; } = {
        new Cost(){ItemID = 0, Count = 0}
    };

    protected override void OnRemove() {
        Debug.Log(Name + ": OnRemove");
    }

    protected override void OnStart() {
        Debug.Log(Name + ": OnStart");
    }

    protected override void OnStop() {
        Debug.Log(Name + ": OnStop");
    }
}
