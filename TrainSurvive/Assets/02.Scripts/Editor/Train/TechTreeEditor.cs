/*
 * 描述：科技树辅助类
 * 作者：刘旭涛
 * 创建时间：2018/11/26 0:07:08
 * 版本：v0.1
 */
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(TechTree))]
public class TechTreeEditor : Editor {
    
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUILayout.Button("Build Tree")) {
            BuildTree(target as TechTree);
        }
    }

    private void DestroyChildren(RectTransform content) {
        int children = content.childCount;
        for (int i = 0; i < children; i++) {
            DestroyImmediate(content.GetChild(0).gameObject);
        }
    }

    private List<List<int>> BuildLayer(TechTree techTree) {
        List<List<int>> layers = new List<List<int>>();
        bool[] checks = new bool[TechTreeManager.TechSettings.Length];

        // Build layers, Max 100 layers.
        for (int layer = 0; layer < 100; layer++) {
            layers.Add(new List<int>());  // Add Layer.

            for (int i = 0; i < TechTreeManager.TechSettings.Length; i++) {
                if (TechTreeManager.TechSettings[i] == null) {
                    checks[i] = true;
                }
                if (checks[i]) {  // i已经被check过了
                    continue;
                }
                bool flag = true;
                if (layer >= 1) {  // If Layer 1 - n
                    for (int j = 0; j < TechTreeManager.TechSettings[i].Dependencies.Length; j++) {
                        // Not Exists in last Layer.
                        bool bFlag = false;
                        for (int layerI = layers.Count - 2; layerI >= 0; layerI--) {
                            if (layers[layerI].BinarySearch(TechTreeManager.TechSettings[i].Dependencies[j]) >= 0) {
                                bFlag = true;
                                break;
                            }
                        }
                        if (!bFlag) {
                            flag = false;
                            break;
                        }
                    }
                } else {
                    // If Layer 0
                    flag = TechTreeManager.TechSettings[i].Dependencies.Length == 0; // 当i无依赖时，应当直接加入第一层。
                }
                if (flag) {
                    layers[layers.Count - 1].Add(i);
                    checks[i] = true;
                }
            }
            // For BinarySearch
            layers[layers.Count - 1].Sort();

            // Check if all techs have been processed.
            bool checkFlag = true;
            for (int check = 0; check < checks.Length; check++) {
                if (!checks[check]) {
                    checkFlag = false;
                    break;
                }
            }
            if (checkFlag) {
                break;
            }
        }
        return layers;
    }

    private void BuildTree(TechTree techTree) {
        RectTransform tree = techTree.transform.Find("TreePanel/Viewport/Content/Tree").transform as RectTransform;
        RectTransform lines = techTree.transform.Find("TreePanel/Viewport/Content/Lines").transform as RectTransform;
        DestroyChildren(tree);
        DestroyChildren(lines);

        List<List<int>> layers = BuildLayer(techTree);

        CreateContent(techTree, tree, lines, layers);
    }

    private void CreateContent(TechTree techTree, RectTransform tree, RectTransform lines, List<List<int>> layers) {
        techTree.TechObjects = new ProgressButton[TechTreeManager.TechSettings.Length];
        techTree.TechLines = new TechTree.Line[TechTreeManager.TechSettings.Length];

        for (int i = 0; i < layers.Count; i++) {
            RectTransform verticalGroup = Instantiate(techTree.VerticalGroup, tree).GetComponent<RectTransform>();
            for (int j = 0; j < layers[i].Count; j++) {
                GameObject tech = Instantiate(techTree.TechPrefab, verticalGroup);
                tech.GetComponentInChildren<Text>().text = TechTreeManager.TechSettings[layers[i][j]].Name;

                techTree.TechObjects[layers[i][j]] = tech.GetComponent<ProgressButton>();
                techTree.TechLines[layers[i][j]] = new TechTree.Line {
                    Lines = new Image[TechTreeManager.TechSettings[layers[i][j]].Dependencies.Length]
                };

                if (i == 0) {
                    continue;
                }
                
                RectTransform techRect = tech.GetComponent<RectTransform>();
                HorizontalLayoutGroup horizontalLayoutGroup = tree.GetComponent<HorizontalLayoutGroup>();
                VerticalLayoutGroup verticalLayoutGroup = verticalGroup.GetComponent<VerticalLayoutGroup>();
                float sx = horizontalLayoutGroup.padding.left + i * verticalGroup.rect.width + (verticalGroup.rect.width - techRect.rect.width) / 2 + horizontalLayoutGroup.spacing * i;
                float sy = -(horizontalLayoutGroup.padding.top + verticalLayoutGroup.padding.top + (techRect.rect.height + verticalLayoutGroup.spacing) * j + techRect.rect.height / 2);
                Vector2 start = new Vector2(sx, sy);

                int[] deps = TechTreeManager.TechSettings[layers[i][j]].Dependencies;
                for (int depi = 0; depi < deps.Length; depi++) {
                    int yIndex = 0, xIndex;
                    for (xIndex = i - 1; xIndex >= 0; xIndex--) {
                        yIndex = layers[xIndex].BinarySearch(deps[depi]);
                        if (yIndex >= 0) {
                            break;
                        }
                    }
                    float x = horizontalLayoutGroup.padding.left + xIndex * verticalGroup.rect.width + (verticalGroup.rect.width - techRect.rect.width) / 2 + techRect.rect.width + horizontalLayoutGroup.spacing * xIndex;
                    float y = -(horizontalLayoutGroup.padding.top + verticalLayoutGroup.padding.top + (techRect.rect.height + verticalLayoutGroup.spacing) * yIndex + techRect.rect.height / 2);
                    Vector2 end = new Vector2(x, y);
                    float length = (end - start).magnitude;
                    float degree = Vector2.Angle(Vector2.up, end - start) + 90;
                    RectTransform rectTransform = Instantiate(techTree.LinePrefab, lines).GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = start;
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, length);
                    rectTransform.rotation = Quaternion.Euler(0, 0, degree);

                    techTree.TechLines[layers[i][j]].Lines[depi] = rectTransform.GetComponent<Image>();
                }
            }
        }
        
        // Set Content size.
        RectTransform content = techTree.transform.Find("TreePanel/Viewport/Content").transform as RectTransform;
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tree.rect.width + 10);
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tree.rect.height + 10);
    }
}
