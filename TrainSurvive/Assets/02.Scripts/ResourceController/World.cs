using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class World {
    private World() {
        //测试用 xys
        for(int i = 0; i < 3; i++)
        {
            Person p=Person.CreatePerson();
            p.name = WorldMap.StaticResource.RandomNPCName(true);
            //p.vitality= UnityEngine.Random.Range(0,10);
            persons.Add(p);
        }
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

        //map.save()待补足
        /*
        if(TimeController.instance!=null)
            game_time = TimeController.getInstance().getGameTime();
            */
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
    private double game_time=0;
    public const int mapWidth = 300;
    public const int mapHeight = 300;
    //public const int trainWidth = 10;
    //public const int trainHeight = 5;
    private uint foodIn = 0;
    private uint foodOut = 0;
    public uint foodConsumed_eachPerson = 100;
    public int energy = 0;
    private uint coal = 0;
    private uint wood = 0;
    private uint metal = 0;
    public int foodInMax = 1000;
    public int foodOutMax = 1000;
    public int energyMax = 1000;
    public int coalMax = 1000;
    public int woodMax = 1000;
    public int metalMax = 1000;
   
    public int time = 0;
    public int timeSpd = 1;
    public int dayCnt=1;
    public int weekCnt = 0;

    public bool ifOuting = false;
    public bool ifMoving = false;
    public bool ifGather = false;

    public float outVit = 0;
    public float outVitMax = 100;
    public float outMood = 0;
    public float outMoodMax = 100;
    
    public int posTrainX ;
    public int posTrainY ;
    public int posTeamX;
    public int posTeamY;
    public int distView=1;

    public int numWeapon = 101;
    public int numPersonPlayer = 1;
    public int numPersonWolrd = 101;
    public int numBuildInst = 101;


    //public gridMap[,] gridsMap=new gridMap[mapWidth, mapHeight];
    //public gridTrain[,] gridsTrain = new gridTrain[trainWidth, trainHeight];


    public WorldMap.Model.Town[] towns;
    //public List<weapon> weapons = new List<weapon>();
    public List<Person> persons = new List<Person>();
    public int[] personTeamIDArray;
    

    public List<Structure> buildInstArray = new List<Structure>();
    public bool[] buildUnlock;
    public bool[] sciUnlock;
    
    public int[] abiAllin;
    public int[] abiAllOut;
    public int numIn;
    public int numOut;

    public double getGame_time()
    {
        return game_time;
    }
    public void setGame_time(double time)
    {
        game_time = time;
    }
    public uint getCoal()
    {
        return coal;
    }
    public uint getFoodIn()
    {
        return foodIn;
    }
    public uint getFoodOut()
    {
        return foodOut;
    }
    public uint getWood()
    {
        return wood;
    }
    public uint getMetal()
    {
        return metal;
    }
    public bool setFoodOut(uint food)
    {
        if(food > foodOutMax)
        {
            foodOut = (uint)foodOutMax;
            return false;
        }
        foodOut = food;
        return true;
    }
    /// <summary>
    /// num可为负代表减少
    /// </summary>
    /// <param name="num"></param>
    /// <returns>0代表资源过少，2代表资源过多，1正常</returns>
    public int addFoodIn(int num)
    {
        if ((foodIn + num) < 0)
        {
            foodIn = 0;
            return 0;
        }
        if ((foodIn + num) > foodInMax)
        {
            foodIn = (uint)foodInMax;
            return 2;
        }
        foodIn = (uint)(foodIn + num);
        return 1;
    }
    /// <summary>
    /// num可为负代表减少
    /// </summary>
    /// <param name="num"></param>
    /// <returns>0代表资源过少，2代表资源过多，1正常</returns>
    public int addFoodOut(int num)
    {
        if ((foodOut + num) < 0)
        {
            foodOut = 0;
            return 0;
        }
        if ((foodOut + num) > foodOutMax)
        {
            foodOut = (uint)foodOutMax;
            return 2;
        }
        foodOut = (uint)(foodOut + num);
        return 1;
    }
    /// <summary>
    /// num可为负代表减少
    /// </summary>
    /// <param name="num"></param>
    /// <returns>0代表资源过少，2代表资源过多，1正常</returns>
    public int addCoal(int num)
    {
        if ((coal + num) < 0)
        {
            coal = 0;
            return 0;
        }
        if((coal + num) > coalMax)
        {
            coal = (uint)coalMax;
            return 2;
        }
        coal = (uint)(coal+num);
        return 1; 
    }
    /// <summary>
    /// num可为负代表减少
    /// </summary>
    /// <param name="num"></param>
    /// <returns>0代表资源过少，2代表资源过多，1正常</returns>
    public int addWood(int num)
    {
        if ((wood + num) < 0)
        {
            wood = 0;
            return 0;
        }
        if ((wood + num) > woodMax)
        {
            wood = (uint)woodMax;
            return 2;
        }
        wood = (uint)(wood + num);
        return 1;
    }
    /// <summary>
    /// num可为负代表减少
    /// </summary>
    /// <param name="num"></param>
    /// <returns>0代表资源过少，2代表资源过多，1正常</returns>
    public int addMetal(int num)
    {
        if ((metal + num) < 0)
        {
            metal = 0;
            return 0;
        }
        if ((metal + num) > metalMax)
        {
            metal = (uint)metalMax;
            return 2;
        }
        metal = (uint)(metal + num);
        return 1;
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
    /// num可为负代表减少
    /// </summary>
    /// <param name="num"></param>
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
    


    
    }

