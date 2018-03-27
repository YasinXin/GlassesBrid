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
[ExecuteInEditMode]
[AddComponentMenu("Camera Filter/FaceMorph/Final")]

public class CameraFilter_FaceMorph_Final : Manager
{
   // Vector2 m_Resolution = Vector2.zero;
    #region Variables
    public Shader SCShader;
    private Material SCMaterial;

    public Vector2 _faceleft = Vector2.zero;
    public Vector2 _faceright = Vector2.zero;
    public Vector2 _chin = Vector2.zero;
    public Vector2 _nose = Vector2.zero;
    public Vector2 _eyea = Vector2.zero;
    public Vector2 _eyeb = Vector2.zero;
    public Vector2 _left = Vector2.zero;
    public Vector2 _right = Vector2.zero;
    public Vector2 _chinleft = Vector2.zero;
    public Vector2 _chinright = Vector2.zero;
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
        SCShader = Shader.Find("CameraFilter/FaceMorph_Final");
        //m_Resolution = WebCamera.m_Resolution;
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
    }

   // void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
   // {
   //     if (SCShader != null)
   //     {
			//if (WebCamMgr.u_bolTarck)
   //         {
			//	_faceleft = WebCamMgr.GetMid(3, 4);
			//	_faceright = WebCamMgr.GetMid(12, 13);
			//	_chin = WebCamMgr.GetMid(8);
			//	_nose = WebCamMgr.GetMid(30);
			//	_eyea = WebCamMgr.GetMid(37, 38, 40, 41);
			//	_eyeb = WebCamMgr.GetMid(43, 44, 46, 47);
			//	_left = WebCamMgr.GetMid(48);
			//	_right = WebCamMgr.GetMid(54);
			//	_chinleft = WebCamMgr.GetMid(6, 7);
			//	_chinright = WebCamMgr.GetMid(9, 10);

   //             material.SetVector("_faceleft", new Vector4(_faceleft.x, _faceleft.y, 0.0f, 0.0f));
   //             material.SetVector("_faceright", new Vector4(_faceright.x, _faceright.y, 0.0f, 0.0f));
   //             material.SetVector("_chin", new Vector4(_chin.x, _chin.y, 0.0f, 0.0f));
   //             material.SetVector("_nose", new Vector4(_nose.x, _nose.y, 0.0f, 0.0f));
   //             material.SetVector("_eyea", new Vector4(_eyea.x, _eyea.y, 0.0f, 0.0f));
   //             material.SetVector("_eyeb", new Vector4(_eyeb.x, _eyeb.y, 0.0f, 0.0f));
   //             material.SetVector("_left", new Vector4(_left.x, _left.y, 0.0f, 0.0f));
   //             material.SetVector("_right", new Vector4(_right.x, _right.y, 0.0f, 0.0f));
   //             material.SetVector("_chinleft", new Vector4(_chinleft.x, _chinleft.y, 0.0f, 0.0f));
   //             material.SetVector("_chinright", new Vector4(_chinright.x, _chinright.y, 0.0f, 0.0f));

   //             GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
   //             Graphics.Blit(sourceTexture, destTexture, material);
   //         }
   //         else
   //         {
   //             Graphics.Blit(sourceTexture, destTexture);
   //         }
   //     }
   //     else
   //     {
   //         Graphics.Blit(sourceTexture, destTexture);
   //     }
   // }

    void Update()
    {

#if UNITY_EDITOR
        if (Application.isPlaying != true)
        {
            SCShader = Shader.Find("CameraFilter/FaceMorph_Final");
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
