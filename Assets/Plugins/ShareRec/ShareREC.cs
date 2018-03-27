﻿using UnityEngine;
using System;
using System.Collections;
using System.Threading;

namespace cn.sharerec {
	public class ShareREC : MonoBehaviour {
	#if UNITY_ANDROID
		private const int STATE_IDLE = 0;
		private const int STATE_STARTING = 1;
		private const int STATE_STARTED = 2;
		private const int STATE_PAUSING = 3;
		private const int STATE_PAUSED = 4;
		private const int STATE_RESUMING = 5;
		private const int STATE_RESUMED = STATE_STARTED;
		private const int STATE_STOPPING = 7;
		private const int STATE_STOPPED = STATE_IDLE;

		private const int RETURN_FROM_SHARE = -100;
		private const int RETURN_FROM_VIDEO_CENTER = -101;
		private const int RETURN_FROM_PROFILE = -102;

		private const int RECBAR_PROFILE = 1;
		private const int RECBAR_START = 2;
		private const int RECBAR_STOP = 3;
		private const int RECBAR_VIDEOCENTER = 4;

        public string AppKey = "1a1459e0b3a80";
        public string AppSecret = "5351b8196a6171dc52e564e08ee96ee2";
        public LevelMaxFrameSize MaxFrameSize = LevelMaxFrameSize.LEVEL_1280_720;
		public LevelVideoQuality VideoQuality = LevelVideoQuality.LEVEL_HIGH;
		public int MinDuration = 4;
		public bool RecordAudioFromMic = true;
		public bool SoftwareAudioEncoder = false;
		public bool SoftwareVideoEncoder = false;
		public bool RecordGUILayer = true;
		public string CacheFolder = "/sdcard/LidCV/RecorderVideo";

		private static OnFrameBeginHandler beginHanlder;
		private static OnFrameEndHandler endHanlder;
		private int curAction;

		public static OnRecorderStarting OnRecorderStartingHandler;
		public static OnRecorderStarted OnRecorderStartedHandler;
		public static OnRecorderPausing OnRecorderPausingHandler;
		public static OnRecorderPaused OnRecorderPausedHandler;
		public static OnRecorderResuming OnRecorderResumingHandler;
		public static OnRecorderResumed OnRecorderResumedHandler;
		public static OnRecorderStopping OnRecorderStoppingHandler;
		public static OnRecorderStopped OnRecorderStoppedHandler;
		
		public static OnReturnFromShare OnReturnFromShareHandler;
		public static OnReturnFromProfile OnReturnFromProfileHandler;
		public static OnReturnFromVideoCenter OnReturnFromVideoCenterHandler;

		public static OnPlatformSelected OnPlatformSelectedHandler;

		public void OnAwakeUp(string path) {
			try {
                CacheFolder = path;
                ShareRECImpl.Init(AppKey, AppSecret, gameObject.name, (int) MaxFrameSize);
				ShareRECImpl.SetVideoQuality((int) VideoQuality);
                //获取手机型号信息,判断是否开启GLES30API
                bool NeedOpenGLES30 = false;
				if(NeedOpenGLES30)
				{
					useGLES30API();
				}
				ShareRECImpl.SetForceSoftwareEncoding(SoftwareVideoEncoder, SoftwareAudioEncoder);
				ShareRECImpl.SetCacheFolder(CacheFolder);
				ShareRECImpl.SetMinDuration(MinDuration);
				if (RecordAudioFromMic) {
					ShareRECImpl.SetRecordAudioFromMic();
				}
				if (RecordGUILayer) {
					ShareRECImpl.SetRecordGUILayer();
				}

			} catch (Exception e) {}
			InitializeFrontMostCamera();
			InitializeBackMostCamera();
		}

		private void InitializeFrontMostCamera() {
			if (GameObject.Find("FrontMostCamera") == null) {
				GameObject cameraObject = new GameObject();
				Camera camera = cameraObject.AddComponent<Camera>();
				camera.name = "FrontMostCamera";
				camera.clearFlags = CameraClearFlags.Nothing;
				camera.cullingMask = 0;
				camera.depth = Single.MinValue;
				beginHanlder = camera.gameObject.AddComponent<OnFrameBeginHandler>();
				beginHanlder.enabled = false;
				cameraObject.SetActive(true);
				DontDestroyOnLoad(cameraObject);
			}
		}
		
		private void InitializeBackMostCamera() {
			if (GameObject.Find("BackMostCamera") == null) {
				GameObject cameraObject = new GameObject();
				Camera camera = cameraObject.AddComponent<Camera>();
				camera.name = "BackMostCamera";
				camera.clearFlags = CameraClearFlags.Nothing;
				camera.cullingMask = 0;
				camera.depth = Single.MaxValue;
				endHanlder = camera.gameObject.AddComponent<OnFrameEndHandler>();
				endHanlder.enabled = false;
				cameraObject.SetActive(true);
				DontDestroyOnLoad(cameraObject);
			}
		}

		private void setUnityRenderEvent(string eventID) {
			#if (UNITY_4_5 || UNITY_4_6)
				int[] iparameters = new int[1]{0};
				string[] parameters = eventID.Split('|');
				int i = 0; 
				
				foreach (string parameter in parameters){
					if (!Int32.TryParse(parameter, out iparameters[i] )) {
						return;
					}
					i++;
				}
				ShareRECImpl.setFBOInfo(iparameters[0]);
			#endif
		}
		
		private void onStateChange(string action) {
			int iAction = -1;
			if (!Int32.TryParse(action, out iAction)) {
				return;
			}
			
			switch (iAction) {
			case STATE_STARTING: {
				if (OnRecorderStartingHandler != null) {
					OnRecorderStartingHandler();
				}
			} break;
			case STATE_STARTED: {
				if (curAction == STATE_RESUMING) {
					if (OnRecorderResumedHandler != null) {
						OnRecorderResumedHandler();
					}
				} else if (OnRecorderStartedHandler != null) {
					OnRecorderStartedHandler();
				}
			} break;
			case STATE_PAUSING: {
				if (OnRecorderPausingHandler != null) {
					OnRecorderPausingHandler();
				}
			} break;
			case STATE_PAUSED: {
				if (OnRecorderPausedHandler != null) {
					OnRecorderPausedHandler();
				}
			} break;
			case STATE_RESUMING: {
				if (OnRecorderResumingHandler != null) {
					OnRecorderResumingHandler();
				}
			} break;
			case STATE_STOPPING: {
				if (OnRecorderStoppingHandler != null) {
					OnRecorderStoppingHandler();
				}
			} break;
			case STATE_STOPPED: {
				beginHanlder.enabled = false;
				endHanlder.enabled = false;
				ShareRECImpl.ReleaseRenderTexture();
				if (OnRecorderStoppedHandler != null) {
					OnRecorderStoppedHandler();
				}
			} break;
			}
			
			curAction = iAction;
		}
		
		private void onReturnGame(string action) {
			int iAction = 0;
			if (!Int32.TryParse(action, out iAction)) {
				return;
			}
			
			switch (iAction) {
			case RETURN_FROM_SHARE: {
				if (OnReturnFromShareHandler != null) {
					OnReturnFromShareHandler();
				}
			} break;
			case RETURN_FROM_PROFILE: {
				if (OnReturnFromProfileHandler != null) {
					OnReturnFromProfileHandler();
				}
			} break;
			case RETURN_FROM_VIDEO_CENTER: {
				if (OnReturnFromVideoCenterHandler != null) {
					OnReturnFromVideoCenterHandler();
				}
			} break;
			}
		}

		private void onRecBarAction(string action) {
			int iAction = -1;
			if (!Int32.TryParse(action, out iAction)) {
				return;
			}
			
			switch (iAction) {
				case RECBAR_PROFILE: {
					// 打开个人中心 (show user profile page)
					ShareREC.ShowProfile(); 
				} break;
				case RECBAR_START: {
					if (ShareREC.IsAvailable()) {
						// 启动录制 (start recording)
						ShareREC.StartRecorder();
					}
				} break;
				case RECBAR_STOP: {
					// 停止录制 (stop recording)
					ShareREC.StopRecorder();
				} break;
				case RECBAR_VIDEOCENTER: {
					// 打开视频中心 (show video center)
					ShareREC.ShowVideoCenter();
				} break;
			}
		}

		private void onPlatformSelected(string action) {
			if (OnPlatformSelectedHandler != null) {
				OnPlatformSelectedHandler(action, new MP4(AppKey, AppSecret, action));
			}
		}

		/// <summary>
		/// 此方法在录制模块启动时被调用(This method will be called when the recorder module is starting.)
		/// </summary>
		public delegate void OnRecorderStarting();
		
		/// <summary>
		/// 此方法在录制模块启动后被调用(This method will be called when the recorder module is started.)
		/// </summary>
		public delegate void OnRecorderStarted();
		
		/// <summary>
		/// 此方法在录制模块暂停时被调用(This method will be called when the recorder module is pausing.)
		/// </summary>
		public delegate void OnRecorderPausing();
		
		/// <summary>
		/// 此方法在录制模块暂停后被调用(This method will be called when the recorder module is paused.)
		/// </summary>
		public delegate void OnRecorderPaused();
		
		/// <summary>
		/// 此方法在录制模块恢复时被调用(This method will be called when the recorder module is resuming.)
		/// </summary>
		public delegate void OnRecorderResuming();
		
		/// <summary>
		/// 此方法在录制模块恢复后被调用(This method will be called when the recorder module is resumed.)
		/// </summary>
		public delegate void OnRecorderResumed();
		
		/// <summary>
		/// 此方法在录制模块停止时被调用(This method will be called when the recorder module is stopping.)
		/// </summary>
		public delegate void OnRecorderStopping();
		
		/// <summary>
		/// 此方法在录制模块停止后被调用(This method will be called when the recorder module is stopped.)
		/// </summary>
		public delegate void OnRecorderStopped();
		
		/// <summary>
		/// 此方法在分享页面关闭后调用(This method will be called after the Sharing Page is closed)
		/// </summary>
		public delegate void OnReturnFromShare();
		
		/// <summary>
		/// 此方法在资料页面关闭后调用(This method will be called after the Profile Page is closed)
		/// </summary>
		public delegate void OnReturnFromProfile();
		
		/// <summary>
		/// 此方法在视频中心页面关闭后调用(This method will be called after the Video Center Page is closed)
		/// </summary>
		public delegate void OnReturnFromVideoCenter();

		/// <summary>
		/// 此方法在自定义平台被选中时调用(This method will be called when custom platform is selected)
		/// </summary>
		public delegate void OnPlatformSelected(string name, MP4 mp4);


		/// <summary>
		/// 启用GLES30
		/// </summary>
		public static void useGLES30API() {
			ShareRECImpl.useGLES30API();
		}

		// =======================================

		/// <summary>
		/// 设置视频描述文本(Sets the description of the video.)
		/// </summary>
		public static void SetText(string text) {
			ShareRECImpl.SetText(text);
		}
		
		/// <summary>
		/// 添加视频的自定义属性(Adds the custom attributes of the video.)
		/// </summary>
		public static void AddCustomAttr(string key, string value) {
			ShareRECImpl.AddCustomAttr(key, value);
		}

		/// <summary>
		/// 在视屏预览界面菜单添加自定义分享平台(Sets custom share platform in video preview menu)
		/// </summary>
		public static void AddCustomPlatform(string name) {
			ShareRECImpl.AddCustomPlatform(name);
		}

		// =======================================

		/// <summary>
		/// 判断ShareRec是否支持当前的设备(Determines whether ShareRec is available for the current device.)
		/// </summary>
		public static bool IsAvailable() {
			return ShareRECImpl.IsAvailable();
		}
		private static Boolean recorder = false;
		private static Boolean pause = false;
		private static void recordMic(){
			//Debug.LogError (" ====================== recordMic begin ");
			int record = 0;
			while(recorder){
				record++;
				//Debug.LogError (" ================= recorder now "+record +" recorder : "+recorder);
				if(!pause){
					int SampleRate = 8000;
					Microphone.End (null);
					AudioClip clip = Microphone.Start (null,true,1,SampleRate);
				//	Debug.LogError (" ================= Microphone recorder " );
					float[] recordData = new float[clip.samples];

				//	Debug.LogError (" ================= Microphone recordData.Length : "+recordData.Length );
					clip.GetData (recordData,0);

					//byte[] recordData = clip.GetData ();
					//byte[] recordData = AudioClipCompressor.CompressAudioClip(clip);
					byte[] recordByte = new byte[recordData.Length*4];
					for(int i = 0;i<recordData.Length;i++){
					//	Debug.LogError (" ================= i : "+i);
						byte[] nowByte = BitConverter.GetBytes(recordData[i]);	
						for(int j = 0;j<nowByte.Length;j++){
							recordByte[i*4+i%4] = nowByte[j];	
				//			Debug.LogError (" ================= j : "+j);
						}
					}
					OfferSample (recordByte,0,recordData.Length);	
				//	Debug.LogError (" ================= end ");
					Microphone.End (null);
				}
			}

		}

		/// <summary>
		/// 启动录制模块 (Start the recorder module)
		/// </summary>
		public static void StartRecorder() {
			if (ShareRECImpl.CanStart()) {

			//	recorder = true;
			//	pause = false;
			//	Debug.LogError (" ================= StartRecorder ");
			//	PrepareSoundCopying (1,8000,8000*2);
			//	ThreadStart ts = new ThreadStart(recordMic);
			//	Thread t = new Thread (ts);
		//		Debug.LogError (" ================= StartRecorder 111 ");
			//	t.Start ();
			//	Debug.LogError (" ================= StartRecorder 222 ");
				beginHanlder.enabled = true;
				endHanlder.enabled = true;

				ShareRECImpl.InitRenderTexture();
				ShareRECImpl.Start();
		//		Debug.LogError (" ================= StartRecorder 333");
			}
		}

		/// <summary>
		/// 暂停录制模块(Pauses the recorder module)
		/// </summary>
		public static void PauseRecorder() {
			pause = true;
			ShareRECImpl.Pause();
		}

		/// <summary>
		/// 恢复录制(Resumes the recorder module)
		/// </summary>
		public static void ResumeRecorder() {
			ShareRECImpl.Resume();
			pause = false;
		}

		/// <summary>
		/// 停止录制模块 (Stop the recorder module)
		/// </summary>
		public static void StopRecorder() {
			ShareRECImpl.Stop();
			recorder = false; 
		}

		/// <summary>
		/// 打开视频中心(Shows the video center.)
		/// </summary>
		public static void ShowVideoCenter() {
			ShareRECImpl.ShowVideoCenter();
		}

		/// <summary>
		/// 显示用户资料(Shows the user profile.)
		/// </summary>
		public static void ShowProfile() {
			ShareRECImpl.ShowProfile();
		}

		/// <summary>
		/// 显示分享页面(Shows the share page.)
		/// </summary>
		public static void ShowShare() {
			ShareRECImpl.ShowShare();
		}

		/// <summary>
		/// 清除ShareRec的缓存目录(Clears the cache folder of ShareRec.)
		/// </summary>
		public static void ClearCache() {
			ShareRECImpl.ClearCache();
		}

		/// <summary>
		/// 列出本地已经缓存的视频(Lists the local videos.)
		/// </summary>
		public static long[] ListLocalVideos() {
			return ShareRECImpl.ListLocalVideos();
		}

		/// <summary>
		/// 通过缓存的视频ID获取本地路径(Gets the local video path by its ID.)
		/// </summary>
		public static string GetLocalVideoPath(long videoId) {
			return ShareRECImpl.GetLocalVideoPath(videoId);
		}

		/// <summary>
		/// 删除缓存视频(Deletes the local video by its ID.)
		/// </summary>
		public static void DeleteLocalVideo(long videoId) {
			ShareRECImpl.DeleteLocalVideo(videoId);
		}

		/// <summary>
		/// 添加要录屏的cmaera(add record camera.)
		/// </summary>
		public static void addCameraRecord( RenderTexture src ) {
			ShareRECImpl.AddCameraRecord (src );
		}

		public static RECBar GetRECBar(MonoBehaviour script) {
			return ShareRECImpl.GetRECBar(script.gameObject.name, "onRecBarAction");
		}


		/// <summary>
		/// 不使用默认的record进行录音，使用自定义的录音方式录音  Prepares the sound copying.
		/// 注意，需要再 执行 StartRecorder（）方法之前调用，否则无效
		/// </summary>
		/// <param name="channelCount">Channel count.</param>
		/// <param name="sampleRate">Sample rate.</param>
		/// <param name="maxBufferSizeInBytes">Max buffer size in bytes.</param>
		public static void PrepareSoundCopying(int channelCount, int sampleRate, int maxBufferSizeInBytes){
			ShareRECImpl.PrepareSoundCopying (channelCount,sampleRate, maxBufferSizeInBytes);
		}

		/// <summary>
		/// 将录音得到的 数据添加到录制的视频数据里 Offers the sample.
		/// </summary>
		/// <param name="sample">Sample.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="len">Length.</param>
		public static void OfferSample(byte[] sample, int offset, int len){
			ShareRECImpl.OfferSample (sample,offset,len);
		}

	#endif
	}
}
