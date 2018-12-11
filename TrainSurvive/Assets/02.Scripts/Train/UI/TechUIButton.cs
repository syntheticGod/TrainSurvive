/*
 * 描述：控制主界面科技按钮显示
 * 作者：刘旭涛
 * 创建时间：2018/12/2 16:34:09
 * 版本：v0.1
 */
using System.Collections;
using UnityEngine;

public class TechUIButton : MonoBehaviour {
    
	void Start () {
        StartCoroutine(ResearchProgressChange(GetComponent<ProgressButton>()));
    }

    private IEnumerator ResearchProgressChange(ProgressButton progressButton) {
        progressButton.MaxValue = 1;
        while (true) {
            while (TechTreeManager.Instance.CurrentWorking >= 0 && TechTreeManager.Instance.Techs[TechTreeManager.Instance.CurrentWorking].TechState == Tech.State.WORKING) {
                progressButton.MaxValue = TechTreeManager.TechSettings[TechTreeManager.Instance.CurrentWorking].TotalWorks;
                progressButton.Value = TechTreeManager.Instance.Techs[TechTreeManager.Instance.CurrentWorking].CurrentWorks;
                yield return 1;
            }
            progressButton.Value = 0;
            yield return new WaitWhile(() => TechTreeManager.Instance.CurrentWorking < 0);
        }
    }
}
