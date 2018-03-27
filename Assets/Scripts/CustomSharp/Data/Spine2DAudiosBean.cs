using System;

/// <summary>
/// 2D素材音效配置信息
/// </summary>
[Serializable]
public class Spine2DAudiosBean
{
	#region Property
	//2d音效名称
	public string name { get; set; }

	//是否在识别时播放
	public bool visible { get; set; }

	//是否重复播放
	public bool repeat { get; set; }

	//触发动作类型 
	//"mouth_open" = 张嘴
	public string trigger { get; set; }

	//该音效是否在触发时被停止；
	//某些素材的 音效 在触发操作时会被停止掉；
	//     比如：2d_s007素材，检测到人脸时会播放 一个圣诞老人说话的音效；
	//     此时需求要在触发时停止掉 这个音效，就要将 trigger_stop 设置为true，并且在触发时调用音频管理的停止方法；
	public bool trigger_stop { get; set; }
	//多人支持
	public string multimode{ get; set;}
	#endregion
}
