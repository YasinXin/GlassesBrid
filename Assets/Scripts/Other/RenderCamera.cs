using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RenderCamera : MonoBehaviour {
    #region Variables
    public Shader curShader;
    public Texture SCTexture;

    public float blueColorLevel = 63;
    public float level = 1;
    private Material curMaterial;
    #endregion

    #region Properties
    public Material material
    {
        get
        {
            if (curMaterial == null)
            {
                curMaterial = new Material(curShader);
                curMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return curMaterial;
        }
    }
    #endregion

    // Use this for initialization
    void Start()
    {
        if (SystemInfo.supportsImageEffects == false)
        {
            enabled = false;
            return;
        }

        if (curShader != null && curShader.isSupported == false)
        {
            enabled = false;
        }
    }

    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if (curShader != null)
        {

            material.SetFloat("_blueColorLevel", blueColorLevel);
            material.SetFloat("_level", level);
            material.SetTexture("_inputImageTexture2", SCTexture);
            Graphics.Blit(sourceTexture, destTexture, material);
        }
        else
        {
            Graphics.Blit(sourceTexture, destTexture);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //grayScaleAmount = Mathf.Clamp(grayScaleAmount, 0.0f, 1.0f);
    }

    void OnDisable()
    {
        if (curMaterial != null)
        {
            DestroyImmediate(curMaterial);
        }
    }
}
