Shader "CameraFilter/FaceMorph_Final" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
		SubShader{
			Pass
		{
			ZTest Always
			CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	//#pragma fragmentoption ARB_precision_hint_fastest
	#pragma target 3.0
	//#pragma glsl
	#include "UnityCG.cginc"

		uniform sampler2D _MainTex;

		uniform float4 _faceleft; 		//68Landmarks:mid 3,4          106Landmarks: 7
		uniform float4 _chin; 			//68Landmarks: 8               106Landmarks: 16
		uniform float4 _faceright; 	//68Landmarks:mid 12,13        106Landmarks: 25
		uniform float4 _nose; 			//68Landmarks: 30              106Landmarks: 46
		uniform float4 _eyea; 			//68Landmarks:ave 37,38,40,41  106Landmarks: 74
		uniform float4 _eyeb; 			//68Landmarks:ave 43,44,46,47  106Landmarks: 77
		uniform float4 _left;//嘴			//68Landmarks:48               106Landmarks: 84
		uniform float4 _right;//嘴			//68Landmarks:54               106Landmarks: 90
		uniform float4 _chinleft;		//68Landmarks:mid 6,7          106Landmarks: 13
		uniform float4 _chinright;		//68Landmarks:mid 9,10         106Landmarks: 19

		uniform float2 _MainTex_TexelSize;

		float x_a = 0.72f;
		float y_a = 1.28f;

		struct appdata_t
		{
			float4 vertex   : POSITION;
			float4 color    : COLOR;
			float2 texcoord : TEXCOORD0;

		};
		struct v2f
		{
			half2 texcoord  : TEXCOORD0;
			float4 vertex   : SV_POSITION;
			fixed4 color : COLOR;
			float4 projPos : TEXCOORD1;
		};

		v2f vert(appdata_t IN)
		{
			v2f OUT;
			OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
			OUT.texcoord = IN.texcoord;
			OUT.color = IN.color;
			OUT.projPos = ComputeScreenPos(OUT.vertex);
			return OUT;
		}

		float2 faceStretch(float2 textureCoord, float2 originPosition, float2 targetPosition, float radius, float curve)
		{
			float2 direction = targetPosition - originPosition;
			float lengthA = length(direction);
			float lengthB = min(lengthA, radius);
			direction *= lengthB / lengthA;
			float infect = distance(textureCoord, originPosition) / radius;
			infect = clamp(1.0 - infect, 0.0, 1.0);
			infect = pow(infect, curve);

			return direction * infect;
		}

		float4 frag(v2f i) : COLOR
		{
			float2 uv = i.texcoord.xy;
	#if UNITY_UV_STARTS_AT_TOP
			if (_MainTex_TexelSize.y < 0)
				uv.y = 1 - uv.y;
	#endif
			
			float4 txt = tex2D(_MainTex, uv);

			float2 p_faceleft= float2(_faceleft.x, _faceleft.y); 		//68Landmarks:mid 3,4          106Landmarks: 7
			float2 p_chin = float2(_chin.x, _chin.y); 			//68Landmarks: 8               106Landmarks: 16
			float2 p_faceright = float2(_faceright.x, _faceright.y); 	//68Landmarks:mid 12,13        106Landmarks: 25
			float2 p_nose = float2(_nose.x, _nose.y); 			//68Landmarks: 30              106Landmarks: 46
			float2 p_eyea = float2(_eyea.x, _eyea.y); 			//68Landmarks:ave 37,38,40,41  106Landmarks: 74
			float2 p_eyeb = float2(_eyeb.x, _eyeb.y); 			//68Landmarks:ave 43,44,46,47  106Landmarks: 77
			float2 p_left = float2(_left.x, _left.y);			//68Landmarks:48               106Landmarks: 84
			float2 p_right = float2(_right.x, _right.y);			//68Landmarks:54               106Landmarks: 90
			float2 p_chinleft = float2(_chinleft.x, _chinleft.y);		//68Landmarks:mid 6,7          106Landmarks: 13
			float2 p_chinright = float2(_chinright.x, _chinright.y);		//68Landmarks:mid 9,10         106Landmarks: 19

			//
			if (p_chinleft.x > 0.01 && p_chinleft.y > 0.01)
			{
				float2 pngSize = float2(0.72f, 1.28f);

				float2 eyea = p_eyea * pngSize;
				float2 eyeb = p_eyeb * pngSize;
				float2 left = p_left * pngSize;
				float2 right = p_right * pngSize;
				float2 nose = p_nose * pngSize;
				float2 chin = p_chin * pngSize;

				float weight = 0.0;
				float face_width = distance(eyea, eyeb);

				float eyedis = face_width;

				float2 newCoord = uv * pngSize;

				// eye1
				float eyeRadius = distance(eyea, eyeb)*0.26;
				float dis_eye1 = distance(newCoord, eyea);
				if (dis_eye1 <= eyeRadius)
				{
					weight = pow(dis_eye1 / eyeRadius, 0.24);
					newCoord = eyea + (newCoord - eyea) * weight;
				}

				// eye2
				float dis_eye2 = distance(newCoord, eyeb);
				if (dis_eye2 <= eyeRadius)
				{
					weight = pow(dis_eye2 / eyeRadius, 0.24);
					newCoord = eyeb + (newCoord - eyeb) * weight;
				}

				float radius = face_width*1.0;

				float2 leftF = p_faceleft * pngSize;
				float2 targetleftF = nose + (leftF - nose) * 0.94;
				float2 leftFplus = float2(0.0,0.0);
				leftFplus = faceStretch(newCoord, leftF, targetleftF, radius, 1.0);
				newCoord = newCoord - leftFplus;

				float2 rightF = p_faceright * pngSize;
				float2 targetrightF = nose + (rightF - nose) * 0.94;
				float2 rightFplus = float2(0.0, 0.0);
				rightFplus = faceStretch(newCoord, rightF, targetrightF, radius, 1.0);
				newCoord = newCoord - rightFplus;

				radius = face_width*1.5;
				float2 targetchin = nose + (chin - nose) * 0.95;
				float2 chinplus = float2(0.0, 0.0);
				chinplus = faceStretch(newCoord, chin, targetchin, radius, 1.0);
				newCoord = newCoord - chinplus;

				float2 chinCenter = nose + (chin - nose) * 0.7;

				radius = face_width*1.2;
				float2 leftC = p_chinleft * pngSize;
				float2 targetleftC = chinCenter + (leftC - chinCenter) * 0.93;
				float2 leftCplus = float2(0.0, 0.0);
				leftCplus = faceStretch(newCoord, leftC, targetleftC, radius, 1.0);
				newCoord = newCoord - leftCplus;

				float2 rightC = p_chinright * pngSize;
				float2 targetrightC = chinCenter + (rightC - chinCenter) * 0.93;
				float2 rightCplus = float2(0.0, 0.0);
				rightCplus = faceStretch(newCoord, rightC, targetrightC, radius, 1.0);
				newCoord = newCoord - rightCplus;

				txt = tex2D(_MainTex, newCoord / pngSize);
			}
		return  txt;
		}
			ENDCG
		}
	}
}