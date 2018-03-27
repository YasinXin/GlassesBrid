using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
public class GameManager : Manager
{
    //当程序唤醒时
    void Awake()
    {
        //修改当前的FPS
        Application.targetFrameRate = AppConst.GameFrameRate;
        Screen.orientation = ScreenOrientation.LandscapeRight;
        // CheckExtractResource();
        OnInitOK();
    }


    /// <summary>
    /// 释放资源
    /// </summary>
    void CheckExtractResource()
    {
        bool isExists = Util.DirectoryExistence(Util.DataPath) && File.Exists(Util.DataPath + "Model/files.txt");
        if (isExists)
        {
            OnInitOK();
            return;   //文件已经解压过了，自己可添加检查文件列表逻辑
        }
        StartCoroutine(OnExtractResource());    //启动释放协成 
    }

    IEnumerator OnExtractResource()
    {
        string resPath = Util.AppContentPath(); //游戏包资源目录
        string infile = resPath + "files.txt"; //安装包资源路径

        Util.LogWarning("本地资源文件夹存在，开始复制文件");
        string dataPath = Util.DataPath;  //数据目录
        if (Directory.Exists(dataPath))
            Directory.Delete(dataPath, true);
        Directory.CreateDirectory(dataPath);
        if (Directory.Exists(dataPath+ "Model"))
            Directory.Delete(dataPath + "Model", true);
        Directory.CreateDirectory(dataPath + "Model");

        string outfile = dataPath + "Model/files.txt";
        if (File.Exists(outfile))
            File.Delete(outfile);

        Util.Log(infile);
        Util.Log(outfile);
        if (Application.platform == RuntimePlatform.Android)
        {
            WWW www = new WWW(infile);
            yield return www;

            if (www.isDone)
            {
                File.WriteAllBytes(outfile, www.bytes);
            }
            yield return 0;
        }
        else
            File.Copy(infile, outfile, true);
        yield return new WaitForEndOfFrame();

        //释放所有文件到数据目录
        string[] files = File.ReadAllLines(outfile);
        foreach (var file in files)
        {
            string[] fs = file.Split('|');
            infile = resPath + fs[0];  //
            outfile = dataPath+ "Model/" + fs[0];

            string dir = Path.GetDirectoryName(outfile);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (Application.platform == RuntimePlatform.Android)
            {
                WWW www = new WWW(infile);
                yield return www;

                if (www.isDone)
                {
                    File.WriteAllBytes(outfile, www.bytes);
                }
                yield return 0;
            }
            else {
                if (File.Exists(outfile))
                {
                    File.Delete(outfile);
                }
                File.Copy(infile, outfile, true);
            }
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.1f);

        OnInitOK();
    }

    /// <summary>
    /// 开始初始化
    /// </summary>
    void OnInitOK()
    {
        //Util.ClearMemory();
        //Time.timeScale = 1.0f;
        PlatformMgr.OnAwakeUp();
        
        //初始化摄像头与渲染面板
        WebCamMgr.OnAwakeUp();
        
        UISceneMgr.OnAwakeUp ();
        
        //消息转发中心与读取表情资源
        ReceiveMgr.OnAwakeUp();
        //材质管理初始化
        MaterialsMgr.OnAwakeUp();
        ////录屏管理
        RecorderMgr.OnAwakeUp();
        ////音频初始化
        AudioMgr.OnAwakeUp();
        ////滤镜
        //FilteMgr.OnAwakeUp();
        //GameObject.Find("Text").GetComponent<Text>().text = Screen.width+"宽";
        //GameObject.Find("Text1").GetComponent<Text>().text = Screen.height + "高";
    }

    //消息接收
    public void message(string msg)
    {
        //接受移动端消息
        Util.Log("log.message msg = " + msg);
        ReceiveMgr.SendMessages(msg);
    }

    /// <summary>
    /// 退出应用时清除ShareRec录屏缓存
    /// </summary>
    void OnApplicationQuit()
    {
#if !UNITY_EDITOR
#if UNITY_ANDROID
        RecorderMgr.ClearShareRECCache();
#endif
#endif
    }
}