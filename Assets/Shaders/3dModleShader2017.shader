// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:33024,y:32586,varname:node_4013,prsc:2|diff-1844-OUT,spec-5383-OUT,gloss-3394-OUT,normal-1380-RGB,emission-4492-OUT;n:type:ShaderForge.SFN_Color,id:1304,x:31756,y:32242,ptovrint:False,ptlb:df_Color,ptin:_df_Color,varname:node_1304,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:4467,x:32148,y:32076,ptovrint:False,ptlb:df,ptin:_df,varname:node_4467,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:d0cee3b33f41b884990abeae6e54a0e8,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1844,x:32806,y:32326,varname:node_1844,prsc:2|A-4467-RGB,B-1304-RGB;n:type:ShaderForge.SFN_Slider,id:5312,x:32089,y:32681,ptovrint:False,ptlb:light_slider,ptin:_light_slider,varname:node_5312,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.184779,max:10;n:type:ShaderForge.SFN_Tex2d,id:3166,x:31766,y:32594,ptovrint:False,ptlb:light,ptin:_light,varname:node_3166,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e8423ac4f2eaec9458ae24ed91b681d6,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:5383,x:32606,y:32607,varname:node_5383,prsc:2|A-3166-RGB,B-5312-OUT;n:type:ShaderForge.SFN_Tex2d,id:1380,x:32733,y:32924,ptovrint:False,ptlb:normal,ptin:_normal,varname:node_1380,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:9abaeb4c4aed9c742a03ff4bab865f66,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Slider,id:6046,x:32350,y:33354,ptovrint:False,ptlb:Emission_slider,ptin:_Emission_slider,varname:node_6046,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.6495727,max:1;n:type:ShaderForge.SFN_Multiply,id:4492,x:32788,y:33251,varname:node_4492,prsc:2|A-3209-RGB,B-6046-OUT;n:type:ShaderForge.SFN_Tex2d,id:3209,x:32303,y:33160,ptovrint:False,ptlb:Emission_color,ptin:_Emission_color,varname:node_3209,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:b67082000af684844b95268a49615d65,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:2068,x:32412,y:32779,ptovrint:False,ptlb:gloss_diffuse,ptin:_gloss_diffuse,varname:node_2068,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:fc4b2a80b5b68364197f645ee6858e3a,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3394,x:32605,y:32760,varname:node_3394,prsc:2|A-2068-R,B-9266-OUT;n:type:ShaderForge.SFN_Slider,id:9266,x:32236,y:32976,ptovrint:False,ptlb:gloss,ptin:_gloss,varname:node_9266,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Add,id:6141,x:32475,y:33245,varname:node_6141,prsc:2;proporder:1304-4467-5312-3166-1380-6046-3209-2068-9266;pass:END;sub:END;*/

Shader "Shader Forge/3dModleShader2017" {
    Properties {
        _df_Color ("df_Color", Color) = (1,1,1,1)
        _df ("df", 2D) = "white" {}
        _light_slider ("light_slider", Range(0, 10)) = 0.184779
        _light ("light", 2D) = "white" {}
        _normal ("normal", 2D) = "bump" {}
        _Emission_slider ("Emission_slider", Range(0, 1)) = 0.6495727
        _Emission_color ("Emission_color", 2D) = "white" {}
        _gloss_diffuse ("gloss_diffuse", 2D) = "white" {}
        _gloss ("gloss", Range(0, 3)) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha 
            //ZWrite on
            //Cull Off 

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            //#pragma multi_compile_fwdbase_fullshadows
           // #pragma multi_compile_fog
            //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _df_Color;
            uniform sampler2D _df; uniform float4 _df_ST;
            uniform float _light_slider;
            uniform sampler2D _light; uniform float4 _light_ST;
            uniform sampler2D _normal; uniform float4 _normal_ST;
            uniform float _Emission_slider;
            uniform sampler2D _Emission_color; uniform float4 _Emission_color_ST;
            uniform sampler2D _gloss_diffuse; uniform float4 _gloss_diffuse_ST;
            uniform float _gloss;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                //UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _normal_var = UnpackNormal(tex2D(_normal,TRANSFORM_TEX(i.uv0, _normal)));
                float3 normalLocal = _normal_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float4 _gloss_diffuse_var = tex2D(_gloss_diffuse,TRANSFORM_TEX(i.uv0, _gloss_diffuse));
                float gloss = (_gloss_diffuse_var.r*_gloss);
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float4 _light_var = tex2D(_light,TRANSFORM_TEX(i.uv0, _light));
                float3 specularColor = (_light_var.rgb*_light_slider);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float4 _df_var = tex2D(_df,TRANSFORM_TEX(i.uv0, _df));
                float3 diffuseColor = (_df_var.rgb*_df_Color.rgb);
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float4 _Emission_color_var = tex2D(_Emission_color,TRANSFORM_TEX(i.uv0, _Emission_color));
                float3 emissive = (_Emission_color_var.rgb*_Emission_slider);
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                //UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            //Blend One One
            Blend SrcAlpha OneMinusSrcAlpha 
            //Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            //#pragma multi_compile_fwdadd_fullshadows
            //#pragma multi_compile_fog
            //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _df_Color;
            uniform sampler2D _df; uniform float4 _df_ST;
            uniform float _light_slider;
            uniform sampler2D _light; uniform float4 _light_ST;
            uniform sampler2D _normal; uniform float4 _normal_ST;
            uniform float _Emission_slider;
            uniform sampler2D _Emission_color; uniform float4 _Emission_color_ST;
            uniform sampler2D _gloss_diffuse; uniform float4 _gloss_diffuse_ST;
            uniform float _gloss;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                //UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _normal_var = UnpackNormal(tex2D(_normal,TRANSFORM_TEX(i.uv0, _normal)));
                float3 normalLocal = _normal_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float4 _gloss_diffuse_var = tex2D(_gloss_diffuse,TRANSFORM_TEX(i.uv0, _gloss_diffuse));
                float gloss = (_gloss_diffuse_var.r*_gloss);
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float4 _light_var = tex2D(_light,TRANSFORM_TEX(i.uv0, _light));
                float3 specularColor = (_light_var.rgb*_light_slider);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _df_var = tex2D(_df,TRANSFORM_TEX(i.uv0, _df));
                float3 diffuseColor = (_df_var.rgb*_df_Color.rgb);
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                //UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
