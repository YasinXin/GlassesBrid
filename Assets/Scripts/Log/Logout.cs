﻿using System.IO;
using UnityEngine;

public class Logout
{
    public static void Log(string path, string Content)
    {
        StreamWriter sw = new StreamWriter(path + "\\Log.txt", true);
        string fileTitle = "日志文件创建的时间:" + System.DateTime.Now.ToString();
        sw.WriteLine(fileTitle);
        //开始写入
        sw.WriteLine(Content);
        //清空缓冲区
        sw.Flush();
        //关闭流
        sw.Close();
    }
    public static void Log(string Content)
    {
#if UNITY_EDITOR
        string path = Application.dataPath;
#else
        string path = Application.persistentDataPath;
#endif
        StreamWriter sw = new StreamWriter(path + "\\Log_"  + System.DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt", true);
        string fileTitle = "日志文件创建的时间:" + System.DateTime.Now.ToString();
        sw.WriteLine(fileTitle);
        //开始写入
        sw.WriteLine(Content);
        //清空缓冲区
        sw.Flush();
        //关闭流
        sw.Close();
    }

}
