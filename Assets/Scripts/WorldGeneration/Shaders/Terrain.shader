Shader "Custom/Terrain"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		[NoScaleOffset] _FlowMap("Flow (RG, A noise)", 2D) = "black" {}
		[NoScaleOffset] _DerivHeightMap("Deriv (AG) Height (B)", 2D) = "black" {}
		_UJump("U jump per phase", Range(-0.25, 0.25)) = 0.25
		_VJump("V jump per phase", Range(-0.25, 0.25)) = 0.25
		_Tiling("Tiling", Float) = 1
		_Speed("Speed", Float) = 1
		_FlowStrength("Flow Strength", Float) = 1
		_FlowOffset("Flow Offset", Float) = 0
		_HeightScale("Height Scale, Constant", Float) = 0.25
		_HeightScaleModulated("Height Scale, Modulated", Float) = 0.75
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader
		{
				Tags
				{
					"RenderType" = "Opaque"
				}
				LOD 200

			CGPROGRAM
			#pragma surface surf Standard fullforwardshadows
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

			UNITY_DECLARE_TEX2DARRAY(baseTextures);

			struct Input
			{
				float3 worldPos;
				float3 worldNormal;
			};

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

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				float heightPercent = inverseLerp(minHeight,maxHeight, IN.worldPos.y);
				float3 blendAxes = abs(WorldNormalVector(IN, o.Normal));
				blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

				for (int i = 0; i < layerCount; i++)
				{
					float drawStrength = inverseLerp(-baseBlends[i] / 2 - epsilon, baseBlends[i] / 2, heightPercent - baseStartHeights[i]);

					float3 baseColor = baseColors[i] * baseColorStrength[i];
					float3 textureColor = triplanar(IN.worldPos, baseTextureScales[i], blendAxes, i) * (1 - baseColorStrength[i]);

					o.Albedo = o.Albedo * (1 - drawStrength) + (baseColor + textureColor) * drawStrength;
				}


			}


			ENDCG
		}
			FallBack "Diffuse"
}