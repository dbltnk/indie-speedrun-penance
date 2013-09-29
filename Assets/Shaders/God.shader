Shader "Custom/God" {
	Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _Noise ("Noise", 2D) = "white" {}
      _FadeAmount ("Fade Amount", Range(0,1)) = 0
      _Amount ("Amount", Range(0,1)) = 1
      _FadeColor ("Fade Color", Color) = (1,0,0,0)
    }
    SubShader {
      Tags { "RenderType" = "Transparent" }
      Cull Off
      CGPROGRAM
	  
	  #pragma surface surf Lambert finalcolor:mycolor
      
      struct Input {
          float2 uv_MainTex;
          float3 worldPos;
      };
      
      float _FadeAmount;
      float _Amount;
      
      float4 _FadeColor;
      
      sampler2D _MainTex;
      sampler2D _Noise;
      
      void surf (Input IN, inout SurfaceOutput o) {
			clip (_Amount - tex2D (_Noise, IN.uv_MainTex).r);
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * (1 - _FadeAmount) + 
				_FadeColor.rgb * _FadeAmount;
      }
      
      void mycolor (Input IN, SurfaceOutput o, inout fixed4 color)
      {
          //color.rgb = color.rgb * _Amount + _FadeColor.rgb * (1 - _Amount);
          //color.a = _Amount;
      }
      
      ENDCG
    } 
    
    Fallback "Diffuse"
}
