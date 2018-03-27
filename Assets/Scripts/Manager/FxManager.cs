using System.Collections.Generic;
using UnityEngine;

public class FxManager : Manager
{
	private Dictionary<string, ParticleSystem> fxParticleDict = new Dictionary<string, ParticleSystem> ();

	/// <summary>
	/// 将特效添加到词典
	/// </summary>
	/// <param name="_names">Names.</param>
	public void LoadFx(string[] _names)
	{
		if (_names == null && _names.Length > 0) {
			Util.Log ("LoadFx _names is null or length lessEqule 0");
			return;
		}

		for (int i = 0; i < _names.Length; i++) 
		{
			if (_names[i] != null) 
			{
				GameObject go = MaterialsMgr.GetObject (_names[i]);
				ParticleSystem p = go.GetComponent<ParticleSystem> ();
				if (p != null) {
					p.Stop ();
					p.gameObject.SetActive (false);
					//TODO set particle parent , if it has parentNode
					//p.transform.parent = this.transform.parent;
					if (!fxParticleDict.ContainsKey(_names[i])) {
						fxParticleDict.Add (go.name, p);
					}
				}
				else 
				{
					Util.Log ("LoadFx p is null");
				}
			}
		}
	}

	/// <summary>
	/// 将特效移除词典，在删除贴纸素材后调用；
	/// </summary>
	/// <param name="_key">Key.</param>
	public void RemoveFxByDict(string _key)
	{
		if (string.IsNullOrEmpty(_key))
		{
			Util.LogError ("RemoveFxByDict _key is null !");
			return;
		}

		if (fxParticleDict.ContainsKey(_key))
		{
			Util.Log (" ==== Fx = " + _key + "    is removed form fxParticleDict ! ");
			fxParticleDict.Remove (_key);
		}
	}

	/// <summary>
	/// 播放特效
	/// </summary>
	/// <param name="_name">Name.</param>
	public void PlayFx(string _name)
	{
		ParticleSystem p = fxParticleDict.TryGet (_name);

		if (p) 
		{
			p.gameObject.SetActive (true);
			p.Play ();
		} 
		else 
		{
			Util.Log ("PlayFx P is null");
		}
	}

	/// <summary>
	/// 暂停特效
	/// </summary>
	/// <param name="_name">Name.</param>
	public void PauseFx(string _name)
	{
		ParticleSystem p = fxParticleDict.TryGet (_name);

		if (p) 
		{
			//p.gameObject.SetActive (true);
			if (p.isPlaying) 
				p.Pause ();
		} 
		else 
		{
			Util.Log ("PauseFx P is null");
		}
	}

	/// <summary>
	/// 停止特效
	/// </summary>
	/// <param name="_name">Name.</param>
	public void StopFx(string _name)
	{
		ParticleSystem p = fxParticleDict.TryGet (_name);

		if (p) 
		{
			//Debug.Log ("FxManager StopFx = " + _name);
			if (p.isPlaying) 
			{
				//Debug.Log ("FxManager StopFx SetActive= " + p.name);
				p.gameObject.SetActive (false);
				p.Stop ();
			}
		}
		else
		{
			Util.Log ("StopFx P is null " + _name);
		}
	}
}