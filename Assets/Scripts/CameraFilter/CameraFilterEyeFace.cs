
using UnityEngine;
using System.Collections;
using System.Text;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter/EyeFace")]
public class CameraFilterEyeFace : Manager
{
	#region Variables

	static Shader SCShader;
	static Material SCMaterial;

	public Vector2[] _faceleft_u = { Vector2.zero, Vector2.zero };
	public Vector2[] _faceright_u = { Vector2.zero, Vector2.zero };

	public Vector2[] _eyea = { Vector2.zero, Vector2.zero };
	public Vector2[] _eyeb = { Vector2.zero, Vector2.zero };

	public Vector2[] _chin = { Vector2.zero, Vector2.zero };
	public Vector2[] _chinleft_d = { Vector2.zero, Vector2.zero };
	public Vector2[] _chinright_d = { Vector2.zero, Vector2.zero };
	public Vector2[] _chinleft_u = { Vector2.zero, Vector2.zero };
	public Vector2[] _chinright_u = { Vector2.zero, Vector2.zero };

	public Vector2[] _nose = { Vector2.zero, Vector2.zero };
	public Vector2[] _noseleft = { Vector2.zero, Vector2.zero };
	public Vector2[] _noseright = { Vector2.zero, Vector2.zero };
	//	[Range(0.0f,1.9f)]
	public float u_slimFactor = 0.0f;
	//	[Range(0.0f,0.2f)]
	public float u_bigEyeFactor = 0.0f;
	public int u_tracksuccess = 0;
	public int u_tracknum = 0;
    public float beauty = 0.1f;
	int index = 0;
	#endregion

	#region Properties

	public Material material {
		get {
			if (SCMaterial == null) {
				SCMaterial = new Material (SCShader);
				SCMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return SCMaterial;
		}
	}

	#endregion

	void Awake ()
	{
        SCShader = Shader.Find("lidx/filter_beauty_morph");// lidx_filter_eyeface_new");
		if (!SystemInfo.supportsImageEffects) {
           
			enabled = false;
			return;
		}
	}
    float frame;
	/// <summary>
	/// Gets the material info.
	/// </summary>
	/// <returns>The material info.</returns>
	public Material GetMaterialInfo ()
	{
        
        if (SCShader != null) {
            if (u_slimFactor == FilteMgr.m_slimFace && u_slimFactor == FilteMgr.m_slimFace) //如果数值没有改变，则不改变  
            {
                return material;
            }

            u_slimFactor = FilteMgr.m_slimFace;
			u_bigEyeFactor = FilteMgr.m_bigEye;
			if (WebCamMgr.u_bolTarck && !WebCamMgr.m_tarck2dOr3d)
            {
				if (WebCamMgr.faceNum == 0) {
					return null;
				}
                
				if (WebCamMgr.track2dretResult != null) {
					if (WebCamMgr.track2dretResult.Length > 0) {
                        
						if (WebCamMgr.faceNum >= 2) {
							u_tracknum = 1;
						} else {
							u_tracknum = 0;
						}
						u_tracksuccess = 1;

						for (int i = 0; i < 2; i++) {
							_faceleft_u [i] = Vector2.zero;
							_faceright_u [i] = Vector2.zero;
							_eyea [i] = Vector2.zero;
							_eyeb [i] = Vector2.zero;
							_chin [i] = Vector2.zero;
							_chinleft_d [i] = Vector2.zero;
							_chinright_d [i] = Vector2.zero;
							_chinleft_u [i] = Vector2.zero;
							_chinright_u [i] = Vector2.zero;
							_nose[i] = Vector2.zero;
							_noseleft[i] = Vector2.zero;
							_noseright[i] = Vector2.zero;
						}

						for (int i = 0; i < u_tracknum + 1; i++) {
							if (WebCamManager.multimode.Length > i) {
								index = int.Parse (WebCamManager.multimode [i]); 
								if (index <= WebCamMgr.faceNum) {
									_eyea [i] = WebCamMgr.GetMid (37, 38, 40, 41, index-1);
									_eyeb [i] = WebCamMgr.GetMid (43, 44, 46, 47, index-1);
									_chin [i] = WebCamMgr.GetMid (9, index-1);
									_chinleft_d [i] = WebCamMgr.GetMid (4, 5, index-1);
									_chinright_d [i] = WebCamMgr.GetMid (11, 12, index-1);
									_chinleft_u [i] = WebCamMgr.GetMid (6, 7, index-1);
									_chinright_u [i] = WebCamMgr.GetMid (9, 10, index-1);
									_faceleft_u [i] = WebCamMgr.GetMid (2, 3, index-1);
									_faceright_u [i] = WebCamMgr.GetMid (13, 14, index-1);
									_nose [i] = WebCamMgr.GetMid (30, i);
									_noseleft [i] = WebCamMgr.GET_NOSEWING_POINT (33, 51, 31, 0.45f, i);
									_noseright [i] = WebCamMgr.GET_NOSEWING_POINT (33, 51, 35, 0.45f, i);
								}
							}
						}
                        
						material.SetInt ("u_tracknum", u_tracknum);
						material.SetInt ("u_tracksuccess", u_tracksuccess);
						material.SetFloat ("u_slimFactor", u_slimFactor);
						material.SetFloat ("u_bigEyeFactor", u_bigEyeFactor);
						//material.SetFloat("_beauty",beauty);
						material.SetVector ("_faceleft_u", new Vector4 (_faceleft_u [0].x, _faceleft_u [0].y, _faceleft_u [1].x, _faceleft_u [1].y));
						material.SetVector ("_faceright_u", new Vector4 (_faceright_u [0].x, _faceright_u [0].y, _faceright_u [1].x, _faceright_u [1].y));
						material.SetVector ("_eyea", new Vector4 (_eyea [0].x, _eyea [0].y, _eyea [1].x, _eyea [1].y));
                       // material.SetVector("_eyea", new Vector4(1.0f, 1.0f, _eyea[1].x, _eyea[1].y));
                        material.SetVector ("_eyeb", new Vector4 (_eyeb [0].x, _eyeb [0].y, _eyeb [1].x, _eyeb [1].y));
                        material.SetVector ("_chin", new Vector4 (_chin [0].x, _chin [0].y, _chin [1].x, _chin [1].y));
  
                        material.SetVector ("_chinleft_d", new Vector4 (_chinleft_d [0].x, _chinleft_d [0].y, _chinleft_d [1].x, _chinleft_d [1].y));
						material.SetVector ("_chinright_d", new Vector4 (_chinright_d [0].x, _chinright_d [0].y, _chinright_d [1].x, _chinright_d [1].y));
						material.SetVector ("_chinleft_u", new Vector4 (_chinleft_u [0].x, _chinleft_u [0].y, _chinleft_u [1].x, _chinleft_u [1].y));
						material.SetVector ("_chinright_u", new Vector4 (_chinright_u [0].x, _chinright_u [0].y, _chinright_u [1].x, _chinright_u [1].y));
						material.SetVector ("_nose", new Vector4 (_nose [0].x, _nose [0].y, _nose [1].x, _nose [1].y));
						material.SetVector ("_noseleft", new Vector4 (_noseleft [0].x, _noseleft [0].y, _noseleft [1].x, _noseleft [1].y));
						material.SetVector ("_noseright", new Vector4 (_noseright [0].x, _noseright [0].y, _noseright [1].x, _noseright [1].y));
                       
						return material;
					} else {
						return null;
					}
				} else {
					return null;
				}
			} else {

                material.SetInt("u_tracknum", u_tracknum);
                material.SetInt("u_tracksuccess", u_tracksuccess);
                material.SetFloat("u_slimFactor", u_slimFactor);
                material.SetFloat("u_bigEyeFactor", u_bigEyeFactor);
                //material.SetFloat("_beauty",beauty);
                material.SetVector("_faceleft_u", new Vector4(_faceleft_u[0].x, _faceleft_u[0].y, _faceleft_u[1].x, _faceleft_u[1].y));
                material.SetVector("_faceright_u", new Vector4(_faceright_u[0].x, _faceright_u[0].y, _faceright_u[1].x, _faceright_u[1].y));
                material.SetVector("_eyea", new Vector4(_eyea[0].x, _eyea[0].y, _eyea[1].x, _eyea[1].y));
                // material.SetVector("_eyea", new Vector4(1.0f, 1.0f, _eyea[1].x, _eyea[1].y));
                material.SetVector("_eyeb", new Vector4(_eyeb[0].x, _eyeb[0].y, _eyeb[1].x, _eyeb[1].y));
                material.SetVector("_chin", new Vector4(_chin[0].x, _chin[0].y, _chin[1].x, _chin[1].y));

                material.SetVector("_chinleft_d", new Vector4(_chinleft_d[0].x, _chinleft_d[0].y, _chinleft_d[1].x, _chinleft_d[1].y));
                material.SetVector("_chinright_d", new Vector4(_chinright_d[0].x, _chinright_d[0].y, _chinright_d[1].x, _chinright_d[1].y));
                material.SetVector("_chinleft_u", new Vector4(_chinleft_u[0].x, _chinleft_u[0].y, _chinleft_u[1].x, _chinleft_u[1].y));
                material.SetVector("_chinright_u", new Vector4(_chinright_u[0].x, _chinright_u[0].y, _chinright_u[1].x, _chinright_u[1].y));
                material.SetVector("_nose", new Vector4(_nose[0].x, _nose[0].y, _nose[1].x, _nose[1].y));
                material.SetVector("_noseleft", new Vector4(_noseleft[0].x, _noseleft[0].y, _noseleft[1].x, _noseleft[1].y));
                material.SetVector("_noseright", new Vector4(_noseright[0].x, _noseright[0].y, _noseright[1].x, _noseright[1].y));

                return material;
               // return null;
			}
		} else {
			return null;
		}
	}

	//	void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	//	{
	//		if (SCShader != null)
	//		{
	//			if (WebCamMgr.u_bolTarck)
	//			{
	//				u_tracksuccess = 1;
	//
	//				_faceleft_u = WebCamMgr.GetMid(8);
	//				_faceright_u = WebCamMgr.GetMid(37,38,40,41);
	//				_faceleft = WebCamMgr.GetMid(43,44,46,47);
	//				_faceright = WebCamMgr.GetMid(30);
	//
	//				_eyea = WebCamMgr.GetMid(37, 38, 40, 41);
	//				_eyeb = WebCamMgr.GetMid(43, 44, 46, 47);
	//
	//				_chin = WebCamMgr.GetMid(49);
	//				_chinleft_d = WebCamMgr.GetMid(55);
	//				_chinright_d = WebCamMgr.GetMid(52);
	//				_chinleft_u = WebCamMgr.GetMid(58);
	//				_chinright_u = WebCamMgr.GetMid(49);
	//				_chinleft = WebCamMgr.GetMid(55);
	//				_chinright = WebCamMgr.GetMid(52);
	//
	//				_nose = WebCamMgr.GetMid(30);
	//				_noseleft = WebCamMgr.GetMid(6,7);
	//				_noseright = WebCamMgr.GetMid(9,10);
	//
	//				material.SetInt ("u_tracksuccess", u_tracksuccess);
	//				material.SetFloat ("u_slimFactor", u_slimFactor);
	//				material.SetFloat ("u_bigEyeFactor", u_bigEyeFactor);
	//
	//				material.SetVector("_faceleft_u", new Vector4(_faceleft_u.x, _faceleft_u.y, 0.0f, 0.0f));
	//				material.SetVector("_faceright_u", new Vector4(_faceright_u.x, _faceright_u.y, 0.0f, 0.0f));
	//				material.SetVector("_faceleft", new Vector4(_faceleft.x, _faceleft.y, 0.0f, 0.0f));
	//				material.SetVector("_faceright", new Vector4(_faceright.x, _faceright.y, 0.0f, 0.0f));
	//
	//				material.SetVector("_eyea", new Vector4(_eyea.x, _eyea.y, 0.0f, 0.0f));
	//				material.SetVector("_eyeb", new Vector4(_eyeb.x, _eyeb.y, 0.0f, 0.0f));
	//
	//				material.SetVector("_chin", new Vector4(_chin.x, _chin.y, 0.0f, 0.0f));
	//				material.SetVector("_chinleft_d", new Vector4(_chinleft_d.x, _chinleft_d.y, 0.0f, 0.0f));
	//				material.SetVector("_chinright_d", new Vector4(_chinright_d.x, _chinright_d.y, 0.0f, 0.0f));
	//				material.SetVector("_chinleft_u", new Vector4(_chinleft_u.x, _chinleft_u.y, 0.0f, 0.0f));
	//				material.SetVector("_chinright_u", new Vector4(_chinright_u.x, _chinright_u.y, 0.0f, 0.0f));
	//				material.SetVector("_chinleft", new Vector4(_chinleft.x, _chinleft.y, 0.0f, 0.0f));
	//				material.SetVector("_chinright", new Vector4(_chinright.x, _chinright.y, 0.0f, 0.0f));
	//
	//
	//				material.SetVector("_nose", new Vector4(_nose.x, _nose.y, 0.0f, 0.0f));
	//				material.SetVector("_noseleft", new Vector4(_noseleft.x, _noseleft.y, 0.0f, 0.0f));
	//				material.SetVector("_noseright", new Vector4(_noseright.x, _noseright.y, 0.0f, 0.0f));
	//
	//				GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
	//				Graphics.Blit(sourceTexture, destTexture, material);
	//			}
	//			else
	//			{
	//				Graphics.Blit(sourceTexture, destTexture);
	//			}
	//		}
	//		else
	//		{
	//			Graphics.Blit(sourceTexture, destTexture);
	//		}
	//	}

	void Update ()
	{
		#if UNITY_EDITOR
		if (Application.isPlaying != true) {
			SCShader = Shader.Find ("lidx/filter_beauty_morph");
		}
		#endif
	}

	void OnDisable ()
	{
		if (SCMaterial) {
			DestroyImmediate (SCMaterial);
		}
	}
}