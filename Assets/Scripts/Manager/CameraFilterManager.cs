using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CameraFilterManager : Manager
{
    /// <summary>
    /// 程序中滤镜对象字典
    /// </summary>
    static Dictionary<string, string> m_FilterObjDic = new Dictionary<string, string>();

    GameObject m_UiCameraObj;
    //GameObject m_MainCameraObj;
    string m_CurrentFilterName = "";
    string m_CurrentFaceMorphFilterName = "";
	public UICameraFilterRender m_FilterRender=null;
	public float m_slimFace = 0.0f;
	public float m_bigEye = 0.0f;
    // Use this for initialization
    public void OnAwakeUp()
    {
        //m_MainCameraObj = GameObject.Find("Main Camera");
        m_UiCameraObj = GameObject.Find("2DUIRoot/UICamera");
        OnAddFilterObjToDictionary();

        StartCoroutine(StartFilter());
#if UNITY_EDITOR
        //FilteMgr.CameraFilter("Rosy");
        //FaceMorphFilter("EyeFace");
#endif
    }

    IEnumerator StartFilter()
    {
        yield return new WaitForSeconds(0.1f);
        if (Main.openBuffing)
        {
            FilteMgr.FaceMorphFilter("EyeFace"); //初始默认开启磨皮
            WebCamMgr.SetMaterialFaceLevel(5); //初始默认磨皮为等级为5
        }

        if (Main.openFilter)
        {
            FilteMgr.CameraFilter("Rosy"); //初始默认滤镜为Rosy
        }
    }
    

    void OnAddFilterObjToDictionary()
    {
		//相机添加组件
	   bool bol= Util.AddScriptsComponentS (m_UiCameraObj, "UICameraFilterRender");
		if (bol) {
			m_FilterRender = m_UiCameraObj.GetComponent<UICameraFilterRender> ();
		}

        //原图
        m_FilterObjDic.Add("Origin", "CameraFilterOrigin");
        //可爱
        m_FilterObjDic.Add("Lolita", "CameraFilterLolita");
        //珊瑚
        m_FilterObjDic.Add("Coral", "CameraFilterCoral");
        //浪漫
        m_FilterObjDic.Add("Rosy", "CameraFilterRosy");
        //柔和
        m_FilterObjDic.Add("Crisp", "CameraFilterCrisp");
        //自然
        m_FilterObjDic.Add("Nature", "CameraFilterNature");
        //纯净
        m_FilterObjDic.Add("Clean", "CameraFilterClean");
        //淡雅
        m_FilterObjDic.Add("Vivid", "CameraFilterVivid");
        //甜美
        m_FilterObjDic.Add("Sweety", "CameraFilterSweety");
        //薄暮
        m_FilterObjDic.Add("Sunset", "CameraFilterSunset");
        //草原
        m_FilterObjDic.Add("Grass", "CameraFilterGrass");
        //粉嫩
        m_FilterObjDic.Add("Pink", "CameraFilterPink");
        //苦涩
        m_FilterObjDic.Add("Brannan", "CameraFilterBrannan");
        //黑白
        m_FilterObjDic.Add("Inkwell", "CameraFilterInkwell");
        //默认的（android时会用到，iOS暂时不用）
        m_FilterObjDic.Add("Default", "CameraFilter_Default");
        //廋脸
        m_FilterObjDic.Add("Final", "CameraFilter_FaceMorph_Final");
        //大嘴
        m_FilterObjDic.Add("Ghost", "CameraFilter_FaceMorph_Ghost");
		//大眼瘦脸
		m_FilterObjDic.Add ("EyeFace", "CameraFilterEyeFace");
    }

    /// <summary>
    /// 滤镜操作（滤镜）
    /// </summary>
    /// <param name="bol"></param>
    public void CameraFilter(string key)
    {
        string filterName = "";
        m_FilterObjDic.TryGetValue(key, out filterName);
        if (filterName != m_CurrentFilterName)
        {
		    if (m_FilterRender != null) {
				m_FilterRender.m_Material1Name = "";
				m_FilterRender.methodmat1 = null;
			}
            Util.DestroyScriptsComponent(m_UiCameraObj, m_CurrentFilterName);
            #if !UNITY_EDITOR && UNITY_ANDROID
                        if (filterName == "")
                        {
                            filterName = "Default";
                        }
            #endif
            if (filterName != "")
            {
                //Util.AddScriptsComponent(m_UiCameraObj, filterName);
				bool bol= Util.AddScriptsComponentS (m_UiCameraObj, filterName);
				if (bol) {
					if (m_FilterRender != null) {
						m_FilterRender.m_Material1Name = filterName;
					}
				}
            }
            m_CurrentFilterName = filterName;
        }
    }

    /// <summary>
    /// 滤镜操作（磨皮、大眼瘦脸）
    /// </summary>
    /// <param name="bol"></param>
	public void FaceMorphFilter(string key)
    {
        string filterName = "";
        m_FilterObjDic.TryGetValue(key, out filterName);
        if (filterName != m_CurrentFaceMorphFilterName)
        {
			if (m_FilterRender != null) {
				m_FilterRender.m_Material2Name = "";
				m_FilterRender.methodmat2 = null;
			}
            Util.DestroyScriptsComponent(m_UiCameraObj, m_CurrentFaceMorphFilterName);
#if !UNITY_EDITOR && UNITY_ANDROID
            if (filterName == "")
            {
                filterName = "Default";
            }
#endif
            if (filterName != "")
            {
               // Util.AddScriptsComponent(m_MainCameraObj, filterName);
				bool bol =Util.AddScriptsComponentS (m_UiCameraObj, filterName);
				if (bol) {
					if (m_FilterRender != null) {
						m_FilterRender.m_Material2Name = filterName;
					}
				}
            }
            m_CurrentFaceMorphFilterName = filterName;
        }
    }
	/// <summary>
	/// 修改大眼瘦脸 程度值
	/// </summary>
	/// <param name="f_slim">F slim.</param>
	/// <param name="f_bigEye">F big eye.</param>
	public void OnEyeFaceFilterValue(float f_slim,float f_bigEye)
	{
		m_slimFace = f_slim;
		m_bigEye = f_bigEye;
	}

    public bool isDoubleScreen = false;

    public void ChangeScreenModel(bool bol)
    {
        isDoubleScreen = bol;
        if (!bol)
        {
            WebCamera.m_splitPanel.gameObject.SetActive(false);
            WebCamera.m_RawImage.gameObject.SetActive(true);
        }
        else
        {
            WebCamera.m_splitPanel.gameObject.SetActive(true);
            WebCamera.m_RawImage.gameObject.SetActive(false);
        }

    }


}