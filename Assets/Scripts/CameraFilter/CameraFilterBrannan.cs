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
/// 苦艾
/// </summary>
[ExecuteInEditMode]
[AddComponentMenu("Camera Filter/Brannan")]
public class CameraFilterBrannan : MonoBehaviour
{
    #region Variables
	static Shader SCShader;
	static Material SCMaterial;
	static Texture SCTexture;
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
        SCShader = Shader.Find("lidx/lidx_filter_brannan");
        SCTexture = Resources.Load("images/filter_brannan", typeof(Texture))as Texture;
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
			material.SetTexture("_inputImageTexture2", SCTexture);
			return material;
		} else {
			Debug.Log ("null");
			return null;
		}
	}

    //void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    //{
    //    if (SCShader != null)
    //    {
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
            SCShader = Shader.Find("lidx/lidx_filter_brannan");
            SCTexture = Resources.Load("images/filter_brannan", typeof(Texture)) as Texture;
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