/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/28 15:32:39
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using TTT.Resource;
using UnityEngine.UI;
using UnityEngine;

public class PersonSkillPanel : MonoBehaviour {

    public Text skillDescription;
    public GameObject carryskill1;
    public GameObject carryskill2;
    public Button carry_bt;
    public Button uncarry_bt;
    public PersonSkillListContent skillListContent;
    private int skillSelected_id = -1;
    private int person_index = -1;
    // Use this for initialization
    void Start () {
		
	}
	public void updatePanel(int personIndex)
    {
        carry_bt.interactable = false;
        uncarry_bt.interactable = false;
        person_index = personIndex;
        skillSelected_id = -1;
        Person p = World.getInstance().Persons[person_index];
        skillListContent.updatePanel(p);
        update_skillDescription();
        int skillId_1 = p.getSkillCarryed(1);
        int skillId_2 = p.getSkillCarryed(2);
        if (skillId_1 == -1)
        {
            carryskill1.GetComponent<PersonSkillCell>().skillId = -1;
            carryskill1.GetComponent<Image>().sprite = null;
        }
        else
        {
            carryskill1.GetComponent<PersonSkillCell>().skillId = skillId_1;
            carryskill1.GetComponent<Image>().sprite = StaticResource.GetSkillByID(skillId_1).SmallSprite;
        }
        if (skillId_2 == -1)
        {
            carryskill2.GetComponent<PersonSkillCell>().skillId = -1;
            carryskill2.GetComponent<Image>().sprite = null;
        }
        else
        {
            carryskill2.GetComponent<PersonSkillCell>().skillId = skillId_2;
            carryskill2.GetComponent<Image>().sprite = StaticResource.GetSkillByID(skillId_2).SmallSprite;
        }
    }

    public void selectSkill(int skillId)
    {
        if (skillId != -1)
        {
            carry_bt.interactable = true;
            uncarry_bt.interactable = true;
            skillSelected_id = skillId;
            update_skillDescription();
        }
    }
	public void carrySkill()
    {
        if (skillSelected_id != -1)
        {
            Person p = World.getInstance().Persons[person_index];
            p.carry_skill(skillSelected_id);
            int skillId_1=p.getSkillCarryed(1);
            int skillId_2 = p.getSkillCarryed(2);
            if (skillId_1 == -1)
            {
                carryskill1.GetComponent<PersonSkillCell>().skillId = -1;
                carryskill1.GetComponent<Image>().sprite = null;
            }
            else
            {
                carryskill1.GetComponent<PersonSkillCell>().skillId = skillId_1;
                carryskill1.GetComponent<Image>().sprite = StaticResource.GetSkillByID(skillId_1).SmallSprite;
            }
            if (skillId_2 == -1)
            {
                carryskill2.GetComponent<PersonSkillCell>().skillId = -1;
                carryskill2.GetComponent<Image>().sprite = null;
            }
            else
            {
                carryskill2.GetComponent<PersonSkillCell>().skillId = skillId_2;
                carryskill2.GetComponent<Image>().sprite = StaticResource.GetSkillByID(skillId_2).SmallSprite;
            }
        }
    }

    public void uncarrySkill()
    {
        if (skillSelected_id != -1)
        {
            Person p = World.getInstance().Persons[person_index];
            p.uncarry_skill(skillSelected_id);
            int skillId_1 = p.getSkillCarryed(1);
            int skillId_2 = p.getSkillCarryed(2);
            if (skillId_1 == -1)
            {
                carryskill1.GetComponent<PersonSkillCell>().skillId = -1;
                carryskill1.GetComponent<Image>().sprite = null;
            }
            else
            {
                carryskill1.GetComponent<PersonSkillCell>().skillId = skillId_1;
                carryskill1.GetComponent<Image>().sprite = StaticResource.GetSkillByID(skillId_1).SmallSprite;
            }
            if (skillId_2 == -1)
            {
                carryskill2.GetComponent<PersonSkillCell>().skillId = -1;
                carryskill2.GetComponent<Image>().sprite = null;
            }
            else
            {
                carryskill2.GetComponent<PersonSkillCell>().skillId = skillId_2;
                carryskill2.GetComponent<Image>().sprite = StaticResource.GetSkillByID(skillId_2).SmallSprite;
            }
        }
    }

    private void update_skillDescription()
    {
        if (skillSelected_id == -1)
        {
            skillDescription.text = "";
        }
        else
        {
            SkillInfo info=StaticResource.GetSkillByID(skillSelected_id);
            string skill_str = "";
            skill_str += ("技能名：" + info.Name + "\n");
            skill_str += ("类型：" + info.TypeInfo + "\n");
            skill_str += ("需要ap：" + info.AP + "\n");
            skill_str += "\n";
            skill_str += info.Description;
            skillDescription.text = skill_str;
        }
    }
}
