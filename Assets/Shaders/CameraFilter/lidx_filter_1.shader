Shader "lidx/lidx_filter_1"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_inputImageTexture2("Texture2", 2D) = "white" {}
		_inputImageTexture3("Texture3", 2D) = "white" {}
		_inputColorR("ColorR",float) = 0.16666
		_inputColorG("ColorG",float) = 0.5
		_inputColorB("ColorB",float) = 0.83333
	}
	SubShader
	{
		// No culling or depth
		//Cull Off ZWrite Off ZTest Always
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};
		
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			uniform sampler2D _MainTex;
			uniform sampler2D _inputImageTexture2;
			uniform sampler2D _inputImageTexture3;
			uniform float _inputColorR;
			uniform float _inputColorG;
			uniform float _inputColorB;
			float4 frag(v2f i) : COLOR
			{
				float2 uv = i.uv.xy;
				float3x3 saturateMatrix = { 1.1402,
				-0.0598,
				-0.061,
				-0.1174,
				1.0826,
				-0.1186,
				-0.0228,
				-0.0228,
				1.1772 };

			float3 lumaCoeffs = float3(0.3, 0.59, 0.11);
				float4 col = tex2D(_MainTex, uv);
				float3 texel = tex2D(_MainTex, uv).rgb;
			texel = float3(
				tex2D(_inputImageTexture2, float2(texel.r, _inputColorR)).r,
				tex2D(_inputImageTexture2, float2(texel.g, _inputColorG)).g,
				tex2D(_inputImageTexture2, float2(texel.b, _inputColorB)).b
				);
			texel = mul(saturateMatrix,texel);
			float luma = dot(lumaCoeffs, texel);
			texel = float3(
				tex2D(_inputImageTexture3, float2(luma, texel.r)).r,
				tex2D(_inputImageTexture3, float2(luma, texel.g)).g,
				tex2D(_inputImageTexture3, float2(luma, texel.b)).b);

			return float4(texel, 1.0);
			}
			ENDCG
		}
	}
}
