/** 
 *Copyright(C) 2015 by #COMPANY# 
 *All rights reserved. 
 *FileName:     #SCRIPTFULLNAME# 
 *Author:       #AUTHOR# 
 *Version:      #VERSION# 
 *UnityVersion：#UNITYVERSION# 
 *Date:         #DATE# 
 *Description:    
 *History: 
*/  
using System.Collections.Generic;
using UnityEngine;
public class Constants{
	/**
	 * 人脸检测图标
	 */
	public static string TRACK_ICON = "track_icon";
    /******************* 2D触发名称 *******************/
    /**
	 * open mouth张嘴
	 */
    public static string OPEN_MOUTH = "mouth_open";
	public static string CLOSE_MOUTH = "mouth_close";

    /**
    * head up 抬头
    */
    public static string UP_HEAD = "head_up";
    /**
   * head around 摇头
   */
    public static string AROUND_HEAD = "head_around";
	//点击屏幕
	public static string CLICK_SCREEN = "click_Screen";
	//长按屏幕
	public static string HOLDDOWN_SCREEN = "holdDown_Screen";
	/// <summary>
	/// 储存固定点的词典
	/// </summary>
	private static Dictionary<int, Vector2> fixedPointDict = new Dictionary<int, Vector2> ();

	/// <summary>
	/// 屏幕坐标点
	/// </summary>
	public static void SetFixedPointData()
	{
		fixedPointDict.Add (11, new Vector2(0, Screen.height));
		fixedPointDict.Add (12, new Vector2(Screen.width * 0.5f, Screen.height));
		fixedPointDict.Add (13, new Vector2(Screen.width, Screen.height));
		fixedPointDict.Add (14, new Vector2(0, 0));
		fixedPointDict.Add (15, new Vector2(Screen.width * 0.5f, 0));
		fixedPointDict.Add (16, new Vector2(Screen.width, 0));
		//新增 --kaikai
		fixedPointDict.Add (17, new Vector2(0, Screen.height*0.9057971f));
		fixedPointDict.Add (18, new Vector2(0, Screen.height*0.3432971f));
		fixedPointDict.Add (19, new Vector2(0, Screen.height*0.25f));

		fixedPointDict.Add (20, new Vector2(Screen.width, Screen.height*0.9057971f));
		fixedPointDict.Add (21, new Vector2(Screen.width, Screen.height*0.3432971f));
		fixedPointDict.Add (22, new Vector2(Screen.width, Screen.height*0.25f));
	}

	/// <summary>
	/// Gets the fixed point.
	/// </summary>
	/// <returns>The fixed point.</returns>
	/// <param name="_key">Key.</param>
	public static Vector3 GetFixedPoint(int _key, float _zOder)
	{
		Vector3 v3 = Vector3.zero;

		if (fixedPointDict.ContainsKey(_key))
		{
			v3.x = fixedPointDict [_key].x;
			v3.y = fixedPointDict [_key].y;
			v3.z = _zOder;
			return v3;
		}
		else 
		{
			Debug.Log ("GetFixedPoint fixed Point not exist !");
		}
		return v3;
	}

	/// <summary>
	/// Determines whether this instance is contains fixed point the specified _key.
	/// </summary>
	/// <returns><c>true</c> if this instance is contains fixed point the specified _key; otherwise, <c>false</c>.</returns>
	/// <param name="_key">Key.</param>
	public static bool IsContainsFixedPoint(int _key)
	{
		return fixedPointDict.ContainsKey (_key);
	}
}