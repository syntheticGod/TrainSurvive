/*
 * 描述：这是生成城镇和铁轨的类
 * 必须有一个GameObject叫MapBuild并且有脚本MapGenerate
 * 城镇生成算法是将大地图划分为相等大小的若干个块，城镇在块中生成，保证了城镇之间的距离及随机性
 * 铁轨生成是相邻块的城镇才可搭建铁轨，通过先使x轴相等，再使y轴相等来保证铁轨生成的一致性
 * 
 * 作者：王安鑫
 * 创建时间：2018/11/1 20:24:51
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldMap {
    public class TownsRailGenerate : MonoBehaviour {
        private const float levelOfRail = -0.5F;
        private const float levelOfTown = -0.5F;
        //城镇的图标
        public GameObject townObject;
        //铁轨直线图标
        public GameObject railDirectObject;
        //铁轨转弯图标
        public GameObject railTurnObject;

        //城镇的个数(最好是一个数的平方)
        public int townsNum = 49;

        //城镇图标在一个地块的偏移量
        public Vector3 townOffsetVec3 = new Vector3(0.0f, 0.0f, -0.03f);
        //铁轨图标在一个地块的偏移量
        public Vector3 railOffsetVec3 = new Vector3(0.0f, 0.0f, -0.02f);

        //设置城镇最近的距离
        public int minDist = 10;

        //获取总的地图属性
        private MapGenerate mapGenerate;
        //将城镇图标放到同一GameObject下
        private GameObject townParentObject;
        //将铁轨图标放到同一GameObject下
        private GameObject railParentObject;
        //城镇类
        public Town[,] towns;
        //地图类
        public Map mapData;

        //开始生成
        public void StartGenerate() {
            //对各对象进行初始化
            mapGenerate = GameObject.Find("MapBuild").GetComponent<MapGenerate>();
            townParentObject = new GameObject("towns");
            railParentObject = new GameObject("rails");
            townParentObject.transform.parent = mapGenerate.mapRootObject.transform;
            railParentObject.transform.parent = mapGenerate.mapRootObject.transform;

            //获得地图类
            mapData = mapGenerate.mapData;
            towns = mapData.towns;

            //如果是第一次载入就生成城镇
            if (mapGenerate.isCreateMap) {
                //生成城镇
                BuildTowns();
                //生成铁轨
                BuildRails(mapData);
            }

            //对城镇进行绘画
            PaintTowns();

            //对铁轨进行绘画
            PaintRails();
        }

        /**只生成城镇类
         * 假设地图是nxn
         * 只会生成城镇个数的pow(floor(sqrt(n)), 2)
         * 城镇单位为1x1
        **/
        private void BuildTowns() {
            //先计算出将整块地图分成多少个大块
            int townRowNum = Mathf.FloorToInt(Mathf.Sqrt((float)townsNum));
            int townColNum = townRowNum;
            int offsetX = mapData.rowNum / townRowNum;
            int offsetZ = mapData.colNum / townRowNum;

            //初始化城镇类
            mapData.towns = new Town[townRowNum, townColNum];
            towns = mapData.towns;

            //在大区块的边缘不生成城镇
            int marginX = Mathf.Min(minDist / 4, offsetX / 4);
            int marginZ = Mathf.Min(minDist / 4, offsetZ / 4);

            //按照大块来生成城镇，每个大块生成一个城镇
            //每个大块靠边界的地方不生成城镇，保证每个城镇的最远距离
            for (int i = 0; i < townRowNum; i++) {
                for (int j = 0; j < townColNum; j++) {
                    //生成在该区域内的城镇
                    int posx = Random.Range(marginX, offsetX - marginX) + i * offsetX;
                    int posz = Random.Range(marginZ, offsetZ - marginZ) + j * offsetZ;
                    
                    //Debug.Log(mapData.data[posx, posz].townPos);
                    towns[i, j] = new Town(new Vector2Int(posx, posz));

                    //设置城镇的逻辑属性
                    SetTownProperty(mapData, new Vector2Int(i, j), towns[i, j].position);
                }
            }
        }

        /// <summary>
        /// 在地图上进行城镇的绘画
        /// 并赋予地块相应的属性
        /// </summary>
        private void PaintTowns() {
            //获取行数和列数
            int townRowNum = towns.GetLength(0);
            int townColNum = towns.GetLength(1);

            for (int i = 0; i < townRowNum; i++) {
                for (int j = 0; j < townColNum; j++) {
                    Vector2Int mapPos = towns[i, j].position;
                    //对城镇图标进行绘画
                    GameObject o = Instantiate(townObject,
                        mapGenerate.orign + new Vector3(mapGenerate.spawnOffsetX * mapPos.x, mapGenerate.spawnOffsetZ * mapPos.y, 0),
                        townObject.transform.rotation);
                    //将城镇图标放在同一gameObject下
                    o.transform.parent = townParentObject.transform;
                    //设置城镇的偏移
                    o.transform.position = o.transform.position + townOffsetVec3;

                    //设置城镇的gameObject
                    mapData.spowns[mapPos.x, mapPos.y].SetSpawnObject(SpawnPoint.SpawnObjectEnum.TOWN, o);
                }
            }
        }

        public static void SetTownProperty(Map map, Vector2Int townPos, Vector2Int mapPos) {
            //设置城镇属性
            map.spowns[mapPos.x, mapPos.y].SetSpecialTerrain(SpawnPoint.SpecialTerrainEnum.TOWN);
            map.spowns[mapPos.x, mapPos.y].SetTownId(townPos);
        }

        /**生成铁轨
         *只在相邻大块的城镇生成铁轨
         * 城镇连接规律，TownPos的x轴或y轴越小为from，下一个为to
         */
        public static void BuildRails(Map map) {
            //获取行数和列数
            int townRowNum = map.towns.GetLength(0);
            int townColNum = map.towns.GetLength(1);
            //获取城镇
            Town[,] towns = map.towns;

            //先连接第一行
            for (int j = 1; j < townRowNum; j++) {
                ConnectTown(map, towns[0, j - 1], towns[0, j]);
            }

            //在连接每一行
            for (int i = 1; i < townRowNum; i++) {
                //连接每一行的第一个
                ConnectTown(map, towns[i - 1, 0], towns[i, 0]);

                for (int j = 1; j < townColNum; j++) {
                    ConnectTown(map, towns[i, j - 1], towns[i, j]);
                    ConnectTown(map, towns[i - 1, j], towns[i, j]);
                }
            }
        }

        /** 将城镇连接起来
         * 铁轨的生成算法是，先使x轴相等，再使y轴相等
         * 根据铁轨的路径和转折点绘画出当前铁轨
         */
        private static void ConnectTown(Map map, Town fromTown, Town toTown) {
            //获得城镇的坐标
            Vector2Int from = fromTown.position;
            Vector2Int to = toTown.position;
            //Debug.Log(from + "  " + to);

            //保存当前铁轨轨迹（可能去掉重复铁轨）
            List<Vector2Int> railPath = new List<Vector2Int>();
            //x轴方向的铁轨数目
            int xRailNum = 0;
            //转弯的角度
            int railTurnAngle = -1;

            //先使x轴相等
            if (from.x != to.x) {
                int dir = from.x < to.x ? 1 : -1;
                int posx = 0;
                int posz = from.y;

                for (posx = from.x + dir; posx != to.x; posx += dir) {
                    //去掉重复铁轨
                    if (map.spowns[posx, posz].specialTerrainType != SpawnPoint.SpecialTerrainEnum.NONE) {
                        return;
                    }
                    railPath.Add(new Vector2Int(posx, posz));
                }

                xRailNum = railPath.Count;

                //x轴最后一段铁轨特殊处理（拐弯处）
                if (from.y != to.y) {
                    posx = to.x;
                    posz = from.y;
                    if (map.spowns[posx, posz].specialTerrainType != SpawnPoint.SpecialTerrainEnum.NONE) {
                        return;
                    }

                    //对铁轨图标进行旋转
                    if (from.x < to.x) {
                        if (from.y < to.y) {
                            railTurnAngle = 180;
                        } else {
                            railTurnAngle = 270;
                        }
                    } else {
                        if (from.y < to.y) {
                            railTurnAngle = 90;
                        } else {
                            railTurnAngle = 0;
                        }
                    }
                }
            }

            //让z轴相等
            if (from.y != to.y) {
                int dir = from.y < to.y ? 1 : -1;
                int posx = (int)to.x;
                for (int posz = (int)from.y + dir; posz != (int)to.y; posz += dir) {
                    //去掉重复铁轨
                    if (map.spowns[posx, posz].specialTerrainType != SpawnPoint.SpecialTerrainEnum.NONE) {
                        return;
                    }
                    railPath.Add(new Vector2Int(posx, posz));
                }
            }

            //如果有拐弯的铁轨，将其在最后加入
            if (railTurnAngle != -1) {
                railPath.Add(new Vector2Int(to.x, from.y));
            }

            //设置城镇之间的连接关系（当前城镇连接下一个城镇）
            fromTown.AddConnectTown(toTown);

            //增加一个新的铁轨连接路径，将相应信息保存
            fromTown.railPaths.Add(new Town.RailPath());
            //计算出最后一个下标
            int lastIndex = fromTown.railPaths.Count - 1;
            
            //保存铁轨路径的三个信息
            fromTown.railPaths[lastIndex].railPath = railPath;
            fromTown.railPaths[lastIndex].xRailNum = xRailNum;
            fromTown.railPaths[lastIndex].railTurnAngle = railTurnAngle;

            //设置铁轨的属性
            //记录起始和终止的城镇坐标
            Vector2Int fromTownPos = map.spowns[from.x, from.y].townPos;
            Vector2Int toTownPos = map.spowns[to.x, to.y].townPos;

            //如果无拐弯的铁轨
            int directRailNum = railTurnAngle == -1 ? railPath.Count : railPath.Count - 1;
            //先绘画出x轴方向的铁轨
            for (int curRailIndex = 0; curRailIndex < directRailNum; curRailIndex++) {
                //设置铁轨属性
                SetSpawnPointRailProperty(map, 
                    railPath[curRailIndex].x,
                    railPath[curRailIndex].y,
                    fromTownPos, toTownPos);
            }

            //x轴最后一段铁轨特殊处理（拐弯处）
            if (railTurnAngle != -1) {
                //设置铁轨属性
                SetSpawnPointRailProperty(map,
                    railPath[railPath.Count - 1].x,
                    railPath[railPath.Count - 1].y,
                    fromTownPos, toTownPos);
            }
        }

        private void PaintRails() {
            //获取城镇的行数和列数
            int townRowNum = towns.GetLength(0);
            int townColNum = towns.GetLength(1);

            for (int i = 0; i < townRowNum; i++) {
                for (int j = 0; j < townColNum; j++) {
                    //根据是否有下一个城镇，对铁轨进行绘画
                    for (int k = 0; k < towns[i, j].connectTowns.Count; k++) {
                        PaintRailPath(towns[i, j].railPaths[k], towns[i, j], towns[i, j].connectTowns[k]);
                    }
                }
            }
        }

        /// <summary>
        /// 对铁轨进行绘画处理
        /// </summary>
        /// <param name="railPath">记录每个铁轨坐标的队列</param>
        /// <param name="xRailNum">x轴铁轨的个数（横）</param>
        /// <param name="railTurnAngle">如果有转弯的铁轨，这是转弯铁轨的角度；如果没有，则为-1</param>
        /// <param name="from">起点城镇位置</param>
        /// <param name="to">终点城镇的位置</param>
        private void PaintRailPath(Town.RailPath townRailPaths, Town fromTown, Town toTown) {
            //获取railPath三个属性
            List<Vector2Int> railPath = townRailPaths.railPath;
            int railTurnAngle = townRailPaths.railTurnAngle;
            int xRailNum = townRailPaths.xRailNum;

            //获得城镇的坐标
            Vector2Int from = fromTown.position;
            Vector2Int to = toTown.position;

            //记录起始和终止的城镇坐标
            Vector2Int fromTownPos = mapData.spowns[from.x, from.y].townPos;
            Vector2Int toTownPos = mapData.spowns[to.x, to.y].townPos;

            //如果无拐弯的铁轨
            int directRailNum = railTurnAngle == -1 ? railPath.Count : railPath.Count - 1;
            //先绘画出x轴方向的铁轨
            for (int curRailIndex = 0; curRailIndex < directRailNum; curRailIndex++) {
                int posx = railPath[curRailIndex].x;
                int posz = railPath[curRailIndex].y;

                //对铁轨图标进行绘画
                if (curRailIndex < xRailNum) {
                    PaintSingleRail(posx, posz, true, new Vector3());
                } else {
                    PaintSingleRail(posx, posz, true, new Vector3(0, 0, 90));
                }
                //Debug.Log(mapData.data[posx, posz].townPos + "  " + mapData.data[posx, posz].startTownPos);
            }

            //x轴最后一段铁轨特殊处理（拐弯处）
            if (railTurnAngle != -1) {
                int posx = railPath[railPath.Count - 1].x;
                int posz = railPath[railPath.Count - 1].y;

                //对铁轨图标进行绘画
                PaintSingleRail(posx, posz, false, new Vector3(0, 0, railTurnAngle));
            }
        }

        /// <summary>
        /// 设置铁轨(属性，终点城镇坐标，起点城镇坐标)
        /// </summary>
        /// <param name="posx">当前铁轨的坐标</param>
        /// <param name="posz"></param>
        /// <param name="fromTownPos">起始城镇的位置</param>
        /// <param name="toTownPos">终点城镇的位置</param>
        private static void SetSpawnPointRailProperty(Map map, int posx, int posz, Vector2Int fromTownPos, Vector2Int toTownPos) {
            map.spowns[posx, posz].SetSpecialTerrain(SpawnPoint.SpecialTerrainEnum.RAIL);
            map.spowns[posx, posz].SetStartTownId(fromTownPos);
            map.spowns[posx, posz].SetTownId(toTownPos);
        }

        /// <summary>
        /// 对单独铁轨进行绘画处理
        /// </summary>
        /// <param name="posx">当前铁轨x轴位置</param>
        /// <param name="posz">当前铁轨z轴位置</param>
        /// <param name="isDirect">当前铁轨是否是直的铁轨</param>
        /// <param name="rotate">当前铁轨需要旋转的角度</param>
        private void PaintSingleRail(int posx, int posz, bool isDirect, Vector3 rotate) {
            //判断是转弯的object还是旋转的object
            GameObject railObject = isDirect ? railDirectObject : railTurnObject;

            if (mapGenerate == null || railObject == null) {
                return;
            }

            //对铁轨图标进行绘画
            GameObject o = Instantiate(railObject,
                mapGenerate.orign + new Vector3(mapGenerate.spawnOffsetX * posx, mapGenerate.spawnOffsetZ * posz, 0),
                railObject.transform.rotation);
            //将铁轨图标放在同一gameObject下
            o.transform.parent = railParentObject.transform;
            //设置铁轨的偏移
            o.transform.position = o.transform.position + railOffsetVec3;
            //对铁轨进行旋转
            o.transform.rotation = Quaternion.Euler(o.transform.eulerAngles + rotate);

            //绑定铁轨的gameObject
            mapData.spowns[posx, posz].SetSpawnObject(SpawnPoint.SpawnObjectEnum.RAIL, o);
        }
    }
}

