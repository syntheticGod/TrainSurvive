/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/2/3 13:20:54
 * 版本：v0.7
 */
using Assets._02.Scripts.zhxUIScripts;
using System;
using TTT.Item;
using TTT.Resource;
using TTT.UI;
using UnityEngine;
using UnityEngine.UI;

public class SelectionItemUI : MonoBehaviour {
    private Text _c_Name;
    private Text _c_Description;
    private ResourceItemBase _c_Icon;
    private Button _c_Button;

    private Text C_Name {
        get {
            if (_c_Name == null) {
                _c_Name = transform.Find("Name").GetComponent<Text>();
            }
            return _c_Name;
        }
    }
    private Text C_Description {
        get {
            if (_c_Description == null) {
                _c_Description = transform.Find("Description").GetComponent<Text>();
            }
            return _c_Description;
        }
    }
    private ResourceItemBase C_Icon {
        get {
            if (_c_Icon == null) {
                _c_Icon = transform.Find("Icon").GetComponent<ResourceItemBase>();
            }
            return _c_Icon;
        }
    }
    private Button C_Button {
        get {
            if (_c_Button == null) {
                _c_Button = GetComponent<Button>();
            }
            return _c_Button;
        }
    }
    
    public void SetData(int id, Action<int> action) {
        ItemInfo info = StaticResource.GetItemInfoByID<ItemInfo>(id);
        C_Name.text = info.Name;
        C_Description.text = info.Description;
        C_Icon.SetItemID(id);
        C_Button.onClick.RemoveAllListeners();
        C_Button.onClick.AddListener(() => action?.Invoke(id));
    }
}
