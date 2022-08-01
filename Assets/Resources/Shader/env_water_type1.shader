// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.17 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.17;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:9804,x:33128,y:32733,varname:node_9804,prsc:2|emission-5641-OUT,alpha-1316-R;n:type:ShaderForge.SFN_Tex2d,id:8414,x:32192,y:32247,ptovrint:False,ptlb:Main_Tex,ptin:_Main_Tex,varname:node_8414,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:90,x:31962,y:32472,varname:node_90,prsc:2,uv:1;n:type:ShaderForge.SFN_Tex2d,id:3349,x:32218,y:32447,ptovrint:False,ptlb:Second_Tex(UV2),ptin:_Second_TexUV2,varname:node_3349,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-90-UVOUT;n:type:ShaderForge.SFN_Color,id:7475,x:32237,y:32074,ptovrint:False,ptlb:Main_Color,ptin:_Main_Color,varname:node_7475,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:3671,x:32802,y:32197,varname:node_3671,prsc:2|A-7475-RGB,B-8414-RGB;n:type:ShaderForge.SFN_Add,id:5641,x:32784,y:32529,varname:node_5641,prsc:2|A-3671-OUT,B-3349-RGB;n:type:ShaderForge.SFN_Tex2d,id:1316,x:32483,y:32967,ptovrint:False,ptlb:Mask_Tex,ptin:_Mask_Tex,varname:node_1316,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;proporder:7475-8414-1316-3349;pass:END;sub:END;*/

Shader "TT/env_water_type1" {
    Properties {
        _Main_Color ("Main_Color", Color) = (0.5,0.5,0.5,1)
        _Main_Tex ("Main_Tex", 2D) = "white" {}
        _Mask_Tex ("Mask_Tex", 2D) = "white" {}
        _Second_TexUV2 ("Second_Tex(UV2)", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
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
            Blend SrcAlpha OneMinusSrcAlpha
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
            uniform sampler2D _Main_Tex; uniform float4 _Main_Tex_ST;
            uniform sampler2D _Second_TexUV2; uniform float4 _Second_TexUV2_ST;
            uniform float4 _Main_Color;
            uniform sampler2D _Mask_Tex; uniform float4 _Mask_Tex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
////// Emissive:
                float4 _Main_Tex_var = tex2D(_Main_Tex,TRANSFORM_TEX(i.uv0, _Main_Tex));
                float4 _Second_TexUV2_var = tex2D(_Second_TexUV2,TRANSFORM_TEX(i.uv1, _Second_TexUV2));
                float3 emissive = ((_Main_Color.rgb*_Main_Tex_var.rgb)+_Second_TexUV2_var.rgb);
                float3 finalColor = emissive;
                float4 _Mask_Tex_var = tex2D(_Mask_Tex,TRANSFORM_TEX(i.uv0, _Mask_Tex));
                fixed4 finalRGBA = fixed4(finalColor,_Mask_Tex_var.r);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
