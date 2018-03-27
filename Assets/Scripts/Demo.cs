using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class Demo : Manager
{
	public GameObject objBtnPanel;
	public GameObject objContent;

	public GameObject objButton;

	public GameObject objAll;
	public GameObject objThreefourths;
	public GameObject objOneone;
	public GameObject objTakePicture;
	public GameObject objOnTrackStateSwitch;
	MatrixVectorStruct m_MatrixVector;
	public string str="1.46283,-0.06765,-0.03912,0.0,-0.07521,-1.41784,-0.36063,0.0,-0.01448,0.24720,-0.96886,0.00000,0.00000,0.00000,0.00000,1.00000";

	bool bol=false;
	void Start ()
	{
		objButton.SetActive (true);
		objBtnPanel.SetActive (true);
		objAll.SetActive (true);
		objThreefourths.SetActive (true);
		objOneone.SetActive (true);
		objTakePicture.SetActive (true);
        objOnTrackStateSwitch.SetActive(true);
        GetResNameAndSetUIToggle ();
	}

	/// <summary>
	/// Raises the init unity event.
	/// </summary>
	public void OnInitUnity (GameObject obj)
	{
		GameMgr.message ("FunctionName=InitUnityWebCam");
	}

	public void OnTrackStateSwitch (GameObject obj)
	{
        string Recorder = "";

        if (bol) {
			bol = false;
            Recorder = "StopRecorder";
        } else {
			bol = true;
            Recorder = "StartRecorder";
        }
		GameMgr.message ("FunctionName=RecorderScene|Recorder=" + Recorder);
	}



	public void ChangeCanvasScaler (GameObject obj)
	{
		string Context = "";
		switch (obj.name.ToString ()) {
		case"All":
			Context="1";
			break;
		case"Threefourths":
			Context="2";
			break;
		case"Oneone":
			Context="3";
			break;
		}

		GameMgr.message ("FunctionName=ChangeResolution|Context="+Context);
	}

	public void TakePicture(GameObject obj)
	{
		GameMgr.message ("FunctionName=TakePicture");
//		WebCamMgr.m_tarck2dOr3d = true;
//		WebCamMgr.m_tarckInit = true;
		//Test();
		Test();
	}

	public void Test()
	{
		string[] ar = str.Split (',');
		float[] matrix = new float[ar.Length];
		for (int i = 0; i < ar.Length; i++) {
			matrix [i] = float.Parse (ar [i].ToString());
		}
        //SDK返回矩阵值，重新计算u3d矩阵值，需要实时计算
        //MaterialsMgr.GetTransformMatrix4x4(matrix, ref m_MatrixVector, "Left");
        MaterialsMgr.OnSetActive ("face", true);
		MaterialsMgr.MatrixToValue(ReceiveMgsManager.Localtion, "face", m_MatrixVector);

	}
	void GetResNameAndSetUIToggle ()
	{
		string resPath = Util.AppContentPath () + Util.GetRuntimePlatform ();
		//#if !UNITY_EDITOR
		//            resPath=Application.persistentDataPath + "/" + Util.GetRuntimePlatform();
		//#endif
		Util.Log ("resPath= " + resPath);
		//判断文件夹是否存在
		if (Directory.Exists (resPath)) {
			//获取该路径下文件夹信息
			DirectoryInfo dirInfo = new DirectoryInfo (resPath);
			GameObject toggleObj = Resources.Load ("TestBtnToggle") as GameObject;
			//遍历这个路径下，获取文件夹信息
			foreach (DirectoryInfo dir in dirInfo.GetDirectories()) {
				//输出文件夹名称
				Util.Log ("fdir= " + dir.Name);

				if (toggleObj != null) {
					Toggle toggle = Instantiate (toggleObj).GetComponent<Toggle> ();
					if (toggle != null) {
						toggle.gameObject.transform.SetParent (objContent.transform, false);
						toggle.transform.GetComponentInChildren<Text> ().text = dir.Name;
						toggle.gameObject.name = dir.Name;
						toggle.group = objContent.GetComponent<ToggleGroup> ();
						toggle.isOn = false;
						toggle.onValueChanged.AddListener ((bool isOn) => {
							if (isOn) {
								string param = toggle.gameObject.name;
								string filePath = "file://" + Util.AppContentPath () + Util.GetRuntimePlatform () + "/" + param;
								//#if !UNITY_EDITOR
								//                                filePath=Util.AppContentPath() + Util.GetRuntimePlatform() + "/"+param;
								//#endif
								Util.Log ("按下了 name = " + filePath);
								GameMgr.message ("FunctionName=SpinePath|Path=" + filePath);
							} else {
								//Toggle取消选中事件
								//ReceiveMgr.ReadSpineFilePath ("");
								GameMgr.message ("FunctionName=SpinePath|Path=");
							}
						});
					}
				}
			}
		} else {
			Util.Log ("dir not find");
		}
	}
}
