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
/// 都市
/// </summary>
/// 
[ExecuteInEditMode]
[AddComponentMenu("Camera Filter/Urban")]
public class CameraFilterUrban : MonoBehaviour
{
    #region Variables
    public Shader SCShader;
    private Material SCMaterial;
    private Texture SCTexture;
    [Range(0f, 20f)]
    public float blueColorLevel = 15.0f;
    [Range(0f, 3f)]
    public float level = 1.0f;
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
        SCTexture = Resources.Load("images/filter_urban_do_0930", typeof(Texture))as Texture;
		blueColorLevel = 15.0f;
		level = 1.0f;
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
    }

    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if (SCShader != null)
        {
            material.SetFloat("_blueColorLevel", blueColorLevel);
            material.SetFloat("_level", level);
            material.SetTexture("_inputImageTexture2", SCTexture);
            Graphics.Blit(sourceTexture, destTexture, material);
        }
        else
        {
            Graphics.Blit(sourceTexture, destTexture);
        }
    }


    void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying != true)
        {
            SCShader = Shader.Find("lidx/lidx_filter_weaklight");
            SCTexture = Resources.Load("images/filter_urban_do_0930", typeof(Texture)) as Texture;
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