Shader "Custom/GlassShader" {
	Properties{
		_MainTint("Main Color", Color) = (1, 1, 1, 1)
		_AlphaVal("Alpha", Range(0, 1)) = 0.1
		_CubeMap("Sky Box", CUBE) = ""{}
	_ReflPower("Reflect Power", Range(0, 1)) = 1
	}
		SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 200

		CGPROGRAM
#pragma surface surf BlinnPhong alpha

		fixed4 _MainTint;
	float _AlphaVal;
	samplerCUBE _CubeMap;
	float _ReflPower;

	struct Input {
		float3 worldRefl;
		float3 viewDir;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		half4 s = texCUBE(_CubeMap, IN.worldRefl);
		o.Albedo = _MainTint.rgb;
		o.Emission = s.rgb * _ReflPower;
		o.Alpha = _AlphaVal;
	}
	ENDCG
	}
		FallBack "Diffuse"
}