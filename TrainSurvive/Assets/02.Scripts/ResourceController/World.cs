using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using WorldMap;
using System;
[System.Serializable]
public class World
{
    private World()
    {

        //测试用 xys
        foodIn = (uint)foodInMax;
        //----
    }
    private static World instance;
    public static World getInstance()
    {
        if (instance == null)
        {
            //TEST：加载测试 xys
            string path = PathManager.getInstance().getWorldPath();

            if (File.Exists(path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(path, FileMode.Open);
                instance = (World)bf.Deserialize(file);
                file.Close();
            }
            else
                instance = new World();
        }
        return instance;
    }

    public void save()
    {
        //调用保存委托

        saveDelegateHandler?.Invoke();

        //地图保存
        SaveReadMap.SaveMapInfo();

        string path = PathManager.getInstance().getWorldPath();
        BinaryFormatter bf = new BinaryFormatter();
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        FileStream file = File.Create(path);
        bf.Serialize(file, this);
        file.Close();
    }
    public void initSave()
    {
        saveDelegateHandler?.Invoke();
        string path = PathManager.getInstance().getWorldPath();
        BinaryFormatter bf = new BinaryFormatter();
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        FileStream file = File.Create(path);
        bf.Serialize(file, this);
        file.Close();
    }
    public delegate void saveDelegate();

    public event saveDelegate saveDelegateHandler;

    //基本资源
    private double game_time = 0;
    public const int mapWidth = 300;
    public const int mapHeight = 300;
    //public const int trainWidth = 10;
    //public const int trainHeight = 5;
    private uint foodIn = 800;
    private uint foodOut = 0;
    public uint foodConsumed_eachPerson = 20;
    private float energy = 0;
    private uint money = 100000;
    private float electricity = 0;
    private uint foodInMax = 10000;
    //外出携带默认食物，不需要玩家选择，意味着无外带食物上限
    private uint foodOutMax = 99999999;
    private float energyMax = 1000;
    private float electricityMax = 1000;

    public float trainInventoryMaxSize = 50;    //需要使用相关方法来确定这个属性的值，然后其他的Inventory系统会自己使用它
    public float trainInventoryCurSize = 0;
    public List<ItemData> itemDataInTrain = new List<ItemData>();
    public List<ItemData> itemDataInTeam = new List<ItemData>();


    public List<int> itemIDs;
    public List<int> itemNums;

    public int time = 0;
    public int timeSpd = 1;
    public int dayCnt = 1;
    public int weekCnt = 0;
    /// <summary>
    /// 探险队是否在外面
    /// </summary>
    public bool ifTeamOuting = false;
    /// <summary>
    /// 探险队是否在移动
    /// </summary>
    public bool ifTeamMoving = false;
    /// <summary>
    /// 列车是否在移动
    /// </summary>
    public bool ifTrainMoving = false;
    /// <summary>
    /// 探险队的体力
    /// </summary>
    public float outVit { private set; get; } = 0;
    public float outVitMax { private set; get; } = 100;
    /// <summary>
    /// 探险队的心情
    /// </summary>
    public float outMood = 0;
    public float outMoodMax = 100;

    public int posTrainX;
    public int posTrainY;
    public int posTeamX;
    public int posTeamY;
    public int distView = 1;

    public int numWeapon = 101;
    public int numPersonPlayer = 1;
    public int numPersonWolrd = 101;
    public int numBuildInst = 101;


    //public gridMap[,] gridsMap=new gridMap[mapWidth, mapHeight];
    //public gridTrain[,] gridsTrain = new gridTrain[trainWidth, trainHeight];


    public WorldMap.Model.Town[] towns;
    /// <summary>
    /// 遍历所有城镇，寻找指定ID的NPC
    /// </summary>
    /// <param name="id">NPC的ID</param>
    /// <returns>
    /// NULL：找不到
    /// NOT NULL：指定ID的NPC
    /// </returns>
    public WorldMap.Model.NPC FindNPCByID(int id)
    {
        foreach (WorldMap.Model.Town town in towns)
        {
            WorldMap.Model.NPC npc = town.FindNPCByID(id);
            if (npc != null) return npc;
        }
        return null;
    }
    public List<Person> persons = new List<Person>();
    public Person GetPerson(int index)
    {
        return persons[index];
    }
    public void AddPerson(Person person)
    {
        persons.Add(person);
    }
    public int[] personTeamIDArray;


    public LinkedList<Structure> buildInstArray = new LinkedList<Structure>();
    public LinkedList<TrainCarriage> carriageInstArray = new LinkedList<TrainCarriage>();
    public bool[] carriageUnlock;
    public Tech[] techArray;
    public int techUnlock;

    public int[] abiAllin;
    public int[] abiAllOut;
    public int numIn;
    public int numOut;
    public int personNumMax = 15;

    public TaskController taskCon = null;

    /// <summary>
    /// ID自增值
    /// </summary>
    public int IncreasementOfID = 0;

    [NonSerialized]
    public ResouceBaseUI resourceUI;

    public double getGame_time()
    {
        return game_time;
    }
    public void setGame_time(double time)
    {
        game_time = time;
    }
    public float getEnergy()
    {
        return energy;
    }
    public uint getFoodIn()
    {
        return foodIn;
    }
    public uint getFoodOut()
    {
        return foodOut;
    }
    public uint getMoney()
    {
        return money;
    }
    public float getElectricity()
    {
        return electricity;
    }
    public uint getFoodOutMax()
    {
        return foodOutMax;
    }
    public uint getFoodInMax()
    {
        return foodInMax;
    }
    public float getEnergyMax()
    {
        return energyMax;
    }
    public float getElectricityMax()
    {
        return electricityMax;
    }
    /// <param name="food"></param>
    /// <returns>返回false代表资源超过最大值</returns>
    public bool setFoodIn(uint food)
    {
        bool result = true;
        if (food > foodInMax)
        {
            foodIn = foodInMax;
            result = false;
        }
        else
            foodIn = food;
        if (resourceUI != null)
        {
            resourceUI.setFoodIn(foodIn, foodInMax);
        }
        return result;
    }
    /// <param name="food"></param>
    /// <returns>返回false代表资源超过最大值</returns>
    public bool setFoodOut(uint food)
    {
        bool result = true;
        if (food > foodOutMax)
        {
            foodOut = foodOutMax;
            result = false;
        }
        else
            foodOut = food;
        if (resourceUI != null)
        {
            resourceUI.setFoodOut(foodOut, foodOutMax);
        }
        return result;
    }
    /// <param name="food"></param>
    /// <returns>返回false代表资源超过最大值</returns>
    public bool setEnergy(float energyNum)
    {
        bool result = true;
        if (energyNum > energyMax)
        {
            energy = energyMax;
            result = false;
        }
        else
            energy = energyNum;
        if (resourceUI != null)
        {
            resourceUI.setEnergy(energyNum, energyMax);
        }
        return result;
    }
    public void setFoodInMax(uint num)
    {
        foodInMax = num;
        if (resourceUI != null)
        {
            resourceUI.setFoodIn(foodIn, foodInMax);
        }
    }
    public void setFoodOutMax(uint num)
    {
        foodOutMax = num;
        if (resourceUI != null)
        {
            resourceUI.setFoodOut(foodOut, foodOutMax);
        }
    }
    public void setEnergyMax(float num)
    {
        energyMax = num;
        if (resourceUI != null)
        {
            resourceUI.setEnergy(energy, energyMax);
        }
    }
    public void setElectricityMax(float num)
    {
        electricityMax = num;
        if (resourceUI != null)
        {
            resourceUI.setElectricity(electricity, electricityMax);
        }
    }
    /// <summary>
    /// num可为负代表减少
    /// </summary>
    /// <param name="num"></param>
    /// <returns>0代表资源过少，2代表资源过多，1正常</returns>
    public int addFoodIn(int num)
    {
        int result = 1;
        if ((foodIn + num) < 0)
        {
            foodIn = 0;
            result = 0;
        }
        else if ((foodIn + num) > foodInMax)
        {
            foodIn = (uint)foodInMax;
            result = 2;
        }
        else
            foodIn = (uint)(foodIn + num);
        if (resourceUI != null)
        {
            resourceUI.setFoodIn(foodIn, foodInMax);
        }
        return result;
    }
    /// <summary>
    /// num可为负代表减少
    /// </summary>
    /// <param name="num"></param>
    /// <returns>0代表资源过少，2代表资源过多，1正常</returns>
    public int addFoodOut(int num)
    {
        int result = 1;
        if ((foodOut + num) < 0)
        {
            foodOut = 0;
            result = 0;
        }
        else if ((foodOut + num) > foodOutMax)
        {
            foodOut = (uint)foodOutMax;
            result = 2;
        }
        else
            foodOut = (uint)(foodOut + num);
        if (resourceUI != null)
        {
            resourceUI.setFoodOut(foodOut, foodOutMax);
        }
        return result;
    }
    /// <summary>
    /// num可为负代表减少
    /// </summary>
    /// <param name="num"></param>
    /// <returns>0代表资源过少，2代表资源过多，1正常</returns>
    public int addEnergy(float num)
    {
        int result = 1;
        if ((energy + num) < 0)
        {
            energy = 0;
            result = 0;
        }
        else if ((energy + num) > energyMax)
        {
            energy = energyMax;
            result = 2;
        }
        else
            energy = (energy + num);
        if (resourceUI != null)
        {
            resourceUI.setEnergy(energy, energyMax);
        }
        return result;
    }
    /// <summary>
    /// num可为负代表减少
    /// </summary>
    /// <param name="num"></param>
    /// <returns>false代表金钱不够，true代表金钱够</returns>
    public bool addMoney(int num)
    {
        bool result = true;
        if ((money + num) < 0)
        {
            result = false;
        }
        else
            money = (uint)(money + num);
        if (resourceUI != null)
        {
            resourceUI.setMoney(money);
        }
        return result;
    }
    /// <summary>
    /// num可为负代表减少
    /// </summary>
    /// <param name="num"></param>
    /// <returns>0代表资源过少，2代表资源过多，1正常</returns>
    public int addElectricity(float num)
    {
        int result = 1;
        if ((electricity + num) < 0)
        {
            electricity = 0;
            result = 0;
        }
        else if ((electricity + num) > electricityMax)
        {
            electricity = electricityMax;
            result = 2;
        }
        else
            electricity = (electricity + num);
        if (resourceUI != null)
        {
            resourceUI.setElectricity(electricity, electricityMax);
        }
        return result;
    }
    /// <summary>
    /// num可为负代表减少
    /// </summary>
    /// <param name="num"></param>
    /// <returns>0代表资源过少，2代表资源过多，1正常</returns>
    public int addOutMood(float num)
    {
        if ((outMood + num) < 0)
        {
            outMood = 0;
            return 0;
        }
        if ((outMood + num) > outMoodMax)
        {
            outMood = (uint)outMoodMax;
            return 2;
        }
        outMood = (uint)(outMood + num);
        return 1;
    }
    /// <summary>
    /// 增加/减少探险队的体力
    /// </summary>
    /// <param name="num">num可为负代表减少</param>
    /// <returns>0代表资源过少，2代表资源过多，1正常</returns>
    public int addOutVit(int num)
    {
        if ((outVit + num) < 0)
        {
            outVit = 0;
            return 0;
        }
        if ((outVit + num) > outVitMax)
        {
            outVit = (uint)outVitMax;
            return 2;
        }
        outVit = (uint)(outVit + num);
        return 1;
    }

    //食物管理
    /// <summary>
    /// 列车内人员消耗食物
    /// </summary>
    /// <returns>0代表食物不足，1代表食物充足</returns>
    public int consumeFoodIn()
    {
        return addFoodIn((int)-(foodConsumed_eachPerson * numIn));
    }
    /// <summary>
    /// 探险队人员消耗食物
    /// </summary>
    /// <returns></returns>
    public int consumeFoodOut()
    {
        return addFoodOut((int)-(foodConsumed_eachPerson * numOut));
    }
    /// <summary>
    /// 获取队伍的总属性
    /// </summary>
    /// <returns></returns>
    public int getTotalProperty()
    {
        int totalProperty = 0;
        foreach (Person p in persons)
        {
            totalProperty += p.intelligence;
            totalProperty += p.vitality;
            totalProperty += p.strength;
            totalProperty += p.technique;
            totalProperty += p.agile;
        }
        return totalProperty;
    }

    public static void destroyWorld()
    {
        instance = null;
    }
}



