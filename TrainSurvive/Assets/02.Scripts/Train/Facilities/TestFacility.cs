/*
 * 描述：用于测试的建筑设施
 * 作者：刘旭涛
 * 创建时间：2018/10/29 22:11:02
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFacility : Facility {

    public override void OnBuildCompleted() {
        Debug.Log("BUILT! " + Name);
    }
}
