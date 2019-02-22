/*
 * 描述：小队人物数据集合
 * 作者：项叶盛
 * 创建时间：2019/2/13 19:22:49
 * 版本：v0.7
 */
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using WorldMap;
using TTT.Xml;
using WorldMap.Model;

namespace TTT.Team
{
    [Serializable]
    public class PersonSet : SubjectBase, IEnumerable
    {
        public enum EAction
        {
            NONE = -1,
            /// <summary>
            /// 招募人物
            /// </summary>
            RECRUIT_PERSON,
            /// <summary>
            /// 出战
            /// </summary>
            BECOME_FIGHTER,
            /// <summary>
            ///休息
            /// </summary>
            REST
        }
        [SerializeField]
        private List<Person> persons = new List<Person>();
        public int Count { get { return persons.Count; } }
        /// <summary>
        /// 获取队伍的总属性
        /// </summary>
        /// <returns></returns>
        public int getTotalProperty()
        {
            int totalProperty = 0;
            foreach (Person p in persons)
            {
                totalProperty += p.intelligence;
                totalProperty += p.vitality;
                totalProperty += p.strength;
                totalProperty += p.technique;
                totalProperty += p.agile;
            }
            return totalProperty;
        }
        /// <summary>
        /// 最大出战人数
        /// </summary>
        public const int MAX_NUMBER_FIGHER = 5;
        /// <summary>
        /// 最多人数
        /// </summary>
        public int MaxMember { get; set; } = 15;
        /// <summary>
        /// 根据所有获得列车中的指定人物
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Person At(int index)
        {
            return persons[index];
        }
        public List<Person> CopyAll()
        {
            return new List<Person>(persons); ;
        }
        /// <summary>
        /// 根据npc的ID招募人物
        /// </summary>
        /// <param name="npcID">NPC的ID</param>
        public void RecruitNpc(int npcID)
        {
            NpcInfo npc;
            if(NpcInfoLoader.Instance.Find(npcID, out npc))
            {
                persons.Add(new Person(npc));
                Notify((int)EAction.RECRUIT_PERSON, npcID);
            }
            else
            {
                Debug.LogError("指定NPC不存在，ID" + npcID);
            }
        }
        /// <summary>
        /// 准备出战的人数
        /// </summary>
        /// <returns></returns>
        public int CountOfFighter()
        {
            int count = 0;
            foreach (Person person in persons)
            {
                if (person.ifReadyForFighting)
                    count++;
            }
            return count;
        }
        /// <summary>
        /// 随机生成人物
        /// </summary>
        /// <param name="count"></param>
        public void RandomPersons(int count)
        {
            World.getInstance().numIn = count;
            for (int i = 0; i < count; i++)
            {
                Person person = Person.RandomPerson();
                //默认全部出战，直到上限
                if (i < MAX_NUMBER_FIGHER)
                {
                    person.ifReadyForFighting = true;
                }
                persons.Add(person);
            }
        }
        /// <summary>
        /// 设置指定人物的出战设置
        /// </summary>
        /// <param name="person">人物</param>
        /// <param name="ifReadyForFight">是否出战</param>
        /// <returns>
        /// TRUE：设置成功
        /// FALSE：
        /// 当ifReadyForFight为TRUE时，出战人数不能超过上限
        /// 当ifReadyForFight为FALSE时，出战人数不能少于一人
        /// </returns>
        public bool ConfigFight(Person person, bool ifReadyForFight)
        {
            int num = 0;
            foreach (Person itr in persons)
            {
                if (itr.ifReadyForFighting) num++;
            }

            if (ifReadyForFight)
            {
                if (num >= MAX_NUMBER_FIGHER)
                    return false;
            }
            else
            {
                if (num <= 1)
                    return false;
            }
            person.ifReadyForFighting = ifReadyForFight;
            Notify(person.ifReadyForFighting ? (int)EAction.BECOME_FIGHTER : (int)EAction.REST, person);
            return true;
        }
        public IEnumerator GetEnumerator()
        {
            foreach (Person person in persons)
                yield return person;
        }
        public Person this[int index]
        {
            get { return persons[index]; }
        }
    }
}