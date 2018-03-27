Shader "lidx/lidx_filter_eyeface_new"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			ZTest Always
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

			struct appdata
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

			float2 faceStretch(float2 textureCoord, float2 originPosition, float2 targetPosition, float radius, float curve)
			{
			    float2 direction = targetPosition - originPosition;
			    float lengthA = length(direction);
				if(lengthA<0.0001)  
				{
				 return direction;
				}
			    float lengthB = min(lengthA, radius);
			    direction *= lengthB / lengthA;
			    float infect = distance(textureCoord, originPosition)/radius;
			    infect = clamp(1.0-infect,0.0,1.0);
			    infect = pow(infect, curve);
			    return direction * infect;
			}

			float2 morghone(float2 newCoord,int id)
			{
				float2 p_faceleft_u; 		
				float2 p_faceright_u; 
						

				float2 p_eyea; 		
				float2 p_eyeb; 			

				float2 p_chin;			
				float2 p_chinleft_d;			
				float2 p_chinright_d;		
				float2 p_chinleft_u;	
				float2 p_chinright_u;	

				float2 p_nose;
				float2 p_noseleft;	
				float2 p_noseright;

				if(id == 0)
				{
					p_faceleft_u= float2(_faceleft_u.x, _faceleft_u.y); 		
					p_faceright_u = float2(_faceright_u.x, _faceright_u.y); 
							

					p_eyea = float2(_eyea.x, _eyea.y); 		
					p_eyeb = float2(_eyeb.x, _eyeb.y); 			

					p_chin = float2(_chin.x, _chin.y);			
					p_chinleft_d = float2(_chinleft_d.x, _chinleft_d.y);			
					p_chinright_d = float2(_chinright_d.x, _chinright_d.y);		
					p_chinleft_u = float2(_chinleft_u.x, _chinleft_u.y);	
					p_chinright_u = float2(_chinright_u.x, _chinright_u.y);	

					 p_nose = float2(_nose.x, _nose.y);		
					 p_noseleft = float2(_noseleft.x, _noseleft.y);	
					 p_noseright = float2(_noseright.x, _noseright.y);	
				}


				if(id == 1)
				{
					
					 p_faceleft_u= float2(_faceleft_u.z, _faceleft_u.w); 		
					 p_faceright_u = float2(_faceright_u.z, _faceright_u.w); 
							

					 p_eyea = float2(_eyea.z, _eyea.w); 		
					 p_eyeb = float2(_eyeb.z, _eyeb.w); 			

					 p_chin = float2(_chin.z, _chin.w);			
					 p_chinleft_d = float2(_chinleft_d.z, _chinleft_d.w);			
					 p_chinright_d = float2(_chinright_d.z, _chinright_d.w);		
					 p_chinleft_u = float2(_chinleft_u.z, _chinleft_u.w);	
					 p_chinright_u = float2(_chinright_u.z, _chinright_u.w);	

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
					float2 chin = p_chin*pngSize;
					float2 faceleft = p_faceleft_u*pngSize;
					float2 faceright = p_faceright_u*pngSize;
					float2 chinleft_u = p_chinleft_u*pngSize;
					float2 chinright_u = p_chinright_u*pngSize;
					float2 noseleft = p_noseleft*pngSize;
					float2 noseright = p_noseright*pngSize;

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

					radius = standardLength*1.2;
					srcPoint = chinleft;
					dstPoint = srcPoint + (nose - srcPoint)*0.015;
					offsets = faceStretch(curCoord, srcPoint, dstPoint, radius, 1.0);
					curCoord = curCoord - offsets*u_slimFactor;

					srcPoint = chinright;
					dstPoint = srcPoint + (nose - srcPoint)*0.015;
					offsets = faceStretch(curCoord, srcPoint, dstPoint, radius, 1.0);
					curCoord = curCoord - offsets*u_slimFactor;

					//radius = standardLength*0.8;
					srcPoint = chinleft_u;
					dstPoint = srcPoint + (pChinCenter - srcPoint)*0.08; 
					offsets = faceStretch(curCoord, srcPoint, dstPoint, radius, 1.0);
					curCoord = curCoord - offsets*u_slimFactor;

					srcPoint = chinright_u;
					dstPoint = srcPoint + (pChinCenter - srcPoint)*0.08;
					offsets = faceStretch(curCoord, srcPoint, dstPoint, radius, 1.0);
					curCoord = curCoord - offsets*u_slimFactor;

					radius = standardLength*1.5;
					srcPoint = chin;
					dstPoint = srcPoint + (nose - srcPoint)*0.005;
					offsets = faceStretch(curCoord, srcPoint, dstPoint, radius, 1.0);
					curCoord = curCoord - offsets*u_slimFactor;

					return newCoord = curCoord / pngSize;
						
			}

			v2f vert(appdata IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;
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
			
				if (u_tracksuccess == 1)
				{

					 newCoord = morghone(newCoord,0);
					
					if(u_tracknum == 1)
					{
						newCoord = morghone(newCoord,1);
					}

				}

				float4 col = tex2D(_MainTex, newCoord);
				return col;
			}

			ENDCG
		}
	}
}
