/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/27 13:53:57
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomataTech {
    
    [SerializableAction]
    public static void Run(Tech tech) {
        World.getInstance().automata = true;
    }

}
