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

/// <summary>
/// 薄暮
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter/Sunset")]
public class CameraFilterSunset : MonoBehaviour
{
    #region Variables
	static Shader SCShader;
	static Material SCMaterial;
	static Texture SCTexture;
    [Range(0f, 20f)]
    public float blueColorLevel = 12.5f;
    [Range(0f, 3f)]
    public float level = 0.82f;
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
        SCShader = Shader.Find("lidx/lidx_filter_weaklight");
        SCTexture = Resources.Load("images/filter_sunset_0922", typeof(Texture))as Texture;
		blueColorLevel = 12.5f;
		level = 0.82f;
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
			material.SetFloat("_blueColorLevel", blueColorLevel);
			material.SetFloat("_level", level);
			material.SetTexture("_inputImageTexture2", SCTexture);
			return material;
		} else {
			return null;
		}
	}

    //void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    //{
    //    if (SCShader != null)
    //    {
    //        material.SetFloat("_blueColorLevel", blueColorLevel);
    //        material.SetFloat("_level", level);
    //        material.SetTexture("_inputImageTexture2", SCTexture);
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
            SCShader = Shader.Find("lidx/lidx_filter_weaklight");
            SCTexture = Resources.Load("images/filter_sunset_0922", typeof(Texture)) as Texture;
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