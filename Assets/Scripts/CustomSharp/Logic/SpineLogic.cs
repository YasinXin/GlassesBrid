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
using System.Collections.Generic;
using System;

public class SpineLogic : Manager
{
	//记录做张嘴触发动画素材 int 代表 检测到人脸到序号，0代表检测到到第一个人 ，List<string>代表其所做到动画
	Dictionary<int,List<string>> m_OpenMouthAnimTouchDic = new Dictionary<int, List<string>> ();
	//记录做张嘴触发动画素材
	//	List<string> m_OpenMouthAnimlist = new List<string> ();
	//记录做张嘴触发动画素材
	//	List<string> m_UpheadAnimlist = new List<string> ();
	//记录做张嘴触发动画素材
	//	List<string> m_Aroundheadlist = new List<string> ();
	//记录做点击屏幕触发动画素材
	List<string> m_ClickScreenlist = new List<string> ();
	//记录做按住屏幕触发动画素材
	List<string> m_HoldDownScreenlist = new List<string> ();
	//从未检测到人脸变为检测到人脸是触发
	bool m_bolbackground=false;
    //判断是否要重置素材默认属性
    bool m_bolOnceTrack = false;
    bool m_bolOnceTrack_left = false;
    bool m_bolOnceTrack_right = false;

    bool m_bolFirst = true;

	//一次提示
	//	bool m_OncePrompt = false;
	/**
   * 是否张嘴
   */
   	Dictionary<int,bool> m_bolOpenMouthDic = new Dictionary<int,bool> ();
	

	/**
	 * 是否抬头
	 */
	//	bool m_bolUphead = false;
	/**
	 * 是否摇头
	 */
	//	bool m_bolAroundhead = false;
	/**
	 * 是否向右摇头
	 */
	//	bool m_bolshakeright = false;
	/**
	 * 是否向左摇头
	 */
	//	bool m_bolshakeleft = false;
	//是否点击屏幕
	bool m_isClickScreen = false;
	//是否长按屏幕
	bool m_isHoldDownScreen = false;

	/// <summary>
	/// spine 素材名称
	/// </summary>
	string spineName = "";
	/// <summary>
	/// 音频素材名称
	/// </summary>
	private string audioName = "";
	#if UNITY_EDITOR
	//测试
	double dou = 0.0f;
	//测试
	public static bool OpenMouthTest = false;
	//测试
	public static bool UpheadTest = false;
	//测试
	public static bool AroundheadTest1 = false;
	//测试
	public static bool AroundheadTest2 = false;
	#endif

	void TestOpenMouthSetValue ()
	{
#if UNITY_EDITOR
		if (OpenMouthTest) {
			dou = 0.925f;
		} else {
			dou = 0.2f;
		}
#endif
	}

    //	void TestUpheadSetValue (ref TRACK2DRET track2dret)
    //	{
    //#if UNITY_EDITOR
    //		if (UpheadTest) {
    //			track2dret.rendering_paramsx = -0.5f;
    //		} else {
    //			track2dret.rendering_paramsx = 0.0f;
    //		}
    //#endif
    //	}
    //
    //	void TestAroundheadSetValue (ref TRACK2DRET track2dret)
    //	{
    //#if UNITY_EDITOR
    //		if (!AroundheadTest1 && !AroundheadTest2) {
    //			track2dret.rendering_paramsy = 0.0f;
    //		}
    //
    //		if (AroundheadTest1) {
    //			track2dret.rendering_paramsy = -0.2f;
    //		}
    //
    //		if (AroundheadTest2) {
    //			track2dret.rendering_paramsy = 0.2f;
    //		}
    //#endif
    //	}

    /// <summary>
    /// Spine逻辑操作
    /// </summary>
    /// <param name="trackResult"></param>
    /// <param name="track2dret"></param>
    //	public void LogicSpine (bool trackResult, TRACK2DRET track2dret)
    //	{
    //#if UNITY_EDITOR
    //		TestOpenMouthSetValue ();
    //		TestUpheadSetValue (ref track2dret);
    //		TestAroundheadSetValue (ref track2dret);
    //#endif
    //		SpineActive (trackResult);
    //		SpineTrackTrigger (trackResult, track2dret);
    //	}
    public void LogicSpine(int trackLimit, int beforeFaceNum,int nextFaceNum,bool trackResult, TRACK2DRET[] track2dret)
    {
#if UNITY_EDITOR
        TestOpenMouthSetValue();
        //TestUpheadSetValue(ref track2dret);
        //TestAroundheadSetValue(ref track2dret);
#endif
        SpineTrackSpinePoint(trackResult, nextFaceNum, track2dret);
        SpineActive(trackResult, beforeFaceNum, nextFaceNum, trackLimit);
        SpineTrackTrigger(trackResult, nextFaceNum, track2dret);
    }

    

    /// <summary>
    /// 重置属性值
    /// </summary>
    public void ResetValue ()
	{
		for(int i=0;i<m_bolOpenMouthDic.Count;i++)
		{
			if (m_bolOpenMouthDic.ContainsKey(i)) {
				m_bolOpenMouthDic [i] = false;
			}
		}

		m_isClickScreen = false;
		//是否长按屏幕
		m_isHoldDownScreen = false;
        SetOnceTrackValue(false);
		m_bolbackground = false;
	}

    bool bolVisible = false;
    float zOrder = 0.0f;
    string position = "";
    bool isExist = false;
    bool repeat = false;
    int multimode = 0;
    /// <summary>
    /// 一次检测方法，在人脸检测后需要触发一次的逻辑
    /// </summary>
    void OnTrackingFaceOnceMethod(int beforeFaceNum, int nextFaceNum, int trackLimit)
    {
        if (MaterialsMgr.material2DBeanList != null && MaterialsMgr.material2DBeanList.Count > 0)
        {
            var enumeartor = MaterialsMgr.material2DBeanList.GetEnumerator();
            while (enumeartor.MoveNext())
            {
                Material2DBean mater2DBean = enumeartor.Current.Value;
                OnTrackingFaceOnceMethod(mater2DBean, beforeFaceNum, nextFaceNum, enumeartor.Current.Key, trackLimit);
            }
        }
    }

    void OnTrackingFaceOnceMethod(Material2DBean mater2DBean, int beforeFaceNum, int nextFaceNum, string key, int trackLimit)
    {
        if(mater2DBean == null)
        {
            return;
        }

        if(mater2DBean.spines == null)
        {
            return;
        }

        //播放spine动画
            if (mater2DBean.spines.Count > 0)
            {
                Util.Log("mater2DBean.spines.Count ==> " + mater2DBean.spines.Count);
                for (int i = 0; i < mater2DBean.spines.Count; i++)
                {
                    //true 识别到人脸后显示
                    bolVisible = mater2DBean.spines[i].visible;
                    spineName = mater2DBean.spines[i].name;
                    zOrder = (float)mater2DBean.spines[i].zOrder;
                    position = mater2DBean.spines[i].position.ToString();
                    repeat = mater2DBean.spines[i].repeat;
                    multimode = int.Parse(mater2DBean.spines[i].multimode);
                    //if (multimode > trackLimit)
                    //{
                    //    multimode = trackLimit;
                    //}
                    Util.Log("multimode==> " + multimode);

                    //当前标记存在于上一个检测数内
                    if (multimode <= beforeFaceNum)
                    {
                        //当前标记不存在于下一个检测数内，在列表的加快速度，不在列表的隐藏
                        if (multimode > nextFaceNum)
                        {
                            if (m_OpenMouthAnimTouchDic.ContainsKey(multimode - 1))
                            {
                                if (m_OpenMouthAnimTouchDic[multimode - 1].Contains(spineName))
                                {
                                    AnimSpineMgr.SetAnimatorTimeScale(spineName, "1000");
                                    Util.Log("2DShow==> 11111111");
                                }
                                else {
                                    MaterialsMgr.OnSetActive(key, spineName, false);
                                    Util.Log("2DShow==> 22222222");
                                }
                            }
                            else if (m_ClickScreenlist.Count > 0)
                            {
                                if (m_ClickScreenlist.Contains(spineName))
                                {
                                    AnimSpineMgr.SetAnimatorTimeScale(spineName, "1000");
                                    Util.Log("2DShow==> 33333333");
                                }
                                else {
                                    MaterialsMgr.OnSetActive(key, spineName, false);
                                    Util.Log("2DShow==> 44444444");
                                }
                            }
                            else if (m_HoldDownScreenlist.Count > 0)
                            {
                                if (m_HoldDownScreenlist.Contains(spineName))
                                {
                                    AnimSpineMgr.SetAnimatorTimeScale(spineName, "1000");
                                    Util.Log("2DShow==> 555555555");
                                }
                                else {
                                    MaterialsMgr.OnSetActive(key, spineName, false);
                                    Util.Log("2DShow==> 66666666");
                                }
                            }
                            else {
                                MaterialsMgr.OnSetActive(key, spineName, false);
                                Util.Log("2DShow==> 777777777");
                            }
                        }
                        else {
                            if (m_OpenMouthAnimTouchDic.ContainsKey(multimode - 1))
                            {
                                if (!m_OpenMouthAnimTouchDic[multimode - 1].Contains(spineName))
                                {
                                    MaterialsMgr.OnSetActive(key, spineName, bolVisible);
                                    Util.Log("2DShow==> 88888888");
                                }
                            }
                            else if (m_ClickScreenlist.Count > 0)
                            {
                                if (!m_ClickScreenlist.Contains(spineName))
                                {
                                    MaterialsMgr.OnSetActive(key, spineName, bolVisible);
                                    Util.Log("2DShow==> 99999999");
                                }
                            }
                            else if (m_HoldDownScreenlist.Count > 0)
                            {
                                if (!m_HoldDownScreenlist.Contains(spineName))
                                {
                                    MaterialsMgr.OnSetActive(key, spineName, bolVisible);
                                    Util.Log("2DShow==> qqqqqqqqq");
                                }
                            }
                            else {
                                MaterialsMgr.OnSetActive(key, spineName, bolVisible);
                                Util.Log("2DShow==> wwwwwwwwwww");
                            }
                        }
                    }
                    if (multimode <= nextFaceNum)
                    {
                        if (m_OpenMouthAnimTouchDic.ContainsKey(multimode - 1))
                        {
                            if (!m_OpenMouthAnimTouchDic[multimode - 1].Contains(spineName))
                            {
                                MaterialsMgr.OnSetActive(key, spineName, bolVisible);
                                Util.Log("2DShow==> eeeeeeeee");
                            }
                        }
                        else if (m_ClickScreenlist.Count > 0)
                        {
                            if (!m_ClickScreenlist.Contains(spineName))
                            {
                                MaterialsMgr.OnSetActive(key, spineName, bolVisible);
                                Util.Log("2DShow==> rrrrrrrrrrr");
                            }
                        }
                        else if (m_HoldDownScreenlist.Count > 0)
                        {
                            if (!m_HoldDownScreenlist.Contains(spineName))
                            {
                                MaterialsMgr.OnSetActive(key, spineName, bolVisible);
                                Util.Log("2DShow==> tttttttt");
                            }
                        }
                        else {
                            MaterialsMgr.OnSetActive(key, spineName, bolVisible);
                            Util.Log("2DShow==> yyyyyyyyy");
                        }
                    }
                    if (bolVisible)
                    {
                        //素材循环播放动画
                        AnimSpineMgr.SetAnimator(spineName, repeat);
                    } 
                }            
        }
        if (!m_bolbackground)
        {
            //提示语
            PromptOpenClose(true);

            if (mater2DBean.audios != null)
            {
                if (mater2DBean.audios.Count > 0)
                {
                    for (int i = 0; i < mater2DBean.audios.Count; i++)
                    {
                        //如果触发为空，播放音效
                        if (mater2DBean.audios[i].trigger.ToString() == "")
                        {
                            if (mater2DBean.audios[i].name != "")
                            {
                                audioName = mater2DBean.audios[i].name.Split('.')[0].ToString();
                                AudioMgr.AudioPlay(audioName);
                            }
                        }
                    }
                }
            }

            // 处理背景音乐
            if (mater2DBean.backgroundMusic != null)
            {
                if (mater2DBean.backgroundMusic.name != "")
                {
                    //播放背景音乐
                    AudioMgr.AudioPlay(mater2DBean.backgroundMusic.name.Split('.')[0].ToString());
                }
            }
            m_bolbackground = true;
        }
    }

	//}
    /// <summary>
    /// 丢失人脸检测方法，在人脸丢失后调用
    /// </summary>
    public void OnLostFaceMethod()
    {
        if (MaterialsMgr.material2DBeanList != null && MaterialsMgr.material2DBeanList.Count > 0)
        {
            var enumeartor = MaterialsMgr.material2DBeanList.GetEnumerator();
            while (enumeartor.MoveNext())
            {
                Material2DBean mater2DBean = enumeartor.Current.Value;
                OnLostFaceMethod(mater2DBean, enumeartor.Current.Key);
            }
        }
    }

    public void OnLostFaceMethod(Material2DBean mater2DBean, string key)
    {
        if (mater2DBean != null)
        {
            if (mater2DBean.spines != null)
            {
                if (mater2DBean.spines.Count > 0)
                {
                    for (int i = 0; i < mater2DBean.spines.Count; i++)
                    {
                        spineName = mater2DBean.spines[i].name;
                        //spine 素材显示隐藏设置
                        if (mater2DBean.spines[i].trigger.Equals(""))
                        {
                            MaterialsMgr.OnSetActive(key, spineName, false);
                        }
                        else {
                            AnimSpineMgr.SetAnimatorTimeScale(spineName, "1000");
                        }
                    }
                    //在人脸识别丢失时，若当前有触发停止的动画，在动画回调后会显示。固再判断隐藏触发停止的动画
                    for (int i = 0; i < mater2DBean.spines.Count; i++)
                    {
						AnimSpineMgr.StopAnimator (spineName);
						MaterialsMgr.OnSetActive (key, spineName, false);
                        if (!mater2DBean.spines[i].trigger.Equals("") &&
                            mater2DBean.spines[i].trigger_stop.Equals(true))
                        {
                            spineName = mater2DBean.spines[i].name;
                            //spine 素材显示隐藏设置
                            MaterialsMgr.OnSetActive(key, spineName, false);
                        }
                    }
                }
            }

            //关闭提示
            PromptOpenClose(false);

            //停止音频播放
            if (mater2DBean.audios != null)
            {
                if (mater2DBean.audios.Count > 0)
                {
                    for (int i = 0; i < mater2DBean.audios.Count; i++)
                    {
                        //如果触发为空，停止播放音效
                        if (mater2DBean.audios[i].trigger.ToString() == "")
                        {
                            if (mater2DBean.audios[i].name != "")
                            {
                                audioName = mater2DBean.audios[i].name.Split('.')[0].ToString();
                                AudioMgr.AudioStop(audioName);
                            }
                        }
                    }
                }
            }

            // 处理背景音乐 看看触发的时候 是否要停止播放背景音乐
            if (mater2DBean.backgroundMusic != null)
            {
                if (mater2DBean.backgroundMusic.name != "")
                {
                    AudioMgr.AudioStop(mater2DBean.backgroundMusic.name.Split('.')[0].ToString());
                }
            }
        }
    }
    /// <summary>
	/// 素材默认显示隐藏
	/// </summary>
	/// <param name="bolarck2d">true:检测到人脸，flase:未检测到人脸</param>
	void SpineActive(bool trackResult, int beforeFaceNum, int nextFaceNum, int trackLimit)
    {
        Util.Log("SpineActive.........  " + trackResult);
        if (!trackResult)
            return;
        //检测到人脸
        if (nextFaceNum > 0)
        {
            if (beforeFaceNum != nextFaceNum)
            {
                SetOnceTrackValue(false);
            }
            if (!GetOnceTrackValue())
            {
                SetOnceTrackValue(true);
                OnTrackingFaceOnceMethod(beforeFaceNum, nextFaceNum, trackLimit);
            }
        }
        else
        {
            //未检测到人脸 隐藏素材
            if (GetOnceTrackValue())
            {
                SetOnceTrackValue(false);
                m_bolbackground = false;
                OnLostFaceMethod();
            }
        }
    }

    /// <summary>
    /// 根据挂载的屏幕获取m_bolOnceTrack的值
    /// </summary>
    bool GetOnceTrackValue()
    {
        if (ReceiveMgsManager.Localtion.Equals("Left"))
        {
            return m_bolOnceTrack_left;
        }
        else if (ReceiveMgsManager.Localtion.Equals("Right"))
        {
            return m_bolOnceTrack_right;
        }
        else if (ReceiveMgsManager.Localtion.Equals("SpineType"))
        {
            return m_bolOnceTrack;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// 根据挂载的屏设置m_bolOnceTrack的值
    /// </summary>
    /// <param name="value"></param>
    public void SetOnceTrackValue(bool value)
    {
        if (value)
        {
            if (ReceiveMgsManager.Localtion.Equals("Left"))
            {
                m_bolOnceTrack_left = value;
            }
            else if (ReceiveMgsManager.Localtion.Equals("Right"))
            {
                m_bolOnceTrack_right = value;
            }
            else if (ReceiveMgsManager.Localtion.Equals("SpineType"))
            {
                m_bolOnceTrack = value;
            }
        }
        else
        {
            if (FilteMgr.isDoubleScreen)
            {
                m_bolOnceTrack_right = value;
                m_bolOnceTrack_left = value;
            }
            else
            {
                m_bolOnceTrack = value;
            }
        }

    }

    /// <summary>
	/// 触发操作
	/// </summary>
	void SpineTrackTrigger(bool trackResult, int nextFaceNum, TRACK2DRET[] track2dret)
    {
        Util.Log("SpineTrackTrigger.........   " + trackResult);
        if (!trackResult)
            return;

        //抬头
        //UpHeadJudge (track2dret);

        //摇头
        //AroundHeadJudge(track2dret);

        for (int i = 0; i < nextFaceNum; i++) {
			OpenMouthJudge (track2dret [i], i);
		}
	}

	

    
     void SpineTrackSpinePoint(bool trackResult, int nextFaceNum, TRACK2DRET[] track2dret)
    {
        Util.Log("SpineTrackSpinePoint.." + trackResult);
        if (!trackResult)
        {
            return;
        }
        //#if !UNITY_EDITOR && UNITY_ANDROID
        //spine位置点
        OnSetSpinePoint(track2dret,nextFaceNum);
//#endif
        
    }

    
/*
    /// <summary>
    /// 摇头判断
    /// </summary>
    /// <param name="track2dret"></param>
    void AroundHeadJudge (TRACK2DRET track2dret)
	{
		if (m_bolAroundhead)
			return;

		//if (track2dret.rendering_paramsy < -0.1) {
		//	m_bolshakeright = true;
		//}

		//if (track2dret.rendering_paramsy > 0.1) {
		//	m_bolshakeleft = true;
		//}

		if (m_bolshakeright && m_bolshakeleft) {
			// 摇头触发
			m_bolshakeright = false;
			m_bolshakeleft = false;

			if (!m_bolAroundhead) {
				m_bolAroundhead = true;
			}
		} else {
			if (m_bolAroundhead) {
				m_bolAroundhead = false;
			}
		}

		if (m_bolAroundhead) {
			AroundHead ();
		}
	}

	/// <summary>
	/// 开启摇头触发
	/// </summary>
	void AroundHead ()
	{
		m_Aroundheadlist.Clear ();
		//关闭提示
//		ClosePrompt ();
		PromptOpenClose (false);
		//处理动画
		OpenAnimProcessing (Constants.AROUND_HEAD, ref m_Aroundheadlist);
		//处理音乐
		OpenAudioProcessing (Constants.AROUND_HEAD);
	}

	/// <summary>
	/// 结束摇头触发事件
	/// </summary>
	void CloseAroundHead ()
	{
		//处理音乐
		CloseAudioProcessing (Constants.AROUND_HEAD);
		//处理动画
		CloseAnimProcessing (Constants.AROUND_HEAD);
		m_Aroundheadlist.Clear ();
		m_bolAroundhead = false;
	}

	#endregion

	#region  抬头判断

	/// <summary>
	/// 抬头判断
	/// </summary>
	/// <param name="track2dret"></param>
	void UpHeadJudge (TRACK2DRET track2dret)
	{
		//if (m_bolUphead)
		//	return;
		////抬头
		//if (track2dret.rendering_paramsx < -0.4) {
		//	m_bolUphead = true;
		//} else {
		//	m_bolUphead = false;
		//}

		//if (m_bolUphead) {
		//	UpHead ();
		//}
	}

	/// <summary>
	/// 开启抬头触发事件
	/// </summary>
	void UpHead ()
	{
		m_UpheadAnimlist.Clear ();
		//关闭提示
		ClosePrompt ();
		//处理动画
		OpenAnimProcessing (Constants.UP_HEAD,0);
		//处理音乐
		OpenAudioProcessing (Constants.UP_HEAD);
	}

	/// <summary>
	/// 结束抬头触发事件
	/// </summary>
	void CloseUpHead ()
	{
		//处理音乐
		CloseAudioProcessing (Constants.UP_HEAD);
		//处理动画
		CloseAnimProcessing (Constants.UP_HEAD,0);
		////开启提示
		//OpenPrompt();
		m_UpheadAnimlist.Clear ();
		m_bolUphead = false;
	}

	#endregion

	*/
	#region  张嘴判断

	/// <summary>
	/// 张嘴判断
	/// </summary>
	void OpenMouthJudge (TRACK2DRET track2dret,int index)
	{
		double thresh = 0.5f;
		double dist_lr, dist_ud, dist;
		// l r u d
		dist_lr = Math.Sqrt ((track2dret.allTrackPoints [48].x - track2dret.allTrackPoints [54].x)
		* (track2dret.allTrackPoints [48].x - track2dret.allTrackPoints [54].x)
		+ (track2dret.allTrackPoints [48].y - track2dret.allTrackPoints [54].y)
		* (track2dret.allTrackPoints [48].y - track2dret.allTrackPoints [54].y));
		dist_ud = Math.Sqrt ((track2dret.allTrackPoints [51].x - track2dret.allTrackPoints [57].x)
		* (track2dret.allTrackPoints [51].x - track2dret.allTrackPoints [57].x)
		+ (track2dret.allTrackPoints [51].y - track2dret.allTrackPoints [57].y)
		* (track2dret.allTrackPoints [51].y - track2dret.allTrackPoints [57].y));
		dist = dist_ud / dist_lr;
#if UNITY_EDITOR
		dist = dou;
#endif
		if (WebCamManager.u_mouthopenparamDic.ContainsKey (index)) {
			WebCamManager.u_mouthopenparamDic [index] = (float)dist;
		} else {
			WebCamManager.u_mouthopenparamDic.Add (index, (float)dist);
		}
		if (dist > thresh) {

			bool openmouth = true;
			if (m_bolOpenMouthDic.ContainsKey (index)) {
				openmouth = m_bolOpenMouthDic [index];
			} else {
				openmouth = false;
				m_bolOpenMouthDic.Add (index, openmouth);
			}
			if (!openmouth) {
				m_bolOpenMouthDic [index] = true;
				OpenMouth (index);
			}
		}
	}

	/// <summary>
	/// 开启张嘴触发事件
	/// </summary>
	void OpenMouth (int index)
	{
		if (index == 0) {
			//关闭提示
			PromptOpenClose (false);
		}
		//处理动画
		OpenAnimProcessing (Constants.OPEN_MOUTH, index);
		//处理音乐
		OpenAudioProcessing (Constants.OPEN_MOUTH, index);
	}

	/// <summary>
	/// 结束张嘴触发事件
	/// </summary>
	void CloseMouth (int index)
	{
		//处理音乐
		CloseAudioProcessing (Constants.OPEN_MOUTH, index);
		//处理动画
		CloseAnimProcessing (Constants.OPEN_MOUTH, index);
		if (m_OpenMouthAnimTouchDic.ContainsKey (index)) {
			m_OpenMouthAnimTouchDic [index] = new List<string> ();
		} 
		if (m_bolOpenMouthDic.ContainsKey (index)) {
			m_bolOpenMouthDic [index] = false;
		}
	}

	#endregion

	#region 点击屏幕判断

	/// <summary>
	/// 开启点击屏幕触发事件
	/// </summary>
	void StartClickScreen ()
	{
		m_isClickScreen = true;
		m_ClickScreenlist.Clear ();
        //关闭提示
        PromptOpenClose(false);
        //处理动画
        OpenAnimProcessing (Constants.CLICK_SCREEN, 0);
		//处理音乐
		OpenAudioProcessing (Constants.CLICK_SCREEN, 0);
	}

	/// <summary>
	/// 结束点击屏幕触发事件
	/// </summary>
	void CloseClickScreen ()
	{
		//处理音乐
		CloseAudioProcessing (Constants.CLICK_SCREEN, 0);
		//处理动画
		CloseAnimProcessing (Constants.CLICK_SCREEN,0);
		////开启提示
		//OpenPrompt();
		m_ClickScreenlist.Clear ();
		//countTime = 0;
		m_isClickScreen = false;
	}

	#endregion

	#region 按住屏幕判断

	/// <summary>
	/// 开启长按屏幕触发事件
	/// </summary>
	void StartHoldDownScreen ()
	{
		m_isHoldDownScreen = true;
		m_HoldDownScreenlist.Clear ();
		//关闭提示
//		ClosePrompt ();
		PromptOpenClose (false);
		//处理动画
		OpenAnimProcessing (Constants.HOLDDOWN_SCREEN, 0);
		//处理音乐
		OpenAudioProcessing (Constants.HOLDDOWN_SCREEN, 0);
	}

	/// <summary>
	/// 结束长按屏幕触发事件
	/// </summary>
	void CloseHoldDownScreen ()
	{
		//处理音乐
		CloseAudioProcessing (Constants.HOLDDOWN_SCREEN, 0);
		//处理动画
		CloseAnimProcessing (Constants.HOLDDOWN_SCREEN,0);

		m_HoldDownScreenlist.Clear ();
		//countTime = 0;
		m_isHoldDownScreen = false;
	}

    #endregion

    /// <summary>
    /// Spine位置缩放旋转设置
    /// </summary>
    /// <param name="track2dret"></param>
    void OnSetSpinePoint(TRACK2DRET[] track2dret, int nextFaceNum)
    {

        if (MaterialsMgr.material2DBeanList != null && MaterialsMgr.material2DBeanList.Count > 0)
        {
            var enumeartor = MaterialsMgr.material2DBeanList.GetEnumerator();
            while (enumeartor.MoveNext())
            {
                Material2DBean mater2DBean = enumeartor.Current.Value;
                if (mater2DBean == null)
                {
                    continue;
                }
                SpinePoint(mater2DBean, enumeartor.Current.Key, track2dret, nextFaceNum);
            }
        }
    }

    void SpinePoint(Material2DBean mater2DBean, string key, TRACK2DRET[] track2dret, int nextFaceNum)
    {
        if(mater2DBean == null)
        {
            return;
        }
        if(mater2DBean.spines == null)
        {
            return;
        }

        float zOrder = 0.0f;
        string position = "";
        bool isExist = false;
        int multimode = 0;
        for (int i = 0; i < mater2DBean.spines.Count; i++)
        {
            position = mater2DBean.spines[i].position.ToString();

            //判断 位置点是否为数组
            WebCamMgr.getPosition(position.Split(','), ref position);
            isExist = Constants.IsContainsFixedPoint(int.Parse(position));
            if (!isExist)
            {
                multimode = int.Parse(mater2DBean.spines[i].multimode);
                if (multimode <= nextFaceNum)
                {
                    spineName = mater2DBean.spines[i].name;
                    zOrder = (float)mater2DBean.spines[i].zOrder;
                    float angle = (float)(track2dret[multimode - 1].angle * 180 / 3.14);
                    TrackPointCoord p = track2dret[multimode - 1].mountTrackPoints[int.Parse(position)];

                    MaterialsMgr.OnPixelsPointToWorldPoint(key, spineName, "" + p.y + "," + p.x + "," + zOrder + "");  //位置
                    MaterialsMgr.OnWorldRotation(key, spineName, angle);  //角度
                    if (track2dret[multimode - 1].scale > 10)
                    {
                        track2dret[multimode - 1].scale = 0;
                    }
                    MaterialsMgr.OnPixelsPointToWorldScale(key, spineName, track2dret[multimode - 1].scale + "");  //大小
                }
            }
        }
    }

	/// <summary>
	/// 所有动画完成后事件
	/// </summary>
	/// <param name="spineName"></param>
	public void SpineAnimEnd (string spineName)
	{
		if (spineName == "")
			return;
		
		int index = int.Parse (spineName.Split ('@') [1].ToString ()); 

		#region 张嘴
		if (m_OpenMouthAnimTouchDic != null) {
			if (m_OpenMouthAnimTouchDic.Count > 0) {
				if (m_OpenMouthAnimTouchDic.ContainsKey (index - 1)) {
					List<string> m_list = m_OpenMouthAnimTouchDic [index - 1];
					if (m_list != null) {
						if (m_list.Count > 0) {
							//张嘴
							if (m_list.Contains (spineName)) {
								m_list.Remove (spineName);
								if (m_list.Count == 0) {
									CloseMouth (index - 1);
								}
							}
						}
					}
				}
			}
		}
		#endregion 

		//抬头
//		if (m_UpheadAnimlist.Contains (spineName)) {
//			m_UpheadAnimlist.Remove (spineName);
//		}
//		if (m_UpheadAnimlist.Count == 0) {
//			//CloseUpHead ();
//		}
//		//摇头
//		if (m_Aroundheadlist.Contains (spineName)) {
//			m_Aroundheadlist.Remove (spineName);
//		}
//		if (m_Aroundheadlist.Count == 0) {
//			//CloseAroundHead ();
//		}

		if (index == 1) {
			
			#region 点击屏幕
			if (m_ClickScreenlist.Contains (spineName)) {
				m_ClickScreenlist.Remove (spineName);
				if (m_ClickScreenlist.Count == 0) {
					CloseClickScreen ();
				}
			}
			#endregion 

			#region 长按屏幕
			if (m_HoldDownScreenlist.Contains (spineName)) {
				m_HoldDownScreenlist.Remove (spineName);
				if (m_HoldDownScreenlist.Count == 0) {
					CloseHoldDownScreen ();
				}
			}
			#endregion 
		}
	}

      

    /// <summary>
    /// 关闭音频处理
    /// </summary>
    /// <param name="constants"></param>
    void CloseAudioProcessing(string constants)
    {
        // 处理背景音乐 看看触发的时候 是否要停止播放背景音乐
        if (MaterialsMgr.m_material2DBean.backgroundMusic.trigger_stop)
        {
            if (MaterialsMgr.m_material2DBean.backgroundMusic.name != "")
            {
                AudioMgr.AudioPlay(MaterialsMgr.m_material2DBean.backgroundMusic.name.Split('.')[0].ToString());
            }
        }
        // 这里 除了要处理音乐逻辑 还要处理素材显示跟隐藏逻辑 现在的逻辑一般一个素材只有一种触发模式 一种触发音乐
        if (MaterialsMgr.m_material2DBean.audios != null && MaterialsMgr.m_material2DBean.audios.Count > 0)
        {
            for (int i = 0; i < MaterialsMgr.m_material2DBean.audios.Count; i++)
            {
                if (MaterialsMgr.m_material2DBean.audios[i].trigger.Equals(constants))
                {
                    if (MaterialsMgr.m_material2DBean.audios[i].name != "")
                    {
                        AudioMgr.AudioStop(MaterialsMgr.m_material2DBean.audios[i].name.Split('.')[0].ToString());
                    }
                }
            }
        }
    }

	/*
    /// <summary>
    /// 开启动画处理
    /// </summary>
    /// <param name="constants"></param>
    void OpenAnimProcessing(string constants, ref List<string> animlist)
    {
        if (MaterialsMgr.m_material2DBean.spines != null)
        {
            bool boltouch = false;
            for (int i = 0; i < MaterialsMgr.m_material2DBean.spines.Count; i++)
            {
                if (MaterialsMgr.m_material2DBean.spines[i].trigger.Equals(constants))
                {
                    boltouch = true;
                }
            }
            if (boltouch)
            {
                for (int i = 0; i < MaterialsMgr.m_material2DBean.spines.Count; i++)
                {
                    string spineName2d = MaterialsMgr.m_material2DBean.spines[i].name;
                    if (MaterialsMgr.m_material2DBean.spines[i].trigger.Equals(constants))
                    {
                        animlist.Add(spineName2d);
                    }
                }
                for (int i = 0; i < MaterialsMgr.m_material2DBean.spines.Count; i++)
                {
                    string spineName2d = MaterialsMgr.m_material2DBean.spines[i].name;
                    if (MaterialsMgr.m_material2DBean.spines[i].trigger.Equals(constants))
                    {
                        MaterialsMgr.OnSetActive(spineName2d, true);
                        AnimSpineMgr.SetAnimator(spineName2d, MaterialsMgr.m_material2DBean.spines[i].repeat);
                    }
                    else
                    {
                        if (MaterialsMgr.m_material2DBean.spines[i].trigger_stop)
                        {
                            MaterialsMgr.OnSetActive(spineName2d, false);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 关闭动画处理
    /// </summary>
    /// <param name="constants"></param>
    void CloseAnimProcessing(string constants)
    {
        if (MaterialsMgr.m_material2DBean.spines != null)
        {
            bool boltouch = false;
            for (int i = 0; i < MaterialsMgr.m_material2DBean.spines.Count; i++)
            {
                if (MaterialsMgr.m_material2DBean.spines[i].trigger.Equals(constants))
                {
                    boltouch = true;
                }
            }
            if (boltouch)
            {
                for (int i = 0; i < MaterialsMgr.m_material2DBean.spines.Count; i++)
                {
                    string spineName2d = MaterialsMgr.m_material2DBean.spines[i].name;
                    if (MaterialsMgr.m_material2DBean.spines[i].trigger.Equals(constants))
                    {
                        MaterialsMgr.OnSetActive(spineName2d, false);
                    }
                    else
                    {
                        if (MaterialsMgr.m_material2DBean.spines[i].trigger_stop)
                        {
                            MaterialsMgr.OnSetActive(spineName2d, true);
                        }
                    }
                }
            }
        }
    }
	*/

	/// <summary>
	/// 开启音频处理
	/// </summary>
	/// <param name="constants"></param>
	void OpenAudioProcessing (string constants, int index)
	{
		if (MaterialsMgr.m_material2DBean.backgroundMusic != null) {
			if (MaterialsMgr.m_material2DBean.backgroundMusic.trigger != null) {
				// 处理背景音乐 看看触发的时候 是否要停止播放背景音乐
				if (MaterialsMgr.m_material2DBean.backgroundMusic.trigger.Equals (constants) &&
				   MaterialsMgr.m_material2DBean.backgroundMusic.trigger_stop) {
					if (MaterialsMgr.m_material2DBean.backgroundMusic.name != "") {
						AudioMgr.AudioStop (MaterialsMgr.m_material2DBean.backgroundMusic.name.Split ('.') [0].ToString ());
					}
				}
			}
		}
		int multimode = 0;
		// 这里 除了要处理音乐逻辑 还要处理素材显示跟隐藏逻辑 现在的逻辑一般一个素材只有一种触发模式 一种触发音乐
		if (MaterialsMgr.m_material2DBean.audios != null) {
			if(MaterialsMgr.m_material2DBean.audios.Count > 0)
			{
				for (int i = 0; i < MaterialsMgr.m_material2DBean.audios.Count; i++) {
					if (MaterialsMgr.m_material2DBean.audios [i].trigger.Equals (constants)) {
						multimode = int.Parse (MaterialsMgr.m_material2DBean.audios [i].multimode);
						if (multimode == index + 1) {
							if (MaterialsMgr.m_material2DBean.audios [i].name != "") {
								AudioMgr.AudioPlay (MaterialsMgr.m_material2DBean.audios [i].name.Split ('.') [0].ToString ());
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// 关闭音频处理
	/// </summary>
	/// <param name="constants"></param>
	void CloseAudioProcessing (string constants, int index)
	{
		if (MaterialsMgr.m_material2DBean.backgroundMusic != null) {
			if (MaterialsMgr.m_material2DBean.backgroundMusic.trigger != null) {
				//停止动画重新播放背景音乐
				if (MaterialsMgr.m_material2DBean.backgroundMusic.trigger.Equals (constants) &&
				   MaterialsMgr.m_material2DBean.backgroundMusic.trigger_stop) {
					if (MaterialsMgr.m_material2DBean.backgroundMusic.name != "") {
						AudioMgr.AudioPlay (MaterialsMgr.m_material2DBean.backgroundMusic.name.Split ('.') [0].ToString ());
					}
				}
			}
		}
		int multimode = 0;
		// 这里 除了要处理音乐逻辑 还要处理素材显示跟隐藏逻辑 现在的逻辑一般一个素材只有一种触发模式 一种触发音乐
		if (MaterialsMgr.m_material2DBean.audios != null) {
			if (MaterialsMgr.m_material2DBean.audios.Count > 0) {
				for (int i = 0; i < MaterialsMgr.m_material2DBean.audios.Count; i++) {
					multimode = int.Parse (MaterialsMgr.m_material2DBean.audios [i].multimode);
					if (multimode == index + 1) {
						if (MaterialsMgr.m_material2DBean.audios [i].trigger.Equals (constants)) {
							if (MaterialsMgr.m_material2DBean.audios [i].name != "") {
								AudioMgr.AudioStop (MaterialsMgr.m_material2DBean.audios [i].name.Split ('.') [0].ToString ());
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// 开启动画处理 back up
	/// </summary>
	/// <param name="constants"></param>
	void OpenAnimProcessing (string constants, int index)
	{
		if (MaterialsMgr.m_material2DBean.spines != null) {
			bool boltouch = false;
			int multimode = 0;
			for (int i = 0; i < MaterialsMgr.m_material2DBean.spines.Count; i++) {
				multimode = int.Parse (MaterialsMgr.m_material2DBean.spines [i].multimode);
				if (multimode == index + 1) {
					if (MaterialsMgr.m_material2DBean.spines [i].trigger.Equals (constants)) {
						boltouch = true;
						break;
					}
				}
			}
			if (boltouch) {
				List<string> animlist = new List<string> ();
				for (int i = 0; i < MaterialsMgr.m_material2DBean.spines.Count; i++) {
					multimode = int.Parse (MaterialsMgr.m_material2DBean.spines [i].multimode);
					if (multimode == index + 1) {
						spineName = MaterialsMgr.m_material2DBean.spines [i].name;
						if (MaterialsMgr.m_material2DBean.spines [i].trigger.Equals (constants) &&
						    MaterialsMgr.m_material2DBean.spines [i].trigger_stop.Equals (false)) {
							//需要先将 有触发条件 并且 不需要触发停止的动画添加到列表中；
							animlist.Add (spineName);
						}
					}
				}
				switch (constants) {
				case "mouth_open":
					if (m_OpenMouthAnimTouchDic.ContainsKey (index)) {
						m_OpenMouthAnimTouchDic [index] = animlist;
					} else {
						m_OpenMouthAnimTouchDic.Add (index, animlist);
					}
					break;
				case "click_Screen":
					m_ClickScreenlist = animlist;
					break;
				case  "holdDown_Screen":
					m_HoldDownScreenlist = animlist;
					break;
				}
				for (int i = 0; i < MaterialsMgr.m_material2DBean.spines.Count; i++) {
					multimode = int.Parse (MaterialsMgr.m_material2DBean.spines [i].multimode);
					if (multimode == index + 1) {
						spineName = MaterialsMgr.m_material2DBean.spines [i].name;
						if (MaterialsMgr.m_material2DBean.spines [i].trigger.Equals (constants) &&
						    MaterialsMgr.m_material2DBean.spines [i].trigger_stop.Equals (false)) {
							//播放 有触发条件的且不需要触发停止的动画
							multimode = int.Parse (MaterialsMgr.m_material2DBean.spines [i].multimode);
							MaterialsMgr.OnSetActive (spineName, true);
							AnimSpineMgr.SetAnimator (spineName, MaterialsMgr.m_material2DBean.spines [i].repeat);
						} else if (MaterialsMgr.m_material2DBean.spines [i].trigger.Equals (constants) &&
						           MaterialsMgr.m_material2DBean.spines [i].trigger_stop.Equals (true)) {
							//停止 有触发条件且需要触发停止的动画
							MaterialsMgr.OnSetActive (spineName, false);
						}
					}
				}
			}
		}
	}
    /// <summary>
	/// 关闭动画处理 back up
	/// </summary>
	/// <param name="constants"></param>
	void CloseAnimProcessing(string constants, int index)
    {
        if (MaterialsMgr.m_material2DBean.spines != null)
        {
            bool boltouch = false;
            int multimode = 0;
            for (int i = 0; i < MaterialsMgr.m_material2DBean.spines.Count; i++)
            {
                if (MaterialsMgr.m_material2DBean.spines[i].trigger.Equals(constants))
                {
                    multimode = int.Parse(MaterialsMgr.m_material2DBean.spines[i].multimode);
                    if (multimode == index + 1)
                    {
                        boltouch = true;
                        break;
                    }
                }
            }

            if (boltouch)
            {
                for (int i = 0; i < MaterialsMgr.m_material2DBean.spines.Count; i++)
                {
                    multimode = int.Parse(MaterialsMgr.m_material2DBean.spines[i].multimode);
                    if (multimode == index + 1)
                    {
                        spineName = MaterialsMgr.m_material2DBean.spines[i].name;
                        if (MaterialsMgr.m_material2DBean.spines[i].trigger.Equals(constants) &&
                            MaterialsMgr.m_material2DBean.spines[i].trigger_stop.Equals(false))
                        {
                            MaterialsMgr.OnSetActive(spineName, false);
                        }
                        else if (MaterialsMgr.m_material2DBean.spines[i].trigger.Equals(constants) &&
                                 MaterialsMgr.m_material2DBean.spines[i].trigger_stop.Equals(true))
                        {
                            MaterialsMgr.OnSetActive(spineName, true);
                        }
                    }
                }
            }
        }
    }
   
	/*
	/// <summary>
	/// 开启提示
	/// </summary>
	public void OpenPrompt ()
	{
		if (MaterialsMgr.m_material2DBean != null) {
			//提示语
			if (MaterialsMgr.m_material2DBean.prompt != "" && MaterialsMgr.m_material2DBean.prompt != null) {
				PlatformMgr.OnShowHideIcon (MaterialsMgr.m_material2DBean.prompt, true);
//				MaterialsMgr.OnSetActive (MaterialsMgr.m_material2DBean.prompt, true);
				WebCamMgr.ActiveObject ();
			}
		}
	}

	/// <summary>
	/// 关闭提示
	/// </summary>
	public void ClosePrompt ()
	{
		if (MaterialsMgr.m_material2DBean != null) {
			//提示语
			if (MaterialsMgr.m_material2DBean.prompt != "" && MaterialsMgr.m_material2DBean.prompt != null) {
				PlatformMgr.OnShowHideIcon (MaterialsMgr.m_material2DBean.prompt, false);
//				MaterialsMgr.OnSetActive (MaterialsMgr.m_material2DBean.prompt, false);
			}
		}
	}
	*/
    /// <summary>
	/// 提示开启与关闭处理逻辑
	/// </summary>
	/// <param name="bol">If set to <c>true</c> bol.</param>
	public void PromptOpenClose(bool bol)
    {
        if (MaterialsMgr.m_material2DBean != null)
        {
            //提示语
            if (MaterialsMgr.m_material2DBean.prompt != "" && MaterialsMgr.m_material2DBean.prompt != null)
            {
                PlatformMgr.OnShowHideIcon(MaterialsMgr.m_material2DBean.prompt, bol);
                if (bol)
                {
                    WebCamMgr.ActiveObject();
                }
            }
        }
    }
    /// <summary>
    /// 屏幕点击  true 长按； false 点击
    /// </summary>
    /// <param name="bol">If set to <c>true</c> bol.</param>
    public void ClickScreenCheckIOS (bool bol)
	{
		if (!bol && !m_isClickScreen) {
			//点击
			StartClickScreen ();
		} else if (bol && !m_isHoldDownScreen) {
			//长按
			StartHoldDownScreen ();
		}
	}
}