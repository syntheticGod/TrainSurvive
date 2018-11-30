using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PathManager {
    private static PathManager instance;
    
    private string worldPath;
    private string dynamicMapPath;
    private string staticMapPath;
    private string slotRootPath;
    private PathManager()
    {
        setPath(GameSave.getInstance().getSlotIndexUsedAtNow());
    }

    public void setPath(int slotIndexUsedAtNow)
    {
        GameSave saveCondition = GameSave.getInstance();
        
        if (Application.platform == RuntimePlatform.OSXEditor|| Application.platform == RuntimePlatform.OSXPlayer)
        {
            if (!saveCondition.folderIsCreated)
            {
                for (int i = 0; i < GameSave.saveSlotNums; i++)
                {
                    Directory.CreateDirectory(Application.persistentDataPath  + "/"+i);
                }
                saveCondition.folderIsCreated = true;
            }
            worldPath =Application.persistentDataPath + "/"+ slotIndexUsedAtNow+ "/world.dat";
            dynamicMapPath= Application.persistentDataPath + "/" + slotIndexUsedAtNow  + "/dynamicMap.txt";
            staticMapPath= Application.persistentDataPath + "/" + slotIndexUsedAtNow + "/staticMap.txt";
            slotRootPath = Application.persistentDataPath + "/" + slotIndexUsedAtNow;
           
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor|| Application.platform == RuntimePlatform.WindowsPlayer)
        {
            if (!saveCondition.folderIsCreated)
            {
                for (int i = 0; i < GameSave.saveSlotNums; i++)
                {
                    Directory.CreateDirectory(Application.dataPath+ "/saveRecord"+ "/" + i);
                }
                saveCondition.folderIsCreated = true;
            }
            worldPath = Application.dataPath + "/saveRecord" + "/" + slotIndexUsedAtNow +  "/world.dat";
            dynamicMapPath = Application.dataPath + "/saveRecord" + "/" + slotIndexUsedAtNow + "/dynamicMap.txt";
            staticMapPath = Application.dataPath + "/saveRecord" + "/" + slotIndexUsedAtNow + "/staticMap.txt";
            slotRootPath = Application.dataPath + "/saveRecord" + "/" + slotIndexUsedAtNow;         
        }
        
    }

    public static PathManager getInstance()
    {
        if (instance == null)
            instance = new PathManager();
        return instance;
    }
    public string getWorldPath()
    {
        return worldPath;
    }
    public string getDynamicMapPath()
    {
        return dynamicMapPath;
    }
    public string getStaticMapPath()
    {
        return staticMapPath;
    }
    public string getSlotRootPath()
    {
        return slotRootPath;
    }
    public static string getGameSavePath()
    {
        string gameSavePath="";
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)       
            gameSavePath = Application.persistentDataPath + "/gameSave.dat"; 
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            gameSavePath = Application.dataPath + "/saveRecord"+ "/gameSave.dat";
        return gameSavePath;
    }

     
}
