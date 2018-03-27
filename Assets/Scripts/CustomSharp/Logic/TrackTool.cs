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
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;
using System;
public struct TRACK3DRET
{
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 68)]
	public TrackPointCoord[] allTrackPoints;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
	public TrackPointCoord[] mountTrackPoints;
	
    public int faceShapeIndex;  //  index  0:aquare  1:round  2:slim
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public float[] matrix;

    public TRACK3DRET(TrackPointCoord[] _allTrackPoints, TrackPointCoord[] _mountTrackPoints, float[] _Matrix, int _faceShapeIndex)
    {
        this.allTrackPoints = _allTrackPoints;
        this.mountTrackPoints = _mountTrackPoints;
        this.matrix = _Matrix;
        this.faceShapeIndex = _faceShapeIndex;

    }
}
public struct TrackPointCoord
{
    public float x;
    public float y;
    public TrackPointCoord(float _x, float _y)
    {
        this.x = _x;
        this.y = _y;
    }
};

public struct TRACK2DRET
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 68)]
    public TrackPointCoord[] allTrackPoints;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
    public TrackPointCoord[] mountTrackPoints;
    public float scale;
    public float angle;
    //public float rendering_paramsy;
    //public float rendering_paramsx;

    public TRACK2DRET(TrackPointCoord[] _allTrackPoints, TrackPointCoord[] _mountTrackPoints, float _scale, float _angle)
    {
        this.allTrackPoints = _allTrackPoints;
        this.mountTrackPoints = _mountTrackPoints;
        this.scale = _scale;
        this.angle = _angle;
        //this.rendering_paramsy = _rendering_paramsy;
        //this.rendering_paramsx = _rendering_paramsx;
    }
};

public class TrackTool : Manager
{
    int length = 0;
    byte[] bytes;
    GCHandle handle;
    //是否检测人脸返回值
    bool boltrack = false;
    /// <summary>
    /// 上一次检测人脸数
    /// </summary>
    static int oldfaceNum = 0;
    #region TrackTool interface
#if UNITY_ANDROID
    [DllImport("lidxvrlib")]
    private static extern bool InitFaceTrackForUnity3D(string path);

    [DllImport("lidxvrlib")]
    private static extern IntPtr Track2DForUnity3D(byte[] rgba, int width, int height, float rotation, bool isMirror, ref int faceNumber);
	
	[DllImport("lidxvrlib")]
	private static extern IntPtr Track3DForUnity3D(byte[] rgba, int width, int height, float rotation, bool isMirror, ref int faceNumber);
	
#endif
    #endregion

    /// <summary>
    /// 初始化2d结构体
    /// </summary>
    public TRACK2DRET InitStruct()
    {
        TrackPointCoord[] _allTrackPoints, _mountTrackPoints;
        TrackPointCoord _TrackPointCoord = new TrackPointCoord(0, 0);
        _allTrackPoints = new TrackPointCoord[68];
        _mountTrackPoints = new TrackPointCoord[11];
        for (int i = 0; i < _allTrackPoints.Length; i++)
        {
            _allTrackPoints[i] = _TrackPointCoord;
        }
        for (int i = 0; i < _mountTrackPoints.Length; i++)
        {
            _mountTrackPoints[i] = _TrackPointCoord;
        }
        TRACK2DRET track2dret = new TRACK2DRET(_allTrackPoints, _mountTrackPoints, 0, 0);
        return track2dret;
    }

    /// <summary>
    /// 初始化3d结构体
    /// </summary>
    public TRACK3DRET InitTrackVector()
    {
		TrackPointCoord[] _allTrackPoints, _mountTrackPoints;
		TrackPointCoord _TrackPointCoord = new TrackPointCoord(0, 0);
		_allTrackPoints = new TrackPointCoord[68];
		_mountTrackPoints = new TrackPointCoord[11];
        
		for (int i = 0; i < _allTrackPoints.Length; i++)
		{
			_allTrackPoints[i] = _TrackPointCoord;
		}
		for (int i = 0; i < _mountTrackPoints.Length; i++)
		{
			_mountTrackPoints[i] = _TrackPointCoord;
		}

		float[] _Matrix;
		_Matrix = new float[16];
		for (int i = 0; i < _Matrix.Length; i++)
		{
			_Matrix[i] = 0.0f;
		}

        int _faceShapeIndex;
        _faceShapeIndex = 0;


        TRACK3DRET tv = new TRACK3DRET(_allTrackPoints, _mountTrackPoints, _Matrix, _faceShapeIndex);
		return tv;
    }

#if UNITY_ANDROID

    /// <summary>
    /// 人脸检测初始化
    /// </summary>
    /// <returns><c>true</c>, if init was trackered, <c>false</c> otherwise.</returns>
    public bool TrackerInit(bool checkResolution)
    {
        return TrackerInitWith();
    }

    /// <summary>
    /// Android 人脸检测初始化
    /// </summary>
    /// <returns></returns>
    bool TrackerInitWith()
    {
        bool bols = Util.DirectoryExistence(AppConst.AndroidPath + "/Model");
        Util.Log("Model AndroidPath===>" + bols + "  Path=" + AppConst.AndroidPath + "/Model");
        bool bols1 = Util.FileIsExistence(AppConst.AndroidPath + "/Model/faceDetection.bin");
        Util.Log("Model AndroidPath===>" + bols1 + "  Path=" + AppConst.AndroidPath + "/Model/faceDetection.bin");
        bool bols2 = Util.FileIsExistence(AppConst.AndroidPath + "/Model/sp_svm_tree400_cascade5_depth4_comression.dat");
        Util.Log("Model AndroidPath===>" + bols2 + "  Path=" + AppConst.AndroidPath + "/Model/sp_svm_tree400_cascade5_depth4_comression.dat");
        //Util.datapath
        return InitFaceTrackForUnity3D(AppConst.AndroidPath + "/Model");
       
    }

    public bool GetTrack2d(byte[] Pixels, int width, int height, bool isMirror, float deviceAngle, ref int faceNum, ref TRACK2DRET[] track2dretResult)
    {
        boltrack = false;
        IntPtr ptrTrack =  Track2DForUnity3D(Pixels, width, height, deviceAngle, isMirror, ref faceNum);

        if (ptrTrack != IntPtr.Zero && faceNum > 0)
        {
           
            if (faceNum != track2dretResult.Length)
            {
                track2dretResult = new TRACK2DRET[faceNum];
            }
            //还原成结构体数组  
            for (int i = 0; i < faceNum; i++)
            {
                IntPtr pPonitor = new IntPtr(ptrTrack.ToInt64() + Marshal.SizeOf(typeof(TRACK2DRET)) * i);
                track2dretResult[i] = (TRACK2DRET)Marshal.PtrToStructure(pPonitor, typeof(TRACK2DRET));
            }
            boltrack = true;          
        }

        return boltrack;
    }

	public bool GetTrack3d(byte[] Pixels, int width, int height, bool isMirror, float deviceAngle, ref int faceNum, ref TRACK2DRET[] track2dretResult, ref TRACK3DRET[] track3dretResult)
	{
        Util.Log("GetTrack3d==>");
        if(Pixels == null)
        {
            return false;            
        }
        boltrack = false;

        IntPtr ptr = Track3DForUnity3D(Pixels, width, height, deviceAngle, isMirror, ref faceNum);
        if (ptr != IntPtr.Zero)
        {
            if (faceNum > 0)
            {
                if (faceNum != track3dretResult.Length)
                {
                    track3dretResult = new TRACK3DRET[faceNum];
                }
                //还原成结构体数组  
                for (int i = 0; i < faceNum; i++)
                {
                    IntPtr pPonitor = new IntPtr(ptr.ToInt64() + Marshal.SizeOf(typeof(TRACK3DRET)) * i);
                    track3dretResult[i] = (TRACK3DRET)Marshal.PtrToStructure(pPonitor, typeof(TRACK3DRET));
                }
                boltrack = true;
            }
        }

        IntPtr ptrTrack = Track2DForUnity3D(Pixels, width, height, deviceAngle, isMirror, ref faceNum);
        if (ptrTrack != IntPtr.Zero && faceNum > 0)
        {
            if (faceNum != track2dretResult.Length)
            {
                track2dretResult = new TRACK2DRET[faceNum];
            }
            //还原成结构体数组  
            for (int i = 0; i < faceNum; i++)
            {
                IntPtr pPonitor = new IntPtr(ptrTrack.ToInt64() + Marshal.SizeOf(typeof(TRACK2DRET)) * i);
                track2dretResult[i] = (TRACK2DRET)Marshal.PtrToStructure(pPonitor, typeof(TRACK2DRET));
            }
            boltrack = true;
        }


        TestUI.myText3.text = faceNum.ToString();
        return boltrack;
	}

    private float[] trackFloat;
    public bool GetTrack(ref int faceNum, ref TRACK2DRET[] track2dretResult, ref TRACK3DRET[] track3dretResult)
    {
        //Util.Log("GetTrack++>");
        try
        {
            trackFloat = SendPlatformManager.currentActivity.Call<float[]>("getFaceTracking");
        }
        catch (Exception e)
        {
            Util.Log("GetTrack++>" + "track错误2！" + e.ToString());
            throw;
        }

        faceNum = 1;
        if (trackFloat == null || trackFloat.Length < 150)
        {
            return false;
        }
        else
        {
            boltrack = true;
        }
        track3dretResult = new TRACK3DRET[1];
        track3dretResult[0] = InitTrackVector();

        for (int i = 0; i < 68; i++)
        {
            track3dretResult[0].allTrackPoints[i] = new TrackPointCoord(trackFloat[2 * i], trackFloat[2 * i + 1]);
        }
        //Util.Log("track3dretResultwww==> " + track3dretResult[0].allTrackPoints[0].x + "    " + track3dretResult[0].allTrackPoints[0].y);
        //for (int i = 0; i < 68; i++)
        //{
        //    TestUI.allBall[i].transform.position = Camera.main.ScreenToWorldPoint(new Vector3(track3dretResult[0].allTrackPoints[i].x * Screen.width / WebCamera.m_Resolution.x, Screen.height - track3dretResult[0].allTrackPoints[i].y * Screen.height / WebCamera.m_Resolution.y, 10));
        //}
        track3dretResult[0].faceShapeIndex = 1;
        for (int i = 136; i < 152; i++)
        {
            track3dretResult[0].matrix[i - 136] = trackFloat[i];
        }
        return boltrack;
    }


#endif

    /// <summary>
    /// 初始化颜色数组转字节数组
    /// </summary>
    /// <param name="count">Count.</param>
    public void SetColorByByteInit(int count)
    {
        length = Marshal.SizeOf(typeof(Color32)) * count;
        bytes = new byte[length];
        handle = default(GCHandle);
    }
}