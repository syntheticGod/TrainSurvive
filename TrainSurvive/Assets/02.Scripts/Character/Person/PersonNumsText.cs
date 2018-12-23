/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/22 10:22:14
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PersonNumsText : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Text t = gameObject.GetComponent<Text>();
        int maxP = World.getInstance().personNumMax;
        int numP = World.getInstance().persons.Count;
        t.text = "总人数：" + numP + "/" + maxP;
    }
	
	
}
