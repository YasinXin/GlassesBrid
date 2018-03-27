Shader "lidx/lidx_filter_inkwell_1"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_disableEffect("disableEffect",int)=0
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
			uniform int _disableEffect;
			uniform float _Fade;
			float4 frag(v2f i) : COLOR
			{
			if (_disableEffect == 2) {
				return  float4(0, 0, 0, 1.0);
			}
			float2 uv = i.uv.xy;
			if (_disableEffect == 1) {
				return  tex2D(_MainTex, uv);
			}

			float3 texel = tex2D(_MainTex, uv).rgb;
			texel=dot(float3(0.5,0.6,0.1),texel);
			return float4(texel, 1.0);
			}
			ENDCG
		}
	}
}