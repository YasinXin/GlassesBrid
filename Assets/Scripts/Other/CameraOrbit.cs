using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CameraParameter
{
    public bool limitXAngle;
    public float minXAngle;
    public float maxXAngle;

    public bool limitYAngle;
    public float minYAngle;
    public float maxYAngle;

    public float orbitSensitive;
    public float mouseMoveRatio;

    public CameraParameter(bool limitXAngle = true,
        float minXAngle = 0,
        float maxXAngle = 80,
        bool limitYAngle = false,
        float minYAngle = 0,
        float maxYAngle = 0,
        float orbitSensitive = 10f,
        float mouseMoveRatio = 0.3f)
    {
        this.limitXAngle = limitXAngle;
        this.minXAngle = minXAngle;
        this.maxXAngle = maxXAngle;
        this.limitYAngle = limitYAngle;
        this.minYAngle = minYAngle;
        this.maxYAngle = maxYAngle;
        this.orbitSensitive = orbitSensitive;
        this.mouseMoveRatio = mouseMoveRatio;
    }
}


public class CameraOrbit : MonoBehaviour
{

    private Vector3 lastMousePos;
    private Vector3 targetEulerAngle;
    private Vector3 eulerAngle;

    public CameraParameter freeOrbitParameter;
    private CameraParameter currentCamerParameter;

    public Transform cameraRootTf;
    public Transform cameraTf;

    private float cameraDistance;
    private float targetCameraDistance;

    private float lastTouchDistance;
    public float minDistance = 5f;
    public float maxDistance = 30f;
    public float mouseScrollRatio = 1f;
    public float touchDistanceRatio = 0.01f;
    public float zoomSensitive = 1f;

    private Quaternion originalRotate;

    private Camera ca;
    private int[] widthRange = new int[2];
    private int[] heightRange = new int[2];
    private bool isInside = false;
    private int lockSingle = 0;

    private void Start()
    {
        originalRotate = cameraTf.rotation;
        cameraDistance = cameraTf.localPosition.z;
        targetCameraDistance = cameraDistance;
        currentCamerParameter = freeOrbitParameter;

        ca = cameraTf.GetComponent<Camera>();
        widthRange[0] = (int)(ca.rect.x * Screen.width);
        widthRange[1] = widthRange[0] + (int)(ca.rect.width * Screen.width);
        heightRange[0] = (int)(ca.rect.y * Screen.height);
        heightRange[1] = heightRange[0] + (int)(ca.rect.height * Screen.height);
    }

    private void Update()
    {
        Orbit();
        Zoom();
    }

    //摄像机旋转
    private void Orbit()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StopAllCoroutines();
            if (Input.mousePosition.x > widthRange[0] && Input.mousePosition.x < widthRange[1] && Input.mousePosition.y > heightRange[0] && Input.mousePosition.y < heightRange[1])
            {
                isInside = true;
            }
            else
            {
                isInside = false;
            }
            lastMousePos = Input.mousePosition;
        }

        if (isInside)
        {
            if (Input.GetMouseButton(0) && lockSingle < 1)
            {
                targetEulerAngle.x += (-Input.mousePosition.y + lastMousePos.y) * currentCamerParameter.mouseMoveRatio;
                targetEulerAngle.y -= (Input.mousePosition.x - lastMousePos.x) * currentCamerParameter.mouseMoveRatio;
                if (currentCamerParameter.limitXAngle)
                {
                    targetEulerAngle.x = Mathf.Clamp(targetEulerAngle.x, currentCamerParameter.minXAngle, currentCamerParameter.maxXAngle);
                }
                if (currentCamerParameter.limitYAngle)
                {
                    targetEulerAngle.y = Mathf.Clamp(targetEulerAngle.y, currentCamerParameter.minYAngle, currentCamerParameter.maxYAngle);
                }
                lastMousePos = Input.mousePosition;
            }
        }
        else
        {
            targetEulerAngle.y += 1f * currentCamerParameter.mouseMoveRatio;
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopAllCoroutines();
            StartCoroutine(WaitTime());
        }

        if (Input.touchCount < 2)
        {
            eulerAngle = Vector3.Lerp(eulerAngle, targetEulerAngle, Time.fixedDeltaTime * currentCamerParameter.orbitSensitive);
            cameraRootTf.rotation = originalRotate * Quaternion.Euler(eulerAngle);
        }
    }

    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(2);
        isInside = false;
    }

    //摄像机缩放
    private void Zoom()
    {
        if (Input.touchCount < 2)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                cameraDistance = -cameraTf.localPosition.z;
                targetCameraDistance = cameraDistance - Input.GetAxis("Mouse ScrollWheel") * cameraDistance * mouseScrollRatio;
                targetCameraDistance = Mathf.Clamp(targetCameraDistance, minDistance, maxDistance);

                isInside = true;
                StopAllCoroutines();
                StartCoroutine(WaitTime());
            }
        }
        else
        {
            lockSingle = 2;
            if (Input.GetTouch(1).phase == TouchPhase.Began)
            {
                StopAllCoroutines();
                if (Input.GetTouch(0).position.x > widthRange[0] && Input.GetTouch(0).position.x < widthRange[1] && Input.GetTouch(0).position.y > heightRange[0] && Input.GetTouch(0).position.y < heightRange[1])
                {
                    isInside = true;
                }
                else
                {
                    isInside = false;
                }
                lastTouchDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
            }

            if (isInside)
            {
                if (Input.GetTouch(1).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    cameraDistance = -cameraTf.localPosition.z;
                    targetCameraDistance = cameraDistance -
                        (Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position) - lastTouchDistance) * touchDistanceRatio;
                    targetCameraDistance = Mathf.Clamp(targetCameraDistance, minDistance, maxDistance);
                    lastMousePos = Input.mousePosition;
                }
            }
        }

        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                lockSingle--;
            }

            if (Input.GetTouch(1).phase == TouchPhase.Ended)
            {
                lockSingle--;
            }
        }

        if (lockSingle < 1)
        {
            StopAllCoroutines();
            StartCoroutine(WaitTime());
        }

        if (Mathf.Abs(targetCameraDistance - cameraDistance) > 0.1f)
        {
            cameraDistance = Mathf.Lerp(cameraDistance, targetCameraDistance, Time.fixedDeltaTime * zoomSensitive);
            cameraTf.localPosition = new Vector3(0, 0, -cameraDistance);
        }
    }
}
