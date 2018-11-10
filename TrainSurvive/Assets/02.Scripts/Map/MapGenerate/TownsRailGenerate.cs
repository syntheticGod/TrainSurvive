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

        //城镇的图标
        public GameObject townObject;
        //铁轨直线图标
        public GameObject railDirectObject;
        //铁轨转弯图标
        public GameObject railTurnObject;

        //城镇的个数(最好是一个数的平方)
        public int townsNum = 49;

        //城镇图标在一个地块的偏移量
        public Vector3 townOffsetVec3 = new Vector3(0.0f, 0.02f, 0.0f);
        //铁轨图标在一个地块的偏移量
        public Vector3 railOffsetVec3 = new Vector3(0.0f, 0.01f, 0.0f);

        //设置城镇最近的距离
        public int minDist = 10;

        //获取总的地图属性
        private MapGenerate mapGenerate;
        //将城镇图标放到同一GameObject下
        private GameObject townParentObject;
        //将铁轨图标放到同一GameObject下
        private GameObject railParentObject;
        //城镇类
        private Town[,] towns;
        //地图类
        private Map mapData;

        //开始生成
        public void StartGenerate() {
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
                BuildTowns();
            }
            //对城镇进行绘画
            PaintTowns();

            //生成铁轨
            BuildRails();
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
                    Vector2Int pos = towns[i, j].position;
                    //对城镇图标进行绘画
                    GameObject o = Instantiate(townObject,
                        mapGenerate.orign + new Vector3(mapGenerate.spawnOffsetX * pos.x, 0, mapGenerate.spawnOffsetZ * pos.y),
                        townObject.transform.rotation);
                    //将城镇图标放在同一gameObject下
                    o.transform.parent = townParentObject.transform;
                    //设置城镇的偏移
                    o.transform.position = o.transform.position + townOffsetVec3;

                    //设置城镇属性
                    mapData.data[pos.x, pos.y].SetSpecialTerrain(SpawnPoint.SpecialTerrainEnum.TOWN);
                    mapData.data[pos.x, pos.y].SetTownId(new Vector2Int(i, j));

                    //设置城镇的gameObject
                    mapData.data[pos.x, pos.y].SetSpawnObject(SpawnPoint.SpawnObjectEnum.TOWN, o);
                }
            }
        }

        /**生成铁轨
         *只在相邻大块的城镇生成铁轨
         * 城镇连接规律，x轴或y轴越小为from，下一个为to
         */
        private void BuildRails() {
            //获取行数和列数
            int townRowNum = mapData.towns.GetLength(0);
            int townColNum = mapData.towns.GetLength(1);

            //先连接第一行
            for (int j = 1; j < townRowNum; j++) {
                ConnectTown(towns[0, j - 1].position, towns[0, j].position);
            }

            //在连接每一行
            for (int i = 1; i < townRowNum; i++) {
                //连接每一行的第一个
                ConnectTown(towns[i - 1, 0].position, towns[i, 0].position);
                for (int j = 1; j < townColNum; j++) {
                    ConnectTown(towns[i, j - 1].position, towns[i, j].position);
                    ConnectTown(towns[i - 1, j].position, towns[i, j].position);
                }
            }
        }

        /** 将城镇连接起来
         * 铁轨的生成算法是，先使x轴相等，再使y轴相等
         * 根据铁轨的路径和转折点绘画出当前铁轨
         */
        private void ConnectTown(Vector2Int from, Vector2Int to) {
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
                    if (mapData.data[posx, posz].specialTerrainType != SpawnPoint.SpecialTerrainEnum.NONE) {
                        return;
                    }
                    railPath.Add(new Vector2Int(posx, posz));
                }

                xRailNum = railPath.Count;

                //x轴最后一段铁轨特殊处理（拐弯处）
                if (from.y != to.y) {
                    posx = to.x;
                    posz = from.y;
                    if (mapData.data[posx, posz].specialTerrainType != SpawnPoint.SpecialTerrainEnum.NONE) {
                        return;
                    }

                    //对铁轨图标进行旋转
                    if (from.x < to.x) {
                        if (from.y < to.y) {
                            railTurnAngle = 180;
                        } else {
                            railTurnAngle = 90;
                        }
                    } else {
                        if (from.y < to.y) {
                            railTurnAngle = 270;
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
                    if (mapData.data[posx, posz].specialTerrainType != SpawnPoint.SpecialTerrainEnum.NONE) {
                        return;
                    }
                    railPath.Add(new Vector2Int(posx, posz));
                }
            }

            //如果有拐弯的铁轨，将其在最后加入
            if (railTurnAngle != -1) {
                railPath.Add(new Vector2Int(to.x, from.y));
            }

            //对铁轨进行绘制
            PaintRailPath(railPath, xRailNum, railTurnAngle, from, to);
        }

        /// <summary>
        /// 对铁轨进行绘画处理
        /// </summary>
        /// <param name="railPath">记录每个铁轨坐标的队列</param>
        /// <param name="xRailNum">x轴铁轨的个数（横）</param>
        /// <param name="railTurnAngle">如果有转弯的铁轨，这是转弯铁轨的角度；如果没有，则为-1</param>
        /// <param name="from">起点城镇位置</param>
        /// <param name="to">终点城镇的位置</param>
        private void PaintRailPath(List<Vector2Int> railPath, int xRailNum, int railTurnAngle, Vector2Int from, Vector2Int to) {
            //记录起始和终止的城镇坐标
            Vector2Int fromTownPos = mapData.data[from.x, from.y].townPos;
            Vector2Int toTownPos = mapData.data[to.x, to.y].townPos;
            
            //如果无拐弯的铁轨
            int directRailNum = railTurnAngle == -1 ? railPath.Count : railPath.Count - 1;
            //先绘画出x轴方向的铁轨
            for (int curRailIndex = 0; curRailIndex < directRailNum; curRailIndex++) {
                int posx = railPath[curRailIndex].x;
                int posz = railPath[curRailIndex].y;

                //设置铁轨属性
                SetSpawnPointRailProperty(posx, posz, fromTownPos, toTownPos);
                //对铁轨图标进行绘画
                if (curRailIndex < xRailNum) {
                    PaintSingleRail(posx, posz, true, new Vector3());
                } else {
                    PaintSingleRail(posx, posz, true, new Vector3(0, 90, 0));
                }
                //Debug.Log(mapData.data[posx, posz].townPos + "  " + mapData.data[posx, posz].startTownPos);
            }

            //x轴最后一段铁轨特殊处理（拐弯处）
            if (railTurnAngle != -1) {
                int posx = railPath[railPath.Count - 1].x;
                int posz = railPath[railPath.Count - 1].y;

                //设置铁轨属性
                SetSpawnPointRailProperty(posx, posz, fromTownPos, toTownPos);
                //对铁轨图标进行绘画
                PaintSingleRail(posx, posz, false, new Vector3(0, railTurnAngle, 0));
            }
        }

        /// <summary>
        /// 设置铁轨(属性，终点城镇坐标，起点城镇坐标)
        /// </summary>
        /// <param name="posx">当前铁轨的坐标</param>
        /// <param name="posz"></param>
        /// <param name="fromTownPos">起始城镇的位置</param>
        /// <param name="toTownPos">终点城镇的位置</param>
        private void SetSpawnPointRailProperty(int posx, int posz, Vector2Int fromTownPos, Vector2Int toTownPos) {
            mapData.data[posx, posz].SetSpecialTerrain(SpawnPoint.SpecialTerrainEnum.RAIL);
            mapData.data[posx, posz].SetStartTownId(fromTownPos);
            mapData.data[posx, posz].SetTownId(toTownPos);
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

            //对铁轨图标进行绘画
            GameObject o = Instantiate(railObject,
                mapGenerate.orign + new Vector3(mapGenerate.spawnOffsetX * posx, 0, mapGenerate.spawnOffsetZ * posz),
                railObject.transform.rotation);
            //将铁轨图标放在同一gameObject下
            o.transform.parent = railParentObject.transform;
            //设置铁轨的偏移
            o.transform.position = o.transform.position + railOffsetVec3;
            //对铁轨进行旋转
            o.transform.rotation = Quaternion.Euler(o.transform.eulerAngles + rotate);

            //绑定铁轨的gameObject
            mapData.data[posx, posz].SetSpawnObject(SpawnPoint.SpawnObjectEnum.RAIL, o);
        }
    }
}

