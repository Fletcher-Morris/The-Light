// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Zelda_Shader"
{
    Properties
    {
		_AlbedoTex("Albedo Tex", 2D) = "white" {}
		[HideInInspector]_Control("Control", 2D) = "white" {}
		[HideInInspector]_Splat3("Splat3", 2D) = "white" {}
		[HideInInspector]_Splat2("Splat2", 2D) = "white" {}
		[HideInInspector]_Splat1("Splat1", 2D) = "white" {}
		[HideInInspector]_Splat0("Splat0", 2D) = "white" {}
		[HideInInspector]_Smoothness3("Smoothness3", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness1("Smoothness1", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness0("Smoothness0", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness2("Smoothness2", Range( 0 , 1)) = 1
		_Color("Color", Color) = (1,1,1,0)
		_EmissionTex("Emission Tex", 2D) = "white" {}
		_EmissionStrength("Emission Strength", Float) = 0
		_Reflectivity("Reflectivity", Range( 0 , 1)) = 0
		[Normal]_NormalTex("Normal Tex", 2D) = "white" {}
		[Toggle]_UseNormalTex("Use Normal Tex", Float) = 0
		_SpecularTex("Specular Tex", 2D) = "white" {}
		_SpecularSoftness("Specular Softness", Range( 0.01 , 0.5)) = 0.01
		_SpecularIntensity("Specular Intensity", Range( 0 , 5)) = 2
		_ShineClamp("Shine Clamp", Range( 0 , 1)) = 1
		[Toggle]_Terrain("Terrain", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
    }

    SubShader
    {
		

        Tags { "RenderPipeline"="LightweightPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
        Cull Off
		HLSLINCLUDE
		#pragma target 3.0
		ENDHLSL

		UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
	UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"

        Pass
        {
            Tags { "LightMode"="LightweightForward" }
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
            #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap


            // Lighting include is needed because of GI
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/Shaders/UnlitInput.hlsl"

			float4 LampPositionsArray[16];
			float LampRangesArray[16];
			sampler2D _AlbedoTex;
			sampler2D _Control;
			sampler2D _Splat0;
			sampler2D _Splat1;
			sampler2D _Splat2;
			sampler2D _Splat3;
			sampler2D _NormalTex;
			sampler2D _SpecularTex;
			sampler2D _EmissionTex;
			float4 AmbientColor;
			int LampCount;
			#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
				TEXTURE2D(_TerrainHeightmapTexture);//ASE Terrain Instancing
				TEXTURE2D( _TerrainNormalmapTexture);//ASE Terrain Instancing
			#endif//ASE Terrain Instancing
			UNITY_INSTANCING_BUFFER_START( Terrain )//ASE Terrain Instancing
				UNITY_DEFINE_INSTANCED_PROP( float4, _TerrainPatchInstanceData )//ASE Terrain Instancing
			UNITY_INSTANCING_BUFFER_END( Terrain)//ASE Terrain Instancing
			CBUFFER_START( UnityTerrain)//ASE Terrain Instancing
				#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
					float4 _TerrainHeightmapRecipSize;//ASE Terrain Instancing
					float4 _TerrainHeightmapScale;//ASE Terrain Instancing
				#endif//ASE Terrain Instancing
			CBUFFER_END//ASE Terrain Instancing
			CBUFFER_START( UnityPerMaterial )
			float _ShineClamp;
			float4 _Color;
			float _Terrain;
			float4 _AlbedoTex_ST;
			float4 _Control_ST;
			float _Smoothness0;
			float4 _Splat0_ST;
			float _Smoothness1;
			float4 _Splat1_ST;
			float _Smoothness2;
			float4 _Splat2_ST;
			float _Smoothness3;
			float4 _Splat3_ST;
			float _UseNormalTex;
			float4 _NormalTex_ST;
			float _SpecularSoftness;
			float _SpecularIntensity;
			float4 _EmissionTex_ST;
			float _EmissionStrength;
			float _Reflectivity;
			CBUFFER_END

            struct GraphVertexInput
            {
                float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
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
				float3 ase_normal : NORMAL;
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
			
			GraphVertexInput ApplyMeshModification( GraphVertexInput v )
			{
			#ifdef UNITY_INSTANCING_ENABLED
				float2 patchVertex = v.vertex.xy;
					float4 instanceData = UNITY_ACCESS_INSTANCED_PROP( Terrain, _TerrainPatchInstanceData );
					float2 sampleCoords = ( patchVertex.xy + instanceData.xy ) * instanceData.z;
					float height = UnpackHeightmap( _TerrainHeightmapTexture.Load( int3( sampleCoords, 0 ) ) );
					v.vertex.xz = sampleCoords* _TerrainHeightmapScale.xz;
					v.vertex.y = height* _TerrainHeightmapScale.y;
			# ifdef ATTRIBUTES_NEED_NORMAL
					v.ase_normal = _TerrainNormalmapTexture.Load(int3(sampleCoords, 0)).rgb* 2 - 1;
				#endif
				#if defined(VARYINGS_NEED_TEXCOORD0) || defined(VARYINGS_DS_NEED_TEXCOORD0)
					#ifdef ENABLE_TERRAIN_PERPIXEL_NORMAL
						v.ase_texcoord = sampleCoords;
					#else
						v.ase_texcoord.xy = sampleCoords* _TerrainHeightmapRecipSize.zw;
					#endif
				#endif
			#endif
			#ifdef ATTRIBUTES_NEED_TANGENT
					v.ase_tangent.xyz = cross( v.ase_normal, float3(0, 0, 1));
				v.ase_tangent.w = -1;
			#endif
				return v;
			}
			

            GraphVertexOutput vert (GraphVertexInput v)
            {
                GraphVertexOutput o = (GraphVertexOutput)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				v = ApplyMeshModification(v);
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				o.ase_texcoord1.xyz = ase_worldPos;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord2.xyz = ase_worldNormal;
				VertexPositionInputs ase_vertexInput = GetVertexPositionInputs (v.vertex.xyz);
				#ifdef _MAIN_LIGHT_SHADOWS//ase_lightAtten_vert
				o.ase_texcoord4 = GetShadowCoord( ase_vertexInput );
				#endif//ase_lightAtten_vert
				
				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				o.ase_normal = v.ase_normal;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.w = 0;
				o.ase_texcoord2.w = 0;
				o.ase_texcoord3.zw = 0;
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

            half4 frag (GraphVertexOutput IN ) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
				float4 transform183 = mul(GetWorldToObjectMatrix(),float4( SafeNormalize(_MainLightPosition.xyz) , 0.0 ));
				float4 normalizeResult29 = normalize( transform183 );
				float3 ase_worldPos = IN.ase_texcoord1.xyz;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float dotResult73 = dot( ( -1.0 * normalizeResult29 ) , float4( ase_worldViewDir , 0.0 ) );
				float temp_output_67_0 = ( ( 1.0 - saturate( ( dotResult73 + 0.2 ) ) ) + 0.2 );
				float3 ase_worldNormal = IN.ase_texcoord2.xyz;
				float fresnelNdotV69 = dot( ase_worldNormal, ase_worldViewDir );
				float fresnelNode69 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV69, 4.0 ) );
				float temp_output_11_0 = saturate( ( step( ( temp_output_67_0 + 0.2 ) , fresnelNode69 ) * 2.0 ) );
				float clampResult179 = clamp( temp_output_11_0 , 0.0 , _ShineClamp );
				float2 uv_AlbedoTex = IN.ase_texcoord3.xy * _AlbedoTex_ST.xy + _AlbedoTex_ST.zw;
				float2 uv_Control = IN.ase_texcoord3.xy * _Control_ST.xy + _Control_ST.zw;
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
				float2 uv_Splat0 = IN.ase_texcoord3.xy * _Splat0_ST.xy + _Splat0_ST.zw;
				float4 appendResult36_g1 = (float4(1.0 , 1.0 , 1.0 , _Smoothness1));
				float2 uv_Splat1 = IN.ase_texcoord3.xy * _Splat1_ST.xy + _Splat1_ST.zw;
				float4 appendResult39_g1 = (float4(1.0 , 1.0 , 1.0 , _Smoothness2));
				float2 uv_Splat2 = IN.ase_texcoord3.xy * _Splat2_ST.xy + _Splat2_ST.zw;
				float4 appendResult42_g1 = (float4(1.0 , 1.0 , 1.0 , _Smoothness3));
				float2 uv_Splat3 = IN.ase_texcoord3.xy * _Splat3_ST.xy + _Splat3_ST.zw;
				float4 weightedBlendVar9_g1 = temp_output_59_0_g1;
				float4 weightedBlend9_g1 = ( weightedBlendVar9_g1.x*( appendResult33_g1 * tex2D( _Splat0, uv_Splat0 ) ) + weightedBlendVar9_g1.y*( appendResult36_g1 * tex2D( _Splat1, uv_Splat1 ) ) + weightedBlendVar9_g1.z*( appendResult39_g1 * tex2D( _Splat2, uv_Splat2 ) ) + weightedBlendVar9_g1.w*( appendResult42_g1 * tex2D( _Splat3, uv_Splat3 ) ) );
				float4 MixDiffuse28_g1 = weightedBlend9_g1;
				float clampResult180 = clamp( step( temp_output_67_0 , fresnelNode69 ) , 0.0 , _ShineClamp );
				float ase_lightAtten = 0;
				Light ase_lightAtten_mainLight = GetMainLight( IN.ase_texcoord4 );
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
				float temp_output_27_0 = step( 0.7 , ( ase_lightAtten + 0.0 ) );
				float4 color84 = IsGammaSpace() ? float4(0,0,1,0) : float4(0,0,1,0);
				float2 uv_NormalTex = IN.ase_texcoord3.xy * _NormalTex_ST.xy + _NormalTex_ST.zw;
				float3 temp_output_79_0 = BlendNormal( lerp(color84,tex2D( _NormalTex, uv_NormalTex ),_UseNormalTex).rgb , IN.ase_normal );
				float dotResult98 = dot( normalizeResult29 , float4( temp_output_79_0 , 0.0 ) );
				float smoothstepResult116 = smoothstep( 0.29 , 0.3 , ( temp_output_27_0 * dotResult98 ));
				float4 temp_cast_12 = (_SpecularSoftness).xxxx;
				float4 normalizeResult77 = normalize( ( normalizeResult29 + float4( ase_worldViewDir , 0.0 ) ) );
				float dotResult78 = dot( normalizeResult77 , float4( temp_output_79_0 , 0.0 ) );
				float2 temp_cast_16 = (( 50.0 / 2.0 )).xx;
				float2 uv0107 = IN.ase_texcoord3.xy * temp_cast_16 + float2( 0,0 );
				float cos105 = cos( 0.5 );
				float sin105 = sin( 0.5 );
				float2 rotator105 = mul( uv0107 - float2( 0,0 ) , float2x2( cos105 , -sin105 , sin105 , cos105 )) + float2( 0,0 );
				float4 smoothstepResult109 = smoothstep( float4( 0.01,0,0,0 ) , temp_cast_12 , ( ( ( temp_output_27_0 * dotResult78 ) + (-1.0 + (0.5 - 0.0) * (-0.9 - -1.0) / (1.0 - 0.0)) ) * tex2D( _SpecularTex, rotator105 ) ));
				float2 uv_EmissionTex = IN.ase_texcoord3.xy * _EmissionTex_ST.xy + _EmissionTex_ST.zw;
				float4 lerpResult1 = lerp( ( ( ( clampResult179 + ( 1.5 * ( ( ( _Color * lerp(tex2D( _AlbedoTex, uv_AlbedoTex ),MixDiffuse28_g1,_Terrain) ) * ( _MainLightColor + float4( 0,0,0,0 ) ) ) * ( ( clampResult180 + (0.2 + (smoothstepResult116 - 0.0) * (0.9 - 0.2) / (1.0 - 0.0)) ) + ( smoothstepResult116 * ( smoothstepResult109 * _SpecularIntensity * _SpecularIntensity ) ) ) ) ) ) * ( 1.0 + ( tex2D( _EmissionTex, uv_EmissionTex ) * _EmissionStrength ) ) ) * AmbientColor ) , float4( 0,0,0,0 ) , _Reflectivity);
				float4 break149 = lerpResult1;
				float4 temp_cast_18 = (( ( break149.r + break149.g + break149.b ) / 10.0 )).xxxx;
				float3 WorldPos185 = ase_worldPos;
				float ArraySize185 = (float)LampCount;
				int localLampCheck185 = LampCheck( WorldPos185 , ArraySize185 );
				float4 lerpResult191 = lerp( temp_cast_18 , lerpResult1 , (float)localLampCheck185);
				
		        float3 Color = lerpResult191.rgb;
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

		UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
	UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"

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
            #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap


            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			float4 LampPositionsArray[16];
			float LampRangesArray[16];
			#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
				TEXTURE2D(_TerrainHeightmapTexture);//ASE Terrain Instancing
				TEXTURE2D( _TerrainNormalmapTexture);//ASE Terrain Instancing
			#endif//ASE Terrain Instancing
			UNITY_INSTANCING_BUFFER_START( Terrain )//ASE Terrain Instancing
				UNITY_DEFINE_INSTANCED_PROP( float4, _TerrainPatchInstanceData )//ASE Terrain Instancing
			UNITY_INSTANCING_BUFFER_END( Terrain)//ASE Terrain Instancing
			CBUFFER_START( UnityTerrain)//ASE Terrain Instancing
				#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
					float4 _TerrainHeightmapRecipSize;//ASE Terrain Instancing
					float4 _TerrainHeightmapScale;//ASE Terrain Instancing
				#endif//ASE Terrain Instancing
			CBUFFER_END//ASE Terrain Instancing
			CBUFFER_START( UnityPerMaterial )
			float _ShineClamp;
			float4 _Color;
			float _Terrain;
			float4 _AlbedoTex_ST;
			float4 _Control_ST;
			float _Smoothness0;
			float4 _Splat0_ST;
			float _Smoothness1;
			float4 _Splat1_ST;
			float _Smoothness2;
			float4 _Splat2_ST;
			float _Smoothness3;
			float4 _Splat3_ST;
			float _UseNormalTex;
			float4 _NormalTex_ST;
			float _SpecularSoftness;
			float _SpecularIntensity;
			float4 _EmissionTex_ST;
			float _EmissionStrength;
			float _Reflectivity;
			CBUFFER_END

            struct GraphVertexInput
            {
                float4 vertex : POSITION;
                float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
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

			GraphVertexInput ApplyMeshModification( GraphVertexInput v )
			{
			#ifdef UNITY_INSTANCING_ENABLED
				float2 patchVertex = v.vertex.xy;
					float4 instanceData = UNITY_ACCESS_INSTANCED_PROP( Terrain, _TerrainPatchInstanceData );
					float2 sampleCoords = ( patchVertex.xy + instanceData.xy ) * instanceData.z;
					float height = UnpackHeightmap( _TerrainHeightmapTexture.Load( int3( sampleCoords, 0 ) ) );
					v.vertex.xz = sampleCoords* _TerrainHeightmapScale.xz;
					v.vertex.y = height* _TerrainHeightmapScale.y;
			# ifdef ATTRIBUTES_NEED_NORMAL
					v.ase_normal = _TerrainNormalmapTexture.Load(int3(sampleCoords, 0)).rgb* 2 - 1;
				#endif
				#if defined(VARYINGS_NEED_TEXCOORD0) || defined(VARYINGS_DS_NEED_TEXCOORD0)
					#ifdef ENABLE_TERRAIN_PERPIXEL_NORMAL
						v.ase_texcoord = sampleCoords;
					#else
						v.ase_texcoord.xy = sampleCoords* _TerrainHeightmapRecipSize.zw;
					#endif
				#endif
			#endif
			#ifdef ATTRIBUTES_NEED_TANGENT
					v.ase_tangent.xyz = cross( v.ase_normal, float3(0, 0, 1));
				v.ase_tangent.w = -1;
			#endif
				return v;
			}
			

            VertexOutput ShadowPassVertex(GraphVertexInput v )
            {
                VertexOutput o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
				v = ApplyMeshModification(v);
				o.ase_texcoord = v.ase_texcoord;
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

		UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
	UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"

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
            #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap


            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			float4 LampPositionsArray[16];
			float LampRangesArray[16];
			#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
				TEXTURE2D(_TerrainHeightmapTexture);//ASE Terrain Instancing
				TEXTURE2D( _TerrainNormalmapTexture);//ASE Terrain Instancing
			#endif//ASE Terrain Instancing
			UNITY_INSTANCING_BUFFER_START( Terrain )//ASE Terrain Instancing
				UNITY_DEFINE_INSTANCED_PROP( float4, _TerrainPatchInstanceData )//ASE Terrain Instancing
			UNITY_INSTANCING_BUFFER_END( Terrain)//ASE Terrain Instancing
			CBUFFER_START( UnityTerrain)//ASE Terrain Instancing
				#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
					float4 _TerrainHeightmapRecipSize;//ASE Terrain Instancing
					float4 _TerrainHeightmapScale;//ASE Terrain Instancing
				#endif//ASE Terrain Instancing
			CBUFFER_END//ASE Terrain Instancing
			CBUFFER_START( UnityPerMaterial )
			float _ShineClamp;
			float4 _Color;
			float _Terrain;
			float4 _AlbedoTex_ST;
			float4 _Control_ST;
			float _Smoothness0;
			float4 _Splat0_ST;
			float _Smoothness1;
			float4 _Splat1_ST;
			float _Smoothness2;
			float4 _Splat2_ST;
			float _Smoothness3;
			float4 _Splat3_ST;
			float _UseNormalTex;
			float4 _NormalTex_ST;
			float _SpecularSoftness;
			float _SpecularIntensity;
			float4 _EmissionTex_ST;
			float _EmissionStrength;
			float _Reflectivity;
			CBUFFER_END

			struct GraphVertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

            struct VertexOutput
            {
                float4 clipPos : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

			GraphVertexInput ApplyMeshModification( GraphVertexInput v )
			{
			#ifdef UNITY_INSTANCING_ENABLED
				float2 patchVertex = v.vertex.xy;
					float4 instanceData = UNITY_ACCESS_INSTANCED_PROP( Terrain, _TerrainPatchInstanceData );
					float2 sampleCoords = ( patchVertex.xy + instanceData.xy ) * instanceData.z;
					float height = UnpackHeightmap( _TerrainHeightmapTexture.Load( int3( sampleCoords, 0 ) ) );
					v.vertex.xz = sampleCoords* _TerrainHeightmapScale.xz;
					v.vertex.y = height* _TerrainHeightmapScale.y;
			# ifdef ATTRIBUTES_NEED_NORMAL
					v.ase_normal = _TerrainNormalmapTexture.Load(int3(sampleCoords, 0)).rgb* 2 - 1;
				#endif
				#if defined(VARYINGS_NEED_TEXCOORD0) || defined(VARYINGS_DS_NEED_TEXCOORD0)
					#ifdef ENABLE_TERRAIN_PERPIXEL_NORMAL
						v.ase_texcoord = sampleCoords;
					#else
						v.ase_texcoord.xy = sampleCoords* _TerrainHeightmapRecipSize.zw;
					#endif
				#endif
			#endif
			#ifdef ATTRIBUTES_NEED_TANGENT
					v.ase_tangent.xyz = cross( v.ase_normal, float3(0, 0, 1));
				v.ase_tangent.w = -1;
			#endif
				return v;
			}
			

			VertexOutput vert( GraphVertexInput v  )
			{
					VertexOutput o = (VertexOutput)0;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					v = ApplyMeshModification(v);
					o.ase_texcoord = v.ase_texcoord;
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
272;73;1254;666;258.0368;477.8413;1.713486;True;False
Node;AmplifyShaderEditor.BlendNormalsNode;79;-3665.215,398.4698;Float;True;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;75;-4036.861,-521.2124;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PosVertexDataNode;196;371.9865,-99.33754;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;27;-3285.052,-457.8881;Float;False;2;0;FLOAT;0.7;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;48;-4270.135,-285.4797;Float;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-3640.095,-1194.264;Float;False;2;2;0;FLOAT;-1;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;176;-2328.252,-652.0792;Float;False;Property;_ShineClamp;Shine Clamp;28;0;Create;True;0;0;False;0;1;0.084;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;13;-1937.978,-883.079;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;174;-523.9769,-578.3466;Float;False;Global;AmbientColor;AmbientColor;10;0;Create;True;0;0;False;0;0.5,0.5,0.5,1;1,1,1,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;68;-2202.393,-1146.777;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-2449.776,-8.308884;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;71;-3003.094,-1164.264;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;29;-3664.876,-268.4449;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LightAttenuation;33;-4392.936,-435.0798;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-867.4999,-456.0999;Float;False;2;2;0;FLOAT;1.5;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;149;325.6735,-472.7571;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;115;-1576.99,-13.53461;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalizeNode;77;-3588.408,100.5101;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WorldPosInputsNode;197;719.5444,-102.0008;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;-990.1639,-335.3752;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;104;-3159.465,540.7137;Float;True;Property;_SpecularTex;Specular Tex;25;0;Create;True;0;0;False;0;826e9d3791e89174dbf3097b7e192aa1;826e9d3791e89174dbf3097b7e192aa1;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;120;-3811.941,-1244.104;Float;False;Constant;_Float3;Float 3;9;0;Create;True;0;0;False;0;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GlobalArrayNode;186;349.9993,237.8757;Float;False;LampRangesArray;0;16;0;False;False;0;1;True;Object;186;4;0;INT;0;False;2;INT;0;False;1;INT;0;False;3;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;179;-1012.145,-668.1226;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;185;1350.298,236.1264;Float;False;for(int i = 0@ i < ArraySize@ i++)${$	float d = distance(WorldPos, LampPositionsArray[i])@$	if(d < LampRangesArray[i])$	{$		return 1@$	}$}$return 0@;0;False;2;True;WorldPos;FLOAT3;0,0,0;In;;Float;False;True;ArraySize;FLOAT;0;In;;Float;False;Lamp Check;False;False;0;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;INT;0
Node;AmplifyShaderEditor.TFHCRemapNode;24;-1991.826,129.8219;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.2;False;4;FLOAT;0.9;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-256,-336;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;129;-3260.589,996.5017;Float;False;Constant;_Float4;Float 4;9;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;73;-3430.095,-1185.264;Float;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;100;-2804.795,125.5323;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;76;-3803.838,99.93349;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-1424,-288;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;96;-4136.192,419.411;Float;False;Property;_UseNormalTex;Use Normal Tex;24;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;103;-2734.465,610.7137;Float;True;Property;_TextureSample3;Texture Sample 3;8;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;1;87.96389,-267.9493;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;11;-1456.139,-892.476;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;107;-3366.465,712.7137;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-1787.286,-833.654;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-547.5214,-342.24;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;78;-3342.529,82.20843;Float;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-2381.026,456.5851;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;181;-2475.964,-477.9981;Float;False;Four Splats First Pass Terrain;1;;1;37452fdfb732e1443b7e39720d05b708;0;6;59;FLOAT4;0,0,0,0;False;60;FLOAT4;0,0,0,0;False;61;FLOAT3;0,0,0;False;57;FLOAT;0;False;58;FLOAT;0;False;62;FLOAT;0;False;6;FLOAT4;0;FLOAT3;14;FLOAT;56;FLOAT;45;FLOAT;19;FLOAT3;17
Node;AmplifyShaderEditor.IntNode;201;28.83962,284.8891;Float;False;Global;LampCount;LampCount;13;0;Create;True;0;0;True;0;1;0;0;1;INT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-756.1568,-506.9803;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;118;-1057.275,-485.8297;Float;False;Constant;_Float0;Float 0;9;0;Create;True;0;0;False;0;1.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;64;-1801.204,-525.4883;Float;True;Property;_TextureSample1;Texture Sample 1;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;108;-3593.465,794.7137;Float;True;2;0;FLOAT;50;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;70;-2750.094,-1168.264;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;110;-2357.375,574.9367;Float;False;Property;_SpecularSoftness;Specular Softness;26;0;Create;True;0;0;False;0;0.01;0.1;0.01;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;83;-4902.039,216.146;Float;True;Property;_NormalTex;Normal Tex;23;1;[Normal];Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.ToggleSwitchNode;182;-1848.216,-272.6653;Float;False;Property;_Terrain;Terrain;29;0;Create;True;0;0;False;0;0;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;112;-2257.677,747.5365;Float;False;Property;_SpecularIntensity;Specular Intensity;27;0;Create;True;0;0;False;0;2;2;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-3040.678,89.61349;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;131;-1708.78,-712.0515;Float;False;Property;_Color;Color;19;0;Create;True;0;0;False;0;1,1,1,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;178;-1056.677,-781.2553;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-799.3176,170.259;Float;True;2;2;0;FLOAT;1;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;66;-2179.641,-877.7505;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;17;-1474.059,223.721;Float;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;148;613.3585,-467.6198;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;49;-4268.935,-130.0796;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SmoothstepOpNode;116;-2205.494,171.9221;Float;False;3;0;FLOAT;0.29;False;1;FLOAT;0.29;False;2;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-1058.059,397.721;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;98;-2772.142,-327.7293;Float;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;81;-4626.635,228.9198;Float;True;Property;_TextureSample2;Texture Sample 2;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;-1803.375,593.9367;Float;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;69;-2574.61,-878.5747;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-1297.144,-489.8509;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GlobalArrayNode;184;680.1918,149.7331;Float;False;LampPositionsArray;0;16;2;False;False;0;1;True;Object;184;4;0;INT;0;False;2;INT;0;False;1;INT;0;False;3;INT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;191;1406.44,-347.4988;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-272,64;Float;False;Property;_Reflectivity;Reflectivity;22;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;72;-3199.094,-1178.264;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1389.059,474.721;Float;False;Property;_EmissionStrength;Emission Strength;21;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;168;-4114.369,-376.705;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;119;-1011.938,139.96;Float;False;Constant;_Float1;Float 1;9;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;101;-3125.827,304.9964;Float;False;5;0;FLOAT;0.5;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;-0.9;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;18;-1705.132,223.2666;Float;True;Property;_EmissionTex;Emission Tex;20;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.ColorNode;84;-4508.885,514.5595;Float;False;Constant;_Color0;Color 0;7;0;Create;True;0;0;False;0;0,0,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-1443.716,-540.1296;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RotatorNode;105;-3059.465,772.7137;Float;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0.105;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ClampOpNode;180;-1910.67,-696.4648;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;65;-2037.269,-525.9251;Float;True;Property;_AlbedoTex;Albedo Tex;0;0;Create;True;0;0;False;0;None;f3fe3e58d878dd848b1b93f1a193a19f;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.WorldToObjectTransfNode;183;-3984.854,-259.7147;Float;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;109;-2055.375,482.9367;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.01,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;67;-2478.393,-1138.777;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;80;-4012.418,568.1475;Float;True;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;22;-1214.105,-127.4181;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;150;998.6505,-474.4695;Float;False;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;170;-4032.421,-81.04968;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;165;1857.76,-559.1481;Float;False;False;2;Float;ASEMaterialInspector;0;3;Hidden/Templates/LightWeightSRPUnlit;e2514bdcf5e5399499a9eb24d175b9db;True;ShadowCaster;0;1;ShadowCaster;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;164;1857.76,-559.1481;Float;False;True;2;Float;ASEMaterialInspector;0;3;Zelda_Shader;e2514bdcf5e5399499a9eb24d175b9db;True;Base;0;0;Base;5;False;False;False;True;2;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=LightweightForward;False;0;Hidden/InternalErrorShader;0;0;Standard;3;Vertex Position,InvertActionOnDeselection;1;Receive Shadows;1;Built-in Fog;0;0;3;True;True;True;True;5;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;166;1857.76,-559.1481;Float;False;False;2;Float;ASEMaterialInspector;0;3;Hidden/Templates/LightWeightSRPUnlit;e2514bdcf5e5399499a9eb24d175b9db;True;DepthOnly;0;2;DepthOnly;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=DepthOnly;True;0;0;Hidden/InternalErrorShader;0;0;Standard;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;0
WireConnection;79;0;96;0
WireConnection;79;1;80;0
WireConnection;27;1;168;0
WireConnection;74;0;120;0
WireConnection;74;1;29;0
WireConnection;13;0;68;0
WireConnection;13;1;69;0
WireConnection;68;0;67;0
WireConnection;26;0;27;0
WireConnection;26;1;98;0
WireConnection;71;0;72;0
WireConnection;29;0;183;0
WireConnection;19;0;118;0
WireConnection;19;1;114;0
WireConnection;149;0;1;0
WireConnection;115;0;116;0
WireConnection;115;1;111;0
WireConnection;77;0;76;0
WireConnection;114;0;21;0
WireConnection;114;1;22;0
WireConnection;179;0;11;0
WireConnection;179;2;176;0
WireConnection;185;0;197;0
WireConnection;185;1;201;0
WireConnection;24;0;116;0
WireConnection;4;0;9;0
WireConnection;4;1;174;0
WireConnection;73;0;74;0
WireConnection;73;1;75;0
WireConnection;100;0;99;0
WireConnection;100;1;101;0
WireConnection;76;0;29;0
WireConnection;76;1;75;0
WireConnection;23;0;180;0
WireConnection;23;1;24;0
WireConnection;96;0;84;0
WireConnection;96;1;81;0
WireConnection;103;0;104;0
WireConnection;103;1;105;0
WireConnection;1;0;4;0
WireConnection;1;2;3;0
WireConnection;11;0;12;0
WireConnection;107;0;108;0
WireConnection;12;0;13;0
WireConnection;9;0;10;0
WireConnection;9;1;14;0
WireConnection;78;0;77;0
WireConnection;78;1;79;0
WireConnection;102;0;100;0
WireConnection;102;1;103;0
WireConnection;10;0;179;0
WireConnection;10;1;19;0
WireConnection;64;0;65;0
WireConnection;70;0;71;0
WireConnection;182;0;64;0
WireConnection;182;1;181;0
WireConnection;99;0;27;0
WireConnection;99;1;78;0
WireConnection;178;0;11;0
WireConnection;178;1;176;0
WireConnection;14;0;119;0
WireConnection;14;1;15;0
WireConnection;66;0;67;0
WireConnection;66;1;69;0
WireConnection;17;0;18;0
WireConnection;148;0;149;0
WireConnection;148;1;149;1
WireConnection;148;2;149;2
WireConnection;116;0;26;0
WireConnection;15;0;17;0
WireConnection;15;1;16;0
WireConnection;98;0;29;0
WireConnection;98;1;79;0
WireConnection;81;0;83;0
WireConnection;111;0;109;0
WireConnection;111;1;112;0
WireConnection;111;2;112;0
WireConnection;21;0;62;0
WireConnection;21;1;170;0
WireConnection;191;0;150;0
WireConnection;191;1;1;0
WireConnection;191;2;185;0
WireConnection;72;0;73;0
WireConnection;168;0;33;0
WireConnection;62;0;131;0
WireConnection;62;1;182;0
WireConnection;105;0;107;0
WireConnection;105;2;129;0
WireConnection;180;0;66;0
WireConnection;180;2;176;0
WireConnection;183;0;48;0
WireConnection;109;0;102;0
WireConnection;109;2;110;0
WireConnection;67;0;70;0
WireConnection;22;0;23;0
WireConnection;22;1;115;0
WireConnection;150;0;148;0
WireConnection;170;0;49;0
WireConnection;164;0;191;0
ASEEND*/
//CHKSM=8C0CE58B70F024BAC0CF8D21E0A32F1B4941156D