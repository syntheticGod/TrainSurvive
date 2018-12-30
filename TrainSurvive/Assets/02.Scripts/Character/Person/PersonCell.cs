/*
 * 描述：
 * 作者：Gong Chen
 * 创建时间：2018/11/24 15:04:48
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;



public class PersonCell : MonoBehaviour {
    public int index;
    public Text cellText;
    private GameObject textPanel;
    private GameObject professionPanel;
    private GameObject skillContentOb;
    // Use this for initialization
    void Start () {
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
        textPanel = GameObject.Find("gcTextPanel");
        professionPanel= GameObject.Find("gcProfessionPanel");
        skillContentOb = GameObject.Find("gcSkillPanel");
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void setCellText(string text)
    {
        //GameObject cell = transform.Find("cellText").gameObject;
        //Text cellText = (Text)cell.GetComponent("Text");
        cellText.text = text;
    }

    private void OnClick()
    {
        PersonTextPanel panelCs=(PersonTextPanel)textPanel.GetComponent("PersonTextPanel");
        panelCs.updatePanel(index);
        ProfessionPanel professionpanelCs = professionPanel.GetComponent<ProfessionPanel>();
        professionpanelCs.updatePanel(index);
        PersonSkillPanel personSkillPanel = skillContentOb.GetComponent<PersonSkillPanel>();
        personSkillPanel.updatePanel(index);
        Button bt = (Button)gameObject.GetComponent("Button");
        bt.Select();
    }

}
