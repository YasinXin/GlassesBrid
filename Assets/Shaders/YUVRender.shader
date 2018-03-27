// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/YUVRender" {
	Properties {
		_MainTexY("Base (RGB)", 2D) = "white" { }
		_MainTexU("Base (RGB)", 2D) = "white" { }
		_MainTexV("Base (RGB)", 2D) = "white" { }
		_Nightness("Promit",  Range(0,1)) = 1 //调节亮度
	}

	SubShader {

	Pass
		{
		
		CGPROGRAM
		 #pragma vertex vert
         #pragma fragment frag
         #include "UnityCG.cginc"
		 //#pragma target 3.0

		
		uniform sampler2D _MainTexY;
		uniform sampler2D _MainTexU;
		uniform sampler2D _MainTexV;
		fixed _Nightness;
		struct Input {
			    float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
		};

		struct Output
		{
			    float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;	
		};

		 float3x3 yuv2rgb;

 			Output vert(Input v) {
                Output o = (Output)0;
                o.uv0 = v.texcoord0;
              
                o.pos = UnityObjectToClipPos(v.vertex );
 
                return o;
            }


            fixed4  frag(Output i) : COLOR {
				yuv2rgb = float3x3(
				1.0f, 0.0f, 1.2802f,
					1.0f, -0.214821f, -0.380589f,
					1.0f, 2.127982f, 0.0f
					);
            	float y = 1.16f * (tex2D(_MainTexY, i.uv0).a - 0.063f);
            	float u = tex2D(_MainTexU, i.uv0).a - 0.5f;//-0.5f
            	float v = tex2D(_MainTexV, i.uv0).a - 0.5f;
				/**/
				

            	fixed3 yuv = fixed3(y,u,v);
				
				//float3 yuv = float3(g, b, r);

				fixed3 col = mul(yuv2rgb,yuv ) * _Nightness * 2;
				//col = clamp(col, float3(0.0f, 0.0f, 0.0f), float3(1.0f, 1.0f, 1.0f));
			
				fixed4 finalRGBA = fixed4(col, 1.0f);
				
                return finalRGBA;
				//float r = tex2D(_UVTex, i.uv0).r - 0.5f;
				//float g = tex2D(_UVTex, i.uv0).g - 0.5f;
				//float r = 1.1643f * (tex2D(_MainTex, i.uv0).a - 0.0627f);

				//return fixed4(r, g, 1.0f, 1.0f);
            }
			ENDCG
	  }
	}
}
