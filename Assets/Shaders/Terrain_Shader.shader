// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Terrain_Shader"
{
    Properties
    {
		[HideInInspector]_Control("Control", 2D) = "white" {}
		[HideInInspector]_Splat3("Splat3", 2D) = "white" {}
		[HideInInspector]_Splat2("Splat2", 2D) = "white" {}
		[HideInInspector]_Splat1("Splat1", 2D) = "white" {}
		[HideInInspector]_Splat0("Splat0", 2D) = "white" {}
		[HideInInspector]_Smoothness3("Smoothness3", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness1("Smoothness1", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness0("Smoothness0", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness2("Smoothness2", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}





		//Color stuff
		_Color("Color", Color) = (1,1,1,1)
		_GradientMap("Gradient map", 2D) = "white" {}

		_PlacementTexture("Placement texture", 2D) = "white" {}
		_TerrainScale("Terrain scale", float) = 1

		//Noise and wind
		_NoiseTexture("Noise texture", 2D) = "white" {}
		_WindTexture("Wind texture", 2D) = "white" {}
		_WindStrength("Wind strength", float) = 0
		_WindSpeed("Wind speed", float) = 0
		_WindColor("Wind color", Color) = (1,1,1,1)

		//Position and dimensions
		_GrassHeight("Grass height", float) = 0
		_GrassWidth("Grass width", Range(0.0, 1.0)) = 1.0
		_PositionRandomness("Position randomness", float) = 0

		//Grass blades
		_GrassBlades("Grass blades per triangle", Range(0, 31)) = 1
		_MinimunGrassBlades("Minimum grass blades per triangle", Range(0, 31)) = 1
		_MaxCameraDistance("Max camera distance", float) = 10
    }

    SubShader
    {
		

        Tags { "RenderPipeline"="LightweightPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
        Cull Off
		HLSLINCLUDE
		#pragma target 3.0
		ENDHLSL

		
        Pass
        {
            Tags { "LightMode"="LightweightForward" "SplatCount"="4" "RenderType" = "Opaque"}
            Name "Base"

            Blend One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			Cull Off
			

            HLSLPROGRAM
            
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            // -------------------------------------
            // Lightweight Pipeline keywords
            #pragma shader_feature _SAMPLE_GI

            // -------------------------------------
            // Unity defined keywords
			#ifdef ASE_FOG
            #pragma multi_compile_fog
			#endif
            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            
            #pragma vertex vert
			//eeeeeeeeeeeeeeeeeeeeeeeeee
			#pragma geometry geom
			//eeeeeeeeeeeeeeeeeeeeeeeee
            #pragma fragment frag

            #define ASE_SRP_VERSION 0


            // Lighting include is needed because of GI
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/Shaders/UnlitInput.hlsl"

			float4 LampPositionsArray[16];
			float LampRangesArray[16];
			sampler2D _Control;
			sampler2D _Splat0;
			sampler2D _Splat1;
			sampler2D _Splat2;
			sampler2D _Splat3;
			int LampCount;
			int OverrideLamps;
			CBUFFER_START( UnityPerMaterial )
			float4 _Control_ST;
			float _Smoothness0;
			float4 _Splat0_ST;
			float _Smoothness1;
			float4 _Splat1_ST;
			float _Smoothness2;
			float4 _Splat2_ST;
			float _Smoothness3;
			float4 _Splat3_ST;
			CBUFFER_END



			//eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
			float4 _Color;
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
			float4 _WindColor;
			float _GrassHeight;
			float _GrassWidth;
			float _PositionRandomness;
			float _GrassBlades;
			float _MinimunGrassBlades;
			float _MaxCameraDistance;

			struct appdata
			{
				float4 vertex : POSITION;
			};


			float random(float2 st) {
				return frac(sin(dot(st.xy,
					float2(12.9898, 78.233))) *
					43758.5453123);
			}

			struct v2g
			{
				float4 vertex : POSITION;
			};

			struct g2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 col : COLOR;
			};

			//eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
			

            struct GraphVertexInput
            {
                float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct GraphVertexOutput
            {
                float4 position : POSITION;
				#ifdef ASE_FOG
				float fogCoord : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

			int LampCheck( float3 WorldPos , float ArraySize )
			{
				for(int i = 0; i < ArraySize; i++)
				{
					float d = distance(WorldPos, LampPositionsArray[i]);
					if(d < LampRangesArray[i])
					{
						return 1;
					}
				}
				return 0;
			}
			

            GraphVertexOutput vert (GraphVertexInput v)
            {
                GraphVertexOutput o = (GraphVertexOutput)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				o.ase_texcoord2.xyz = ase_worldPos;
				
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				o.ase_texcoord2.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = v.vertex.xyz;
				#else
				float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue =  defaultVertexValue ;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue; 
				#else
				v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal =  v.ase_normal ;
                o.position = TransformObjectToHClip(v.vertex.xyz);
				#ifdef ASE_FOG
				o.fogCoord = ComputeFogFactor( o.position.z );
				#endif
                return o;
            }

			g2f GetVertex(float4 pos, float2 uv, float4 col) {
				g2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, pos);
				o.uv = uv;
				o.col = col;
				return o;
			}

			v2g vert(appdata v)
			{
				v2g o;
				o.vertex = v.vertex;
				return o;
			}

			[maxvertexcount(96)]
			void geom(triangle v2g input[3], inout TriangleStream<g2f> triStream)
			{
				float3 normal = normalize(cross(input[1].vertex - input[0].vertex, input[2].vertex - input[0].vertex));
				int grassBlades = ceil(lerp(_GrassBlades, _MinimunGrassBlades, saturate(distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, input[0].vertex)) / _MaxCameraDistance)));

				for (uint i = 0; i < grassBlades; i++) {
					float r1 = random(mul(unity_ObjectToWorld, input[0].vertex).xz * (i + 1));
					float r2 = random(mul(unity_ObjectToWorld, input[1].vertex).xz * (i + 1));

					//Random barycentric coordinates from https://stackoverflow.com/a/19654424
					float4 midpoint = (1 - sqrt(r1)) * input[0].vertex + (sqrt(r1) * (1 - r2)) * input[1].vertex + (sqrt(r1) * r2) * input[2].vertex;

					r1 = r1 * 2.0 - 1.0;
					r2 = r2 * 2.0 - 1.0;

					float4 pointA = midpoint + _GrassWidth * normalize(input[i % 3].vertex - midpoint);
					float4 pointB = midpoint - _GrassWidth * normalize(input[i % 3].vertex - midpoint);

					float4 worldPos = mul(unity_ObjectToWorld, midpoint);

					float2 windTex = tex2Dlod(_WindTexture, float4(worldPos.xz * _WindTexture_ST.xy + _Time.y * _WindSpeed, 0.0, 0.0)).xy;
					float2 wind = (windTex * 2.0 - 1.0) * _WindStrength;

					float noise = tex2Dlod(_NoiseTexture, float4(worldPos.xz * _NoiseTexture_ST.xy, 0.0, 0.0)).x;
					float place = 1 - tex2Dlod(_PlacementTexture, float4(worldPos.xz * _TerrainScale, 0.0, 0.0)).r;
					float heightFactor = noise * _GrassHeight;
					//heightFactor = heightFactor * place;

					triStream.Append(GetVertex(pointA, float2(0, 0), float4(0, 0, 0, 1)));

					float4 newVertexPoint = midpoint + float4(normal, 0.0) * heightFactor + float4(r1, 0.0, r2, 0.0) * _PositionRandomness + float4(wind.x, 0.0, wind.y, 0.0);
					triStream.Append(GetVertex(newVertexPoint, float2(0.5, 1), float4(1.0, length(windTex), 1.0, 1.0)));

					triStream.Append(GetVertex(pointB, float2(1, 0), float4(0, 0, 0, 1)));

					triStream.RestartStrip();
				}


				for (int i = 0; i < 3; i++) {
					triStream.Append(GetVertex(input[i].vertex, float2(0, 0), float4(0, 0, 0, 1)));
				}


				triStream.RestartStrip();
			}

            half4 frag (GraphVertexOutput IN ) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
				float2 uv_Control = IN.ase_texcoord1.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode5_g1 = tex2D( _Control, uv_Control );
				float dotResult20_g1 = dot( tex2DNode5_g1 , float4(1,1,1,1) );
				float SplatWeight22_g1 = dotResult20_g1;
				float localSplatClip74_g1 = ( SplatWeight22_g1 );
				float SplatWeight74_g1 = SplatWeight22_g1;
				#if !defined(SHADER_API_MOBILE) && defined(TERRAIN_SPLAT_ADDPASS)
				clip(SplatWeight74_g1 == 0.0f ? -1 : 1);
				#endif
				float4 SplatControl26_g1 = ( tex2DNode5_g1 / ( localSplatClip74_g1 + 0.001 ) );
				float4 temp_output_59_0_g1 = SplatControl26_g1;
				float4 appendResult33_g1 = (float4(1.0 , 1.0 , 1.0 , _Smoothness0));
				float2 uv_Splat0 = IN.ase_texcoord1.xy * _Splat0_ST.xy + _Splat0_ST.zw;
				float4 appendResult36_g1 = (float4(1.0 , 1.0 , 1.0 , _Smoothness1));
				float2 uv_Splat1 = IN.ase_texcoord1.xy * _Splat1_ST.xy + _Splat1_ST.zw;
				float4 appendResult39_g1 = (float4(1.0 , 1.0 , 1.0 , _Smoothness2));
				float2 uv_Splat2 = IN.ase_texcoord1.xy * _Splat2_ST.xy + _Splat2_ST.zw;
				float4 appendResult42_g1 = (float4(1.0 , 1.0 , 1.0 , _Smoothness3));
				float2 uv_Splat3 = IN.ase_texcoord1.xy * _Splat3_ST.xy + _Splat3_ST.zw;
				float4 weightedBlendVar9_g1 = temp_output_59_0_g1;
				float4 weightedBlend9_g1 = ( weightedBlendVar9_g1.x*( appendResult33_g1 * tex2D( _Splat0, uv_Splat0 ) ) + weightedBlendVar9_g1.y*( appendResult36_g1 * tex2D( _Splat1, uv_Splat1 ) ) + weightedBlendVar9_g1.z*( appendResult39_g1 * tex2D( _Splat2, uv_Splat2 ) ) + weightedBlendVar9_g1.w*( appendResult42_g1 * tex2D( _Splat3, uv_Splat3 ) ) );
				float4 MixDiffuse28_g1 = weightedBlend9_g1;
				float4 lerpResult1 = lerp( MixDiffuse28_g1 , float4( 0,0,0,0 ) , float4( 0,0,0,0 ));
				float4 temp_cast_6 = (( ( lerpResult1.x + 0.0 ) / 10.0 )).xxxx;
				float3 ase_worldPos = IN.ase_texcoord2.xyz;
				float3 WorldPos185 = ase_worldPos;
				float ArraySize185 = (float)LampCount;
				int localLampCheck185 = LampCheck( WorldPos185 , ArraySize185 );
				float lerpResult229 = lerp( (float)localLampCheck185 , 1.0 , (float)OverrideLamps);
				float4 lerpResult191 = lerp( temp_cast_6 , lerpResult1 , lerpResult229);
				
		        float3 Color = lerpResult191.xyz;
		        float Alpha = 1;
		        float AlphaClipThreshold = 0;
         #if _AlphaClip
                clip(Alpha - AlphaClipThreshold);
        #endif

				#ifdef ASE_FOG
				Color = float4(1,1,1,1);
				//Color = MixFog( Color, IN.fogCoord );
				#endif
                return half4(Color, 1);
            }
            ENDHLSL
        }

		
        Pass
        {
			
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }
			ZWrite On
			ColorMask 0

            HLSLPROGRAM
            
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            
            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex ShadowPassVertex
			//#pragma geometry geom
            #pragma fragment ShadowPassFragment

            #define ASE_SRP_VERSION 0


            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			float4 LampPositionsArray[16];
			float LampRangesArray[16];
			CBUFFER_START( UnityPerMaterial )
			float4 _Control_ST;
			float _Smoothness0;
			float4 _Splat0_ST;
			float _Smoothness1;
			float4 _Splat1_ST;
			float _Smoothness2;
			float4 _Splat2_ST;
			float _Smoothness3;
			float4 _Splat3_ST;
			CBUFFER_END

            struct GraphVertexInput
            {
                float4 vertex : POSITION;
                float3 ase_normal : NORMAL;
				
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                float4 clipPos : SV_POSITION;
				
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // x: global clip space bias, y: normal world space bias
            float3 _LightDirection;

			
            VertexOutput ShadowPassVertex(GraphVertexInput v )
            {
                VertexOutput o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = v.vertex.xyz;
				#else
				float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue =  defaultVertexValue ;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal =  v.ase_normal ;

                float3 positionWS = TransformObjectToWorld(v.vertex.xyz);
                float3 normalWS = TransformObjectToWorldDir(v.ase_normal.xyz);

                float invNdotL = 1.0 - saturate(dot(_LightDirection, normalWS));
                float scale = invNdotL * _ShadowBias.y;

                // normal bias is negative since we want to apply an inset normal offset
                positionWS = _LightDirection * _ShadowBias.xxx + positionWS;
				positionWS = normalWS * scale.xxx + positionWS;
                float4 clipPos = TransformWorldToHClip(positionWS);

                // _ShadowBias.x sign depens on if platform has reversed z buffer
                //clipPos.z += _ShadowBias.x; 

            #if UNITY_REVERSED_Z
                clipPos.z = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
            #else
                clipPos.z = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
            #endif
                o.clipPos = clipPos;

                return o;
            }

            half4 ShadowPassFragment(VertexOutput IN  ) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
        		

				float Alpha = 1;
				float AlphaClipThreshold = AlphaClipThreshold;
         #if _AlphaClip
        		clip(Alpha - AlphaClipThreshold);
        #endif
                return 0;
            }

            ENDHLSL
        }

		
        Pass
        {
			
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }

            ZWrite On
			ZTest LEqual
			ColorMask 0

            HLSLPROGRAM
            
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex vert
            #pragma fragment frag

            #define ASE_SRP_VERSION 0


            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			float4 LampPositionsArray[16];
			float LampRangesArray[16];
			CBUFFER_START( UnityPerMaterial )
			float4 _Control_ST;
			float _Smoothness0;
			float4 _Splat0_ST;
			float _Smoothness1;
			float4 _Splat1_ST;
			float _Smoothness2;
			float4 _Splat2_ST;
			float _Smoothness3;
			float4 _Splat3_ST;
			CBUFFER_END

			struct GraphVertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

            struct VertexOutput
            {
                float4 clipPos : SV_POSITION;
				
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

			
			VertexOutput vert( GraphVertexInput v  )
			{
					VertexOutput o = (VertexOutput)0;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					
					#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
					#else
					float3 defaultVertexValue = float3(0, 0, 0);
					#endif
					float3 vertexValue =  defaultVertexValue ;	
					#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
					#else
					v.vertex.xyz += vertexValue;
					#endif
					v.ase_normal =  v.ase_normal ;
					o.clipPos = TransformObjectToHClip(v.vertex.xyz);
					return o;
			}

            half4 frag( VertexOutput IN  ) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
				

				float Alpha = 1;
				float AlphaClipThreshold = AlphaClipThreshold;

         #if _AlphaClip
        		clip(Alpha - AlphaClipThreshold);
        #endif
                return 0;
            }
            ENDHLSL
        }
		
    }
    Fallback "Hidden/InternalErrorShader"
	CustomEditor "ASEMaterialInspector"
	
}
/*ASEBEGIN
Version=17000
0;0;1080;1859;387.0143;4204.371;3.353407;True;False
Node;AmplifyShaderEditor.PosVertexDataNode;196;1290.564,-416.8708;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;181;496.2525,-619.7297;Float;False;Four Splats First Pass Terrain;0;;1;37452fdfb732e1443b7e39720d05b708;0;6;59;FLOAT4;0,0,0,0;False;60;FLOAT4;0,0,0,0;False;61;FLOAT3;0,0,0;False;57;FLOAT;0;False;58;FLOAT;0;False;62;FLOAT;0;False;6;FLOAT4;0;FLOAT3;14;FLOAT;56;FLOAT;45;FLOAT;19;FLOAT3;17
Node;AmplifyShaderEditor.GlobalArrayNode;184;1598.769,-167.8;Float;False;LampPositionsArray;0;16;2;False;False;0;1;True;Object;184;4;0;INT;0;False;2;INT;0;False;1;INT;0;False;3;INT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.IntNode;201;1029.976,-222.5294;Float;False;Global;LampCount;LampCount;13;0;Create;True;0;0;True;0;1;16;0;1;INT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;150;1917.228,-792.0027;Float;False;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;148;1531.936,-785.153;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;149;1244.251,-790.2903;Float;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.IntNode;231;2178.556,-86.62788;Float;False;Global;OverrideLamps;OverrideLamps;15;0;Create;True;0;0;False;0;0;0;0;1;INT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;197;1638.122,-419.534;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;1;999.4504,-601.0829;Float;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CustomExpressionNode;185;2025.329,-308.4435;Float;False;for(int i = 0@ i < ArraySize@ i++)${$	float d = distance(WorldPos, LampPositionsArray[i])@$	if(d < LampRangesArray[i])$	{$		return 1@$	}$}$return 0@;0;False;2;True;WorldPos;FLOAT3;0,0,0;In;;Float;False;True;ArraySize;FLOAT;0;In;;Float;False;Lamp Check;False;False;0;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;INT;0
Node;AmplifyShaderEditor.LerpOp;229;2483.094,-308.6097;Float;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GlobalArrayNode;186;1268.577,-79.65739;Float;False;LampRangesArray;0;16;0;False;False;0;1;True;Object;186;4;0;INT;0;False;2;INT;0;False;1;INT;0;False;3;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;191;2873.257,-645.0718;Float;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;166;1857.76,-559.1481;Float;False;False;2;Float;ASEMaterialInspector;0;3;Hidden/Templates/LightWeightSRPUnlit;e2514bdcf5e5399499a9eb24d175b9db;True;DepthOnly;0;2;DepthOnly;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=DepthOnly;True;0;0;Hidden/InternalErrorShader;0;0;Standard;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;164;3496.121,-755.5497;Float;False;True;2;Float;ASEMaterialInspector;0;3;Terrain_Shader;e2514bdcf5e5399499a9eb24d175b9db;True;Base;0;0;Base;5;False;False;False;True;2;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;2;LightMode=LightweightForward;SplatCount=4;False;0;Hidden/InternalErrorShader;0;0;Standard;3;Vertex Position,InvertActionOnDeselection;1;Receive Shadows;1;Built-in Fog;0;0;3;True;True;True;False;5;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;165;1857.76,-559.1481;Float;False;False;2;Float;ASEMaterialInspector;0;3;Hidden/Templates/LightWeightSRPUnlit;e2514bdcf5e5399499a9eb24d175b9db;True;ShadowCaster;0;1;ShadowCaster;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;0
WireConnection;150;0;148;0
WireConnection;148;0;149;0
WireConnection;149;0;1;0
WireConnection;1;0;181;0
WireConnection;185;0;197;0
WireConnection;185;1;201;0
WireConnection;229;0;185;0
WireConnection;229;2;231;0
WireConnection;191;0;150;0
WireConnection;191;1;1;0
WireConnection;191;2;229;0
WireConnection;164;0;191;0
ASEEND*/
//CHKSM=7320AAD462C33EBD92DB6B0943EBFCC3F538681B