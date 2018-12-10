/*
 * 描述：
 * 作者：Gong Chen
 * 创建时间：2018/11/24 23:14:24
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets._02.Scripts.zhxUIScripts;
public class PersonTextPanel : MonoBehaviour {
    public Text vit;
    public Text str;
    public Text agl;
    public Text tec;
    public Text intl;
    public Text hp;
    public Text ap;
    public Text hpRec;
    public Text apRec;
    public UnitInventoryCtrl WeaponGridCtrl;
    private int indexOfpersonUsed=-1;
    // Use this for initialization
    void Start () {
       
    }


    public void updatePanel(int personIndex, bool isSelectPeople = true)
    {
        if (isSelectPeople)
        {
            if (WeaponGridCtrl.grid)
            {
                int personID = GameObject.Find("gcTextPanel").GetComponent<PersonTextPanel>().getIndexOfpersonUsed();
                Person curPerson = World.getInstance().persons[personID];
                GameObject.Find("gcTextPanel").GetComponent<PersonTextPanel>().updatePanel(personID, false);
                WeaponGridCtrl.grid.SetActive(false);
                WeaponGridCtrl.ClearWithNotDestroyMyself();
            }
        }
           
        
        Person p = World.getInstance().persons[personIndex];
        //Weapon w = (Weapon)PublicMethod.GenerateItem(p.weaponId)[0];  
        vit.text = "体力：" + p.vitality;
        str.text = "力量：" + p.strength;
        agl.text = "敏捷：" + p.agile;
        tec.text = "技巧：" + p.technique;
        intl.text = "智力：" + p.intelligence;
        hp.text = "hp：" + p.getApMax();
        ap.text = "ap：" + p.getApMax();
        hpRec.text = "hp恢复：" + p.getHpRec();
        apRec.text = "ap恢复：" + p.getApRec();
        indexOfpersonUsed = personIndex;
        if (p.hasWeapon && isSelectPeople)
        {
            WeaponGridCtrl.GeneratorItem(p.weaponId);
        }
    }

    /// <summary>
    /// 返回-1代表当前没有任何人物被选中
    /// </summary>
    /// <returns></returns>
    public int getIndexOfpersonUsed()
    {
        return indexOfpersonUsed;
    }
}
