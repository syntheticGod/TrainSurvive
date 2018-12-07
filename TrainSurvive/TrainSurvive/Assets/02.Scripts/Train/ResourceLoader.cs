/*
 * 描述：资源加载器
 * 作者：刘旭涛
 * 创建时间：2018/12/1 1:10:18
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader {

    private static Dictionary<string, Object> Resources { get; } = new Dictionary<string, Object>();
    
    public static T GetResource<T>(string path) where T : Object {
        if (Resources.ContainsKey(path)) {
            return Resources[path] as T;
        }
        T o = UnityEngine.Resources.Load<T>(path);
        Resources.Add(path, o);
        return o;
    }
}
