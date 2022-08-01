// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.17 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.17;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:9361,x:34448,y:32744,varname:node_9361,prsc:2|emission-3533-OUT,custl-5085-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:8068,x:33397,y:33478,varname:node_8068,prsc:2;n:type:ShaderForge.SFN_LightColor,id:3406,x:33397,y:33346,varname:node_3406,prsc:2;n:type:ShaderForge.SFN_LightVector,id:6869,x:31858,y:32654,varname:node_6869,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:9684,x:31858,y:32782,prsc:2,pt:True;n:type:ShaderForge.SFN_HalfVector,id:9471,x:31858,y:32932,varname:node_9471,prsc:2;n:type:ShaderForge.SFN_Dot,id:7782,x:32070,y:32697,cmnt:Lambert,varname:node_7782,prsc:2,dt:1|A-6869-OUT,B-9684-OUT;n:type:ShaderForge.SFN_Dot,id:3269,x:32070,y:32871,cmnt:Blinn-Phong,varname:node_3269,prsc:2,dt:1|A-9684-OUT,B-9471-OUT;n:type:ShaderForge.SFN_Multiply,id:2746,x:32632,y:32869,cmnt:Specular Contribution,varname:node_2746,prsc:2|A-7782-OUT,B-5267-OUT,C-4726-OUT;n:type:ShaderForge.SFN_Tex2d,id:851,x:32154,y:32186,ptovrint:False,ptlb:Main_Tex,ptin:_Main_Tex,varname:node_851,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1941,x:32632,y:32698,cmnt:Diffuse Contribution,varname:node_1941,prsc:2|A-544-OUT,B-7782-OUT;n:type:ShaderForge.SFN_Color,id:5927,x:32154,y:32385,ptovrint:False,ptlb:Main_Color,ptin:_Main_Color,varname:node_5927,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Exp,id:1700,x:32070,y:33054,varname:node_1700,prsc:2,et:1|IN-9978-OUT;n:type:ShaderForge.SFN_Slider,id:5328,x:31529,y:33056,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:node_5328,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Power,id:5267,x:32268,y:32940,varname:node_5267,prsc:2|VAL-3269-OUT,EXP-1700-OUT;n:type:ShaderForge.SFN_Add,id:2159,x:33397,y:33200,cmnt:Combine,varname:node_2159,prsc:2|A-1941-OUT,B-2746-OUT;n:type:ShaderForge.SFN_Multiply,id:5085,x:33674,y:33231,cmnt:Attenuate and Color,varname:node_5085,prsc:2|A-2159-OUT,B-3406-RGB,C-8068-OUT;n:type:ShaderForge.SFN_ConstantLerp,id:9978,x:31858,y:33056,varname:node_9978,prsc:2,a:1,b:11|IN-5328-OUT;n:type:ShaderForge.SFN_AmbientLight,id:7528,x:33175,y:32763,varname:node_7528,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2460,x:33342,y:32739,cmnt:Ambient Light,varname:node_2460,prsc:2|A-544-OUT,B-7528-RGB;n:type:ShaderForge.SFN_Multiply,id:544,x:32434,y:32382,cmnt:Diffuse Color,varname:node_544,prsc:2|A-851-RGB,B-5927-RGB;n:type:ShaderForge.SFN_Cubemap,id:2133,x:33024,y:32129,ptovrint:False,ptlb:Cube_Tex,ptin:_Cube_Tex,varname:node_2133,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,pvfc:0|DIR-7751-OUT;n:type:ShaderForge.SFN_Slider,id:607,x:33082,y:32515,ptovrint:False,ptlb:Cube_Power,ptin:_Cube_Power,varname:node_607,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:5,max:5;n:type:ShaderForge.SFN_Multiply,id:857,x:33263,y:32286,varname:node_857,prsc:2|A-2133-RGB,B-2599-OUT;n:type:ShaderForge.SFN_ViewReflectionVector,id:7751,x:32814,y:32005,varname:node_7751,prsc:2;n:type:ShaderForge.SFN_Add,id:3533,x:33618,y:32697,varname:node_3533,prsc:2|A-2702-OUT,B-2460-OUT,C-1289-OUT;n:type:ShaderForge.SFN_Power,id:2702,x:33433,y:32496,varname:node_2702,prsc:2|VAL-857-OUT,EXP-607-OUT;n:type:ShaderForge.SFN_Slider,id:2599,x:32895,y:32304,ptovrint:False,ptlb:Cube_intensity,ptin:_Cube_intensity,varname:node_2599,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:4726,x:32255,y:33188,ptovrint:False,ptlb:Spec_Intensity,ptin:_Spec_Intensity,varname:node_4726,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.787388,max:1;n:type:ShaderForge.SFN_Fresnel,id:2740,x:33710,y:32870,varname:node_2740,prsc:2|EXP-924-OUT;n:type:ShaderForge.SFN_Slider,id:924,x:33227,y:32958,ptovrint:False,ptlb:Rim_Power,ptin:_Rim_Power,varname:node_924,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Color,id:7482,x:33710,y:33022,ptovrint:False,ptlb:Rim_Color,ptin:_Rim_Color,varname:node_7482,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Multiply,id:1289,x:33926,y:32940,varname:node_1289,prsc:2|A-2740-OUT,B-7482-RGB;proporder:5927-851-4726-5328-2133-607-2599-7482-924;pass:END;sub:END;*/

Shader "TT/CubeGloss" {
    Properties {
        _Main_Color ("Main_Color", Color) = (0.5,0.5,0.5,1)
        _Main_Tex ("Main_Tex", 2D) = "white" {}
        _Spec_Intensity ("Spec_Intensity", Range(0, 1)) = 0.787388
        _Gloss ("Gloss", Range(0, 1)) = 0.5
        _Cube_Tex ("Cube_Tex", Cube) = "_Skybox" {}
        _Cube_Power ("Cube_Power", Range(0, 5)) = 5
        _Cube_intensity ("Cube_intensity", Range(0, 1)) = 0
        _Rim_Color ("Rim_Color", Color) = (0,0,0,1)
        _Rim_Power ("Rim_Power", Range(0, 5)) = 0
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
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _Main_Tex; uniform float4 _Main_Tex_ST;
            uniform float4 _Main_Color;
            uniform float _Gloss;
            uniform samplerCUBE _Cube_Tex;
            uniform float _Cube_Power;
            uniform float _Cube_intensity;
            uniform float _Spec_Intensity;
            uniform float _Rim_Power;
            uniform float4 _Rim_Color;
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
                o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
////// Emissive:
                float4 _Main_Tex_var = tex2D(_Main_Tex,TRANSFORM_TEX(i.uv0, _Main_Tex));
                float3 node_544 = (_Main_Tex_var.rgb*_Main_Color.rgb); // Diffuse Color
                float3 emissive = (pow((texCUBE(_Cube_Tex,viewReflectDirection).rgb*_Cube_intensity),_Cube_Power)+(node_544*UNITY_LIGHTMODEL_AMBIENT.rgb)+(pow(1.0-max(0,dot(normalDirection, viewDirection)),_Rim_Power)*_Rim_Color.rgb));
                float node_7782 = max(0,dot(lightDirection,normalDirection)); // Lambert
                float3 finalColor = emissive + (((node_544*node_7782)+(node_7782*pow(max(0,dot(normalDirection,halfDirection)),exp2(lerp(1,11,_Gloss)))*_Spec_Intensity))*_LightColor0.rgb*attenuation);
                fixed4 finalRGBA = fixed4(finalColor,1);
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
            #include "Lighting.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _Main_Tex; uniform float4 _Main_Tex_ST;
            uniform float4 _Main_Color;
            uniform float _Gloss;
            uniform samplerCUBE _Cube_Tex;
            uniform float _Cube_Power;
            uniform float _Cube_intensity;
            uniform float _Spec_Intensity;
            uniform float _Rim_Power;
            uniform float4 _Rim_Color;
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float4 _Main_Tex_var = tex2D(_Main_Tex,TRANSFORM_TEX(i.uv0, _Main_Tex));
                float3 node_544 = (_Main_Tex_var.rgb*_Main_Color.rgb); // Diffuse Color
                float node_7782 = max(0,dot(lightDirection,normalDirection)); // Lambert
                float3 finalColor = (((node_544*node_7782)+(node_7782*pow(max(0,dot(normalDirection,halfDirection)),exp2(lerp(1,11,_Gloss)))*_Spec_Intensity))*_LightColor0.rgb*attenuation);
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
