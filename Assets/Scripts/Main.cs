
using System;
using UnityEngine;
/// <summary>
/// </summary>
/// 
public class Main : Manager
{
    public static bool openFilter;
    public static bool openBuffing;
    [Tooltip("是否开启滤镜")]
    public bool openfilter;
    [Tooltip("是否开启磨皮")]
    public bool openbuffing;

    [Tooltip("Unity版本")]
    public static string unityVersion;
    public string unityversion = "Unity_E_V0.0";

    void Awake()
    {
        openFilter = openfilter;
        openBuffing = openbuffing;
        unityVersion = unityversion;
    }

    void Start()
    {
        try
        {
            AppFacade.Instance.StartUp();   //启动游戏
        }
        catch (ArgumentNullException e)
        {
            Logout.Log("Unity初始化异常  ArgumentNullException: => " + e);
        }
        catch (Exception e)
        {
            Logout.Log("Unity初始化异常  Exception: => " + e);
        }

        //Logout.Log("xyh测试！");
    }
    
}