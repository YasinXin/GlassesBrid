using UnityEngine;
using System.Collections.Generic;
using UObject = UnityEngine.Object;
using System;
using System.Collections;

public class MaterialsManager : Manager
{
    //Spine素材json类型
    [HideInInspector]//设置公有对象属性面板不显示
    public Material2DBean m_material2DBean;
    //Model素材json类型
    [HideInInspector]//设置公有对象属性面板不显示
    public Material3DBean m_material3DBean;

    [HideInInspector]//设置公有对象属性面板不显示 ,key 为diyype,懒人模式的key设置为diyType
    public Dictionary<string, Material3DBean> material3DBeanList = new Dictionary<string, Material3DBean>();
    [HideInInspector]
    public Dictionary<string, Material2DBean> material2DBeanList = new Dictionary<string, Material2DBean>();

    //根据挂载位置，存储到相应的分类字典中
    public static Dictionary<string, Dictionary<string, GameObject>> classifyDic = new Dictionary<string, Dictionary<string, GameObject>>();
    //人脸模型数组
    public List<GameObject> faceList = new List<GameObject>();

#if UNITY_EDITOR
    GameObject m_promptParent;
#endif
    //模型网格
    MeshFilter mfFace;
    MeshRenderer mrFace;
    //用来存放结果的矩阵，axb的结果为a的行数和b的列数
    Matrix4x4 result = Matrix4x4.zero;
    //基准矩阵
    Matrix4x4 size_fit_matrix = Matrix4x4.zero;
    //sdk中矩阵值
    Matrix4x4 matr = Matrix4x4.zero;
    //模型网格定点数组
    Vector3[] v_index;
    //fbx对应obj模型网格索引列表
    List<int> m_listIndex = new List<int>();
    //摄像机点原始默认orthographicSize  值
    float m_originalOrthographicSize = 1.0f;
    //摄像机点当前orthographicSize  值
    float m_newOrthographicSize = 1.0f;
    //模型缩放值
    Vector3 vScale = Vector3.zero;
    //模型根据屏幕自适应计算出的缩放因子
    float m_screneScaleFactor = 0.0f;
    FileStreamRead fs = null;
    /// <summary>
    /// 屏幕当前宽度值
    /// </summary>
    float m_screenWidth = 0.0f;
    /// <summary>
    /// 背景版比值
    /// </summary>
    float m_ResolutionSize = 0.0f;

    //特效播放标志
    public bool m_fxCompleteFlag = true;
    /// <summary>
    /// 素材.unity名称
    /// </summary>
    static string manifestName = "";

    /// <summary>
    /// 获取一个GameObject
    /// </summary>
    public GameObject GetObject(string key)
    {
        return GetObjFromClassifyDic(ReceiveMgsManager.Localtion, key);
    }

    /// <summary>
    /// 添加元素到分类字典
    /// </summary>
    /// <param name="value"></param>
    void AddclassifyDic(GameObject value)
    {
        Dictionary<string, GameObject> dic = null;

        if (!string.IsNullOrEmpty(ReceiveMgsManager.Localtion) && value != null)
        {
            if (classifyDic.ContainsKey(ReceiveMgsManager.Localtion))
            {
                dic = classifyDic[ReceiveMgsManager.Localtion];

                if (!dic.ContainsKey(value.name))
                {
                    dic.Add(value.name, value);
                }
                else
                {
                    dic[value.name] = value;
                }
                classifyDic[ReceiveMgsManager.Localtion] = dic;
            }
            else
            {
                dic = new Dictionary<string, GameObject>();
                dic.Add(value.name, value);
                classifyDic.Add(ReceiveMgsManager.Localtion, dic);
            }
        }
        //Util.Log("AddclassifyDic => " + ReceiveMgsManager.Localtion  + "      " + dic.Count + "      " + value);
    }

    /// <summary>
    /// 从分类字典获取元素
    /// categroy 字典类名
    /// </summary>
    public GameObject GetObjFromClassifyDic(string categroy, string key)
    {
        if (!classifyDic.ContainsKey(categroy))
        {
            return null;
        }

        if (classifyDic[categroy] == null)
        {
            return null;
        }

        Dictionary<string, GameObject> dic = classifyDic[categroy];
        if (dic == null || dic.Count <= 0)
        {
            return null;
        }
        var enu = dic.GetEnumerator();
        while (enu.MoveNext())
        {
            string name = enu.Current.Key;
            if (!string.IsNullOrEmpty(name) && name == key)
            {
                GameObject child = enu.Current.Value;
                if (child == null)
                {
                    return null;
                }
                return child;
            }
        }
        return null;
    }

    public void OnAwakeUp()
    {
        fs = new FileStreamRead();
        //获取正交相机大小
        GetCameraSize();
#if UNITY_EDITOR
        m_promptParent = GameObject.Find("StaticCanvas");
#endif
        TextAsset rs1 = Resources.Load("Index_Obj", typeof(TextAsset)) as TextAsset;
        m_listIndex = Util.GetFileTextIntVlaue(rs1.text.ToString());
        v_index = new Vector3[m_listIndex.Count];
        CheckIntoDataInfo();
        OnInitFace();
    }

    /// <summary>
    /// 获取正交相机大小
    /// </summary>
    void GetCameraSize()
    {
        Camera _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        if (_camera == null)
        {
            Util.LogError("InitMaterialsManager OnAwakeUp=>Main Camera not found!");
            return;
        }
        m_originalOrthographicSize = _camera.orthographicSize;
        //m_screneScaleFactor = ((float)Screen.width / (float)Screen.height);
        m_screneScaleFactor = ((float)1080 / (float)1920);
        //计算当前屏幕宽度
        m_screenWidth = m_screneScaleFactor * m_originalOrthographicSize * 2f;
    }

    /// <summary>
    /// 切换分辨率后重新计算初始化数据
    /// </summary>
    public void CheckIntoDataInfo()
    {
        m_newOrthographicSize = m_screenWidth * WebCamera.m_Resolution.x / WebCamera.m_Resolution.y * 0.5f;

        m_ResolutionSize = m_screneScaleFactor / (WebCamera.m_Resolution.y / WebCamera.m_Resolution.x);
        m_ResolutionSize = m_ResolutionSize * 1.2f;
        GetInitMatrix4x4(m_newOrthographicSize);
    }


    /// <summary>
    /// 实例化初始默认人脸模型
    /// </summary>
    void OnInitFace()
    {
        GameObject obj = OnInitLoadResources("face", "face");
        faceList.Add(obj);

        GameObject obj_ = OnInitLoadResources("face", "face_");
        faceList.Add(obj_);
        //GameObject currtenGo = Instantiate(obj) as GameObject;
        //currtenGo.name = name.ToString();
        //currtenGo.name = "face";
        //currtenGo.layer = LayerMask.NameToLayer("Default");
        //currtenGo.SetActive(false);
        GameObject obj2 = OnInitLoadResources("face", "face");
        faceList.Add(obj2);

        if (obj != null)
        {

            mfFace = Util.Get<MeshFilter>(obj, "mean_seamed");
            mrFace = Util.Get<MeshRenderer>(obj, "mean_seamed");

        }
    }

    /// <summary>
    /// 实例化资源文件夹中文件
    /// </summary>
    GameObject OnInitLoadResources(string strName, string name)
    {
        GameObject currtenGo = null;
        GameObject prefab = Resources.Load(strName, typeof(GameObject)) as GameObject;
        if (prefab != null)
        {
            currtenGo = Instantiate(prefab) as GameObject;
            currtenGo.name = name.ToString();
            currtenGo.layer = LayerMask.NameToLayer("Default");
            currtenGo.SetActive(false);
            //AddObject(currtenGo.name.ToString(), currtenGo);
            AddclassifyDic(currtenGo);
            Util.Log("InitMaterialsManager OnInitFace=> Instantiate Success! ===>" + prefab.name.ToString());
        }
        return currtenGo;
    }
    /// <summary>
	/// 新建Spine
	/// </summary>
	/// <param name="name">名称</param>
	public void OnInitSpineManager(string paths, string abName, string[] assetName, Action<bool, bool> acton)
    {
        //判断不存在场景中素材是否存在于预设中
        List<string> newPrefabList = new List<string>();
        for (int i = 0; i < assetName.Length; i++)
        {
            if (assetName[i] != "")
            {
                if (!GetObjFromClassifyDic(ReceiveMgsManager.Localtion, assetName[i]))
                {
                    newPrefabList.Add(assetName[i]);                  
                }
            }
        }
        if (newPrefabList.Count == 0)
        {
            acton(true, true);
        }
        else
        {
            Util.Log("=====>ResLuaMgr.Initialize<=====");
            manifestName = abName.Replace(AppConst.ExtName, "");

            AssetBundle assetbundle_asset = null, assetbundle_manifest = null;
            bool bol = true;
            string path = paths.Replace("file://", "");
            List<Material> materlist = new List<Material>();
            
            assetbundle_manifest = AssetBundle.LoadFromFile(path + "/" + manifestName);
            if(assetbundle_manifest == null)
            {
                acton(true, false);
                return;
            }
            assetbundle_asset = AssetBundle.LoadFromFile(path + "/" + manifestName + ".unity3d");
            Util.Log("url: " + path + "/" + manifestName + ".unity3d");
            if (assetbundle_asset == null)
            {
                acton(true, false);
                return;
            }
            else
            {
                Material[] mats = assetbundle_asset.LoadAllAssets<Material>();

                Util.Log("assetbundle_asset~~~");
                //AddListObj(manifestName, newPrefabList);
                GameObject prefab = null;
                string prefabName = "";
                for (int i = 0; i < newPrefabList.Count; i++)
                {
                    Util.Log("newPrefabList~~~");
                    prefabName = newPrefabList[i].Split('@')[0].ToString();
                    prefab = assetbundle_asset.LoadAsset<GameObject>(prefabName);
                    if (prefab != null)
                    {
                        Util.Log("prefab~~~");
                        materlist.Clear();
                        if (prefab.name.ToString().Contains("2d_"))
                        {
                            for (int j = 0; j < mats.Length; j++)
                            {
                                if (mats[j].name.ToString().Contains(prefab.name.ToString() + "_"))
                                {
                                    materlist.Add(mats[j]);
                                }
                            }
                            //							foreach (Material item in mats) {
                            //								if (item.name.ToString ().Contains (prefab.name.ToString () + "_")) {
                            //									materlist.Add (item);
                            //								}
                            //							}


                        }
                        else
                        {
                            InstantiateModel(prefab, newPrefabList[i].ToString() + "_", path, materlist);

                            GameObject currtenGo = Instantiate(prefab) as GameObject;
                            currtenGo.name = "SHOW_" + newPrefabList[i].ToString();
                            currtenGo.SetActive(false);
                            AnimSpineMgr.GetSkeletonAnimation(currtenGo);
                            AddclassifyDic(currtenGo);
                            //給2d素材添加材质球中贴图
                            LoadGameObjMat(prefab.name.ToString(), path, materlist);
                            currtenGo.transform.SetParent(WebCamera.GlassesRoot);
                            currtenGo.transform.localScale = Vector3.one;
                            currtenGo.transform.localPosition = Vector3.zero;
                            currtenGo.transform.localEulerAngles = Vector3.zero;
                            currtenGo.SetActive(true);
                        }
                      
                        Util.Log("InitMaterialsManager OnInitManager=> Instantiate Success! ===>" + prefab.name.ToString());
                        InstantiateModel(prefab, newPrefabList[i].ToString(), path, materlist);
               
                    }
                    else
                    {
                        Util.Log("InitMaterialsManager OnInitManager=> Prefab Object is null!");
                    }
                }
            }
            if (assetbundle_asset != null)
            {
                assetbundle_asset.Unload(false);
            }
            if (assetbundle_manifest != null)
            {
                assetbundle_manifest.Unload(false);
            }
            //Util.ClearMemory ();
            acton(bol, false);
        }
    }

    void InstantiateModel(GameObject prefab, string modelName, string path, List<Material> materlist)
    {
        GameObject currtenGo = Instantiate(prefab) as GameObject;
        currtenGo.name = modelName;
        currtenGo.SetActive(false);
        AnimSpineMgr.GetSkeletonAnimation(currtenGo);
        AddclassifyDic(currtenGo);
        //給2d素材添加材质球中贴图
        LoadGameObjMat(prefab.name.ToString(), path, materlist);
    }

    string matName = "";
    /// <summary>
    /// 給材质球添加贴图
    /// </summary>
    /// <param name="objName">Object name.</param>
    /// <param name="path">Path.</param>
    /// <param name="materlist">Materlist.</param>
    void LoadGameObjMat(string objName, string path, List<Material> materlist)
    {
        if (objName.Contains("2d_"))
        {
            if (materlist != null)
            {
                if (materlist.Count > 0)
                {
                    for (int m = 0; m < materlist.Count; m++)
                    {
                        matName = materlist[m].name.ToString();
                        Util.Log("materlist matName ==>" + matName);
                        if (matName.IndexOf("_Material") >= 0)
                        {
                            matName = matName.Replace("_Material", "");
                        }
                        else
                        {
                            matName = matName.Replace(objName + "_", "");
                        }

                        //加载贴图
                        LoadTexture(path + "/" + matName + ".png", false, (t2d) => {
                            if (t2d != null)
                            {

                                materlist[m].SetTexture("_MainTex", t2d as Texture);
#if UNITY_EDITOR
                                materlist[m].shader = Shader.Find("Spine/Sprite/Unlit");
#endif
                                Util.Log("materlist _MainTex ==>" + matName);
                            }
                        });
                    }
                }
            }
        }
    }

    /// <summary>
    /// Load Texture重载方法，判断是从硬盘中加载还是Resources目录下加载
    /// </summary>
    /// <param name="path">图片路径</param>
    /// <param name="isUnityResPath">如果是true<c>true</c>则从Unity Resources目录中加载</param>
    /// <param name="action">返回图片</param>
    void LoadTexture(string path, bool isUnityResPath, Action<Texture2D> action)
    {
        Util.Log("LoadTexture path==>" + path);
        Texture2D texture = null;
        if (!isUnityResPath)
        {
            if (Util.FileIsExistence(path))
            {
                byte[] bytes = fs.GetByteFromPath(path);
                texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);
            }
            else
            {
                Util.Log("log.LoadTexture path is not found");
            }
        }
        else
        {
            texture = Resources.Load<Texture2D>(path);
        }

        if (texture)
            action(texture);
    }

    public void OnDestroyGameObject(GameObject obj)
    {
        Util.Log("Destroy---obj => " + obj);
        if (null != obj)
        {
            FxMgr.RemoveFxByDict(obj.name);
            AnimSpineMgr.OnDestroySkeletonAnim(obj, obj.name);

            MeshRenderer mesh = obj.GetComponent<MeshRenderer>();
            if (mesh != null)
            {
                DestroyImmediate(mesh);
            }

            DestroyImmediate(obj);

        }
        else
        {
            Util.LogWarning("InitMaterialsManager OnDestroyGameObject=> Model Object is not exist!");
        }
        Util.ClearMemory();
    }

    /// <summary>
    /// 设置父类物体重载方法
    /// </summary>
    /// <param name="parentName">Parent name.</param>
    /// <param name="name">Name.</param>
    /// <param name="resetPosition">If set to <c>true</c> reset position.</param>
    public void OnSetParent(string parentName, string name, bool resetPosition)
    {
        GameObject obj = GetObject(name);
        GameObject parent = GetObject(parentName);
        if (obj != null && parent != null)
        {
            obj.transform.parent = parent.transform;
            if (resetPosition)
                obj.transform.localPosition = Vector3.zero;
        }
        else
        {
            Util.LogWarning("InitMaterialsManager OnSetParent=> Model Object is not exist!");
        }
    }

    /// <summary>
    /// Raises the set parent event.
    /// </summary>
    /// <param name="parentName">Parent name.</param>
    /// <param name="name">Name.</param>
    public void OnSetParent(string parentName, string name)
    {
        GameObject obj = GetObject(name);
        GameObject parent = GetObject(parentName);
        if (obj != null && parent != null)
        {
            obj.transform.parent = parent.transform;
        }
        else
        {
            Util.LogWarning("InitMaterialsManager OnSetParent=> Model Object is not exist!");
        }
    }

    /// <summary>
    /// 设置父对象
    /// </summary>
    /// <param name="name"></param>
    public void OnSetParent(GameObject parent, string name)
    {
        GameObject obj = GetObject(name);
        if (obj != null && parent != null)
        {
            obj.transform.parent = parent.transform;
        }
        else
        {
            Util.LogWarning("InitMaterialsManager OnSetParent=> Model Object is not exist!");
        }
    }

    /// <summary>
    /// 显示隐藏
    /// </summary>
    /// <param name="name">名称</param>
    /// <param bol="bol">true或false   true显示，false隐藏</param>
    public void OnSetActive(string name, bool bol)
    {
        GameObject obj = GetObject(name);
        if (obj != null)
        {
            obj.SetActive(bol);
            Util.Log("SetActive 1111=> " + name + "      " + bol);
        }
    }
    public void OnSetActive(string categroy, string name, bool bol)
    {
        GameObject obj = GetObjFromClassifyDic(categroy, name);
        if (obj != null)
        {
            obj.SetActive(bol);
            Util.Log("SetActive 2222=> " + name + "      " + bol);
        }
    }

#region 2D素材位置、旋转、大小计算
    /// <summary>
    /// 2D素材位置
    /// </summary>
    Vector3 v3 = Vector3.zero;
    public void OnPixelsPointToWorldPoint(string localtion, string name, string position)
    {
        //GameObject model = GetObject(name);
        GameObject model = GetObjFromClassifyDic(localtion, name);
        if (model == null)
        {
            return;
        }

        string[] arr = position.Split(',');
        float x = 0.0f, y = 0.0f, z = 0.0f;
        x = float.Parse(arr[0].ToString());
        y = float.Parse(arr[1].ToString());
        z = float.Parse(arr[2].ToString()) + 1;

        x = x * Screen.width / WebCamera.m_Resolution.x;
        y = y * Screen.height / WebCamera.m_Resolution.y;
        Util.Log("Rect==> fff "  + "x:" + x + "  y:" + y );
        if (localtion.Equals("Left"))
        {
            //x -= Screen.width / 4;
            x = (WebCamMgr.m_webCamera.leftRect.height / Screen.height * (x - 960)) + WebCamMgr.m_webCamera.leftRect.x + 960;
            y = (WebCamMgr.m_webCamera.leftRect.height / Screen.height * (x - 540)) + WebCamMgr.m_webCamera.leftRect.y + 540;
        }
        else if (localtion.Equals("Right"))
        {
            //x += Screen.width / 4;
            x = (WebCamMgr.m_webCamera.rightRect.height / Screen.height * (x - 960)) + WebCamMgr.m_webCamera.rightRect.x + 960;
            y = (WebCamMgr.m_webCamera.rightRect.height / Screen.height * (y - 540)) + WebCamMgr.m_webCamera.rightRect.y + 540;
        }
        v3 = new Vector3(x, y, z);
        Util.Log("Rect==> bbb " + "x:" + x + "  y:" + y);
        model.transform.position = Camera.main.ScreenToWorldPoint(v3);
        Util.Log("Transform==> position:" + model.transform.position);
    }

    /// <summary>
    /// 2D素材旋转
    /// </summary>
    /// <param name="localtion"></param>
    /// <param name="name"></param>
    /// <param name="angle"></param>
    public void OnWorldRotation(string localtion, string name, float angle)
    {
        GameObject model = GetObjFromClassifyDic(localtion, name);
        //GameObject model = GetObject(name);
        if(model == null)
        {
            return;
        }
        model.transform.eulerAngles = new Vector3(0, 0, angle + 90);
        Util.Log("Transform==> eulerAngles:" + model.name + model.activeSelf + "    " + model.transform.localScale + "    " + model.transform.localPosition );
        //Util.Log("Transform==> transform.parent:" + model.transform.parent.name);
    }

    /// <summary>
    /// 2D素材缩放
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="scale">Scale.</param>
    public void OnPixelsPointToWorldScale(string localtion, string name, string scale)
    {
        if (scale == "")
            return;
        GameObject model = GetObjFromClassifyDic(localtion, name);
        //GameObject model = GetObject(name);
        if (model == null)
        {
            return;
        }
        float fscale = float.Parse(scale) / m_ResolutionSize;
        if (localtion.Equals("Left"))
        {
            fscale *= WebCamMgr.m_webCamera.leftRect.height / Screen.height;
        }
        else if (localtion.Equals("Right"))
        {
            fscale *= WebCamMgr.m_webCamera.rightRect.height / Screen.height;
        }
        model.transform.localScale = new Vector3(fscale, fscale, fscale);
        Util.Log("Transform==> localScale:" + model.transform.localScale);
    }
#endregion

    /// <summary>
    /// 获取变换之后的矩阵数据
    /// 3D素材位置、旋转、大小的计算
    /// </summary>
    /// <param name="strArr">sdk出来的矩阵数据</param>
    /// <returns>变换之后的矩阵数据</returns>
    private Vector2 lastPos;
    private Vector3 facePos;
    private Vector3 faceAngle;
    private Vector3 faceScale;
    private float screenRatio_w;
    private float screenRatio_h;
    private float distance_H;
    private float distance_W;
    private float scaleRate;
    public void GetTransformMatrix4x4(TrackPointCoord[] allTrackPoints, float[] strArr, ref MatrixVectorStruct m_MatrixVector, string key)
    {
        GetSdkMatrix4x4(strArr);
        Matrix4x4 mat = Mutiply(size_fit_matrix, matr);
        mat.m23 = 185.5f;
        mat.m13 = mat.m13 + (m_originalOrthographicSize - m_newOrthographicSize);
        TestUI.myText3.text = "";
        for (int i = 0; i < 16; i++)
        {
            TestUI.myText3.text  += strArr[i] + "  ";
        }

        #region 计算角度
        Quaternion q = MatrixTool.GetRotation(mat);
        faceAngle.x = -q.eulerAngles.x - GetAngle(q.eulerAngles.y);
        faceAngle.y = -q.eulerAngles.y;
        faceAngle.z = q.eulerAngles.z;
        m_MatrixVector.Angles = faceAngle;
        Vector3 vc = MatrixTool.GetRotationEulerAngles(mat);
        Util.Log("GetTransformMatrix4x4--faceAngle==> " + q.eulerAngles + "    "  + vc);
        #endregion

        #region 计算大小
        vScale = MatrixTool.GetScale(mat);
        vScale.z = vScale.x;
        scaleRate = vScale.x;
        faceScale = vScale * 1.7f * TestUI.sizeRate;//调整眼镜大小      
        m_MatrixVector.Scale = faceScale;
        Util.Log("GetTransformMatrix4x4--faceScale==> " + faceScale);
        #endregion

        #region 计算位置
        screenRatio_w = Screen.width / WebCamera.m_Resolution.x;
        screenRatio_h = Screen.height / WebCamera.m_Resolution.y;
        facePos.x = (matr[0, 3]) * screenRatio_w;
        facePos.y = (WebCamera.m_Resolution.y - matr[1, 3]) * screenRatio_h;
        facePos.z = 0;
        if (key.Equals("Left"))
        {
            //facePos.x -= Screen.width / 4;
            facePos.x = (WebCamMgr.m_webCamera.leftRect.height / Screen.height * (facePos.x - 960)) + WebCamMgr.m_webCamera.leftRect.x + 960;
            facePos.y = (WebCamMgr.m_webCamera.leftRect.height / Screen.height * (facePos.y - 540)) + WebCamMgr.m_webCamera.leftRect.y + 540;
        }
        else if (key.Equals("Right"))
        {
            //facePos.x += Screen.width / 4;
            facePos.x = (WebCamMgr.m_webCamera.rightRect.height / Screen.height * (facePos.x - 960)) + WebCamMgr.m_webCamera.rightRect.x + 960;
            facePos.y = (WebCamMgr.m_webCamera.rightRect.height / Screen.height * (facePos.y - 540)) + WebCamMgr.m_webCamera.rightRect.y + 540;
        }
        Util.Log("GetTransformMatrix4x4--facePos==> " + facePos);
        facePos = Camera.main.ScreenToWorldPoint(facePos);
        m_MatrixVector.Point = facePos;
        #endregion
    }

    float GetAngle(float angle)
    {
        if(angle > 180)
        {
            angle = angle - 360;
        }else if(angle < -180)
        {
            angle = angle + 360;
        }

        return Mathf.Abs(angle / 6);
    }

    /// <summary>
    /// 对模型设置矩阵值
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="matrix">矩阵值</param>
    /// 矩阵格式：
    /// m00,m01,m02,m03
    /// m10,m11,m12,m13
    /// m20,m21,m22,m23
    /// m30,m31,m32,m33
    /// 缩放：m00,m11,m22
    /// 位置：m03,m13,m23
    /// 旋转X：m11,m22,m21,m22
    /// 旋转Y：m00,m02,m20,m22
    /// 旋转Z：m00,m01,m10,m11
    public void MatrixToValue(string category, string name, MatrixVectorStruct m_MatrixVector)
    {
        GameObject model = GetObjFromClassifyDic(category, name);
        if(model == null)
        {
            return;
        }
        else
        {
            MatrixToValue(model, m_MatrixVector);
        }     
    }

    private Vector3 targetAngle;
    private float targetScalse;
    public void MatrixToValue(GameObject model, MatrixVectorStruct m_MatrixVector)
    {
        model.transform.position = m_MatrixVector.Point;
        model.transform.eulerAngles = m_MatrixVector.Angles;
        model.transform.localScale = m_MatrixVector.Scale;

        //Util.Log("MatrixToValue => " + model.name + "    " + model.transform.position + "    " + model.transform.localScale.x + "    " + model.activeSelf);
        Util.Log("MatrixToValue => " + model.transform.eulerAngles.x + "  " + model.transform.eulerAngles.y + "  " + model.transform.eulerAngles.z);
    }

    /// <summary>
    /// 获取初始缩放矩阵
    /// </summary>
    public void GetInitMatrix4x4(float cameraSize)
    {
        //		//获取正交相机下屏幕坐下脚（0，0）对应的3d坐标中位置点。
        Vector3 Point_0 = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Util.Log("Point_0" + Point_0.x + "Y:" + Point_0.y + "Z:" + Point_0.z);
        //		//由3d坐标点获取屏幕中其它3个脚点位置点
        //		//根据3d坐标点算出其在图坐标系中点像素点
        float x = ((Point_0.x * WebCamera.m_Resolution.x + cameraSize * WebCamera.m_Resolution.y) / cameraSize) * 0.5f;
        //y = (WebCamera.m_Resolution.x * cameraSize - Point_0.y * WebCamera.m_Resolution.x) / cameraSize;
        //由图坐标系中点像素点推导出其它3个像素点
        float s_factor, x_factor, y_factor;
        size_fit_matrix = Matrix4x4.identity;

        s_factor = (cameraSize * 2) / WebCamera.m_Resolution.x;
        x_factor = Point_0.x - (s_factor * x);
        y_factor = cameraSize;

        size_fit_matrix.m00 = s_factor;
        size_fit_matrix.m03 = x_factor;
        size_fit_matrix.m11 = -s_factor;
        size_fit_matrix.m13 = y_factor;
        size_fit_matrix.m22 = -s_factor;
    }

    /// <summary>
    /// 获取sdk中矩阵数据
    /// </summary>
    /// <param name="strArr">String arr.</param>
    void GetSdkMatrix4x4(float[] strArr)
    {
        matr[0, 0] = strArr[0];
        matr[0, 1] = strArr[1];
        matr[0, 2] = strArr[2];
        matr[0, 3] = strArr[3];
        matr[1, 0] = strArr[4];
        matr[1, 1] = strArr[5];
        matr[1, 2] = strArr[6];
        matr[1, 3] = strArr[7];
        matr[2, 0] = strArr[8];
        matr[2, 1] = strArr[9];
        matr[2, 2] = strArr[10];
        matr[2, 3] = strArr[11];
        matr[3, 0] = strArr[12];
        matr[3, 1] = strArr[13];
        matr[3, 2] = strArr[14];
        matr[3, 3] = strArr[15];
    }

    /// <summary>
    /// 矩阵相乘
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public Matrix4x4 Mutiply(Matrix4x4 a, Matrix4x4 b)
    {
        float c = 0;
        //确保矩阵a的列数和b的行数相等
        //用来存放结果的矩阵，axb的结果为a的行数和b的列数
        result = Matrix4x4.zero;
        //对a的每行进行遍历
        for (int i = 0; i < 4; i++)
        {
            //对b的每列进行遍历
            for (int j = 0; j < 4; j++)
            {
                //c为每一个点的值
                c = 0;
                //第i行j列的值为a的第i行上的n个数和b的第j列上的n个数对应相乘之和，其中n为a的列数，也是b的行数，a的列数和b的行数相等
                for (int k = 0; k < 4; k++)
                {
                    c += (a[i, k] * b[k, j]);
                }
                result[i, j] = c;
            }
        }

        return result;
    }

    /// <summary>
    /// 脸部形变操作
    /// </summary>
    /// <param name="name"></param>
    /// <param name="v3"></param>
    public void MeshVerticesToValue(string name, Vector3[] v3)
    {
        GameObject model = GetObject(name);
        if (model != null)
        {
            if (mfFace != null)
            {
                mfFace.mesh.vertices = GetIndexArray(v3);
            }
        }
    }

    /// <summary>
    /// 排序索引
    /// </summary>
    /// <param name="v3"></param>
    /// <returns></returns>
    Vector3[] GetIndexArray(Vector3[] v3)
    {
        for (int i = 0; i < v3.Length; i++)
        {
            v_index[i].x = (-v3[m_listIndex[i]].x);
            v_index[i].y = (-v3[m_listIndex[i]].z);
            v_index[i].z = (v3[m_listIndex[i]].y);
        }
        return v_index;
    }

    /// <summary>
    /// 重置模型皮肤
    /// </summary>
    /// <param name="value">Value.</param>
    public void SetModelFaceTexture(string path, bool isUnityResPath)
    {
        Util.Log("SetModelFaceTexture===>" + path);
        LoadTexture(path, isUnityResPath, (t2d) => {
            if (t2d != null)
            {
                SetTexture("face", t2d);
            }
        });
    }

    /// <summary>
    /// 设置shader贴图
    /// </summary>
    /// <param name="name"></param>
    /// <param name="t2d"></param>
    public void SetTexture(string name, Texture2D t2d)
    {
        GameObject model = GetObject(name);
        if (model != null)
        {
            if (mrFace != null)
            {
                mrFace.material.SetTexture("_MainTex", t2d);
            }
        }
    }

    /// <summary>
    /// 设置模型父节点
    /// </summary>
    /// <param name="parentName">父类名称</param>
    /// <param name="childName">子节点名称</param>
    /// <param name="objName">要设置物体的名称</param>
    /// <param name="resetPosition">是否重置要设置物体的坐标<c>true</c>重置物体localPosition，跟父类保持一致</param>
    public void SetModelParent(string parentName, string childName, string objName, bool resetPosition)
    {
        if (parentName == null)
        {
            return;
        }

        //设置Node
        if (parentName != "" && childName == "")
            MaterialsMgr.OnSetParent(parentName, objName, resetPosition);
        else if (parentName != "" && childName != "")
            MaterialsMgr.OnSetParentFromChild(parentName, childName, objName, resetPosition);
    }

    /// <summary>
    /// 设置世界位置坐标
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="position">位置点</param>
    public void OnSetModelZOrder(string name, float zValue)
    {
        GameObject model = GetObject(name);
        if (model != null)
        {
            model.transform.position = new Vector3(model.transform.position.x, model.transform.position.y, zValue);
        }
        else
        {
            Util.Log("log.message OnModelZOrder Model Not Found! Name=" + name);
        }
    }

    public void SetAnimationEventS(Animator anim, LDAnimationClipInfo clipInfo)
    {
        //		AnimatorController animatorController = anim.runtimeAnimatorController as AnimatorController;
        //		animatorController.layers [0].stateMachine.states[clipInfo.animClipID].state.speed = (float)clipInfo.speed;

        if (anim == null)
        {
            Util.Log("SetAnimationEventS anim is null ! ");
            return;
        }

        if (clipInfo == null)
        {
            Util.Log("SetAnimationEventS clipInfo is null ! ");
            return;
        }

        AnimationClip animClip;
        AnimationEvent animEvent;
        animClip = new AnimationClip();
        animEvent = new AnimationEvent();

        if (clipInfo.animConfigs != null && clipInfo.animConfigs.Count > 0)
        {
            int count = clipInfo.animConfigs.Count;
            //Util.Log ("!!!!!count = " + count);
            for (int i = 0; i < count; i++)
            {
                animEvent.functionName = clipInfo.animConfigs[i].functionName;

                string v = clipInfo.animConfigs[i].functionParameValue;

                switch (clipInfo.animConfigs[i].functionParameType)
                {
                    case "float":
                        animEvent.floatParameter = float.Parse(v);
                        break;

                    case "int":
                        animEvent.intParameter = int.Parse(v);
                        break;

                    case "string":
                        animEvent.stringParameter = v;
                        break;
                }

                animEvent.time = (float)clipInfo.animConfigs[i].startTime;

                animClip = anim.runtimeAnimatorController.animationClips[clipInfo.animClipIndex];
                animClip.AddEvent(animEvent);
            }
        }
    }

    /// <summary>
    /// 设置父类对象
    /// 目标为一个物体的子类，通过路径找到子类节点
    /// 设置子类的localPosition为0
    /// 物体保持自身位置还是父类位置
    /// </summary>
    /// <param name="name"></param>
    public void OnSetParentFromChild(string parentName, string childName, string name, bool resetPosition)
    {
        GameObject obj = GetObject(name);
        GameObject parent = GetObject(parentName);
        if (obj != null && parent != null)
        {
            Transform childTransform = parent.transform.Find(childName);
            obj.transform.parent = childTransform;
            if (resetPosition)
                obj.transform.localPosition = Vector3.zero;
        }
        else
        {
            Util.LogWarning("InitMaterialsManager OnSetParent=> Model Object is not exist!");
        }
    }
}