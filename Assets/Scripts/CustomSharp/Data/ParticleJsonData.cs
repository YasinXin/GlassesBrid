using System;

/// <summary>
/// Particle json data.
/// </summary>
[Serializable]
public class ParticleJsonData
{
	#region Property
	//3d model prefab名称，用于加载动画GameObject
	public string name { get; set; }

	//是否在识别时播放
	public bool visible { get; set; }

	//3D模型父节点，value = "" 挂载到世界坐标上
	public string Node { get; set; }

	//3D模型父节点，value = "" 挂载到世界坐标上
	public string ChildNode { get; set; }

	//是否需要根据张嘴的值进行缩放
	//public bool follow { get; set; }

	//是否需要根据张嘴的值进行缩放
	public bool fxScale { get; set; }

	//该特效绑定的音效名称，字段为null或字符串为""则无绑定音效
	public string sfxName { get; set; }

	//3d model z次序，用于调整model的层级关系，数值越小越靠前；
	public double zOrder { get; set; }

	//触发动作类型 
	//"mouth_open" = 张嘴
	public string trigger { get; set; }

	//该动画素材是否在触发时被停止；
	//某些动画素材在触发操作时会被隐藏掉；
	//     比如：2d_s007素材，张嘴触发，圣诞老人动画播放，从左侧进入。
	//     此时需求要把鲜花头饰隐藏掉，就要在配置文件设置头戴鲜花的此属性为true，并且在触发时进行隐藏设置；
	public bool trigger_stop { get; set; }

	//独立控制模式
	//如果为true开启特效独立控制，在特效上添加FxEvent脚本，在播放时会读取特效Prefab上最外层空ParticleSystem组件的时长、loop等参数进行设置和播放；
	//如果特效为loop则循环播放，特效不为loop则按特效Duration时长来播放，播放完毕后自动停止;
	//
	//如果为false不开启独立控制，将随整体逻辑进行
	//例如需要识别就显示的特效无需开启独立控制模式;
	//
	//组合 
	//1、识别播放，触发不停止，			 有音效 : visible = true  | sfxName = "fire" | trigger = "" 			 | trigger_stop = false | indieControl_mode = false | 特效需为loop
	//2、识别播放，触发停止，还原继续播放，无音效 : visible = true  | sfxName = ""     | trigger = "mouth_open" | trigger_stop = true  | indieControl_mode = false | 特效需为loop
	//3、张嘴播放，闭嘴不停止，			 有音效 : visible = false | sfxName = "fire" | trigger = "mouth_open" | trigger_stop = false | indieControl_mode = true
	//4、张嘴播放，闭嘴停止，			 无音效 : visible = false | sfxName = "" 		| trigger = "mouth_open" | trigger_stop = false | indieControl_mode = false
	public bool indieControl_mode { get; set; }

	#endregion
}