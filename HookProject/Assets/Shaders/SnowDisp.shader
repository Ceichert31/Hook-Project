Shader "Custom/SnowDisp"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainColor] _WalkColor("Walk Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [MainTexture] _NoiseMap("Noise Map", 2D) = "white" {}
        _DisplacementAmount("Displacement Amount", Range(0,5)) = 0.5
        _BaseHeight("Base Snow Height", Range(0, 5)) = 2
        _NoiseResolution("Noise Resolution", Vector, 2) = (2, 2, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float footprintAmount : TEXCOORD1;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NoiseMap);
            SAMPLER(sampler_NoiseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _WalkColor;
                float4 _BaseMap_ST;

                float _DisplacementAmount;
                float _BaseHeight;

                float2 _NoiseResolution;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                //Set snow height
                OUT.positionHCS.y -= _BaseHeight;

                //Noise for snow height
                float4 noise = SAMPLE_TEXTURE2D_LOD(_NoiseMap, sampler_NoiseMap, IN.uv, 0.0);

                //Improve by changing along normal
                OUT.positionHCS.y += noise.y;

                //Apply displacement from render texture
                float4 renderTex = SAMPLE_TEXTURE2D_LOD(_BaseMap, sampler_BaseMap, IN.uv, 0.0);

                float oldPos = OUT.positionHCS.y;

                OUT.positionHCS.y += renderTex.x * _DisplacementAmount;
                OUT.footprintAmount = renderTex.x;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 noise = SAMPLE_TEXTURE2D(_NoiseMap, sampler_NoiseMap, _NoiseResolution * IN.uv);

                half4 snowColor = lerp(_BaseColor, _WalkColor, IN.footprintAmount);

                snowColor.rgb *= (0.8 + noise.r * 0.4);

                return snowColor;
            }
            ENDHLSL
        }
    }
}
