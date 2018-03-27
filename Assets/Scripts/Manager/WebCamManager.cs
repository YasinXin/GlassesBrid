/** 
 *Copyright(C) 2015 by #COMPANY# 
 *All rights reserved. 
 *FileName:     #SCRIPTFULLNAME# 
 *Author:       #AUTHOR# 
 *Version:      #VERSION# 
 *UnityVersion??UNITYVERSION# 
 *Date:         #DATE# 
 *Description:    
 *History: 
*/
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using UnityEngine.UI;
using System;
/// <summary>
/// 分辨率类型
/// </summary>
public enum ResolutionType
{
    All = 1,
    //全屏 9:16
    Threefourths = 2,
    //3:4
    Oneone = 3
    //1:1
}

public class WebCamManager : Manager
{
	/// <summary>
	/// 张嘴数值
	/// </summary>
    public static Dictionary<int, float> u_mouthopenparamDic = new Dictionary<int, float>();
    //定时数据
    const float _TimeToDisable = 3.0f;

    #region 结构体对象

    //[HideInInspector]//设置公有对象属性面板不显示
    //public TRACK2DRET track2dret;
    //[HideInInspector]//设置公有对象属性面板不显示
    //public TRACK3DRET track3dret;

    [HideInInspector]//设置公有对象属性面板不显示
    public TRACK2DRET[] track2dretResult;
	[HideInInspector]
	public TRACK3DRET[] track3dretResult;
    //当前检测人脸数值
	[HideInInspector]
    public int faceNum = 1;
    //前一个检测到人脸数
    int beforeFaceNum = 0;
    #endregion

    #region 类对象

    [HideInInspector]//设置公有对象属性面板不显示
    public WebCamera m_webCamera;
    TrackTool m_trackTool;
    SpineLogic m_spineLogic;
    ModelLogic m_modelLogic;

	[HideInInspector]//人脸检测开关。为IOS提供的接口。在人脸检测启动后，在不关闭检测的情况下，手动控制是否需要检测;
	public bool trackStateSwitch = true;
	//用于手动控制人脸检测的布尔值
	[HideInInspector]//设置公有对象属性面板不显示
	public bool u_bolTarck = false;
	//检测状态控制
	public bool m_tarckState = false;
	//默认人脸检测结果
	public bool m_refbolTarck = false;
	/// <summary>
	/// 判断当前基础方法    false:2d   true:3d
	/// </summary>
	[HideInInspector]//设置公有对象属性面板不显示
	public bool m_tarck2dOr3d = false;
    [HideInInspector]
    public bool m_exist2D = false;  //是否挂载2D素材
    [HideInInspector]
    public bool m_exist3D = false;  //是否挂载3D素材
	/// <summary>
	/// 开启检测
	/// </summary>
	[HideInInspector]//设置公有对象属性面板不显示
	public bool m_tarckInit_2d = false;
    public bool m_tarckInit_3d = false;

    #endregion

    /// <summary>
    /// 分辨率显示类??
    /// </summary>
    public ResolutionType resolutionType;
    /// <summary>
    /// 图片像素??
    /// </summary>
    int m_ImageResolution = 0;
    //摄像头是否初始化完成
    private bool m_camInited = false;
    //分辨率切换完??
    private bool m_camChange = false;
    /// <summary>
    /// 张嘴数??
    /// </summary>
    public static float u_mouthopenparam = 0.0f;
	/// 多人脸参数标记
	/// </summary>
	public static string[] multimode = { "1", "2" };
    /// <summary>
    /// 等待再次sdk人脸检测时??
    /// </summary>
    private float m_waitForSeconds = 0.03f;
    //0.075f
    /// <summary>
    /// 判断图标状??
    /// </summary>
    bool bol_TarckIocn = true;
    /// <summary>
    /// 切换分辨率标??true:全屏 720x1280,float:480x640
    /// </summary>
    bool m_CheckResolution = false;
    //安卓端传过来的每帧的时间
    long currentTime = 0;
    long preTime = 0;

    //是否第一次进入应用
    bool bol_IsFirst = true;
    //当前屏幕分辨率
    //1.全屏 2、4：3 3、1:1
    string resolutionTypeStr = "1";
    /// <summary>
    /// 前后摄像头标识 1为前置 2为后置
    /// </summary>
    int cameraID = 1;
    int camerid;
    bool bol_ChangeOK = true;
#if UNITY_EDITOR
    //测试
    public bool OpenMouthTest = false;
    //测试
    public bool UpheadTest = false;
    //测试
    public bool AroundheadTest1 = false;
    //测试
    public bool AroundheadTest2 = false;
#endif

	float gyroAngles=0.0f;

    /// <summary>
    /// 脚本初始化事??
    /// </summary>
    public void OnAwakeUp()
    {     
        resolutionType = ResolutionType.All;
        Constants.SetFixedPointData();
        //检测方法类
       m_trackTool = new TrackTool();
        //逻辑方法??
        m_spineLogic = new SpineLogic();
        m_modelLogic = new ModelLogic ();
        //相机的初始化  
        m_webCamera = new WebCamera();
        //协程定时调用检测方??
        StartCoroutine(DoSomething());
		//初始化3d矩阵顶点结构体
		m_modelLogic.InitStruct ();
        //初始化结构体数据
        if (faceNum <= 0)
        {
            faceNum = 1;
        }
		track2dretResult = new TRACK2DRET[faceNum];
		
		track3dretResult = new TRACK3DRET[faceNum];
		
		
        //for (int i = 0; i < faceNum; i++)
        //{
        //    track2dretResult = new TRACK2DRET[faceNum];
        //    track2dretResult[0] = m_trackTool.InitStruct();
        //}
        //m_modelLogic.InitStruct ();
        //track2dret = m_trackTool.InitStruct ();
        //track3dret = m_trackTool.InitTrackVector ();
        //获取2DRoot下PanelImage
        m_webCamera.Get2DRootPanel();
    }

    /// <summary>
    /// 开启图标提示定时器
    /// </summary>
    public void ActiveObject()
    {
       Invoke("OnClosePrompt", _TimeToDisable);
    }

    /// <summary>
    /// 协程
    /// </summary>
    /// <returns>The something.</returns>

    IEnumerator DoSomething()
    {
        
        while (true)
        {
            //Util.Log("DoSomething!");
#if UNITY_EDITOR
            SpineLogic.OpenMouthTest = OpenMouthTest;
            ModelLogic.OpenMouthTest = OpenMouthTest;
            SpineLogic.UpheadTest = UpheadTest;
            SpineLogic.AroundheadTest1 = AroundheadTest1;
            SpineLogic.AroundheadTest2 = AroundheadTest2;
#endif

                TrackEvent();

         
            //间隔时间??.1??
            yield return new WaitForSeconds(m_waitForSeconds);
        }
    }

    public void SetFrameMask(string url)
    {
        UnityEngine.Debug.Log("url==>  " + url);
        StartCoroutine(GetTexture(url));
    }

    IEnumerator GetTexture(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        UnityEngine.Debug.Log(www.texture.name);
        if(www.texture != null)
        {
            WebCamera.frameImage.texture = www.texture;
        }    
        yield return null;
    }

    byte[] colors = null;
    int length;

    ///设置亮度
    public void SetBrightness(float birghtness)
    {
        m_webCamera.SetScreenBrightness(birghtness);
    }

    /// <summary>
    /// 定时调用方法
    /// </summary>
    void TrackEvent()
    {
        if (!m_tarckState)
            return;

        m_tarckState = false;

        //人脸识别
        u_bolTarck = trackStateSwitch ? m_refbolTarck : false;

        if (trackStateSwitch) 
        {
            //float angle = Mathf.Atan2(Input.acceleration.x, Input.acceleration.y);
            //float degree = Mathf.Rad2Deg * angle;
            //gyroAngles = 0.5f - degree / 360.0f;
            //三石专用-横屏专用
            gyroAngles = 0.75f;

            m_webCamera.UpdateTexture2D(); //更新显示视频

                    TrackFace(); //人脸检测
        }
        else
        {
            m_spineLogic.OnLostFaceMethod();
        }
        if (!u_bolTarck) {
			m_spineLogic.OnLostFaceMethod ();
		}
        if (m_tarckInit_2d || m_tarckInit_3d)
        {
            if (bol_TarckIocn != u_bolTarck)
            {
                bol_TarckIocn = u_bolTarck;
                UISceneMgr.SetDistinguish(!u_bolTarck);
            }

            Util.Log("track3dretResult==> " + track3dretResult.Length + "     faceNum: " + faceNum + "    track2dretResult==> " + track2dretResult.Length + "   " + WebCamMgr.m_exist3D);
            //人脸检测是否存在的图标，需要实时去根据底层返回的bool值进行设??
            if (FilteMgr.isDoubleScreen)
            {
                if (WebCamMgr.m_exist2D)
                {
                    m_spineLogic.LogicSpine(1, beforeFaceNum, faceNum, u_bolTarck, track2dretResult);
                }

                if (WebCamMgr.m_exist3D)
                {
                    //分屏时限单人试戴
                    m_modelLogic.LogicModel(u_bolTarck, track3dretResult[0], 1);
                }
            }
            else
            {
                if (WebCamMgr.m_exist2D)
                {
                    m_spineLogic.LogicSpine(2, beforeFaceNum, faceNum, u_bolTarck, track2dretResult);
                }

                if (WebCamMgr.m_exist3D)
                {
                    //单屏时可以单人或双人试戴
                    if (track3dretResult.Length < 2)
                    {
                        m_modelLogic.LogicModel(u_bolTarck, track3dretResult[0], 1);
                    }
                    else
                    {
                        m_modelLogic.LogicModel(u_bolTarck, track3dretResult[0], 1); //第一个脸
                        m_modelLogic.LogicModel(u_bolTarck, track3dretResult[1], 2); //第二个脸
                    }
                }
            }
        }

		beforeFaceNum = faceNum;
		m_tarckState = true;
	}

    /// <summary>
    /// 是否追踪到人脸
    /// </summary>
    byte[] data = null;
    public void TrackFace()
    {
        if (m_webCamera.Bol_UpdateTexture)
        {
            if (bol_IsFirst)
            {
                bol_IsFirst = false;
            }
            else
            {
                m_refbolTarck = m_trackTool.GetTrack(ref faceNum, ref track2dretResult, ref track3dretResult);
                //m_refbolTarck = m_trackTool.GetTrack3d(m_webCamera.GetPixels(vd), (int)m_webCamera.m_webCamWidthHeight.x, (int)m_webCamera.m_webCamWidthHeight.y,
                //    m_webCamera.m_bolMirror, gyroAngles, ref faceNum, ref track2dretResult, ref track3dretResult);
                //if (m_tarck2dOr3d)
                //{
                //    m_refbolTarck = m_trackTool.GetTrack3d(m_webCamera.GetPixels(vd), (int)m_webCamera.m_webCamWidthHeight.x, (int)m_webCamera.m_webCamWidthHeight.y,
                //        m_webCamera.m_bolMirror, gyroAngles, ref faceNum, ref track2dretResult, ref track3dretResult);
                //}
                //else
                //{                  
                //    m_refbolTarck = m_trackTool.GetTrack2d(m_webCamera.GetPixels(vd), (int)m_webCamera.m_webCamWidthHeight.x, (int)m_webCamera.m_webCamWidthHeight.y,
                //        m_webCamera.m_bolMirror, gyroAngles, ref faceNum, ref track2dretResult);
                //}
            }
        }
    }

    public void StopTrackFace(int index)
    {
        if (index == 0)
        {
            m_webCamera.Bol_UpdateTexture = true;         
        }
        else if(index == 1)
        {
            m_webCamera.Bol_UpdateTexture = false;
            ReceiveMgsManager.Localtion = "";
        }
    }

    /// <summary>
    /// 所有动画完成后事件
    /// </summary>
    /// <param name="spineName"></param>
    public void SpienAnimEnd(string spineName)
    {
        m_spineLogic.SpineAnimEnd(spineName);
    }

    /// <summary>
    /// 前后相机切换
    /// </summary>
    public void PhotoSwitchCallBack()
    {
        m_tarckState = false;
        m_webCamera.CameraChange();
        m_webCamera.Bol_UpdateTexture = true;
        cameraID++;
        if (cameraID>=3)
        {
            cameraID = 1;
        }
        m_tarckState = true;
    }

	/// <summary>
	/// 重置属性??
	/// </summary>
	public void ResetValue (bool _bool, string type)
	{
        if(type == "2D")
        {
            m_tarckInit_2d = _bool;
            m_spineLogic.ResetValue();
        }
        else if(type == "3D")
        {
            m_tarckInit_3d = _bool;
            m_modelLogic.ResetValue();
        }
        else
        {
            m_tarckInit_2d = _bool;
            m_tarckInit_3d = _bool;
            m_spineLogic.ResetValue();
            m_modelLogic.ResetValue();
        }

        bol_TarckIocn = true;

		UISceneMgr.SetDistinguish (false);
        OnClosePrompt();
    }

    public void ResetValue(string type)
    {
        if (type == "2D")
        {
            m_spineLogic.SetOnceTrackValue(false);
        }
        else if (type == "3D")
        {
            m_modelLogic.SetOnceTrackValue(false);
        }
    }

    public bool isChangeValue = false;
    public float beauty = 0.0f;
    /// <summary>
    /// 设置美颜级别
    /// </summary>
    /// <param name="value"></param>
    public void SetMaterialFaceLevel(float value)
    {          
        isChangeValue = true;
        switch (value.ToString())
        {
            case "0":
                beauty = 0.0f;
                break;
            case "1":
                beauty = 0.3f;
                break;
            case "2":
                beauty = 0.5f;
                break;
            case "3":
                beauty = 0.65f;
                break;
            case "4":
                beauty = 0.8f;
                break;
            case "5":
                beauty = 1.0f;
                break;
        }
        Util.Log("调整美颜级别>>>>" + value);
    }

    /// <summary>
    /// 关闭提示
    /// </summary>
    /// <param name="value"></param>
    public void OnClosePrompt()
    {
        m_modelLogic.PromptOpenClose(false);
        m_spineLogic.PromptOpenClose(false);
        //if (m_tarck2dOr3d)
        //	m_modelLogic.PromptOpenClose (false);
        //else
        //	m_spineLogic.PromptOpenClose (false);
    }

    /// <summary>
    /// 点击触发操作IOS的消息接口
    /// </summary>
    /// <param name="_shotrOrLong">If set to <c>true</c> shotr or long.</param>
    public void ClickScreenCheckIOS(bool _shotrOrLong)
    {
        if (m_tarckInit_2d)
        {
            m_spineLogic.ClickScreenCheckIOS(_shotrOrLong);
        }
    }

    /// <summary>
    /// 获取中间??
    /// </summary>
    /// <returns>The middle.</returns>
    /// <param name="v1">V1.</param>
    public Vector2 GetMid(int v1)
    {
        Vector2 v = Vector2.zero;
        v.x = track2dretResult[0].allTrackPoints[v1].x;
        v.y = track2dretResult[0].allTrackPoints[v1].y;
        return GetPointValue(v);
    }
	
	/// <summary>
	/// 获取中间值
	/// </summary>
	/// <returns>The middle.</returns>
	/// <param name="v1">V1.</param>
	public Vector2 GetMid (int v1, int index)
	{
		Vector2 v = Vector2.zero;
		v.x = track2dretResult [index].allTrackPoints [v1].x;
		v.y = track2dretResult [index].allTrackPoints [v1].y;
		return GetPointValue (v);
	}

    /// <summary>
    /// 获取中间??
    /// </summary>
    /// <returns>The middle.</returns>
    /// <param name="v1">V1.</param>
    /// <param name="v2">V2.</param>
    //public Vector2 GetMid(int v1, int v2)
    //{
    //    Vector2 v = Vector2.zero;
    //    v.x = (track2dretResult[0].allTrackPoints[v1].x + track2dretResult[0].allTrackPoints[v2].x) * 0.5f;
    //    v.y = (track2dretResult[0].allTrackPoints[v1].y + track2dretResult[0].allTrackPoints[v2].y) * 0.5f;
    //    return GetPointValue(v);
    //}
	/// <summary>
	/// 获取中间值
	/// </summary>
	/// <returns>The middle.</returns>
	/// <param name="v1">V1.</param>
	/// <param name="v2">V2.</param>
	public Vector2 GetMid (int v1, int v2, int index)
	{
		Vector2 v = Vector2.zero;
		v.x = (track2dretResult [index].allTrackPoints [v1].x + track2dretResult [index].allTrackPoints [v2].x) * 0.5f;
		v.y = (track2dretResult [index].allTrackPoints [v1].y + track2dretResult [index].allTrackPoints [v2].y) * 0.5f;
		return GetPointValue (v);
	}
	
	/// <summary>
	/// GEs the t NOSEWIN g POIN.
	/// </summary>
	/// <returns>The t NOSEWIN g POIN.</returns>
	/// <param name="v1">V1.</param>
	/// <param name="v2">V2.</param>
	/// <param name="v3">V3.</param>
	/// <param name="fvalue">Fvalue.</param>
	public Vector2 GET_NOSEWING_POINT (int v1, int v2, int v3, float fvalue, int index)
	{
		Vector2 v = Vector2.zero;

		v.x = (track2dretResult [index].allTrackPoints [v1].x + track2dretResult [index].allTrackPoints [v2].x) / 2.0f; 
		v.y = (track2dretResult [index].allTrackPoints [v1].y + track2dretResult [index].allTrackPoints [v2].y) / 2.0f; 

		v.x = track2dretResult [index].allTrackPoints [v3].x * (1.0f + fvalue) - v.x * fvalue;
		v.y = track2dretResult [index].allTrackPoints [v3].y * (1.0f * fvalue) - v.y * fvalue;

		return GetPointValue (v);
	}


    /// <summary>
    /// 获取中间??
    /// </summary>
    /// <returns>The middle.</returns>
    /// <param name="v1">V1.</param>
    /// <param name="v2">V2.</param>
    /// <param name="v3">V3.</param>
    /// <param name="v4">V4.</param>
    //public Vector2 GetMid(int v1, int v2, int v3, int v4)
    //{
    //    Vector2 v = Vector2.zero;
    //    v.x = (track2dretResult[0].allTrackPoints[v1].x + track2dretResult[0].allTrackPoints[v2].x + track2dretResult[0].allTrackPoints[v3].x + track2dretResult[0].allTrackPoints[v4].x) * 0.25f;
    //    v.y = (track2dretResult[0].allTrackPoints[v1].y + track2dretResult[0].allTrackPoints[v2].y + track2dretResult[0].allTrackPoints[v3].y + track2dretResult[0].allTrackPoints[v4].y) * 0.25f;
    //    return GetPointValue(v);
    //}
	
	/// <summary>
	/// 获取中间值
	/// </summary>
	/// <returns>The middle.</returns>
	/// <param name="v1">V1.</param>
	/// <param name="v2">V2.</param>
	/// <param name="v3">V3.</param>
	/// <param name="v4">V4.</param>
	public Vector2 GetMid (int v1, int v2, int v3, int v4, int index)
	{
		Vector2 v = Vector2.zero;
		v.x = (track2dretResult [index].allTrackPoints [v1].x + track2dretResult [index].allTrackPoints [v2].x + track2dretResult [index].allTrackPoints [v3].x + track2dretResult [index].allTrackPoints [v4].x) * 0.25f;
		v.y = (track2dretResult [index].allTrackPoints [v1].y + track2dretResult [index].allTrackPoints [v2].y + track2dretResult [index].allTrackPoints [v3].y + track2dretResult [index].allTrackPoints [v4].y) * 0.25f;
		return GetPointValue (v);
	}

    /// <summary>
    /// 根据识别到的屏幕点计算量化后的x,y。量化后的x，y取值范围在0~1之间
    /// </summary>
    /// <returns>The point value.</returns>
    /// <param name="v2">V2.</param>
    Vector2 GetPointValue(Vector2 v2)
    {
        Vector2 v = Vector2.zero;
        v.x = v2.x / WebCamera.m_Resolution.y;
		switch (resolutionType) {
		case ResolutionType.All:
        v.y = 1 - (v2.y / WebCamera.m_Resolution.x);
			break;
		default:
			v.y = 1 - (v2.y / m_webCamera.GetRectTransformSizeDelta());
			break;
		}
        return v;
    }

    /// <summary>
    /// 相机暂停
    /// </summary>
    //public void OnUnityWebCameraPause(bool bolState)
    //{
    //    if (bolState)
    //    {
    //        m_webCamera.WebCameraTex.Pause();
    //    }
    //    else {
    //        m_webCamera.WebCameraTex.Play();
    //        m_webCamera.SetReaderImage();
    //    }
    //}

    /// <summary>
	/// 获取坐标??
	/// </summary>
	/// <returns>The position.</returns>
	/// <param name="arrPosition">Arr position.</param>
	public void getPosition(string[] arrPosition, ref string position)
    {
        if (arrPosition != null)
        {
            if (arrPosition.Length > 0)
            {
                if (arrPosition.Length == 3)
                {
                    switch (resolutionType)
                    {
                        case ResolutionType.All:
                            position = arrPosition[0];
                            break;
                        case ResolutionType.Threefourths:
                            position = arrPosition[1];
                            break;
                        case ResolutionType.Oneone:
                            position = arrPosition[2];
                            break;
                    }
                }
            }
        }
    }

    // <summary>
    /// 给IOS提供的控制调用UnityWebCamera的接??
    /// </summary>
    /// <param name="_isInit">If set to <c>true</c> is init.</param>
    public void InitUnityWebCamera()
    {
        Util.Log("InitUnityWebCam=>  222 =>  " + m_camInited);
        if (m_camInited)
        {        
            return;
        }
        else
        {
            InitUnityWebCamera("1", true);
        }
    }

    /// <summary>
    /// 切换分辨??
    /// </summary>
    /// <param name="bol">true 9:16 ??false 3:4</param>
    public void ChangeCanvasScaler(string level)
    {
        if (m_camChange)
        {
            return;
        }
        else {
            m_camChange = true;
            // StartCoroutine(InitUnityWebCamera(level, false));
            InitUnityWebCamera(level, false);
        }
    }


    /// <summary>
    /// 启动unity相机
    /// </summary>
    /// <returns>The unity web camera.</returns>
    /// <param name="level">Level.</param>
    /// <param name="isInit">If set to <c>true</c> is init.</param>
    void InitUnityWebCamera(string level, bool isInit)
    {
        //标记切换状??
        bool bolSwitchTrackState = false;
        bool temp = false;
        switch (level)
        {
            case "1":
                temp = true;
                resolutionType = ResolutionType.All;
                break;
            case "2":
                temp = false;
                resolutionType = ResolutionType.Threefourths;
                break;
            case "3":
                temp = false;
                resolutionType = ResolutionType.Oneone;
                break;
            default:
                temp = m_CheckResolution;
                break;
        }
       
        if (temp != m_CheckResolution)
        {
            m_CheckResolution = temp;
            if (!isInit)
            {
                //打开挡板
                UISceneMgr.OnSetActive(true);
                if (trackStateSwitch)
                {
                    bolSwitchTrackState = true;
                    trackStateSwitch = false;
                }
            }
            //设置相机宽与高
            m_webCamera.SetWebCamDeviceSize(m_CheckResolution);
            //计算像素??
            m_ImageResolution = (int)m_webCamera.m_webCamWidthHeight.x * (int)m_webCamera.m_webCamWidthHeight.y;
            //初始化颜色数组转字节数组
           // m_trackTool.SetColorByByteInit(m_ImageResolution);
            //切换分辨率后重新计算初始化数??
            MaterialsMgr.CheckIntoDataInfo();
            int value = 1;
            value = m_CheckResolution == true ? 1 : 0;
            //设置2DRoot下BackCanvas的CanvasScaler
            m_webCamera.SetCanvasScaler(value);

            //yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            //判断应用是否已获得摄像头权限
            //if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            //{
                if (!isInit)
                {
                    //停止相机
                    //m_webCamera.CameraStop();
                    m_webCamera.CameraR();
                }

                // m_webCamera.CameraPlay(() =>
#if !UNITY_EDITOR
                //m_webCamera.initTextureFirst(LoadAndroidJavaObject(), () =>
#endif
                 {

                m_webCamera.Bol_UpdateTexture = false;
#if !UNITY_EDITOR
						//人脸检测初始化检测方法类
						m_tarckState = m_trackTool.TrackerInit(m_CheckResolution);
                        Util.Log("m_tarckState==" + m_tarckState);
                     // m_tarckState=true;
#endif
                StartCoroutine(SetRenderPanelPosition(m_CheckResolution, isInit, bolSwitchTrackState));
                    

                  }
#if !UNITY_EDITOR
               // );
#endif
                //);
            //}
            //else {
            //    Util.Log("Camera 没有获取到权限");
            //}
        }
        else {
            Util.Log("Camera:>>>>>分辨率更改，去除档板" + isInit);
            //去除挡板
            PlatformMgr.RemovePanel();
            m_camChange = false;
        }
    }
   
    /// <summary>
    /// 重置渲染板位置与去除遮挡??
    /// </summary>
    /// <returns>The render panel position.</returns>
    /// <param name="bol">If set to <c>true</c> bol.</param>
    IEnumerator SetRenderPanelPosition(bool bol, bool isInit, bool bolSwitchTrackState)
    {
        yield return new WaitForSeconds(0.1f);  //0.1
       
        m_webCamera.ReSetRenderPanelPosition(bol, () =>
        {
            if (isInit)
            {
                //m_webCamera.Bol_UpdateTexture = true;
                //初始化完成消??
                PlatformMgr.PlatformInit();
                PlatformMgr.GetUnityVersion(Main.unityVersion);
                m_camInited = true;
            }
        });
    }
}