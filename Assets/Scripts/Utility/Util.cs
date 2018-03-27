using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using LitJson;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Util
{
	public static bool m_Log=true;

    public static int Int(object o)
    {
        return Convert.ToInt32(o);
    }

    public static float Float(object o)
    {
        return (float)Math.Round(Convert.ToSingle(o), 2);
    }

    public static long Long(object o)
    {
        return Convert.ToInt64(o);
    }

    public static int Random(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static float Random(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static string Uid(string uid)
    {
        int position = uid.LastIndexOf('_');
        return uid.Remove(0, position + 1);
    }

    public static long GetTime()
    {
        TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
        return (long)ts.TotalMilliseconds;
    }

    /// <summary>
    /// 获取当前时间 
    /// </summary>
    /// <returns></returns>
    public static string GetCurrTime()
    {
        return DateTime.Now.ToString("yyyyMMddHHmmssfff");
    }

    /// <summary>
    /// 搜索子物体组件-GameObject版
    /// </summary>
    public static T Get<T>(GameObject go, string subnode) where T : Component
    {
        if (go != null)
        {
            Transform sub = go.transform.FindChild(subnode);
            if (sub != null)
                return sub.GetComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// 搜索子物体组件-Transform版
    /// </summary>
    public static T Get<T>(Transform go, string subnode) where T : Component
    {
        if (go != null)
        {
            Transform sub = go.FindChild(subnode);
            if (sub != null)
                return sub.GetComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// 搜索子物体组件-Component版
    /// </summary>
    public static T Get<T>(Component go, string subnode) where T : Component
    {
        return go.transform.FindChild(subnode).GetComponent<T>();
    }

    /// <summary>
    /// 搜索子物体组件-Transform版
    /// </summary>
    public static T Get<T>(GameObject go) where T : Component
    {
        if (go != null)
        {
            if (go != null)
                return go.GetComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// 添加组件
    /// </summary>
    public static T Add<T>(GameObject go) where T : Component
    {
        if (go != null)
        {
            T[] ts = go.GetComponents<T>();
            for (int i = 0; i < ts.Length; i++)
            {
                if (ts[i] != null)
                    GameObject.Destroy(ts[i]);
            }
            return go.gameObject.AddComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// 添加组件
    /// </summary>
    public static T Add<T>(Transform go) where T : Component
    {
        return Add<T>(go.gameObject);
    }

    public static void AddScriptsComponent(GameObject go, string name)
    {
        if (go == null || name == null || name.Length <= 0)
        {
            return;
        }
        Type type = GetConvertType(name);
		if(type==null)
			return;
        if (go.GetComponent(type) != null)
        {
            return;
        }
        else
        {
            go.AddComponent(type);
        }
    }
public static bool AddScriptsComponentS(GameObject go, string name)
    {
		bool bol = false;
        if (go == null || name == null || name.Length <= 0)
        {
			return bol;
        }
        Type type = GetConvertType(name);
		if(type==null)
			return bol;
        if (go.GetComponent(type) != null)
        {
			return bol;
        }
        else
        {
            go.AddComponent(type);
			bol = true;
			return bol;
        }
    }
    public static void DestroyScriptsComponent(GameObject go, string name)
    {
        if (go == null || name == null || name.Length <= 0)
        {
            return;
        }
        Type type = GetConvertType(name);
		if(type==null)
			return;
        if (go.GetComponent(type) == null)
        {
            return;
        }
        else
        {
            UnityEngine.Object.Destroy(go.GetComponent(type));
        }
    }

    /// <summary>
    /// 查找父节点下所有子对象
    /// </summary>
    /// <param name="check"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static List<Transform> GetAllTransform(Transform check)
    {
        List<Transform> listTran = new List<Transform>();
        foreach (Transform child in check)
        {
            Log(child.name.ToString());
            listTran.Add(child);
        }
        return listTran;
    }

    /// <summary>
    /// 查找子对象
    /// </summary>
    public static GameObject Child(GameObject go, string subnode)
    {
        return Child(go.transform, subnode);
    }

    /// <summary>
    /// 查找子对象
    /// </summary>
    public static GameObject Child(Transform go, string subnode)
    {
        Transform tran = go.FindChild(subnode);
        if (tran == null)
            return null;
        return tran.gameObject;
    }

    /// <summary>
    /// 取平级对象
    /// </summary>
    public static GameObject Peer(GameObject go, string subnode)
    {
        return Peer(go.transform, subnode);
    }

    /// <summary>
    /// 取平级对象
    /// </summary>
    public static GameObject Peer(Transform go, string subnode)
    {
        Transform tran = go.parent.FindChild(subnode);
        if (tran == null)
            return null;
        return tran.gameObject;
    }

    /// <summary>
    /// 计算字符串的MD5值
    /// </summary>
    public static string md5(string source)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
        byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
        md5.Clear();

        string destString = "";
        for (int i = 0; i < md5Data.Length; i++)
        {
            destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
        }
        destString = destString.PadLeft(32, '0');
        return destString;
    }

    /// <summary>
    /// 计算文件的MD5值
    /// </summary>
    public static string md5file(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }

    /// <summary>
    /// 清除所有子节点
    /// </summary>
    public static void ClearChild(Transform go)
    {
        if (go == null)
            return;
        for (int i = go.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(go.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 清理内存
    /// </summary>
    public static void ClearMemory()
    {
        Resources.UnloadUnusedAssets();
		GC.Collect();
    }

    /// <summary>
    /// 应用程序内容路径
    /// </summary>
    public static string AppContentPath()
    {
        string path = string.Empty;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                path = "jar:file://" + Application.dataPath + "!/assets/";
                break;
            case RuntimePlatform.IPhonePlayer:
                path = Application.dataPath + "/Raw/";
                break;
            default:
                path = Application.dataPath + "/StreamingAssets/";
                break;
        }
        return path;
    }

    /// <summary>
    /// 取得数据存放目录
    /// </summary>
    public static string DataPath
    {
        get
        {
            string game = AppConst.AppName.ToLower();
            if (Application.isMobilePlatform)
            {
                return Application.persistentDataPath + "/" + game + "/";
            }
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                int i = Application.dataPath.LastIndexOf('/');
                return Application.dataPath.Substring(0, i + 1) + game + "/";
            }
            return "c:/" + game + "/";
        }
    }

    public static string GetRelativePath()
    {
        if (Application.isMobilePlatform || Application.isConsolePlatform)
            return "file:///" + DataPath;
        else // For standalone player.
             //return "file://" + Application.streamingAssetsPath + "/";
            return "file:///" + DataPath;
    }

    /// <summary>
    /// 平台对应文件夹
    /// </summary>
    /// <returns></returns>
    public static string GetRuntimePlatform()
    {
        string platform = "";
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            platform = "Windows";
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            platform = "Android";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            platform = "IOS";
        }
        else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            platform = "OSX";
        }
        return platform;
    }

    /// <summary>
    /// 判断路径，文件是否存在
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static bool FileIsExistence(string filePath)
    {
        bool bol = false;
        FileInfo t_binskin = new FileInfo(filePath);
        if (t_binskin.Exists)
        {
            bol = true;
        }
        return bol;
    }

    /// <summary>
    /// 判断文件夹是否存在
    /// </summary>
    /// <param name="directorypath"></param>
    /// <returns></returns>
    public static bool DirectoryExistence(string directorypath)
    {
        return Directory.Exists(directorypath);
    }

    /// <summary>
    /// 取得行文本
    /// </summary>
    public static string GetFileText(string path)
    {
        return File.ReadAllText(path);
    }

    /// <summary>
    /// 取得行文本文件返回list列表
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static List<string> GetFileTextVlaue(string str)
    {
        List<string> strList = new List<string>();
        string[] strarr = str.Split("\r"[0]);
        for (int i = 0; i < strarr.Length; i++)
        {
            strList.Add(strarr[i].Trim().ToString());
        }
        return strList;
    }

    /// <summary>
	/// 取得行文本文件返回list列表
	/// </summary>
	/// <param name="filePath"></param>
	/// <returns></returns>
	public static List<int> GetFileTextIntVlaue(string str)
    {
        List<int> strList = new List<int>();
        string[] strarr = str.Split("\n"[0]);
        for (int i = 0; i < strarr.Length; i++)
        {
            strList.Add(int.Parse(strarr[i].Trim().ToString()));
        }
        return strList;
    }

    /// <summary>
    /// 网络可用
    /// </summary>
    public static bool NetAvailable
    {
        get
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }

    /// <summary>
    /// 是否是无线
    /// </summary>
    public static bool IsWifi
    {
        get
        {
            return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        }
    }

    /// <summary>
    /// 根据类名称获取类型
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    public static Type GetConvertType(string typeName)
    {
        if (typeName == null)
        {
            return null;
        }
        return Type.GetType(typeName);
    }

    public static void Log(string str)
    {
		if (m_Log) {
			Debug.Log(GetCurrTime() + " Unity Log  " + str);
		}
    }

    public static void LogWarning(string str)
    {
		if (m_Log) {
			Debug.LogWarning (GetCurrTime () + " Unity Log " + str);
		}
    }

    public static void LogError(string str)
    {
		if (m_Log) {
			Debug.LogError (GetCurrTime () + " Unity Log  " + str);
		}
    }

    /// <summary>
    /// 防止初学者不按步骤来操作
    /// </summary>
    /// <returns></returns>
    public static int CheckRuntimeFile()
    {
        if (!Application.isEditor)
            return 0;
        string streamDir = Application.dataPath + "/StreamingAssets/";
        if (!Directory.Exists(streamDir))
        {
            return -1;
        }
        else {
            string[] files = Directory.GetFiles(streamDir);
            if (files.Length == 0)
                return -1;

            if (!File.Exists(streamDir + "files.txt"))
            {
                return -1;
            }
        }
        return 0;
    }

    /// <summary>
    /// 检查运行环境
    /// </summary>
    public static bool CheckEnvironment()
    {
#if UNITY_EDITOR
        int resultId = Util.CheckRuntimeFile();
        if (resultId == -1)
        {
            Debug.LogError("没有找到框架所需要的资源，单击Game菜单下Build xxx Resource生成！！");
            EditorApplication.isPlaying = false;
            return false;
        }
#endif
        return true;
    }

    /// <summary>
    /// 保存JPG图片
    /// </summary>
    /// <param name="t2d">T2d.</param>
    /// <param name="ImagePaths">Image Path.</param>
    public static void SaveJpgTexturet(Texture2D t2d, string ImagePaths)
    {
        byte[] bytes = t2d.EncodeToJPG();
        File.WriteAllBytes(ImagePaths, bytes);
    }

    /// <summary>
    /// 保存PNG图片
    /// </summary>
    /// <param name="t2d">T2d.</param>
    /// <param name="ImagePaths">Image Path.</param>
    public static void SavePngTexturet(Texture2D t2d, string ImagePaths)
    {
        byte[] bytes = t2d.EncodeToPNG();
        File.WriteAllBytes(ImagePaths, bytes);
    }

    public static Color32[] ByPathReadPic(string ImagePath)
    {
        if (null==ImagePath)
        {
            return null;
        }
        //创建文件读取流
        FileStream fileStream = new FileStream(ImagePath, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        //创建文件长度缓冲区
        byte[] bytes = new byte[fileStream.Length];
        //读取文件
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        //释放文件读取流
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        //创建Texture
        
        Texture2D texture = new Texture2D(1280,720);
        texture.LoadImage(bytes);
        return texture.GetPixels32();
    }
    public static string getPic()
    {
        string[] s= Directory.GetFiles(Application.persistentDataPath);
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i].IndexOf(".png")>=0)
            {
                return s[i];
            }
        }
        return null;
        
    }

    /// <summary>
    /// 遍历目录
    /// </summary>
    public static List<string> Recursive(string path)
    {
        List<string> modelfiles = new List<string>();
        string[] dirs = Directory.GetDirectories(path);
        foreach (string dir in dirs)
        {
            string[] headPath = dir.Split('/');
            if (headPath == null)
                continue;
            if (headPath.Length <= 0)
                continue;
            //根据路径取得角色名
            string characterName = headPath[headPath.Length - 1];
            modelfiles.Add(characterName);
        }
        return modelfiles;
    }

    #region Json and XML

    /// <summary>
    /// 通过path解析Json，返回一个数据
    /// </summary>
    /// <typeparam name="T">获取的Json类型</typeparam>
    /// <param name="path">目标Json路径</param>
    /// <param name="isFormWWW">是否解析WWW上的Json</param>
    /// <returns>返回T类型数据</returns>
    public static T GetJsonData<T>(string path, bool isFormWWW = false)
    {
        if (path == null)
            Log("GetJsonData : Json path is null");

        T jsonData = default(T);
        if (!isFormWWW)
            jsonData = JsonMapper.ToObject<T>(File.ReadAllText(path));
        else
            jsonData = JsonMapper.ToObject<T>(path);

        if (jsonData == null)
            Log("GetJsonData : Json parsing fail");

        return jsonData;
    }

    /// <summary>
    /// 通过path解析Json数据并返回一个List集合
    /// </summary>
    /// <typeparam name="T">获取的Json类型</typeparam>
    /// <param name="path">目标Json路径</param>
    /// <param name="isFormWWW">是否解析WWW上的Json</param>
    /// <returns>返回T类型数据集合</returns>
    public static List<T> GetJsonDataList<T>(string path, bool isFormWWW = false)
    {
        if (path == null)
            Log("GetJsonDataList : Json path is null");

        List<T> tList = new List<T>();
        if (!isFormWWW)
            tList = JsonMapper.ToObject<List<T>>(File.ReadAllText(path));
        else
            tList = JsonMapper.ToObject<List<T>>(path);

        if (tList == null)
            Log("GetJsonDataList : Json parsing fail");

        return tList;
    }
    #endregion

    /// <summary>
    /// color32 array to byte array
    /// </summary>
    /// <param name="colors"></param>
    /// <returns></returns>
    public static byte[] Color32ArrayToByteArray(Color32[] colors)
    {
        if (colors == null || colors.Length == 0)
            return null;

        int length = Marshal.SizeOf(typeof(Color32)) * colors.Length;
        byte[] bytes = new byte[length];

        GCHandle handle = default(GCHandle);
        try
        {
            handle = GCHandle.Alloc(colors, GCHandleType.Pinned);
            Marshal.Copy(handle.AddrOfPinnedObject(), bytes, 0, length);
        }
        finally
        {
            if (handle != default(GCHandle))
                handle.Free();
        }

        return bytes;
    }

	/// <summary>
	/// color32 array to byte array
	/// </summary>
	/// <param name="colors"></param>
	/// <returns></returns>
	public static byte[] Color32ArrayToByteArray(Color32[] colors,int length,byte[] bytes,GCHandle handle)
	{
		if (colors == null || colors.Length == 0)
			return null;

		//		int length = Marshal.SizeOf(typeof(Color32)) * colors.Length;
		//		byte[] bytes = new byte[length];
		//
		//		GCHandle handle = default(GCHandle);
		try
		{
            
			handle = GCHandle.Alloc(colors, GCHandleType.Pinned);
			Marshal.Copy(handle.AddrOfPinnedObject(), bytes, 0, length);
		}
		finally
		{
			if (handle != default(GCHandle))
				handle.Free();
		}

		return bytes;
	}

    public static byte[] Color32ArrayToByteArray(byte[] colors, int length, byte[] bytes, GCHandle handle)
    {
        if (colors == null || colors.Length == 0)
            return null;

        //		int length = Marshal.SizeOf(typeof(Color32)) * colors.Length;
        //		byte[] bytes = new byte[length];
        //
        //		GCHandle handle = default(GCHandle);
        try
        {

            handle = GCHandle.Alloc(colors, GCHandleType.Pinned);
            Marshal.Copy(handle.AddrOfPinnedObject(), bytes, 0, length);
        }
        finally
        {
            if (handle != default(GCHandle))
                handle.Free();
        }

        return bytes;
    }

    //	Copy (IntPtr source, float[] destination, int startIndex, int length);

    public static float[] ByteArrayToFloatArray(byte[] by)
	{
		if (by == null || by.Length == 0)
			return null;

		int length = Marshal.SizeOf(typeof(float)) * by.Length;
		float[] f = new float[length];

		GCHandle handle = default(GCHandle);
		try
		{
			handle = GCHandle.Alloc(by, GCHandleType.Pinned);
			Marshal.Copy(handle.AddrOfPinnedObject(), f, 0, length);
		}
		finally
		{
			if (handle != default(GCHandle))
				handle.Free();
		}

		return f;
	}
    /// <summary>
    /// 文件删除
    /// </summary>
    /// <param name="path">Path.</param>
    public static void DelectFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

	/// <summary>
	/// 文件拷贝
	/// </summary>
	/// <param name="fullName">文件名称</param>
	/// <param name="resPath">目标路径.</param>
	/// <param name="originPath">原始路径</param>
	public static void OnCopyFile(string fullName,string resPath,string originPath)
	{
		string fileName = Path.GetFileName (fullName);
		string targetPath = Path.Combine (resPath, fileName);
		string sourcePath = Path.Combine (originPath, fileName);
		File.Copy (sourcePath, targetPath, true);
	}
}