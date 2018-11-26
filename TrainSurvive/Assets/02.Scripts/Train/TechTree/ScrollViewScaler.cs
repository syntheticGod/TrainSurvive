/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/11/25 20:22:50
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollViewScaler : MonoBehaviour, IScrollHandler {

    //[Tooltip("滚轮缩放系数")]
    //public float ScaleDelta = 0.2f;

    //public RectTransform Content;

    ////private RectTransform _content;
    ////private RectTransform Content {
    ////    get {
    ////        if (_content == null) {
    ////            _content = GetComponent<ScrollRect>().content;
    ////        }
    ////        return _content;
    ////    }
    ////}

    //public void OnScroll(PointerEventData eventData) {
    //    Vector2 focus = ScreenToContent(eventData.position);
    //    Content.pivot = focus;
    //    float scale = Content.localScale.x + eventData.scrollDelta.y * ScaleDelta;
    //    Content.localScale = new Vector3(scale, scale);
    //}

    //private Vector2 ScreenToContent(Vector2 focus) {
    //    RectTransform rectTransform = transform as RectTransform;
    //    float x = focus.x - (Camera.main.scaledPixelWidth - rectTransform.rect.width) / 2;
    //    float y = focus.y - (Camera.main.scaledPixelHeight - rectTransform.rect.height) / 2;
    //    return Camera.main.ScreenToViewportPoint(new Vector2(x, y));
    //}

    public float minFactor = 1;
    public float maxFactor = 3;
    public bool wholeSizeFactor = true;
    
    private float _sizeFactor = 1f;
    
    public float SizeFactor {
        get {
            if (wholeSizeFactor)
                return Mathf.Round(_sizeFactor);
            return _sizeFactor;
        }
        set {
            SetFactor(value);
        }
    }

    [SerializeField]
    RectTransform content;
    [SerializeField]
    RectTransform viewport;

    Rect _rect;
    Vector2 _focusPos;

    void Start() {
        _rect = GetWorldRect(viewport);
        _focusPos = _rect.center;
    }

    public static Rect GetWorldRect(RectTransform rt) {
        Vector3[] cors_ = new Vector3[4];
        rt.GetWorldCorners(cors_);
        Vector2 center = cors_[0];
        float width = Mathf.Abs(cors_[0].x - cors_[2].x);
        float height = Mathf.Abs(cors_[0].y - cors_[2].y);
        Rect rect_ = new Rect(center.x, center.y, width, height);
        return rect_;
    }

    private void SetFactor(float value) {
        value = ClampFactorValue(value);

        if (value != _sizeFactor) {
            _sizeFactor = value;
            ChangeSizeFactor(_sizeFactor);
        }
    }

    private void ChangeSizeFactor(float v) {
        Vector2 viewportSize_ = _rect.size;

        //缩放过程中的焦点位置（此处为中心位置）
        //_focusPos = _rect.center;

        Rect contentRect_ = GetWorldRect(content);
        Vector2 contentSize_ = contentRect_.size;
        Vector2 contentCenter_ = contentRect_.center;

        Vector2 contentCenter2ViewportCenter_ = _focusPos - contentCenter_;
        float centerPosPercentX_ = contentCenter2ViewportCenter_.x / contentSize_.x;
        float centerPosPercentY_ = contentCenter2ViewportCenter_.y / contentSize_.y;

        Vector2 scorllSize_ = viewportSize_ + (v - 1) * viewportSize_ / 5;
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scorllSize_.x);
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scorllSize_.y);

        Vector2 sizeDelta_ = scorllSize_ - contentSize_;
        Vector2 posOffset_ = new Vector2(sizeDelta_.x * -centerPosPercentX_, sizeDelta_.y * -centerPosPercentY_);
        content.anchoredPosition += posOffset_;

        Vector3[] viewCorner_ = new Vector3[4];
        Vector3[] contentCorner_ = new Vector3[4];
        viewport.GetWorldCorners(viewCorner_);
        content.GetWorldCorners(contentCorner_);

        float xFixDelta_ = 0;
        float yFixDelta_ = 0;

        if (viewCorner_[0].x < contentCorner_[0].x) xFixDelta_ = viewCorner_[0].x - contentCorner_[0].x;
        if (viewCorner_[0].y < contentCorner_[0].y) yFixDelta_ = viewCorner_[0].y - contentCorner_[0].y;
        if (viewCorner_[2].x > contentCorner_[2].x) xFixDelta_ = viewCorner_[2].x - contentCorner_[2].x;
        if (viewCorner_[2].y > contentCorner_[2].y) yFixDelta_ = viewCorner_[2].y - contentCorner_[2].y;

        content.anchoredPosition += new Vector2(xFixDelta_, yFixDelta_);
    }

    private float ClampFactorValue(float value) {
        float factor_ = Mathf.Clamp(value, minFactor, maxFactor);

        if (wholeSizeFactor) factor_ = Mathf.Round(factor_);

        return factor_;
    }

    public void OnScroll(PointerEventData eventData) {
        if (!isActiveAndEnabled) return;
        _focusPos = eventData.position;//焦点为鼠标所在位置
        float delta_ = 0;
        if (Mathf.Abs(eventData.scrollDelta.x) > Mathf.Abs(eventData.scrollDelta.y))
            delta_ = eventData.scrollDelta.x;
        else delta_ = eventData.scrollDelta.y;

        SetFactor(_sizeFactor + delta_);
    }
}
