using UnityEngine;
using System.Collections;
using System.IO;
using System;
#if UNITY_ANDROID
using cn.sharerec;
#endif
/// <summary>
/// 录屏管理 只集成了ShareREC android部分
/// </summary>
public class RecorderManager : Manager
{
    public static string photoPath = String.Empty;
    public static bool isCanRead = false;
    bool IsAvailable = false;
#if UNITY_ANDROID
    ShareREC rec;
#endif

    public void OnAwakeUp()
    {
#if !UNITY_EDITOR
#if UNITY_ANDROID
        rec = GameObject.Find("Main Camera").GetComponent<ShareREC>();
       // 判断目录是否存在,不存在则会创建目录  
        if (!Directory.Exists((AppConst.AndroidPath + "/Video")))
        {
            Directory.CreateDirectory(AppConst.AndroidPath + "/Video");
        }
        rec.OnAwakeUp(AppConst.AndroidPath + "/Video");
        IsAvailable = ShareREC.IsAvailable();
        ShareREC.OnRecorderStoppedHandler += StoppedHandler;
#endif
#endif
    }

    /// <summary>
    /// 录屏类型
    /// </summary>
    /// <param name="type"></param>
    public void RecorderType(string type)
    {
        switch (type)
        {
            case "StartRecorder":
                UISceneMgr.StartRecorder();
                StartRecorder();
                break;
            case "StopRecorder":
                UISceneMgr.StopRecorder();
                StopRecorder();
                break;
        }
    }

    /// <summary>
    /// 启动录制
    /// </summary>
    void StartRecorder()
    {
#if UNITY_ANDROID
        Util.Log("log RecorderManager Start Recorder!");
        // 启动录制
        ShareREC.StartRecorder();
#endif
    }

    /// <summary>
    /// 停止录制
    /// </summary>
    void StopRecorder()
    {
#if UNITY_ANDROID
        Util.Log("log RecorderManager Stop Recorder!");
        // 停止录制
        ShareREC.StopRecorder();
#endif
    }

    /// <summary>
    /// 录屏结束后回调 获取视频的路径.
    /// </summary>
    void StoppedHandler()
    {
#if UNITY_ANDROID
        Util.Log("log RecorderManager StoppedHandler");
        WebCamMgr.OnClosePrompt();
        string path = "";
        long[] Videos = ShareREC.ListLocalVideos();
        if (Videos != null)
        {
            if (Videos.Length > 0)
            {
                int Length = Videos.Length;
                long id = Videos[Length - 1];
                path = ShareREC.GetLocalVideoPath(id);
            }
        }
        Util.Log("log RecorderManager Video Path===" + path);
        PlatformMgr.OnRecorderVideoPath(path);
#endif
    }

    /// <summary>
    /// 退出应用清除sharerec缓存
    /// </summary>
    public void ClearShareRECCache()
    {
#if UNITY_ANDROID
        ShareREC.ClearCache();
        long[] Videos = ShareREC.ListLocalVideos();
        if (Videos != null)
        {
            if (Videos.Length > 0)
            {
                int Length = Videos.Length;
                long id = Videos[Length - 1];
                ShareREC.DeleteLocalVideo(id);
            }
        }
#endif
    }

    /// <summary>
    /// 设置拍照图片标志位
    /// </summary>
    /// <param name="bol">If set to <c>true = 拍照</c> bol.</param>
    public void SetPhotoFlag()
    {
        Rect rect = new Rect(1.0f, 1.0f, 1.0f, 1.0f);
        switch (WebCamMgr.resolutionType)
        {
            case ResolutionType.All:
                rect = new Rect(Screen.width * 0.0f, Screen.height * 0.0f, Screen.width * 1.0f, Screen.height * 1.0f);
                break;
            case ResolutionType.Threefourths:
                rect = new Rect(Screen.width * 0.0f, Screen.height * 0.25f, Screen.width * 1.0f, Screen.height * 0.75f);
                break;
            case ResolutionType.Oneone:
                rect = new Rect(Screen.width * 0.0f, Screen.height * 0.3432971f, Screen.width * 1.0f, Screen.width * 1.0f);
                break;
        }

        StartCoroutine(CaptureScreenshot(rect));
        //StartCoroutine(CaptureScreenshotByCamera(WebCamMgr.m_webCamera.WebCameraTex.GetPixels32()));
    }

    /// <summary>
    /// Captures the screenshot2.
    /// </summary>
    /// <returns>The screenshot2.</returns>
    /// <param name="rect">Rect.截图的区域，左下角为o点</param>
    IEnumerator CaptureScreenshotByCamera(Color32[] colors)
    {
        photoPath = Application.persistentDataPath;

        WebCamMgr.OnClosePrompt();
        yield return new WaitForEndOfFrame();
        // 先创建一个的空纹理，大小可根据实现需要来设置
        Texture2D screenShot = new Texture2D((int)WebCamMgr.m_webCamera.m_webCamWidthHeight.x, (int)WebCamMgr.m_webCamera.m_webCamWidthHeight.y, TextureFormat.RGBA32, false);
        screenShot.SetPixels32(colors);
        
        // 读取屏幕像素信息并存储为纹理数据，
        //screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();
        photoPath = Application.persistentDataPath + "/" + Util.GetCurrTime() + ".png";
        Util.SavePngTexturet(screenShot, photoPath);
        FileInfo t = new FileInfo(photoPath);
        bool isTakePhoto = false;
        if (!t.Exists)
        {
            Util.Log("log.图片不存在=========>filePath=" + photoPath);
            isTakePhoto = false;
            PlatformMgr.OnTakeAPicture(isTakePhoto, photoPath);
        }
        else {
            Util.Log("log.截屏了一张图片=========>filePath=" + photoPath);
            isTakePhoto = true;
            PlatformMgr.OnTakeAPicture(isTakePhoto, photoPath);
            isCanRead = true;
        }
    }


    /// <summary>
    /// Captures the screenshot2.
    /// </summary>
    /// <returns>The screenshot2.</returns>
    /// <param name="rect">Rect.截图的区域，左下角为o点</param>
    IEnumerator CaptureScreenshot(Rect rect)
    {
        photoPath = Application.persistentDataPath;
#if UNITY_ANDROID
        //判断目录是否存在,不存在则会创建目录  
        photoPath = AppConst.AndroidPath + "/Photo";
        if (!Directory.Exists(photoPath))
        {
            Directory.CreateDirectory(photoPath);
        }
#endif
        WebCamMgr.OnClosePrompt();
        yield return new WaitForEndOfFrame();
        // 先创建一个的空纹理，大小可根据实现需要来设置
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        // 读取屏幕像素信息并存储为纹理数据，
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();
        photoPath = Application.persistentDataPath + "/" + Util.GetCurrTime() + ".png";
        Util.SavePngTexturet(screenShot, photoPath);
        FileInfo t = new FileInfo(photoPath);
        bool isTakePhoto = false;
        if (!t.Exists)
        {
            Util.Log("log.图片不存在=========>filePath=" + photoPath);
            isTakePhoto = false;
            PlatformMgr.OnTakeAPicture(isTakePhoto, photoPath);
        }
        else {
            Util.Log("log.截屏了一张图片=========>filePath=" + photoPath);
            isTakePhoto = true;
            PlatformMgr.OnTakeAPicture(isTakePhoto, photoPath);
        }
    }
}