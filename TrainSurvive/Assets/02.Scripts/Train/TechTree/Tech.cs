/*
 * 描述：科技功能接口
 * 作者：刘旭涛
 * 创建时间：2018/11/25 23:50:05
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Tech : UnityEngine.Object {

    public enum State {
        LOCKED,
        UNLOCKED,
        COMPLETED
    }

    public State TechState { get; set; }
    public Image Image { get; set; }
}
