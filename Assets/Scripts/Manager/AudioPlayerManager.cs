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
using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerManager : Manager
{
	static Dictionary<string, AudioClip> audioClipDic = new Dictionary<string, AudioClip> ();
	static Dictionary<string, AudioSource> audioSourceDic = new Dictionary<string, AudioSource> ();
	static List<GameObject> audioSourcelist = new List<GameObject> ();

	static GameObject audioPerfab;
	static float m_AudioVolume = 1.0f;

	public void OnAwakeUp ()
	{
		audioPerfab = Resources.Load ("AudioSource", typeof(GameObject)) as GameObject;
	}

	//获取音频组建
	void GetAudioSourceEvent (ref AudioSource audiosource, GameObject obj)
	{
		audiosource = obj.GetComponent<AudioSource> ();
		if (null == audiosource) {
			audiosource = obj.AddComponent<AudioSource> ();
		}
	}

	/// <summary>
	/// 添加一个声音剪辑
	/// </summary>
	void Add (string key, AudioClip value)
	{
		if (audioClipDic.ContainsKey (key) || value == null)
			return;
		audioClipDic.Add (key, value);
	}

	/// <summary>
	/// 获取一个声音剪辑
	/// </summary>
	AudioClip Get (string key)
	{
		if (audioClipDic.ContainsKey (key)) {
			return audioClipDic [key] as AudioClip;
		} else {
			return null;
		}
	}

	/// <summary>
	/// 添加一个声音源
	/// </summary>
	void AddAudioSource (string key, AudioSource value)
	{
		if (audioSourceDic.ContainsKey (key) || value == null)
			return;
		audioSourceDic.Add (key, value);
	}

	/// <summary>
	/// 获取一个声音源
	/// </summary>
	AudioSource GetAudioSource (string key)
	{
		if (audioSourceDic.ContainsKey (key)) {
			return audioSourceDic [key] as AudioSource;
		} else {
			return null;
		}
	}

	/// <summary>
	/// 加载音频文件
	/// </summary>
	/// <param name="filepath"></param>
	public void LoadAudio (string filepath, string name, Action action)
	{
		if (filepath == "") {
			Util.LogError ("AudioPlayerManager LoadAudio=> filepath is null!");
			action ();
		}
		if (name == "") {
			Util.LogError ("AudioPlayerManager LoadAudio=> name is null!");
			action ();
		}
		string[] arrayName = name.Split (',');
		if (arrayName == null) {
			Util.LogError ("AudioPlayerManager LoadAudio=> name array is null!");
			action ();
		}

		//判断不存在场景中素材是否存在于预设中
		List<string> newAudioList = new List<string> ();
		string audioName = "";
		for (int i = 0; i < arrayName.Length; i++) {
			if (arrayName [i] != "") {
				audioName = arrayName [i].Split ('.') [0].ToString ();
				audioName = audioName.Split ('@') [0].ToString ();
				if (!Get(audioName)) 
				{
					if (!newAudioList.Contains (arrayName [i])) {
						newAudioList.Add (arrayName [i].ToString ());
					}
						
				}
			}
		}
		AudioClip audioClip = null;
		AssetBundle assetbundle_mp3 = null;
		if (newAudioList.Count > 0) {
			for (int i = 0; i < newAudioList.Count; i++) {
				audioName = newAudioList [i].Split ('.') [0].ToString ();
				assetbundle_mp3 = AssetBundle.LoadFromFile (filepath + "/" + newAudioList [i].ToString () + ".unity3d");
				if (assetbundle_mp3 != null) {
					audioClip = assetbundle_mp3.LoadAsset<AudioClip> (audioName);
					if (audioClip != null) {
						Add (audioName, audioClip);
					}
				}
			}
		}
		action ();

//		Queue<string> que = new Queue<string> ();
//		if (arrayName != null) {
//			if (arrayName.Length > 0) {
//				foreach (string str in arrayName) {
//					if (str != "") {
//						que.Enqueue (filepath + "/" + str);
//					}
//				}
//			}
//		}
//
//		QueLoadAudio (que, () => {
//			action ();
//		});
	}

	void QueLoadAudio (Queue<string> que, Action action)
	{
		if (que != null) {
			if (que.Count > 0) {
				string filepath = que.Dequeue ();
				if (filepath != "") {
					int index = filepath.LastIndexOf ("/");
					string abName = filepath.Substring (index + 1).ToString ();
					string audioName = abName.Split ('.') [0].ToString ();
					if (Get (audioName)) {
						QueLoadAudio (que, action);
					} else {
						AudioClip audioClip = null;
						AssetBundle assetbundle_mp3 = null;
						assetbundle_mp3 = AssetBundle.LoadFromFile (filepath + ".unity3d");
						if (assetbundle_mp3 != null) {
							audioClip = assetbundle_mp3.LoadAsset<AudioClip> (audioName);
							if (audioClip != null) {
								Add (audioName, audioClip);
							}
							QueLoadAudio (que, action);
						} else {
							Util.Log ("log.GetLoadAudioEvent error  filepath=" + filepath);
							QueLoadAudio (que, action);
						}
					}
				} else {
					QueLoadAudio (que, action);
				}
			} else {
				action ();
			}
		} else {
			action ();
		}
	}

	/// <summary>
	/// 设置音效属性
	/// </summary>
	/// <param name="parentName"></param>
	/// <param name="audioName"></param>
	/// <param name="bol"></param>
	public void SetAudio (string audioName, string resetaudioName, bool bol)//
	{
		if (audioName == "")
			return;
//		if (audioNameList == null)
//			return;
//
//		if (audioNameList.Count <= 0)
//			return;

		AudioClip audioclip = null;
		AudioSource m_audiosource = null;
		GameObject parentObj = null;
		audioclip = Get (audioName);
		if (audioclip == null)
			return;
		m_audiosource = GetAudioSource (audioName);
		if (m_audiosource == null) {
//			for (int i = 0; i < audioNameList.Count; i++) {
			parentObj = Instantiate (audioPerfab);
			parentObj.name = resetaudioName;
			if (parentObj == null)
				return;
			audioSourcelist.Remove (parentObj);
			audioSourcelist.Add (parentObj);
			m_audiosource = parentObj.GetComponent<AudioSource> ();
			if (null == m_audiosource) {
				m_audiosource = parentObj.AddComponent<AudioSource> ();
			}
			if (m_audiosource == null)
				return;
			m_audiosource.clip = null;
			m_audiosource.Stop ();
			m_audiosource.loop = bol;
			m_audiosource.clip = audioclip;
			AddAudioSource (resetaudioName.ToString (), m_audiosource);
//			}
		}
	}

	/// <summary>
	/// 清除音频
	/// </summary>
	public void ClearAudioSource ()
	{
		if (audioSourcelist != null) {
			if (audioSourcelist.Count > 0) {
				for (int i = 0; i < audioSourcelist.Count; i++) {
					Destroy (audioSourcelist [i].gameObject);
				}
				audioSourcelist.Clear ();
			}
		}
		if (audioSourceDic != null) {
			if (audioSourceDic.Count > 0) {
				audioSourceDic.Clear ();
			}
		}
	}

	/// <summary>
	/// 播放
	/// </summary>
	/// <param name="audioName"></param>
	public void AudioPlay (string audioName)
	{
		if (audioName == "")
			return;
		AudioSource m_audiosource = null;
		m_audiosource = GetAudioSource (audioName);
		if (m_audiosource != null) {
			m_audiosource.volume = m_AudioVolume;
			m_audiosource.Play ();
		}
	}

	/// <summary>
	/// 暂停
	/// </summary>
	/// <param name="audioName"></param>
	void AudioPause (string audioName)
	{
		if (audioName == "")
			return;
		AudioSource m_audiosource = null;
		m_audiosource = GetAudioSource (audioName);
		if (m_audiosource != null) {
			m_audiosource.Pause ();
		}
	}

	/// <summary>
	/// 停止播放
	/// </summary>
	/// <param name="audioName"></param>
	public void AudioStop (string audioName)
	{
		if (audioName == "")
			return;
		AudioSource m_audiosource = null;
		m_audiosource = GetAudioSource (audioName);
		if (m_audiosource != null) {
			m_audiosource.Stop ();
		}
	}

	/// <summary>
	/// 音频的音量控制
	/// </summary>
	/// <param name="value"></param>
	public void AudioVolume (float value)
	{
		m_AudioVolume = value;
		if (audioSourcelist == null)
			return;
		if (audioSourcelist.Count < 0)
			return;
		if (audioSourceDic == null)
			return;
		if (audioSourceDic.Count < 0)
			return;
		string audioName = "";
		AudioSource m_audiosource = null;
		for (int i = 0; i < audioSourcelist.Count; i++) {
			audioName = audioSourcelist [i].gameObject.name.ToString ();
			if (audioName != "") {
				m_audiosource = GetAudioSource (audioName);
				if (m_audiosource != null) {
					m_audiosource.volume = m_AudioVolume;
				}
			}
		}
	}

	public void CloseIe ()
	{
		StopCoroutine ("GetLoadAudio");
	}
}