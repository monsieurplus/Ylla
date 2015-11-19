Shader "Custom/Move Sea" {
	SubShader{
		Pass{

		CGPROGRAM

#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		fixed3 color : COLOR0;
	};

	struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float4 texcoord : TEXCOORD0;
	};

	float Hash2d(fixed2 uv)
	{
		float f = uv.x + uv.y * 37.0;
		return frac(sin(f)*104003.9);
	}

	v2f vert(appdata v)
	{
		v2f o;
		
		
		o.pos = v.vertex;
		
		
		//o.pos += sin(_Time.z) * fixed4(0, 0, Hash2d(v.texcoord + fixed2(_Time.x, _Time.x)), 0);
		//o.pos += sin(_Time.z) * fixed4(0, 0, 1, 0);
		o.pos += sin(_Time.z) * fixed4(0, 0, Hash2d(v.texcoord + fixed2(_Time.x, _Time.x)) * 2.0, 0);
		
		
		
		o.pos = mul(UNITY_MATRIX_MVP, o.pos);
		
		//o.color = v.normal * 0.5 + 0.5;
		o.color = v.normal - 0.25;
		
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		return fixed4(i.color, 1);
	}
		ENDCG

	}
	}
}