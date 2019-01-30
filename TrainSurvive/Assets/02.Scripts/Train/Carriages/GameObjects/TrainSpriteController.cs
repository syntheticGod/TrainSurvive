/*
 * 描述：用于统一管理Sprite相关，如切换贴图、动画等。
 * 作者：刘旭涛
 * 创建时间：2019/1/30 13:08:59
 * 版本：v0.7
 */
using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class TrainSpriteController : MonoBehaviour {

    public int DefaultLevel = -1;

    private SpriteRenderer _c_SpriteRenderer;
    private Animator _c_Animator;
    private int _level = -1;

    private SpriteRenderer C_SpriteRenderer {
        get {
            if (_c_SpriteRenderer == null) {
                _c_SpriteRenderer = GetComponent<SpriteRenderer>();
            }
            return _c_SpriteRenderer;
        }
    }
    private Animator C_Animator {
        get {
            if (_c_Animator == null) {
                _c_Animator = GetComponent<Animator>();
            }
            return _c_Animator;
        }
    }

    public int Level {
        get {
            return _level;
        }
        set {
            if (value < 0) {
                gameObject.SetActive(false);
            } else {
                gameObject.SetActive(true);
                Sprite sprite = ResourceLoader.GetResource<Sprite>("Sprite/Carriage/" + name + "/" + value + "/origin");
                RuntimeAnimatorController animator = ResourceLoader.GetResource<RuntimeAnimatorController>("Sprite/Carriage/" + name + "/" + value + "/anim");
                C_SpriteRenderer.sprite = sprite;
                C_Animator.runtimeAnimatorController = animator;
            }
            _level = value;
        }
    }
    
}
