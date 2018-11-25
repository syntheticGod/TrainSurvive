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
    public GameObject textPanel;
    // Use this for initialization
    void Start () {
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
        textPanel = GameObject.Find("gcTextPanel");
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
    }

}
