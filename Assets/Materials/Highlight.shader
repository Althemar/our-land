Shader "Spine/Skeleton Highlight" {
	Properties {
		_Cutoff ("Shadow alpha cutoff", Range(0,1)) = 0.1
		[NoScaleOffset] _MainTex ("Main Texture", 2D) = "black" {}
		[Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }

		Fog { Mode Off }
		Cull Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		Lighting Off

		Pass {
			CGPROGRAM
			#pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float _Intensity;

			struct VertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};


			float rand(fixed2 co) {
				return frac(sin(dot(co.xy, fixed2(12.9898, 78.233))) * 43758.5453);
			}

			VertexOutput vert (VertexInput v) {
				VertexOutput o;

				v.vertex += float4((0.5 - rand(v.uv.y)) * _Intensity * _SinTime.z, (0.5 - rand(v.uv.x)) * _Intensity * _CosTime.z, (0.5 - rand(v.uv)) * _Intensity * _SinTime.x, 0) * sin(_Time.w) * 50;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.vertexColor = v.vertexColor;
				return o;
			}

			float4 frag (VertexOutput i) : COLOR {
				float4 texColor = tex2D(_MainTex, i.uv);

				#if defined(_STRAIGHT_ALPHA_INPUT)
				texColor.rgb *= texColor.a;
				#endif
				float4 col = (texColor * i.vertexColor);
				col = lerp(col, float4(rand(_Time.z), rand(_Time.y), rand(_Time.x), 1), _Intensity * 1.2f);
				return col;
			}
			ENDCG
		}

		Pass {
			Name "Caster"
			Tags { "LightMode"="ShadowCaster" }
			Offset 1, 1
			ZWrite On
			ZTest LEqual

			Fog { Mode Off }
			Cull Off
			Lighting Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			fixed _Cutoff;

			struct VertexOutput { 
				V2F_SHADOW_CASTER;
				float2 uv : TEXCOORD1;
			};

			VertexOutput vert (appdata_base v) {
				VertexOutput o;
				o.uv = v.texcoord;
				TRANSFER_SHADOW_CASTER(o)
				return o;
			}

			float4 frag (VertexOutput i) : COLOR {
				fixed4 texcol = tex2D(_MainTex, i.uv);
				clip(texcol.a - _Cutoff);
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
}
