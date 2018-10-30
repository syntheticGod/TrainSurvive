/*
 * 描述：
 * 作者：����
 * 创建时间：2018/10/30 22:26:59
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ResourceController : MonoBehaviour {

    private static ResourceController instance;
    public enum speedRate
    {
        stop=0,
        normal = 1,
        fast = 2,
        veryfast = 3
    }
    //游戏里的游戏时间
    private double game_time;
    //当前场景是否允许时间流动，若不流动则为true，默认false
    private bool is_paused_scene;
    //游戏进行速度
    private speedRate game_speed;
   /// <summary>
   /// 获取静态实例
   /// </summary>
   /// <returns></returns>
    public static ResourceController getInstance()
    {
        return instance;
    }

    /// <summary>
    /// 获取用于展示的时间，即X秒，X为uint
    /// </summary>
    /// <returns></returns>
    public uint getDisplayTime()
    {
        return (uint)game_time;
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
        is_paused_scene = nextSceneTimeIsPaused;
        if (nextSceneTimeIsPaused)
            Time.timeScale = 1;
        else
            Time.timeScale= (float)game_speed;
    }




    private void Awake()
    {     
        game_time = 0;
        is_paused_scene = false;
        game_speed = speedRate.normal;
        instance = this;
        DontDestroyOnLoad(instance);
    }
    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        if (!is_paused_scene)
            instance.game_time += Time.deltaTime;

        /*
        if((instance.game_time)%1<0.15)
            Debug.Log("DisplayTime is" + instance.getDisplayTime());*/
    }
}
