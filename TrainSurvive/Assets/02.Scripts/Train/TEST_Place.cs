/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/10/29 22:26:57
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.SceneManagement;

public class TEST_Place : MonoBehaviour {

    public void Place(int facility) {
        ConstructionManager.Instance.Place(facility);
    }

    public void ChangeScene() {
        SceneManager.LoadScene("MapScene");
    }

    public void ChangeScene(string name) {
        SceneManager.LoadScene(name);
    }
}
