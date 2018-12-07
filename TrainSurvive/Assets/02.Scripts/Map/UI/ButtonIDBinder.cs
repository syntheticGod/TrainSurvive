/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/10 17:52:01
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace WorldMap
{
    public class ButtonIDBinder : MonoBehaviour
    {

        public static string[] BUTTON_NAMES =
                {
            "NONE",

            "TEAM_NONE",
            "TeamEntryAreaBtn",
            "TeamReturnBtn",
            "TeamGatherBtn",
            "TeamPackBtn",
            "TEAM_NUM",

            "TRAIN_NONE",
            "TrainRunOrStopBtn",
            "TrainEntryAreaBtn",
            "TrainTeamActionBtn",
            "TrainChangeBtn",
            "TRAIN_NUM",

            "TOWN_NONE",
            "TownTavernBtn",
            "TownSchoolBtn",
            "TownShopBtn",
            "TOWN_NUM",

            "TAVERN_NONE",
            "TavernRecruitBtn",
            "TavernCancelBtn",
            "TAVERN_NUM",

            "TEAM_SELECT_DIALOG_NONE",
            "TeamSelectFoodPlusBtn",
            "TeamSelectFoodSubtractBtn",
            "TeamSelectFoodOkBtn",
            "TeamSelectFoodCancelBtn",
            "TEAM_SELECT_DIALOG_NUM",

            "NUM" };
        public static string GetButtonName(BUTTON_ID id)
        {
            return BUTTON_NAMES[(int)id];
        }
        public BUTTON_ID ButtonID { get; private set; }
        void Start()
        {
            BindID();
            Button button = GetComponent<Button>();
            Debug.Assert(null != button, "改脚本必须绑定在Button上");
            button.onClick.AddListener(delegate ()
            {
                ButtonHandler.Instance.OnClick(ButtonID);
            });
        }
        private void BindID()
        {
            //绑定按钮的ID
            for (int i = (int)BUTTON_ID.NONE; i < (int)BUTTON_ID.NUM; i++)
            {
                if (BUTTON_NAMES[i].Equals(name))
                    ButtonID = (BUTTON_ID)i;
            }
            Debug.Assert(BUTTON_ID.NONE != ButtonID, "未找到"+gameObject.name+"的ButtonID");
        }
    }
}