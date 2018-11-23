/*
 * 描述：
 * 作者：Gong Chen
 * 创建时间：2018/10/30 22:26:59
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class TimeController : MonoBehaviour {
    public enum speedRate
    {
        stop = 0,
        normal = 1,
        fast = 2,
        veryfast = 3
    }

    
    private static TimeController instance;
    
    //管理所有的时间变动
    //private SerializableDictionary<Time_Resource_Loop_Key, Time_Resource_Loop_Value> Loop_Map;

    //游戏里的游戏时间(unty秒)
    private double game_time;
    //当前场景是否允许时间流动，若不流动则为true，默认false
    private bool is_paused_scene;
    //游戏进行速度
    private speedRate game_speed;
    /// <summary>
    /// 现实1S=游戏game_time_scale分钟
    /// </summary>
    private const double game_time_scale = 10.0f;
    private World world;

    private IEnumerator Time_PerSecond_Corutine()
    {

        yield return new WaitForSeconds((float)(System.Math.Ceiling(game_time) - game_time));
        while (true)
        {      
            int today_minutes = ((int)(game_time) * (int)game_time_scale) % 1440;
            if ((today_minutes == 360) || (today_minutes == 720) || (today_minutes == 1080))
            {
                world.consumeFoodIn();
                world.consumeFoodOut();
            }
            if (world.ifMoving)
            {
                if (world.ifOuting)
                {
                    world.addOutMood(-1);
                    world.addOutVit(-2);
                }
                else
                {
                    world.addCoal(-20);
                }
            }
            else
            {
                if (world.ifOuting)
                {
                    if (world.ifGather)
                    {
                        world.gather();
                    }
                    else
                        world.addOutMood(2);
                }
            }
            yield return new WaitForSeconds((float)(System.Math.Ceiling(game_time)- game_time));
        }
        /*
          //注意用键值遍历时修改值可能会报错
          List<Time_Resource_Loop_Key> keyList = new List<Time_Resource_Loop_Key>();
          foreach (Time_Resource_Loop_Key key in Loop_Map.Keys)
          {

              Time_Resource_Loop_Value value = Loop_Map[key];
              value.time_remain = value.time_remain - 1;
              if (value.time_remain % key.time_interval == 0)
              {
                  if (key.loops != 0)
                  {
                      key.loops--;
                      if (key.loops == 0)
                          keyList.Add(key);
                  }
                  else
                  {
                      value.time_remain = key.time_interval;
                  }
              }
          }
          foreach (Time_Resource_Loop_Key key in keyList)
              Loop_Map.Remove(key);
              */
    }



    /// <summary>
    /// 获取静态实例
    /// </summary>
    /// <returns></returns>
    public static TimeController getInstance()
    {
        return instance;
    }

    /// <summary>
    /// 获取用于展示的时间，即X分钟，X为uint
    /// </summary>
    /// <returns></returns>
    public uint getDisplayTime()
    {
        return (uint)(game_time* game_time_scale);
    }
    /// <summary>
    /// 获取游戏里的游戏时间，单位秒
    /// </summary>
    /// <returns></returns>
    public double getGameTime()
    {
        return game_time;
    }
    /// <summary>
    /// 设置游戏进行速度，在时停场景下调用没效果
    /// </summary>
    /// <param name="rate"></param>
    public void setGameSpeedRate(speedRate rate)
    {
        game_speed= rate;
        if (!is_paused_scene)
            Time.timeScale = (float)game_speed;
        else
            Time.timeScale = 1;
    }

    public speedRate getGameSpeedRate()
    {
        return game_speed;
    }
    /// <summary>
    /// 每次切换场景时必须调用
    /// </summary>
    /// <param name="nextSceneTimeIsPaused">下个场景是否是时停场景</param>
    public void changeScene(bool nextSceneTimeIsPaused)
    {
        if(is_paused_scene!= nextSceneTimeIsPaused)
        {
            is_paused_scene = nextSceneTimeIsPaused;
            if (nextSceneTimeIsPaused)
            {
                StopCoroutine("Time_PerSecond_Corutine");
                Time.timeScale = 1;
            }
            else
            {
                StartCoroutine("Time_PerSecond_Corutine");
                Time.timeScale = (float)game_speed;
            }
        }   
    }
    
    public void saveGameTime()
    {
        world = World.getInstance();
        world.setGame_time(game_time);
    }


    private void Awake()
    {
        world = World.getInstance();
        game_time = world.getGame_time();
        world.saveDelegateHandler += saveGameTime;
        is_paused_scene = false;
        game_speed = speedRate.normal;
        instance = this;
        
        DontDestroyOnLoad(instance);
    }
    // Use this for initialization
    void Start () {
        StartCoroutine("Time_PerSecond_Corutine");
    }
	
	// Update is called once per frame
	void Update () {
        if (!is_paused_scene)
            instance.game_time += Time.deltaTime;

        
        //if((instance.game_time)%1<0.3)
         //   Debug.Log("DisplayTime is" + instance.getDisplayTime());
        
    }
}


/*
public class Time_Resource_Loop_Key
{
    /// <summary>
    /// 资源种类
    /// </summary>
    public string resource_type;
    /// <summary>
    /// 资源变化的时间间隔（标准unity时间）
    /// </summary>
    public uint time_interval;
    /// <summary>
    /// 执行几次，为0时代表无限循环
    /// </summary>
    public uint loops;

    public Time_Resource_Loop_Key(string type, uint interval, uint lo)
    {
        resource_type = type;
        time_interval = interval;
        loops = lo;    
    }
}

public class Time_Resource_Loop_Value
{
    /// <summary>
    /// 单词资源变化量
    /// </summary>
    public int num;
    /// <summary>
    /// 记录执行剩余时间（标准unity时间)
    /// </summary>
    public uint time_remain;


    public Time_Resource_Loop_Value(int num, uint time_remain)
    {
        this.num = num;
        this.time_remain = time_remain;
    }
}



[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> _keys = new List<TKey>();
    [SerializeField]
    private List<TValue> _values = new List<TValue>();

    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();
        _keys.Capacity = this.Count;
        _values.Capacity = this.Count;
        foreach (var kvp in this)
        {
            _keys.Add(kvp.Key);
            _values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();
        int count = Mathf.Min(_keys.Count, _values.Count);
        for (int i = 0; i < count; ++i)
        {
            this.Add(_keys[i], _values[i]);
        }
    }
}



   //对于各种资源的变化需要增加控制器，比如列车控制器，在开车时调用addTime(当前消耗），停车时调用removeTime（当前消耗），在升级科技后会导致消耗值的变化，要在升级科技事件时增加差值。
   //以下方法需要在控制里进一步封装，即如时间间隔类的参数直接调用预设值。

   /// <summary>
   /// 添加资源时间循环,如果输入有误（资源变化间隔为0，resouce为基类）则什么都不会发生
   /// </summary>
   /// <param name="resource_changed">要改变的资源（内部设置变化量）</param>
   /// <param name="time_interval">资源变化的时间间隔（标准Unity时间，受timescale影响，单位秒，如游戏时间10MIN则设为1S）</param>
   /// <param name="loops">执行几次，为0时代表无限循环</param>
   /// <param name="isMinus">true时代表减少对应资源，反之增加</param>
   public void addTime_Resouce_Loop( Resource resource_changed,uint time_interval,uint loops,bool isMinus) {
       if (time_interval == 0)
           return;
       uint total_times = time_interval * loops;
       if (resource_changed.getTypeName() == "Resource")
           return;
       Time_Resource_Loop_Key key = new Time_Resource_Loop_Key(resource_changed.getTypeName(), time_interval, loops);
       int nums_change = (int)resource_changed.num;
       if (isMinus)
           nums_change = -nums_change;

       if (Loop_Map.ContainsKey(key)){
           Time_Resource_Loop_Value exist_value = Loop_Map[key];
           if ((exist_value.num + nums_change) == 0)
               Loop_Map.Remove(key);
           else
           {
               Time_Resource_Loop_Value value = new Time_Resource_Loop_Value(nums_change+ exist_value.num, exist_value.time_remain);
               Loop_Map[key] = value;
           }
       }
       else
       {
           Time_Resource_Loop_Value value = new Time_Resource_Loop_Value(nums_change, (loops == 0 ? time_interval : time_interval * loops));
           Loop_Map.Add(key, value);
       }  
   }



   /// <summary>
   /// 取消资源时间循环,如果输入有误（资源变化间隔为0，resouce为基类）则什么都不会发生
   /// </summary>
   /// <param name="resource_changed">要改变的资源（内部设置变化量）</param>
   /// <param name="time_interval">资源变化的时间间隔（标准Unity时间，受timescale影响，单位秒，如游戏时间10MIN则设为1S）</param>
   /// <param name="loops">执行几次，为0时代表无限循环</param>
   /// <param name="isMinus">true时代表减少对应资源，反之增加</param>
   /// <returns>如果取消成功则返回true，反之false</returns>
   public bool removeTime_Resouce_Loop(Resource resource_changed, uint time_interval, uint loops, bool isMinus)
   {
       bool is_successful = true;
       if (time_interval == 0)
           return false;
       uint total_times = time_interval * loops;
       if (resource_changed.getTypeName() == "Resource")
           return false;
       Time_Resource_Loop_Key key = new Time_Resource_Loop_Key(resource_changed.getTypeName(), time_interval, loops);
       int nums_change = ((int)resource_changed.num)*-1;
       if (isMinus)
           nums_change = -nums_change;

       if (Loop_Map.ContainsKey(key))
       {
           Time_Resource_Loop_Value exist_value = Loop_Map[key];
           if ((exist_value.num + nums_change) == 0)
               Loop_Map.Remove(key);
           else
           {
               Time_Resource_Loop_Value value = new Time_Resource_Loop_Value(nums_change + exist_value.num, exist_value.time_remain);
               Loop_Map[key] = value;
           }
       }
       else
       {
           is_successful = false;
       }

       return is_successful;
   }

   /*
   /// <summary>
   /// 获取当前系统总体资源趋势（是否充足），待补足
   /// </summary>
   /// <returns>为true则资源充足，反之缺乏</returns>
   public bool getTime_Resouce_LoopStatus(Resource re)
   {
       return true;
   }
   */
