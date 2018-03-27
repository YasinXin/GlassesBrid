Shader "CameraFilter/FaceMorph_Ghost"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
		SubShader
	{
		ZTest Always
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma target 3.0

			#include "UnityCG.cginc"


			uniform sampler2D _MainTex;
			uniform int u_tracksuccess;
			uniform float u_mouthopenparam;

			uniform float4 _chin;//68Landmarks: 8               106Landmarks: 16
			uniform float4 _eyea;//68Landmarks:ave 37,38,40,41  106Landmarks: 74
			uniform float4 _eyeb;//68Landmarks:ave 43,44,46,47  106Landmarks: 77
			uniform float4 _nose;//68Landmarks: 30              106Landmarks: 46
			uniform float4 _chinleft;//5
			uniform float4 _chinright;//11
			uniform float4 _mouthleft;  //49
			uniform float4 _mouthright; //55
			uniform float4 _mouthtop;//52
			uniform float4 _mouthbottom;//58

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
				float3 centralColor;
				float2 newCoord = i.texcoord.xy;
				float2 p_chin= float2(_chin.x, _chin.y); 		//68Landmarks:mid 3,4          106Landmarks: 7
				float2 p_eyea = float2(_eyea.x, _eyea.y); 			//68Landmarks: 8               106Landmarks: 16
				float2 p_eyeb = float2(_eyeb.x, _eyeb.y); 	//68Landmarks:mid 12,13        106Landmarks: 25
				float2 p_nose = float2(_nose.x, _nose.y); 			//68Landmarks: 30              106Landmarks: 46
				float2 p_chinleft = float2(_chinleft.x, _chinleft.y); 			//68Landmarks:ave 37,38,40,41  106Landmarks: 74
				float2 p_chinright = float2(_chinright.x, _chinright.y); 			//68Landmarks:ave 43,44,46,47  106Landmarks: 77
				float2 p_mouthleft = float2(_mouthleft.x, _mouthleft.y);			//68Landmarks:48               106Landmarks: 84
				float2 p_mouthright = float2(_mouthright.x, _mouthright.y);			//68Landmarks:54               106Landmarks: 90
				float2 p_mouthtop = float2(_mouthtop.x, _mouthtop.y);		//68Landmarks:mid 6,7          106Landmarks: 13
				float2 p_mouthbottom = float2(_mouthbottom.x, _mouthbottom.y);		//68Landmarks:mid 9,10         106Landmarks: 19


				if (u_tracksuccess == 1)
				{
					float2 pngSize = float2(0.72f, 1.28f);
					float2 eyea = p_eyea * pngSize;
					float2 eyeb = p_eyeb * pngSize;
					float2 nose = p_nose;
					float2 chin = p_chin;
					float2 chinCenter = nose + (chin - nose) * 0.99;
					float2 mouthLeft = p_mouthleft * pngSize;
					float2 mouthRight = p_mouthright * pngSize;
					float2 mouthTop = p_mouthtop * pngSize;
					float2 mouthbottom = p_mouthbottom * pngSize;
					float2 mouthCenter = (mouthTop + mouthbottom) * 0.5;
					float mouth_width = distance(mouthLeft, mouthRight);
					float2 newCoordNorm = newCoord * pngSize;

					if (u_mouthopenparam > 0.9)
					{
						float weight = 0.0;

						float eyeRadius = distance(eyea, eyeb)*0.35;
						float dis_eye1 = distance(newCoordNorm, eyea);
						if (dis_eye1 <= eyeRadius)
						{
							weight = pow(dis_eye1 / eyeRadius, 0.5);
							newCoordNorm = eyea + (newCoordNorm - eyea) * weight;
							newCoord = newCoordNorm / pngSize;
						}

						float dis_eye2 = distance(newCoordNorm, eyeb);
						if (dis_eye2 <= eyeRadius)
						{
							weight = pow(dis_eye2 / eyeRadius,0.5);
							newCoordNorm = eyeb + (newCoordNorm - eyeb) * weight;
							newCoord = newCoordNorm / pngSize;
						}

						float dis_mouth = distance(newCoordNorm, mouthCenter);
						if (dis_mouth <= mouth_width * 0.8)
						{
							weight = dis_mouth / (mouth_width * 0.8);
							newCoordNorm = mouthCenter + (newCoordNorm - mouthCenter) * weight;
							newCoord = newCoordNorm / pngSize;
						}

						float face_width = distance(eyea, eyeb);
						float radius = face_width*0.8;
						float2 targetchin = nose + (chin - nose) * 1.1;
						float2 chinplus = float2(0.0,0.0);
						chinplus = faceStretch(newCoord, chin, targetchin, radius, 1.0);
						newCoord = newCoord - chinplus;
						radius = face_width*0.85;
						float2 leftC = p_chinleft;
						float2 targetleftC = nose + (leftC - nose) * 0.85;
						float2 leftCplus = float2(0.0, 0.0);
						leftCplus = faceStretch(newCoord, leftC, targetleftC, radius, 1.0);
						newCoord = newCoord - leftCplus;
						float2 rightC = p_chinright;
						float2 targetrightC = nose + (rightC - nose) * 0.85;
						float2 rightCplus = float2(0.0, 0.0);
						rightCplus = faceStretch(newCoord, rightC, targetrightC, radius, 1.0);
						newCoord = newCoord - rightCplus;
					}
				}

				float4 txt = tex2D(_MainTex, newCoord);//.bgr;
				return  txt;
			}
			ENDCG
		}
	}
}