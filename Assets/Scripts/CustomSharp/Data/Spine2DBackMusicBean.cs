using System;

/// <summary>
/// 2D素材背景音乐配置表
/// </summary>

[Serializable]
public class Spine2DBackMusicBean
{
    #region Property
    //背景音乐名称
    public string name { get; set; }
	//"mouth_open" = 张嘴
	public string trigger { get; set; }
    //该背景音乐是否在触发时被停止；
    //某些素材的 背景音乐 在触发操作时会被停止掉；
    //     比如：2d_s007素材，检测到人脸时会播放 背景音乐；
    //     此时需求要在触发时停止掉 背景音乐，就要将 trigger_stop 设置为true，并且在触发时调用音频管理的停止方法；
    public bool trigger_stop { get; set; }

    #endregion
}
