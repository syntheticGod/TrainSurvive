/*
 * 描述：建筑等级
 * 作者：刘旭涛
 * 创建时间：2019/1/28 18:57:52
 * 版本：v0.7
 */
using System;
using UnityEngine;

public class StructureGameObject : MonoBehaviour {
    
    #region 组件
    private SpriteRenderer C_SpriteRenderer {
        get {
            if (_c_SpriteRenderer == null) {
                _c_SpriteRenderer = GetComponent<SpriteRenderer>();
            }
            return _c_SpriteRenderer;
        }
    }
    #endregion

    #region 公有属性
    /// <summary>
    /// 建筑等级，-1表示不显示，然后从0，1，2...依次增高
    /// </summary>
    public int Level {
        get {
            return _level;
        }
        set {
            _level = value;
            if (value == -1) {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            C_SpriteRenderer.sprite = Sprites[value];
        }
    }
    #endregion

    #region 私有属性
    private Sprite[] Sprites {
        get {
            if (_sprites == null) {
                _sprites = ResourceLoader.GetResources<Sprite>("Sprite/Carriage/" + name);
                Array.Sort(_sprites, (a, b) => int.Parse(a.name).CompareTo(int.Parse(b.name)));
            }
            return _sprites;
        }
    }
    #endregion

    #region 严禁调用的隐藏变量
    private SpriteRenderer _c_SpriteRenderer;
    private int _level = -1;
    private static Sprite[] _sprites;
    #endregion

    #region 生命周期
    #endregion

    #region 公有函数
    #endregion

    #region 私有函数
    #endregion
}
