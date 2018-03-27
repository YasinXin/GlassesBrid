using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.Reflection;  

public class UICameraFilterRender : Manager {
	public MethodInfo methodmat1;
	public MethodInfo methodmat2;
	object objmat1=null;
	object objmat2=null;

	public Material material1=null;
	public Material material2=null;
	string destTextureValue="";
	public string m_Material1Name="";
	public string m_Material2Name="";

    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		//申请RenderTexture
		RenderTexture rt = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height, 0, sourceTexture.format);
		RenderTexture rt1 = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height, 0, sourceTexture.format);
		RenderTexture rt2 = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height, 0, sourceTexture.format);
		//直接将原图拷贝到RT上    
		Graphics.Blit(sourceTexture, rt);
		destTextureValue="rt";
        
		material1 = getMaterial (m_Material1Name,"rt1");
		//场景滤镜
		if (material1 != null) {
			destTextureValue = "rt1";
			Graphics.Blit (rt, rt1, material1);
		}
        
       material2 = getMaterial (m_Material2Name,"rt2");
        //人脸形变滤镜
        if (material2 != null) {
            //调整美颜值
            if (WebCamMgr.isChangeValue)
            {
                material2.SetFloat("_beauty", WebCamMgr.beauty);
                WebCamMgr.isChangeValue = false;               
            }
			if (destTextureValue=="rt1") {
				Graphics.Blit (rt1, rt2, material2);
			} else {
				Graphics.Blit (rt, rt2, material2);
			}
			destTextureValue = "rt2";
            
        }

		switch (destTextureValue) {
		case "rt1":
			Graphics.Blit(rt1, destTexture); 
			break;
		case "rt2":
              
                Graphics.Blit(rt2, destTexture);
			break;
		default:
			Graphics.Blit(rt, destTexture); 
			break;
		}

		//释放申请的两块RenderBuffer内容 
		RenderTexture.ReleaseTemporary(rt);    
		RenderTexture.ReleaseTemporary(rt1);    
		RenderTexture.ReleaseTemporary(rt2);  
	}

	void OnDisable()
	{
		if (material1)
		{
			DestroyImmediate(material1);
		}
		if (material2)
		{
			DestroyImmediate(material2);
		}
	}

	Material getMaterial(string filterName,string lvevl)
	{	
		if (filterName == "") {
			return null;
		} else {
			if (lvevl.Equals("rt1")) {
                
                if (methodmat1 == null) {
                    Type t = Type.GetType (filterName);  
					objmat1 = t.Assembly.CreateInstance (filterName);  
					methodmat1 = t.GetMethod ("GetMaterialInfo");
				}
                return methodmat1.Invoke(objmat1, null) as Material;
            } else {
                
                if (methodmat2 == null) {
                    Type t = Type.GetType(filterName);  
					objmat2 = t.Assembly.CreateInstance(filterName);
					methodmat2 = t.GetMethod("GetMaterialInfo");
                }
                return methodmat2.Invoke(objmat2, null) as Material;
            }
		}
	}
}