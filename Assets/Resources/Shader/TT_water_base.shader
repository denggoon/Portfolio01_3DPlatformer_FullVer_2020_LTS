// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.17 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.17;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:False,mssp:False,bkdf:False,hqlp:True,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:200,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:9361,x:35263,y:33269,varname:node_9361,prsc:2|diff-5927-RGB,emission-7305-OUT,alpha-1434-OUT;n:type:ShaderForge.SFN_Color,id:5927,x:34314,y:32814,ptovrint:False,ptlb:Main_Color,ptin:_Main_Color,varname:node_5927,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.2426471,c2:0.4985802,c3:1,c4:0.278;n:type:ShaderForge.SFN_Tex2d,id:9540,x:33177,y:33289,ptovrint:False,ptlb:shiny_1,ptin:_shiny_1,varname:node_9540,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3e0322e299f34804d986ff710b581e28,ntxv:0,isnm:False|UVIN-25-UVOUT;n:type:ShaderForge.SFN_Multiply,id:9842,x:33433,y:33270,varname:node_9842,prsc:2|A-9540-RGB,B-4083-RGB;n:type:ShaderForge.SFN_TexCoord,id:25,x:32415,y:33097,varname:node_25,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:4083,x:33340,y:33824,ptovrint:False,ptlb:shiny_2,ptin:_shiny_2,varname:_WATER_2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3e0322e299f34804d986ff710b581e28,ntxv:0,isnm:False|UVIN-8425-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:8425,x:32977,y:33843,varname:node_8425,prsc:2,uv:0;n:type:ShaderForge.SFN_Slider,id:2045,x:33600,y:33412,ptovrint:False,ptlb:intensity,ptin:_intensity,varname:node_2045,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3,max:3;n:type:ShaderForge.SFN_Multiply,id:3624,x:34057,y:33329,varname:node_3624,prsc:2|A-9842-OUT,B-2045-OUT;n:type:ShaderForge.SFN_ComponentMask,id:9820,x:34301,y:33422,varname:node_9820,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-3624-OUT;n:type:ShaderForge.SFN_Add,id:1434,x:34598,y:33526,varname:node_1434,prsc:2|A-5927-A,B-9820-OUT;n:type:ShaderForge.SFN_Multiply,id:5825,x:33453,y:34116,varname:node_5825,prsc:2|A-9896-RGB,B-6776-RGB;n:type:ShaderForge.SFN_TexCoord,id:2212,x:32435,y:33943,varname:node_2212,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:6776,x:33360,y:34670,ptovrint:False,ptlb:wave_2,ptin:_wave_2,varname:_WATER_3,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3e0322e299f34804d986ff710b581e28,ntxv:0,isnm:False|UVIN-9920-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:9920,x:32997,y:34689,varname:node_9920,prsc:2,uv:0;n:type:ShaderForge.SFN_Slider,id:3973,x:33620,y:34258,ptovrint:False,ptlb:intensity2,ptin:_intensity2,varname:_intensity_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3,max:3;n:type:ShaderForge.SFN_Multiply,id:5645,x:34023,y:34107,varname:node_5645,prsc:2|A-5825-OUT,B-3973-OUT;n:type:ShaderForge.SFN_Add,id:7305,x:34664,y:33371,varname:node_7305,prsc:2|A-3624-OUT,B-5645-OUT;n:type:ShaderForge.SFN_Tex2d,id:9896,x:33176,y:34134,ptovrint:False,ptlb:wave_1,ptin:_wave_1,varname:node_9896,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2212-UVOUT;proporder:5927-9540-4083-9896-6776-2045-3973;pass:END;sub:END;*/

Shader "TT/water_base" {
    Properties {
        _Main_Color ("Main_Color", Color) = (0.2426471,0.4985802,1,0.278)
        _shiny_1 ("shiny_1", 2D) = "white" {}
        _shiny_2 ("shiny_2", 2D) = "white" {}
        _wave_1 ("wave_1", 2D) = "white" {}
        _wave_2 ("wave_2", 2D) = "white" {}
        _intensity ("intensity", Range(0, 3)) = 3
        _intensity2 ("intensity2", Range(0, 3)) = 3
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent+200"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _Main_Color;
            uniform sampler2D _shiny_1; uniform float4 _shiny_1_ST;
            uniform sampler2D _shiny_2; uniform float4 _shiny_2_ST;
            uniform float _intensity;
            uniform sampler2D _wave_2; uniform float4 _wave_2_ST;
            uniform float _intensity2;
            uniform sampler2D _wave_1; uniform float4 _wave_1_ST;
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 diffuseColor = _Main_Color.rgb;
                float3 diffuse = directDiffuse * diffuseColor;
////// Emissive:
                float4 _shiny_1_var = tex2D(_shiny_1,TRANSFORM_TEX(i.uv0, _shiny_1));
                float4 _shiny_2_var = tex2D(_shiny_2,TRANSFORM_TEX(i.uv0, _shiny_2));
                float3 node_3624 = ((_shiny_1_var.rgb*_shiny_2_var.rgb)*_intensity);
                float4 _wave_1_var = tex2D(_wave_1,TRANSFORM_TEX(i.uv0, _wave_1));
                float4 _wave_2_var = tex2D(_wave_2,TRANSFORM_TEX(i.uv0, _wave_2));
                float3 emissive = (node_3624+((_wave_1_var.rgb*_wave_2_var.rgb)*_intensity2));
/// Final Color:
                float3 finalColor = diffuse + emissive;
                return fixed4(finalColor,(_Main_Color.a+node_3624.r));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
