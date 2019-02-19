Shader "Unlit/OutlineWalk"
{
	Properties{
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_GrainTex("Grain Texture", 2D) = "white" {}
		_CutOff("Cut Off", Float) = 0.2
	}

	SubShader{
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 wpos : TEXCOORD2;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _GrainTex;
			float4 _GrainTex_ST;
			float _CutOff;

			float rand(fixed2 co) {
				return frac(sin(dot(co.xy, fixed2(12.9898, 78.233))) * 43758.5453);
			}

			v2f vert(appdata_t v)
			{
				v.vertex.xy += (0.5 - rand(v.vertex.xy + (floor(_Time.z) * fixed2(0.5, 0.5)))) * 0.1;

				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.wpos = mul(UNITY_MATRIX_M, float4(v.vertex.xyz, 1.0));
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);

				UNITY_APPLY_FOG(i.fogCoord, col);
			
				fixed4 grain = tex2D(_GrainTex, TRANSFORM_TEX(i.wpos.xy, _GrainTex) + (floor(_Time.z) * fixed2(0.5, 0)));

				clip(grain.r - _CutOff);

				
				return col;
			}

			ENDCG
		}
	}
}
