/*
 * 描述：
 * 作者：李巡
 * 创建时间：2019/1/21 15:34:52
 * 版本：v0.7
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;

namespace Story.MyTools{
    public static class Tools{
        static Random rd = new Random();

        public static int random(int max){
            return rd.Next(max);
        }

        public static int random(int min, int max){
            return rd.Next(min, max);
        }

        /// <summary>
        /// 从n个数中随机选m个不重复的数
        /// </summary>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static List<int> randomSelect(int n,int m){
            List<int> selected = new List<int>();
            
            for (int i = 0; i < m; i++){
                int rdNum = rd.Next(0, n);
                while (selected.Contains(rdNum)){
                    rdNum = rd.Next(0, n);
                }
                selected.Add(rdNum);
            }
            return selected;
        }
    }
}