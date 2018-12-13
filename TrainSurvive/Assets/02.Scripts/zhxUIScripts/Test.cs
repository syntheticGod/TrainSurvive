/*
 * 描述：
 * 作者：张皓翔
 * 创建时间：2018/12/9 15:53:12
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets._02.Scripts.zhxUIScripts;

public class Test : MonoBehaviour {

	public void ConsumeTestFun()
    {
        ItemData[] consumeList = new ItemData[] {new ItemData(211,2),   //本次消耗中，ID为111的物品消耗2个
                                                 new ItemData(212,5),   //           ID为222的物品消耗5个
                                                 new ItemData(213,6),   //  ..........................
                                                 new ItemData(214,1)};  //  ..........................
        if (PublicMethod.ConsumeItems(consumeList))
        {
            Debug.Log("后台成功扣除物品");
            //成功进行后台扣除，实现对应功能
        }
        else
        {
            //后台物品数不足，扣除失败。
            Debug.Log("物品不够消耗");
        }
    }

    public void AppendTestFun()
    {
        ItemData[] appendList = new ItemData[] {new ItemData(211,2),   //本次消耗中，ID为111的物品消耗2个
                                                 new ItemData(212,5),   //           ID为222的物品消耗5个
                                                 new ItemData(213,6),   //  ..........................
                                                 new ItemData(214,1)};  //  ..........................
        if (PublicMethod.AppendItemsInBackEnd(appendList))
        {
            Debug.Log("后台成功添加物品");
            //成功进行后台添加，实现对应功能
        }
        else
        {
            //添加失败
            Debug.Log("容器容量不够添加");
        }

    }

    private void Awake()
    {
        World.getInstance().persons.Add(Person.RandomPerson());
        World.getInstance().persons.Add(Person.RandomPerson());
        World.getInstance().persons.Add(Person.RandomPerson());
        World.getInstance().persons.Add(Person.RandomPerson());
        World.getInstance().persons.Add(Person.RandomPerson());
    }
}
