// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Grass_Shader"
{
    Properties
    {
		_TipColor("Tip Color", Color) = (0.2574063,0.3773585,0,0)
		_RootColor("Root Color", Color) = (0.1116701,0.245283,0,0)
		_GroundColor("Ground Color", Color) = (0.2470588,0.1525654,0,0)
		_TipHeight("Tip Height", Float) = 1
		_GroundFade("Ground Fade", Float) = 0.2
		_Float0("Float 0", Float) = 0.2
		_WindSpeed("Wind Speed", Float) = 1
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
            Tags { "LightMode"="LightweightForward" "SplatCount"="4" }
            Name "Base"

            Blend One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

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
            #pragma fragment frag

            #define ASE_SRP_VERSION 0
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT


            // Lighting include is needed because of GI
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/Shaders/UnlitInput.hlsl"

			float4 LampPositionsArray[16];
			float LampRangesArray[16];
			int GamePausedInt;
			float4 AmbientColor;
			int LampCount;
			int OverrideLamps;
			CBUFFER_START( UnityPerMaterial )
			float _GroundFade;
			float _WindSpeed;
			float _Float0;
			float4 _GroundColor;
			float4 _RootColor;
			float4 _TipColor;
			float _TipHeight;
			CBUFFER_END

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
				float4 ase_texcoord3 : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
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
				float clampResult253 = clamp( ( v.vertex.xyz.y / _GroundFade ) , 0.0 , 1.0 );
				float mulTime281 = _Time.y * (float)GamePausedInt;
				float3 objToWorld301 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float2 appendResult299 = (float2(objToWorld301.x , objToWorld301.z));
				float2 uv0279 = v.ase_texcoord * float2( 1,1 ) + ( mulTime281 * appendResult299 * _WindSpeed * 0.0025 );
				float simplePerlin2D294 = snoise( uv0279*0.2 );
				simplePerlin2D294 = simplePerlin2D294*0.5 + 0.5;
				float simplePerlin2D293 = snoise( uv0279*0.112 );
				simplePerlin2D293 = simplePerlin2D293*0.5 + 0.5;
				float3 appendResult292 = (float3(( simplePerlin2D294 - 0.5 ) , 0.0 , ( simplePerlin2D293 - 0.5 )));
				
				VertexPositionInputs ase_vertexInput = GetVertexPositionInputs (v.vertex.xyz);
				#ifdef _MAIN_LIGHT_SHADOWS//ase_lightAtten_vert
				o.ase_texcoord1 = GetShadowCoord( ase_vertexInput );
				#endif//ase_lightAtten_vert
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				o.ase_texcoord2.xyz = ase_worldPos;
				
				o.ase_texcoord3 = v.vertex;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = v.vertex.xyz;
				#else
				float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( pow( clampResult253 , 2.0 ) * appendResult292 * _Float0 );
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

            half4 frag (GraphVertexOutput IN ) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
				float3 ase_worldPos = IN.ase_texcoord2.xyz;
				float ase_lightAtten = 0;
				Light ase_lightAtten_mainLight = GetMainLight( IN.ase_texcoord1 );
				ase_lightAtten = ase_lightAtten_mainLight.distanceAttenuation * ase_lightAtten_mainLight.shadowAttenuation;
				#ifdef _ADDITIONAL_LIGHTS//ase_lightAtten_frag
				int ase_lightAtten_pixelLightCount = GetAdditionalLightsCount();
				for (int i = 0; i < ase_lightAtten_pixelLightCount; ++i)
				{//ase_lightAtten_frag
				Light ase_lightAtten_pointLight = GetAdditionalLight( i, ase_worldPos );
				ase_lightAtten += ase_lightAtten_pointLight.distanceAttenuation * ase_lightAtten_pointLight.shadowAttenuation;
				}//ase_lightAtten_frag
				#endif//ase_lightAtten_frag
				ase_lightAtten = saturate( ase_lightAtten );
				float clampResult263 = clamp( step( 0.7 , ( ase_lightAtten + 0.0 ) ) , 0.5 , 1.0 );
				float clampResult253 = clamp( ( IN.ase_texcoord3.xyz.y / _GroundFade ) , 0.0 , 1.0 );
				float4 lerpResult252 = lerp( _GroundColor , _RootColor , clampResult253);
				float4 lerpResult241 = lerp( lerpResult252 , _TipColor , ( IN.ase_texcoord3.xyz.y / _TipHeight ).r);
				float4 clampResult243 = clamp( lerpResult241 , min( _TipColor , lerpResult252 ) , max( _TipColor , lerpResult252 ) );
				float4 clampResult261 = clamp( ( AmbientColor * clampResult263 * clampResult243 ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
				float4 temp_cast_2 = (( ( clampResult261.x + 0.0 ) / 10.0 )).xxxx;
				float3 WorldPos185 = ase_worldPos;
				float ArraySize185 = (float)LampCount;
				int localLampCheck185 = LampCheck( WorldPos185 , ArraySize185 );
				float lerpResult229 = lerp( (float)localLampCheck185 , 1.0 , (float)OverrideLamps);
				float4 lerpResult191 = lerp( temp_cast_2 , clampResult261 , lerpResult229);
				
		        float3 Color = lerpResult191.xyz;
		        float Alpha = 1;
		        float AlphaClipThreshold = 0;
         #if _AlphaClip
                clip(Alpha - AlphaClipThreshold);
        #endif

				#ifdef ASE_FOG
				Color = MixFog( Color, IN.fogCoord );
				#endif
                return half4(Color, Alpha);
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
            #pragma fragment ShadowPassFragment

            #define ASE_SRP_VERSION 0


            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			float4 LampPositionsArray[16];
			float LampRangesArray[16];
			int GamePausedInt;
			CBUFFER_START( UnityPerMaterial )
			float _GroundFade;
			float _WindSpeed;
			float _Float0;
			float4 _GroundColor;
			float4 _RootColor;
			float4 _TipColor;
			float _TipHeight;
			CBUFFER_END

            struct GraphVertexInput
            {
                float4 vertex : POSITION;
                float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
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

			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			

            VertexOutput ShadowPassVertex(GraphVertexInput v )
            {
                VertexOutput o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
				float clampResult253 = clamp( ( v.vertex.xyz.y / _GroundFade ) , 0.0 , 1.0 );
				float mulTime281 = _Time.y * (float)GamePausedInt;
				float3 objToWorld301 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float2 appendResult299 = (float2(objToWorld301.x , objToWorld301.z));
				float2 uv0279 = v.ase_texcoord * float2( 1,1 ) + ( mulTime281 * appendResult299 * _WindSpeed * 0.0025 );
				float simplePerlin2D294 = snoise( uv0279*0.2 );
				simplePerlin2D294 = simplePerlin2D294*0.5 + 0.5;
				float simplePerlin2D293 = snoise( uv0279*0.112 );
				simplePerlin2D293 = simplePerlin2D293*0.5 + 0.5;
				float3 appendResult292 = (float3(( simplePerlin2D294 - 0.5 ) , 0.0 , ( simplePerlin2D293 - 0.5 )));
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = v.vertex.xyz;
				#else
				float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( pow( clampResult253 , 2.0 ) * appendResult292 * _Float0 );
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
			int GamePausedInt;
			CBUFFER_START( UnityPerMaterial )
			float _GroundFade;
			float _WindSpeed;
			float _Float0;
			float4 _GroundColor;
			float4 _RootColor;
			float4 _TipColor;
			float _TipHeight;
			CBUFFER_END

			struct GraphVertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

            struct VertexOutput
            {
                float4 clipPos : SV_POSITION;
				
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			

			VertexOutput vert( GraphVertexInput v  )
			{
					VertexOutput o = (VertexOutput)0;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					float clampResult253 = clamp( ( v.vertex.xyz.y / _GroundFade ) , 0.0 , 1.0 );
					float mulTime281 = _Time.y * (float)GamePausedInt;
					float3 objToWorld301 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
					float2 appendResult299 = (float2(objToWorld301.x , objToWorld301.z));
					float2 uv0279 = v.ase_texcoord * float2( 1,1 ) + ( mulTime281 * appendResult299 * _WindSpeed * 0.0025 );
					float simplePerlin2D294 = snoise( uv0279*0.2 );
					simplePerlin2D294 = simplePerlin2D294*0.5 + 0.5;
					float simplePerlin2D293 = snoise( uv0279*0.112 );
					simplePerlin2D293 = simplePerlin2D293*0.5 + 0.5;
					float3 appendResult292 = (float3(( simplePerlin2D294 - 0.5 ) , 0.0 , ( simplePerlin2D293 - 0.5 )));
					
					#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
					#else
					float3 defaultVertexValue = float3(0, 0, 0);
					#endif
					float3 vertexValue = ( pow( clampResult253 , 2.0 ) * appendResult292 * _Float0 );	
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
0;0;1080;1859;-182.3501;2853.054;1;True;False
Node;AmplifyShaderEditor.TransformPositionNode;301;665.8652,-2188.2;Float;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.IntNode;307;532.296,-2356.85;Float;False;Global;GamePausedInt;GamePausedInt;31;0;Create;True;0;0;False;0;1;1;0;1;INT;0
Node;AmplifyShaderEditor.DynamicAppendNode;299;907.9495,-2149.5;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;302;692.0619,-2041.285;Float;False;Property;_WindSpeed;Wind Speed;6;0;Create;True;0;0;False;0;1;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;305;764.8914,-1944.163;Float;False;Constant;_Float1;Float 1;7;0;Create;True;0;0;False;0;0.0025;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;281;768.0249,-2360.212;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;298;1059.949,-2141.5;Float;False;4;4;0;FLOAT;0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;251;-1159.813,-291.7218;Float;False;Property;_GroundFade;Ground Fade;4;0;Create;True;0;0;False;0;0.2;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;260;-1241.806,-484.5206;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;279;1261.576,-2252.489;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;294;1618.773,-2303.892;Float;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;293;1627.634,-2067.226;Float;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;0.112;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;254;-931.8976,-568.4446;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;303;1943.827,-2291.2;Float;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;304;1933.879,-2059.546;Float;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;253;-778.9216,-637.0591;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;306;1879.391,-1953.745;Float;True;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;297;2137.914,-1885.774;Float;False;Property;_Float0;Float 0;5;0;Create;True;0;0;False;0;0.2;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;292;2152.06,-2200.391;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMinOpNode;244;-73.74204,-1041.578;Float;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;243;320.8004,-827.8781;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector4Node;174;349.4616,-1269.091;Float;False;Global;AmbientColor;AmbientColor;10;0;Create;True;0;0;False;0;0.5,0.5,0.5,1;1,1,1,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GlobalArrayNode;184;1598.769,-167.8;Float;False;LampPositionsArray;0;16;2;False;False;0;1;True;Object;184;4;0;INT;0;False;2;INT;0;False;1;INT;0;False;3;INT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;259;-1242.69,-689.2725;Float;False;Property;_GroundColor;Ground Color;2;0;Create;True;0;0;False;0;0.2470588,0.1525654,0,0;0.2574063,0.3773585,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.IntNode;231;2178.556,-86.62788;Float;False;Global;OverrideLamps;OverrideLamps;15;0;Create;True;0;0;False;0;0;1;0;1;INT;0
Node;AmplifyShaderEditor.LerpOp;191;2873.257,-645.0718;Float;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;239;-638.3131,-1175.096;Float;False;Property;_TipColor;Tip Color;0;0;Create;True;0;0;False;0;0.2574063,0.3773585,0,0;0.2574063,0.3773585,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;247;-163.8698,-432.2054;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;150;1917.228,-792.0027;Float;False;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;148;1531.936,-785.153;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;33;21.44477,-207.8682;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;185;2025.329,-308.4435;Float;False;for(int i = 0@ i < ArraySize@ i++)${$	float d = distance(WorldPos, LampPositionsArray[i])@$	if(d < LampRangesArray[i])$	{$		return 1@$	}$}$return 0@;0;False;2;True;WorldPos;FLOAT3;0,0,0;In;;Float;False;True;ArraySize;FLOAT;0;In;;Float;False;Lamp Check;False;False;0;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;INT;0
Node;AmplifyShaderEditor.ClampOpNode;261;1119.351,-544.6166;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;1,1,1,1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;240;-1186.46,-911.8824;Float;False;Property;_RootColor;Root Color;1;0;Create;True;0;0;False;0;0.1116701,0.245283,0,0;0.2574063,0.3773585,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;27;501.2351,-220.1645;Float;True;2;0;FLOAT;0.7;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;241;-37.04302,-724.7268;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;252;-637.1942,-730.4189;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;245;-20.16986,-860.2543;Float;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GlobalArrayNode;186;1268.577,-79.65739;Float;False;LampRangesArray;0;16;0;False;False;0;1;True;Object;186;4;0;INT;0;False;2;INT;0;False;1;INT;0;False;3;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;246;-419.9371,-429.1981;Float;False;Property;_TipHeight;Tip Height;3;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;201;1029.976,-222.5294;Float;False;Global;LampCount;LampCount;13;0;Create;True;0;0;True;0;1;16;0;1;INT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;831.3207,-590.3572;Float;True;3;3;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ClampOpNode;263;729.5577,-144.3762;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;197;1638.122,-419.534;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.BreakToComponentsNode;149;1244.251,-790.2903;Float;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;168;300.0116,-149.4934;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;229;2483.094,-308.6097;Float;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;291;2380.875,-2026.192;Float;True;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;166;1857.76,-559.1481;Float;False;False;2;Float;ASEMaterialInspector;0;3;Hidden/Templates/LightWeightSRPUnlit;e2514bdcf5e5399499a9eb24d175b9db;True;DepthOnly;0;2;DepthOnly;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=DepthOnly;True;0;0;Hidden/InternalErrorShader;0;0;Standard;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;164;3683.091,-1294.585;Float;False;True;2;Float;ASEMaterialInspector;0;3;Grass_Shader;e2514bdcf5e5399499a9eb24d175b9db;True;Base;0;0;Base;5;False;False;False;True;2;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;2;LightMode=LightweightForward;SplatCount=4;False;0;Hidden/InternalErrorShader;0;0;Standard;3;Vertex Position,InvertActionOnDeselection;1;Receive Shadows;1;Built-in Fog;0;0;3;True;True;True;False;5;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;165;1857.76,-559.1481;Float;False;False;2;Float;ASEMaterialInspector;0;3;Hidden/Templates/LightWeightSRPUnlit;e2514bdcf5e5399499a9eb24d175b9db;True;ShadowCaster;0;1;ShadowCaster;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;0
WireConnection;299;0;301;1
WireConnection;299;1;301;3
WireConnection;281;0;307;0
WireConnection;298;0;281;0
WireConnection;298;1;299;0
WireConnection;298;2;302;0
WireConnection;298;3;305;0
WireConnection;279;1;298;0
WireConnection;294;0;279;0
WireConnection;293;0;279;0
WireConnection;254;0;260;2
WireConnection;254;1;251;0
WireConnection;303;0;294;0
WireConnection;304;0;293;0
WireConnection;253;0;254;0
WireConnection;306;0;253;0
WireConnection;292;0;303;0
WireConnection;292;2;304;0
WireConnection;244;0;239;0
WireConnection;244;1;252;0
WireConnection;243;0;241;0
WireConnection;243;1;244;0
WireConnection;243;2;245;0
WireConnection;191;0;150;0
WireConnection;191;1;261;0
WireConnection;191;2;229;0
WireConnection;247;0;260;2
WireConnection;247;1;246;0
WireConnection;150;0;148;0
WireConnection;148;0;149;0
WireConnection;185;0;197;0
WireConnection;185;1;201;0
WireConnection;261;0;9;0
WireConnection;27;1;168;0
WireConnection;241;0;252;0
WireConnection;241;1;239;0
WireConnection;241;2;247;0
WireConnection;252;0;259;0
WireConnection;252;1;240;0
WireConnection;252;2;253;0
WireConnection;245;0;239;0
WireConnection;245;1;252;0
WireConnection;9;0;174;0
WireConnection;9;1;263;0
WireConnection;9;2;243;0
WireConnection;263;0;27;0
WireConnection;149;0;261;0
WireConnection;168;0;33;0
WireConnection;229;0;185;0
WireConnection;229;2;231;0
WireConnection;291;0;306;0
WireConnection;291;1;292;0
WireConnection;291;2;297;0
WireConnection;164;0;191;0
WireConnection;164;3;291;0
ASEEND*/
//CHKSM=CC2FC11FA0DB0846278729DE8001CA6A5C58D73D