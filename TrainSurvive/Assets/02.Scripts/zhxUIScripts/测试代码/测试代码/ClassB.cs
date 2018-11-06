/*
 * 描述：
 * 作者：����
 * 创建时间：2018/11/6 23:29:41
 * 版本：v0.1
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 测试代码
{
    class ClassB : ClassA
    {
        private int ID;
        public int ID2;
        public override int id {
            get
            {
                return ID;
            }
            set
            {
                ID = value;
            }
        }
        public ClassB()
        {

        }
        public ClassB(int id)
        {
            this.ID = id;
        }
        public override string ToString()
        {
            return "ClassB:" + ID.ToString();
        }
        ~ClassB()
        {
            Console.WriteLine("我被分解了");
        }
    }
}
