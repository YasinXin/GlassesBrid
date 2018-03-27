/***********************************************
   页面名称:				SendToPlatform
   作用/描述:				移动端信息发送
   自定义方法说明:
   原作者:				    楷楷
   原作日期:			        
   修改者[1]:				
   修改日期[1]:			
   备注[1]:					
************************************************/
using UnityEngine;
using System.Runtime.InteropServices;

public class SendPlatformManager : Manager
{
#if UNITY_ANDROID
    AndroidJavaClass unityPlayer;
    public static AndroidJavaObject currentActivity;
#endif
    public void OnAwakeUp()
    {
#if !UNITY_EDITOR
#if UNITY_ANDROID
        unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AppConst.AndroidPath = getFilePath();
        Util.Log("PlatformMgr AndroidPath===>" + AppConst.AndroidPath);
#endif
#endif
    }

    /// <summary>
    /// 设置屏幕亮度
    /// </summary>
    //public void SetApplicationBrightnessTo(float Brightness)
    //{
    //    currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
    //        AndroidJavaObject Window = null, Attributes = null;
    //        Window = currentActivity.Call<AndroidJavaObject>("getWindow");
    //        Attributes = Window.Call<AndroidJavaObject>("getAttributes");
    //        Attributes.Set("screenBrightness", Brightness);
    //        Window.Call("setAttributes", Attributes);
    //    }));
    //}



    //Apps启动时unity向移动端发送启动完成信息。
    public void PlatformInit()
    {
        Debug.Log("log.StartApp");
#if !UNITY_EDITOR
#if UNITY_ANDROID
        currentActivity.Call("Init");
#endif
#endif
    }

    public void TimePause()
    {
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 去除挡板
    /// </summary>
    public void RemovePanel()
    {
        Util.Log("log.RemovePanel==>");
#if !UNITY_EDITOR
#if UNITY_ANDROID
		    currentActivity.Call("RemovePanel"); 
#endif
#endif
    }

    /// <summary>
    /// 图标显示隐藏 
    /// </summary>
    /// <param name="typeName">图标到类型</param>
    /// <param name="bol">If set to <c>true</c> bol.</param>
    public void OnShowHideIcon(string typeName, bool bol)
    {
        Util.Log("log.OnShowHideIcon===>typeName=" + typeName + " bol=" + bol);
#if UNITY_EDITOR
        MaterialsMgr.OnSetActive(typeName, bol);
#endif
#if !UNITY_EDITOR
#if UNITY_ANDROID
             currentActivity.Call("OnShowHideIcon",typeName,bol);
#endif
#endif
    }

    /// <summary>
    /// 拍照的接口，将生成的路径返回给端
    /// </summary>
    /// <param name="path">Path.</param>
    public void OnTakeAPicture(bool bol, string path)
    {
#if !UNITY_EDITOR
#if UNITY_ANDROID
              currentActivity.Call("OnTakePicture", bol, path);
#endif
#endif
    }

    public void changeScreen(bool bol)
    {
        currentActivity.Call("changeScreen", bol);
    }

    /// <summary>
    /// 录屏完成获取视频路径
    /// </summary>
    /// <param name="path">Path.</param>
    public void OnRecorderVideoPath(string path)
    {
        try
        {
#if !UNITY_EDITOR
#if UNITY_ANDROID
		             currentActivity.Call("OnRecorderVideoPath",path);
#endif
#endif
        }
        catch
        {
            Util.Log("currentActivity Call OnRecorderVideoPath error");
        }
    }

    /// <summary>
    ///获取拍照保存路径 
    /// </summary>
    /// <returns></returns>
    string getFilePath()
    {
        string path = Application.persistentDataPath;
#if !UNITY_EDITOR
#if UNITY_ANDROID
             path = currentActivity.Call<string>("getFilePath");
#endif
#endif
        return path;
    }

    /// <summary>
    ///返回脸型
    /// </summary>
    /// <returns></returns>  
    public void GetFaceShape(int index)
    {
#if !UNITY_EDITOR
#if UNITY_ANDROID
              //currentActivity.Call("GetFaceShape", index); //  index  0:aquare  1:round  2:slim
#endif
#endif

    }

    /// <summary>
    ///返回是否分屏
    /// </summary>
    /// <returns></returns>  
    public void GetScreenState()
    {
        int index = 1;
        if (FilteMgr.isDoubleScreen)
        {
            index = 2;
        }
        else
        {
            index = 1;
        }

#if !UNITY_EDITOR
#if UNITY_ANDROID
              currentActivity.Call("GetScreenState", index); //  index 1:单屏  2:双屏
#endif
#endif
    }

    ///返回Unity版本
    /// </summary>
    /// <returns></returns>  
    public void GetUnityVersion(string version)
    {
#if !UNITY_EDITOR
#if UNITY_ANDROID
              currentActivity.Call("GetUnityVersion", version); 
#endif
#endif
    }

    /// <summary>
    ///返回素材挂载在哪边
    /// </summary>
    /// <returns></returns>  
    public void GetModelSide()
    {
        int index = 1;
        if (ReceiveMgsManager.Localtion.Equals("Left"))
        {
            index = 0;
        }
        else if (ReceiveMgsManager.Localtion.Equals("Right"))
        {
            index = 1;
        }
        else
        {
            index = 2;
        }

#if !UNITY_EDITOR
#if UNITY_ANDROID
              currentActivity.Call("GetModelSide", index); //  index 0:左边  1:右边   2：单屏挂载
#endif
#endif
    }

    /// <summary>
    /// 返回保存好图像的调整
    /// </summary>
    public void SavedFrameData()
    {
#if !UNITY_EDITOR
#if UNITY_ANDROID
        currentActivity.Call("SavedFrameData");
#endif
#endif
    }
}