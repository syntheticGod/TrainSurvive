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
            worldPath = Application.persistentDataPath + "/world.gd";
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            worldPath = Application.streamingAssetsPath + "/world.gd";
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
