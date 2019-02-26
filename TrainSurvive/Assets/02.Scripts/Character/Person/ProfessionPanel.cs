/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/14 12:48:55
 * 版本：v0.1
 */
using UnityEngine.UI;
using UnityEngine;
using TTT.Common;

public class ProfessionPanel : MonoBehaviour {
    //以后添加sprite
    public Text professionText1;
    public Text professionText2;
    public Text professionText3;
    private string[] pStr= new string[3];
    public Text mainProfessionText;
    // Use this for initialization
    void Start () {
		
	}
	
	public void updatePanel(int personIndex)
    {
        Person person = World.getInstance().Persons.At(personIndex);
        Profession mainProfession = person.getTopProfession();
        Profession[] ps = new Profession[3];
        for(int i = 0; i < 3; i++)
        {
            ps[i] = person.getProfession(i);
            if (ps[i] != null)
            {
                Profession.AbiReq[] req = ps[i].AbiReqs;
                pStr[i] = "";
                foreach (Profession.AbiReq abiReq in req)
                    pStr[i] += AttriTool.Chinese(abiReq.Abi);
            }
            else
                pStr[i] = "无";           
        }
           

        if (mainProfession != null)
            mainProfessionText.text = mainProfession.Name;
        else
            mainProfessionText.text = "无";

        professionText1.text = pStr[0];
        professionText2.text = pStr[1];
        professionText3.text = pStr[2];
    }
}
