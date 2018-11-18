/*
 * 描述：创建文件时，给每个文件添加注释。
 *           这是为了养成良好的编码习惯，为了减轻后期测试的压力，
 *           还是需要写清楚每个文件的“描述”和“版本”。
 *           
 *           使用方法：新建一个Assets/Scripts/Editor/name.txt文件，
 *           该文件里面只写开发者的名字，同时该文件不需要传到Github上，
 *           会在.gitignore
 * 作者：项叶盛
 * 创建时间：10/29/2018 2:15:41 PM
 * 版本：v0.1
 */
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScriptInitializer : UnityEditor.AssetModificationProcessor
{
    private static string annotationFormat =
           "/*\r\n" +
           " * 描述：#DESCRIPTION#\r\n" +
           " * 作者：#AUTHOR#\r\n" +
           " * 创建时间：#CREATETIME#\r\n" +
           " * 版本：#VERSION#\r\n" +
           " */\r\n";
    private static string author = null;
    private static string authorFile = "Assets/02.Scripts/Editor/name.txt";
    public static void OnWillCreateAsset(string path)
    {
        path = path.Replace(".meta", "");
        IEnumerable<string> enumerable= File.ReadLines(path);
        IEnumerator<string> enumerator = enumerable.GetEnumerator();
        if (enumerator.MoveNext())
        {
            //如果注释已经存在
            if (enumerator.Current.Trim().StartsWith("/*"))
            {
                Debug.Log("注释已存在，不再继续生成。" + path);
                return;
            }
        }
        if (path.ToLower().EndsWith(".cs") || path.ToLower().EndsWith(".lua"))
        {
            if (author == null)
            {
                if (File.Exists(authorFile))
                    author = File.ReadAllText(authorFile);
                else
                    throw new FileNotFoundException("开发者姓名文件未找到。" 
                        + "请建立"+ authorFile + "，并写入姓名。请删去重新建立。"
                        , authorFile);
            }
            string content = annotationFormat.Clone() as string;
            content = content.Replace("#DESCRIPTION#", "");
            content = content.Replace("#AUTHOR#", author);
            content = content.Replace("#CREATETIME#", 
                System.DateTime.Now.ToString());
            content = content.Replace("#VERSION#", "v0.1");
            content += File.ReadAllText(path);
            File.WriteAllText(path, content);
        }
    }
}
