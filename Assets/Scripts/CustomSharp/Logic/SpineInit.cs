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
using System.Text;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SpineInit : Manager
{
	/// <summary>
	/// 素材计数
	/// </summary>
	private int number = 0;
	/// <summary>
	/// 素材中spine名称
	/// </summary>
	private string spineName = "";
	/// <summary>
	/// 音频素材名称
	/// </summary>
	private string audioName = "";
	/// <summary>
	/// 隐藏旧素材
	/// </summary>
	public void DelectSpineMateria (Material2DBean material2DBean, string key)
	{
        WebCamMgr.m_exist2D = false;
        Util.Log("DelectSpineMateria==> " + key + "    " + WebCamMgr.m_tarckInit_2d);
		//判断是否是首次操作
		if (WebCamMgr.m_tarckInit_2d) {
			//设置默认属性值
			WebCamMgr.ResetValue (false, "2D");
			//删除屏幕中旧素材及其动画
			if (material2DBean != null) {
				
				if (material2DBean.spines != null) {
					number = material2DBean.spines.Count;
					if (number > 0) {
						for (int i = 0; i < number; i++) {
							spineName = material2DBean.spines [i].name;
							AnimSpineMgr.StopAnimator (spineName);            //停止spine动画
                            Util.Log("DelectSpineMateria==>eeee " + key + "    " + spineName);
                            GameObject obj = MaterialsMgr.GetObjFromClassifyDic(key, spineName);
                            if(obj == null)
                            {
                                break;
                            }
                            //MaterialsMgr.OnSetActive(key, spineName, false);  //隐藏spine素材
                            MaterialsMgr.OnDestroyGameObject(obj);
                        }
					}
				}
				//添加大眼瘦脸形变 改为大眼瘦脸形变
				FilteMgr.FaceMorphFilter ("EyeFace");
                //				if (material2DBean.prompt != "" && material2DBean.prompt != null) {
                ////					MaterialsMgr.OnSetActive (material2DBean.prompt, false);
                //					PlatformMgr.OnShowHideIcon (material2DBean.prompt,false);
                //				}
                //删除屏幕中旧音效
                if (material2DBean.audios != null) {
					number = material2DBean.audios.Count;
					if (number > 0) {
						//停止播放屏幕中素材音效
						for (int i = 0; i < number; i++) {
							audioName = material2DBean.audios [i].name;
							if (audioName != "") {
								AudioMgr.AudioStop (audioName.Split ('.') [0].ToString ());
							}
						}
					}
				}
				//停止播放屏幕中旧背景音效
				if (material2DBean.backgroundMusic != null) {
					audioName = material2DBean.backgroundMusic.name;
					if (audioName != "") {
						AudioMgr.AudioStop (audioName.Split ('.') [0].ToString ());
					}
				}
			}
		}
		//清除所有旧音频
		AudioMgr.ClearAudioSource ();
		Util.ClearMemory ();
	}

    public void DelectSpine(string path)
    {
        if (MaterialsMgr.material2DBeanList.ContainsKey(ReceiveMgsManager.Localtion))
        {
            DelectSpineMateria(MaterialsMgr.material2DBeanList[ReceiveMgsManager.Localtion], ReceiveMgsManager.Localtion);
        }
    }

    /// <summary>
    /// 切换屏幕后，根据全屏、分屏删除Material3DBean
    /// </summary>
    /// <param name="isDouble"></param>
    public void DelectMaterial2DBean(bool isDouble)
    {
        Util.Log("DelectMaterial2DBean==>111  " + isDouble);
        //双屏状态，删除单屏数据
        if (isDouble)
        {
            if (MaterialsMgr.material2DBeanList.ContainsKey("SpineType"))
            {
                DelectSpineMateria(MaterialsMgr.material2DBeanList["SpineType"], "SpineType");
                MaterialsMgr.material2DBeanList.Remove("SpineType");
            }
        }
        //单屏状态，删除双屏数据
        else
        {
            if (MaterialsMgr.material2DBeanList.ContainsKey("Left"))
            {
                DelectSpineMateria(MaterialsMgr.material2DBeanList["Left"], "Left");
                MaterialsMgr.material2DBeanList.Remove("Left");
                WebCamMgr.m_tarckInit_2d = true; //此为一次性执行开关控制，再次打开是为了删除Right的数据
            }
            if (MaterialsMgr.material2DBeanList.ContainsKey("Right"))
            {
                DelectSpineMateria(MaterialsMgr.material2DBeanList["Right"], "Right");
                MaterialsMgr.material2DBeanList.Remove("Right");
            }
        }
    }

    string filteMgrName = "EyeFace";
	string filteMultimode = "1,2";
	MorphFilterBean deformation = new MorphFilterBean ();
	/// <summary>
	/// 获取spine素材
	/// </summary>
	/// <param name="info"></param>
	public void LoadSpineMateria (string path, string info, Action<bool> action)
	{
		// DelectSpineMateria();
		//控制是否进行人脸检测
		WebCamMgr.m_tarck2dOr3d = false;
        WebCamMgr.m_exist2D = true;
		MaterialsMgr.m_material2DBean = null;
		//解析jsonh填充数据到结构体
		MaterialsMgr.m_material2DBean = Util.GetJsonData<Material2DBean> (info, true);
		if (MaterialsMgr.m_material2DBean != null) {
			//获取音频素材
			audioName = CreateAudio (MaterialsMgr.m_material2DBean);
			GetAudioStruct ();
			if (audioName != "") {
				//加载音频素材
				AudioMgr.LoadAudio (path, audioName, () => {
					//设置音频素材属性
					SetAudioRepeat (MaterialsMgr.m_material2DBean);
				});
			}
			#region 形变 为兼容老素材 所以默认所有素材都有大眼瘦脸形变支持2个人脸
//			string filteMgrName = "EyeFace";
//			string filteMultimode = "1,2";
//			MorphFilterBean deformation = new MorphFilterBean ();
			//添加大眼瘦脸形变 如果morphfilter为空或者morphfilter==“”时，则代表要改为大眼瘦脸形变
			if (MaterialsMgr.m_material2DBean.deformation != null) {
				if (MaterialsMgr.m_material2DBean.deformation.name != "") {
					filteMgrName = MaterialsMgr.m_material2DBean.deformation.name;
					filteMultimode = MaterialsMgr.m_material2DBean.deformation.multimode;
				} 
			} 
			deformation.name = filteMgrName;
			deformation.multimode = filteMultimode;
			WebCamManager.multimode = filteMultimode.Split (',');
			MaterialsMgr.m_material2DBean.deformation = deformation;
			FilteMgr.FaceMorphFilter (MaterialsMgr.m_material2DBean.deformation.name);
			#endregion 

			//获取spine名称与素材名称
			string mateName = MaterialsMgr.m_material2DBean.name;
			GetSpineStruct ();
			spineName = CreateSpine (MaterialsMgr.m_material2DBean.spines);
			if (spineName != "") {
				//加载spine文件
//				path = path.Replace ("file://", "");
				MaterialsMgr.OnInitSpineManager (path, mateName, spineName.Split (','), (bol, isActive) => {
					if (bol) {
						//开启人脸检测
						WebCamMgr.m_tarckInit_2d = true;
					}
					action (bol);
				});
			}
            //添加material2DBean,不能够对引用的数据直接进行删除操作
            if (MaterialsMgr.material2DBeanList != null && !string.IsNullOrEmpty(ReceiveMgsManager.Localtion))
            {
                if (MaterialsMgr.material2DBeanList.ContainsKey(ReceiveMgsManager.Localtion))
                {
                    MaterialsMgr.material2DBeanList.Remove(ReceiveMgsManager.Localtion);
                }

                MaterialsMgr.material2DBeanList.Add(ReceiveMgsManager.Localtion, MaterialsMgr.m_material2DBean);  //没有当前素材就添加

                Util.Log("ReceiveMgsManager.Localtion => " + ReceiveMgsManager.Localtion);
            }
        }
	}

	/**
	 * 创建spine
	 * 
	 * @param unityPlayer
	 * @param spinePath
	 */
	string CreateSpine (List<Spine2DBean> spines)
	{
		StringBuilder inputName = new StringBuilder ();
		if (spines != null && spines.Count > 0) {
			for (int i = 0; i < spines.Count; i++) {
				if (i == 0) {
					inputName.Append (spines [i].name);
				} else {
					inputName.Append ("," + spines [i].name);
				}
			}
		}

//        //素材提示
//        string prompt = MaterialsMgr.m_material2DBean.prompt.ToString();
//        if (prompt!="")
//        {
//            inputName.Append("," + prompt);
//        }

		return inputName.ToString ();
	}

	/**
	 * 加载audio文件
	 * 
	 * @param unityPlayer
	 * @param spinePath
	 */
	string CreateAudio (Material2DBean currentBean)
	{
		StringBuilder inputName = new StringBuilder ();
		//添加背景音乐名称
		if (currentBean != null && currentBean.backgroundMusic != null && currentBean.backgroundMusic.name != "") {
			inputName.Append (currentBean.backgroundMusic.name);
		}

		if (currentBean.audios != null) {
			if (currentBean.audios.Count > 0) {
				for (int i = 0; i < currentBean.audios.Count; i++) {
					if (!currentBean.audios [i].name.Equals ("")) {
						if (inputName.ToString () != "") {
							inputName.Append ("," + currentBean.audios [i].name);
						} else {
							inputName.Append (currentBean.audios [i].name);
						}
					}
				}
			}
		}
		return inputName.ToString ();
	}

	/// <summary>
	/// 设置音频属性
	/// </summary>
	/// <param name="currentBean"></param>
	void SetAudioRepeat (Material2DBean currentBean)
	{
		if (currentBean.audios != null) {
			number = currentBean.audios.Count;
			if (number > 0) {
				for (int i = 0; i < number; i++) {
					if (currentBean.audios [i].trigger.ToString () != "") {
						if (currentBean.audios [i].name != "") {
							string currentAudioName = currentBean.audios [i].name.Split ('.') [0].ToString ();
							string audioName = currentAudioName.Split ('@') [0].ToString ();
							AudioMgr.SetAudio (audioName, currentAudioName, currentBean.audios [i].repeat);
						}
					}
				}
			}
		}

		if (currentBean.backgroundMusic != null&&  currentBean.backgroundMusic.name != "") {
			string currentAudioName = currentBean.backgroundMusic.name.Split ('.') [0].ToString ();
			// 处理背景音乐 看看触发的时候 是否要停止播放背景音乐
			AudioMgr.SetAudio (currentAudioName, currentAudioName, true);
		}
	}

	/// <summary>
	/// 重新组装音频到结构体信息
	/// </summary>
	void GetAudioStruct ()
	{
		List<Spine2DAudiosBean> audios = new List<Spine2DAudiosBean> ();
		Spine2DAudiosBean audiobean = new Spine2DAudiosBean ();
		if (MaterialsMgr.m_material2DBean.audios != null) {
			number = MaterialsMgr.m_material2DBean.audios.Count;
			if (number > 0) {
				string[] arrString = new string[1];
				string name = "";
				string audioformat = "";
				string audioName = "";
				bool visible = false;
				bool repeat = false;
				string trigger = "";
				bool trigger_stop = false;
				string multimode = "";
				for (int i = 0; i < number; i++) {
					if (MaterialsMgr.m_material2DBean.audios [i].name == "") {
						continue;
					}
					name = MaterialsMgr.m_material2DBean.audios [i].name.ToString ();
					audioName = name.Split ('.') [0].ToString ();
					audioformat = name.Split ('.') [1].ToString ();
					visible = MaterialsMgr.m_material2DBean.audios [i].visible;
					repeat = MaterialsMgr.m_material2DBean.audios [i].repeat;
					trigger = MaterialsMgr.m_material2DBean.audios [i].trigger.ToString ();
					trigger_stop = MaterialsMgr.m_material2DBean.audios [i].trigger_stop;
					arrString [0] = "1";
					if (MaterialsMgr.m_material2DBean.audios [i].multimode != null) {
						multimode = MaterialsMgr.m_material2DBean.audios [i].multimode.ToString ();
						if (multimode != "") {
							string[] arrmultimode = multimode.Split (',');
							if (arrmultimode != null && arrmultimode.Length > 0) {
								arrString = new string[arrmultimode.Length];
								for (int n = 0; n < arrmultimode.Length; n++) {
									arrString [n] = arrmultimode [n].ToString ();
								}
							}
						}
					}
					for (int n = 0; n < arrString.Length; n++) {
						audiobean = new Spine2DAudiosBean ();
						multimode = arrString [n].ToString ();
						audiobean.name = audioName + "@" + multimode + "." + audioformat;
						audiobean.visible = visible;
						audiobean.repeat = repeat;
						audiobean.trigger = trigger;
						audiobean.trigger_stop = trigger_stop;
						audiobean.multimode = multimode;
						audios.Add (audiobean);
					}
				}
				if (audios != null && audios.Count > 0) {
					MaterialsMgr.m_material2DBean.audios = audios;
				} else {
					MaterialsMgr.m_material2DBean.audios = null;
				}
			}
		}
	}

	/// <summary>
	/// 重组2d spien到结构体数据
	/// </summary>
	void GetSpineStruct ()
	{
		List<Spine2DBean> spines = new List<Spine2DBean> ();
		Spine2DBean spinebean = new Spine2DBean ();
		if (MaterialsMgr.m_material2DBean.spines != null) {
			number = MaterialsMgr.m_material2DBean.spines.Count;
			if (number > 0) {
				string[] arrString = new string[1];
				string position = "";
				double zOrder = 0.0f;
				bool visible = false;
				bool repeat = false;
				string trigger = "";
				bool trigger_stop = false;
				int trigger_order = 0;
				string multimode = "";
				for (int i = 0; i < number; i++) {
					if (MaterialsMgr.m_material2DBean.spines [i].name == "") {
						continue;
					}
					spineName = MaterialsMgr.m_material2DBean.spines [i].name.ToString ();
					position = MaterialsMgr.m_material2DBean.spines [i].position.ToString ();
					zOrder = MaterialsMgr.m_material2DBean.spines [i].zOrder;
					visible = MaterialsMgr.m_material2DBean.spines [i].visible;
					repeat = MaterialsMgr.m_material2DBean.spines [i].repeat;
					trigger = MaterialsMgr.m_material2DBean.spines [i].trigger.ToString ();
					trigger_stop = MaterialsMgr.m_material2DBean.spines [i].trigger_stop;
					trigger_order = MaterialsMgr.m_material2DBean.spines [i].trigger_order;
					arrString [0] = "1";
					if (MaterialsMgr.m_material2DBean.spines [i].multimode != null) {
						multimode = MaterialsMgr.m_material2DBean.spines [i].multimode.ToString ();
						if (multimode != "") {
							string[] arrmultimode = multimode.Split (',');
							if (arrmultimode != null && arrmultimode.Length > 0) {
								arrString = new string[arrmultimode.Length];
								for (int n = 0; n < arrmultimode.Length; n++) {
									arrString [n] = arrmultimode [n].ToString ();
								}
							}
						}
					}

					for (int n = 0; n < arrString.Length; n++) {
						spinebean = new Spine2DBean ();
						multimode = arrString [n].ToString ();
						spinebean.name = spineName + "@" + multimode;
						spinebean.position = position;
						spinebean.visible = visible;
						spinebean.repeat = repeat;
						spinebean.zOrder = zOrder;
						spinebean.trigger = trigger;
						spinebean.trigger_stop = trigger_stop;
						spinebean.trigger_order = trigger_order;
						spinebean.multimode = multimode;
						spines.Add (spinebean);
					}
				}
				if (spines != null && spines.Count > 0) {
					MaterialsMgr.m_material2DBean.spines = spines;
				} else {
					MaterialsMgr.m_material2DBean.spines = null;
				}
			}
		}
	}
}
