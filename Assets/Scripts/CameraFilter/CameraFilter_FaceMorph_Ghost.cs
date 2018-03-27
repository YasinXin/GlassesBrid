using UnityEngine;
using System.Text;
[ExecuteInEditMode]
[AddComponentMenu("Camera Filter/FaceMorph/Ghost")]
public class CameraFilter_FaceMorph_Ghost:Manager {
	
	#region Variables
	public Shader SCShader;
	private Material SCMaterial;

	public Vector2 _chin= Vector2.zero;//68Landmarks: 8               106Landmarks: 16
	public Vector2 _eyea= Vector2.zero;//68Landmarks:ave 37,38,40,41  106Landmarks: 74
	public Vector2 _eyeb= Vector2.zero;//68Landmarks:ave 43,44,46,47  106Landmarks: 77
	public Vector2 _nose= Vector2.zero;//68Landmarks: 30              106Landmarks: 46
	public Vector2 _chinleft= Vector2.zero;//5
	public Vector2 _chinright= Vector2.zero;//11
	public Vector2 _mouthleft= Vector2.zero;  //49
	public Vector2 _mouthright= Vector2.zero; //55
	public Vector2 _mouthtop= Vector2.zero;//52
	public Vector2 _mouthbottom= Vector2.zero;//58
	public float u_mouthopenparam=0.0f;
	public int u_tracksuccess=0;

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
		SCShader = Shader.Find("CameraFilter/FaceMorph_Ghost");
		if (!SystemInfo.supportsImageEffects)
		{
			enabled = false;
			return;
		}
	}

	//void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	//{
	//	if (SCShader != null)
	//	{
	//		if (WebCamMgr.u_bolTarck)
	//		{
	//			u_tracksuccess = 1;
	//			u_mouthopenparam = WebCamManager.u_mouthopenparam;

	//			_chin = WebCamMgr.GetMid(8);
	//			_eyea = WebCamMgr.GetMid(37,38,40,41);
	//			_eyeb = WebCamMgr.GetMid(43,44,46,47);
	//			_nose = WebCamMgr.GetMid(30);
	//			_chinleft = WebCamMgr.GetMid(5);
	//			_chinright = WebCamMgr.GetMid(11);
	//			_mouthleft = WebCamMgr.GetMid(49);
	//			_mouthright = WebCamMgr.GetMid(55);
	//			_mouthtop = WebCamMgr.GetMid(52);
	//			_mouthbottom = WebCamMgr.GetMid(58);

	//			material.SetInt ("u_tracksuccess", u_tracksuccess);
	//			material.SetFloat ("u_mouthopenparam", u_mouthopenparam);
	//			material.SetVector("_chin", new Vector4(_chin.x, _chin.y, 0.0f, 0.0f));
	//			material.SetVector("_eyea", new Vector4(_eyea.x, _eyea.y, 0.0f, 0.0f));
	//			material.SetVector("_eyeb", new Vector4(_eyeb.x, _eyeb.y, 0.0f, 0.0f));
	//			material.SetVector("_nose", new Vector4(_nose.x, _nose.y, 0.0f, 0.0f));
	//			material.SetVector("_chinleft", new Vector4(_chinleft.x, _chinleft.y, 0.0f, 0.0f));
	//			material.SetVector("_chinright", new Vector4(_chinright.x, _chinright.y, 0.0f, 0.0f));
	//			material.SetVector("_mouthleft", new Vector4(_mouthleft.x, _mouthleft.y, 0.0f, 0.0f));
	//			material.SetVector("_mouthright", new Vector4(_mouthright.x, _mouthright.y, 0.0f, 0.0f));
	//			material.SetVector("_mouthtop", new Vector4(_mouthtop.x, _mouthtop.y, 0.0f, 0.0f));
	//			material.SetVector("_mouthbottom", new Vector4(_mouthbottom.x, _mouthbottom.y, 0.0f, 0.0f));

	//			GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
	//			Graphics.Blit(sourceTexture, destTexture, material);
	//		}
	//		else
	//		{
	//			u_tracksuccess = 0;
	//			u_mouthopenparam = 0.0f;
	//			Graphics.Blit(sourceTexture, destTexture);
	//		}
	//	}
	//	else
	//	{
	//		Graphics.Blit(sourceTexture, destTexture);
	//	}
	//}

	void Update()
	{
		#if UNITY_EDITOR
		if (Application.isPlaying != true)
		{
			SCShader = Shader.Find("CameraFilter/FaceMorph_Ghost");
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
