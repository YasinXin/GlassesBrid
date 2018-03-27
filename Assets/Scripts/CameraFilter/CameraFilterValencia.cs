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
/// 海港
/// </summary>
[ExecuteInEditMode]
[AddComponentMenu("Camera Filter/Valencia")]
public class CameraFilterValencia : MonoBehaviour {
    #region Variables
    public Shader SCShader;
    private Material SCMaterial;
    private Texture SCTexture1;
    private Texture SCTexture2;
    public float inputColorR = 0.16666f;
    public float inputColorG = 0.5f;
    public float inputColorB = 0.83333f;
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
        SCShader = Shader.Find("lidx/lidx_filter_1");
        SCTexture1 = Resources.Load("images/f1_pict0", typeof(Texture)) as Texture;
        SCTexture2 = Resources.Load("images/f1_pict1", typeof(Texture)) as Texture;
		inputColorR = 0.16666f;
		inputColorG = 0.5f;
		inputColorB = 0.83333f;
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
            material.SetFloat("_inputColorR", inputColorR);
            material.SetFloat("_inputColorG", inputColorG);
            material.SetFloat("_inputColorB", inputColorB);
            material.SetTexture("_inputImageTexture2", SCTexture1);
            material.SetTexture("_inputImageTexture3", SCTexture2);
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
            SCShader = Shader.Find("lidx/lidx_filter_1");
            SCTexture1 = Resources.Load("images/f1_pict0", typeof(Texture)) as Texture;
            SCTexture2 = Resources.Load("images/f1_pict1", typeof(Texture)) as Texture;
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
