/// <summary>
/// 模型资源初始化
/// 1、解析接收IOS传递的路径消息来解析Json,并填充2D素材数据模型对象;
/// 2、获取实例化的模型、Spine2D、特效名字，通过MaterialMgr来Load和实例化资源。获取音频名称，通过AudioMgr来初始化音频;
/// 3、然后在回调加载完成回调中按照配置表默认参数设置模型、Spine2D、特效属性;
/// </summary>
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ModelInit : Manager
{
    /// <summary>
    /// 隐藏旧素材
    /// </summary>
    public void DelectModelMateria(Material3DBean material3DBean, string key)
    {
        WebCamMgr.m_exist3D = false;
        Util.Log("DelectMaterial3DBean==>444  " + WebCamMgr.m_tarckInit_3d);
        //判断是否是首次操作
        if (WebCamMgr.m_tarckInit_3d)
        {
            //设置默认属性值
            WebCamMgr.ResetValue(false, "3D");
            //隐藏屏幕中当前的模型、停止特效、音效和背景音乐
            if (material3DBean != null)
            {
                //隐藏模型
                //模型隐藏后，动画状态机中的动画自动停止
                if (material3DBean.model != null)
                {
                    if (material3DBean.model.Count > 0)
                    {
                        for (int i = 0; i < material3DBean.model.Count; i++)
                        {
                            string modelName3d = material3DBean.model[i].name;
                            GameObject obj = MaterialsMgr.GetObjFromClassifyDic(key, modelName3d);
                            GameObject obj_ = MaterialsMgr.GetObjFromClassifyDic(key, modelName3d + "_");
                            GameObject SHOW_obj = MaterialsMgr.GetObjFromClassifyDic(key, "SHOW_" + modelName3d);
                            //MaterialsMgr.OnSetActive(key, modelName3d, false);
                            //MaterialsMgr.OnDestroyGameObject(MaterialsMgr.GetObjFromClassifyDic(key, modelName3d));

                            if (obj == null || obj_ == null || SHOW_obj == null)
                            {
                                break;
                            }

                            obj.SetActive(false);
                            MaterialsMgr.OnDestroyGameObject(obj);

                            obj_.SetActive(false);
                            MaterialsMgr.OnDestroyGameObject(obj_);

                            SHOW_obj.SetActive(false);
                            MaterialsMgr.OnDestroyGameObject(SHOW_obj);
                            Util.Log("隐藏==>" + "glass");
                        }
                    }
                }
                //隐藏face脸
                MaterialsMgr.OnSetActive(key, "face", false);
                MaterialsMgr.OnSetActive(key, "face_", false);
                Util.Log("隐藏==>" + "face");

                //是否显示提示图标
                if (material3DBean.prompt != null)
                {
                    if (material3DBean.prompt != "")
                    {
                        PlatformMgr.OnShowHideIcon(material3DBean.prompt, false);
                    }
                }
                #region  
                //停止/隐藏屏幕中的特效
                if (material3DBean.particlies != null)
                {
                    if (material3DBean.particlies.Count > 0)
                    {
                        int count = material3DBean.particlies.Count;
                        for (int i = 0; i < count; i++)
                        {
                            string fxName = material3DBean.particlies[i].name;
                            Util.Log("DelectSpineMateria Stop Fx = " + fxName);
                            FxMgr.StopFx(fxName);
                        }
                    }
                }

                //停止播放spine动画
                if (material3DBean.spines != null)
                {
                    if (material3DBean.spines.Count > 0)
                    {
                        for (int i = 0; i < material3DBean.spines.Count; i++)
                        {
                            string spineName3d = material3DBean.spines[i].name;
                            AnimSpineMgr.StopAnimator(spineName3d);
                        }
                        for (int i = 0; i < material3DBean.spines.Count; i++)
                        {
                            string spineName3d = material3DBean.spines[i].name;
                            MaterialsMgr.OnSetActive(spineName3d, false);
                        }
                    }
                }

                //停止播放音效
                if (material3DBean.audios != null)
                {
                    if (material3DBean.audios.Count > 0)
                    {
                        for (int i = 0; i < material3DBean.audios.Count; i++)
                        {
                            if (material3DBean.audios[i].name != "")
                            {
                                AudioMgr.AudioStop(material3DBean.audios[i].name.Split('.')[0].ToString());
                            }
                        }
                    }
                }

                //停止播放背景音乐
                if (material3DBean.backgroundMusic != null)
                {
                    if (material3DBean.backgroundMusic.Count > 0)
                    {
                        for (int i = 0; i < material3DBean.backgroundMusic.Count; i++)
                        {
                            if (material3DBean.backgroundMusic[i].name != "")
                            {
                                AudioMgr.AudioStop(material3DBean.backgroundMusic[i].name.Split('.')[0].ToString());
                            }
                        }
                    }
                }
                #endregion
            }
            //清空音源组件
            AudioMgr.ClearAudioSource();
            Util.ClearMemory();
        }
    }

    public void DelectModel(string path)
    {
        if(MaterialsMgr.material3DBeanList.ContainsKey(ReceiveMgsManager.Localtion))
        {
            DelectModelMateria(MaterialsMgr.material3DBeanList[ReceiveMgsManager.Localtion], ReceiveMgsManager.Localtion);
        }
    }

    /// <summary>
    /// 切换屏幕后，根据全屏、分屏删除Material3DBean
    /// </summary>
    /// <param name="isDouble"></param>
    public void DelectMaterial3DBean(bool isDouble)
    {
        Util.Log("DelectMaterial3DBean==>111  " + isDouble);
        //双屏状态，删除单屏数据
        if (isDouble)
        {
            if (MaterialsMgr.material3DBeanList.ContainsKey("SpineType"))
            {
                DelectModelMateria(MaterialsMgr.material3DBeanList["SpineType"], "SpineType");
                MaterialsMgr.material3DBeanList.Remove("SpineType");
            }
        }
       //单屏状态，删除双屏数据
        else
        {
            if (MaterialsMgr.material3DBeanList.ContainsKey("Left"))
            {
                DelectModelMateria(MaterialsMgr.material3DBeanList["Left"], "Left");
                MaterialsMgr.material3DBeanList.Remove("Left");
                WebCamMgr.m_tarckInit_3d = true; //此为一次性执行开关控制，再次打开是为了删除Right的数据
            }
            if (MaterialsMgr.material3DBeanList.ContainsKey("Right"))
            {
                DelectModelMateria(MaterialsMgr.material3DBeanList["Right"], "Right");
                MaterialsMgr.material3DBeanList.Remove("Right");
            }
        }
    }
    
    /// <summary>
    /// 获取Model素材
    /// </summary>
    /// <param name="info"></param>
    public void LoadModelMateria(string path, string info, Action<bool> action)
    {
        //DelectSpineMateria();
        //控制进行2d或者3d人脸检测
        WebCamMgr.m_tarck2dOr3d = true;
        WebCamMgr.m_exist3D = true;
        MaterialsMgr.m_material3DBean = null;
        //解析jsonh填充数据到结构体
        MaterialsMgr.m_material3DBean = Util.GetJsonData<Material3DBean>(info, true);
        if (MaterialsMgr.m_material3DBean != null)
        {
            //判断是否有面皮
            if (MaterialsMgr.m_material3DBean.faceTexture != null)
            {
                if (MaterialsMgr.m_material3DBean.faceTexture != "")
                {
                    //如果面皮为face_map则从Resourcers中读取透明贴图
                    if (MaterialsMgr.m_material3DBean.faceTexture == "face_map")
                        MaterialsMgr.SetModelFaceTexture("face_map", true);
                    else //否则从硬盘加载对应面皮贴图
                        MaterialsMgr.SetModelFaceTexture(path + "/" + MaterialsMgr.m_material3DBean.faceTexture, false);
                }
            }

            //判断该素材是否有需要实例化的物体，有拼接名字加载素材，没有删除之前素材列表；
            if (MaterialsMgr.m_material3DBean.NeedInstantiateAssets)
            {
                //Debug.Log("   有   要实例化的素材   有  要实例化的素材");
                //获取音频素材
                if ((MaterialsMgr.m_material3DBean.audios != null && MaterialsMgr.m_material3DBean.audios.Count > 0) ||
                    (MaterialsMgr.m_material3DBean.backgroundMusic != null && MaterialsMgr.m_material3DBean.backgroundMusic.Count > 0))
                {
                    string audioName = GetAllAudioNames(MaterialsMgr.m_material3DBean);
                    Util.Log(" create All audioName = " + audioName);
                    if (audioName != "")
                    {
                        //加载音频素材
                        Util.Log("==========path = " + path + "      audioName = " + audioName);
                        AudioMgr.LoadAudio(path, audioName, () => {
                            //设置音频素材属性
                            SetAudioRepeat(MaterialsMgr.m_material3DBean);
                        });
                    }
                }

                //获取要实例化对象的名字，通过名字实例化对象
                //获取该资源bundle名称
                //string mateName = MaterialsMgr.m_material3DBean.name;

                //需要实例化的素材名字
                ///	string needInstanceName = "";

                string modelName = "";
                //获取要实例化的模型名称
                if (MaterialsMgr.m_material3DBean.model != null && MaterialsMgr.m_material3DBean.model.Count > 0)
                {
                    modelName = GetAllModelNames(MaterialsMgr.m_material3DBean.model);
                    Debug.Log(" create All modelName = " + modelName);
                }

                //CreateNeedInstanceName (ref needInstanceName, modelName);

                string spineName = "";
                //获取要实例化的spine2d资源名称
                if (MaterialsMgr.m_material3DBean.spines != null && MaterialsMgr.m_material3DBean.spines.Count > 0)
                {
                    spineName = GetAllSpine2DNames(MaterialsMgr.m_material3DBean.spines);
                    Util.Log(" create All spineName = " + spineName);
                }
                //CreateNeedInstanceName (ref needInstanceName, spineName);

                string fxName = "";
                //获取需要实例化的特效名称
                if (MaterialsMgr.m_material3DBean.particlies != null && MaterialsMgr.m_material3DBean.particlies.Count > 0)
                {
                    fxName = GetAllFxNames(MaterialsMgr.m_material3DBean.particlies);
                    Util.Log(" create All Fx = " + fxName);
                }
                //计算需要实例化的物体名称，该素材有没有需要被实例化的物体根据这个值去判断
                //CreateNeedInstanceName (ref needInstanceName, fxName);
                //needInstanceName = needInstanceName.TrimStart (',');
                //Debug.Log (" +++++++++ needInstanceName = " + needInstanceName);

                //拼接需要实例化的素材名字
                string needInstanceName = modelName + "," + spineName + "," + fxName;
                Util.Log("needInstanceName => " + needInstanceName);
                //获取该资源bundle名称
                string mateName = MaterialsMgr.m_material3DBean.name;
                if (mateName != "" && needInstanceName != "")
                {
                    //Debug.Log("   有   要实例化的素材   有  要实例化的素材");
                    //加载Model文件
                    MaterialsMgr.OnInitSpineManager(path, mateName, needInstanceName.Split(','), (bol, isActive) => {
                        if (bol)
                        {
                            Material3DBean material3DBean = MaterialsMgr.m_material3DBean;
                            WebCamMgr.m_tarckInit_3d = true;
                            Util.Log("UNITY - 3D assets init success ! ");
                            InitMode3D(MaterialsMgr.m_material3DBean);
                        }
                        action(bol);
                    });
                }

                //添加material3DBean,不能够对引用的数据直接进行删除操作
                if (MaterialsMgr.material3DBeanList != null && !string.IsNullOrEmpty(ReceiveMgsManager.Localtion))
                {
                    if (MaterialsMgr.material3DBeanList.ContainsKey(ReceiveMgsManager.Localtion))
                    {
                        MaterialsMgr.material3DBeanList.Remove(ReceiveMgsManager.Localtion);
                    }

                    MaterialsMgr.material3DBeanList.Add(ReceiveMgsManager.Localtion, MaterialsMgr.m_material3DBean);  //没有当前素材就添加
                    
                    Util.Log("ReceiveMgsManager.Localtion => " + ReceiveMgsManager.Localtion);
                }
            }
            else
            {
                //没有模型的素材，直接卸载前一个素材
                //Debug.Log("没有   要实例化的素材 没有   要实例化的素材");
                //MaterialsMgr.UnloadQuePrefab();
                WebCamMgr.m_tarckInit_3d = true;
                action(true);
            }
        }
    }

    /// <summary>
    /// 获取所有要实例化的模型名称
    /// </summary>
    /// <returns>The model.</returns>
    /// <param name="models">Models.</param>
    string GetAllModelNames(List<Model3DBean> models)
    {
        StringBuilder inputName = new StringBuilder();
        if (models != null && models.Count > 0)
        {
            for (int i = 0; i < models.Count; i++)
            {
                if (i == 0)
                {
                    inputName.Append(models[i].name);
                }
                else
                {
                    inputName.Append("," + models[i].name);
                }
            }
        }
        return inputName.ToString();
    }

    /// <summary>
    /// 获取所有要实例化的Spine2D名称
    /// </summary>
    /// <returns>The spine2 d.</returns>
    /// <param name="spines">Spines.</param>
    string GetAllSpine2DNames(List<Spine2DBean> spines)
    {
        StringBuilder inputName = new StringBuilder();
        if (spines != null && spines.Count > 0)
        {
            for (int i = 0; i < spines.Count; i++)
            {
                if (i == 0)
                {
                    inputName.Append(spines[i].name);
                }
                else
                {
                    inputName.Append("," + spines[i].name);
                }
            }
        }
        return inputName.ToString();
    }

    /// <summary>
    /// 获取所有音效和背景音乐名称
    /// </summary>
    /// <returns>The audio.</returns>
    /// <param name="currentBean">Current bean.</param>
    string GetAllAudioNames(Material3DBean currentBean)
    {
        StringBuilder inputName = new StringBuilder();

        if (currentBean.backgroundMusic != null)
        {
            if (currentBean.backgroundMusic.Count > 0)
            {
                for (int i = 0; i < currentBean.backgroundMusic.Count; i++)
                {
                    if (!currentBean.backgroundMusic[i].name.Equals(""))
                    {
                        if (inputName.ToString() != "")
                        {
                            inputName.Append("," + currentBean.backgroundMusic[i].name);
                        }
                        else
                        {
                            inputName.Append(currentBean.backgroundMusic[i].name);
                        }
                    }
                }
            }
        }

        if (currentBean.audios != null)
        {
            if (currentBean.audios.Count > 0)
            {
                for (int i = 0; i < currentBean.audios.Count; i++)
                {
                    if (!currentBean.audios[i].name.Equals(""))
                    {
                        if (inputName.ToString() != "")
                        {
                            inputName.Append("," + currentBean.audios[i].name);
                        }
                        else
                        {
                            inputName.Append(currentBean.audios[i].name);
                        }
                    }
                }
            }
        }
        return inputName.ToString();
    }

    /// <summary>
    /// 获取所有特效名称
    /// </summary>
    /// <returns>The fx.</returns>
    /// <param name="_fxData">Fx data.</param>
    string GetAllFxNames(List<ParticleJsonData> _fxData)
    {
        StringBuilder inputName = new StringBuilder();
        if (_fxData != null && _fxData.Count > 0)
        {
            for (int i = 0; i < _fxData.Count; i++)
            {
                if (i == 0)
                {
                    inputName.Append(_fxData[i].name);
                }
                else
                {
                    inputName.Append("," + _fxData[i].name);
                }
            }
        }
        return inputName.ToString();
    }

    /// <summary>
    /// 获取要实例化对象名称，如果该类别名字为空，略过
    /// </summary>
    /// <param name="originName">要拼接的字符串</param>
    /// <param name="_targetName">目标字符串</param>
    void CreateNeedInstanceName(ref string originName, string _targetName)
    {
        if (_targetName == "")
        {
            Util.Log("CreateNeedInstanceName -> name is null !");
            return;
        }
        originName += "," + _targetName;
    }

    string currentAudioName;
    string audioName;
    /// <summary>
    /// 设置音频属性
    /// </summary>
    /// <param name="currentBean"></param>
    void SetAudioRepeat(Material3DBean currentBean)
    {
        if (currentBean.audios != null && currentBean.audios.Count > 0)
        {
            for (int i = 0; i < currentBean.audios.Count; i++)
            {
                if (currentBean.audios[i].name != "")
                {
                    currentAudioName = currentBean.audios[i].name.Split('.')[0].ToString();
                    audioName = currentAudioName.Split('@')[0].ToString();
                    AudioMgr.SetAudio(audioName, currentAudioName, currentBean.audios[i].repeat);
                }
            }
        }

        if (currentBean.backgroundMusic != null && currentBean.backgroundMusic.Count > 0)
        {
            for (int i = 0; i < currentBean.backgroundMusic.Count; i++)
            {
                if (currentBean.backgroundMusic[i].name != "")
                {
                    currentAudioName = currentBean.backgroundMusic[i].name.Split('.')[0].ToString();
                    AudioMgr.SetAudio(currentAudioName, currentAudioName, true);
                }
            }
        }
    }


    string modelName;
    /// <summary>
    /// 初始化3D模型
    /// </summary>
    /// <param name="material3DBen">Material3 D ben.</param>
    private void InitMode3D(Material3DBean material3DBen)
    {
        if (material3DBen == null)
        {
            Util.LogError(" InitModeParams -> material3DBen is null ! ");
            return;
        }

        if (material3DBen.model == null || material3DBen.model.Count < 0)
        {
            Util.Log(" InitModeParams -> model is null or mode count less zero ! ");
            return;
        }

        //创建模型动画状态机列表
        material3DBen.anim = new List<Animator>();

        for (int i = 0; i < material3DBen.model.Count; i++)
        {
            //获取名称
            modelName = material3DBen.model[i].name;
            if (modelName == "")
            {
                continue;
            }
            Util.Log("InitMode3D ==>modelName=" + modelName);
            //设置挂载点
            MaterialsMgr.SetModelParent(material3DBen.model[i].Node, material3DBen.model[i].ChildNode, material3DBen.model[i].name, false);

            //设置跟随

            //根据follow判断模型是否跟随面皮进行计算，如果跟随则在检测中设置Z值，如果不跟随则在初始化时候设置Z值；
            //Debug.Log ("  Z ORDER = " + (float)material3DBen.model [i].zOrder);
            if (!material3DBen.model[i].follow)
            {
                if (material3DBen.model[i].zOrder < 0 && material3DBen.model[i].zOrder > 189)
                    Util.LogError("set zOrder out Range ! ");
                else
                    MaterialsMgr.OnSetModelZOrder(modelName, (float)material3DBen.model[i].zOrder);
            }
        }
    }
}
