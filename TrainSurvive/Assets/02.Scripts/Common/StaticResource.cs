/*
 * 描述：静态资源
 * 作者：项叶盛
 * 创建时间：2018/11/12 21:46:39
 * 版本：v0.1
 */
using UnityEngine;
using System.Xml;

using TTT.Utility;

namespace TTT.Resource
{
    public static class StaticResource
    {
        /// <summary>
        /// 根据EProfession的序号排序
        /// </summary>
        private static Profession[] professions;
        /// <summary>
        /// 一级专精映射
        /// </summary>
        private static Profession[] firstProfessions;
        /// <summary>
        /// 二级专精映射
        /// </summary>
        private static Profession[,] secondProfessions;
        private static void LoadProfessionFromXml()
        {
            professions = new Profession[(int)EProfession.NUM];
            firstProfessions = new Profession[NumOfAttribute];
            secondProfessions = new Profession[NumOfAttribute, NumOfAttribute];
            string xmlString = Resources.Load("xml/profession").ToString();
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlString);

            XmlNode root = document.SelectSingleNode("professions");
            string spriteFloder = root.SelectSingleNode("spriteFloder").InnerText + "/";

            XmlNodeList professionList = root.SelectNodes("profession");
            int indexOfProf = 0;
            foreach (XmlNode professionNode in professionList)
            {
                XmlNodeList abiReqList = professionNode.SelectNodes("abiReq");
                Profession.AbiReq[] abiReq = new Profession.AbiReq[abiReqList.Count];
                EProfession eProfession = EProfession.NONE + 1 + int.Parse(professionNode.Attributes["type"].Value);
                int indexOfAbi = 0;
                foreach (XmlNode abiReqNode in abiReqList)
                {
                    Profession.AbiReq tempAbi = new Profession.AbiReq();
                    tempAbi.Abi = EAttribute.NONE + 1 + int.Parse(abiReqNode.Attributes["abi"].Value);
                    tempAbi.Number = int.Parse(abiReqNode.Attributes["Number"].Value);
                    tempAbi.costFix = float.Parse(abiReqNode.Attributes["costFix"].Value);
                    abiReq[indexOfAbi++] = tempAbi;
                }
                string iconFile = spriteFloder + professionNode.Attributes["sprite"].Value;
                string name = professionNode.Attributes["name"].Value;
                string info = professionNode.Attributes["info"].Value;
                Profession profession = new Profession(abiReq, name, eProfession, iconFile, info);
                professions[indexOfProf] = profession;
                if (indexOfProf < NumOfAttribute)
                {
                    //一级专精
                    firstProfessions[indexOfProf] = profession;
                }
                else
                {
                    //二级专精
                    if (profession.AbiReqs.Length == 1)
                    {
                        int x = (int)profession.AbiReqs[0].Abi;
                        secondProfessions[x, x] = profession;
                    }
                    else
                    {
                        int x = (int)profession.AbiReqs[0].Abi;
                        int y = (int)profession.AbiReqs[1].Abi;
                        secondProfessions[y, x] = secondProfessions[x, y] = profession;
                    }
                }
                indexOfProf++;
            }
        }
        private const int NumOfAttribute = 5;
        public static Profession GetProfession(EProfession professionType)
        {
            if (professions == null)
            {
                LoadProfessionFromXml();
            }
            return professions[(int)professionType];
        }
        public static Profession[] GetNextProfessions(Profession profession)
        {
            if (firstProfessions == null) LoadProfessionFromXml();
            Profession[] result = new Profession[NumOfAttribute];
            if (profession == null)
            {
                for (int i = 0; i < result.Length; i++)
                    result[i] = firstProfessions[i];
            }
            else if (profession.Level == EProfessionLevel.LEVEL1)
            {
                for (int i = 0; i < result.Length; i++)
                    result[i] = secondProfessions[(int)profession.Type, i];
            }
            else if(profession.Level == EProfessionLevel.LEVEL2)
            {
                //TODO 三级专精
                Debug.LogError("三级专精——待扩展");
                return null;
            }
            else if(profession.Level == EProfessionLevel.LEVEL3)
            {
                //顶级专精
                return null;
            }
            return result;
        }
        //方块大小
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
        private static string[] AttributeName { get; } = { "体力", "力量", "敏捷", "技巧", "智力" };
        public static int AttributeCount { get { return AttributeName.Length; } }
        public static string GetAttributeName(int index)
        {
            if (index >= AttributeName.Length)
            {
                Debug.LogError("获取属性名失败，index=" + index);
                return "";
            }
            return AttributeName[index];
        }
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
        private static Sprite[] spriteStore = new Sprite[(int)ESprite.NUM];
        private static string[] spriteFileName = {
            "Sprite/map/person/person1_bottom_0", "Sprite/map/person/person1_left_0", "Sprite/map/person/person1_top_0",
            "Sprite/map/person/person2_bottom", "Sprite/map/person/person2_left", "Sprite/map/person/person2_top",
            "Sprite/map/person/person3_bottom", "Sprite/map/person/person3_left", "Sprite/map/person/person3_top",
            "Sprite/map/person/person4_bottom", "Sprite/map/person/person4_left", "Sprite/map/person/person4_top",
            "Sprite/map/person/person5_bottom", "Sprite/map/person/person5_left", "Sprite/map/person/person5_top",

            "Sprite/map/Train",
        };
        public static Sprite GetSprite(ESprite eSprite)
        {
            if(spriteStore[(int)eSprite] == null)
            {
                if(MathTool.IfBetweenBoth((int)ESprite.PERSON1_B, (int)ESprite.PERSON1_T, (int)eSprite))
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
                    for(ESprite i = ESprite.PERSON2_B; i < ESprite.PERSON5_T; i++)
                    {
                        spriteStore[(int)i] = sprites[index++];
                    }
                }
                else
                {
                    spriteStore[(int)eSprite] = Resources.Load<Sprite>(spriteFileName[(int)eSprite]);
                }
                if(spriteStore[(int)eSprite] == null)
                {
                    Debug.LogError("Sprite资源不存在");
                }
            }
            return spriteStore[(int)eSprite];
        }
    }
    public enum ESprite
    {
        NONE = -1,
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
        TRAIN,
        NUM
    }
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