/** 
 *Copyright(C) 2015 by #COMPANY# 
 *All rights reserved. 
 *FileName:     #SCRIPTFULLNAME# 
 *Author:       #AUTHOR# 
 *Version:      #VERSION# 
 *UnityVersion：#UNITYVERSION# 
 *Date:         #DATE# 
 *Description:    
 *History: 
*/
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Diagnostics;

public class WebCamera : MonoBehaviour
{
    private Texture2D nativeTexture = null;
    private Texture2D splitTexture = null;
    Int32 mTextureId;

    [HideInInspector]
    public int width = 64;
    [HideInInspector]
    public int height = 48;

    ///<summry>
    ///是否是前置摄像头 默认为true
    ///</summry>
    [HideInInspector]
    public bool Bol_FontCam = true;

    //背景视频状态
    private bool videostate = false;
    private bool bol_UpdateTexture = false;


    //前置摄像头
    WebCamTexture m_webcamTexFront;
	//后置摄像头
	WebCamTexture m_webcamTexBack;
	//前后摄像头
	int m_devID = 0;
	//摄像头名称
	string m_deviceName;
    // Use this for initialization
    public Vector2 m_webCamWidthHeight = new Vector2(1920, 1080);
    //分屏位置与大小
    public Rect leftRect = new Rect(-480, 0, 1920, 1080);
    public Rect rightRect = new Rect(480, 0, 1920, 1080);
    //获取分辨率于显示跟踪点时使用
    public static Vector2 m_Resolution = new Vector2 (640, 360);
	//相机旋转值
	public int m_angle = 0;
	//相机
	public float m_scaleY = 0.0f;
	//是否镜像
	public bool m_bolMirror = false;
	//用于前后镜切换中标记
	public int m_rotateIndex = 0;
	/// <summary>
	/// 渲染面板的对象
	/// </summary>
    public static RawImage m_RawImage;  //单屏面板 RawImage
    public static Transform m_splitPanel;  //双屏面板
    public RawImage leftRawImage;  //左屏幕
    public RawImage rightRawImage;  //右屏幕
    public Image leftImage;  //左屏遮罩
    public Image rightImage;  //右屏遮罩
    public static RawImage frameImage;  //相框

    /// <summary>
    /// 3D详情节点
    /// </summary>
    public static Transform _3DRoot;
    public static Transform GlassesRoot;

	/// <summary>
	/// 画布对象
	/// </summary>
	GameObject BackCanvas = null;
	/// <summary>
	/// 画布对象组件
	/// </summary>
	public CanvasScaler m_canvasScaler = null;
    Slider m_slider = null;
	/// <summary>
	/// 渲染面板的rectTransform
	/// </summary>
	RectTransform m_rectTransform = null;

    public bool Videostate
    {
        get
        {
            return videostate;
        }

        set
        {
            videostate = value;
        }
    }
    /// <summary>
    /// 是否更新画面进行人脸识别
    /// </summary>
    public bool Bol_UpdateTexture
    {
        get
        {
            return bol_UpdateTexture;
        }

        set
        {
            bol_UpdateTexture = value;
        }
    }

    public byte[] GetPixels(AndroidJavaObject ajo)
    {
        return ajo.Get<byte[]>("dataY");
        //if (TestUI.option)
        //{
        //    return ajo.Get<byte[]>("dataY");
        //}
        //else
        //{
        //    return null;
        //}
    }

    void LoadVideo(Texture2D t2d, byte[] bty)
    {        
        t2d.LoadRawTextureData(bty);
        t2d.Apply();
    }

    /// <summary>
    /// 前后镜切换
    /// </summary>
    public void CameraSwitch()
    {
        // Checks how many and which cameras are available on the device
        foreach (WebCamDevice device in WebCamTexture.devices)
        {
            if (m_deviceName != device.name)
            {
               // WebCameraTex.Stop();
                m_devID++;
                if (m_devID >= 2)
                    m_devID = 0;
                m_deviceName = device.name;

                Util.Log("m_devID" + m_devID);
                Util.Log("m_deviceName" + m_deviceName);

                //WebCameraTex.Play();

                // SetReaderImage();
                CameraChange();
                break;
            }
        }
    }

    public void CameraChange()
    {
       
        Bol_FontCam = !Bol_FontCam;
        //Videostate = false;
        //Bol_UpdateTexture = false;
        
    }
    public void CameraR()
    {
        Videostate = false;
        Bol_UpdateTexture = false;
        //ByDataYUVInitTexture(ajo);
    }

   
    /// <summary>
    /// 获取panel面板与mesh渲染
    /// </summary>
    /// <returns></returns>
    public void Get2DRootPanel ()
	{
#region 获取2DRoot下PanelImage
		BackCanvas = GameObject.Find ("BackCanvas");
		m_canvasScaler = BackCanvas.GetComponent<CanvasScaler> ();
		m_rectTransform = BackCanvas.GetComponent<RectTransform> ();

        //单屏面板
        m_RawImage = BackCanvas.transform.FindChild("RawImage").GetComponent<RawImage>();

        //分屏面板
        m_splitPanel = BackCanvas.transform.FindChild("splitPanel").transform;
        leftRawImage = m_splitPanel.GetChild(0).GetChild(0).GetComponent<RawImage>();
        rightRawImage = m_splitPanel.GetChild(1).GetChild(0).GetComponent<RawImage>();
        leftImage = m_splitPanel.GetChild(0).GetComponent<Image>();
        rightImage = m_splitPanel.GetChild(1).GetComponent<Image>();
        //Debug.Log(leftImage.rectTransform.position + "    " + rightRawImage.rectTransform.rect + "    " + m_RawImage.rectTransform.rect + "    " + m_RawImage.rectTransform.anchoredPosition);
        leftRect = new Rect(leftImage.rectTransform.anchoredPosition.x, leftImage.rectTransform.anchoredPosition.y, leftImage.rectTransform.rect.width, leftImage.rectTransform.rect.height);
        rightRect = new Rect(rightImage.rectTransform.anchoredPosition.x, rightImage.rectTransform.anchoredPosition.y, rightImage.rectTransform.rect.width, rightImage.rectTransform.rect.height);
        //Debug.Log(leftRect + "    " + rightRect);
        frameImage = m_splitPanel.GetChild(2).GetComponent<RawImage>();
        m_splitPanel.gameObject.SetActive(false);
    
        //详情节点
        _3DRoot = GameObject.Find("3DRoot").transform;
        GlassesRoot = _3DRoot.FindChild("GlassesRoot").transform;
        #endregion
    }

    /// <summary>
    /// 设置2DRoot下BackCanvas的CanvasScaler
    /// </summary>
    public void SetCanvasScaler (int value)
	{
#region 设置2DRoot下BackCanvas的CanvasScaler
		//根据屏幕宽高去设置ReferenceResolution
		// 并且设置ScreenMatchMode为WithdOrHeight，将Match设置为0,竖屏显示按宽度去缩放；
		m_canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
		m_canvasScaler.matchWidthOrHeight = 0;
		Vector2 m_screenSize = new Vector2 (m_webCamWidthHeight.x, m_webCamWidthHeight.y);
		m_canvasScaler.referenceResolution = m_screenSize;
#endregion
	}

    /// <summary>
    /// 设置亮度
    /// </summary>
    Color lightColor;
    public void SetScreenBrightness(float brightness)
    {
        lightColor = new Color(brightness, brightness, brightness, 1);
        m_RawImage.color = lightColor;
        leftRawImage.color = lightColor;
        rightRawImage.color = lightColor;
    }

    Int32 texPtrs = 0;
    public void UpdateTexture2D()
    {
        if (SendPlatformManager.currentActivity == null)
        {
            return;
        }
        try
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            texPtrs = SendPlatformManager.currentActivity.Call<Int32>("DrawFrame");
            //sw.Stop();
            //Util.Log(string.Format("total: {0} ms", sw.ElapsedMilliseconds));
            if (mTextureId != texPtrs)
            {
                mTextureId = texPtrs;
                nativeTexture = Texture2D.CreateExternalTexture((int)m_webCamWidthHeight.x, (int)m_webCamWidthHeight.y, TextureFormat.RGBA32, false, false, new System.IntPtr(mTextureId));
                m_RawImage.texture = nativeTexture;
                leftRawImage.texture = nativeTexture;
                rightRawImage.texture = nativeTexture;
            }

            GL.InvalidateState();
        }
        catch (Exception)
        {
            Util.Log("DrawFrame出错！");
        }
    }

    /// <summary>
    /// 设置渲染面板位置
    /// </summary>
    /// <param name="bol">true :表示为1280:720分辨率 ，false：表示640:480分辨率</param>
    public void ReSetRenderPanelPosition (bool bol, Action action)
	{
		action ();
	}
	
		/// <summary>
	/// Getrects the transform size delta.
	/// </summary>
	/// <returns>The transform size delta.</returns>
	public float GetRectTransformSizeDelta()
	{
		return m_rectTransform.sizeDelta.y;
	}

	/// <summary>
	/// 根据设备类型判断相机分辨率为1280*720  true 还是640*480 false
	/// </summary>
	public void SetWebCamDeviceSize (bool bol)
	{
        Util.Log("InitUnityWebCam=>  444 =>  " + bol);
        //若设备存在白名单中，则设置相机大小为1280*720
        if (bol)
        { //if (DeviceInfo.GetDeviceModel())
            m_webCamWidthHeight = new Vector2(1920, 1080);
            //识别图为360*640分辨率大小下
            m_Resolution = new Vector2(640,360);//(640, 360);
        }
        else {
            m_webCamWidthHeight = new Vector2(640, 480);
            //识别图为480*640分辨率大小下
            m_Resolution = new Vector2(640, 480);
        }
    }
}