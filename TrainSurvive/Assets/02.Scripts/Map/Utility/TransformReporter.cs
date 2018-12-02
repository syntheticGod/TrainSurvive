/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/1 23:16:18
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;

public class TransformReporter : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Debug.Log("Transform Report Start:" + GetComponent<RectTransform>().localPosition); 
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Transform Report Update:" + GetComponent<RectTransform>().localPosition);
    }
}
