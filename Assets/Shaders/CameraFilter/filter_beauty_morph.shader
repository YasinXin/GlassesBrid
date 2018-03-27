Shader "lidx/filter_beauty_morph"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	_beauty("beauty",Range(0, 1)) = 0.45//平滑差值-磨皮
	}
		SubShader
	{
		Pass
	{
		//ZTest Always
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
	uniform int u_tracknum;

	uniform int u_tracksuccess;
	uniform float u_slimFactor;
	uniform float u_bigEyeFactor;

	uniform float4 _faceleft_u;
	uniform float4 _faceright_u;


	uniform float4 _eyea;
	uniform float4 _eyeb;

	uniform float4 _chin;
	uniform float4 _chinleft_d;
	uniform float4 _chinright_d;
	uniform float4 _chinleft_u;
	uniform float4 _chinright_u;


	uniform float4 _nose;
	uniform float4 _noseleft;
	uniform float4 _noseright;


	uniform float2 _MainTex_TexelSize;

	float _beauty;

	struct appdata
	{
		float4 vertex   : POSITION;
		//float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float2 texcoord  : TEXCOORD0;
		float4 vertex   : SV_POSITION;
		//float4 color : COLOR;
		float4 projPos : TEXCOORD1;
	};

	float2 faceStretch(float2 textureCoord, float2 originPosition, float2 targetPosition, float radius, float curve)
	{
		float2 direction = targetPosition - originPosition;
		float lengthA = length(direction);
		if (lengthA<0.0001)
		{
			return direction;
		}
		float lengthB = min(lengthA, radius);
		direction *= lengthB / lengthA;
		float infect = distance(textureCoord, originPosition) / radius;
		infect = clamp(1.0 - infect,0.0,1.0);
		//infect = pow(infect, curve);
		return direction * infect;
	}

	float2 morghone(float2 newCoord,int id)
	{
		half2 p_faceleft_u;
		half2 p_faceright_u;


		float2 p_eyea;
		float2 p_eyeb;

		half2 p_chin;
		half2 p_chinleft_d;
		half2 p_chinright_d;
		half2 p_chinleft_u;
		half2 p_chinright_u;

		float2 p_nose;
		float2 p_noseleft;
		float2 p_noseright;

		if (id == 0)
		{
			p_faceleft_u = half2(_faceleft_u.x, _faceleft_u.y);
			p_faceright_u = half2(_faceright_u.x, _faceright_u.y);


			p_eyea = float2(_eyea.x, _eyea.y);
			p_eyeb = float2(_eyeb.x, _eyeb.y);

			p_chin = half2(_chin.x, _chin.y);
			p_chinleft_d = half2(_chinleft_d.x, _chinleft_d.y);
			p_chinright_d = half2(_chinright_d.x, _chinright_d.y);
			p_chinleft_u = half2(_chinleft_u.x, _chinleft_u.y);
			p_chinright_u = half2(_chinright_u.x, _chinright_u.y);

			p_nose = float2(_nose.x, _nose.y);
			p_noseleft = float2(_noseleft.x, _noseleft.y);
			p_noseright = float2(_noseright.x, _noseright.y);
		}


		if (id == 1)
		{

			p_faceleft_u = half2(_faceleft_u.z, _faceleft_u.w);
			p_faceright_u = half2(_faceright_u.z, _faceright_u.w);


			p_eyea = float2(_eyea.z, _eyea.w);
			p_eyeb = float2(_eyeb.z, _eyeb.w);

			p_chin = half2(_chin.z, _chin.w);
			p_chinleft_d = half2(_chinleft_d.z, _chinleft_d.w);
			p_chinright_d = half2(_chinright_d.z, _chinright_d.w);
			p_chinleft_u = half2(_chinleft_u.z, _chinleft_u.w);
			p_chinright_u = half2(_chinright_u.z, _chinright_u.w);

			p_nose = float2(_nose.z, _nose.w);
			p_noseleft = float2(_noseleft.z, _noseleft.w);
			p_noseright = float2(_noseright.z, _noseright.w);
		}
		float2 pngSize = float2(0.72f, 1.28f);
		float2 curCoord = newCoord*pngSize;

		float2 eyea = p_eyea*pngSize;
		float2 eyeb = p_eyeb*pngSize;
		float2 chinleft = p_chinleft_d*pngSize;
		float2 chinright = p_chinright_d*pngSize;
		float2 nose = p_nose*pngSize;
		half2 chin = p_chin*pngSize;
		half2 faceleft = p_faceleft_u*pngSize;
		half2 faceright = p_faceright_u*pngSize;
		half2 chinleft_u = p_chinleft_u*pngSize;
		half2 chinright_u = p_chinright_u*pngSize;
		half2 noseleft = p_noseleft*pngSize;
		half2 noseright = p_noseright*pngSize;

		float2 pChinCenter = nose + (chin - nose)*0.7;
		float2 srcPoint = float2(0.0f,0.0f);
		float2 dstPoint = float2(0.0f,0.0f);
		float2 offsets = float2(0.0f,0.0f);

		float radius = 0.0f;
		float weight = 1.0f;
		float currentDistance = 0.0f;

		float standardLength = distance(eyea,eyeb);
		radius = standardLength*0.3f;
		currentDistance = distance(curCoord, eyea);
		if (currentDistance <= radius)
		{
			weight = pow(currentDistance / radius, u_bigEyeFactor);
			curCoord = eyea + (curCoord - eyea)*weight;
		}
		currentDistance = distance(curCoord, eyeb);
		if (currentDistance <= radius)
		{
			weight = pow(currentDistance / radius, u_bigEyeFactor);
			curCoord = eyeb + (curCoord - eyeb)*weight;
		}

		radius = standardLength*0.4f;
		srcPoint = noseleft;
		dstPoint = srcPoint + (nose - srcPoint)*0.05f;
		offsets = faceStretch(curCoord, srcPoint, dstPoint, radius, 1.0f);
		if (u_slimFactor <= 1.0f)
		{
			curCoord = curCoord - offsets*u_slimFactor;
		}
		else
		{
			curCoord = curCoord - offsets;
		}

		srcPoint = noseright;
		dstPoint = srcPoint + (nose - srcPoint)*0.05;
		offsets = faceStretch(curCoord, srcPoint, dstPoint, radius, 1.0);
		if (u_slimFactor <= 1.0)
		{
			curCoord = curCoord - offsets*u_slimFactor;
		}
		else
		{
			curCoord = curCoord - offsets;
		}

		radius = standardLength*0.7;
		srcPoint = nose;
		dstPoint = srcPoint + (chin - srcPoint)*0.03;
		offsets = faceStretch(curCoord, srcPoint, dstPoint, radius, 1.0);
		if (u_slimFactor <= 1.0)
		{
			curCoord = curCoord - offsets*u_slimFactor;
		}
		else
		{
			curCoord = curCoord - offsets;
		}

		radius = standardLength*1.0;
		srcPoint = faceleft;
		dstPoint = srcPoint + (nose - srcPoint)*0.015;
		offsets = faceStretch(curCoord, srcPoint, dstPoint, radius, 1.0);
		curCoord = curCoord - offsets*u_slimFactor;

		srcPoint = faceright;
		dstPoint = srcPoint + (nose - srcPoint)*0.015;
		offsets = faceStretch(curCoord, srcPoint, dstPoint, radius, 1.0);
		curCoord = curCoord - offsets*u_slimFactor;

		/*radius = standardLength*1.2;
		srcPoint = chinleft;
		dstPoint = srcPoint + (nose - srcPoint)*0.015;
		offsets = faceStretch(curCoord, srcPoint, dstPoint, radius, 1.0);
		curCoord = curCoord - offsets*u_slimFactor;

		srcPoint = chinright;
		dstPoint = srcPoint + (nose - srcPoint)*0.015;
		offsets = faceStretch(curCoord, srcPoint, dstPoint, radius, 1.0);
		curCoord = curCoord - offsets*u_slimFactor;*/

		//radius = standardLength*0.8;
		srcPoint = chinleft_u;
		dstPoint = srcPoint + (pChinCenter - srcPoint)*0.08;
		offsets = faceStretch(curCoord, srcPoint, dstPoint, radius, 1.0);
		curCoord = curCoord - offsets*u_slimFactor;

		srcPoint = chinright_u;
		dstPoint = srcPoint + (pChinCenter - srcPoint)*0.08;
		offsets = faceStretch(curCoord, srcPoint, dstPoint, radius, 1.0);
		curCoord = curCoord - offsets*u_slimFactor;

		radius = standardLength*1.0;
		srcPoint = chin;
		dstPoint = srcPoint + (nose - srcPoint)*0.005;
		offsets = faceStretch(curCoord, srcPoint, dstPoint, radius, 1.0);
		curCoord = curCoord - offsets*u_slimFactor;

		return newCoord = curCoord / pngSize;

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

		float mul_x = 2.0 / x_a;
		float mul_y = 2.0 / y_a;

		float2 blurCoordinates0 = uv + float2(0.0 * mul_x, -10.0 * mul_y);
		//float2 blurCoordinates1 = uv + float2(5.0 * mul_x, -8.0 * mul_y);
		//float2 blurCoordinates2 = uv + float2(8.0 * mul_x, -5.0 * mul_y);
		//float2 blurCoordinates3 = uv + float2(10.0 * mul_x, 0.0 * mul_y);
		float2 blurCoordinates4 = uv + float2(8.0 * mul_x, 5.0 * mul_y);
		//float2 blurCoordinates5 = uv + float2(5.0 * mul_x, 8.0 * mul_y);
		//float2 blurCoordinates6 = uv + float2(0.0 * mul_x, 10.0 * mul_y);
		//float2 blurCoordinates7 = uv + float2(-5.0 * mul_x, 8.0 * mul_y);
		//float2 blurCoordinates8 = uv + float2(-8.0 * mul_x, 5.0 * mul_y);
		float2 blurCoordinates9 = uv + float2(-10.0 * mul_x, 0.0 * mul_y);
		//float2 blurCoordinates10 = uv + float2(-8.0 * mul_x, -5.0 * mul_y);
		//float2 blurCoordinates11 = uv + float2(-5.0 * mul_x, -8.0 * mul_y);

		mul_x = 1.6 / x_a;
		mul_y = 1.6 / y_a;

		//float2 blurCoordinates12 = uv + float2(0.0 * mul_x, -6.0 * mul_y);
		//float2 blurCoordinates13 = uv + float2(-4.0 * mul_x, -4.0 * mul_y);
		float2 blurCoordinates14 = uv + float2(-6.0 * mul_x, 0.0 * mul_y);
		//float2 blurCoordinates15 = uv + float2(-4.0 * mul_x, 4.0 * mul_y);
		//float2 blurCoordinates16 = uv + float2(0.0 * mul_x, 6.0 * mul_y);
		//float2 blurCoordinates17 = uv + float2(4.0 * mul_x, 4.0 * mul_y);
		float2 blurCoordinates18 = uv + float2(6.0 * mul_x, 0.0 * mul_y);
		float2 blurCoordinates19 = uv + float2(4.0 * mul_x, -4.0 * mul_y);

		distanceNormalizationFactor = 3.6;

		central = tex2D(_MainTex, uv).g;
		gaussianWeightTotal = 0.2;
		sum = central * 0.2;

		sample_ = tex2D(_MainTex, blurCoordinates0).g;
		distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
		gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
		gaussianWeightTotal += gaussianWeight;
		sum += sample_ * gaussianWeight;

		//			sample_ = tex2D(_MainTex, blurCoordinates1).g;
		//			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
		//			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
		//			gaussianWeightTotal += gaussianWeight;
		//			sum += sample_ * gaussianWeight;
		//
		//			sample_ = tex2D(_MainTex, blurCoordinates2).g;
		//			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
		//			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
		//			gaussianWeightTotal += gaussianWeight;
		//			sum += sample_ * gaussianWeight;
		//
		//			sample_ = tex2D(_MainTex, blurCoordinates3).g;
		//			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
		//			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
		//			gaussianWeightTotal += gaussianWeight;
		//			sum += sample_ * gaussianWeight;

		sample_ = tex2D(_MainTex, blurCoordinates4).g;
		distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
		gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
		gaussianWeightTotal += gaussianWeight;
		sum += sample_ * gaussianWeight;

		//			sample_ = tex2D(_MainTex, blurCoordinates5).g;
		//			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
		//			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
		//			gaussianWeightTotal += gaussianWeight;
		//			sum += sample_ * gaussianWeight;
		//
		//			sample_ = tex2D(_MainTex, blurCoordinates6).g;
		//			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
		//			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
		//			gaussianWeightTotal += gaussianWeight;
		//			sum += sample_ * gaussianWeight;
		//
		//			sample_ = tex2D(_MainTex, blurCoordinates7).g;
		//			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
		//			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
		//			gaussianWeightTotal += gaussianWeight;
		//			sum += sample_ * gaussianWeight;
		//
		//			sample_ = tex2D(_MainTex, blurCoordinates8).g;
		//			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
		//			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
		//			gaussianWeightTotal += gaussianWeight;
		//			sum += sample_ * gaussianWeight;

		sample_ = tex2D(_MainTex, blurCoordinates9).g;
		distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
		gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
		gaussianWeightTotal += gaussianWeight;
		sum += sample_ * gaussianWeight;

		//			sample_ = tex2D(_MainTex, blurCoordinates10).g;
		//			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
		//			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
		//			gaussianWeightTotal += gaussianWeight;
		//			sum += sample_ * gaussianWeight;
		//
		//			sample_ = tex2D(_MainTex, blurCoordinates11).g;
		//			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
		//			gaussianWeight = 0.08 * (1.0 - distanceFromCentralColor);
		//			gaussianWeightTotal += gaussianWeight;
		//			sum += sample_ * gaussianWeight;
		//
		//			sample_ = tex2D(_MainTex, blurCoordinates12).g;
		//			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
		//			gaussianWeight = 0.1 * (1.0 - distanceFromCentralColor);
		//			gaussianWeightTotal += gaussianWeight;
		//			sum += sample_ * gaussianWeight;
		//
		//			sample_ = tex2D(_MainTex, blurCoordinates13).g;
		//			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
		//			gaussianWeight = 0.1 * (1.0 - distanceFromCentralColor);
		//			gaussianWeightTotal += gaussianWeight;
		//			sum += sample_ * gaussianWeight;
		//
		sample_ = tex2D(_MainTex, blurCoordinates14).g;
		distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
		gaussianWeight = 0.1 * (1.0 - distanceFromCentralColor);
		gaussianWeightTotal += gaussianWeight;
		sum += sample_ * gaussianWeight;

		//			sample_ = tex2D(_MainTex, blurCoordinates15).g;
		//			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
		//			gaussianWeight = 0.1 * (1.0 - distanceFromCentralColor);
		//			gaussianWeightTotal += gaussianWeight;
		//			sum += sample_ * gaussianWeight;
		//
		//			sample_ = tex2D(_MainTex, blurCoordinates16).g;
		//			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
		//			gaussianWeight = 0.1 * (1.0 - distanceFromCentralColor);
		//			gaussianWeightTotal += gaussianWeight;
		//			sum += sample_ * gaussianWeight;
		//
		//			sample_ = tex2D(_MainTex, blurCoordinates17).g;
		//			distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
		//			gaussianWeight = 0.1 * (1.0 - distanceFromCentralColor);
		//			gaussianWeightTotal += gaussianWeight;
		//			sum += sample_ * gaussianWeight;

		sample_ = tex2D(_MainTex, blurCoordinates18).g;
		distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
		gaussianWeight = 0.1 * (1.0 - distanceFromCentralColor);
		gaussianWeightTotal += gaussianWeight;
		sum += sample_ * gaussianWeight;

		sample_ = tex2D(_MainTex, blurCoordinates19).g;
		distanceFromCentralColor = min(abs(central - sample_) * distanceNormalizationFactor, 1.0);
		gaussianWeight = 0.1 * (1.0 - distanceFromCentralColor);
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

		float aa = 1.0 + pow(sum, 0.3)*0.09;
		float3 smoothColor = centralColor*aa - float3(sample_, sample_, sample_)*(aa - 1.0);// get smooth color
		smoothColor = clamp(smoothColor, float3(0.0, 0.0, 0.0), float3(1.0, 1.0, 1.0));//make smooth color right
																					   ////smoothColor = lerp(centralColor, smoothColor, pow(centralColor.g, 0.09));
		smoothColor = lerp(centralColor, smoothColor, pow(centralColor.g, 0.5));
		smoothColor = lerp(centralColor, smoothColor, _beauty);//0.99
		return float4(pow(smoothColor, float3(1.0,1.0,1.0)), 1.0);//0.8, 0.92, 0.9
	}

	/////////////////////////////////////
	v2f vert(appdata IN)
	{
		v2f OUT;
		OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
		OUT.texcoord = IN.texcoord;
		//OUT.color = IN.color;
		OUT.projPos = ComputeScreenPos(OUT.vertex);
		return OUT;
	}

	float4 frag(v2f i) : COLOR
	{
		float2 newCoord = i.texcoord.xy;
#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			newCoord.y = 1 - newCoord.y;
#endif

		//if (u_tracksuccess == 1)
		//{

		//	newCoord = morghone(newCoord, 0);

		//	if (u_tracknum == 1)
		//	{
		//		newCoord = morghone(newCoord, 1);
		//	}

		//}

		half4 col = tex2D(_MainTex, newCoord);
		//float4 col = tex2D(_MainTex, i.texcoord.xy);//美颜值与大眼瘦脸都不开启
		if (_beauty == 0)
		{
			// apply fog
			return col;
		}
		//col = Beauty(i.texcoord.xy);//只开启美颜

		col = Beauty(newCoord);//开启美颜值与大眼瘦脸
		return col;
	}




		ENDCG
	}
	}
}
