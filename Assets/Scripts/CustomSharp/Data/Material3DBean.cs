using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 3D素材配置文件
/// 对应3d_config.txt内的配置文件
/// </summary>
[Serializable]
public class Material3DBean {
	#region Property
	//3D unity资源文件，通过此文件加载打包的资源；
	//资源以.unity结尾
	public string name { get; set; }

	//该素材是否有需要实例化的资源。
	public bool NeedInstantiateAssets { get; set;}

	//判断模型 face
	public string faceTexture { get; set; }

	//判断模型 face 是否显示
	public bool showface { get; set; }

	//3D动画数据对象列表，配置基本属性、动画事件
	public List<Model3DBean> model { get; set; }

	public List<ParticleJsonData> particlies { get; set;}

	//spine动画数据对象列表，配置动画属性，prefab名称，挂载点，是否可见等
	public List<Spine2DBean> spines { get; set; }

	//spine音效对象列表，配置spine动画对应的音效
	public List<Spine2DAudiosBean> audios { get; set; }

	//素材背景音乐对象，该素材对应的背景音乐
	public List<Spine2DBackMusicBean> backgroundMusic { get; set; }

	//模型动画状态机列表
	public List<Animator> anim { get; set;}

	//提示图片
	public string prompt { get; set; }
	#endregion
}
