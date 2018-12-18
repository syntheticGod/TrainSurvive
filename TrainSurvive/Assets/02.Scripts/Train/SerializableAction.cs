/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/11 01:15:37
 * 版本：v0.1
 */
 using System;
using System.Reflection;
using UnityEngine;

[Serializable]
public class SerializableAction {

    [SerializeField]
    private string Class;
    [SerializeField]
    protected string[] AcceptTypes;
    [SerializeField]
    private string[] MethodCandidateNames;
    [SerializeField]
    private int CandidateSelection;
    [SerializeField]
    private UnityEngine.Object ClassObject;

    private MethodInfo _method;
    protected MethodInfo Method {
        get {
            if (_method == null && MethodCandidateNames.Length > 0) {
                string[] split = MethodCandidateNames[CandidateSelection].Split('|');
                string methodName = split[0];
                if (split[1].Length == 0) {
                    _method = Type.GetType(Class).GetMethod(methodName, new Type[] { });
                } else {
                    string[] methodParamNames = split[1].Split(',');
                    Type[] types = new Type[methodParamNames.Length];
                    for (int i = 0; i < types.Length; i++) {
                        types[i] = Type.GetType(methodParamNames[i]);
                    }
                    _method = Type.GetType(Class).GetMethod(methodName, types);
                }
            }
            return _method;
        }
    }

    public void Invoke() {
        Method?.Invoke(null, null);
    }
}

[Serializable]
public class SerializableAction<T> : SerializableAction {
    
    public void Invoke(T o) {
        Method?.Invoke(null, new object[] { o });
    }

    public SerializableAction() {
        AcceptTypes = new string[]{ typeof(T).FullName };
    }
}
