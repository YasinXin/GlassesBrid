Shader "lidx/lidx_filter_3"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_inputImageTexture2("Texture2", 2D) = "white" {}
		_inputImageTexture3("Texture3", 2D) = "white" {}
		_inputColorR("ColorR",float)= 0.16666
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
				float3 texel = tex2D(_MainTex, uv).rgb;
				texel = float3(
					tex2D(_inputImageTexture2, float2(texel.r, _inputColorR)).r,
					tex2D(_inputImageTexture2, float2(texel.g, _inputColorG)).g,
					tex2D(_inputImageTexture2, float2(texel.b, _inputColorB)).b);

				float2 tc = (2.0 * uv) - 1.0;
				float d = dot(tc, tc);
				float2 lookup = float2(d, texel.r);
				texel.r = tex2D(_inputImageTexture3, lookup).r;
				lookup.y = texel.g;
				texel.g = tex2D(_inputImageTexture3, lookup).g;
				lookup.y = texel.b;
				texel.b = tex2D(_inputImageTexture3, lookup).b;
				return float4(texel,1.0);
			}
			ENDCG
		}
	}
}