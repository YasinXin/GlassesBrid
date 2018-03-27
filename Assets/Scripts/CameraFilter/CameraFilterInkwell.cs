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
using UnityEngine;
using System.Collections;

/// <summary>
/// 默片
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter/Inkwell")]
public class CameraFilterInkwell : MonoBehaviour {

    #region Variables
	static Shader SCShader;
	static Material SCMaterial;
    #endregion

    #region Properties
    Material material
    {
        get
        {
            if (SCMaterial == null)
            {
                SCMaterial = new Material(SCShader);
                SCMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return SCMaterial;
        }
    }
    #endregion

    void Start()
    {
        SCShader = Shader.Find("lidx/lidx_filter_inkwell_1");
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
    }

/// <summary>
	/// Gets the material info.
	/// </summary>
	/// <returns>The material info.</returns>
	public Material GetMaterialInfo()
	{
		if (SCShader != null) {
			return material;
		} else {
			return null;
		}
	}
    //void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    //{
    //    if (SCShader != null)
    //    {
    //        Graphics.Blit(sourceTexture, destTexture, material);
    //    }
    //    else
    //    {
    //        Graphics.Blit(sourceTexture, destTexture);
    //    }
    //}

    void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying != true)
        {
            SCShader = Shader.Find("lidx/lidx_filter_inkwell_1");
        }
#endif
    }

    void OnDisable()
    {
        if (SCMaterial)
        {
            DestroyImmediate(SCMaterial);
        }
    }
}
