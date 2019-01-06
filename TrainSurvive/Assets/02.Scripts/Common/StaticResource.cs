/*
 * 描述：静态资源，这些资源是常存于内存中的静态不能被改变的数据。
 * 作者：项叶盛
 * 创建时间：2018/11/12 21:46:39
 * 版本：v0.1
 */
using UnityEngine;
using System.Xml;

using System.Collections.Generic;
using System;
using TTT.Item;
using TTT.Utility;
using System.Collections;

namespace TTT.Resource
{
    public static class StaticResource
    {
        //----------专精----------↓
        /// <summary>
        /// 根据EProfession的序号排序
        /// </summary>
        private static Profession[] professions;
        private static Profession[] firstProfessions;
        private static Profession[,] secondProfessions;
        private static Profession[,,] thirdProfessions;
        /// <summary>
        /// 根据专精的ID返回专精
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Profession GetProfessionByID(int id)
        {
            if (professions == null)
            {
                LoadProfessionFromXml();
            }
            return professions[id];
        }
        public static Profession GetProfession(EAttribute atr)
        {
            return firstProfessions[(int)atr];
        }
        public static Profession GetProfession(EAttribute atr1, EAttribute atr2)
        {
            return secondProfessions[(int)atr1, (int)atr2];
        }
        public static Profession GetProfession(EAttribute atr1, EAttribute atr2, EAttribute atr3)
        {
            return thirdProfessions[(int)atr1, (int)atr2, (int)atr3];
        }
        /// <summary>
        /// 根据传入的专精获取下一级别的专精
        /// NULL => 一级
        /// 一级   => 二级
        /// 二级   => 三级
        /// 三级   => NULL
        /// </summary>
        /// <param name="profession"></param>
        /// <returns></returns>
        public static Profession[] GetNextProfessions(Profession profession)
        {
            const int NumOfAttribute = (int)EAttribute.NUM;
            if (firstProfessions == null) LoadProfessionFromXml();
            Profession[] result = new Profession[NumOfAttribute];
            if (profession == null)
            {
                //返回一级专精
                for (int i = 0; i < result.Length; i++)
                    result[i] = firstProfessions[i];
            }
            else if (profession.Level == EProfessionLevel.LEVEL1)
            {
                //返回二级专精
                for (int i = 0; i < result.Length; i++)
                    result[i] = secondProfessions[(int)profession.AbiReqs[0].Abi, i];
            }
            else if (profession.Level == EProfessionLevel.LEVEL2)
            {
                //返回三级专精
                for (int i = 0; i < result.Length; i++)
                {
                    int x = (int)profession.AbiReqs[0].Abi;
                    int y = x;
                    if (profession.AbiReqs.Length == 2)
                        y = (int)profession.AbiReqs[1].Abi;
                    result[i] = thirdProfessions[x, y, i];
                }

            }
            else if (profession.Level == EProfessionLevel.LEVEL3)
            {
                //顶级专精
                return null;
            }
            return result;
        }
        private static void SetProfession(EAttribute atr, Profession profession)
        {
            firstProfessions[(int)atr] = profession;
        }
        private static void SetProfession(EAttribute atr1, EAttribute atr2, Profession profession)
        {
            secondProfessions[(int)atr1, (int)atr2] = profession;
            secondProfessions[(int)atr2, (int)atr1] = profession;
        }
        private static void SetProfession(EAttribute atr1, EAttribute atr2, EAttribute atr3, Profession profession)
        {
            thirdProfessions[(int)atr1, (int)atr2, (int)atr3] = profession;
            thirdProfessions[(int)atr1, (int)atr3, (int)atr2] = profession;
            thirdProfessions[(int)atr2, (int)atr1, (int)atr3] = profession;
            thirdProfessions[(int)atr2, (int)atr3, (int)atr1] = profession;
            thirdProfessions[(int)atr3, (int)atr1, (int)atr2] = profession;
            thirdProfessions[(int)atr3, (int)atr2, (int)atr1] = profession;
        }
        /// <summary>
        /// 从profession.xml中读取数据
        /// </summary>
        private static void LoadProfessionFromXml()
        {
            const int NumOfAttribute = (int)EAttribute.NUM;
            firstProfessions = new Profession[NumOfAttribute];
            secondProfessions = new Profession[NumOfAttribute, NumOfAttribute];
            thirdProfessions = new Profession[NumOfAttribute, NumOfAttribute, NumOfAttribute];

            string xmlString = Resources.Load("xml/profession").ToString();
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlString);

            XmlNode root = document.SelectSingleNode("professions");
            string spriteFloder = root.SelectSingleNode("spriteFloder").InnerText + "/";

            XmlNodeList professionList = root.SelectNodes("profession");
            professions = new Profession[professionList.Count];
            foreach (XmlNode professionNode in professionList)
            {
                //属性状态
                EProfessionState eState = EProfessionState.NONE + 1 + int.Parse(professionNode.Attributes["state"].Value);
                //属性类型
                int id = int.Parse(professionNode.Attributes["id"].Value);
                //前置属性
                XmlNodeList abiReqList = professionNode.SelectNodes("abiReq");
                Profession.AbiReq[] abiReq = null;
                if (abiReqList.Count != 0)
                {
                    abiReq = new Profession.AbiReq[abiReqList.Count];
                    int indexOfAbi = 0;
                    foreach (XmlNode abiReqNode in abiReqList)
                    {
                        Profession.AbiReq tempAbi = new Profession.AbiReq();
                        tempAbi.Abi = EAttribute.NONE + 1 + int.Parse(abiReqNode.Attributes["abi"].Value);
                        //tempAbi.Number = int.Parse(abiReqNode.Attributes["Number"].Value);
                        tempAbi.costFix = float.Parse(abiReqNode.Attributes["costFix"].Value);
                        abiReq[indexOfAbi++] = tempAbi;
                    }
                }
                //名字
                string name = professionNode.Attributes["name"].Value;
                //信息
                string info = professionNode.Attributes["info"].Value;
                Profession profession = new Profession(id, abiReq, eState, name, info);
                //类型映射
                professions[id] = profession;
                //根据前置属性，填充映射关系
                EAttribute atr1 = abiReq[0].Abi;
                switch (profession.Level)
                {
                    case EProfessionLevel.LEVEL1:
                        {
                            SetProfession(atr1, profession);
                        }
                        break;
                    case EProfessionLevel.LEVEL2:
                        {
                            EAttribute atr2 = atr1;
                            if (profession.AbiReqs.Length != 1)
                                atr2 = profession.AbiReqs[1].Abi;
                            SetProfession(atr1, atr2, profession);
                        }
                        break;
                    case EProfessionLevel.LEVEL3:
                        {
                            EAttribute atr2 = atr1;
                            EAttribute atr3 = atr1;
                            if (profession.AbiReqs.Length != 1)
                            {
                                atr2 = profession.AbiReqs[1].Abi;
                                atr3 = profession.AbiReqs[2].Abi;
                            }
                            SetProfession(atr1, atr2, atr3, profession);
                        }
                        break;
                }
            }
        }
        //----------专精----------↑
        /// <summary>
        /// 方块大小
        /// </summary>
        public static Vector2 BlockSize
        {
            set;
            get;
        }
        //地图第一块的中心坐标
        public static Vector2 MapOrigin
        {
            set;
            get;
        }
        /// <summary>
        ///方便块索引计算的 常量
        ///(position - (o - b / 2)) / blockSize;
        /// </summary>
        public static Vector2 MapOriginUnit
        {
            set;
            get;
        }
        /// <summary>
        /// 根据世界坐标计算地图坐标
        /// </summary>
        /// <param name="position">世界坐标</param>
        /// <returns>地图坐标</returns>
        public static Vector2Int BlockIndex(Vector2 position)
        {
            //因为块中心位于原点坐标处，所以要减去blockSize/2
            //(position - (o - b / 2)) / blockSize;
            //公式优化如下
            //position / blockSize - ((mapOrigin - blockSize/2)/blockSize)
            Vector2 index2F = position / BlockSize - MapOriginUnit;
            return new Vector2Int(Mathf.FloorToInt(index2F.x), Mathf.FloorToInt(index2F.y));
        }
        /// <summary>
        /// 计算指定块中心的世界坐标
        /// </summary>
        /// <param name="index2d">地图坐标</param>
        /// <returns>世界坐标</returns>
        public static Vector2 BlockCenter(Vector2Int index2d)
        {
            //索引 * 块大小 = 原点到块中心的向量
            //再加上 原块的中心坐标 就是世界坐标
            return index2d * BlockSize + MapOrigin;
        }
        /// <summary>
        /// 获取当前坐标所在块的中心坐标
        /// </summary>
        /// <param name="position">世界坐标</param>
        /// <returns></returns>
        public static Vector2 FormatBlock(Vector2 position)
        {
            return BlockCenter(BlockIndex(position));
        }
        /// <summary>
        /// 3D世界坐标转化为地图平面的2D世界坐标
        /// </summary>
        /// <param name="worldPosition">3D世界坐标</param>
        /// <returns></returns>
        public static Vector2 WorldPosToMapPos(Vector3 worldPosition)
        {
            return MathTool.IgnoreZ(worldPosition);
        }
        /// <summary>
        /// 地图平面的2D世界坐标转化为3D世界坐标
        /// </summary>
        /// <param name="mapPosition">地图平面的2D世界坐标</param>
        /// <returns></returns>
        public static Vector3 MapPosToWorldPos(Vector2 mapPosition, int level)
        {
            return MathTool.AcceptZ(mapPosition, level);
        }
        //----------属性----------↓
        private static string[] AttributeName { get; } = { "体力", "力量", "敏捷", "技巧", "智力" };
        public static int AttributeCount { get { return AttributeName.Length; } }
        public static string GetAttributeName(int index)
        {
#if DEBUG
            if (index >= AttributeName.Length)
            {
                Debug.LogError("获取属性名失败，index=" + index);
                return "";
            }
#endif
            if (index < 0)
                return "无";
            return AttributeName[index];
        }
        public static string GetAttributeName(EAttribute attribute)
        {
            if (attribute == EAttribute.NONE)
                return "无";
            return AttributeName[(int)attribute];
        }
        //----------属性----------↑----------名字----------↓
        private static string[] TOWN_NAME = {
            "香格里拉", "枫丹白露", "翡冷翠", "米兰", "墨尔本",
            "爱丁堡", "普罗旺斯", "哥本哈根", "耶路撒冷", "柏林", "布达佩斯",
            "都灵", "伯尔尼", "阿姆斯特布", "布里斯班", "达累斯萨拉姆",
            "斯德哥尔摩", "赫尔辛基", "布依诺斯艾利斯", "凤凰城" };
        private static string[] NPC_NAME_MAN =
        {
            "亚伦", "亚伯", "亚伯拉罕", "亚当", "艾德里安","阿尔瓦", "亚历克斯", "艾伯特" ,"阿尔弗雷德","安德鲁","安迪","安格斯","安东尼","亚瑟","奥斯汀","本森","布兰特","布伦特","卡尔","凯里","卡斯帕","查尔斯","采尼","克里斯蒂安","克里斯多夫","科林","科兹莫","丹尼尔","丹尼斯","德里克","唐纳德","道格拉斯","埃德加","艾德文","艾略特","埃尔维斯","埃里克","埃文","弗朗西斯","弗兰克","加百利","加比","加菲尔德","加里","加文","乔治","基诺","格林","格林顿","哈里森","汉克","霍华德","亨利","伊凡","艾萨克","杰克","杰克逊","雅各布","詹森","杰弗瑞","杰罗姆","杰西","吉姆","吉米","乔","约翰","约翰尼","约瑟夫","约书亚","贾斯汀","凯斯","肯","肯尼斯","肯尼","凯文","兰斯","拉里","劳伦特","劳伦斯","利安德尔","李","雷","雷纳德","利奥波特","劳瑞","劳瑞恩","卢克","马库斯","马西","马克","马尔斯","马丁","马修","迈克尔","麦克","尼尔","保罗","帕特里克","菲利普","菲比","昆廷","兰德尔","伦道夫","兰迪","列得","雷克斯","理查德","里奇","罗伯特","罗宾逊","洛克","罗杰","罗伊","赖安"
        };
        private static string[] NPC_NAME_WOMAN =
        {
            "阿比盖尔","艾比","艾达","阿德莱德","艾德","亚伦","亚伯","亚伯拉罕","亚当","艾德里安","阿尔瓦","亚历克斯","亚历山大","艾伦","艾伯特","阿尔弗雷德","安德鲁","安迪","安格斯","安东尼","亚瑟","奥斯汀","本","本森","比尔","鲍伯","布兰登","布兰特","布伦特","布莱恩","布鲁斯","卡尔","凯里","卡斯帕","查尔斯","采尼","克里斯","克里斯蒂安","克里斯多夫","科林","科兹莫","丹尼尔","丹尼斯","德里克","唐纳德","道格拉斯","大卫","丹尼","埃德加","爱德华","艾德文","艾略特","埃尔维斯","埃里克","埃文","弗朗西斯","弗兰克","富兰克林","弗瑞德","加百利","加比","加菲尔德","加里","加文","乔治","基诺","格林","格林顿","哈里森","雨果","汉克","霍华德","亨利","伊格纳缇伍兹","伊凡","艾萨克","杰克","杰克逊","雅各布","詹姆士","詹森","杰弗瑞","杰罗姆","杰瑞","杰西","吉姆","吉米","乔","约翰","约翰尼","约瑟夫","约书亚","贾斯汀","凯斯","肯","肯尼斯","肯尼","凯文","兰斯","拉里","劳伦特","劳伦斯","利安德尔","李","雷欧","雷纳德","利奥波特","劳伦","劳瑞","劳瑞恩","卢克","马库斯","马西","马克","马科斯","马尔斯","马丁","马修","迈克尔","麦克","尼尔","尼古拉斯","奥利弗","奥斯卡","保罗","帕特里克","彼得","菲利普","菲比","昆廷","兰德尔","伦道夫","兰迪","列得","雷克斯","理查德","里奇","罗伯特","罗宾","罗宾逊","洛克","罗杰","罗伊","赖安","阿比盖尔","艾比","艾达","阿德莱德","艾德"
        };
        public static string RandomTownName()
        {
            return TOWN_NAME[MathTool.RandomInt(TOWN_NAME.Length)];
        }
        public static string RandomNPCName(bool man)
        {
            if (man)
                return NPC_NAME_MAN[MathTool.RandomInt(NPC_NAME_MAN.Length)];
            else
                return NPC_NAME_WOMAN[MathTool.RandomInt(NPC_NAME_WOMAN.Length)];
        }
        //----------名字----------↑----------Sprite----------↓
        private static Sprite[] spriteStore = new Sprite[(int)ESprite.NUM];
        private static string[] spriteFileName = {
            "Commen/developing_icon_01_big","Commen/developing_icon_01_small",
            "Sprite/map/person/person1_bottom_0", "Sprite/map/person/person1_left_0", "Sprite/map/person/person1_top_0",
            "Sprite/map/person/person2_bottom", "Sprite/map/person/person2_left", "Sprite/map/person/person2_top",
            "Sprite/map/person/person3_bottom", "Sprite/map/person/person3_left", "Sprite/map/person/person3_top",
            "Sprite/map/person/person4_bottom", "Sprite/map/person/person4_left", "Sprite/map/person/person4_top",
            "Sprite/map/person/person5_bottom", "Sprite/map/person/person5_left", "Sprite/map/person/person5_top",

            "Sprite/map/Train",

            "ProfessionIcon/profession0_icon_big",
            "ProfessionIcon/profession1_icon_big",
            "ProfessionIcon/profession2_icon_big",
            "ProfessionIcon/profession3_icon_big",
            "ProfessionIcon/profession4_icon_big",
            //第二专精
            "ProfessionIcon/profession00_icon_big",
            "ProfessionIcon/profession01_icon_big",
            "ProfessionIcon/profession02_icon_big",
            "ProfessionIcon/profession03_icon_big",
            "ProfessionIcon/profession04_icon_big",

            "ProfessionIcon/profession11_icon_big",
            "ProfessionIcon/profession12_icon_big",
            "ProfessionIcon/profession13_icon_big",
            "ProfessionIcon/profession14_icon_big",

            "ProfessionIcon/profession22_icon_big",
            "ProfessionIcon/profession23_icon_big",
            "ProfessionIcon/profession24_icon_big",

            "ProfessionIcon/profession33_icon_big",
            "ProfessionIcon/profession34_icon_big",

            "ProfessionIcon/profession44_icon_big",
            //第三专精
            "ProfessionIcon/profession000_icon_big",
            "ProfessionIcon/profession012_icon_big",
            "ProfessionIcon/profession013_icon_big",
            "ProfessionIcon/profession014_icon_big",
            "ProfessionIcon/profession023_icon_big",
            "ProfessionIcon/profession024_icon_big",
            "ProfessionIcon/profession034_icon_big",

            "ProfessionIcon/profession111_icon_big",
            "ProfessionIcon/profession123_icon_big",
            "ProfessionIcon/profession124_icon_big",
            "ProfessionIcon/profession134_icon_big",

            "ProfessionIcon/profession222_icon_big",
            "ProfessionIcon/profession234_icon_big",

            "ProfessionIcon/profession333_icon_big",

            "ProfessionIcon/profession444_icon_big",

            "ProfessionIcon/profession0_icon_small",
            "ProfessionIcon/profession1_icon_small",
            "ProfessionIcon/profession2_icon_small",
            "ProfessionIcon/profession3_icon_small",
            "ProfessionIcon/profession4_icon_small",

            "ProfessionIcon/profession00_icon_small",
            "ProfessionIcon/profession01_icon_small",
            "ProfessionIcon/profession02_icon_small",
            "ProfessionIcon/profession03_icon_small",
            "ProfessionIcon/profession04_icon_small",

            "ProfessionIcon/profession11_icon_small",
            "ProfessionIcon/profession12_icon_small",
            "ProfessionIcon/profession13_icon_small",
            "ProfessionIcon/profession14_icon_small",

            "ProfessionIcon/profession22_icon_small",
            "ProfessionIcon/profession23_icon_small",
            "ProfessionIcon/profession24_icon_small",

            "ProfessionIcon/profession33_icon_small",
            "ProfessionIcon/profession34_icon_small",

            "ProfessionIcon/profession44_icon_small",

            "ProfessionIcon/profession000_icon_small",
            "ProfessionIcon/profession012_icon_small",
            "ProfessionIcon/profession013_icon_small",
            "ProfessionIcon/profession014_icon_small",
            "ProfessionIcon/profession023_icon_small",
            "ProfessionIcon/profession024_icon_small",
            "ProfessionIcon/profession034_icon_small",

            "ProfessionIcon/profession111_icon_small",
            "ProfessionIcon/profession123_icon_small",
            "ProfessionIcon/profession124_icon_small",
            "ProfessionIcon/profession134_icon_small",

            "ProfessionIcon/profession222_icon_small",
            "ProfessionIcon/profession234_icon_small",

             "ProfessionIcon/profession333_icon_small",

             "ProfessionIcon/profession444_icon_small",

             "ItemSprite/Money",
             "ItemSprite/Money"//战略点图标，暂时使用金钱
        };
        public static Sprite GetSprite(ESprite eSprite)
        {
            if (spriteStore[(int)eSprite] == null)
            {
                if (MathTool.IfBetweenBoth((int)ESprite.PERSON1_B, (int)ESprite.PERSON1_T, (int)eSprite))
                {
                    Sprite[] sprites = Resources.LoadAll<Sprite>("Sprite/map/person/person1");
                    spriteStore[(int)ESprite.PERSON1_B] = sprites[3];
                    spriteStore[(int)ESprite.PERSON1_L] = sprites[6];
                    spriteStore[(int)ESprite.PERSON1_T] = sprites[9];
                }
                else if (MathTool.IfBetweenBoth((int)ESprite.PERSON2_B, (int)ESprite.PERSON5_T, (int)eSprite))
                {
                    Sprite[] sprites = Resources.LoadAll<Sprite>("Sprite/map/person/persons");
                    int index = 0;
                    for (ESprite i = ESprite.PERSON2_B; i < ESprite.PERSON5_T; i++)
                    {
                        spriteStore[(int)i] = sprites[index++];
                    }
                }
                else
                {
                    spriteStore[(int)eSprite] = Resources.Load<Sprite>(spriteFileName[(int)eSprite]);
                }
                if (spriteStore[(int)eSprite] == null)
                {
                    Debug.LogWarning("Sprite资源不存在" + eSprite.ToString());
                    if (spriteStore[(int)ESprite.DEVELOPING_BIG] == null)
                        spriteStore[(int)ESprite.DEVELOPING_BIG] = Resources.Load<Sprite>(spriteFileName[(int)ESprite.DEVELOPING_BIG]);
                    if (spriteStore[(int)ESprite.DEVELOPING_SMALL] == null)
                        spriteStore[(int)ESprite.DEVELOPING_SMALL] = Resources.Load<Sprite>(spriteFileName[(int)ESprite.DEVELOPING_SMALL]);
                    return spriteStore[(int)ESprite.DEVELOPING_BIG];
                }
            }
            return spriteStore[(int)eSprite];
        }
        //----------Sprite----------↑ ----------技能----------↓
        /// <summary>
        /// 所有技能包括无条件获得的
        /// </summary>
        private static SkillInfo[] skills;
        /// <summary>
        /// 无条件就获得的技能
        /// </summary>
        private static SkillInfo[] Skills { get { if (skills == null) LoadSkillFormXml(); return skills; } }
        private static void LoadSkillFormXml()
        {
            string xmlString = Resources.Load("xml/SkillInfo").ToString();
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlString);
            XmlNode root = document.SelectSingleNode("SkillInfo");
            XmlNodeList skillNodes = root.SelectNodes("Skill");
            skills = new SkillInfo[skillNodes.Count];
            foreach(XmlNode skillNode in skillNodes)
            {
                try
                {
                    SkillInfo skillInfo = new SkillInfo(skillNode);
                    skills[skillInfo.ID] = skillInfo;
                }
                catch (FormatException e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
        public static SkillInfo GetSkillByID(int id)
        {
            return Skills[id];
        }
        /// <summary>
        /// 获得符合五个属性的技能ID
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns>
        /// NOT NULL：技能ID数组
        /// NULL：无符合的
        /// </returns>
        public static int[] GetAvailableSkillIDs(int[] attributes)
        {
            List<int> availableSkills = new List<int>();
            SkillInfo[] skillInfos = Skills;
            for (int i = 0; i < skillInfos.Length; i++)
            {
                if (skillInfos[i].IfAvailable(attributes))
                    availableSkills.Add(skillInfos[i].ID);
            }
            return availableSkills.ToArray();
        }
        /// <summary>
        /// 获得符合五个属性的，能够从学校习得的技能
        /// </summary>
        /// <param name="attributes">五个属性</param>
        /// <returns>
        /// NOT NULL：技能数组
        /// NULL：无符合的
        /// </returns>
        public static SkillInfo[] GetAvailableSkills(int[] attributes, ESkillComeFrom comeFrom)
        {
            List<SkillInfo> availableSkills = new List<SkillInfo>();
            SkillInfo[] skillInfos = Skills;
            for (int i = 0; i < skillInfos.Length; i++)
            {
                if (skillInfos[i].ComeFrom == comeFrom && skillInfos[i].IfAvailable(attributes))
                    availableSkills.Add(skillInfos[i]);
            }
            return availableSkills.ToArray();
        }
        //----------技能----------↑----------物品----------↓
        private static Hashtable itemTable;
        private static Hashtable ItemTable { get { if (itemTable == null) LoadItemInfoFromXml(); return itemTable; } }
        private static void LoadItemInfoFromXml()
        {
            string xmlString = Resources.Load("xml/items").ToString();
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlString);
            XmlNodeList itemNodes = document.SelectSingleNode("items").ChildNodes;
            itemTable = new Hashtable();
            foreach (XmlNode itemNode in itemNodes)
            {
                ItemInfo item = null;
                if (itemNode.Name.Equals("weapon"))
                    item = new WeaponInfo(itemNode);
                else if (itemNode.Name.Equals("material"))
                    item = new MaterialInfo(itemNode);
                else if (itemNode.Name.Equals("special"))
                    item = new SpecialInfo(itemNode);
                else
                {
                    Debug.LogWarning("不支持的物品类型" + itemNode.Name);
                    continue;
                }
                itemTable.Add(item.ID, item);
            }
        }
        /// <summary>
        /// 根据物品ID获取物品
        /// </summary>
        /// <typeparam name="T">ItemInfo的基类（WeaponInfo、MaterialInfo、SpecialInfo等）</typeparam>
        /// <param name="id">物品ID</param>
        /// <returns>
        /// NOT NULL：如果该类型的物品存在这个ID物品
        /// NULL：该类型的物品不存在这个ID的物品
        /// </returns>
        public static T GetItemInfoByID<T>(int id)
            where T : ItemInfo
        {
            if (!ItemTable.ContainsKey(id)) return null;
            return ItemTable[id] as T;
        }
        //----------物品----------↑
    }
    public enum ESprite
    {
        NONE = -1,
        DEVELOPING_BIG,
        DEVELOPING_SMALL,
        /// <summary>
        /// 人物在大地图中的图标
        /// </summary>
        PERSON1_B,
        PERSON1_L,
        PERSON1_T,
        PERSON2_B,
        PERSON2_L,
        PERSON2_T,
        PERSON3_B,
        PERSON3_L,
        PERSON3_T,
        PERSON4_B,
        PERSON4_L,
        PERSON4_T,
        PERSON5_B,
        PERSON5_L,
        PERSON5_T,

        /// <summary>
        /// 列车在大地图中的图标
        /// </summary>
        TRAIN,

        /// <summary>
        /// 专精大图标
        /// </summary>
        PROFESSION0_BIG,
        PROFESSION1_BIG,
        PROFESSION2_BIG,
        PROFESSION3_BIG,
        PROFESSION4_BIG,

        PROFESSION00_BIG,
        PROFESSION01_BIG,
        PROFESSION02_BIG,
        PROFESSION03_BIG,
        PROFESSION04_BIG,

        PROFESSION11_BIG,
        PROFESSION12_BIG,
        PROFESSION13_BIG,
        PROFESSION14_BIG,

        PROFESSION22_BIG,
        PROFESSION23_BIG,
        PROFESSION24_BIG,

        PROFESSION33_BIG,
        PROFESSION34_BIG,

        PROFESSION44_BIG,

        PROFESSION000_BIG,
        PROFESSION012_BIG,
        PROFESSION013_BIG,
        PROFESSION014_BIG,
        PROFESSION023_BIG,
        PROFESSION024_BIG,
        PROFESSION034_BIG,

        PROFESSION111_BIG,
        PROFESSION123_BIG,
        PROFESSION124_BIG,
        PROFESSION134_BIG,

        PROFESSION222_BIG,
        PROFESSION234_BIG,

        PROFESSION333_BIG,

        PROFESSION444_BIG,

        /// <summary>
        /// 专精小图标
        /// </summary>
        PROFESSION0_SMALL,
        PROFESSION1_SMALL,
        PROFESSION2_SMALL,
        PROFESSION3_SMALL,
        PROFESSION4_SMALL,

        PROFESSION00_SMALL,
        PROFESSION01_SMALL,
        PROFESSION02_SMALL,
        PROFESSION03_SMALL,
        PROFESSION04_SMALL,

        PROFESSION11_SMALL,
        PROFESSION12_SMALL,
        PROFESSION13_SMALL,
        PROFESSION14_SMALL,

        PROFESSION22_SMALL,
        PROFESSION23_SMALL,
        PROFESSION24_SMALL,

        PROFESSION33_SMALL,
        PROFESSION34_SMALL,

        PROFESSION44_SMALL,

        PROFESSION000_SMALL,
        PROFESSION012_SMALL,
        PROFESSION013_SMALL,
        PROFESSION014_SMALL,
        PROFESSION023_SMALL,
        PROFESSION024_SMALL,
        PROFESSION034_SMALL,

        PROFESSION111_SMALL,
        PROFESSION123_SMALL,
        PROFESSION124_SMALL,
        PROFESSION134_SMALL,

        PROFESSION222_SMALL,
        PROFESSION234_SMALL,

        PROFESSION333_SMALL,

        PROFESSION444_SMALL,

        MONEY_ICON,//金钱图标
        STRATEGY_ICON,//战略点图标
        NUM
    }
    /// <summary>
    /// 人物基础属性
    /// </summary>
    public enum EAttribute
    {
        NONE = -1,
        VITALITY,//体力
        STRENGTH,//力量
        AGILE,//敏捷
        TECHNIQUE,//技巧
        INTELLIGENCE,//智力
        NUM
    }
}