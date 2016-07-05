Shader "Custom/FogOfWar" {
	Properties {
		_FogMapImage ("FogMapImage", 2D) = "black" {}
		_Transparency ("Transparency", Float) = 1.0
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Cull Off 
		ZTest Always
		ZWrite Off

		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _FogMapImage;
		uniform float _Transparency;

		struct Input {
			float2 uv_FogMapImage;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_FogMapImage, IN.uv_FogMapImage);

			o.Albedo = c.rgb;
			if (c.r > 0 || c.g > 0 || c.b > 0)
				o.Alpha = 0;
			else o.Alpha = _Transparency;			
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
