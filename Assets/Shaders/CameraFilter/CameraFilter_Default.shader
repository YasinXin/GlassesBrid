
Shader "CameraFilter/Default" { 
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader
{
Pass
{
ZTest Always
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"
uniform sampler2D _MainTex;
uniform float2 _MainTex_TexelSize;
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

float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy;
#if UNITY_UV_STARTS_AT_TOP
if (_MainTex_TexelSize.y < 0)
uv.y = 1-uv.y;
#endif

float4 txt = tex2D(_MainTex, uv);

return  txt;
}
ENDCG
}
}
}