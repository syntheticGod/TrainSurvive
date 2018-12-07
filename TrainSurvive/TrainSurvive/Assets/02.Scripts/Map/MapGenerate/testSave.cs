/*
 * 描述：
 * 作者：王安鑫
 * 创建时间：2018/11/4 10:44:25
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;

public class FileOperator {
    //设置
    const string MAP_GENERATE = "mapGenerate";
    //文本中每行的内容
    ArrayList infoall;

    void Start() {
        Debug.Log("当前文件路径:" + Application.persistentDataPath);
        //删除文件
        DeleteFile(Application.persistentDataPath, "FileName.txt");

        //创建文件，共写入3次数据
        CreateFile(Application.persistentDataPath, "FileName.txt", "dingxiaowei");
        CreateFile(Application.persistentDataPath, "FileName.txt", "丁小未");
        //CreateFile(Application.persistentDataPath ,"Filename.assetbundle","丁小未");

        //得到文本中每一行的内容
        infoall = LoadFile(Application.persistentDataPath, "FileName.txt");

    }

    void CreateModelFile(string path, string name, byte[] info, int length) {
        //文件流信息
        //StreamWriter sw;
        Stream sw;
        FileInfo t = new FileInfo(path + "//" + name);
        if (!t.Exists) {
            //如果此文件不存在则创建
            sw = t.Create();
        } else {
            //如果此文件存在则打开
            //sw = t.Append();
            return;
        }
        //以行的形式写入信息
        //sw.WriteLine(info);
        sw.Write(info, 0, length);
        //关闭流
        sw.Close();
        //销毁流
        sw.Dispose();
    }

    /**
    * path：文件创建目录
    * name：文件的名称
    *  info：写入的内容
    */
    void CreateFile(string path, string name, string info) {
        //文件流信息
        StreamWriter sw;
        FileInfo t = new FileInfo(path + "//" + name);
        if (!t.Exists) {
            //如果此文件不存在则创建
            sw = t.CreateText();
        } else {
            //如果此文件存在则打开
            sw = t.AppendText();
        }
        //以行的形式写入信息
        sw.WriteLine(info);
        //关闭流
        sw.Close();
        //销毁流
        sw.Dispose();
    }


    /**
     * 读取文本文件
     * path：读取文件的路径
     * name：读取文件的名称
     */
    ArrayList LoadFile(string path, string name) {
        //使用流的形式读取
        StreamReader sr = null;
        try {
            sr = File.OpenText(path + "//" + name);
        }
        catch (Exception e) {
            //路径与名称未找到文件则直接返回空
            return null;
        }
        string line;
        ArrayList arrlist = new ArrayList();
        while ((line = sr.ReadLine()) != null) {
            //一行一行的读取
            //将每一行的内容存入数组链表容器中
            arrlist.Add(line);
        }
        //关闭流
        sr.Close();
        //销毁流
        sr.Dispose();
        //将数组链表容器返回
        return arrlist;
    }

    /**
     * path：删除文件的路径
     * name：删除文件的名称
     */
    void DeleteFile(string path, string name) {
        File.Delete(path + "//" + name);
    }
}
