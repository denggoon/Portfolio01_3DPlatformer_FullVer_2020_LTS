// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.17 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.17;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:False,mssp:False,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:9361,x:35008,y:33276,varname:node_9361,prsc:2|emission-5171-OUT;n:type:ShaderForge.SFN_Tex2d,id:9540,x:33171,y:33041,ptovrint:False,ptlb:magma_base,ptin:_magma_base,varname:node_9540,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:99d74fdc3c7fe8844ba2b79c19a3b2a4,ntxv:0,isnm:False|UVIN-9472-OUT;n:type:ShaderForge.SFN_Tex2d,id:3792,x:32169,y:33379,ptovrint:False,ptlb:dist_map,ptin:_dist_map,varname:node_3792,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ae5b997ef9566374e932193b5d52a32a,ntxv:0,isnm:True;n:type:ShaderForge.SFN_Append,id:761,x:32408,y:33407,varname:node_761,prsc:2|A-3792-R,B-3792-G;n:type:ShaderForge.SFN_Slider,id:8667,x:32284,y:33701,ptovrint:False,ptlb:DIST_Power,ptin:_DIST_Power,varname:node_8667,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2820508,max:1;n:type:ShaderForge.SFN_TexCoord,id:25,x:32415,y:33097,varname:node_25,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:9472,x:32976,y:33422,varname:node_9472,prsc:2|A-25-UVOUT,B-3988-OUT;n:type:ShaderForge.SFN_Multiply,id:3988,x:32742,y:33564,varname:node_3988,prsc:2|A-761-OUT,B-8667-OUT;n:type:ShaderForge.SFN_Color,id:8934,x:33921,y:32657,ptovrint:False,ptlb:Main_Color,ptin:_Main_Color,varname:node_8934,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:5171,x:34312,y:33123,varname:node_5171,prsc:2|A-8934-RGB,B-1347-OUT;n:type:ShaderForge.SFN_Tex2d,id:7241,x:33488,y:33431,ptovrint:False,ptlb:magama_glow,ptin:_magama_glow,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-3678-UVOUT;n:type:ShaderForge.SFN_Multiply,id:2869,x:33681,y:33431,varname:node_2869,prsc:2|A-7241-RGB,B-4540-OUT;n:type:ShaderForge.SFN_Slider,id:4540,x:33285,y:33673,ptovrint:False,ptlb:magma_glow,ptin:_magma_glow,varname:node_4540,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_TexCoord,id:3678,x:33190,y:33272,varname:node_3678,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:1347,x:33991,y:33268,varname:node_1347,prsc:2|A-9540-RGB,B-2869-OUT;proporder:8934-9540-7241-4540-3792-8667;pass:END;sub:END;*/

Shader "TT/magma" {
    Properties {
        _Main_Color ("Main_Color", Color) = (0.5,0.5,0.5,1)
        _magma_base ("magma_base", 2D) = "white" {}
        _magama_glow ("magama_glow", 2D) = "white" {}
        _magma_glow ("magma_glow", Range(0, 1)) = 0
        _dist_map ("dist_map", 2D) = "white" {}
        _DIST_Power ("DIST_Power", Range(0, 1)) = 0.2820508
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
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _magma_base; uniform float4 _magma_base_ST;
            uniform sampler2D _dist_map; uniform float4 _dist_map_ST;
            uniform float _DIST_Power;
            uniform float4 _Main_Color;
            uniform sampler2D _magama_glow; uniform float4 _magama_glow_ST;
            uniform float _magma_glow;
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
                float3 _dist_map_var = UnpackNormal(tex2D(_dist_map,TRANSFORM_TEX(i.uv0, _dist_map)));
                float2 node_3988 = (float2(_dist_map_var.r,_dist_map_var.g)*_DIST_Power);
                float2 node_9472 = (i.uv0+node_3988);
                float4 _magma_base_var = tex2D(_magma_base,TRANSFORM_TEX(node_9472, _magma_base));
                float4 _magama_glow_var = tex2D(_magama_glow,TRANSFORM_TEX(i.uv0, _magama_glow));
                float3 emissive = (_Main_Color.rgb*(_magma_base_var.rgb+(_magama_glow_var.rgb*_magma_glow)));
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
