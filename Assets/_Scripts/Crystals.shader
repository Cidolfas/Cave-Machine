Shader "Cid/Crystals" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_Illum ("Illumin", Range(0,1)) = 1
	_Gloss ("Gloss", Range(0,1)) = 1
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 300
	
	CGPROGRAM
	#pragma surface surf BlinnPhong

	// Static properties
	half _Illum;
	half _Gloss;
	fixed4 _Color;
	half _Shininess;

	struct Input {
		float2 uv_main;
	};

	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 c = _Color;
		o.Albedo = c.rgb;
		o.Emission = c.rgb * _Illum;
		o.Gloss = _Gloss;
		o.Alpha = c.a;
		o.Specular = _Shininess;
	}
	ENDCG
}
FallBack "Self-Illumin/Diffuse"
}
