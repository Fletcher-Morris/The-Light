﻿Shader "Custom/Sexy_Grass_Shader"
{
	Properties
	{
		_GradientMap("Gradient map", 2D) = "white" {}
		_TipColor("Tip Color", Color) = (0.2574063,0.3773585,0,0)
		_RootColor("Root Color", Color) = (0.1116701,0.245283,0,0)
		_PlacementTexture("Placement texture", 2D) = "white" {}
		_TerrainScale("Terrain scale", float) = 1
		_NoiseTexture("Noise texture", 2D) = "white" {}
		_WindTexture("Wind texture", 2D) = "white" {}
		_WindStrength("Wind strength", float) = 0
		_WindSpeed("Wind speed", float) = 0
		_WindColor("Wind color", Color) = (1,1,1,1)
		_GrassHeight("Grass height", float) = 0
		_GrassWidth("Grass width", Range(0.0, 1.0)) = 1.0
		_PositionRandomness("Position randomness", float) = 0
		_GrassBlades("Grass blades per triangle", Range(0, 25)) = 1
		_MinimunGrassBlades("Minimum grass blades per triangle", Range(0, 25)) = 1
		_MaxCameraDistance("Max camera distance", float) = 10
		_MinNormal("Min Normal", Range(0.0, 1.0)) = 0.5
	}
		SubShader
	{

		CGINCLUDE

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2g
			{
				float4 vertex : POSITION;
			};

			struct g2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD1;
				float4 col : COLOR;
			};

			sampler2D _GradientMap;
			sampler2D _PlacementTexture;
			sampler2D _PlacementTexture_ST;
			float _TerrainScale;
			sampler2D _NoiseTexture;
			float4 _NoiseTexture_ST;
			sampler2D _WindTexture;
			float4 _WindTexture_ST;
			float _WindStrength;
			float _WindSpeed;
			fixed4 _WindColor;

			float _GrassHeight;
			float _GrassWidth;
			float _PositionRandomness;

			float _GrassBlades;
			float _MinimunGrassBlades;
			float _MaxCameraDistance;
			float _MinNormal;
			float4 _RootColor;
			float4 _TipColor;

			float4 Lamps[16];
			float4 Colors[16];
			float3 PlayerPosition;
			float4 AmbientColor;
			float4 LightColor = (1,1,1,1);
			float OverrideLamps;

			float random(float2 st) {
				return frac(sin(dot(st.xy,
									float2(12.9898,78.233)))*
					43758.5453123);
			}


			g2f GetVertex(float4 pos, float2 uv, fixed4 col) {
				g2f o;
				o.vertex = UnityObjectToClipPos(pos);
				o.uv = uv;
				o.worldPos = mul(unity_ObjectToWorld, pos);
				o.col = col;
				return o;
			}

			v2g vert(appdata v)
			{
				v2g o;
				o.vertex = v.vertex;
				return o;
			}

			//3 + (3 * 31) = 96
			[maxvertexcount(78)]
			void geom(triangle v2g input[3], inout TriangleStream<g2f> triStream)
			{
				float3 normal = normalize(cross(input[1].vertex - input[0].vertex, input[2].vertex - input[0].vertex));
				int grassBlades = ceil(lerp(_GrassBlades, _MinimunGrassBlades, saturate(distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, input[0].vertex)) / _MaxCameraDistance)));

				for (uint i = 0; i < grassBlades; i++) {
					float r1 = random(mul(unity_ObjectToWorld, input[0].vertex).xz * (i + 1));
					float r2 = random(mul(unity_ObjectToWorld, input[1].vertex).xz * (i + 1));
					float4 midpoint = (1 - sqrt(r1)) * input[0].vertex + (sqrt(r1) * (1 - r2)) * input[1].vertex + (sqrt(r1) * r2) * input[2].vertex;

					r1 = r1 * 2.0 - 1.0;
					r2 = r2 * 2.0 - 1.0;

					float4 worldPos = mul(unity_ObjectToWorld, midpoint);

					float2 windTex = tex2Dlod(_WindTexture, float4(worldPos.xz * _WindTexture_ST.xy + _Time.y * _WindSpeed, 0.0, 0.0)).xy;
					float2 wind = (windTex * 2.0 - 1.0) * _WindStrength;

					float useWidth = _GrassWidth;


					float playerDist = abs(length(worldPos.xyz - PlayerPosition.xyz));
					float closeness = saturate(playerDist / 1.0);



					float noise = tex2Dlod(_NoiseTexture, float4(worldPos.xz * _NoiseTexture_ST.xy, 0.0, 0.0)).x;
					float d = 1.0 / _TerrainScale;
					float place = saturate(tex2Dlod(_PlacementTexture, float4(worldPos.x * d, worldPos.z * d, 0.0, 0.0)).r);
					float heightFactor = 1.0;
					heightFactor *= place;
					float slopeFactor = 1.0;
					slopeFactor *= saturate(normal.y);
					heightFactor *= closeness;
					if (slopeFactor < _MinNormal)
					{
						useWidth = 0.0;
						slopeFactor = 0.0;
					}
					heightFactor *= slopeFactor;
					heightFactor *= _GrassHeight;
					heightFactor *= noise;
					if (heightFactor <= 0.001)
					{
						useWidth = 0.0;
					}

					float4 pointA = midpoint + useWidth * normalize(input[i % 3].vertex - midpoint);
					float4 pointB = midpoint - useWidth * normalize(input[i % 3].vertex - midpoint);


					triStream.Append(GetVertex(pointA, float2(0,0), fixed4(0,0,0,1)));

					float4 newVertexPoint = midpoint + float4(normal, 0.0) * heightFactor + float4(r1, 0.0, r2, 0.0) * _PositionRandomness + float4(wind.x, 0.0, wind.y, 0.0);
					triStream.Append(GetVertex(newVertexPoint, float2(0.5, 1), fixed4(1.0, length(windTex), 1.0, 1.0)));

					triStream.Append(GetVertex(pointB, float2(1,0), fixed4(0,0,0,1)));

					triStream.RestartStrip();
				}


				for (int i = 0; i < 3; i++) {
					triStream.Append(GetVertex(input[i].vertex, float2(0,0), fixed4(0,0,0,1)));
				}


				triStream.RestartStrip();
			}

			int LampCheck(float3 WorldPos)
			{
				for (int i = 0; i < 16; i++)
				{
					float d = distance(WorldPos, Lamps[i].xyz);
					if (d < Lamps[i].w)
					{
						return 1;
					}
				}
				return 0;
			}

			int LampColor(float3 WorldPos)
			{
				for (int i = 0; i < 16; i++)
				{
					float d = distance(WorldPos, Lamps[i].xyz);
					if (d < Lamps[i].w)
					{
						return Colors[i];
					}
				}
				return float3(0,0,0);
			}

			fixed4 frag(g2f i) : SV_Target
			{
				fixed4 gradientMapCol = tex2D(_GradientMap, float2(i.col.x, 0.0));
				fixed4 col = lerp(_RootColor, _TipColor, i.col.x);

				fixed4 lampCol = lerp(LampColor(i.worldPos.xyz), LightColor, OverrideLamps);
				fixed4 colAdjust = lerp(AmbientColor, lampCol, LampCheck(i.worldPos.xyz));

				col *= colAdjust;
				return col;
			}



		ENDCG

		Pass
		{
			Tags { "RenderType" = "Opaque"}
			Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag

			ENDCG
		}

	}
}