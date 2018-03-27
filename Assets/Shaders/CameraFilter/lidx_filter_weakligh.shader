Shader "lidx/lidx_filter_weaklight"
{
	Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_inputImageTexture2("Texture2", 2D) = "white" {}
_blueColorLevel("blueColorLevel",float)= 15.0
_level("level",float) = 1.0
}
SubShader 
{
Pass
{
ZTest Always
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0
#include "UnityCG.cginc"

uniform sampler2D _MainTex;
uniform sampler2D _inputImageTexture2;
uniform float _blueColorLevel;
uniform float _level;

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
fixed4 color    : COLOR;
};   

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;

return OUT;
}


float4 frag (v2f i) : COLOR
{
float4 textureColor = tex2D(_MainTex, i.texcoord.xy);

				float blueColor = textureColor.b *_blueColorLevel;// _blueColorLevel;

				float2 quad1;

				quad1.y = floor(floor(blueColor) / 4.0);
				quad1.x = floor(blueColor) - (quad1.y * 4.0);
			
				float2 quad2;
				quad2.y = floor(ceil(blueColor) / 4.0);
				quad2.x = ceil(blueColor) - (quad2.y * 4.0);
			
				float2 texPos1;
				texPos1.x = (quad1.x * 0.25) + 0.5 / 64.0 + ((0.25 - 1.0 / 64.0) * textureColor.r);
				texPos1.y = 1 - ((quad1.y * 0.25) + 0.5 / 64.0 + ((0.25 - 1.0 / 64.0) * textureColor.g));
				
				float2 texPos2;
				texPos2.x = (quad2.x * 0.25) + 0.5 / 64.0 + ((0.25 - 1.0 / 64.0) * textureColor.r);
				texPos2.y = 1 - ((quad2.y * 0.25) + 0.5 / 64.0 + ((0.25 - 1.0 / 64.0) * textureColor.g));
				
				float4 newColor1 = tex2D(_inputImageTexture2, texPos1);

				float4 newColor2 = tex2D(_inputImageTexture2, texPos2);
				
				float4 newColor = lerp(newColor1, newColor2, frac(blueColor));

				return lerp(textureColor, float4(newColor.rgb, textureColor.a),_level);//
}

ENDCG
}

}
}