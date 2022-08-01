// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.17 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.17;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:2768,x:33624,y:32747,varname:node_2768,prsc:2|emission-7551-OUT,alpha-3232-A;n:type:ShaderForge.SFN_Tex2d,id:1917,x:32401,y:33143,ptovrint:False,ptlb:dist_texture,ptin:_dist_texture,varname:node_1917,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Append,id:1648,x:32660,y:33150,varname:node_1648,prsc:2|A-1917-R,B-1917-G;n:type:ShaderForge.SFN_Slider,id:9693,x:32262,y:33411,ptovrint:False,ptlb:distotion_power,ptin:_distotion_power,varname:node_9693,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3302612,max:0.5;n:type:ShaderForge.SFN_Tex2d,id:4093,x:33085,y:32849,ptovrint:False,ptlb:main_texture,ptin:_main_texture,varname:node_4093,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:66321cc856b03e245ac41ed8a53e0ecc,ntxv:0,isnm:False|UVIN-6631-OUT;n:type:ShaderForge.SFN_TexCoord,id:2772,x:32615,y:32848,varname:node_2772,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:9067,x:32888,y:33150,varname:node_9067,prsc:2|A-1648-OUT,B-6898-OUT;n:type:ShaderForge.SFN_Append,id:6898,x:32645,y:33388,varname:node_6898,prsc:2|A-9693-OUT,B-9693-OUT;n:type:ShaderForge.SFN_Add,id:6631,x:32894,y:32849,varname:node_6631,prsc:2|A-2772-UVOUT,B-9067-OUT;n:type:ShaderForge.SFN_Color,id:3232,x:33090,y:32674,ptovrint:False,ptlb:main_color,ptin:_main_color,varname:node_3232,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:7551,x:33385,y:32676,varname:node_7551,prsc:2|A-3232-RGB,B-4093-RGB;proporder:3232-4093-1917-9693;pass:END;sub:END;*/

Shader "TT/distotion" {
    Properties {
        _main_color ("main_color", Color) = (0.5,0.5,0.5,1)
        _main_texture ("main_texture", 2D) = "white" {}
        _dist_texture ("dist_texture", 2D) = "white" {}
        _distotion_power ("distotion_power", Range(0, 0.5)) = 0.3302612
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
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            uniform sampler2D _dist_texture; uniform float4 _dist_texture_ST;
            uniform float _distotion_power;
            uniform sampler2D _main_texture; uniform float4 _main_texture_ST;
            uniform float4 _main_color;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
////// Emissive:
                float4 _dist_texture_var = tex2D(_dist_texture,TRANSFORM_TEX(i.uv0, _dist_texture));
                float2 node_6631 = (i.uv0+(float2(_dist_texture_var.r,_dist_texture_var.g)*float2(_distotion_power,_distotion_power)));
                float4 _main_texture_var = tex2D(_main_texture,TRANSFORM_TEX(node_6631, _main_texture));
                float3 emissive = (_main_color.rgb*_main_texture_var.rgb);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,_main_color.a);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
