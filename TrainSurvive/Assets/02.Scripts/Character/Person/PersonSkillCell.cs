/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/28 15:15:59
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PersonSkillCell : MonoBehaviour {
    public int skillId;
    private GameObject gcSkillPanel;
    // Use this for initialization
    void Start () {
        gcSkillPanel = GameObject.Find("gcSkillPanel");
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(onClick);
    }
	
	private void onClick()
    {
        gcSkillPanel.GetComponent<PersonSkillPanel>().selectSkill(skillId);
        Button bt = (Button)gameObject.GetComponent("Button");
        bt.Select();
    }
}
