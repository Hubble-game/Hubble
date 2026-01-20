Shader "Custom/CoolWater_VR"
{
    Properties
    {
        [Header(Colors)]
        _ShallowColor("Shallow Color", Color) = (0.4, 0.9, 0.8, 0.5)
        _DeepColor("Deep Color", Color) = (0.0, 0.2, 0.4, 1.0)
        _DepthRange("Depth Range", Range(0.1, 10.0)) = 2.0

        [Header(Normals and Waves)]
        _BumpMap("Normal Map", 2D) = "bump" {}
        _NormalScale("Normal Scale", Range(0, 1)) = 0.5
        _WaveSpeed("Wave Speed", Vector) = (0.1, 0.1, -0.1, -0.1)
        _Tiling("Tiling", Float) = 1.0

        [Header(Foam)]
        _FoamColor("Foam Color", Color) = (1, 1, 1, 1)
        _FoamRange("Foam Range", Range(0.01, 1.0)) = 0.2
        
        _Smoothness("Smoothness", Range(0, 1)) = 0.9
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float3 normalWS : TEXCOORD3;
                float3 positionWS : TEXCOORD4;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _ShallowColor;
                float4 _DeepColor;
                float _DepthRange;
                float4 _WaveSpeed;
                float _Tiling;
                float4 _FoamColor;
                float _FoamRange;
                float _NormalScale;
                float _Smoothness;
            CBUFFER_END

            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(output.positionWS);
                output.uv = input.uv * _Tiling;
                
                // Calcul de la position écran pour la profondeur
                output.screenPos = ComputeScreenPos(output.positionCS);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // --- 1. GESTION DE LA PROFONDEUR (VR Compatible) ---
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                
                // Correction UV pour la VR (Single Pass Instanced)
                float2 uvVR = UnityStereoTransformScreenSpaceTex(screenUV);
                
                float rawDepth = SampleSceneDepth(uvVR);
                float sceneZ = LinearEyeDepth(rawDepth, _ZBufferParams);
                float surfaceZ = input.screenPos.w;
                float waterDepth = sceneZ - surfaceZ;

                // --- 2. COULEUR DE L'EAU ---
                float depthFactor = saturate(waterDepth / _DepthRange);
                half4 waterColor = lerp(_ShallowColor, _DeepColor, depthFactor);

                // --- 3. NORMALES ET VAGUES (Animation) ---
                float2 uv1 = input.uv + _Time.y * _WaveSpeed.xy;
                float2 uv2 = input.uv + _Time.y * _WaveSpeed.zw;
                
                half3 n1 = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv1));
                half3 n2 = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv2));
                half3 normalWS = normalize(n1 + n2);
                normalWS = lerp(input.normalWS, normalWS, _NormalScale);

                // --- 4. ÉCUME (FOAM) ---
                float foamFactor = saturate(waterDepth / _FoamRange);
                float foamLine = 1.0 - foamFactor;
                waterColor = lerp(waterColor, _FoamColor, foamLine * _FoamColor.a);

                // --- 5. ÉCLAIRAGE SIMPLE ---
                Light mainLight = GetMainLight();
                half NdotL = saturate(dot(normalWS, mainLight.direction));
                waterColor.rgb *= (NdotL * 0.5 + 0.5); // Diffuse simple
                
                return waterColor;
            }
            ENDHLSL
        }
    }
}