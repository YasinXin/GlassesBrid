Shader "lidx/SkinUnlitShader_LCB"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_beauty("beauty",Range(0, 1)) = 0.45//平滑差值-磨皮
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			
			#include "UnityCG.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _beauty;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}


		float sum = 0.0;
		float gaussianWeightTotal = 0.2;
		float sample_ = 0.0;
		float distanceFromCentralColor = 0.0;
		float gaussianWeight = 0.0;
		float distanceNormalizationFactor = 3.6;
		float central = 0.0;
		//美颜
		float4 Beauty(float2 uv)
		{
			float x_a = 640.0;
			float y_a = 1136.0;

			float mul_x = 1.6 / x_a;
			float mul_y = 1.6 / y_a;

			float2 blurCoordinates0 = uv + float2(0.0 * mul_x, -10.0 * mul_y);
			float2 blurCoordinates1 = uv + float2(5.0 * mul_x, -8.0 * mul_y);
			float2 blurCoordinates2 = uv + float2(8.0 * mul_x, -5.0 * mul_y);
			float2 blurCoordinates3 = uv + float2(10.0 * mul_x, 0.0 * mul_y);
			float2 blurCoordinates4 = uv + float2(8.0 * mul_x, 5.0 * mul_y);
			float2 blurCoordinates5 = uv + float2(5.0 * mul_x, 8.0 * mul_y);
			float2 blurCoordinates6 = uv + float2(0.0 * mul_x, 10.0 * mul_y);
			float2 blurCoordinates7 = uv + float2(-5.0 * mul_x, 8.0 * mul_y);
			float2 blurCoordinates8 = uv + float2(-8.0 * mul_x, 5.0 * mul_y);
			float2 blurCoordinates9 = uv + float2(-10.0 * mul_x, 0.0 * mul_y);
			float2 blurCoordinates10 = uv + float2(-8.0 * mul_x, -5.0 * mul_y);
			float2 blurCoordinates11 = uv + float2(-5.0 * mul_x, -8.0 * mul_y);

			central = tex2D(_MainTex, uv).g;
			distanceNormalizationFactor = 3.6;
			gaussianWeightTotal = 0.2;
			sum = central * 0.2;

			sample_ = tex2D(_MainTex, blurCoordinates0).g;
			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
			gaussianWeightTotal += gaussianWeight;
			sum += sample_ * gaussianWeight;

			sample_ = tex2D(_MainTex, blurCoordinates1).g;
			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
			gaussianWeightTotal += gaussianWeight;
			sum += sample_ * gaussianWeight;

			sample_ = tex2D(_MainTex, blurCoordinates2).g;
			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
			gaussianWeightTotal += gaussianWeight;
			sum += sample_ * gaussianWeight;

			sample_ = tex2D(_MainTex, blurCoordinates3).g;
			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
			gaussianWeightTotal += gaussianWeight;
			sum += sample_ * gaussianWeight;

			sample_ = tex2D(_MainTex, blurCoordinates4).g;
			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
			gaussianWeightTotal += gaussianWeight;
			sum += sample_ * gaussianWeight;

			sample_ = tex2D(_MainTex, blurCoordinates5).g;
			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
			gaussianWeightTotal += gaussianWeight;
			sum += sample_ * gaussianWeight;

			sample_ = tex2D(_MainTex, blurCoordinates6).g;
			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
			gaussianWeightTotal += gaussianWeight;
			sum += sample_ * gaussianWeight;

			sample_ = tex2D(_MainTex, blurCoordinates7).g;
			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
			gaussianWeightTotal += gaussianWeight;
			sum += sample_ * gaussianWeight;

			sample_ = tex2D(_MainTex, blurCoordinates8).g;
			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
			gaussianWeightTotal += gaussianWeight;
			sum += sample_ * gaussianWeight;

			sample_ = tex2D(_MainTex, blurCoordinates9).g;
			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
			gaussianWeightTotal += gaussianWeight;
			sum += sample_ * gaussianWeight;

			sample_ = tex2D(_MainTex, blurCoordinates10).g;
			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
			gaussianWeightTotal += gaussianWeight;
			sum += sample_ * gaussianWeight;

			sample_ = tex2D(_MainTex, blurCoordinates11).g;
			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
			gaussianWeightTotal += gaussianWeight;
			sum += sample_ * gaussianWeight;

			sum = sum / gaussianWeightTotal;

			float3 centralColor = tex2D(_MainTex, uv).rgb;

			sample_ = centralColor.g - sum + 0.5;

			for (int i = 0; i < 5; i++)
			{
				if (sample_ <= 0.5)
				{
					sample_ = sample_ * sample_ * 2.0;
				}
				else
				{
					sample_ = 1.0 - ((1.0 - sample_)*(1.0 - sample_) * 2.0);
				}
			}

			float aa = 1.0 + pow(sum, 0.3)*0.07;
			float3 smoothColor = centralColor*aa - float3(sample_, sample_, sample_)*(aa - 1.0);// get smooth color
			smoothColor = clamp(smoothColor, float3(0.0, 0.0, 0.0), float3(1.0, 1.0, 1.0));//make smooth color right
			smoothColor = lerp(centralColor, smoothColor, pow(centralColor.g, 0.33));
			smoothColor = lerp(centralColor, smoothColor, pow(centralColor.g, 0.39));
			smoothColor = lerp(centralColor, smoothColor, _beauty);//0.99
			return float4(pow(smoothColor, float3(0.96,0.96,0.96)), 1.0);//0.8, 0.92, 0.9
		}


			float4 frag (v2f i) : color
			{
				// sample the texture
				float4 col = tex2D(_MainTex, i.uv);
			    if(_beauty==0)
			    {
			    	// apply fog
					return col;
			    }
			    col=Beauty(i.uv.xy);

				// apply fog
				return col;
			}
			ENDCG
		}
	}
}
