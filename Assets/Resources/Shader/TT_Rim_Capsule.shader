// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.17 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.17;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:True,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:6,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:9361,x:35469,y:32233,varname:node_9361,prsc:2|emission-2563-OUT;n:type:ShaderForge.SFN_Fresnel,id:1604,x:33910,y:32073,varname:node_1604,prsc:2|NRM-6948-OUT,EXP-9827-OUT;n:type:ShaderForge.SFN_Slider,id:9288,x:33970,y:32285,ptovrint:False,ptlb:Rim_Power,ptin:_Rim_Power,varname:node_9288,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:20,max:20;n:type:ShaderForge.SFN_Multiply,id:6499,x:34447,y:32234,varname:node_6499,prsc:2|A-7122-OUT,B-9288-OUT;n:type:ShaderForge.SFN_NormalVector,id:6948,x:33567,y:31893,prsc:2,pt:False;n:type:ShaderForge.SFN_Color,id:4455,x:34154,y:31916,ptovrint:False,ptlb:Rim_Color,ptin:_Rim_Color,varname:node_4455,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.8382353,c2:0.9129817,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:7122,x:34352,y:32025,varname:node_7122,prsc:2|A-4455-RGB,B-1604-OUT;n:type:ShaderForge.SFN_Slider,id:9827,x:33628,y:32272,ptovrint:False,ptlb:Rim_Intensity,ptin:_Rim_Intensity,varname:node_9827,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:4.059022,max:5;n:type:ShaderForge.SFN_ConstantLerp,id:8681,x:34696,y:32160,varname:node_8681,prsc:2,a:0,b:0.5|IN-6499-OUT;n:type:ShaderForge.SFN_Tex2d,id:3259,x:34624,y:32374,ptovrint:False,ptlb:Main_tex,ptin:_Main_tex,varname:node_3259,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:2563,x:34858,y:32360,varname:node_2563,prsc:2|A-8681-OUT,B-3259-RGB,C-5000-RGB;n:type:ShaderForge.SFN_Tex2d,id:5000,x:34942,y:32696,ptovrint:False,ptlb:Scroll_tex,ptin:_Scroll_tex,varname:node_5000,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2482-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:2482,x:34775,y:32744,varname:node_2482,prsc:2,uv:1;proporder:3259-5000-4455-9288-9827;pass:END;sub:END;*/

Shader "TT/Rim_Capsule" {
    Properties {
        _Main_tex ("Main_tex", 2D) = "white" {}
        _Scroll_tex ("Scroll_tex", 2D) = "white" {}
        _Rim_Color ("Rim_Color", Color) = (0.8382353,0.9129817,1,1)
        _Rim_Power ("Rim_Power", Range(0, 20)) = 20
        _Rim_Intensity ("Rim_Intensity", Range(0, 5)) = 4.059022
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcColor
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float _Rim_Power;
            uniform float4 _Rim_Color;
            uniform float _Rim_Intensity;
            uniform sampler2D _Main_tex; uniform float4 _Main_tex_ST;
            uniform sampler2D _Scroll_tex; uniform float4 _Scroll_tex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 _Main_tex_var = tex2D(_Main_tex,TRANSFORM_TEX(i.uv0, _Main_tex));
                float4 _Scroll_tex_var = tex2D(_Scroll_tex,TRANSFORM_TEX(i.uv1, _Scroll_tex));
                float3 emissive = (lerp(0,0.5,((_Rim_Color.rgb*pow(1.0-max(0,dot(i.normalDir, viewDirection)),_Rim_Intensity))*_Rim_Power))+_Main_tex_var.rgb+_Scroll_tex_var.rgb);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
