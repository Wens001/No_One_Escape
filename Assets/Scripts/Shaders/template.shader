Shader "Universal Render Pipeline/Custom/Physically Based Example"
{
    Properties
    {
        [MainColor] _BaseColor("Color", Color) = (0.5,0.5,0.5,1)
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
		_Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_Range("_Range",Range(0,1)) = .1
		_OtherColor("_OtherColor",Color) = (1,1,1,1)
		_NoiseMap("_NoiseMap", 2D) = "Black" {}
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" "IgnoreProjector" = "True"}

        Pass
        {
            Tags{"LightMode" = "UniversalForward"}

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
            Cull Back

            HLSLPROGRAM

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 uv           : TEXCOORD0;
                float2 uvLM         : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv                       : TEXCOORD0;
                float2 uvLM                     : TEXCOORD1;
                float4 positionWSAndFogFactor   : TEXCOORD2; // xyz: positionWS, w: vertex fog factor
                half3  normalWS                 : TEXCOORD3;
				float4 otherColor               : TEXCOORD4;
                float4 shadowCoord              : TEXCOORD6; // compute shadow coord per-vertex for the main light
                float4 positionCS               : SV_POSITION;
            };

			float _Range;
			float4 _NoiseMap_ST;
			sampler2D _NoiseMap;
			float3 forwaddDir;
			half4 _OtherColor;

            Varyings LitPassVertex(Attributes input)
            {
                Varyings output;

				float3 wPos = TransformObjectToWorld(input.positionOS.xyz);
				float3 lightWDir = forwaddDir;
				float3 wNormal = TransformObjectToWorldNormal(input.normalOS);

				half2 uv = TRANSFORM_TEX(input.uv, _NoiseMap);
				output.otherColor = 1;
				if (dot(lightWDir, wNormal) < -.2) {
					lightWDir.y = 0;
					wPos -= normalize(lightWDir) * _Range * tex2Dlod(_NoiseMap,half4(uv,0,0));
					output.otherColor = _OtherColor;
				}

                VertexPositionInputs vertexInput = GetVertexPositionInputs(TransformWorldToObject(wPos));
                

                // Computes fog factor per-vertex.
                float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.uvLM = input.uvLM.xy * unity_LightmapST.xy + unity_LightmapST.zw;

                output.positionWSAndFogFactor = float4(vertexInput.positionWS, fogFactor);
                output.normalWS = wNormal;

                output.shadowCoord = GetShadowCoord(vertexInput);
                output.positionCS = vertexInput.positionCS;
                return output;
            }

            half4 LitPassFragment(Varyings input) : SV_Target
            {
                SurfaceData surfaceData;
                InitializeStandardLitSurfaceData(input.uv, surfaceData);

                half3 normalWS = input.normalWS;

                normalWS = normalize(normalWS);

                half3 bakedGI = SampleSH(normalWS);

                float3 positionWS = input.positionWSAndFogFactor.xyz;
                half3 viewDirectionWS = SafeNormalize(GetCameraPositionWS() - positionWS);

                BRDFData brdfData;
                InitializeBRDFData(surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.alpha, brdfData);

                Light mainLight = GetMainLight(input.shadowCoord);

                half3 color = GlobalIllumination(brdfData, bakedGI, surfaceData.occlusion, normalWS, viewDirectionWS);

                color += LightingPhysicallyBased(brdfData, mainLight, normalWS, viewDirectionWS);

//#ifdef _ADDITIONAL_LIGHTS
//                int additionalLightsCount = GetAdditionalLightsCount();
//                for (int i = 0; i < additionalLightsCount; ++i)
//                {
//                    Light light = GetAdditionalLight(i, positionWS);
//                    // Same functions used to shade the main light.
//                    color += LightingPhysicallyBased(brdfData, light, normalWS, viewDirectionWS);
//                }
//#endif
                color += surfaceData.emission;
				//color *= input.otherColor;

                float fogFactor = input.positionWSAndFogFactor.w;
                color = MixFog(color, fogFactor);
                return half4(color, surfaceData.alpha);
            }
            ENDHLSL
        }
        // Used for rendering shadowmaps
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
        UsePass "Universal Render Pipeline/Lit/Meta"
    }
}