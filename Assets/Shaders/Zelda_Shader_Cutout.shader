// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Zelda_Shader_Cutout"
{
	Properties
	{
		_AlbedoTex("Albedo Tex", 2D) = "white" {}
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
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
			float3 viewDir;
			float3 worldNormal;
			float2 uv_texcoord;
		};

		uniform float3 LampPosition;
		uniform float LampRange;
		uniform float _ShineClamp;
		uniform float4 _Color;
		uniform sampler2D _AlbedoTex;
		uniform float4 _AlbedoTex_ST;
		uniform float _UseNormalTex;
		uniform sampler2D _NormalTex;
		uniform float4 _NormalTex_ST;
		uniform float _SpecularSoftness;
		uniform sampler2D _SpecularTex;
		uniform float _SpecularIntensity;
		uniform sampler2D _EmissionTex;
		uniform float4 _EmissionTex_ST;
		uniform float _EmissionStrength;
		uniform float4 AmbientColor;
		uniform float _Reflectivity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 normalizeResult29 = normalize( ase_worldlightDir );
			float dotResult73 = dot( ( -1.0 * normalizeResult29 ) , i.viewDir );
			float temp_output_67_0 = ( ( 1.0 - saturate( ( dotResult73 + 0.2 ) ) ) + 0.2 );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV69 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode69 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV69, 4.0 ) );
			float temp_output_11_0 = saturate( ( step( ( temp_output_67_0 + 0.2 ) , fresnelNode69 ) * 2.0 ) );
			float clampResult179 = clamp( temp_output_11_0 , 0.0 , _ShineClamp );
			float2 uv_AlbedoTex = i.uv_texcoord * _AlbedoTex_ST.xy + _AlbedoTex_ST.zw;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float clampResult180 = clamp( step( temp_output_67_0 , fresnelNode69 ) , 0.0 , _ShineClamp );
			float temp_output_27_0 = step( 0.7 , ( 1 + 0.0 ) );
			float4 color84 = IsGammaSpace() ? float4(0,0,1,0) : float4(0,0,1,0);
			float2 uv_NormalTex = i.uv_texcoord * _NormalTex_ST.xy + _NormalTex_ST.zw;
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float3 temp_output_79_0 = BlendNormals( lerp(color84,tex2D( _NormalTex, uv_NormalTex ),_UseNormalTex).rgb , ase_vertexNormal );
			float dotResult98 = dot( normalizeResult29 , temp_output_79_0 );
			float smoothstepResult116 = smoothstep( 0.29 , 0.3 , ( temp_output_27_0 * dotResult98 ));
			float4 temp_cast_1 = (_SpecularSoftness).xxxx;
			float3 normalizeResult77 = normalize( ( normalizeResult29 + i.viewDir ) );
			float dotResult78 = dot( normalizeResult77 , temp_output_79_0 );
			float2 temp_cast_3 = (( 50.0 / 2.0 )).xx;
			float2 uv_TexCoord107 = i.uv_texcoord * temp_cast_3;
			float cos105 = cos( 0.5 );
			float sin105 = sin( 0.5 );
			float2 rotator105 = mul( uv_TexCoord107 - float2( 0,0 ) , float2x2( cos105 , -sin105 , sin105 , cos105 )) + float2( 0,0 );
			float4 smoothstepResult109 = smoothstep( float4( 0.01,0,0,0 ) , temp_cast_1 , ( ( ( temp_output_27_0 * dotResult78 ) + (-1.0 + (0.5 - 0.0) * (-0.9 - -1.0) / (1.0 - 0.0)) ) * tex2D( _SpecularTex, rotator105 ) ));
			float2 uv_EmissionTex = i.uv_texcoord * _EmissionTex_ST.xy + _EmissionTex_ST.zw;
			float4 lerpResult1 = lerp( ( ( ( clampResult179 + ( 1.5 * ( ( ( _Color * tex2D( _AlbedoTex, uv_AlbedoTex ) ) * ( ase_lightColor + float4( 0,0,0,0 ) ) ) * ( ( clampResult180 + (0.2 + (smoothstepResult116 - 0.0) * (0.9 - 0.2) / (1.0 - 0.0)) ) + ( smoothstepResult116 * ( smoothstepResult109 * _SpecularIntensity * _SpecularIntensity ) ) ) ) ) ) * ( 1.0 + ( tex2D( _EmissionTex, uv_EmissionTex ) * _EmissionStrength ) ) ) * AmbientColor ) , float4( 0,0,0,0 ) , _Reflectivity);
			float4 break149 = lerpResult1;
			float4 temp_cast_5 = (( ( break149.r + break149.g + break149.b ) / 10.0 )).xxxx;
			float4 ifLocalVar143 = 0;
			if( distance( LampPosition , ase_worldPos ) <= LampRange )
				ifLocalVar143 = lerpResult1;
			else
				ifLocalVar143 = temp_cast_5;
			o.Albedo = ifLocalVar143.rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = worldViewDir;
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17000
95;78;1920;1029;-918.3607;855.6488;1.447413;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;83;-4902.039,216.146;Float;True;Property;_NormalTex;Normal Tex;5;1;[Normal];Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;48;-4121.937,-275.0797;Float;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;29;-3724.678,-274.9449;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;81;-4626.635,228.9198;Float;True;Property;_TextureSample2;Texture Sample 2;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;120;-3811.941,-1244.104;Float;False;Constant;_Float3;Float 3;9;0;Create;True;0;0;False;0;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;75;-4036.861,-521.2124;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ColorNode;84;-4508.885,514.5595;Float;False;Constant;_Color0;Color 0;7;0;Create;True;0;0;False;0;0,0,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalVertexDataNode;80;-4012.418,568.1475;Float;True;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;76;-3803.838,99.93349;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightAttenuation;33;-4392.936,-435.0798;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-3640.095,-1194.264;Float;False;2;2;0;FLOAT;-1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;96;-4136.192,419.411;Float;False;Property;_UseNormalTex;Use Normal Tex;6;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;108;-3593.465,794.7137;Float;True;2;0;FLOAT;50;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;79;-3665.215,398.4698;Float;True;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;73;-3430.095,-1185.264;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;77;-3588.408,100.5101;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;168;-4114.369,-376.705;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;107;-3366.465,712.7137;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;27;-3285.052,-457.8881;Float;False;2;0;FLOAT;0.7;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;78;-3342.529,82.20843;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;72;-3199.094,-1178.264;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;129;-3260.589,996.5017;Float;False;Constant;_Float4;Float 4;9;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;71;-3003.094,-1164.264;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-3040.678,89.61349;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;105;-3059.465,772.7137;Float;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0.105;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;101;-3125.827,304.9964;Float;False;5;0;FLOAT;0.5;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;-0.9;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;104;-3159.465,540.7137;Float;True;Property;_SpecularTex;Specular Tex;7;0;Create;True;0;0;False;0;826e9d3791e89174dbf3097b7e192aa1;826e9d3791e89174dbf3097b7e192aa1;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.OneMinusNode;70;-2750.094,-1168.264;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;100;-2804.795,125.5323;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;98;-2772.142,-327.7293;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;103;-2734.465,610.7137;Float;True;Property;_TextureSample3;Texture Sample 3;8;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-2381.026,456.5851;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-2522.413,-382.2845;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;110;-2357.375,574.9367;Float;False;Property;_SpecularSoftness;Specular Softness;8;0;Create;True;0;0;False;0;0.01;0.01;0.01;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;67;-2478.393,-1138.777;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;69;-2574.61,-878.5747;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;65;-2037.269,-525.9251;Float;True;Property;_AlbedoTex;Albedo Tex;0;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;112;-2257.677,747.5365;Float;False;Property;_SpecularIntensity;Specular Intensity;9;0;Create;True;0;0;False;0;2;2;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;176;-2328.252,-652.0792;Float;False;Property;_ShineClamp;Shine Clamp;10;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;116;-2278.131,-202.0535;Float;False;3;0;FLOAT;0.29;False;1;FLOAT;0.29;False;2;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;109;-2055.375,482.9367;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.01,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;66;-2179.641,-877.7505;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;64;-1801.204,-525.4883;Float;True;Property;_TextureSample1;Texture Sample 1;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;68;-2202.393,-1146.777;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;131;-1708.78,-712.0515;Float;False;Property;_Color;Color;1;0;Create;True;0;0;False;0;1,1,1,0;0.5,0.5,0.5,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;24;-2064.463,-244.1537;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.2;False;4;FLOAT;0.9;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;180;-1910.67,-696.4648;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;-1803.375,593.9367;Float;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;49;-4268.935,-130.0796;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-1424,-288;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-1443.716,-540.1296;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;115;-1576.99,-13.53461;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;13;-1937.978,-883.079;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;170;-4032.421,-81.04968;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-1787.286,-833.654;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;18;-1705.132,223.2666;Float;True;Property;_EmissionTex;Emission Tex;2;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleAddOpNode;22;-1214.105,-127.4181;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-1297.144,-489.8509;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;-990.1639,-335.3752;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;17;-1474.059,223.721;Float;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;16;-1389.059,474.721;Float;False;Property;_EmissionStrength;Emission Strength;3;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;11;-1456.139,-892.476;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;118;-1057.275,-485.8297;Float;False;Constant;_Float0;Float 0;9;0;Create;True;0;0;False;0;1.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;119;-1011.938,139.96;Float;False;Constant;_Float1;Float 1;9;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;179;-1012.145,-668.1226;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-867.4999,-456.0999;Float;False;2;2;0;FLOAT;1.5;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-1058.059,397.721;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-756.1568,-506.9803;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-799.3176,170.259;Float;True;2;2;0;FLOAT;1;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector4Node;174;-522.9769,-578.3466;Float;False;Global;AmbientColor;AmbientColor;10;0;Create;True;0;0;False;0;0.5,0.5,0.5,1;1,1,1,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-547.5214,-342.24;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-272,64;Float;False;Property;_Reflectivity;Reflectivity;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-256,-336;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;1;87.96389,-267.9493;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;149;325.6735,-472.7571;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;148;613.3585,-467.6198;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;140;435.1316,-1283.332;Float;False;Global;LampPosition;LampPosition;10;0;Create;True;0;0;False;0;0,0,0;-1.407576,1.361,8.701469;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;141;440.8295,-996.9578;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;139;831.5317,-1026.231;Float;False;Global;LampRange;LampRange;10;0;Create;True;0;0;False;0;20;10.07356;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;142;816.8148,-1114.803;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;150;998.6505,-474.4695;Float;False;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;178;-1056.677,-781.2553;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;181;1647.299,-72.36565;Float;True;Property;_TextureSample4;Texture Sample 4;11;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ConditionalIfNode;143;1381.152,-452.3806;Float;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;190;1983.831,-57.18664;Float;False;Constant;_Float2;Float 2;13;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;183;1624.351,175.01;Float;False;Property;_AlphaClip;Alpha Clip;12;0;Create;True;0;0;False;0;0.5;0.901;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;182;1389.323,-75.32533;Float;True;Property;_Alpha;Alpha;11;0;Create;True;0;0;False;0;None;dcca09d7676355a419b8744273c1179a;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;191;1985.622,9.596436;Float;False;Constant;_Float5;Float 5;13;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;192;2326.542,-58.96211;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Zelda_Shader_Cutout;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;29;0;48;0
WireConnection;81;0;83;0
WireConnection;76;0;29;0
WireConnection;76;1;75;0
WireConnection;74;0;120;0
WireConnection;74;1;29;0
WireConnection;96;0;84;0
WireConnection;96;1;81;0
WireConnection;79;0;96;0
WireConnection;79;1;80;0
WireConnection;73;0;74;0
WireConnection;73;1;75;0
WireConnection;77;0;76;0
WireConnection;168;0;33;0
WireConnection;107;0;108;0
WireConnection;27;1;168;0
WireConnection;78;0;77;0
WireConnection;78;1;79;0
WireConnection;72;0;73;0
WireConnection;71;0;72;0
WireConnection;99;0;27;0
WireConnection;99;1;78;0
WireConnection;105;0;107;0
WireConnection;105;2;129;0
WireConnection;70;0;71;0
WireConnection;100;0;99;0
WireConnection;100;1;101;0
WireConnection;98;0;29;0
WireConnection;98;1;79;0
WireConnection;103;0;104;0
WireConnection;103;1;105;0
WireConnection;102;0;100;0
WireConnection;102;1;103;0
WireConnection;26;0;27;0
WireConnection;26;1;98;0
WireConnection;67;0;70;0
WireConnection;116;0;26;0
WireConnection;109;0;102;0
WireConnection;109;2;110;0
WireConnection;66;0;67;0
WireConnection;66;1;69;0
WireConnection;64;0;65;0
WireConnection;68;0;67;0
WireConnection;24;0;116;0
WireConnection;180;0;66;0
WireConnection;180;2;176;0
WireConnection;111;0;109;0
WireConnection;111;1;112;0
WireConnection;111;2;112;0
WireConnection;23;0;180;0
WireConnection;23;1;24;0
WireConnection;62;0;131;0
WireConnection;62;1;64;0
WireConnection;115;0;116;0
WireConnection;115;1;111;0
WireConnection;13;0;68;0
WireConnection;13;1;69;0
WireConnection;170;0;49;0
WireConnection;12;0;13;0
WireConnection;22;0;23;0
WireConnection;22;1;115;0
WireConnection;21;0;62;0
WireConnection;21;1;170;0
WireConnection;114;0;21;0
WireConnection;114;1;22;0
WireConnection;17;0;18;0
WireConnection;11;0;12;0
WireConnection;179;0;11;0
WireConnection;179;2;176;0
WireConnection;19;0;118;0
WireConnection;19;1;114;0
WireConnection;15;0;17;0
WireConnection;15;1;16;0
WireConnection;10;0;179;0
WireConnection;10;1;19;0
WireConnection;14;0;119;0
WireConnection;14;1;15;0
WireConnection;9;0;10;0
WireConnection;9;1;14;0
WireConnection;4;0;9;0
WireConnection;4;1;174;0
WireConnection;1;0;4;0
WireConnection;1;2;3;0
WireConnection;149;0;1;0
WireConnection;148;0;149;0
WireConnection;148;1;149;1
WireConnection;148;2;149;2
WireConnection;142;0;140;0
WireConnection;142;1;141;0
WireConnection;150;0;148;0
WireConnection;178;0;11;0
WireConnection;178;1;176;0
WireConnection;181;0;182;0
WireConnection;143;0;142;0
WireConnection;143;1;139;0
WireConnection;143;2;150;0
WireConnection;143;3;1;0
WireConnection;143;4;1;0
WireConnection;192;0;143;0
ASEEND*/
//CHKSM=051F5E95C5C0CDA910A0A425D4ECD1B8A7E21BC1