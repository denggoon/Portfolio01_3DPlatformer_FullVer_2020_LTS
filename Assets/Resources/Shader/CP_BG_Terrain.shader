  Shader "TT/Terrain2Blend" 
  {
  
  Properties
  {
  	 _Color ("Main Color", Color) = (1,1,1,1)
  	 _MainTex ("Base (RGB)", 2D) = "white" {}
  	 _DetailTex ("Detail (RGB)", 2D) = "white" {}
  	 _BlendTex ("Tint (RGB) Blend (A)", 2D) = "white" {}
  }
  
    SubShader 
    {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf T4M exclude_path:prepass noforwardadd

      inline fixed4 LightingT4M (SurfaceOutput s, fixed3 lightDir, fixed atten)
{
	fixed diff = dot (s.Normal, lightDir);
	fixed4 c;
	c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2);
	c.a = 0.0;
	return c;
}
      
      struct Input 
      {
          half2 uv_MainTex;
          half2 uv_DetailTex;
          half2 uv_BlendTex; 
      };
      
      sampler2D _MainTex;
      sampler2D _DetailTex;
      sampler2D _BlendTex;
      fixed4 _Color;
      void surf (Input IN, inout SurfaceOutput o) 
      {
	      fixed4 blendTintColor = tex2D (_BlendTex, IN.uv_BlendTex) * _Color;    
       	  o.Albedo = blendTintColor.rgb * (((1-blendTintColor.a) * tex2D (_MainTex, IN.uv_MainTex).rgb) + (blendTintColor.a * tex2D (_DetailTex, IN.uv_DetailTex).rgb));
      }
      ENDCG
    }
    
    
    Fallback "Mobile/VertexLit"
  }