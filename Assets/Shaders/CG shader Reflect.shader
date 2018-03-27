// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "CG shader Reflect"{
	Properties{
		environmentMap("Environment Map", Cube) = "" {}
	reflectivity("reflectivity1", float) = 1 //反射系数  影响反射强度
		decalMap("decalMap", 2D) = "white" {}
	}
		SubShader{
		Pass{
		CGPROGRAM
#pragma vertex vert //顶点编程入口
#pragma fragment frag //片段编程入口
#include "UnityCG.cginc" //注意引入
	struct VertInput {
		float4 position:POSITION;
		float2 texCoordw:TEXCOORD0;
		float3 normal1:NORMAL;
	};
	struct VertOutput {
		float4 oPosition:SV_POSITION;
		float2 oTexCoord:TEXCOORD0;
		float3 R:TEXCOORD1;

	};
	// uniform 类型的参数 需要在Properties 
	uniform samplerCUBE environmentMap;
	uniform float reflectivity;
	uniform sampler2D decalMap;
	
	/*float3 reflect(float3 I, float3 N)
	{
		return I - 2.0*N*dot(N, I);
	}*/

	VertOutput vert(VertInput input)
	{
		VertOutput o;
		o.oPosition = mul(UNITY_MATRIX_MVP,input.position);//UNITY_MATRIX_MVP变量, 就是对应图形中的模型视图投影矩阵(ModelViewProj),unity中规定 必须这么写
		o.oTexCoord = input.texCoordw;
		float3 positionW = mul(unity_ObjectToWorld,input.position).xyz;//_Object2World 模型矩阵，把本地坐标转到世界坐标
		float3 N = mul((float3x3)unity_ObjectToWorld,input.normal1);
		N = normalize(N);
		float3 I = positionW - _WorldSpaceCameraPos;//计算入射光线，需要在世界坐标系中计算。_WorldSpaceCameraPos视点(相机)在世界坐标的位置
		o.R = reflect(I,N);//计算反射光线 reflect系统自带函数
		return o;
	}

	float4 frag(VertOutput output) :COLOR
	{
		float4 reflectionColor = texCUBE(environmentMap,output.R);
		float4 decalColor = tex2D(decalMap,output.oTexCoord);

		float4 color1 = lerp(decalColor,reflectionColor,reflectivity);
		return color1;
	}
		ENDCG
	}
	}
}