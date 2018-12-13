Shader "Outline" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,0.5)
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

			struct v2f {
				float4 pos : SV_POSITION;
				half4 color : COLOR0;
			};

			v2f vert(appdata_full v) {
				v2f o;

				o.color = v.color;

				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				return i.color * _Color;
			}
			ENDCG
		}
	}
	FallBack "Unlit/Transparent Cutout"
}