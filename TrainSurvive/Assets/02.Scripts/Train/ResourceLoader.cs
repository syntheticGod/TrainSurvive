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

    private static Dictionary<string, object> Cache { get; } = new Dictionary<string, object>();
    
    public static T GetResource<T>(string path, bool cache = true) where T : Object {
        if (cache && Cache.ContainsKey(path)) {
            return Cache[path] as T;
        }
        T o = Resources.Load<T>(path);
        if (cache)
            Cache.Add(path, o);
        return o;
    }

    public static T[] GetResources<T>(string path, bool cache = true) where T : Object {
        if (cache && Cache.ContainsKey(path)) {
            return Cache[path] as T[];
        }
        T[] o = Resources.LoadAll<T>(path);
        if (cache)
            Cache.Add(path, o);
        return o;
    }
}
