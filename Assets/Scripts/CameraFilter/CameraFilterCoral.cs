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
/// 珊瑚
/// </summary>
[ExecuteInEditMode]
[AddComponentMenu("Camera Filter/Coral")]
public class CameraFilterCoral : MonoBehaviour
{
    #region Variables
	static Shader SCShader;
	static Material SCMaterial;
	static Texture SCTexture;
    [Range(0f, 20f)]
    public float blueColorLevel =13.0f;
    [Range(0f, 3f)]
    public float level = 1.03f;
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
        SCTexture = Resources.Load("images/filter_coral_0901", typeof(Texture))as Texture;
		blueColorLevel =13.0f;
		level = 1.03f;
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
			Debug.Log ("null");
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
            SCTexture = Resources.Load("images/filter_coral_0901", typeof(Texture)) as Texture;
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