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



			#include "Flow.cginc"
			sampler2D _MainTex, _FlowMap, _DerivHeightMap;
			float _UJump, _VJump, _Tiling, _Speed, _FlowStrength, _FlowOffset;
			float _HeightScale, _HeightScaleModulated;

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

			float3 UnpackDerivativeHeight(float4 textureData) {
				float3 dh = textureData.agb;
				dh.xy = dh.xy * 2 - 1;
				return dh;
			}

			void createWaterTexture(Input IN,inout SurfaceOutputStandard o)
			{
				float3 flow = tex2D(_FlowMap, IN.uv_MainTex).rgb;
				flow.xy = flow.xy * 2 - 1;
				flow *= _FlowStrength;
				float noise = tex2D(_FlowMap, IN.uv_MainTex).a;
				float time = _Time.y * _Speed + noise;
				float2 jump = float2(_UJump, _VJump);

				float3 uvwA = FlowUVW(IN.uv_MainTex, flow.xy, jump,_FlowOffset, _Tiling, time, false);
				float3 uvwB = FlowUVW(IN.uv_MainTex, flow.xy, jump,_FlowOffset, _Tiling, time, true);

				float finalHeightScale = flow.z * _HeightScaleModulated + _HeightScale;

				float3 dhA = UnpackDerivativeHeight(tex2D(_DerivHeightMap, uvwA.xy)) * (uvwA.z * finalHeightScale);
				float3 dhB = UnpackDerivativeHeight(tex2D(_DerivHeightMap, uvwB.xy)) * (uvwB.z * finalHeightScale);
				o.Normal = o.Normal;
				normalize(float3(-(dhA.xy + dhB.xy), 1));

				fixed4 texA = tex2D(_MainTex, uvwA.xy) * uvwA.z;
				fixed4 texB = tex2D(_MainTex, uvwB.xy) * uvwB.z;

				fixed4 c = (texA + texB) * _Color;
				o.Albedo = c.rgb;
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
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