using System;

/// <summary>
/// 2D spine动画数据模型
/// 用于配置2D spine动画属性
/// </summary>
[Serializable]
public class Spine2DBean
{
    #region Property
    //2d spine prefab名称，用于加载spine动画GameObject
    public string name { get; set; }

    //挂载点的位置。人脸检测的点，参考挂点图.jpg
	public string position { get; set; }

    //在初始化时 是否为可见
    public bool visible { get; set; }

    //该动画文件是否重复播放，如果重复播放将采用Loop播放
    public bool repeat { get; set; }

    //2d spine z次序，用于调整spine的层级关系，数值越小越靠前；
	public double zOrder { get; set; }

    //触发动作类型 
    //"mouth_open" = 张嘴
    public string trigger { get; set; }

    //该动画素材是否在触发时被停止；
    //某些动画素材在触发操作时会被隐藏掉；
    //     比如：2d_s007素材，张嘴触发，圣诞老人动画播放，从左侧进入。
    //     此时需求要把鲜花头饰隐藏掉，就要在配置文件设置头戴鲜花的此属性为true，并且在触发时进行隐藏设置；
    public bool trigger_stop { get; set; }

    // 触发顺序，动画触发的先后顺序？ 暂时未用到此属性
    public int trigger_order { get; set; }

	//多人脸支持，对于贴图部分最高支持5人（根据性能调整），即该字段值最高为"1,2,3,4,5"
	//*注：当该贴图为前景，即"position"字段值为3个时，无"multimode"字段
	public string multimode{ get; set;}
    #endregion
}
