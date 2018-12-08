/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/7 19:21:41
 * 版本：v0.1
 */
using System;
using UnityEngine;

public abstract class BaseItem : MonoBehaviour
{
    protected void Awake()
    {
        CreateModel();
        InitModel();
        PlaceModel();
    }
    protected void Start()
    { }

    protected abstract void PlaceModel();

    protected abstract void InitModel();

    protected abstract void CreateModel();
}
