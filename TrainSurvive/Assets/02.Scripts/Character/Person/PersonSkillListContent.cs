/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/27 15:58:03
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using TTT.Resource;
using UnityEngine.UI;
using UnityEngine;

public class PersonSkillListContent : MonoBehaviour {
   
    // Use this for initialization
    void Start () {
		
	}
	
	public void updatePanel(Person p)
    {
        clearCells();
        foreach (int skillId in p.GetSkillsGot())
        {
            SkillInfo skill = StaticResource.GetSkillByID(skillId);
            Sprite skillSprite = skill.BigSprite;
            GameObject skillCell = Resources.Load("Prefabs/PersonList/skillcell") as GameObject;
            GameObject cellInstance = Instantiate(skillCell);
            Image sp = cellInstance.GetComponent<Image>();
            PersonSkillCell cell= cellInstance.GetComponent<PersonSkillCell>();
            sp.sprite = skillSprite;
            cell.skillId = skill.ID;
            cellInstance.transform.parent = gameObject.transform;
        }
    }

    public void clearCells()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
