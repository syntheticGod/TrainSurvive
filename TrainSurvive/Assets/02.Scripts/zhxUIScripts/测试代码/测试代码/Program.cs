/*
 * 描述：
 * 作者：����
 * 创建时间：2018/11/6 23:29:40
 * 版本：v0.1
 */
/*
 * 描述：
 * 作者：����
 * 创建时间：2018/10/30 22:25:28
 * 版本：v0.1
 */
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace 测试代码
{
    class Program
    {
        static ClassA fun(ClassA b)
        {
            ClassA temp = Copy<ClassA, ClassB>.Trans(b);
            Console.WriteLine(temp.GetType().Name);
            Console.WriteLine(b);
            return b;
        }
        static void Main(string[] args)
        {
            Dictionary<int, List<ClassA>> dc = new Dictionary<int, List<ClassA>>();
            List<ClassA> list = new List<ClassA>();
            ClassC c = new ClassC(1);
            ClassB b = new ClassB(2);
            ClassA a = b.Clone();
            Console.WriteLine(a);
            Console.WriteLine(b);
            b.id = 5;
            Console.WriteLine(a);
            Console.WriteLine(b);

            //bool flag = true;
            //List<ClassA> list = new List<ClassA>();
            //for(int i=0; i<20; ++i)
            //{
            //    if (flag)
            //    {
            //        list.Add(new ClassB(i));
            //        flag = !flag;
            //    }
            //    else
            //    {
            //        list.Add(new ClassC(i));
            //        flag = !flag;
            //    }
            //}
            //Console.WriteLine(list.Count);
            //for(int i=0; i<list.Count; ++i)
            //{
            //    Console.WriteLine("Type: {0}    id: {1}",list[i].GetType().Name,list[i].id);
            //}
            Console.ReadKey();
        }
    }
}
