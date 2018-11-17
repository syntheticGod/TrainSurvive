using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager {
    private static PathManager instance;
    private string worldPath;

    private PathManager()
    {
        setPath();
    }

    private void setPath()
    {
       
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            worldPath=Application.persistentDataPath + "/world.dat";
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor|| Application.platform == RuntimePlatform.WindowsPlayer)
        {
            worldPath = Application.dataPath + "/world.dat";
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
}
