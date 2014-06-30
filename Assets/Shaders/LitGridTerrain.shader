Shader "Custom/LitGridTerrain" {
Properties {
	_Color0 ("Color0", Color) = (0, 1, 0, 1)
	_Color1 ("Color1", Color) = (1, 0, 0, 1)
	_Color2 ("Color2", Color) = (0, 0, 1, 1)
	_Color3 ("Color3", Color) = (1, 1, 1, 1)

	_Background0 ("Background0", Color) = (0.3, 0.3, 0.3, 1)
	_Background1 ("Background1", Color) = (0.3, 0.3, 0.3, 1)
	_Background2 ("Background2", Color) = (0.3, 0.3, 0.3, 1)
	_Background3 ("Background3", Color) = (0.3, 0.3, 0.3, 1)
	
	// set by terrain engine
	[HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
	[HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
	[HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
	[HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
	[HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
	[HideInInspector] _Normal3 ("Normal 3 (A)", 2D) = "bump" {}
	[HideInInspector] _Normal2 ("Normal 2 (B)", 2D) = "bump" {}
	[HideInInspector] _Normal1 ("Normal 1 (G)", 2D) = "bump" {}
	[HideInInspector] _Normal0 ("Normal 0 (R)", 2D) = "bump" {}
	// used in fallback on old cards & base map
	[HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
	[HideInInspector] _Color ("Main Color", Color) = (1,1,1,1)
}
	
SubShader {
	Tags {
		"SplatCount" = "4"
		"Queue" = "Geometry-100"
		"RenderType" = "Opaque"
	}
CGPROGRAM
#pragma surface surf BlinnPhong vertex:vert
#pragma target 3.0

void vert (inout appdata_full v)
{
	v.tangent.xyz = cross(v.normal, float3(0,0,1));
	v.tangent.w = -1;
}

struct Input {
	float2 uv_Control : TEXCOORD0;
	float2 uv_Splat0 : TEXCOORD1;
	float2 uv_Splat1 : TEXCOORD2;
	float2 uv_Splat2 : TEXCOORD3;
	float2 uv_Splat3 : TEXCOORD4;
};

sampler2D _Control;
sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
sampler2D _Normal0,_Normal1,_Normal2,_Normal3;
fixed4 _Color0, _Color1, _Color2, _Color3;
fixed4 _Background0, _Background1, _Background2, _Background3;

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 splat_control = tex2D (_Control, IN.uv_Control);
	
	// get grid alpha values
	fixed4 a0 = tex2D (_Splat0, IN.uv_Splat0).a;
	fixed4 a1 = tex2D (_Splat1, IN.uv_Splat1).a;
	fixed4 a2 = tex2D (_Splat2, IN.uv_Splat2).a;
	fixed4 a3 = tex2D (_Splat3, IN.uv_Splat3).a;

	fixed4 emis;
	emis  = splat_control.r * (a0 * _Color0);
	emis += splat_control.g * (a1 * _Color1);
	emis += splat_control.b * (a2 * _Color2);
	emis += splat_control.a * (a3 * _Color3);
	
	fixed4 col;
	col  = splat_control.r * ((1 - a0) * _Background0);
	col += splat_control.g * ((1 - a1) * _Background1);
	col += splat_control.b * ((1 - a2) * _Background2);
	col += splat_control.a * ((1 - a3) * _Background3);

	o.Albedo = col + emis;
	o.Emission = emis;

	o.Alpha = 0.0;
}
ENDCG  
}

Dependency "AddPassShader" = "Hidden/Nature/Terrain/Bumped Specular AddPass"
Dependency "BaseMapShader" = "Specular"

Fallback "Nature/Terrain/Diffuse"
}
