using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMap;
using WorldMap.Model;
using Assets._02.Scripts.zhxUIScripts;

public class Gather {
    private static int berryId = 1;
    private static int wheatId = 11;
    private static int breadId = 12;
    private static int rawMeatId = 13;
    private static int meatId = 14;
    private static int coalId = 21;
    private static int charcoalId = 22;//木炭
    private static int woodId = 23;
    private static int copperOre = 31;
    private static int copperIngot = 32;
    private static int ironOre = 33;
    private static int ironIngot = 34;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="terranType"></param>
    /// <returns>物品id</returns>
    public static int itemsFromGatherAtTerrain(SpawnPoint.TerrainEnum terranType)
    {
        int tempRate= Random.Range(0, 100);
        int result = -1;
        switch (terranType)
        {
            case SpawnPoint.TerrainEnum.PLAIN:
                if (tempRate < 60)
                    result = wheatId;
                else if(tempRate<80)
                    result = rawMeatId;
                else if (tempRate < 90)
                    result = woodId;
                else
                    result = berryId;
                break;
            case SpawnPoint.TerrainEnum.HILL:
                if (tempRate < 40)
                    result = wheatId;
                else if (tempRate < 60)
                    result = woodId;
                else if (tempRate < 80)
                    result = berryId;
                else if (tempRate < 95)
                    result = copperOre;
                else
                    result = ironOre;
                break;
            case SpawnPoint.TerrainEnum.FOREST:
                if (tempRate < 45)
                    result = woodId;
                else if (tempRate < 70)
                    result = berryId;
                else if (tempRate < 90)
                    result = rawMeatId;
                else
                    result = coalId;
                break;
            case SpawnPoint.TerrainEnum.MOUNTAIN:
                if (tempRate < 45)
                    result = copperOre;
                else if (tempRate < 60)
                    result = ironOre;
                else if (tempRate < 80)
                    result = coalId;
                else 
                    result = rawMeatId;
                break;
        }
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
            case SpawnPoint.ClimateEnum.NONE:
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
        double gatherRate = climateGatherRate * terranGatherRate * (1 + 0.3 * world.numOut);
        double randomResult = Random.Range(Mathf.Floor((float)gatherRate), Mathf.Ceil((float)gatherRate));
        int itemNums = 0;
        if (randomResult < gatherRate)
            itemNums = (int)Mathf.Floor((float)gatherRate);
        else
            itemNums = (int)Mathf.Ceil((float)gatherRate);
        Team team = Team.Instance;
        int itemId = itemsFromGatherAtTerrain(teamPlace.terrainType);
        team.Inventory.PushItem(PublicMethod.GenerateItem(itemId, itemNums)[0]);
    }
    /*
    public void pushPossibility(int )
    {
    以后可考虑使用权值式添加几率
    }*/
}
