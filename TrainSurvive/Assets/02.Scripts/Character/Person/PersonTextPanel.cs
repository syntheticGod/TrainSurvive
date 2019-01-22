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
using TTT.Resource;
using TTT.Item;

public class PersonTextPanel : MonoBehaviour
{
    public Text vit;
    public Text str;
    public Text agl;
    public Text tec;
    public Text intl;
    public Text hp;
    public Text ap;
    public Text hpRec;
    public Text apRec;
    public Text atk;
    public Text ats;
    public Text crc;
    public Text crd;
    public Text hrate;
    public Text evade;
    public Text hit;
    public Text range;
    public Text spd;
    public UnitInventoryCtrl WeaponGridCtrl;
    private int indexOfpersonUsed = -1;
    // Use this for initialization
    void Start()
    {
        clearPanel();
        WeaponGridCtrl.OnChargeIn += canEuqip;
    }

    public void clearPanel()
    {
        vit.text = "体力：";
        str.text = "力量：";
        agl.text = "敏捷：";
        tec.text = "技巧：";
        intl.text = "智力：";
        hp.text = "hp：";
        ap.text = "ap：";
        hpRec.text = "hp恢复：";
        apRec.text = "ap恢复：";
        atk.text = "攻击力：";
        ats.text = "攻速：";
        crc.text = "暴击率：";
        crd.text = "暴击倍率：：";
        hrate.text = "命中率：";
        evade.text = "闪避率：";
        hit.text = "受伤比例：";
        range.text = "射程：";
        spd.text = "移速：";
        //WeaponGridCtrl.gameObject.SetActive(false);
        WeaponGridCtrl.Clear();
    }
    public void updatePanel(int personIndex, bool isSelectPeople = true)
    {
        if (isSelectPeople)
        {
            int personID = GameObject.Find("gcTextPanel").GetComponent<PersonTextPanel>().getIndexOfpersonUsed();
            //Person curPerson = World.getInstance().persons[personID];
            GameObject.Find("gcTextPanel").GetComponent<PersonTextPanel>().updatePanel(personID, false);
            //WeaponGridCtrl.gameObject.SetActive(false);
            WeaponGridCtrl.Clear();
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
        atk.text = "攻击力：" + p.getValAtk();
        ats.text = "攻速：" + p.getValAts();
        crc.text = "暴击率：" + p.getValCrc() * 100 + "%";
        crd.text = "暴击倍率：：" + p.getValCrd();
        hrate.text = "命中率：" + p.getValHrate() * 100 + "%";
        evade.text = "闪避率：" + p.getValErate() * 100 + "%";
        hit.text = "受伤比例：" + p.getValHit() * 100 + "%";
        range.text = "射程：" + p.getRange();
        spd.text = "移速：" + p.getValSpd();
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

    bool canEuqip(int itemID, int number)
    {
        if (StaticResource.GetItemInfoByID<WeaponInfo>(itemID) == null)
            return false;
        //以后补充职业相关
        return true;
    }
}
