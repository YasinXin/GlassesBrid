using UnityEngine;
using System;
public class DeviceInfo
{
    //添加设备白名单
    private static string[] use720PhoneStyles = {
            "Xiaomi MI 5","Xiaomi MI 2S","Xiaomi MI NOTE LTE",
            "EVA-AL00","EVA-AL10","EVA-TL00","EVA-DL00","EVA-CL00",
            "FRD-AL10",
            "KNT-AL20",
            "MHA-AL00",
            "MX5",
            "PLK-AL10",
            "vivo vivo X6S A"
    };
     
    /// <summary>
    /// 判断当前设备是否存在白名单中
    /// </summary>
    /// <returns></returns>
    public static bool GetDeviceModel()
    {
		bool device = true;
        string model = SystemInfo.deviceModel;
		if (Array.Exists(use720PhoneStyles, element => element.Equals(model)))
		{
			device = true;
		}
        
      	return device;
    }
    /*
    //指定输出文本框
    public UnityEngine.UI.Text messageText;
    //存储临时字符串
    System.Text.StringBuilder info = new System.Text.StringBuilder();
    // Use this for initialization
    void Start()
    {
        //将输出文本框置空
        messageText.text = "";
        info.AppendLine("设备与系统信息:");
        //设备的模型
        GetMessage("设备模型", SystemInfo.deviceModel);
        //设备的名称
        GetMessage("设备名称", SystemInfo.deviceName);
        //设备的类型
        GetMessage("设备类型（PC电脑，掌上型）", SystemInfo.deviceType.ToString());
        //系统内存大小
        GetMessage("系统内存大小MB", SystemInfo.systemMemorySize.ToString());
        //操作系统
        GetMessage("操作系统", SystemInfo.operatingSystem);
        //设备的唯一标识符
        GetMessage("设备唯一标识符", SystemInfo.deviceUniqueIdentifier);
        //显卡设备标识ID
        GetMessage("显卡ID", SystemInfo.graphicsDeviceID.ToString());
        //显卡名称
        GetMessage("显卡名称", SystemInfo.graphicsDeviceName);
        //显卡类型
        GetMessage("显卡类型", SystemInfo.graphicsDeviceType.ToString());
        //显卡供应商
        GetMessage("显卡供应商", SystemInfo.graphicsDeviceVendor);
        //显卡供应唯一ID
        GetMessage("显卡供应唯一ID", SystemInfo.graphicsDeviceVendorID.ToString());
        //显卡版本号
        GetMessage("显卡版本号", SystemInfo.graphicsDeviceVersion);
        //显卡内存大小
        GetMessage("显存大小MB", SystemInfo.graphicsMemorySize.ToString());
        //显卡是否支持多线程渲染
        GetMessage("显卡是否支持多线程渲染", SystemInfo.graphicsMultiThreaded.ToString());
        //支持的渲染目标数量
        GetMessage("支持的渲染目标数量", SystemInfo.supportedRenderTargetCount.ToString());
        //输出
        messageText.text = info.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        //退出
        if (Input.GetKeyUp("escape"))
        {
            if (Input.GetKeyUp("escape"))
            {
                Application.Quit();
            }
        }
    }

    void GetMessage(params string[] str)
    {
        if (str.Length == 2)
        {
            info.AppendLine(str[0] + ":" + str[1]);
        }
    }*/
}