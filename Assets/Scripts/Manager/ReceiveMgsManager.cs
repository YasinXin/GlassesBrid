using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 资源类型 
/// </summary>
public enum AssetType
{
    None = -1,  //空
    Spine2D,    //2D素材
    Model3D     //3D素材
}

public class ReceiveMgsManager : Manager
{
    //spineJson文件名
    string m_spineJsonfileName = "2d_config.txt";
    //spineJson文件名
    string m_modelJsonfileName = "3d_config.txt";
    //消息函数名
    string functionname = "FunctionName";
    //bool值
    string bol = "Bool";
    //录屏
    string recorder = "Recorder";
    //路径
    string path = "Path";
    //上下文
    string context = "Context";
    //数值
    string value = "Value";
	//客户端发送消息到列表
	static List<string> pathList = new List<string> ();
	//判断是否可以进行下一个消息
	static bool bolInitEnd = true;
	//2d与3d消息类型判断
	public AssetType assetType;
	//2d类
	SpineInit p;
	//3d类
	ModelInit m;

    string Type;  //素材类型（2D，3D，photoFrame）
    public static int SplitScreen;  //是否分屏，1是分屏，0是全屏
    public static string Localtion = "SpineType";  //挂载位置-- 中间：SpineType、 左屏：Left、 右屏：Right
    public static int ClearOption = 0;		//ClearOption  0：挂载  1：清除

    public static bool showLeft;
    public static bool showRight;

    public TestUI testUI;
    public void OnAwakeUp()
    {
        p = new SpineInit();
        m = new ModelInit();
        assetType = AssetType.None;

        testUI = GameObject.Find("UIRoot/3DUIRoot/TouchOrMouseCanvas/TestUI").GetComponent<TestUI>();
    }
    
    public void SendMessages(string parms)
    {
        Dictionary<string, string> paraDic = new Dictionary<string, string>();
        string[] parmList = parms.Split('|');
        foreach (string parm in parmList)
        {
            string[] keyValueList = parm.Split('=');
            string key = keyValueList[0];
            string value = keyValueList[1];
            paraDic.Add(key, value);
        }
        string functionName = paraDic[functionname];
        switch (functionName)
        {
            case "InitUnityWebCam":               
                WebCamMgr.InitUnityWebCamera();
                break;

            case "SpinePath"://spine素材包路径
                Type = paraDic["Type"];

                if (Type.Equals("2D") || Type.Equals("3D"))
                {            
                    SplitScreen = int.Parse(paraDic["SplitScreen"]);
                    if (SplitScreen == 0) //全屏
                    {
                        Localtion = "SpineType";
                    }
                    else  //分屏
                    {
                        Localtion = paraDic["Locale"];
                        if (paraDic["Locale"].Equals("left"))
                        {
                            Localtion = "Left";
                            showLeft = true;                         
                        }
                        else
                        {
                            Localtion = "Right";
                            showRight = true;
                        }
                    }
                    WebCamMgr.ResetValue(Type);
                    pathList.Add(paraDic[path]);
                    ClearOption = int.Parse(paraDic["ClearOption"]);
                    WebCamMgr.StopTrackFace(ClearOption);  //ClearOption  0：挂载  1：清除       
                }
                else
                {
                    //相框
                }

                break;
            case "ChangeScreenModel":             
                FilteMgr.isDoubleScreen =bool.Parse( paraDic["Model"]);
                p.DelectMaterial2DBean(FilteMgr.isDoubleScreen);  //清除2D素材
                m.DelectMaterial3DBean(FilteMgr.isDoubleScreen);  //清除3D素材
                FilteMgr.ChangeScreenModel(FilteMgr.isDoubleScreen);  //切换分屏显示
                WebCamMgr.ResetValue(true, "");
                WebCamMgr.StopTrackFace(1);  //关闭人脸检测
                if (!FilteMgr.isDoubleScreen)
                {
                    showLeft = false;
                    showRight = false;
                }

                #region 隐藏脸皮
                for (int i = 0; i < MaterialsMgr.faceList.Count; i++)
                {
                    MaterialsMgr.faceList[i].SetActive(false);
                }
                MaterialsMgr.OnSetActive("SpineType", "face", false);
                #endregion

                PlatformMgr.changeScreen(FilteMgr.isDoubleScreen);
  
                break;
            case "CameraFilter"://滤镜    
                FilteMgr.CameraFilter(paraDic[context]);
                //FilteMgr.FaceMorphFilter("EyeFace");
                //FilteMgr.OnEyeFaceFilterValue (0.9f, 0.9f);
               

                break;
            case "FaceMorphFilter"://滤镜磨皮
                FilteMgr.FaceMorphFilter(paraDic[context]);
                Util.Log("调整美颜值 =>" + paraDic[context]);
                break;
            case "TakePicture"://拍照
                RecorderMgr.SetPhotoFlag();
                break;
            case "RecorderScene"://录屏
                RecorderMgr.RecorderType(paraDic[recorder]);
                break;
            case "PhotoSwitch"://摄像头前后镜切换
                //UISceneMgr.OnSetActive(true);
                //WebCamMgr.m_webCamera.Bol_UpdateTexture = false;
                //WebCamMgr.PhotoSwitchCallBack();       
                break;
            case "AudioVolume"://音量调节
                AudioMgr.AudioVolume(float.Parse(paraDic[value]));
                break;
            case "FaceLevel"://美颜调节
                WebCamMgr.SetMaterialFaceLevel(float.Parse(paraDic[value]));                
                break;
            case "ClickTrigger"://点击触发交互
                WebCamMgr.ClickScreenCheckIOS(bool.Parse(paraDic[bol]));
                break;
            case "DebugLog"://Log开关
                Util.m_Log = bool.Parse(paraDic[bol]);
                break;
            case "TrackStateSwitch"://人脸检测开关
                WebCamMgr.trackStateSwitch = bool.Parse(paraDic[bol]);
                break;
            case "ChangeResolution"://屏幕分辨率切换
                //UISceneMgr.OnSetActive(true);
                //WebCamMgr.ChangeCanvasScaler(paraDic[context]);

                break;
            case "CameraPause"://相机暂停
                Util.Log("Camera:>>>>>CameraPause接收到相机暂停消息");
               //WebCamMgr.OnUnityWebCameraPause(bool.Parse(paraDic[bol]));
                break;
            case "TimePause":
                PlatformMgr.TimePause();
                break;
            case "SetBrightness"://调节亮度
                WebCamMgr.SetBrightness(float.Parse(paraDic[value]));
                //Util.Log("SetBrightness=> " + float.Parse(paraDic[value]));
                break;
            case "GetEvent":
                if (paraDic[value].Equals("ScreenState"))
                {
                    PlatformMgr.GetScreenState();
                }
                else if(paraDic[value].Equals("ModelSide"))
                {
                    PlatformMgr.GetModelSide();
                }
                else if (paraDic[value].Equals("UnityVersion"))
                {
                    PlatformMgr.GetUnityVersion(Main.unityVersion);
                }
      
                break;
            case "ShowFrameControl":
                testUI.SetUIPanelActive(bool.Parse(paraDic[bol]));
                break;
            default:
                Util.LogError("SendMessages==>Error parms=" + parms);
                break;
        }
    }

    void Update()
    {
        if (bolInitEnd && pathList != null && pathList.Count > 0) {
            string paths = pathList[pathList.Count - 1];
            ReadSpineFilePath (pathList [pathList.Count - 1]);
			pathList.Clear ();
		 }
    }

	string fileName = "";
	string filePath = "";
	string strjson = "";
    /// <summary>
    /// 读取路径
    /// </summary>
    /// <param name="path"></param>
    void ReadSpineFilePath(string resorcepath)
    {
        if (bolInitEnd)
        {
            bolInitEnd = false;
            Util.Log("ReadSpineFilePath====>Start");

          

            //控制进行2d人脸检测
            //WebCamMgr.m_tarck2dOr3d = false;

            //string path = resorcepath.Replace("file://", "");
            if (!string.IsNullOrEmpty(resorcepath.Replace("file://", "")))
            {
                Util.Log("PATH=> " + resorcepath.Replace("file://", "") + "    //    " + resorcepath.Replace("file://", "").IndexOf("2d_"));
                if (resorcepath.Replace("file://", "").IndexOf("2d_") >= 0)
                {
                    assetType = AssetType.Spine2D;
                    fileName = m_spineJsonfileName;
                }
                else
                {
                    assetType = AssetType.Model3D;
                    fileName = m_modelJsonfileName;
                }

                DifferencetType();  // 清除上一次素材

                filePath = resorcepath.Replace("file://", "") + "/" + fileName;
                Debug.Log("PATH=>    filePath:" + filePath);
                if (Util.FileIsExistence(filePath))
                {
                    strjson = Util.GetFileText(filePath);
                    Debug.Log("PATH=>   strjson：" + strjson + "    filePath: " + filePath);
                    if (strjson != "")
                    {
                        //Util.Log ("strjson===>" + filePath);
                        DifferencetType(resorcepath.Replace("file://", ""), strjson);
                    }
                    else
                    {
                        bolInitEnd = true;
                        //Util.Log ("11111111 ReadSpineFilePath json is null ====>End   bolInitEnd==>" + bolInitEnd);
                    }
                }
                else
                {
                    bolInitEnd = true;
                    //Util.Log ("22222222 ReadSpineFilePath path is not found ====>End  bolInitEnd==>" + bolInitEnd);
                }
            }
            else
            {
                DifferencetType();  // 清除上一次素材
                bolInitEnd = true;
                //Util.Log ("333333 ReadSpineFilePath path is null ====>End  bolInitEnd==>" + bolInitEnd);

            }
        }
    }

    /// <summary>
    /// 类型去区别，判断 2d类型与3d类型走不同逻辑
    /// </summary>
    /// <param name="path">Path.</param>
    /// <param name="str">String.</param>
    void DifferencetType (string path = "", string strjson = "")
    {
        Util.Log("DifferencetType==> " + assetType + "     " + path);
        switch (assetType)
        {                
            case AssetType.Spine2D:
                if (path == "")
                {
                    p.DelectSpine(path);
    
                }
                else {
                    p.LoadSpineMateria(path, strjson, (bol) =>
                    {
                        bolInitEnd = true;
                    });
                }

                break;

            case AssetType.Model3D:        
                if (string.IsNullOrEmpty(path))
                {
                    m.DelectModel(path);
                  
                }
                else
                {
                    m.LoadModelMateria(path, strjson, (bol) =>
                    {
                        bolInitEnd = true;
                    });
                }
                break;


            default:
                break;
        }
    }
}