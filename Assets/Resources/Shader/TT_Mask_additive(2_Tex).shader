// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.17 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.17;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:4795,x:33151,y:32625,varname:node_4795,prsc:2|emission-2473-OUT;n:type:ShaderForge.SFN_Tex2d,id:1533,x:32288,y:32604,ptovrint:False,ptlb:Wave_Tex1,ptin:_Wave_Tex1,varname:node_1533,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-8162-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:8363,x:32302,y:33086,ptovrint:False,ptlb:Wave_Tex2,ptin:_Wave_Tex2,varname:_Wave_Tex2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9040-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:8162,x:32068,y:32638,varname:node_8162,prsc:2,uv:0;n:type:ShaderForge.SFN_TexCoord,id:9040,x:32091,y:33108,varname:node_9040,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:5850,x:32582,y:32734,varname:node_5850,prsc:2|A-9073-OUT,B-2015-OUT;n:type:ShaderForge.SFN_Tex2d,id:2229,x:32668,y:32401,ptovrint:False,ptlb:Mask_Tex,ptin:_Mask_Tex,varname:node_2229,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:9084,x:32288,y:32385,ptovrint:False,ptlb:Tex1_Color,ptin:_Tex1_Color,varname:node_9084,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:9073,x:32497,y:32527,varname:node_9073,prsc:2|A-9084-RGB,B-1533-RGB;n:type:ShaderForge.SFN_Color,id:5891,x:32279,y:32889,ptovrint:False,ptlb:Tex2_Color,ptin:_Tex2_Color,varname:node_5891,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:2015,x:32500,y:32953,varname:node_2015,prsc:2|A-5891-RGB,B-8363-RGB;n:type:ShaderForge.SFN_Multiply,id:2473,x:32943,y:32561,varname:node_2473,prsc:2|A-2229-RGB,B-5850-OUT;proporder:9084-1533-5891-8363-2229;pass:END;sub:END;*/

Shader "TT/Mask_additive(2_Tex)" {
    Properties {
        _Tex1_Color ("Tex1_Color", Color) = (0.5,0.5,0.5,1)
        _Wave_Tex1 ("Wave_Tex1", 2D) = "white" {}
        _Tex2_Color ("Tex2_Color", Color) = (0.5,0.5,0.5,1)
        _Wave_Tex2 ("Wave_Tex2", 2D) = "white" {}
        _Mask_Tex ("Mask_Tex", 2D) = "white" {}
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
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _Wave_Tex1; uniform float4 _Wave_Tex1_ST;
            uniform sampler2D _Wave_Tex2; uniform float4 _Wave_Tex2_ST;
            uniform sampler2D _Mask_Tex; uniform float4 _Mask_Tex_ST;
            uniform float4 _Tex1_Color;
            uniform float4 _Tex2_Color;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
////// Emissive:
                float4 _Mask_Tex_var = tex2D(_Mask_Tex,TRANSFORM_TEX(i.uv0, _Mask_Tex));
                float4 _Wave_Tex1_var = tex2D(_Wave_Tex1,TRANSFORM_TEX(i.uv0, _Wave_Tex1));
                float4 _Wave_Tex2_var = tex2D(_Wave_Tex2,TRANSFORM_TEX(i.uv0, _Wave_Tex2));
                float3 emissive = (_Mask_Tex_var.rgb*((_Tex1_Color.rgb*_Wave_Tex1_var.rgb)+(_Tex2_Color.rgb*_Wave_Tex2_var.rgb)));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
