using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager {
    private static PathManager instance;
    private string worldPath;
    private string dynamicMapPath;
    private string staticMapPath;

    private PathManager()
    {
        setPath();
    }

    private void setPath()
    {
       
        if (Application.platform == RuntimePlatform.OSXEditor|| Application.platform == RuntimePlatform.OSXPlayer)
        {
            worldPath=Application.persistentDataPath + "/world.dat";
            dynamicMapPath= Application.persistentDataPath + "/dynamicMap.txt";
            staticMapPath= Application.persistentDataPath + "/staticMap.txt";
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor|| Application.platform == RuntimePlatform.WindowsPlayer)
        {
            worldPath = Application.dataPath + "/world.dat";
            dynamicMapPath = Application.dataPath + "/dynamicMap.txt";
            staticMapPath = Application.dataPath + "/staticMap.txt";
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
}
