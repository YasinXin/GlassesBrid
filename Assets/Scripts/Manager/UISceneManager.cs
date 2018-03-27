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

public class UISceneManager : Manager
{
    ///判断当前是否是录屏状态
    bool m_Recorder = false;
    //切换分辨率时的黑屏档板
	GameObject m_ScreenPanel=null;
    //4：3录屏时的黑底档板
    GameObject m_ScreenPanelBG = null;

	/// <summary>
	/// 初始化
	/// </summary>
	public void OnAwakeUp()
	{
		m_ScreenPanel = GameObject.Find ("ScreenPanel");
        m_ScreenPanelBG = GameObject.Find("ScreenPanelBG");
        OnSetActive (false);
        OnSetActiveBG(false);

    }
    public void OnSetActiveBG(bool bol)
    {
        if (m_ScreenPanelBG != null)
        {
            m_ScreenPanelBG.SetActive(bol);
        }
    }
	/// <summary>
	/// 设置遮挡板显示隐藏
	/// </summary>
	/// <param name="bol">If set to <c>true</c> bol.</param>
	public void OnSetActive(bool bol)
	{
		if (m_ScreenPanel != null) {
			m_ScreenPanel.SetActive (bol);
		}
	}
 
    /// <summary>
    /// 录屏开始时开启水印
    /// </summary>
    public void StartRecorder()
    {
        m_Recorder = true;
		//人脸检测图标显示隐藏
		PlatformMgr.OnShowHideIcon (Constants.TRACK_ICON,false);
		// 关闭提示
		WebCamMgr.OnClosePrompt ();
        UISceneMgr.OnSetActiveBG(true);
    }

    /// <summary>
    /// 录屏结束时开启水印
    /// </summary>
    public void StopRecorder()
    {
        UISceneMgr.OnSetActiveBG(false);
        m_Recorder = false;
    }

    /// <summary>
    /// 设置提示
    /// </summary>
    /// <param name="bol"></param>
    public void SetDistinguish(bool bol)
    {
		if (!m_Recorder) {
			PlatformMgr.OnShowHideIcon (Constants.TRACK_ICON,bol);
		}
    }
}