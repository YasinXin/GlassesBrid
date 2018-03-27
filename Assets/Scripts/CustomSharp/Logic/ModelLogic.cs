/// <summary>
/// 3D模型素材逻辑
/// </summary>
using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// 模型位置，旋转角，缩放
/// </summary>
public struct MatrixVectorStruct
{
    public Vector3 Point;
    public Vector3 Scale;
    public Vector3 Angles;
    public MatrixVectorStruct(Vector3 _Point, Vector3 _Scale, Vector3 _Angles)
    {
        this.Point = _Point;
        this.Scale = _Scale;
        this.Angles = _Angles;
    }
}

public class ModelLogic : Manager
{
    #region 3D素材内 Spine2D素材逻辑
    //记录做张嘴触发动画素材
    List<string> m_OpenMouthAnimlist = new List<string>();
    //记录做抬头触发动画素材
    //List<string> m_UpheadAnimlist = new List<string> ();
    //记录做摇头触发动画素材
    //List<string> m_Aroundheadlist = new List<string> ();
    //记录做点击屏幕触发动画素材
    //List<string> m_ClickScreenlist = new List<string> ();
    //记录做按住屏幕触发动画素材
    //List<string> m_HoldDownScreenlist = new List<string> ();
    #endregion

    #region 3D逻辑字段
    //3D模型矩阵变化结构体
    //MatrixVectorStruct m_MatrixVector;
    //是否张嘴
    //用张嘴和闭嘴的标志位控制张嘴时与闭嘴检测时 逻辑不会循环执行
    //张嘴标志
    bool m_bolOpenMouth = false;
    //闭嘴标志
    bool m_bolCloseMouth = true;

    //控制识别时一次检测还是持续检测的阀门，检测后立即设为true，闭嘴后再设置为false
    bool m_bolOnceTrack = false;
    bool m_bolOnceTrack_left = false;
    bool m_bolOnceTrack_right = false;

    //张嘴触发用于计算的值
    double lip_distance, ratio;
    //摇头点头出发计算值
    double headRadio;
    //张嘴触发上嘴唇与下嘴唇监测点的值
    double upper_x, upper_y, upper_z, lower_x, lower_y, lower_z;
    //张嘴触发最大值
    const double trackMaxValue = 0.5f;

    //一次提示
    bool m_OncePrompt = false;

    //粒子缩放脚本
    ScaleParticles scaleParticle;



    #endregion

    #region 测试字段

    //#if UNITY_EDITOR
    //张嘴值测试
    double ratioTest = 0.0f;
    //张嘴测试
    public static bool OpenMouthTest = false;
    //#endif


    /// <summary>
    /// 测试张嘴触发的方法，通过设置ratioTest
    /// </summary>
    void TestOpenMouthSetValue()
    {
#if UNITY_EDITOR
        if (OpenMouthTest)
        {
            ratioTest = 0.51f;

        }
        else
        {
            ratioTest = 0.2f;
        }
#endif
    }

    //在编辑器下张嘴测试方法
    void TestOpenMouth()
    {
        if (OpenMouthTest)
        {
            m_bolOpenMouth = true;
        }
        else
        {
            m_bolOpenMouth = false;
        }
    }

    #endregion

    /// <summary>
    /// 初始化3D结构体
    /// </summary>
    public void InitStruct()
    {
        Vector3 Point = Vector3.zero;
        Vector3 Scale = Vector3.one;
        Vector3 Angles = Vector3.zero;

        //m_MatrixVector = new MatrixVectorStruct(Point, Scale, Angles);

    }

    /// <summary>
    /// 模型逻辑方法，WebCamera检测中调用
    /// </summary>
    /// <param name="trackResult">If set to <c>true</c> track result.</param>
    /// <param name="track3dret">Track3dret.</param>
    public void LogicModel(bool trackResult, TRACK3DRET track3dret, int index)
    {
#if UNITY_EDITOR
        TestOpenMouthSetValue();
#endif

        ModelActiveLogic(trackResult, track3dret, index);

        ModelTrackTrigger(trackResult, track3dret);
    }

    /// <summary>
    /// 重置属性值
    /// </summary>
    public void ResetValue()
    {
        SetOnceTrackValue(false);
        m_bolOpenMouth = false;
        m_bolCloseMouth = true;
        MaterialsMgr.m_fxCompleteFlag = true;
    }

    void OnTrackingFaceLoopMethod(bool boltarck, ref TRACK3DRET track3dret, int index)
    {

        if (MaterialsMgr.material3DBeanList != null && MaterialsMgr.material3DBeanList.Count > 0)
        {
            var enumeartor = MaterialsMgr.material3DBeanList.GetEnumerator();
            Util.Log("SpinePointcount:" + MaterialsMgr.material3DBeanList.Count);
            while (enumeartor.MoveNext())
            {
                Material3DBean mater3DBean = enumeartor.Current.Value;
                if (mater3DBean == null)
                {
                    continue;
                }
                OnTrackingFaceLoopMethod(boltarck, ref track3dret, mater3DBean, enumeartor.Current.Key, index);
            }
        }
    }
    GameObject go;
    /// <summary>
    /// 循环检测方法，需要检测人脸实时计算的逻辑
    /// </summary>
    /// <param name="track3dret">Track3dret.</param>
    MatrixVectorStruct m_MatrixVector;
    void OnTrackingFaceLoopMethod(bool boltarck, ref TRACK3DRET track3dret, Material3DBean material3DBean, string key, int index)
    {

        Vector3 Point = Vector3.zero;
        Vector3 Scale = Vector3.one;
        Vector3 Angles = Vector3.zero;

        m_MatrixVector = new MatrixVectorStruct(Point, Scale, Angles);

        ////InitStruct();

        //#if !UNITY_EDITOR
        //SDK返回矩阵值，重新计算u3d矩阵值，需要实时计算
        MaterialsMgr.GetTransformMatrix4x4(track3dret.allTrackPoints, track3dret.matrix, ref m_MatrixVector, key);

        //面皮矩阵变换
        m_MatrixVector.Point.z = 183.0f;

        Util.Log("enumeartor.Current.Key => " + key);
        if (key.Equals("Left"))
        {
            MaterialsMgr.MatrixToValue(MaterialsMgr.faceList[0], m_MatrixVector);
        }
        else if (key.Equals("Right"))
        {
            MaterialsMgr.MatrixToValue(MaterialsMgr.faceList[1], m_MatrixVector);
            Debug.Log("MaterialsMgr.faceList[1]==>" + MaterialsMgr.faceList[1].activeSelf);
        }
        else
        {
            if (index == 1)
            {
                MaterialsMgr.OnSetActive(key, "face_", false);
                MaterialsMgr.MatrixToValue(key, "face", m_MatrixVector);
            }
            else
            {
                MaterialsMgr.OnSetActive(key, "face_", true);
                MaterialsMgr.MatrixToValue(key, "face_", m_MatrixVector);
            }
        }


        //#endif

        //模型跟随
        if (material3DBean != null)
        {
            //检测状态
            if (material3DBean != null && material3DBean.model != null && material3DBean.model.Count > 0)
            {
                string modelName3d = "";
                for (int i = 0; i < material3DBean.model.Count; i++)
                {
                    //如果跟随的并且显示的才实时显示并且跟随
                    if (material3DBean.model[i].follow)
                    {
                        modelName3d = material3DBean.model[i].name;
                        //Util.Log("DifferencetType:AssetType.Model3D========"+modelName3d+"MM"+ m_MatrixVector.Point.x);
                        //模型跟随，设置矩阵值
                        m_MatrixVector.Point.z = (float)material3DBean.model[i].zOrder;
                        if (modelName3d != "")
                        {
                            //设置模型矩阵值

                            if (key.Equals("Left") && m_MatrixVector.Point.x > 0f)
                            {
                                m_MatrixVector.Point += new Vector3(100, 100, 0);
                            }
                            else if (key.Equals("Right") && m_MatrixVector.Point.x < 0f)
                            {
                                m_MatrixVector.Point += new Vector3(100, 100, 0);
                            }

                            if (index == 1)
                            {
                                MaterialsMgr.OnSetActive(modelName3d + "_", false);
                                MaterialsMgr.MatrixToValue(key, modelName3d, m_MatrixVector);
                            }
                            else
                            {
                                MaterialsMgr.OnSetActive(modelName3d + "_", true);
                                MaterialsMgr.MatrixToValue(key, modelName3d + "_", m_MatrixVector);
                            }

                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 一次检测方法，在人脸检测后需要触发一次的逻辑
    /// </summary>
    void OnTrackingFaceOnceMethod(Material3DBean material3DBean, string key, int index)
    {
        if (material3DBean != null)
        {
            Util.Log("isDoubleScreen=>" + FilteMgr.isDoubleScreen);
            //显示面皮模型
            if (FilteMgr.isDoubleScreen)
            {
                Util.Log("isDoubleScreen  wwww=>" + ReceiveMgsManager.showLeft + "    " + ReceiveMgsManager.showLeft);

                MaterialsMgr.faceList[0].SetActive(ReceiveMgsManager.showLeft);
                MaterialsMgr.faceList[1].SetActive(ReceiveMgsManager.showRight);
            }
            else
            {
                for (int i = 0; i < MaterialsMgr.faceList.Count; i++)
                {
                    MaterialsMgr.faceList[i].SetActive(false);
                }

                MaterialsMgr.OnSetActive(key, "face", true);
            }

            //设置模型显示隐藏
            if (material3DBean.model != null && material3DBean.model.Count > 0)
            {
                string modelName3d = "";

                for (int i = 0; i < material3DBean.model.Count; i++)
                {
                    modelName3d = material3DBean.model[i].name;
                    bool bolVisible = material3DBean.model[i].visible;
                    MaterialsMgr.OnSetActive(key, modelName3d, bolVisible);
                }
            }

            //开启提示
            PromptOpenClose(true);
        }
    }

    void OnTrackingFaceOnceMethod(int index)
    {
        if (MaterialsMgr.material3DBeanList != null && MaterialsMgr.material3DBeanList.Count > 0)
        {
            var enumeartor = MaterialsMgr.material3DBeanList.GetEnumerator();
            while (enumeartor.MoveNext())
            {
                Material3DBean mater3DBean = enumeartor.Current.Value;
                //if (MaterialsMgr.material3DBeanList == null)
                //    continue;
                //Util.Log ("OnTrackingFaceOnceMethod====OnTrackingFaceOnceMethod=======================MaterialsMgr.material2DBeanList:" + MaterialsMgr.material2DBeanList + "====================mater2DBean:" + mater2DBean.name);
                OnTrackingFaceOnceMethod(mater3DBean, enumeartor.Current.Key, index);
            }
        }
    }


    /// <summary>
    /// 丢失人脸检测方法，在人脸丢失后调用
    /// </summary>
    /// <param name="bolarck">If set to <c>true</c> bolarck.</param>
    void OnLostFaceMethod(Material3DBean material3DBean, string key)
    {
        if (material3DBean != null)
        {
            //隐藏所有model
            if (FilteMgr.isDoubleScreen)
            {
                for (int i = 0; i < MaterialsMgr.faceList.Count; i++)
                {
                    MaterialsMgr.faceList[i].SetActive(false);
                }
            }
            else
            {
                MaterialsMgr.OnSetActive(key, "face", false);
            }

            if (material3DBean.model != null)
            {
                if (material3DBean.model.Count > 0)
                {
                    for (int i = 0; i < material3DBean.model.Count; i++)
                    {
                        string modelName3d = material3DBean.model[i].name;
                        MaterialsMgr.OnSetActive(key, modelName3d, false);
                    }
                }
            }
        }
    }

    public void OnLostFaceMethod()
    {
        if (MaterialsMgr.material3DBeanList != null && MaterialsMgr.material3DBeanList.Count > 0)
        {
            var enumeartor = MaterialsMgr.material3DBeanList.GetEnumerator();
            while (enumeartor.MoveNext())
            {
                Material3DBean mater3DBean = enumeartor.Current.Value;
                OnLostFaceMethod(mater3DBean, enumeartor.Current.Key);
            }
        }
    }


    /// <summary>
    /// 模型检测显示逻辑，主逻辑
    /// </summary>
    /// <param name="bolarck">If set to <c>true</c> bolarck.</param>
    /// <param name="track3dret">Track3dret.</param>
    void ModelActiveLogic(bool boltarck, TRACK3DRET track3dret, int index)
    {
        //检测到人脸
        if (boltarck)
        {
            //循环检测
            OnTrackingFaceLoopMethod(boltarck, ref track3dret, index);

            Util.Log("GetOnceTrackValue==>" + ReceiveMgsManager.Localtion + "    " + m_bolOnceTrack_left + "    " + m_bolOnceTrack_right + "    " + GetOnceTrackValue());
            if (!GetOnceTrackValue())
            {
                SetOnceTrackValue(true);
                OnTrackingFaceOnceMethod(index); //检测到人脸
                Util.Log("faceShapeIndex = " + track3dret.faceShapeIndex);
                PlatformMgr.GetFaceShape(track3dret.faceShapeIndex);  //返回脸型参数
            }
        }
        else
        {
            if (GetOnceTrackValue())
            {
                //未检测到人脸
                SetOnceTrackValue(false);
                //丢失人脸检测方法，在人脸丢失后调用
                OnLostFaceMethod();
                // 关闭提示处理逻辑
                //				ClosePrompt ();
                PromptOpenClose(false);
                //重置属性值
                ResetValue();
            }
        }
    }

    /// <summary>
    /// 根据挂载的屏幕获取m_bolOnceTrack的值
    /// </summary>
    bool GetOnceTrackValue()
    {
        if (ReceiveMgsManager.Localtion.Equals("Left"))
        {
            return m_bolOnceTrack_left;
        }
        else if (ReceiveMgsManager.Localtion.Equals("Right"))
        {
            return m_bolOnceTrack_right;
        }
        else if (ReceiveMgsManager.Localtion.Equals("SpineType"))
        {
            return m_bolOnceTrack;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// 根据挂载的屏设置m_bolOnceTrack的值
    /// </summary>
    /// <param name="value"></param>
    public void SetOnceTrackValue(bool value)
    {
        if (value)
        {
            if (ReceiveMgsManager.Localtion.Equals("Left"))
            {
                m_bolOnceTrack_left = value;
            }
            else if (ReceiveMgsManager.Localtion.Equals("Right"))
            {
                m_bolOnceTrack_right = value;

            }
            else if (ReceiveMgsManager.Localtion.Equals("SpineType"))
            {
                m_bolOnceTrack = value;
            }
        }
        else
        {
            if (FilteMgr.isDoubleScreen)
            {
                m_bolOnceTrack_right = value;
                m_bolOnceTrack_left = value;
            }
            else
            {
                m_bolOnceTrack = value;
            }
        }

    }

    /// <summary>
    /// 模型检测触发方法
    /// </summary>
    /// <param name="trackResult">If set to <c>true</c> track result.</param>
    void ModelTrackTrigger(bool trackResult, TRACK3DRET _track3dret)
    {
        if (!trackResult)
            return;

        //张嘴检测判断
        OpenMouthCheck(_track3dret);

        //摇头点头判断
        ShakeHead(_track3dret);
    }

    /// <summary>
    /// 张嘴检测判断
    /// </summary>
    void OpenMouthCheck(TRACK3DRET _track3dret)
    {
        #region 3D 张嘴检测算法
#if !UNITY_EDITOR
//		upper_x = _track3dret.allVectorPoints[423].x;
//		upper_y = _track3dret.allVectorPoints[423].y;
//		upper_z = _track3dret.allVectorPoints[423].z;
//
//		lower_x = _track3dret.allVectorPoints[442].x;
//		lower_y = _track3dret.allVectorPoints[442].y;
//		lower_z = _track3dret.allVectorPoints[442].z;
//
//		lip_distance = Mathf.Sqrt( Mathf.Pow(((float)(lower_x-upper_x)), 2) +  
//		Mathf.Pow(((float)(lower_y-upper_y)), 2) + 
//		Mathf.Pow(((float)(lower_z-upper_z)), 2) );
//		ratio = lip_distance / 29.06f;

		double dist_lr, dist_ud, dist;
		// l r u d
		dist_lr = Mathf.Sqrt ((_track3dret.allTrackPoints [48].x - _track3dret.allTrackPoints [54].x)
			* (_track3dret.allTrackPoints [48].x - _track3dret.allTrackPoints [54].x)
			+ (_track3dret.allTrackPoints [48].y - _track3dret.allTrackPoints [54].y)
			* (_track3dret.allTrackPoints [48].y - _track3dret.allTrackPoints [54].y));

		dist_ud = Mathf.Sqrt ((_track3dret.allTrackPoints [51].x - _track3dret.allTrackPoints [57].x)
			* (_track3dret.allTrackPoints [51].x - _track3dret.allTrackPoints [57].x)
			+ (_track3dret.allTrackPoints [51].y - _track3dret.allTrackPoints [57].y)
			* (_track3dret.allTrackPoints [51].y - _track3dret.allTrackPoints [57].y));

		ratio = dist_ud / dist_lr;
#endif
        #endregion

        //WebCamManager.u_mouthopenparam = (float)ratio;

#if UNITY_EDITOR //编辑器下张嘴测试控制
        ratio = ratioTest;
#endif

        if (ratio >= trackMaxValue)
        {
            if (!m_bolOpenMouth)
                OpenMouth();
        }
        else
        {
            if (!m_bolCloseMouth)
                CloseMouth();
        }
    }

    /// <summary>
    /// 摇头点头逻辑
    /// </summary>
    bool nod_Option = false;
    bool shake_Option = false;
    void ShakeHead(TRACK3DRET _track3dret)
    {
        Vector3 faceAngle = m_MatrixVector.Angles;

        if (m_MatrixVector.Angles.x < -180)
        {
            faceAngle.x = m_MatrixVector.Angles.x + 360;
        }

        if (m_MatrixVector.Angles.y < -180)
        {
            faceAngle.y = m_MatrixVector.Angles.y + 360;
        }


        if (faceAngle.x < -20)
        {
            nod_Option = true;
        }
        else
        {
            if (nod_Option)
            {
                //点头
                TestUI.myText2.text = "点头";
                nod_Option = false;
            }
        }

        if (faceAngle.y < -40 || faceAngle.y > 40)
        {
            shake_Option = true;
        }
        else
        {
            if (shake_Option)
            {
                //摇头
                TestUI.myText2.text = "摇头";
                shake_Option = false;
            }
        }
        //TestUI.myText2.text = faceAngle.ToString();
    }

    /// <summary>
    /// 张嘴逻辑
    /// </summary>
    void OpenMouth()
    {
        m_bolOpenMouth = true;
        m_bolCloseMouth = false;
        m_OpenMouthAnimlist.Clear();
        //		ClosePrompt ();
        PromptOpenClose(false);
        TestUI.myText2.text = "张嘴";
    }

    /// <summary>
    /// 闭嘴逻辑
    /// </summary>
    void CloseMouth()
    {
        m_OpenMouthAnimlist.Clear();
        m_bolOpenMouth = false;
        m_bolCloseMouth = true;

        TestUI.myText2.text = "闭嘴";
    }


    /// <summary>
    /// 开启张嘴提示处理逻辑
    /// </summary>
    public void OpenPrompt()
    {
        if (MaterialsMgr.m_material3DBean != null)
        {
            //提示语
            if (MaterialsMgr.m_material3DBean.prompt != "" && MaterialsMgr.m_material3DBean.prompt != null)
            {
                PlatformMgr.OnShowHideIcon(MaterialsMgr.m_material3DBean.prompt, true);
                WebCamMgr.ActiveObject();
            }
        }
    }

    /// <summary>
    /// 关闭张嘴提示处理逻辑
    /// </summary>
    public void ClosePrompt()
    {
        if (MaterialsMgr.m_material3DBean != null)
        {
            //提示语
            if (MaterialsMgr.m_material3DBean.prompt != "" && MaterialsMgr.m_material3DBean.prompt != null)
                PlatformMgr.OnShowHideIcon(MaterialsMgr.m_material3DBean.prompt, false);
        }
    }

    /// <summary>
    /// 提示开启与关闭处理逻辑
    /// </summary>
    /// <param name="bol">If set to <c>true</c> bol.</param>
    public void PromptOpenClose(bool bol)
    {
        if (MaterialsMgr.m_material3DBean != null)
        {
            if (MaterialsMgr.m_material3DBean.prompt != "" && MaterialsMgr.m_material3DBean.prompt != null)
            {
                PlatformMgr.OnShowHideIcon(MaterialsMgr.m_material3DBean.prompt, bol);
                if (bol)
                {
                    WebCamMgr.ActiveObject();
                }
            }
        }
    }
}

