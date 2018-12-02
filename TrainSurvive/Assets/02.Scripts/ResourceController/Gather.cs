using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMap;
using WorldMap.Model;
using Assets._02.Scripts.zhxUIScripts;

public class Gather {
    private static int berryId = 201;
    private static int wheatId = 211;
    private static int breadId = 212;
    private static int rawMeatId = 213;
    private static int meatId =214;
    private static int coalId = 221;
    private static int charcoalId = 222;//木炭
    private static int woodId = 223;
    private static int copperOre = 231;
    private static int copperIngot = 232;
    private static int ironOre = 233;
    private static int ironIngot = 234;
    private Dictionary<int, int> possibilityMap;
    private int totalWeight=0;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="terranType"></param>
    /// <returns>物品id</returns>
    public int itemsFromGatherAtTerrain(SpawnPoint.TerrainEnum terranType)
    {
        int result = -1;
        //这里在地形判断物品生成时，今后会需要调用科技树中的内容，来控制一些物品概率的push
        switch (terranType)
        {
            case SpawnPoint.TerrainEnum.PLAIN:
                pushPossibility(wheatId, 60);
                pushPossibility(rawMeatId, 20);
                pushPossibility(woodId, 10);
                pushPossibility(berryId, 10);               
                break;
            case SpawnPoint.TerrainEnum.HILL:
                pushPossibility(wheatId, 40);
                pushPossibility(woodId, 20);
                pushPossibility(berryId, 20);
                pushPossibility(copperOre, 15);
                pushPossibility(ironOre, 5);
                break;
            case SpawnPoint.TerrainEnum.FOREST:
                pushPossibility(woodId, 45);
                pushPossibility(berryId, 25);
                pushPossibility(rawMeatId, 20);
                pushPossibility(coalId, 10);
                break;
            case SpawnPoint.TerrainEnum.MOUNTAIN:
                pushPossibility(copperOre, 45);
                pushPossibility(ironOre, 15);
                pushPossibility(coalId, 20);
                pushPossibility(rawMeatId, 20);
                break;
        }
        result=calItem();
        return result; 
    }

    public static void gather()
    {
        World world = World.getInstance();
        Map mp = Map.GetIntanstance();
        SpawnPoint teamPlace = mp.spowns[world.posTeamX, world.posTeamY];
        double climateGatherRate = 1;
        double terranGatherRate = 1;
        switch (teamPlace.climateType)
        {
            case SpawnPoint.ClimateEnum.COLD:
                climateGatherRate = 0.3;
                break;
            case SpawnPoint.ClimateEnum.TUNDRA:
                climateGatherRate = 0.8;
                break;
            case SpawnPoint.ClimateEnum.TEMPERATE:
                climateGatherRate = 1;
                break;
            case SpawnPoint.ClimateEnum.TROPIC:
                climateGatherRate = 1.2;
                break;
            case SpawnPoint.ClimateEnum.HEAT:
                climateGatherRate = 0.4;
                break;
        }

        switch (teamPlace.terrainType)
        {
            case SpawnPoint.TerrainEnum.PLAIN:
                terranGatherRate = 1.2;
                break;
            case SpawnPoint.TerrainEnum.HILL:
                terranGatherRate = 0.8;
                break;
            case SpawnPoint.TerrainEnum.FOREST:
                terranGatherRate = 1;
                break;
            case SpawnPoint.TerrainEnum.MOUNTAIN:
                terranGatherRate = 0.7;
                break;
        }
        double gatherRate = climateGatherRate * terranGatherRate * (1 + 0.3 * world.numOut)*(1+World.getInstance().getTotalProperty(true)*0.01);
        double randomResult = Random.Range(Mathf.Floor((float)gatherRate), Mathf.Ceil((float)gatherRate));
        int itemNums = 0;
        if (randomResult < gatherRate)
            itemNums = (int)Mathf.Floor((float)gatherRate);
        else
            itemNums = (int)Mathf.Ceil((float)gatherRate);
        Team team = Team.Instance;
        Gather g = new Gather();
        int itemId = g.itemsFromGatherAtTerrain(teamPlace.terrainType);
        Item[] items= PublicMethod.GenerateItem(itemId, itemNums);
        for(int i=0;i< items.Length; i++)
            team.Inventory.PushItem(items[i]);
      
    }
    

    private void pushPossibility(int itemId,int weight)
    {
        possibilityMap.Add(itemId, weight);
        totalWeight += weight;
    }
    /// <summary>
    /// 返回物品类型id
    /// </summary>
    /// <returns></returns>
    private int calItem()
    {
        int resultWeight=Random.Range(0,totalWeight);
        int tempWeight = 0;
        int resultItemId = -1;
        foreach (KeyValuePair<int, int> pair in possibilityMap)
        {
            if (tempWeight > resultWeight)
                break;
            else
            {
                resultItemId = pair.Key;
                tempWeight += pair.Value;
            }
        }
        possibilityMap.Clear();
        totalWeight = 0;
        return resultItemId;
    }
}
