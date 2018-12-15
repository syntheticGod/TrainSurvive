/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/14 12:48:55
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ProfessionPanel : MonoBehaviour {
    //以后添加sprite
    public Text professionText1;
    public Text professionText2;
    public Text professionText3;

    // Use this for initialization
    void Start () {
		
	}
	
	public void updatePanel(int personIndex)
    {
        Person person = World.getInstance().persons[personIndex];
        Profession p1 = person.getProfession(0);
        Profession p2 = person.getProfession(1);
        Profession p3 = person.getProfession(2);
        if (p1 != null)
            professionText1.text = p1.Name;
        else
            professionText1.text = "无";
        if (p2 != null)
            professionText2.text = p2.Name;
        else
            professionText2.text = "无";
        if (p3 != null)
            professionText3.text = p3.Name;
        else
            professionText3.text = "无";
    }
}
