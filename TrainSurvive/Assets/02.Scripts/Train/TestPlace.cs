/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/10/29 22:26:57
 * 版本：v0.1
 */
using UnityEngine;

public class TestPlace : MonoBehaviour {

    public void Place(Facility facility) {
        PlaceFacility.Place(this, facility);
    }
}
