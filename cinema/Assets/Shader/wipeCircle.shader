

Shader "Custom/wipeCircle" {
    Properties {
        _Radius("Radius", Range(0, 2)) = 2
        _MainTex("MainTex", 2D) = "" {}
    }
    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _Radius;
            sampler2D _MainTex;

            fixed4 frag(v2f_img i) : COLOR {
                fixed4 c = tex2D(_MainTex, i.uv);
                i.uv -= fixed2(0.5f, 0.5f);
                float4 projectionSpaceUpperRight = float4(1, 1, UNITY_NEAR_CLIP_VALUE, _ProjectionParams.y);
                float4 viewSpaceUpperRight = mul(unity_CameraInvProjection, projectionSpaceUpperRight);
                i.uv.x *= viewSpaceUpperRight.x / viewSpaceUpperRight.y;
                if(distance(i.uv, fixed2(0, 0)) < _Radius) {
                    return c;
                }
                return fixed4(0, 0, 0, 1);
            }
            ENDCG
        }
    }
}