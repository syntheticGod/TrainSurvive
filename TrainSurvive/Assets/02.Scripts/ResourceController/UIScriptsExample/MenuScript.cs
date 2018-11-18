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
using UnityEngine.UI;


public class MenuScript : MonoBehaviour {
    TimeController rc;
    public Text timeText;
	// Use this for initialization
	void Start () {
        rc = TimeController.getInstance();
    }
	
	// Update is called once per frame
	void Update () {
        timeText.text = rc.getDisplayTime().ToString();
    }
    public void saveGame(){
        World.getInstance().save();
    }
    public void pauseGame()
    {
        rc.setGameSpeedRate(TimeController.speedRate.stop);
    }
    public void setNormalSpeed()
    {
        rc.setGameSpeedRate(TimeController.speedRate.normal);
  
    }
    public void setHighSpeed()
    {
        rc.setGameSpeedRate(TimeController.speedRate.fast);
        
    }
    public void setMaxSpeed()
    {
        rc.setGameSpeedRate(TimeController.speedRate.veryfast);
    }

    public void changeToNoTimeScene(string sceneName)
    {
        
        //以下切换场景时必须调用
        rc.changeScene(true);
        SceneManager.LoadScene(sceneName);
    }

    public void changeToHasTimeScene(string sceneName)
    {
        Debug.Log("changeToHasTime");
        //以下切换场景时必须调用
        rc.changeScene(false);
        SceneManager.LoadScene(sceneName);
    }

    
}
