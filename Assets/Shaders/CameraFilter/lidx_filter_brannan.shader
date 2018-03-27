Shader "lidx/lidx_filter_brannan"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_inputImageTexture2("Texture2", 2D) = "white" {}
		_disableEffect("disableEffect",int) = 0
	}
	SubShader
	{
		// No culling or depth
		//Cull Off ZWrite Off ZTest Always
		ZTest Always
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
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
			uniform int _disableEffect;

			float4 frag (v2f i) : COLOR
			{
				float4 col;
				
			if (_disableEffect == 2) {
				col = float4(0, 0, 0, 1.0);
				return col;
			}
			float2 uv = i.uv.xy;
			if (_disableEffect == 1) {
				col = tex2D(_MainTex,uv);
				return col;
			}

			float3 texel = tex2D(_MainTex, uv).rgb;

			float2 lookup;
			lookup.y = 0.1;
			lookup.x = texel.r;
			texel.r = tex2D(_inputImageTexture2, lookup).r;
			lookup.x = texel.g;
			texel.g = tex2D(_inputImageTexture2, lookup).g;
			lookup.x = texel.b;
			texel.b = tex2D(_inputImageTexture2, lookup).b;

			float3x3 saturateMatrix = {
				1.105150,
				-0.044850,
				-0.046000,
				-0.088050,
				1.061950,
				-0.089200,
				-0.017100,
				-0.017100,
				1.132900 };
			texel = mul(saturateMatrix, texel);

			float2 tc = (2.0 * uv) - 1.0;
			float d = dot(tc, tc);
			float3 sampled;
			lookup.y = 0.3;
			lookup.x = texel.r;
			sampled.r = tex2D(_inputImageTexture2, lookup).r;
			lookup.x = texel.g;
			sampled.g = tex2D(_inputImageTexture2, lookup).g;
			lookup.x = texel.b;
			sampled.b = tex2D(_inputImageTexture2, lookup).b;
			float value = smoothstep(0.0, 1.0, d);
			texel = lerp(sampled, texel, value);

			lookup.y = 0.5;
			lookup.x = texel.r;
			texel.r = tex2D(_inputImageTexture2, lookup).r;
			lookup.x = texel.g;
			texel.g = tex2D(_inputImageTexture2, lookup).g;
			lookup.x = texel.b;
			texel.b = tex2D(_inputImageTexture2, lookup).b;

			float3 luma = float3(0.8, 0.59, 0.11);
			lookup.y = 0.7;
			lookup.x = dot(texel, luma);
			texel = lerp(tex2D(_inputImageTexture2, lookup).rgb, texel, .5);

			lookup.y = 0.9;
			lookup.x = texel.r;
			texel.r = tex2D(_inputImageTexture2, lookup).r;
			lookup.x = texel.g;
			texel.g = tex2D(_inputImageTexture2, lookup).g;
			lookup.x = texel.b;
			texel.b = tex2D(_inputImageTexture2, lookup).b;
			col = float4(texel, 1.0);
			float3 newf = float3(0.0, 0.35, 0.0);
			texel= float3(((col.rgb - newf) *0.75 + newf));
			return float4(texel, 1);
			}
			ENDCG
		}
	}
}