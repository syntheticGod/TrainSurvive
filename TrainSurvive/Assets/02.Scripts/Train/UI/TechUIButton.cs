/*
 * 描述：控制主界面科技按钮显示
 * 作者：刘旭涛
 * 创建时间：2018/12/2 16:34:09
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechUIButton : MonoBehaviour {
    
	void Start () {
        StartCoroutine(ResearchProgressChange(GetComponent<ProgressButton>()));
    }

    private IEnumerator ResearchProgressChange(ProgressButton progressButton) {
        yield return new WaitWhile(() => TechTree.Instance == null);
        while (true) {
            while (TechTree.Instance.CurrentWorking >= 0 && TechTree.Techs.Length > TechTree.Instance.CurrentWorking && TechTree.Techs[TechTree.Instance.CurrentWorking].TechState == Tech.State.WORKING) {
                progressButton.MaxValue = TechTree.Instance.TechObjects[TechTree.Instance.CurrentWorking].MaxValue;
                progressButton.Value = TechTree.Techs[TechTree.Instance.CurrentWorking].CurrentWorks;
                yield return 1;
            }
            progressButton.Value = 0;
            yield return new WaitWhile(() => TechTree.Instance.CurrentWorking < 0 || TechTree.Techs.Length <= TechTree.Instance.CurrentWorking);
        }
    }
}
