// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/VisionCone"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0

        _Color("Color", Color) = (1,0,0,0.5)

        _VisionAngle("Vision Angle", Float) = 45
        _VisionRadius("Vision Radius", Float) = 3
    }

    SubShader
    {
        Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _VisionRadius;
            float _VisionAngle;
            static const float PI = 3.14159265f;

            float3 stretch(float3 vec, float x, float y)
            {
                float2x2 stretchMatrix = float2x2(x, 0, 0, y);
                return float3(mul(stretchMatrix, vec.xy), vec.z).xyz;
            };


            v2f vert(appdata v)
            {
                v2f o;

                float3 vertex = stretch(v.vertex, _VisionRadius, _VisionRadius);
                float4 stretched_vertex = UnityObjectToClipPos(vertex);
                o.vertex = stretched_vertex;

                // float2 uv = stretch(v.uv, _VisionRadius, _VisionRadius);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float radius = _VisionRadius;
                // if i.uv.x * i.uv.x + i.uv.y * i.uv.y > radius * radius 
                float x = 2 * (i.uv.x - 0.5);
                float y = 2 * (i.uv.y - 0.5);
                float ratio = (x * x + y * y) / radius * radius;
                float opacity = min(1, ratio);
                // if opacity < 1 => 0
                // ratio = ratio - 1;
                // float opacity = max(0, ratio);
                opacity = 1 - opacity;

                // vA dot vB = VA * 1 * cos(theta) // 1 since vB = unit vec
                float VA = x*x + y*y;
                float lhs = x* 1 + y * 0;
                float cos_t = VA / lhs;
                float t = acos(cos_t);

                if (t > _VisionAngle / 180 * PI) {
                    opacity = 0;
                }

                opacity = t;

                col = col * opacity;

                return col;
            }
            ENDCG
        }
    }
}
