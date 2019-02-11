/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/10 17:52:01
 * 版本：v0.1
 */
namespace WorldMap.UI
{
    public enum BUTTON_ID
    {
        NONE,

        TEAM_NONE,//小队模式显示的按键
        TEAM_ENTRY_AREA,//小队进入区域
        TEAM_RETRUN,//小队回车
        TEAM_GATHER,//小队采集
        TEAM_PACK,//小队背包
        TEAM_CHARACTER,//小队人物
        TEAM_NUM,

        TRAIN_NONE,//列车模式显示的按钮
        TRAIN_ENTRY_AREA,//进入区域
        TRAIN_TEAM_ACTION,//小队下车行动
        TRAIN_RUN_OR_STOP,//开/停车
        TRAIN_CHANGE,//列车内部
        TRAIN_NUM,

        TOWN_NONE,//城镇界面显示的按钮
        TOWN_TAVERN,//酒馆
        TOWN_SCHOOL,//学校
        TOWN_SHOP,//商店
        TOWN_SPECIAL_BUILDING,//特殊建筑 ——神殿
        TOWN_NUM,

        TAVERN_NONE,//酒馆界面
        TAVERN_BUTTON1,
        TAVERN_BUTTON2,
        TAVERN_BUTTON3,
        TAVERN_NUM,

        TEAM_SELECT_DIALOG_NONE,//探险队选择框
        TEAM_SELECT_FOOD_PLUS,//增加食物
        TEAM_SELECT_FOOD_SUBTRCT,//减少食物
        TEAM_SELECT_DIALOG_NUM,
        NUM,
    }
}