using System;
/// <summary>
/// 2D素材形变shader配置信息
/// </summary>
[Serializable]
public class MorphFilterBean{
	#region Property
	////形变名称，用于调取形变shader参数
	public string name { get; set; }

	//多人脸支持，没有"mutimode"字段时默认该部分只支持单人
	//在形变部分中，"multimode"字段只支持"1""2"和"1,2"三种，即最大支持两人使用不同形变效果
	//当屏幕中出现人数小于2人时，则只展示第一个人的形变，当屏幕中出现人数大于2人时，第2个人之后的人脸不做处理
	public string multimode{ get; set;}
	#endregion
}