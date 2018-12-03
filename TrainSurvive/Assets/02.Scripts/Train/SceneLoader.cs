/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/2 17:00:49
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    public void ChangeScene(string name) {
        SceneManager.LoadScene(name);
    }
}
