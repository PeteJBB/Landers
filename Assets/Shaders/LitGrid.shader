Shader "Custom/LitGrid" {
	Properties {
		_GridAlphaTex ("Grid Alpha Texture", 2D) = "white" {}
		_MainColor ("Main Color", Color) = (0.2, 0.2, 0.2, 1)
		_GridColor ("Grid Color", Color) = (0, 1, 0, 1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _GridAlphaTex;
		fixed4 _MainColor;
		fixed4 _GridColor;

		struct Input {
			float2 uv_GridAlphaTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_GridAlphaTex, IN.uv_GridAlphaTex);
			float val = c.r + c.g + c.b;
			o.Albedo = ((1 - val) * _MainColor) + (val * _GridColor);
			o.Emission = c.r * _GridColor;
			o.Alpha = 1;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
