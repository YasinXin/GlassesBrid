/*
 By XiaoY
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TouchOption
{
    public bool singleClick;
    public bool singleSwipe;
    public bool twoFingerSwipe;
    public bool twoFingerPinch;

    public TouchOption(bool singleClick, bool singleSwipe, bool twoFingerSwipe, bool twoFingerPinch)
    {
        this.singleClick = singleClick;
        this.singleSwipe = singleSwipe;
        this.twoFingerSwipe = twoFingerSwipe;
        this.twoFingerPinch = twoFingerPinch;
    }
}

[Serializable]
public struct TouchParameter
{
    public float swipeRatio;
    public float rotateSensitive;
    public bool lockYRotate;
    public float pinchRatio;
    public float zoomSensitive;
    public float minScaleRate;
    public float maxScaleRate;
}

public class TouchEvent : MonoBehaviour {

    public delegate void MouseCilckEvent();
    public static event MouseCilckEvent MouseLeftCilck;
    public static event MouseCilckEvent MouseRightCilck;

    public delegate void SingleFingerClickEvent();
    public static event SingleFingerClickEvent SingleClick;

    public delegate void SingleFingerLongPressEvent();
    public static event SingleFingerLongPressEvent SingleLongPress;

    public delegate void SingleFingerSwipeEvent();
    public static event SingleFingerSwipeEvent SingleSwipe;

    public delegate void TwoFingerSwipeEvent(Vector2 eulerAngle);
    public static event TwoFingerSwipeEvent TwoFingerSwipe;

    public delegate void TwoFingerPinchEvent(float scaleRate);
    public static event TwoFingerPinchEvent TwoFingerPinch;

    public TouchOption touchOption = new TouchOption(true, true, true, true);
    public TouchParameter touchParameter;

    private Vector2 lastTouchPos_1;
    private Vector2 lastTouchPos_2;
    private Vector2 touchStartPos;
    private Vector2 currentTouchPos_1;
    private Vector2 currentTouchPos_2;
    private Vector3 eulerAngle;
    private Vector3 targetEulerAngle;
    private bool ismove = false;

    private float lastTouchDistance;
    private float scaleRate = 0.1f;
    private float targetScaleRate;
    private float startTime;


    private void Start()
    {
        targetScaleRate = scaleRate;
    }

    internal static void RaiseMouseLeftCilck()
    {
        if (MouseRightCilck != null)
        {
            MouseLeftCilck();
        }
    }

    internal static void RaiseMouseRightCilck()
    {
        if (MouseRightCilck != null)
        {
            MouseRightCilck();
        }
    }

    internal static void RaiseSingleClick()
    {
        if(SingleClick != null)
        {
            SingleClick();
        }
    }

    internal static void RaiseSingleLongPress()
    {
        if(SingleLongPress != null)
        {
            SingleLongPress();
        }
    }

    internal static void RaiseSingleSwipe()
    {
        if(SingleSwipe != null)
        {
            SingleSwipe();
        }
    }

    internal static void RaiseTwoFingerSwip(Vector2 eulerAngle)
    {
        if(TwoFingerSwipe != null)
        {
            TwoFingerSwipe(eulerAngle);
        }
    }

    internal static void RaiseTwoFingerPinch(float scaleRate)
    {
        if(TwoFingerPinch != null)
        {
            TwoFingerPinch(scaleRate);
        }
    }

    private void Update()
    {
        TouchControl();
        MouseControl();
    }

    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaiseMouseLeftCilck();   
        }
        else if (Input.GetMouseButtonDown(1))
        {
            RaiseMouseRightCilck();
        }
    }

    void TouchControl(){
        //单指操作--移动、点击
        if (Input.touchCount == 1)
        {
            currentTouchPos_1 = Input.GetTouch(0).position;

            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                startTime = Time.time;
                ismove = true;
                touchStartPos = lastTouchPos_1 = currentTouchPos_1;
            }

            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                if (Vector2.Distance(touchStartPos, currentTouchPos_1) > 1f && ismove)
                {
                    RaiseSingleSwipe();
                }

                lastTouchPos_1 = Input.GetTouch(0).position;
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (Vector2.Distance(touchStartPos, currentTouchPos_1) < 1f)
                {
                    if(Time.time - startTime < 0.5f)
                    {
                        RaiseSingleClick();
                    }
                    else
                    {
                        RaiseSingleLongPress();
                    }                  
                }
            }
        }

        //双指操作--缩放、旋转
        if (Input.touchCount == 2)
        {
            currentTouchPos_1 = Input.GetTouch(0).position;
            currentTouchPos_2 = Input.GetTouch(1).position;

            if (Input.GetTouch(1).phase == TouchPhase.Began)
            {
                ismove = false;
                lastTouchPos_1 = currentTouchPos_1;
                lastTouchPos_2 = currentTouchPos_2;
                lastTouchDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
            }
            if (Input.GetTouch(1).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                targetScaleRate = scaleRate -
                    (Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position) - lastTouchDistance) * touchParameter.pinchRatio;
                targetScaleRate = Mathf.Clamp(targetScaleRate, touchParameter.minScaleRate, touchParameter.maxScaleRate);

                if (!touchParameter.lockYRotate)
                {
                    targetEulerAngle.x += ((currentTouchPos_1.y - lastTouchPos_1.y) + (currentTouchPos_2.y - lastTouchPos_2.y)) * touchParameter.swipeRatio;
                }
                targetEulerAngle.y += ((-currentTouchPos_1.x + lastTouchPos_1.x) + (-currentTouchPos_1.x + lastTouchPos_1.x)) * touchParameter.swipeRatio;

                lastTouchPos_1 = Input.GetTouch(0).position;
                lastTouchPos_2 = Input.GetTouch(0).position;
            }
        }
     
        //写在这里才有缓动效果，所以不放在if的双指判断条件中
        if (Mathf.Abs(targetScaleRate - scaleRate) > 0.002f)
        {
            scaleRate = Mathf.Lerp(scaleRate, targetScaleRate, Time.fixedDeltaTime * touchParameter.zoomSensitive);
            RaiseTwoFingerPinch(scaleRate);
        }
        else
        {
            eulerAngle = Vector3.Lerp(eulerAngle, targetEulerAngle, Time.fixedDeltaTime * touchParameter.rotateSensitive);
            RaiseTwoFingerSwip(eulerAngle);
        }
    }
}


