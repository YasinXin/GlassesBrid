using UnityEngine;

public class FxEvent : Manager
{
	public ParticleSystem _ps;
	public ParticleSystem Ps
	{
		get
		{
			if (_ps == null) {
				_ps = GetComponent<ParticleSystem> ();
			}
			return _ps;
		}
	}

	public float duraition
	{
		get
		{
			return Ps.duration;
		}
	}

	public bool isLoop
	{
		get
		{
			return Ps.loop;
		}
	}

	//public bool isLoop = true;

	//要播放的音频组件，需要再特效资源初始化时候从Json中获取
	public string sfxName = "";

	/// <summary>
	/// Fx完成事件，完成后，才可以继续播放特效，完成后隐藏该特效，用于触发Disable和Enable
	/// </summary>
	void OnComplete()
	{
		MaterialsMgr.m_fxCompleteFlag = true;
		gameObject.SetActive (false);
	}

	/// <summary>
	/// 在启动时开始计时并播放音效
	/// 在计时完成时调用完成回调
	/// </summary>
	void OnEnable()
	{
		//开启如果是不循环 开始计时 
		//播放声音
		if (!isLoop) 
		{
			if (duraition > 0)
				Invoke("OnComplete", 5);
		}

		if (!string.IsNullOrEmpty(sfxName)) 
			AudioMgr.AudioPlay (sfxName);
	}

	/// <summary>
	/// 在禁用组件时停止音效播放，停止Invoke计时
	/// </summary>
	void OnDisable()
	{
		AudioMgr.AudioStop (sfxName);
		CancelInvoke ();
	}	

}
