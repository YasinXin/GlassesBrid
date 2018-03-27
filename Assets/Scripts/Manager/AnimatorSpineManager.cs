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
using System.Collections.Generic;
using Spine.Unity;
using Spine;
public class AnimatorSpineManager : Manager {
    static Dictionary<string, SkeletonAnimation> animation2dSpineDic = new Dictionary<string, SkeletonAnimation>();

    /// <summary>
    /// 添加
    /// </summary>
    void Add(string key, SkeletonAnimation value)
    {
        if (animation2dSpineDic.ContainsKey(key) || value == null)
            return;
        animation2dSpineDic.Add(key, value);
    }

    /// <summary>
    /// 获取
    /// </summary>
    SkeletonAnimation Get(string key)
    {
        if (animation2dSpineDic.ContainsKey(key))
        {
            return animation2dSpineDic[key] as SkeletonAnimation;
        }
        else {
            return null;
        }
    }

	public void OnDestroySkeletonAnim(GameObject go, string key)
    {
        animation2dSpineDic.Clear();
//        SkeletonAnimation obj = Get(key);
//        if (obj != null)
//        {
////            animation2dSpineDic.Remove(name);
//			animation2dSpineDic.Clear();
////			go.GetComponent<SkeletonAnimation> ().SkeletonDataAsset.Clear ();
////			Destroy (go.GetComponent<SkeletonDataAsset>());
			Destroy (go.GetComponent<SkeletonAnimation>());
//        }
//        else
//        {
//            Util.LogWarning("AnimatorSpineManager OnDestroySkeletonAnim=> key Object is not exist!");
//        }

    }

    /// <summary>
    /// 事件回调
    /// </summary>
    /// <param name="trackEntry"></param>
    /// <param name="e"></param>
    void HandleEvent(TrackEntry trackEntry, Spine.Event e)
    {
        //Debug.Log(trackEntry.TrackGameObjectName+"  " + trackEntry.trackIndex + " " + trackEntry.animation.name+" "+ e.data.name);
        //PlatformMgr.OnTrackEntryMessage("Handle", trackEntry.trackIndex, trackEntry.TrackGameObjectName, trackEntry.animation.name,e.data.name);
    }

    /// <summary>
    /// 开始回调
    /// </summary>
    /// <param name="trackEntry"></param>
    void StartHandleEvent(TrackEntry trackEntry)
    {
        //Debug.Log("start11111===>: " + trackEntry.trackIndex + " " + trackEntry.animation.name+ "  "+ trackEntry.TrackGameObjectName);
        //PlatformMgr.OnTrackEntryMessage("StartHandle", trackEntry.trackIndex, trackEntry.TrackGameObjectName, trackEntry.animation.name,"");
    }

    /// <summary>
    /// 结束回调
    /// </summary>
    /// <param name="trackEntry"></param>
    void EndHandleEvent(TrackEntry trackEntry)
    {
		//Util.Log("end====>: " + trackEntry.trackIndex + " " + trackEntry.animation.name + "  " + trackEntry.TrackGameObjectName);
        WebCamMgr.SpienAnimEnd(trackEntry.TrackGameObjectName);
    }

    /// <summary>
    /// 注册回调
    /// </summary>
    /// <param name="go"></param>
    public void GetSkeletonAnimation(GameObject go)
    {
        SkeletonAnimation skeletonAnimation = go.GetComponent<SkeletonAnimation>();
        if (skeletonAnimation!=null)
        {
            skeletonAnimation.state.Start += StartHandleEvent;
            skeletonAnimation.state.Event += HandleEvent; ;
            skeletonAnimation.state.End += EndHandleEvent;
            Add(go.name.ToString(), skeletonAnimation);
        }
    }

    /// <summary>
    /// 动画属性设置
    /// </summary>
    /// <param name="key"></param>
    /// <param name="bol"></param>
   public void SetAnimator(string key, bool bol)
    {
        SkeletonAnimation skeletonAnimation = Get(key);
        if (skeletonAnimation != null)
        {
            skeletonAnimation.state.ClearTracks();
            skeletonAnimation.timeScale = 1.0f;
            skeletonAnimation.state.SetCurrentGameObject(key,0, "animation", bol); // Set jump to be played on track 0 immediately.
        }
    }

    /// <summary>
    ///停止spine动画 
    /// </summary>
    /// <param name="key"></param>
    public void StopAnimator(string key)
    {
        SkeletonAnimation skeletonAnimation = Get(key);
        if (skeletonAnimation != null)
        {
            skeletonAnimation.state.ClearTracks();
        }
    }

    /// <summary>
    /// 设置播放速度
    /// </summary>
    /// <param name="key"></param>
    /// <param name="time"></param>
    public void SetAnimatorTimeScale(string key,string time)
    {
        SkeletonAnimation skeletonAnimation = Get(key);
        if (skeletonAnimation != null)
        {
            skeletonAnimation.timeScale = float.Parse(time);
        }
    }
}
