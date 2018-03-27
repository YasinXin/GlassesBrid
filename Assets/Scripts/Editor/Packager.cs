using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class Packager
{
	static List<AssetBundleBuild> maps = new List<AssetBundleBuild> ();

	///-----------------------------------------------------------
	private const string AssetBundlesOutputPath = "Assets/StreamingAssets";

	[MenuItem ("Examples/Build iPhone Resource_ZIP IOS压缩打包", false, 11)]
	public static void BuildiPhoneResource ()
	{
		BuildTarget target;
		target = BuildTarget.iOS;
		BuildAssetResource (target, "IOS", true);
	}

	[MenuItem ("Examples/Build Android Resource_ZIP ANDROID压缩打包", false, 12)]
	public static void BuildAndroidResource ()
	{
		BuildAssetResource (BuildTarget.Android, "Android", true);
	}

	[MenuItem ("Examples/Build Windows Resource", false, 13)]
	public static void BuildWindowsResource ()
	{
		BuildAssetResource (BuildTarget.StandaloneWindows, "Windows");
	}

	[MenuItem ("Examples/Build OSX Resource", false, 14)]
	public static void BuildOSXResource ()
	{
		BuildAssetResource (BuildTarget.StandaloneOSXIntel, "OSX");
	}

	/// <summary>
	/// 生成绑定素材
	/// </summary>
	static void BuildAssetResource (BuildTarget target, string platform, bool bolzip = false)
	{
		string streamPath = Application.streamingAssetsPath;
		if (!Directory.Exists (streamPath)) {
			Directory.CreateDirectory (streamPath);
			AssetDatabase.Refresh ();
		}

		platform = Path.Combine (AssetBundlesOutputPath, platform);
		if (!Directory.Exists (platform)) {
			Directory.CreateDirectory (platform);
			AssetDatabase.Refresh ();
		}

		Object[] selects = Selection.GetFiltered (typeof(Object), SelectionMode.Assets);

		HandleFile (selects, platform);
		BuildMP3 (selects, platform, target);
		BuildAsset (selects, platform, target);
		AssetDatabase.Refresh ();
		if (bolzip) {
			Zip (selects, platform);
			AssetDatabase.Refresh ();
		}
	}

	/// <summary>
	/// 创建文件夹与拷贝文件到指定路径
	/// </summary>
	/// <param name="selects"></param>
	/// <param name="platform"></param>
	static void HandleFile (Object[] selects, string platform)
	{
		foreach (object str in selects) {
			string bundleName = str.ToString ().Replace (" (UnityEngine.DefaultAsset)", "");
			string assetType = "";
			if (bundleName.Contains ("2d_")) {
				assetType = "2D";
			} else {
				assetType = "3D";
			}
			string originPath = "Assets/Examples/"+assetType+"/Model/" + bundleName;

			string resPath = Path.Combine (platform, bundleName.ToLower ());
		
			if (Directory.Exists (resPath)) {
				Directory.Delete (resPath, true);
			}
			Directory.CreateDirectory (resPath);
			AssetDatabase.Refresh ();

			if (Directory.Exists (originPath)) {
				if (bundleName.Contains ("2d_")) {
					if (Directory.Exists (resPath)) {
						DirectoryInfo dirInfo = new DirectoryInfo (originPath);
						foreach (FileInfo f in dirInfo.GetFiles()) {
							if (f.FullName.Contains ("_config.txt")) {
								if (f.FullName.EndsWith (".meta"))
									continue;
								Util.OnCopyFile (f.FullName, resPath.ToLower (), originPath);
							} else if (f.FullName.Contains (".png")) {
								if (f.FullName.EndsWith (".meta"))
									continue;
								if (f.FullName.Contains ("2d_")) {
									if (f.FullName.Contains (bundleName + "_")) {
										Util.OnCopyFile (f.FullName, resPath.ToLower (), originPath);
									}
								}
							}
						}
					}
				} else {
					if (Directory.Exists (resPath)) {
						DirectoryInfo dirInfo = new DirectoryInfo (originPath);
						foreach (FileInfo f in dirInfo.GetFiles()) {
							if (f.FullName.EndsWith (".meta"))
								continue;
							if (f.FullName.Contains ("_config.txt")) {
								Util.OnCopyFile (f.FullName, resPath.ToLower (), originPath);
							} else if (f.FullName.Contains (".png")) {
								if (f.FullName.Contains ("_map")) {
									Util.OnCopyFile (f.FullName, resPath.ToLower (), originPath);
								}
							}
						}
						if (Directory.Exists (originPath + "/Spine2D")) {
							//3d素材中所有的2d都在Spine2D文件夹下
							DirectoryInfo dirInfoSpine2D = new DirectoryInfo (originPath + "/Spine2D");
							foreach (FileInfo f in dirInfoSpine2D.GetFiles()) {
								if (f.FullName.EndsWith (".meta"))
									continue;
								if (f.FullName.Contains (".png")) {
									Util.OnCopyFile (f.FullName, resPath.ToLower (), originPath + "/Spine2D");
								}
							}
						}
					}
				}
			}

//			//将原路径下的.mp3和config文件拷贝到目标路径
//			if (Directory.Exists (resPath)) {
//				DirectoryInfo dirInfo = new DirectoryInfo (originPath);
//				foreach (FileInfo f in dirInfo.GetFiles()) {
//					if (f.FullName.Contains ("_config.txt")) {
//						if (f.FullName.EndsWith (".meta"))
//							continue;
//							Util.OnCopyFile (f.FullName,resPath,originPath);
//					}else if (f.FullName.Contains (".png")) {
//						if (f.FullName.EndsWith (".meta"))
//							continue;
//						if (f.FullName.Contains ("_map.png")) {//||f.FullName.Contains (bundleName+"_")
//                            Util.OnCopyFile(f.FullName,resPath,originPath);
//						}
//					}
//				}
//			}
		}
	}

	/// <summary>
	/// 打包压缩素材
	/// </summary>
	static void BuildAsset (Object[] selects, string platform, BuildTarget target)
	{
		foreach (object str in selects) {
			maps.Clear ();
			string[] files = null;
			string bundleName = str.ToString ().Replace (" (UnityEngine.DefaultAsset)", "");
			string assetType = "";
			if (bundleName.Contains ("2d_")) {
				assetType = "2D";
			} else {
				assetType = "3D";
			}
			string originPath = "Assets/Examples/"+assetType+"/Model/" + bundleName;
			Dictionary<string,string> materNameDic = new Dictionary<string,string> ();
			if (Directory.Exists (originPath)) {
				if (bundleName.Contains ("2d_")) {
					Util.OnCopyFile (originPath+ "/defaultSpine.png", originPath, "Assets/AssetData");
					files = Directory.GetFiles (originPath, "*.mat");
					AssetDatabase.Refresh ();
					if (files != null) {
						if (files.Length > 0) {
							Texture	defaultSpine = AssetDatabase.LoadAssetAtPath<Texture> (originPath+ "/defaultSpine.png");
							for (int i = 0; i < files.Length; i++) {
								//string fileName = Path.GetFileName (files [i].ToString ());
								Material objSkeletonMaterial = AssetDatabase.LoadAssetAtPath<Material> (files [i].ToString ());
								if (!materNameDic.ContainsKey (objSkeletonMaterial.name.ToString ())) {
									materNameDic.Add (objSkeletonMaterial.name.ToString (), objSkeletonMaterial.mainTexture.name);
								}
								if (objSkeletonMaterial != null && defaultSpine != null) {
									objSkeletonMaterial.mainTexture = defaultSpine;
								}
							}
						}
					}
					AddBuildMap (bundleName, "*.prefab", "Assets/Examples/"+assetType+"/Builds/" + bundleName);
					AddBuildMap (bundleName, "*.png", originPath);
					AddBuildMap (bundleName, "*.jpeg",originPath);
					AddBuildMap (bundleName, "*.controller",originPath);
					AddBuildMap (bundleName, "*.anim",originPath);
					AddBuildMap (bundleName, "*.atlas.txt",originPath);
					AddBuildMap (bundleName, "*.mat",originPath);
					AddBuildMap (bundleName, "*.json",originPath);
					AddBuildMap (bundleName, "*.asset",originPath);

					string resPath = platform + "/" + bundleName.ToLower ();
					BuildAssetBundleOptions options = BuildAssetBundleOptions.DeterministicAssetBundle |
					                                  BuildAssetBundleOptions.ChunkBasedCompression;
					BuildPipeline.BuildAssetBundles (resPath, maps.ToArray (), options, target);

					if (files != null) {
						if (files.Length > 0) {
							for (int i = 0; i < files.Length; i++) {
								string fileName = Path.GetFileName (files [i].ToString ());
								string materName = fileName.Replace (".mat", "");
								if (materNameDic.ContainsKey (materName)) {
									string pngName = materNameDic [materName];
									Texture	objSkeletonTexture = AssetDatabase.LoadAssetAtPath<Texture> (originPath+ "/" + pngName + ".png");
									Material objSkeletonMaterial = AssetDatabase.LoadAssetAtPath<Material> (files [i].ToString ());
									if (objSkeletonMaterial != null && objSkeletonTexture != null) {
										objSkeletonMaterial.mainTexture = objSkeletonTexture;
									}
								}
							}
						}
					}
					AssetDatabase.Refresh ();
					if (files != null) {
						if (files.Length > 0) {
							string	path = Path.Combine (originPath, "defaultSpine.png");
							if (path != "") {
								Util.DelectFile (path);
							}
							AssetDatabase.Refresh ();
						}
					}
				} else {
					string originPath_3d = originPath + "/Spine2D";
					if (Directory.Exists (originPath_3d)) {
						Util.OnCopyFile (originPath_3d+ "/defaultSpine.png", originPath_3d, "Assets/AssetData");
						files = Directory.GetFiles (originPath_3d, "*.mat");
						AssetDatabase.Refresh ();
						if (files != null) {
							if (files.Length > 0) {
								Texture	defaultSpine = AssetDatabase.LoadAssetAtPath<Texture> (originPath_3d+ "/defaultSpine.png");
								for (int i = 0; i < files.Length; i++) {
									//string fileName = Path.GetFileName (files [i].ToString ());
									Material objSkeletonMaterial = AssetDatabase.LoadAssetAtPath<Material> (files [i].ToString ());
									if (!materNameDic.ContainsKey (objSkeletonMaterial.name.ToString ())) {
										materNameDic.Add (objSkeletonMaterial.name.ToString (), objSkeletonMaterial.mainTexture.name);
									}
									if (objSkeletonMaterial != null && defaultSpine != null) {
										objSkeletonMaterial.mainTexture = defaultSpine;
									}
								}
							}
						}
						AddBuildMap (bundleName, "*.mat",originPath_3d);
					}
					AddBuildMap (bundleName, "*.prefab", "Assets/Examples/"+assetType+"/Builds/" + bundleName);
					AddBuildMap (bundleName, "*.png", originPath);
					AddBuildMap (bundleName, "*.jpeg",originPath);
					AddBuildMap (bundleName, "*.controller",originPath);
					AddBuildMap (bundleName, "*.anim",originPath);
					AddBuildMap (bundleName, "*.atlas.txt",originPath);
					AddBuildMap (bundleName, "*.mat",originPath);
					AddBuildMap (bundleName, "*.json",originPath);
					AddBuildMap (bundleName, "*.asset",originPath);

					string resPath = platform + "/" + bundleName.ToLower ();
					BuildAssetBundleOptions options = BuildAssetBundleOptions.DeterministicAssetBundle |
						BuildAssetBundleOptions.ChunkBasedCompression;
					BuildPipeline.BuildAssetBundles (resPath, maps.ToArray (), options, target);

					if (files != null) {
						if (files.Length > 0) {
							for (int i = 0; i < files.Length; i++) {
								string fileName = Path.GetFileName (files [i].ToString ());
								string materName = fileName.Replace (".mat", "");
								if (materNameDic.ContainsKey (materName)) {
									string pngName = materNameDic [materName];
									Texture	objSkeletonTexture = AssetDatabase.LoadAssetAtPath<Texture> (originPath_3d+ "/" + pngName + ".png");
									Material objSkeletonMaterial = AssetDatabase.LoadAssetAtPath<Material> (files [i].ToString ());
									if (objSkeletonMaterial != null && objSkeletonTexture != null) {
										objSkeletonMaterial.mainTexture = objSkeletonTexture;
									}
								}
							}
						}
					}
					AssetDatabase.Refresh ();
					if (files != null) {
						if (files.Length > 0) {
							string	path = Path.Combine (originPath_3d, "defaultSpine.png");
							if (path != "") {
								Util.DelectFile (path);
							}
							AssetDatabase.Refresh ();
						}
					}
				}
			}

//			AddBuildMap (bundleName, "*.prefab", "Assets/Examples/Builds/" + bundleName);
//			AddBuildMap (bundleName, "*.png", "Assets/Examples/Model/" + bundleName);
//			AddBuildMap (bundleName, "*.jpeg", "Assets/Examples/Model/" + bundleName);
//			AddBuildMap (bundleName, "*.controller", "Assets/Examples/Model/" + bundleName);
//			AddBuildMap (bundleName, "*.anim", "Assets/Examples/Model/" + bundleName);
//			AddBuildMap (bundleName, "*.atlas.txt", "Assets/Examples/Model/" + bundleName);
//			AddBuildMap (bundleName, "*.mat", "Assets/Examples/Model/" + bundleName);
//			AddBuildMap (bundleName, "*.json", "Assets/Examples/Model/" + bundleName);
//			AddBuildMap (bundleName, "*.asset", "Assets/Examples/Model/" + bundleName);
//			string resPath = platform + "/" + bundleName.ToLower ();
//			BuildAssetBundleOptions options = BuildAssetBundleOptions.DeterministicAssetBundle |
//			                                  BuildAssetBundleOptions.ChunkBasedCompression;
//			BuildPipeline.BuildAssetBundles (resPath, maps.ToArray (), options, target);
			AssetDatabase.Refresh ();
		}
	}

	/// <summary>
	/// 打包素材
	/// </summary>
	static void BuildMP3 (Object[] selects, string platform, BuildTarget target)
	{
		string ExtName = ".unity3d";                   //素材扩展名
		foreach (object str in selects) {
			string bundleName = str.ToString ().Replace (" (UnityEngine.DefaultAsset)", "").ToLower ();
			string assetType = "";
			if (bundleName.Contains ("2d_")) {
				assetType = "2D";
			} else {
				assetType = "3D";
			}
			string fullPath = "Assets/Examples/"+assetType+"/Model/" + bundleName;
			string resPath = platform + "/" + bundleName;
			uint crc = 0;
			//公共资源入栈;
			BuildPipeline.PushAssetDependencies ();
//			string fullPath = "Assets/Examples/Model/" + bundleName;
			//获取指定路径下面的所有资源文件
			if (Directory.Exists (fullPath)) {
				DirectoryInfo direction = new DirectoryInfo (fullPath);
				FileInfo[] files = direction.GetFiles ("*", SearchOption.AllDirectories);
				for (int i = 0; i < files.Length; i++) {
					if (files [i].Name.Contains (".mp3")) {
						if (files [i].Name.EndsWith (".meta")) {
							continue;
						}
						Debug.Log ("Name:" + files [i].Name);
						string filePath = fullPath + "/" + files [i].Name;
						Object obj = AssetDatabase.LoadMainAssetAtPath (filePath);
						//Build;
						if (BuildPipeline.BuildAssetBundle (obj, null, resPath + "/" + files [i].Name + ExtName, out crc, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle, target)) {
							AssetDatabase.Refresh ();
						}
					}
				}
			}

			BuildPipeline.PopAssetDependencies ();
			AssetDatabase.Refresh ();
		}
	}

	static void AddBuildMap (string bundleName, string pattern, string path)
	{
		string[] files = Directory.GetFiles (path, pattern);
		if (files.Length == 0)
			return;
		List<string> strlist = new List<string> ();
		for (int i = 0; i < files.Length; i++) {
			string fileName = Path.GetFileName (files [i].ToString ());
			if (fileName.Contains ("png")) {
				if (fileName.Contains ("_map")) {
					continue;
				}
				if (fileName.Contains ("2d_")) {
					if (fileName.Contains (bundleName + "_")) {
						continue;
					}
				}
			}
			Debug.Log ("fileName==>" + fileName);
			strlist.Add (files [i].Replace ('\\', '/'));
		}
		if (strlist.Count == 0)
			return;

		AssetBundleBuild build = new AssetBundleBuild ();
		build.assetBundleName = bundleName + ".unity3d".ToLower ();
		build.assetNames = strlist.ToArray ();
		maps.Add (build);
	}

	/// <summary>
	/// 压缩文件夹
	/// </summary>
	/// <param name="selects">Selects.</param>
	/// <param name="platform">Platform.</param>
	static void Zip (Object[] selects, string platform)
	{
		foreach (object str in selects) {
			string bundleName = str.ToString ().Replace (" (UnityEngine.DefaultAsset)", "").ToLower ();
			string resPath = Path.Combine (platform, bundleName);
			ZipUtility.Zip (new string[]{ resPath }, resPath + ".zip");
			if (Directory.Exists (resPath)) {
				Directory.Delete (resPath, true);
			}
		}
	}
}