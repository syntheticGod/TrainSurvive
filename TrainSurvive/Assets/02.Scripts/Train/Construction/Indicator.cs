/*
 * 描述：设施UI指示器
 * 作者：刘旭涛
 * 创建时间：2018/10/31 14:17:37
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour {

    [SerializeField]
    [Tooltip("进度条")]
    private Slider progressSlider;

    [SerializeField]
    [Tooltip("停止指示Sprite图标")]
    private GameObject stopIndicator;

    /// <summary>
    /// 获取或设置进度值，先调用ShowProgress启用进度条。
    /// </summary>
    public float Progress {
        get {
            return progressSlider.value;
        }
        set {
            progressSlider.value = value;
        }
    }

    /// <summary>
    /// 启用进度条
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="start">初始值</param>
    public void ShowProgress(float min, float max, float start) {
        progressSlider.minValue = min;
        progressSlider.maxValue = max;
        Progress = start;
        progressSlider.gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏进度条
    /// </summary>
    public void HideProgress() {
        progressSlider.gameObject.SetActive(false);
    }

    /// <summary>
    /// 切换停止指示图标显示状态
    /// </summary>
    /// <param name="isShow">true 显示；false 不显示。</param>
    public void ToggleStopIndicator(bool isShow) {
        stopIndicator.SetActive(isShow);
    }
}
