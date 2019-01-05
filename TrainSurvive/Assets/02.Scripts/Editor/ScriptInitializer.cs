/*
 * 描述：创建文件时，给每个文件添加注释。
 *           这是为了养成良好的编码习惯，为了减轻后期测试的压力，
 *           还是需要写清楚每个文件的“描述”和“版本”。
 *           
 *           使用方法：新建一个Assets/Scripts/Editor/name.txt文件，文件编码格式为UTF-8
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
        if (path.ToLower().EndsWith(".cs") || path.ToLower().EndsWith(".lua"))
        {
            string text = File.ReadAllText(path);
            string content = "";
            if (text.Trim().StartsWith("/*"))
            {
                Debug.Log("该文件已经存在注释。" + path);
            }
            else
            {
                if (author == null)
                {
                    if (File.Exists(authorFile))
                        author = File.ReadAllText(authorFile);
                    else
                    {
                        author = "NONE";
                        Debug.LogError("开发者姓名文件未找到。"
                            + "请建立" + authorFile + "，并写入姓名（文件编码格式为UTF-8）");
                    }
                }
                content = annotationFormat.Clone() as string;
                content = content.Replace("#DESCRIPTION#", "");
                content = content.Replace("#AUTHOR#", author);
                content = content.Replace("#CREATETIME#",
                    System.DateTime.Now.ToString());
                content = content.Replace("#VERSION#", "v0.7");
            }
            content += text;
            File.WriteAllText(path, content);
        }
    }
}
