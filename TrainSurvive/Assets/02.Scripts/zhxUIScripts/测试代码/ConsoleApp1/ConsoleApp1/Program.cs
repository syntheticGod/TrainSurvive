/*
 * 描述：
 * 作者：张皓翔
 * 创建时间：2018/12/9 15:45:08
 * 版本：v0.1
 */
using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> list = new List<int>();
            list.Add(3);
            List<int> list2 = list;
            list2.Add(4);
            foreach(var item in list)
            {
                Console.WriteLine(item);
            }
            Console.Read();
        }
    }
}
