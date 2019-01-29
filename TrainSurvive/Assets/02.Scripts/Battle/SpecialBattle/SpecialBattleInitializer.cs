/*
 * 描述：
 * 作者：NONE
 * 创建时间：2019/1/29 17:40:46
 * 版本：v0.7
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBattleInitializer  {
    private SpecialBattleInitializer()
    {
        //以后可能补充预加载机制 
    }
    private static SpecialBattleInitializer instance;
    public SpecialBattleInitializer getInstance()
    {
         if (instance == null)
                instance = new SpecialBattleInitializer();
            return instance;
    }
    //public 
}
