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
		_WindSpeed("Wind Speed", Float) = 1
		_AlphaTexture("Alpha Texture", 2D) = "white" {}
		_MaxPlayerBend("Max Player Bend", Range( 0 , 2)) = 1.5
		[Toggle]_UseTexture("Use Texture", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
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
            #define _AlphaClip 1
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

			float4 Lamps[16];
			float3 PlayerPosition;
			sampler2D _AlphaTexture;
			int GamePausedInt;
			half WindGust;
			float4 AmbientColor;
			int OverrideLamps;
			CBUFFER_START( UnityPerMaterial )
			float _UseTexture;
			float _TipHeight;
			float4 _AlphaTexture_ST;
			float _MaxPlayerBend;
			float _WindSpeed;
			float4 _GroundColor;
			float4 _RootColor;
			float _GroundFade;
			float4 _TipColor;
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
				float4 ase_texcoord4 : TEXCOORD4;
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
			
			int LampCheck( float3 WorldPos )
			{
				for(int i = 0; i < 16; i++)
				{
					float d = distance(WorldPos, Lamps[i].xyz);
					if(d < Lamps[i].w)
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
				float4 appendResult331 = (float4(PlayerPosition.x , 0.0 , PlayerPosition.z , 0.0));
				float3 objToWorld311 = mul( GetObjectToWorldMatrix(), float4( v.vertex.xyz, 1 ) ).xyz;
				float3 appendResult330 = (float3(objToWorld311.x , 0.0 , objToWorld311.z));
				float temp_output_314_0 = distance( appendResult331 , float4( appendResult330 , 0.0 ) );
				float clampResult319 = clamp( temp_output_314_0 , 0.0 , 0.8 );
				float ifLocalVar323 = 0;
				if( temp_output_314_0 < 0.8 )
				ifLocalVar323 = ( 1.0 - clampResult319 );
				float4 break313 = ( float4( appendResult330 , 0.0 ) - appendResult331 );
				float4 appendResult320 = (float4(break313.x , 0.0 , break313.z , 0.0));
				float temp_output_247_0 = ( v.vertex.xyz.y / _TipHeight );
				float clampResult328 = clamp( temp_output_247_0 , 0.0 , 1.0 );
				float2 uv_AlphaTexture = v.ase_texcoord * _AlphaTexture_ST.xy + _AlphaTexture_ST.zw;
				float4 tex2DNode342 = tex2Dlod( _AlphaTexture, float4( uv_AlphaTexture, 0, 0.0) );
				float4 temp_cast_2 = (( 0.0 - _MaxPlayerBend )).xxxx;
				float4 temp_cast_3 = (_MaxPlayerBend).xxxx;
				float4 clampResult332 = clamp( ( ifLocalVar323 * appendResult320 * lerp(clampResult328,tex2DNode342.g,_UseTexture) * _MaxPlayerBend ) , temp_cast_2 , temp_cast_3 );
				float mulTime281 = _Time.y * (float)GamePausedInt;
				float3 objToWorld301 = mul( GetObjectToWorldMatrix(), float4( v.vertex.xyz, 1 ) ).xyz;
				float2 appendResult299 = (float2(objToWorld301.x , objToWorld301.z));
				float2 uv0279 = v.ase_texcoord.xy * float2( 1,1 ) + ( mulTime281 * appendResult299 * _WindSpeed * 0.01 );
				float simplePerlin2D294 = snoise( uv0279*0.2 );
				simplePerlin2D294 = simplePerlin2D294*0.5 + 0.5;
				float simplePerlin2D293 = snoise( uv0279*0.112 );
				simplePerlin2D293 = simplePerlin2D293*0.5 + 0.5;
				float3 appendResult292 = (float3(( simplePerlin2D294 - 0.5 ) , 0.0 , ( simplePerlin2D293 - 0.5 )));
				float clampResult362 = clamp( WindGust , 0.0 , 1.0 );
				float3 lerpResult361 = lerp( appendResult292 , float3(1,0,1) , clampResult362);
				float4 break335 = ( clampResult332 + float4( ( lerp(clampResult328,tex2DNode342.g,_UseTexture) * lerpResult361 * 0.2 ) , 0.0 ) );
				float2 appendResult337 = (float2(break335.x , break335.z));
				float3 appendResult339 = (float3(break335.x , ( 0.0 - distance( appendResult337 , float2( 0,0 ) ) ) , break335.z));
				
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				o.ase_texcoord1.xyz = ase_worldPos;
				VertexPositionInputs ase_vertexInput = GetVertexPositionInputs (v.vertex.xyz);
				#ifdef _MAIN_LIGHT_SHADOWS//ase_lightAtten_vert
				o.ase_texcoord3 = GetShadowCoord( ase_vertexInput );
				#endif//ase_lightAtten_vert
				
				o.ase_texcoord2 = v.vertex;
				o.ase_texcoord4.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.w = 0;
				o.ase_texcoord4.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = v.vertex.xyz;
				#else
				float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = appendResult339;
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
				float3 ase_worldPos = IN.ase_texcoord1.xyz;
				float3 WorldPos2_g2 = ase_worldPos;
				int localLampCheck2_g2 = LampCheck( WorldPos2_g2 );
				float lerpResult1_g2 = lerp( (float)localLampCheck2_g2 , 1.0 , (float)OverrideLamps);
				float4 lerpResult191 = lerp( AmbientColor , _MainLightColor , lerpResult1_g2);
				float clampResult253 = clamp( ( IN.ase_texcoord2.xyz.y / _GroundFade ) , 0.0 , 1.0 );
				float4 lerpResult252 = lerp( _GroundColor , _RootColor , clampResult253);
				float temp_output_247_0 = ( IN.ase_texcoord2.xyz.y / _TipHeight );
				float4 lerpResult241 = lerp( lerpResult252 , _TipColor , temp_output_247_0);
				float4 clampResult243 = clamp( lerpResult241 , min( _TipColor , lerpResult252 ) , max( _TipColor , lerpResult252 ) );
				float ase_lightAtten = 0;
				Light ase_lightAtten_mainLight = GetMainLight( IN.ase_texcoord3 );
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
				float clampResult263 = clamp( ase_lightAtten , 0.5 , 1.0 );
				float4 clampResult261 = clamp( ( clampResult243 * clampResult263 ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
				float4 clampResult368 = clamp( ( 1.0 * ( lerpResult191 * clampResult261 ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
				
				float2 uv_AlphaTexture = IN.ase_texcoord4.xy * _AlphaTexture_ST.xy + _AlphaTexture_ST.zw;
				float4 tex2DNode342 = tex2D( _AlphaTexture, uv_AlphaTexture );
				
		        float3 Color = clampResult368.rgb;
		        float Alpha = tex2DNode342.r;
		        float AlphaClipThreshold = 0.5;
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
            #define _AlphaClip 1


            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			float4 Lamps[16];
			float3 PlayerPosition;
			sampler2D _AlphaTexture;
			int GamePausedInt;
			half WindGust;
			CBUFFER_START( UnityPerMaterial )
			float _UseTexture;
			float _TipHeight;
			float4 _AlphaTexture_ST;
			float _MaxPlayerBend;
			float _WindSpeed;
			float4 _GroundColor;
			float4 _RootColor;
			float _GroundFade;
			float4 _TipColor;
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
				float4 ase_texcoord : TEXCOORD0;
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
                
				float4 appendResult331 = (float4(PlayerPosition.x , 0.0 , PlayerPosition.z , 0.0));
				float3 objToWorld311 = mul( GetObjectToWorldMatrix(), float4( v.vertex.xyz, 1 ) ).xyz;
				float3 appendResult330 = (float3(objToWorld311.x , 0.0 , objToWorld311.z));
				float temp_output_314_0 = distance( appendResult331 , float4( appendResult330 , 0.0 ) );
				float clampResult319 = clamp( temp_output_314_0 , 0.0 , 0.8 );
				float ifLocalVar323 = 0;
				if( temp_output_314_0 < 0.8 )
				ifLocalVar323 = ( 1.0 - clampResult319 );
				float4 break313 = ( float4( appendResult330 , 0.0 ) - appendResult331 );
				float4 appendResult320 = (float4(break313.x , 0.0 , break313.z , 0.0));
				float temp_output_247_0 = ( v.vertex.xyz.y / _TipHeight );
				float clampResult328 = clamp( temp_output_247_0 , 0.0 , 1.0 );
				float2 uv_AlphaTexture = v.ase_texcoord * _AlphaTexture_ST.xy + _AlphaTexture_ST.zw;
				float4 tex2DNode342 = tex2Dlod( _AlphaTexture, float4( uv_AlphaTexture, 0, 0.0) );
				float4 temp_cast_2 = (( 0.0 - _MaxPlayerBend )).xxxx;
				float4 temp_cast_3 = (_MaxPlayerBend).xxxx;
				float4 clampResult332 = clamp( ( ifLocalVar323 * appendResult320 * lerp(clampResult328,tex2DNode342.g,_UseTexture) * _MaxPlayerBend ) , temp_cast_2 , temp_cast_3 );
				float mulTime281 = _Time.y * (float)GamePausedInt;
				float3 objToWorld301 = mul( GetObjectToWorldMatrix(), float4( v.vertex.xyz, 1 ) ).xyz;
				float2 appendResult299 = (float2(objToWorld301.x , objToWorld301.z));
				float2 uv0279 = v.ase_texcoord.xy * float2( 1,1 ) + ( mulTime281 * appendResult299 * _WindSpeed * 0.01 );
				float simplePerlin2D294 = snoise( uv0279*0.2 );
				simplePerlin2D294 = simplePerlin2D294*0.5 + 0.5;
				float simplePerlin2D293 = snoise( uv0279*0.112 );
				simplePerlin2D293 = simplePerlin2D293*0.5 + 0.5;
				float3 appendResult292 = (float3(( simplePerlin2D294 - 0.5 ) , 0.0 , ( simplePerlin2D293 - 0.5 )));
				float clampResult362 = clamp( WindGust , 0.0 , 1.0 );
				float3 lerpResult361 = lerp( appendResult292 , float3(1,0,1) , clampResult362);
				float4 break335 = ( clampResult332 + float4( ( lerp(clampResult328,tex2DNode342.g,_UseTexture) * lerpResult361 * 0.2 ) , 0.0 ) );
				float2 appendResult337 = (float2(break335.x , break335.z));
				float3 appendResult339 = (float3(break335.x , ( 0.0 - distance( appendResult337 , float2( 0,0 ) ) ) , break335.z));
				
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = v.vertex.xyz;
				#else
				float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = appendResult339;
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
        		float2 uv_AlphaTexture = IN.ase_texcoord.xy * _AlphaTexture_ST.xy + _AlphaTexture_ST.zw;
        		float4 tex2DNode342 = tex2D( _AlphaTexture, uv_AlphaTexture );
        		

				float Alpha = tex2DNode342.r;
				float AlphaClipThreshold = 0.5;
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
            #define _AlphaClip 1


            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			float4 Lamps[16];
			float3 PlayerPosition;
			sampler2D _AlphaTexture;
			int GamePausedInt;
			half WindGust;
			CBUFFER_START( UnityPerMaterial )
			float _UseTexture;
			float _TipHeight;
			float4 _AlphaTexture_ST;
			float _MaxPlayerBend;
			float _WindSpeed;
			float4 _GroundColor;
			float4 _RootColor;
			float _GroundFade;
			float4 _TipColor;
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
				float4 ase_texcoord : TEXCOORD0;
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
					float4 appendResult331 = (float4(PlayerPosition.x , 0.0 , PlayerPosition.z , 0.0));
					float3 objToWorld311 = mul( GetObjectToWorldMatrix(), float4( v.vertex.xyz, 1 ) ).xyz;
					float3 appendResult330 = (float3(objToWorld311.x , 0.0 , objToWorld311.z));
					float temp_output_314_0 = distance( appendResult331 , float4( appendResult330 , 0.0 ) );
					float clampResult319 = clamp( temp_output_314_0 , 0.0 , 0.8 );
					float ifLocalVar323 = 0;
					if( temp_output_314_0 < 0.8 )
					ifLocalVar323 = ( 1.0 - clampResult319 );
					float4 break313 = ( float4( appendResult330 , 0.0 ) - appendResult331 );
					float4 appendResult320 = (float4(break313.x , 0.0 , break313.z , 0.0));
					float temp_output_247_0 = ( v.vertex.xyz.y / _TipHeight );
					float clampResult328 = clamp( temp_output_247_0 , 0.0 , 1.0 );
					float2 uv_AlphaTexture = v.ase_texcoord * _AlphaTexture_ST.xy + _AlphaTexture_ST.zw;
					float4 tex2DNode342 = tex2Dlod( _AlphaTexture, float4( uv_AlphaTexture, 0, 0.0) );
					float4 temp_cast_2 = (( 0.0 - _MaxPlayerBend )).xxxx;
					float4 temp_cast_3 = (_MaxPlayerBend).xxxx;
					float4 clampResult332 = clamp( ( ifLocalVar323 * appendResult320 * lerp(clampResult328,tex2DNode342.g,_UseTexture) * _MaxPlayerBend ) , temp_cast_2 , temp_cast_3 );
					float mulTime281 = _Time.y * (float)GamePausedInt;
					float3 objToWorld301 = mul( GetObjectToWorldMatrix(), float4( v.vertex.xyz, 1 ) ).xyz;
					float2 appendResult299 = (float2(objToWorld301.x , objToWorld301.z));
					float2 uv0279 = v.ase_texcoord.xy * float2( 1,1 ) + ( mulTime281 * appendResult299 * _WindSpeed * 0.01 );
					float simplePerlin2D294 = snoise( uv0279*0.2 );
					simplePerlin2D294 = simplePerlin2D294*0.5 + 0.5;
					float simplePerlin2D293 = snoise( uv0279*0.112 );
					simplePerlin2D293 = simplePerlin2D293*0.5 + 0.5;
					float3 appendResult292 = (float3(( simplePerlin2D294 - 0.5 ) , 0.0 , ( simplePerlin2D293 - 0.5 )));
					float clampResult362 = clamp( WindGust , 0.0 , 1.0 );
					float3 lerpResult361 = lerp( appendResult292 , float3(1,0,1) , clampResult362);
					float4 break335 = ( clampResult332 + float4( ( lerp(clampResult328,tex2DNode342.g,_UseTexture) * lerpResult361 * 0.2 ) , 0.0 ) );
					float2 appendResult337 = (float2(break335.x , break335.z));
					float3 appendResult339 = (float3(break335.x , ( 0.0 - distance( appendResult337 , float2( 0,0 ) ) ) , break335.z));
					
					o.ase_texcoord.xy = v.ase_texcoord.xy;
					
					//setting value to unused interpolator channels and avoid initialization warnings
					o.ase_texcoord.zw = 0;
					#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
					#else
					float3 defaultVertexValue = float3(0, 0, 0);
					#endif
					float3 vertexValue = appendResult339;	
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
				float2 uv_AlphaTexture = IN.ase_texcoord.xy * _AlphaTexture_ST.xy + _AlphaTexture_ST.zw;
				float4 tex2DNode342 = tex2D( _AlphaTexture, uv_AlphaTexture );
				

				float Alpha = tex2DNode342.r;
				float AlphaClipThreshold = 0.5;

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
0;0;1080;1859;-4407.648;3279.844;1.594384;True;False
Node;AmplifyShaderEditor.PosVertexDataNode;353;108.1376,-2194.1;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.IntNode;307;532.296,-2356.85;Float;False;Global;GamePausedInt;GamePausedInt;31;0;Create;True;0;0;False;0;1;1;0;1;INT;0
Node;AmplifyShaderEditor.TransformPositionNode;301;376.8652,-2249.2;Float;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PosVertexDataNode;309;1995.956,-2698.129;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;299;907.9495,-2149.5;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;302;692.0619,-2041.285;Float;False;Property;_WindSpeed;Wind Speed;5;0;Create;True;0;0;False;0;1;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;281;768.0249,-2360.212;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;305;667.609,-1959.818;Float;False;Constant;_Float1;Float 1;7;0;Create;True;0;0;False;0;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TransformPositionNode;311;2171.956,-2698.129;Float;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;308;2118.637,-3031.258;Float;False;Global;PlayerPosition;PlayerPosition;7;0;Create;True;0;0;False;0;0,0,0;268.3745,30.70081,-297.4908;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;298;1059.949,-2141.5;Float;False;4;4;0;FLOAT;0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;330;2435.66,-2615.86;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;331;2375.759,-2852.137;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PosVertexDataNode;260;-1241.806,-484.5206;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;334;2654.231,-3047.175;Float;False;Constant;_PlayerBendStart;Player Bend Start;7;0;Create;True;0;0;False;0;0.8;0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;246;-403.9371,-355.1981;Float;False;Property;_TipHeight;Tip Height;3;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;279;1261.576,-2252.489;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DistanceOpNode;314;2658.125,-2895.086;Float;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;247;-163.8698,-432.2054;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;319;2841.583,-2860.07;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;293;1627.634,-2067.226;Float;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;0.112;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;312;2706.125,-2703.086;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;294;1618.773,-2303.892;Float;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;304;1933.879,-2059.546;Float;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;354;1973.825,-1822.514;Half;False;Global;WindGust;WindGust;9;0;Create;True;0;0;False;0;0;0.2055036;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;313;2961.404,-2660.037;Float;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.OneMinusNode;318;2994.069,-2848.519;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;342;1715.352,-1534.791;Float;True;Property;_AlphaTexture;Alpha Texture;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;303;1943.827,-2291.2;Float;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;328;1791.596,-1735.554;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;350;2139.354,-1666.077;Float;False;Property;_UseTexture;Use Texture;8;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;292;2152.06,-2200.391;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;362;2184.115,-1824.638;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;359;1965.052,-1968.561;Float;False;Constant;_WindDirection;Wind Direction;9;0;Create;True;0;0;False;0;1,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ConditionalIfNode;323;3188.529,-3042.592;Float;False;False;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;329;2630.437,-2262.458;Float;False;Property;_MaxPlayerBend;Max Player Bend;7;0;Create;True;0;0;False;0;1.5;1.5;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;320;3229.384,-2660.394;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;349;3410.181,-2463.381;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;321;3425.373,-2602.541;Float;False;4;4;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;361;2390.115,-2004.638;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;297;2642.893,-1874.118;Float;False;Constant;_WindScale;Wind Scale;5;0;Create;True;0;0;False;0;0.2;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;291;2884.559,-1903.155;Float;True;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;332;3611.406,-2483.174;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;1,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;327;3856.207,-2252.644;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;335;4059.673,-2295.596;Float;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;337;4376.886,-2333.23;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DistanceOpNode;336;4613.446,-2335.982;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;338;4801.872,-2291.968;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;174;1653.536,-1077.111;Float;False;Global;AmbientColor;AmbientColor;10;0;Create;True;0;0;False;0;0.5,0.5,0.5,1;1,1,1,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;241;-37.04302,-724.7268;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;239;-638.3131,-1175.096;Float;False;Property;_TipColor;Tip Color;0;0;Create;True;0;0;False;0;0.2574063,0.3773585,0,0;0.2574063,0.3773585,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;240;-1186.46,-911.8824;Float;False;Property;_RootColor;Root Color;1;0;Create;True;0;0;False;0;0.1116701,0.245283,0,0;0.2574063,0.3773585,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;263;504.0464,-393.8358;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;191;2082.28,-592.3397;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;368;5190.848,-1653.48;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;370;2158.205,14.77029;Float;False;LampCheck;-1;;2;0cb7a14e6412b7e4da15d79b5462b235;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;252;-637.1942,-730.4189;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;371;1691.11,-473.0671;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.ColorNode;259;-1242.69,-689.2725;Float;False;Property;_GroundColor;Ground Color;2;0;Create;True;0;0;False;0;0.2470588,0.1525654,0,0;0.2574063,0.3773585,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMaxOpNode;245;-20.16986,-860.2543;Float;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;343;5087.277,-1960.433;Float;False;Constant;_Float2;Float 2;8;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;33;21.44477,-207.8682;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;365;3079.906,-778.6982;Float;False;Constant;_Float0;Float 0;9;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;831.3207,-590.3572;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;243;320.8004,-827.8781;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;253;-778.9216,-637.0591;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;251;-1159.813,-291.7218;Float;False;Property;_GroundFade;Ground Fade;4;0;Create;True;0;0;False;0;0.2;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;339;5013.539,-2263.006;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;254;-931.8976,-568.4446;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;244;-73.74204,-1041.578;Float;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;364;3305.287,-670.6238;Float;True;2;2;0;FLOAT;0;False;1;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;261;1119.351,-544.6166;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;366;2613.847,-695.8065;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;165;1857.76,-559.1481;Float;False;False;2;Float;ASEMaterialInspector;0;3;Hidden/Templates/LightWeightSRPUnlit;e2514bdcf5e5399499a9eb24d175b9db;True;ShadowCaster;0;1;ShadowCaster;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;166;1857.76,-559.1481;Float;False;False;2;Float;ASEMaterialInspector;0;3;Hidden/Templates/LightWeightSRPUnlit;e2514bdcf5e5399499a9eb24d175b9db;True;DepthOnly;0;2;DepthOnly;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=DepthOnly;True;0;0;Hidden/InternalErrorShader;0;0;Standard;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;164;5641.939,-2092.613;Float;False;True;2;Float;ASEMaterialInspector;0;3;Grass_Shader;e2514bdcf5e5399499a9eb24d175b9db;True;Base;0;0;Base;5;False;False;False;True;2;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;2;LightMode=LightweightForward;SplatCount=4;False;0;Hidden/InternalErrorShader;0;0;Standard;3;Vertex Position,InvertActionOnDeselection;1;Receive Shadows;1;Built-in Fog;0;0;3;True;True;True;False;5;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;0
WireConnection;301;0;353;0
WireConnection;299;0;301;1
WireConnection;299;1;301;3
WireConnection;281;0;307;0
WireConnection;311;0;309;0
WireConnection;298;0;281;0
WireConnection;298;1;299;0
WireConnection;298;2;302;0
WireConnection;298;3;305;0
WireConnection;330;0;311;1
WireConnection;330;2;311;3
WireConnection;331;0;308;1
WireConnection;331;2;308;3
WireConnection;279;1;298;0
WireConnection;314;0;331;0
WireConnection;314;1;330;0
WireConnection;247;0;260;2
WireConnection;247;1;246;0
WireConnection;319;0;314;0
WireConnection;319;2;334;0
WireConnection;293;0;279;0
WireConnection;312;0;330;0
WireConnection;312;1;331;0
WireConnection;294;0;279;0
WireConnection;304;0;293;0
WireConnection;313;0;312;0
WireConnection;318;0;319;0
WireConnection;303;0;294;0
WireConnection;328;0;247;0
WireConnection;350;0;328;0
WireConnection;350;1;342;2
WireConnection;292;0;303;0
WireConnection;292;2;304;0
WireConnection;362;0;354;0
WireConnection;323;0;314;0
WireConnection;323;1;334;0
WireConnection;323;4;318;0
WireConnection;320;0;313;0
WireConnection;320;2;313;2
WireConnection;349;1;329;0
WireConnection;321;0;323;0
WireConnection;321;1;320;0
WireConnection;321;2;350;0
WireConnection;321;3;329;0
WireConnection;361;0;292;0
WireConnection;361;1;359;0
WireConnection;361;2;362;0
WireConnection;291;0;350;0
WireConnection;291;1;361;0
WireConnection;291;2;297;0
WireConnection;332;0;321;0
WireConnection;332;1;349;0
WireConnection;332;2;329;0
WireConnection;327;0;332;0
WireConnection;327;1;291;0
WireConnection;335;0;327;0
WireConnection;337;0;335;0
WireConnection;337;1;335;2
WireConnection;336;0;337;0
WireConnection;338;1;336;0
WireConnection;241;0;252;0
WireConnection;241;1;239;0
WireConnection;241;2;247;0
WireConnection;263;0;33;0
WireConnection;191;0;174;0
WireConnection;191;1;371;0
WireConnection;191;2;370;0
WireConnection;368;0;364;0
WireConnection;252;0;259;0
WireConnection;252;1;240;0
WireConnection;252;2;253;0
WireConnection;245;0;239;0
WireConnection;245;1;252;0
WireConnection;9;0;243;0
WireConnection;9;1;263;0
WireConnection;243;0;241;0
WireConnection;243;1;244;0
WireConnection;243;2;245;0
WireConnection;253;0;254;0
WireConnection;339;0;335;0
WireConnection;339;1;338;0
WireConnection;339;2;335;2
WireConnection;254;0;260;2
WireConnection;254;1;251;0
WireConnection;244;0;239;0
WireConnection;244;1;252;0
WireConnection;364;0;365;0
WireConnection;364;1;366;0
WireConnection;261;0;9;0
WireConnection;366;0;191;0
WireConnection;366;1;261;0
WireConnection;164;0;368;0
WireConnection;164;1;342;1
WireConnection;164;2;343;0
WireConnection;164;3;339;0
ASEEND*/
//CHKSM=2188197F5BD739E37EBAE0903B78F5B5F11C8822