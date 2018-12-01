/*
 * 描述：
 * 作者：Gong Chen
 * 创建时间：2018/11/29 17:15:40
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour {
    public int buttonIndex;//1基
    public Text buttonText;
    public GameObject saveLoadBasePanel;

	// Use this for initialization
	void Start () {
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }
	
    public void setText(string text)
    {
        buttonText.text = text;
    }
	
    public void OnClick()
    {
        
        Button bt = gameObject.GetComponent<Button>();
        bt.Select();
        SavePanel panelCs = (SavePanel)saveLoadBasePanel.GetComponent("SavePanel");
        panelCs.slotSelect(buttonIndex);
    }
}
