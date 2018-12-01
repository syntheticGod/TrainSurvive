/*
 * 描述：
 * 作者：Gong Chen
 * 创建时间：2018/11/29 15:59:04
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class GameSave  {
    //slotIndex是0基的，buttonIndex是1基的
    private static GameSave instance;
    public static int saveSlotNums = 32;
    public bool folderIsCreated ;
    private int slotIndexUsedAtNow;//该值只有存档面板控件才能改变
    private bool[] slotIsUsed ;
    public string[] slotDescription;


    private GameSave()
    {
        folderIsCreated = false;
        slotIndexUsedAtNow = 0;
        slotIsUsed = new bool[saveSlotNums];
        slotDescription = new string[saveSlotNums];
        for (int i = 0; i < saveSlotNums; i++)
        {
            slotIsUsed[i] = false;
            slotDescription[i] = "无记录";
        }
            
    }

    public static GameSave getInstance()
    {
        if (instance == null)
        {         
           string path = PathManager.getGameSavePath();           
           if (File.Exists(path))
           {
               BinaryFormatter bf = new BinaryFormatter();
               FileStream file = File.Open(path, FileMode.Open);
               instance = (GameSave)bf.Deserialize(file);
               file.Close();
           }
           else                  
            instance = new GameSave();
        }      
        return instance;
    }

    //储存总体的存档状态，不包括各存档自身具体的状态
    public void save()
    {
        string path = PathManager.getGameSavePath();
        BinaryFormatter bf = new BinaryFormatter();
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        FileStream file = File.Create(path);
        bf.Serialize(file, this);
        file.Close();
    }


    /// <summary>
    /// 选择存档时调用
    /// </summary>
    /// <param name="index"></param>
    public void setSlotIndexUsedAtNow(int index)
    {
        slotIndexUsedAtNow = index;
        PathManager pm = PathManager.getInstance();
        pm.setPath(slotIndexUsedAtNow);
    }
    public int getSlotIndexUsedAtNow()
    {
        return slotIndexUsedAtNow;
    }
    /// <summary>
    /// 新建游戏
    /// </summary>
    public void createGame()
    {
        slotDescription[slotIndexUsedAtNow] = "游戏时间：" + 0 + "分钟";
        slotIsUsed[slotIndexUsedAtNow] = true;
        save();
        World world = World.getInstance();
        world.save();
    }


    /// <summary>
    /// 覆盖式存储
    /// </summary>
    public void saveGame()
    {
       //以后可能还要取world里的属性，比如人数和列车所在地等，现在暂时只用时间
        TimeController timeCon = TimeController.getInstance();
        slotDescription[slotIndexUsedAtNow] = "游戏时间：" + timeCon.getDisplayTime() + "分钟";
        slotIsUsed[slotIndexUsedAtNow] = true;
        save();
        World world = World.getInstance();
        world.save();
    }

    public void deleteGame()
    {
        slotDescription[slotIndexUsedAtNow] = "无记录";
        slotIsUsed[slotIndexUsedAtNow] = false;
        save();
        //与上面不同，直接清空文件夹
        PathManager pm = PathManager.getInstance();
        DirectoryInfo dir = new DirectoryInfo(pm.getSlotRootPath());
        FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
        foreach (FileSystemInfo i in fileinfo)
        {
            if (i is DirectoryInfo)            //判断是否文件夹
            {
                DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                subdir.Delete(true);          //删除子目录和文件
            }
            else
            {
                File.Delete(i.FullName);      //删除指定文件
            }
        }
    }

    public bool presentIndexHasData()
    {
        return slotIsUsed[slotIndexUsedAtNow];
    }
}
