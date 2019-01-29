/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/1/29 15:19:16
 * 版本：v0.7
 */
using System;
using UnityEngine;

public class CarriageManager : MonoBehaviour {
    
    #region 公有属性
    public static CarriageManager Instance { get; private set; }
    #endregion

    #region 私有属性
    #endregion

    #region 严禁调用的隐藏变量
    #endregion

    #region 生命周期
    private void Awake() {
        Instance = this;
        Load();
    }
    #endregion

    #region 公有函数
    #endregion

    #region 私有函数
    private void Load() {
        Transform carriagesTransform = GameObject.Find("Carriages").transform;
        for (int i = 0; i < carriagesTransform.childCount; i++) {
            CarriageGameObject carriageObject = carriagesTransform.GetChild(i).GetComponent<CarriageGameObject>();
            if (World.getInstance().carriageBackends.ContainsKey(carriageObject.BackendClass)) {
                carriageObject.CarriageBackend = World.getInstance().carriageBackends[carriageObject.BackendClass];
            } else {
                CarriageBackend backend = Type.GetType(carriageObject.BackendClass).GetConstructor(new Type[] { }).Invoke(new object[] { }) as CarriageBackend;
                World.getInstance().carriageBackends.Add(carriageObject.BackendClass, backend);
                carriageObject.CarriageBackend = backend;
            }
        }
    }
    #endregion
}
