using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Windows.Forms;
using UObject = UnityEngine.Object;
using Spine.Unity;
using UnityEngine.UI;
using System.Collections.Generic;

public class MyEditorWindow : EditorWindow
{
	[UnityEditor.MenuItem ("Examples/MyEditorWindow")]
	static void AddWindow ()
	{
		//创建窗口
		Rect wr = new Rect (0, 0, 500, 500);
		MyEditorWindow window = (MyEditorWindow)EditorWindow.GetWindowWithRect (typeof(MyEditorWindow), wr, true, "widow name");
		window.Show ();
	}

	string CompentPath;
	//接受选择文件的路径
	string UnityPath;
	//接受转成功后的路径 也就是Unity所需要的路径
	bool bol2dSpine = false;
//显示2d
	bool bolChangeExtension = false;
//是否显示修改素材扩展名方法
	bool bolCreatePrefab = false;
//是否显示创建预设方法

	//绘制窗口时调用
	void OnGUI ()
	{
		bol2dSpine = EditorGUILayout.Toggle ("2d", bol2dSpine);
		if (bol2dSpine) {
			bolChangeExtension = EditorGUILayout.Toggle ("是否修改素材扩展名", bolChangeExtension);
			if (bolChangeExtension) {
				//输入框控件
				UnityPath = EditorGUILayout.TextField ("选择修改扩展名素材路径:", UnityPath);
				if (GUILayout.Button ("选择文件夹", GUILayout.Width (200))) {
					SelectionPath ();
				}
				if (GUILayout.Button ("修改扩展名素材", GUILayout.Width (200))) {
					OnChangeExtension (UnityPath);
				}
			}

			bolCreatePrefab = EditorGUILayout.Toggle ("创建预设方法", bolCreatePrefab);
			if (bolCreatePrefab) {
				if (GUILayout.Button ("创建预设", GUILayout.Width (200))) {
					OnCreatePrefab ();
				}
			}
		}
		if (GUILayout.Button ("关闭窗口", GUILayout.Width (200))) {
			//关闭窗口
			this.Close ();
		}
	}

	void OnInspectorUpdate ()
	{
//        Debug.Log("窗口面板的更新");
		//这里开启窗口的重绘，不然窗口信息不会刷新
		this.Repaint ();
	}

	void OnSelectionChange ()
	{
		//当窗口出去开启状态，并且在Hierarchy视图中选择某游戏对象时调用
		foreach (Transform t in Selection.transforms) {
			//有可能是多选，这里开启一个循环打印选中游戏对象的名称
			Debug.Log ("OnSelectionChange " + t.name);
		}
	}

	/// <summary>
	/// 选择文件夹
	/// </summary>
	void SelectionPath ()
	{
		FolderBrowserDialog fb = new FolderBrowserDialog ();   //创建控件并实例化
		fb.Description = "选择文件夹";
		fb.RootFolder = Environment.SpecialFolder.MyComputer;  //设置默认路径
		fb.ShowNewFolderButton = false;   //创建文件夹按钮关闭

		//如果按下弹窗的OK按钮
		if (fb.ShowDialog () == DialogResult.OK) {
			//接受路径
			CompentPath = fb.SelectedPath;
			//将路径中的 \ 替换成 /            由于unity路径的规范必须转
			UnityPath = CompentPath.Replace (@"\", "/");
		}
	}

	/// <summary>
	/// 根据路径修改文件扩展名
	/// </summary>
	/// <param name="originPath"></param>
	void OnChangeExtension (string originPath)
	{
		if (originPath == "")
			return;

		DirectoryInfo dirInfo = new DirectoryInfo (originPath);
		DirectoryInfo[] di = dirInfo.GetDirectories ();
		foreach (DirectoryInfo NextFolder in di) {
			FileInfo[] fileInfo = NextFolder.GetFiles ();
			foreach (FileInfo f in fileInfo) {  //遍历文件
				if (f.FullName.Contains ("_config.json")) {
					if (f.FullName.EndsWith (".meta"))
						continue;
					string dfileName = Path.ChangeExtension (f.ToString (), ".txt");
					File.Move (f.ToString (), dfileName);
				} else if (f.FullName.Contains (".atlas")) {
					if (f.FullName.EndsWith (".meta"))
						continue;
					if (f.FullName.EndsWith (".atlas.txt"))
						continue;
					string dfileName = Path.ChangeExtension (f.ToString (), ".atlas.txt");
					File.Move (f.ToString (), dfileName);
				}
			}
		}
		Debug.Log ("SetChangeExtension===>end");
	}

	/// <summary>
	/// 创建预设文件
	/// </summary>
	void OnCreatePrefab ()
	{
		GameObject prefab_ObjSkeleton = AssetDatabase.LoadAssetAtPath<GameObject> ("Assets/AssetData/ObjSkeletonAnimation.prefab");
		if (prefab_ObjSkeleton == null) {
			Debug.LogError ("Assets/AssetData/ObjSkeletonAnimation.prefab is null");
			return;
		}

		GameObject prefab_ObjRawImage = AssetDatabase.LoadAssetAtPath<GameObject> ("Assets/AssetData/ObjRawImage.prefab");
		if (prefab_ObjRawImage == null) {
			Debug.LogError ("Assets/AssetData/ObjRawImage.prefab is null");
			return;
		}

		UObject[] selects = Selection.GetFiltered (typeof(UObject), UnityEditor.SelectionMode.Assets);
		if (selects.Length > 0) {
			foreach (object str in selects) {
				string bundleName = str.ToString ().Replace (" (UnityEngine.DefaultAsset)", "");
				Debug.Log ("bundleName====>" + bundleName);
				string assetType = "";
				string origin = "";
				if (bundleName.Contains ("2d_")) {
					assetType = "2D";
					origin = "Assets/Examples/" + assetType;
				} else {
					assetType = "3D";
					origin = "Assets/Examples/" + assetType;
				}

				if (!Directory.Exists (origin)) {
					Directory.CreateDirectory (origin);
					AssetDatabase.Refresh ();
				}

				string originPath = origin + "/Model/" + bundleName;
				string resPath = Path.Combine (origin + "/Builds/", bundleName);
				Debug.Log ("resPath====>" + resPath);
				if (!Directory.Exists (resPath)) {
					Directory.CreateDirectory (resPath);
					AssetDatabase.Refresh ();
				}

				if (Directory.Exists (resPath)) {
					if (assetType == "3D") {
						originPath = originPath + "/Spine2D";
					}

					if (Directory.Exists (originPath)) {
						DirectoryInfo dirInfo = new DirectoryInfo (originPath);
						foreach (FileInfo f in dirInfo.GetFiles()) {
							if (f.FullName.Contains ("_SkeletonData.asset")) {
								if (f.FullName.EndsWith (".meta"))
									continue;
								string fileName = Path.GetFileName (f.FullName);
								Debug.Log ("fssss====>" + f);
								string asset = fileName.Replace ("_SkeletonData.asset", "");
								Debug.Log ("f====>" + f + "   FullName=" + asset.ToString ());
								SkeletonAnimation skeleton = prefab_ObjSkeleton.GetComponent<SkeletonAnimation> ();
								SkeletonDataAsset prefabss = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset> (originPath + "/" + asset + "_SkeletonData.asset");
								skeleton.skeletonDataAsset = prefabss;
								skeleton.AnimationName = "animation";
								if (Util.FileIsExistence (resPath + "/" + asset + ".prefab")) {
									Util.DelectFile (resPath + "/" + asset + ".prefab");
									AssetDatabase.Refresh ();
								}
								PrefabUtility.CreatePrefab (resPath + "/" + asset + ".prefab", prefab_ObjSkeleton);
							} 
						}
					}
				}
			}
		}
	}

	void OnDestroy ()
	{
		Debug.Log ("当窗口关闭时调用");
	}
}