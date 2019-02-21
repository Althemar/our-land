Shader "Outline" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,0.5)
		_GrainTex("Grain Texture", 2D) = "white" {}
		_Speed("Speed", Float) = 1
	}
	SubShader {
		Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Stencil {
			Ref 2
			Comp NotEqual
			Pass Replace
		}

		Pass {

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			fixed4 _Color;

			sampler2D _GrainTex;
			float4 _GrainTex_ST;
			float2 _MousePosition;
			float _Speed;

			struct v2f {
				float4 pos : SV_POSITION;
				half4 color : COLOR0;
				float4 wpos : TEXCOORD0;
			};

			float rand(fixed2 co) {
				return frac(sin(dot(co.xy, fixed2(12.9898, 78.233))) * 43758.5453);
			}

			v2f vert(appdata_full v) {
				v2f o;

				o.color = v.color;

				v.vertex.xy += (0.5 - rand(v.vertex.xy + _Speed * (floor(_Time.z) * fixed2(0.5, 0.5)))) * 0.1;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.wpos = mul(UNITY_MATRIX_M, float4(v.vertex.xyz, 1.0));
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				fixed4 col = tex2D(_GrainTex, i.wpos.xy + _Speed * (floor(_Time.z) * fixed2(0.5, 0)));

				clip(col.r - 0.2);

				return i.color * _Color;
			}

			ENDCG
		}
	}
	FallBack "Unlit/Transparent Cutout"
}