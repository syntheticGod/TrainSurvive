/*
 * 描述：
 * 作者：����
 * 创建时间：2018/10/30 22:25:28
 * 版本：v0.1
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 测试代码
{
    public abstract class ClassA
    {
        public abstract int id
        {
            get;
            set;
        }
        public ClassA Clone()
        {
            return this.MemberwiseClone() as ClassA;
        }
    }

}
