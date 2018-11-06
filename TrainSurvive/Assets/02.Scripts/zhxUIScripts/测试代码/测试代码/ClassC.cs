using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 测试代码
{
    class ClassC:ClassA
    {
        private int ID2;
        public override int id
        {
            get
            {
                return ID2 * 10;
            }
            set
            {
                ID2 = value;
            }
        }
        public ClassC(int id)
        {
            this.id = id;
        }
        public override string ToString()
        {
            return "ClassC:" + ID2.ToString();
        }
    }
}
