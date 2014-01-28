Shader "Cid/Noise" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Gloss ("Gloss", Range(0,1)) = 1
	_EmissionLM ("Emission (Lightmapper)", Float) = 0
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200
	
CGPROGRAM
#pragma surface surf Lambert

sampler2D _MainTex;
fixed4 _Color;
half _Gloss;

struct Input {
	float2 uv_MainTex;
	float2 uv_Illum;
};

void surf (Input IN, inout SurfaceOutput o) {
	float2 ofst = IN.uv_MainTex;
	ofst.x += sin(_Time.x * _Time.x * 403);
	ofst.y += sin(_Time.x * 1569863);
	fixed4 tex = tex2D(_MainTex, ofst);
	fixed4 c = tex * _Color;
	o.Albedo = c.rgb;
	o.Emission = c.rgb;
	o.Alpha = _Gloss;
}
ENDCG
} 
FallBack "Self-Illumin/VertexLit"
}
