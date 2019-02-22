/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/1/29 12:49:14
 * 版本：v0.7
 */
using UnityEngine;
using System.Collections;
using System.Xml;
using TTT.Resource;
using TTT.Utility;
using TTT.Xml;

namespace WorldMap.Model
{
    public class NpcInfo
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        /// <summary>
        /// 性别。男：1；女：0
        /// </summary>
        public bool Gender { get; private set; }
        public int[] AttriNumber { get; private set; }
        public EAttribute[] Professions { get; private set; }
        public int WeaponID { get; private set; }
        public int DecorationsID { get; private set; }
        public int Birthplace { get; private set; }
        public string Description { get; private set; }
        public NpcInfo(XmlNode node)
        {
            ID = int.Parse(node.Attributes["id"].Value);
            Name = node.Attributes["name"].Value;
            Gender = int.Parse(node.Attributes["gender"].Value) == 1;
            AttriNumber = new int[(int)EAttribute.NUM];
            AttriNumber[(int)EAttribute.VITALITY] = int.Parse(node.Attributes["vitality"].Value);
            AttriNumber[(int)EAttribute.STRENGTH] = int.Parse(node.Attributes["strength"].Value);
            AttriNumber[(int)EAttribute.AGILE] = int.Parse(node.Attributes["agile"].Value);
            AttriNumber[(int)EAttribute.TECHNIQUE] = int.Parse(node.Attributes["technique"].Value);
            AttriNumber[(int)EAttribute.INTELLIGENCE] = int.Parse(node.Attributes["intelligence"].Value);
            Professions = new EAttribute[3];
            for (int i = 0; i < Professions.Length; i++)
                Professions[i] = Compile(node.Attributes["profession" + (i + 1)].Value);
            WeaponID = int.Parse(node.Attributes["weaponID"].Value);
            DecorationsID = int.Parse(node.Attributes["decorationsID"].Value);
            Birthplace = CompileBirthplace(node.Attributes["birthplace"].Value);
            Description = node.Attributes["description"].Value;
        }
        private int CompileBirthplace(string birthplace)
        {
            if (birthplace.Equals("-")) return -1;
            if (birthplace[0] == '(')
            {
                birthplace = birthplace.Remove(birthplace.Length - 1, 1);
                birthplace = birthplace.Remove(0, 1);
                string[] coord = birthplace.Split(',');
                int x = int.Parse(coord[0]);
                int y = int.Parse(coord[1]);
                TownData town;
                if (!World.getInstance().Towns.Find(x, y, out town))
                {
                    Debug.LogError("找不到区块坐标为 (" + x + "," + y + ")" + "中的城镇，默认成不分配。");
                    return -1;
                }
                return town.ID;
            }
            int birthtown = int.Parse(birthplace);
            if (birthtown == 0)
                return TownInfo.RandomCommenID();
            return birthtown;
        }
        private EAttribute Compile(string profession)
        {
            switch (profession)
            {
                case "vitality":
                    return EAttribute.VITALITY;
                case "strength":
                    return EAttribute.STRENGTH;
                case "agile":
                    return EAttribute.AGILE;
                case "technique":
                    return EAttribute.TECHNIQUE;
                case "intelligence":
                    return EAttribute.INTELLIGENCE;
            }
            return EAttribute.NONE;
        }
    }
}
