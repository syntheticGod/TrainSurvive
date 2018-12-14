/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/11 14:12:58
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//木桩型AI，不动只会挨打
namespace WorldBattle
{
    public class NoneAI : BattleActor
    {
        protected override void AIStrategy()
        {
            
        }

        protected override void otherInit()
        {
            changeSubState(ActionStateEnum.NONE);
        }

        

    }
}