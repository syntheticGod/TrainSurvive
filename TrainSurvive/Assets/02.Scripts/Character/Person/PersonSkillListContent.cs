/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/27 15:58:03
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using TTT.Resource;
using UnityEngine;

public class PersonSkillListContent : MonoBehaviour {


	// Use this for initialization
	void Start () {
		
	}
	
	public void updatePanel(int personIndex)
    {
       
        Person p = World.getInstance().persons[personIndex];
        foreach(int skillId in p.GetSkillsGot())
        {
            SkillInfo skill = StaticResource.GetSkillByID(skillId);
            Debug.Log(skill.Name);
            Sprite skillSprite = skill.BigSprite;
            GameObject skillCell = Resources.Load("Prefabs/PersonList/skillcell") as GameObject;
            GameObject cellInstance = Instantiate(skillCell);
            SpriteRenderer sp = cellInstance.GetComponent<SpriteRenderer>();
            sp.sprite = skillSprite;
            cellInstance.transform.parent = gameObject.transform;
        }
        Debug.Log("技能数量:" + p.GetSkillsGot().Count);
    }
}
