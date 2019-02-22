/*
 * 描述：
 * 作者：NONE
 * 创建时间：2019/1/29 17:40:46
 * 版本：v0.7
 */
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;
using WorldMap;

public class SpecialBattleInitializer  {

    private SpecialBattleInitializer()
    {
        //以后可能补充预加载机制 
    }
    private static SpecialBattleInitializer instance;
    public static SpecialBattleInitializer getInstance()
    {
         if (instance == null)
                instance = new SpecialBattleInitializer();
            return instance;
    }
    /// <summary>
    /// 读取xml获得特殊战斗结构体
    /// </summary>
    /// <param name="battleId"></param>
    /// <returns>null代表对应战斗不存在</returns>
    public SpecialBattle loadBattle(int battleId)
    {
        SpecialBattle bt = new SpecialBattle();
        string xmlString = Resources.Load("xml/SpecialBattle").ToString();
        string XPath = string.Format("./battle[@id='{0:D}']", battleId);
        XmlDocument document = new XmlDocument();
        document.LoadXml(xmlString);
        XmlNode root = document.SelectSingleNode("battlelist");
        XmlNode aimNode = root.SelectSingleNode(XPath);
        if (aimNode == null)
            return null;
        bt.id = battleId;
        bt.name= aimNode.Attributes["name"].Value;
        bt.expNum = int.Parse(aimNode.Attributes["expNum"].Value);   
        bt.is_talk_battle = aimNode.Attributes["type"].Value == "talk";
        if (!bt.is_talk_battle)
        {
            bt.posX= aimNode.Attributes["posX"].Value;
            bt.posY = aimNode.Attributes["posY"].Value;
        }

        XmlNode monsterListNode = aimNode.SelectSingleNode("./monsterList");
        if (monsterListNode != null)
        {
            foreach (XmlElement monster in monsterListNode.ChildNodes)
            {
                int id = int.Parse(monster.Attributes["id"].Value);
                int num = int.Parse(monster.Attributes["num"].Value);
                bt.monsterList.Add(new System.ValueTuple<int, int>(id, num));
            }
        }
        XmlNode rewardListNode = aimNode.SelectSingleNode("./rewardList");
        if (rewardListNode != null)
        {
            foreach (XmlElement reward in rewardListNode.ChildNodes)
            {
                if (reward.Attributes["id"].Value == "item")
                {//或许以后额外奖励还有金钱？
                    int id = int.Parse(reward.Attributes["id"].Value);
                    int num = int.Parse(reward.Attributes["num"].Value);
                    bt.rewardList.Add(new System.ValueTuple<int, int>(id, num));
                }            
            }
        }
        return bt;
    }
    /// <summary>
    /// 在地图上生成特殊战斗,会修改map
    /// </summary>
    /// <param name="battle">结构体的位置是大区块坐标</param>
    /// <returns>false意味着该区块满了或者该战斗是对话战斗，即生成失败</returns>
    public bool generateSpecialBattle(SpecialBattle battle)
    {
        if (battle.is_talk_battle)
            return false;
        int posX;
        int posY;
        if (battle.posX == "now")
        {
            Vector2Int teamPosition = World.getInstance().PMarker.TeamMapPos;
            Vector2Int blockPos = Map.GetInstance().CalArea(teamPosition);
            posX = blockPos.x;
            posY= blockPos.y;
        }
        else
        {
            posX = int.Parse(battle.posX);
            posY = int.Parse(battle.posY);
        }
        return Map.GetInstance().generateSpecialArea(new Vector2Int(posX, posY), battle.id);
    }

    /// <summary>
    /// 读取特殊战斗（从已有的任务信息里读取，不读取xml），不存在返回null
    /// </summary>
    /// <param name="battleId"></param>
    /// <returns></returns>
    public SpecialBattle getBattle(int battleId)
    {
        return TaskController.getInstance().getBattle(battleId);
    }
}
