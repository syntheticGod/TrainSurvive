/*
 * 描述：车厢升级配置
 * 作者：刘旭涛
 * 创建时间：2019/1/28 19:48:41
 * 版本：v0.7
 */
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "策划/车厢升级配置")]
public class CarriageResearchSetting : ScriptableObject {

    [Serializable]
    private struct Cost {
        public int ID;
        public int Count;
    }

    [Serializable]
    public struct UpgradeSprite {
        public string Name;
        public int Level;
    }

    [Tooltip("ID")]
    public int ID;
    [Tooltip("依赖ID")]
    public int Dependency;
    [Tooltip("名称")]
    public string Name;
    [Tooltip("描述")]
    public string Description;
    [Tooltip("升级时需要更新的贴图")]
    public UpgradeSprite[] UpgradeSprites;
    [Tooltip("耗材")]
    [SerializeField]
    private Cost[] CostItems;

    public ItemData[] Costs {
        get {
            if (_costs == null) {
                _costs = new ItemData[CostItems.Length];
                for (int i = 0; i < CostItems.Length; i++) {
                    _costs[i] = new ItemData(CostItems[i].ID, CostItems[i].Count);
                }
            }
            return _costs;
        }
    }

    private ItemData[] _costs;
}
