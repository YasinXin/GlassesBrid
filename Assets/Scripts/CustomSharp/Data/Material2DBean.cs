using System.Collections.Generic;
using System;

/// <summary>
/// 2D素材配置文件
/// 对应2d_config.txt内的配置文件
/// </summary>
[Serializable]
public class Material2DBean
{
    #region Property
    //2D unity资源文件，通过此文件加载打包的资源；
    //资源以.unity结尾
    public string name { get; set; }

	// 形变滤镜
	public MorphFilterBean deformation{ get; set; }

    //spine动画数据对象列表，配置动画属性，prefab名称，挂载点，是否可见等
    public List<Spine2DBean> spines { get; set; }

    //spine音效对象列表，配置spine动画对应的音效
    public List<Spine2DAudiosBean> audios { get; set; }

    //素材背景音乐对象，该素材对应的背景音乐
    public Spine2DBackMusicBean backgroundMusic { get; set; }

    //提示图片
    public string prompt { get; set; }

    #endregion
}
