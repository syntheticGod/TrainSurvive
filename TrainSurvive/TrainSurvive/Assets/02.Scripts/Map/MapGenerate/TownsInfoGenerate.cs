/*
 * 描述：城镇信息的随机生成
 * 作者：项叶盛
 * 创建时间：2018/11/22 0:59:42
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;

namespace WorldMap
{
    public class TownsInfoGenerate
    {
        public Model.Town[] Random(Town[,] towns)
        {

            int townNumOfX = towns.GetLength(0);
            int townNumOfZ = towns.GetLength(1);
            Model.Town[] townInfos = new Model.Town[townNumOfX * townNumOfZ];
            int index = 0;
            for (int x = 0; x < townNumOfX; ++x)
                for (int z = 0; z < townNumOfZ; ++z)
                {
                    Model.Town town = Model.Town.Random();
                    town.PosIndexX = towns[x, z].position.x;
                    town.PosIndexY = towns[x, z].position.y;
                    Model.SerializableVector2Int posKey = new Model.SerializableVector2Int(town.PosIndexX, town.PosIndexY);
                    townInfos[index++] = town;
                }
            return townInfos;
        }
        /// <summary>
        /// 从文件中加载数据
        /// </summary>
        public void LoadData()
        {

        }
    }
}