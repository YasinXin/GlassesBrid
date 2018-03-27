// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:33059,y:32710,varname:node_4013,prsc:2|diff-5028-OUT,amdfl-307-OUT;n:type:ShaderForge.SFN_Cubemap,id:4964,x:32634,y:32859,ptovrint:False,ptlb:cubemap,ptin:_cubemap,varname:node_4964,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,cube:69d2c4eb87d2b0c43b218c2872058795,pvfc:0;n:type:ShaderForge.SFN_Multiply,id:307,x:32855,y:32912,varname:node_307,prsc:2|A-4964-R,B-5488-OUT;n:type:ShaderForge.SFN_Slider,id:5488,x:32504,y:33039,ptovrint:False,ptlb:cubemap_slider,ptin:_cubemap_slider,varname:node_5488,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Color,id:7288,x:32650,y:32681,ptovrint:False,ptlb:color,ptin:_color,varname:node_7288,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:5028,x:32887,y:32706,varname:node_5028,prsc:2|A-9715-RGB,B-7288-RGB;n:type:ShaderForge.SFN_Tex2d,id:9715,x:32646,y:32499,ptovrint:False,ptlb:texture_diffuse,ptin:_texture_diffuse,varname:node_9715,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:8eaf6280ea404a340b4017f2db6d4285,ntxv:0,isnm:False;proporder:4964-5488-7288-9715;pass:END;sub:END;*/

Shader "Shader Forge/Glass_Surface2018" {
    Properties {
        _cubemap ("cubemap", Cube) = "_Skybox" {}
        _cubemap_slider ("cubemap_slider", Range(0, 1)) = 1
        _color ("color", Color) = (0.5,0.5,0.5,1)
        _texture_diffuse ("texture_diffuse", 2D) = "white" {}
        _alpha_slider ("_alpha_slider", Range(0, 1)) = 1
    }
    SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

		Blend SrcAlpha OneMinusSrcAlpha 
		Cull OFF
        Pass {
//            Name "FORWARD"
//           Tags {
//                "LightMode"="ForwardBase"
//            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            //#pragma multi_compile_fog
            //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform samplerCUBE _cubemap;
            uniform float _cubemap_slider;
            uniform float4 _color;
            uniform sampler2D _texture_diffuse; uniform float4 _texture_diffuse_ST;
            uniform float _alpha_slider;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                 float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse *= UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 node_307 = (texCUBE(_cubemap,viewReflectDirection).rgb*_cubemap_slider);

                //indirectDiffuse += float3(node_307,node_307,node_307); // Diffuse Ambient Light
                indirectDiffuse += node_307;
                float4 _texture_diffuse_var = tex2D(_texture_diffuse,i.uv0);

                float3 diffuseColor = (_texture_diffuse_var.rgb*_color.rgb);
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
                //float3 diffuse = node_307 + diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                //float3 finalColor = diffuseColor;
                fixed4 finalRGBA = fixed4(finalColor,_alpha_slider);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            //#pragma multi_compile_fog
            //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _color;
            uniform sampler2D _texture_diffuse; uniform float4 _texture_diffuse_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                //UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _texture_diffuse_var = tex2D(_texture_diffuse,TRANSFORM_TEX(i.uv0, _texture_diffuse));
                float3 diffuseColor = (_texture_diffuse_var.rgb*_color.rgb);
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
