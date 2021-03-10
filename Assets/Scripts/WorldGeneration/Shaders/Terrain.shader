Shader "Custom/Terrain"
{
	Properties
	{
		testTexture("Texture", 2D) = "white"{}
		testScale("Scale", Float) = 1
		_Color("Color", Color) = (1,1,1,1)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		[NoScaleOffset] _FlowMap("Flow (RG, A noise)", 2D) = "black" {}
		_UJump("U jump per phase", Range(-0.25, 0.25)) = 0.25
		_VJump("V jump per phase", Range(-0.25, 0.25)) = 0.25
		_Tiling("Tiling", Float) = 1
		_Speed("Speed", Float) = 1
		_FlowStrength("Flow Strength", Float) = 1
		_FlowOffset("Flow Offset", Float) = 0
	}
		SubShader
		{
			Tags
			{
				"RenderType" = "Opaque"
			}
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			const static int maxLayerCount = 8;
			const static float epsilon = 1E-4;

			int layerCount;
			float3 baseColors[maxLayerCount];
			float baseStartHeights[maxLayerCount];
			float baseBlends[maxLayerCount];
			float baseColorStrength[maxLayerCount];
			float baseTextureScales[maxLayerCount];

			float minHeight;
			float maxHeight;

			sampler2D testTexture;
			float testScale;

			UNITY_DECLARE_TEX2DARRAY(baseTextures);



			#include "Flow.cginc"
			sampler2D _MainTex, _FlowMap;
			float _UJump, _VJump, _Tiling, _Speed, _FlowStrength, _FlowOffset;

			struct Input
			{
				float3 worldPos;
				float3 worldNormal;
				float2 uv_MainTex;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

			float inverseLerp(float a, float b, float value)
			{
				return saturate((value - a) / (b - a));
			}

			float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex)
			{
				float3 scaledWorldPos = worldPos / scale;
				float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.y, scaledWorldPos.z, textureIndex)) * blendAxes.x;
				float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.z, textureIndex)) * blendAxes.y;
				float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.y, textureIndex)) * blendAxes.z;
				return xProjection + yProjection + zProjection;
			}

			float3 FlowUVW(float2 uv, float2 flowVector, float2 jump, float flowOffset, float tiling, float time, bool flowB)
			{
				float phaseOffset = flowB ? 0.5 : 0;
				float progress = frac(time + phaseOffset);
				float3 uvw;
				uvw.xy = uv - flowVector * (progress + flowOffset);
				uvw.xy *= tiling;
				uvw.xy += phaseOffset;
				uvw.xy += (time - progress) * jump;
				uvw.z = 1 - abs(1 - 2 * progress);
				return uvw;
			}

			void createWaterTexture(Input IN,inout SurfaceOutputStandard o)
			{
				float2 flowVector = tex2D(_FlowMap, IN.uv_MainTex).rg * 2 - 1;
				flowVector *= _FlowStrength;
				float noise = tex2D(_FlowMap, IN.uv_MainTex).a;
				float time = _Time.y * _Speed + noise;
				float2 jump = float2(_UJump, _VJump);

				float3 uvwA = FlowUVW(IN.uv_MainTex, flowVector, jump, _FlowOffset, _Tiling, time, false);
				float3 uvwB = FlowUVW(IN.uv_MainTex, flowVector, jump, _FlowOffset, _Tiling, time, true);

				fixed4 texA = tex2D(_MainTex, uvwA.xy) * uvwA.z;
				fixed4 texB = tex2D(_MainTex, uvwB.xy) * uvwB.z;

				fixed4 c = (texA + texB) * _Color;
				o.Albedo = c.rgb;
			}

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				float heightPercent = inverseLerp(minHeight,maxHeight, IN.worldPos.y);
				float3 blendAxes = abs(IN.worldNormal);
				blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

				for (int i = 0; i < layerCount; i++)
				{
					if (i == 0) {
						createWaterTexture(IN,o);
					}
					else {
						float drawStrength = inverseLerp(-baseBlends[i] / 2 - epsilon, baseBlends[i] / 2, heightPercent - baseStartHeights[i]);

						float3 baseColor = baseColors[i] * baseColorStrength[i];
						float3 textureColor = triplanar(IN.worldPos, baseTextureScales[i], blendAxes, i) * (1 - baseColorStrength[i]);

						o.Albedo = o.Albedo * (1 - drawStrength) + (baseColor + textureColor) * drawStrength;
					}

				}


			}


			ENDCG
		}
			FallBack "Diffuse"
}