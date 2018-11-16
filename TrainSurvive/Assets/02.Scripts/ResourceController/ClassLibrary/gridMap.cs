using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gridMap {
    public enum climateE
    {
        temp=0,//温带
        trop=1,//热带
        tund=2,//冻原
        hot=3, //炎热
        cold=4,//极寒
        NUM
    }
    public enum terrainE
    {
        plain=0, //平原
        hill=1,  //丘陵
        fore=2,  //森林
        mount=3, //山脉
        NUM
    }
    public enum buildE
    {
        none=0,//无
        town=1,//城镇
        rail=2,//铁轨
        scen=3,//剧情副本
        reso=4,//大资源点
        rand=5,//随机副本
        NUM
    }
    int townID=0;
    int scenID=0;
    public enum resourceE
    {
        none=0,//无
        lake=1,//湖泊
        jung=2,//丛林
        volc=3,//火山
        ice=4, //冰川
        NUM
    }

}
