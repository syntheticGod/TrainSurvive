/*
 * 描述：这是一个buff控制类
 * 从xml中获取各种buff保存到对象池中
 * 作者：王安鑫
 * 创建时间：2018/12/11 17:10:55
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using static WorldBattle.ChangePropertyValueBuff;
using static WorldBattle.ChangePropertyRateBuff;

namespace WorldBattle {
    public class BuffFactory {

        /// <summary>
        /// 现有的buff类型
        /// </summary>
        enum BuffType {
            NONE = -1,

            //改变比率的buff
            CHANGE_RATE,

            //改变值的buff
            CHANGE_VALUE,

            NUM
        }

        //通过string返回对应的map
        private static Dictionary<string, Buff> buffMap;

        //根据指定的buff名获取Buff效果
        public static Buff getBuff(string buff, BattleActor battleActor) {
            //如果没进行过初始化，则对其进行第一次初始化
            if (buffMap == null) {
                initBuffSystem();
            }

            //根据buff名获取指定buff
            Buff noneBuff = new Buff(battleActor, buff);
            if (buffMap.TryGetValue(buff, out noneBuff) == false) {
                Debug.Log("正在获取未知的buff？" + buff + battleActor);
                Debug.Break();
            }

            //返回该类的clone（并绑定指定对象）
            return (Buff)noneBuff.Clone(battleActor);
        }

        //初始化buff系统（从json数据中读取各种buff效果）
        public static void initBuffSystem() {
            //初始化buff映射表
            buffMap = new Dictionary<string, Buff>();

            //获取Buff库的xml文件
            string xmlString = Resources.Load("xml/Buff").ToString();

            //解析xml
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            //查找<BuffPool>
            XmlNode rootNode = xmlDoc.SelectSingleNode("BuffPool");
            //遍历所有子节点
            foreach (XmlNode curBuffNode in rootNode.ChildNodes) {
                //先获取buff的name
                string buffName = curBuffNode.Attributes["name"].Value;
                //每次出创建一个绑定空角色的buff
                Buff buff = new Buff(null, buffName);

                //遍历buff的每个子效果
                foreach (XmlNode buffEffectNode in curBuffNode.ChildNodes) {
                    //获取当前buff子效果改变的类型
                    string changeWhat = buffEffectNode.Attributes["changeWhat"].Value;
                    BuffType buffType = (BuffType)Enum.Parse(typeof(BuffType), changeWhat);

                    //创建一个基础buff类
                    BuffBase buffBase = null;

                    //根据当前buff类型生成对应的子buff
                    switch(buffType) {
                        //获取改变比率的buff
                        case BuffType.CHANGE_RATE:
                            buffBase = getChangeRateBuff(buffEffectNode);
                            break;

                        //获取改变值的buff
                        case BuffType.CHANGE_VALUE:
                            buffBase = getChangeValueBuff(buffEffectNode);
                            break;
                    }

                    //测试
                    if (buffBase == null) {
                        Debug.Log("Buff获取失败？？？");
                        Debug.Break();
                    }

                    //将buff添加到buff列表中
                    buff.buffList.Add(buffBase);
                }

                //将当前buff添加到映射表中
                buffMap.Add(buff.buffType, buff);
            }
        }

        /// <summary>
        /// 根据指定的xmlNode生成一个改变比率的buff（空角色绑定）
        /// </summary>
        /// <param name="buffEffectNode"></param>
        /// <returns></returns>
        public static ChangePropertyRateBuff getChangeRateBuff(XmlNode buffEffectNode) {
            //设置当前属性修改的值
            float changeRate = float.Parse(buffEffectNode.Attributes["changeRate"].Value);

            //设置当前最大的持续时间
            float maxDurationTime = float.Parse(buffEffectNode.Attributes["maxDurationTime"].Value);

            //设置当前所要更改的角色属性
            string propertyEnum = buffEffectNode.Attributes["propertyEnum"].Value;
            RatePropertyEnum actorProperty = (RatePropertyEnum)Enum.Parse(typeof(RatePropertyEnum), propertyEnum);

            //是否可叠加
            bool isCanOverlay = bool.Parse(buffEffectNode.Attributes["isCanOverlay"].Value);
            //最大的层数（如果可叠加，最大层数默认999，如果不可叠加，最大层数默认为1）
            int maxFloorNum = int.Parse(buffEffectNode.Attributes["maxFloorNum"].Value);

            return new ChangePropertyRateBuff(
                null,
                changeRate,
                maxDurationTime,
                actorProperty,
                isCanOverlay,
                maxFloorNum);
        }

        /// <summary>
        /// 根据指定的xmlNode生成一个改变值的buff（空角色绑定）
        /// </summary>
        /// <param name="buffEffectNode"></param>
        /// <returns></returns>
        public static ChangePropertyValueBuff getChangeValueBuff(XmlNode buffEffectNode) {
            //设置当前属性修改的值
            float changeValue = float.Parse(buffEffectNode.Attributes["changeValue"].Value);

            //设置间隔时间
            float intervalTime = float.Parse(buffEffectNode.Attributes["intervalTime"].Value);

            //设置当前最大的持续时间
            float maxDurationTime = float.Parse(buffEffectNode.Attributes["maxDurationTime"].Value);

            //设置当前所要更改的角色属性
            string propertyEnum = buffEffectNode.Attributes["propertyEnum"].Value;
            ValuePropertyEnum actorProperty = (ValuePropertyEnum)Enum.Parse(typeof(ValuePropertyEnum), propertyEnum);

            //是否可叠加
            bool isCanOverlay = bool.Parse(buffEffectNode.Attributes["isCanOverlay"].Value);
            //最大的层数（如果可叠加，最大层数默认999，如果不可叠加，最大层数默认为1）
            int maxFloorNum = int.Parse(buffEffectNode.Attributes["maxFloorNum"].Value);

            return new ChangePropertyValueBuff(
                null,
                changeValue,
                maxDurationTime,
                intervalTime,
                actorProperty,
                isCanOverlay,
                maxFloorNum);
        }
    }
}
