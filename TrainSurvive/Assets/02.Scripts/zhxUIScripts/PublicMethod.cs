/*
 * 描述：
 * 作者：����
 * 创建时间：2018/11/6 23:29:41
 * 版本：v0.1
 */
/*
 * 描述：公有方法、一般放置常用的过程处理函数
 * 作者：张皓翔
 * 创建时间：2018/10/31 20:21:23
 * 版本：v0.1
 */

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._02.Scripts.zhxUIScripts
{
    public static class PublicMethod
    {
        public static string GenerateRdString(int length)                   //生成某个长度的随机字符串
        {
            string result = "";   
            for(int i=0; i<length; ++i)
            {
                result += ((char)Random.Range(65,81)).ToString();
                
            }
            return result;
        }
        public static void Useless()                                        //回调函数测试占位变量
        {
            Debug.Log("Test");
        }
    }
}
