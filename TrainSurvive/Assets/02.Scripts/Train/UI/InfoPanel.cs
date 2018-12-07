/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/6 20:05:30
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {
    
    private Text Title {
        get {
            if (_title == null)
                _title = transform.Find("Title").GetComponent<Text>();
            return _title;
        }
    }

    private Text Content {
        get {
            if (_content == null)
                _content = transform.Find("Content").GetComponent<Text>();
            return _content;
        }
    }

    public string TitleText {
        get {
            return Title.text;
        }
        set {
            Title.text = value;
        }
    }

    public string ContentText {
        get {
            return Content.text;
        }
        set {
            Content.text = value;
        }
    }

    private Text _title;
    private Text _content;
}
