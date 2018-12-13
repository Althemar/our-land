Shader "Outline" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,0.5)
		_AlphaTex("Base (RGB) Trans (A)", 2D) = "white" {}
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
			sampler2D _AlphaTex;
			float4 _AlphaTex_ST;

			struct v2f {
				float4 pos : SV_POSITION;
				half4 color : COLOR0;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_full v) {
				v2f o;

				o.color = v.color;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _AlphaTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 texcol = tex2D(_AlphaTex, i.uv) * i.color;
				return texcol * _Color;
			}
			ENDCG
		}
	}
	FallBack "Unlit/Transparent Cutout"
}