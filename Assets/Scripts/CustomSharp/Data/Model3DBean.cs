using System;
using System.Collections.Generic;

/// <summary>
/// 动画事件数据模型
/// </summary>
[Serializable]
public class LDAnimationClipEventData
{
	//事件方法名
	public string functionName{ get; set;}
	//事件参数类型
	public string functionParameType{ get; set;}
	//事件参数值
	public string functionParameValue{ get; set;}
	//事件开始时间
	public double startTime{ get; set;}
}

/// <summary>
/// 动画状态机播放条件
/// </summary>
[Serializable]
public class LDAnimationConditionData
{	
	//条件名称
	public string conditionsName{ get; set;}
	//条件类型
	public string conditionsType{ get; set;}
	//条件值
	public string conditionsValue{ get; set;}
}

/// <summary>
/// 动画信息数据模型
/// </summary>
[Serializable]
public class LDAnimationClipInfo
{
	/// <summary>
	/// 动画状态机中动画片段ID
	/// default动画ID = Length, default动画下一个条件ID为0
	/// </summary>
	/// <value>The animation clip I.</value>
	public int animClipIndex { get; set;}
	//动画片段播放速度
	public double speed{ get; set;}
	//动画事件数据模型
	public List<LDAnimationClipEventData> animConfigs{ get; set;}

	public List<LDAnimationConditionData> animConditions{ get; set;}
}

/// <summary>
/// 2D spine动画数据模型
/// 用于配置2D spine动画属性
/// </summary>
[Serializable]
public class Model3DBean
{
	#region Property
	//3d model prefab名称，用于加载动画GameObject
	public string name { get; set; }

	//在初始化时 是否为可见
	public bool visible { get; set; }

	//3D模型父节点，value = "" 挂载到世界坐标上
	public string Node { get; set; }

	//	//3D模型子节点。例：模型B为子模型要挂到模型A下边的第三级的某一个物体上，这个就是这里需要填子模型名字。当然Node也需要写上模型A的名字，因为需要A模型
	public string ChildNode { get; set; }

	//模型是否跟随检测运动
	public bool follow { get; set; }

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

	//该模型下所有动画片段信息列表
	public List<LDAnimationClipInfo> animations{ get; set;}

	#endregion
}
