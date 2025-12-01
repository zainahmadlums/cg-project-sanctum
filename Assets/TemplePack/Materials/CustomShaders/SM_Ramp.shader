// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SM_RampShader"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		[Normal]_NormalMap("NormalMap", 2D) = "bump" {}
		_SecondaryNormals("SecondaryNormals", 2D) = "bump" {}
		_SecondNrmTiling("SecondNrmTiling", Vector) = (0.05,0.05,0,0)
		_ColorOffset("ColorOffset", Color) = (0.9716981,0.9716981,0.9716981,0)
		[Toggle]_GrassTop("GrassTop", Float) = 1
		_MossColor("MossColor", Color) = (0.2694077,0.572549,0.01202351,0.6627451)
		_TopGrassTexture("TopGrassTexture", 2D) = "white" {}
		_VoidTexture("VoidTexture", 2D) = "white" {}
		_GrassCoverage("GrassCoverage", Range( 0 , 0.8)) = 0
		_StripColor("StripColor", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		#define ASE_TEXTURE_PARAMS(textureName) textureName

		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
		};

		uniform sampler2D _SecondaryNormals;
		uniform float2 _SecondNrmTiling;
		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float _GrassTop;
		uniform float4 _StripColor;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform sampler2D _TopGrassTexture;
		uniform float4 _MossColor;
		uniform float4 _ColorOffset;
		uniform sampler2D _VoidTexture;
		uniform float _GrassCoverage;


		inline float3 TriplanarSamplingSNF( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = ( tex2D( ASE_TEXTURE_PARAMS( topTexMap ), tiling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( ASE_TEXTURE_PARAMS( topTexMap ), tiling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( ASE_TEXTURE_PARAMS( topTexMap ), tiling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			xNorm.xyz = half3( UnpackScaleNormal( xNorm, normalScale.y ).xy * float2( nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ).zyx;
			yNorm.xyz = half3( UnpackScaleNormal( yNorm, normalScale.x ).xy * float2( nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;
			zNorm.xyz = half3( UnpackScaleNormal( zNorm, normalScale.y ).xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ).xyz;
			return normalize( xNorm.xyz * projNormal.x + yNorm.xyz * projNormal.y + zNorm.xyz * projNormal.z );
		}


		inline float4 TriplanarSamplingCF( sampler2D topTexMap, sampler2D midTexMap, sampler2D botTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			float negProjNormalY = max( 0, projNormal.y * -nsign.y );
			projNormal.y = max( 0, projNormal.y * nsign.y );
			half4 xNorm; half4 yNorm; half4 yNormN; half4 zNorm;
			xNorm = ( tex2D( ASE_TEXTURE_PARAMS( midTexMap ), tiling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( ASE_TEXTURE_PARAMS( topTexMap ), tiling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			yNormN = ( tex2D( ASE_TEXTURE_PARAMS( botTexMap ), tiling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( ASE_TEXTURE_PARAMS( midTexMap ), tiling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + yNormN * negProjNormalY + zNorm * projNormal.z;
		}


		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float2 _Vector3 = float2(-0.3,0.7);
			float3 triplanar275 = TriplanarSamplingSNF( _SecondaryNormals, ase_worldPos, ase_worldNormal, 1.0, _SecondNrmTiling, _Vector3.x, 0 );
			float3 tanTriplanarNormal275 = mul( ase_worldToTangent, triplanar275 );
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			o.Normal = BlendNormals( tanTriplanarNormal275 , UnpackScaleNormal( tex2D( _NormalMap, uv_NormalMap ), _Vector3.y ) );
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float temp_output_330_0 = ( 1.0 - tex2D( _TextureSample0, uv_TextureSample0 ).a );
			float4 lerpResult326 = lerp( _StripColor , float4( 0,0,0,0 ) , temp_output_330_0);
			float2 _Vector1 = float2(-1.89,1.69);
			float2 _MossLevels = float2(3.91,38.63);
			float2 _Vector6 = float2(1,10);
			float smoothstepResult197 = smoothstep( _MossLevels.x , _MossLevels.y , (ase_worldPos.y*_Vector6.x + _Vector6.y));
			float VertGrad200 = saturate( ( ( smoothstepResult197 + 0.4 ) + 0.1 ) );
			float smoothstepResult212 = smoothstep( _Vector1.x , _Vector1.y , VertGrad200);
			float ramp342 = smoothstepResult212;
			float4 triplanar190 = TriplanarSamplingCF( _TopGrassTexture, _TopGrassTexture, _TopGrassTexture, ase_worldPos, ase_worldNormal, 1.0, float2( 0.3,-0.3 ), float3( 1,1,1 ), float3(0,0,0) );
			float grayscale205 = Luminance(triplanar190.xyz);
			float4 lerpResult155 = lerp( ( grayscale205 * _MossColor * _MossColor.a ) , ( smoothstepResult212 * saturate( ( tex2D( _TextureSample0, uv_TextureSample0 ) * 0.5 * _ColorOffset ) ) ) , VertGrad200);
			float4 lerpResult332 = lerp( saturate( ( ( lerpResult326 * _StripColor.a ) * ramp342 ) ) , lerpResult155 , temp_output_330_0);
			float4 lerpResult341 = lerp( lerpResult332 , lerpResult155 , ( 1.0 - _StripColor.a ));
			float4 triplanar215 = TriplanarSamplingCF( _TopGrassTexture, _VoidTexture, _VoidTexture, ase_worldPos, ase_worldNormal, 1.0, float2( 0.04,0.04 ), float3( 1,1,1 ), float3(0,0,0) );
			float4 color310 = IsGammaSpace() ? float4(0.9811321,0.9533642,0.3378426,0.7568628) : float4(0.957614,0.8971726,0.09339396,0.7568628);
			float2 _Vector8 = float2(-0.7,-1.72);
			float4 lerpResult228 = lerp( triplanar215 , float4( 0,0,0,0 ) , -2.0);
			float grayscale231 = Luminance(lerpResult228.xyz);
			float smoothstepResult237 = smoothstep( _Vector8.x , _Vector8.y , ( 1.0 - grayscale231 ));
			float4 color268 = IsGammaSpace() ? float4(0.5882353,0.572549,0.2039216,0.7764706) : float4(0.3049874,0.2874408,0.03433981,0.7764706);
			float temp_output_288_0 = ( smoothstepResult237 * color268.a );
			float4 lerpResult239 = lerp( ( triplanar215 * color310 * ( 1.0 - temp_output_288_0 ) * color268 ) , float4( 0,0,0,0 ) , temp_output_288_0);
			float smoothstepResult320 = smoothstep( 0.0 , _GrassCoverage , temp_output_288_0);
			float4 lerpResult254 = lerp( lerpResult239 , lerpResult341 , smoothstepResult320);
			o.Albedo = (( _GrassTop )?( lerpResult254 ):( lerpResult341 )).rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Lambert keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
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
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
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
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
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
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
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
Version=17900
-1481;-1550;1286;899;-554.0399;2357.539;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;201;375.3374,-2002.038;Inherit;False;1214.038;512.0387;Comment;10;194;195;196;198;197;203;204;200;345;346;Vertical Gradient;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;194;627.3373,-1945.638;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector2Node;195;648.5378,-1813.037;Inherit;False;Constant;_Vector6;Vector 6;13;0;Create;True;0;0;False;0;1,10;1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ScaleAndOffsetNode;196;867.5382,-1949.038;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;198;852.8662,-1724.323;Inherit;False;Constant;_MossLevels;MossLevels;5;0;Create;True;0;0;False;0;3.91,38.63;5.31,42.03;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SmoothstepOpNode;197;1103.286,-1742.999;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0.11;False;2;FLOAT;0.24;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;346;1159.04,-1899.539;Inherit;False;Constant;_Float1;Float 1;11;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;204;1380.857,-1971.911;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.4;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;345;1389.04,-1878.539;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;266;241.8078,300.5328;Inherit;False;2149.865;793.1596;;14;221;228;231;236;237;239;216;252;215;267;268;285;316;319;GrassFromTop;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;216;656.2532,711.1785;Inherit;False;Constant;_Vector0;Vector 0;12;0;Create;True;0;0;False;0;0.04,0.04;0.06,0.06;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SaturateNode;203;1390.122,-1729.681;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;221;291.8078,512.2411;Inherit;True;Property;_TopGrassTexture;TopGrassTexture;7;0;Create;True;0;0;False;0;None;af8620bd60150c142b297e37c7da6f64;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;267;626.8695,369.6849;Inherit;True;Property;_VoidTexture;VoidTexture;8;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;319;696.7068,882.3301;Inherit;False;Constant;_Float3;Float 3;11;0;Create;True;0;0;False;0;1;0.81;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;316;928.4333,835.6724;Inherit;False;Constant;_Float2;Float 2;9;0;Create;True;0;0;False;0;-2;-2.11;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;215;910.437,619.1225;Inherit;True;Cylindrical;World;False;Top Texture 1;_TopTexture1;white;0;None;Mid Texture 0;_MidTexture0;white;3;None;Bot Texture 0;_BotTexture0;white;5;None;Triplanar Sampler;False;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT3;1,1,1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;200;1389.096,-1659.871;Inherit;False;VertGrad;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;214;495.217,-393.9886;Inherit;False;Constant;_Vector1;Vector 1;9;0;Create;True;0;0;False;0;-1.89,1.69;0.6,1.2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;327;1802.484,-1020.341;Inherit;True;Property;_TextureSample1;Texture Sample 1;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Instance;1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;228;1447.523,742.9413;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;202;560.0657,-551.8237;Inherit;False;200;VertGrad;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;191;758.0065,-911.2255;Inherit;False;Constant;_Float0;Float 0;6;0;Create;True;0;0;False;0;1;0.95;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;24;-235.4635,127.0576;Inherit;False;Property;_ColorOffset;ColorOffset;4;0;Create;True;0;0;False;0;0.9716981,0.9716981,0.9716981,0;0.825,0.7788,0.7788,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;212;716.217,-396.9886;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;193;752.4075,-1062.633;Inherit;False;Constant;_Vector7;Vector 7;12;0;Create;True;0;0;False;0;0.3,-0.3;0.66,-0.57;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.OneMinusNode;330;2172.145,-814.3292;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;71.77106,7.766079;Inherit;False;Constant;_Brightness;Brightness;3;0;Create;True;0;0;False;0;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;328;2098.268,-1196.948;Inherit;False;Property;_StripColor;StripColor;10;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCGrayscale;231;1666.399,581.2162;Inherit;True;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-262.4696,-80.35483;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;-1;None;8fea0b1fcfecf404690ef32d0350ffca;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;317;1881.063,987.981;Inherit;False;Constant;_Vector8;Vector 8;16;0;Create;True;0;0;False;0;-0.7,-1.72;-0.7,-1.72;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.OneMinusNode;236;1671.01,816.6547;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;342;789.7634,-582.8575;Inherit;False;ramp;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;190;976.1906,-1056.043;Inherit;True;Cylindrical;World;False;Top Texture 0;_TopTexture0;black;1;None;Mid Texture 1;_MidTexture1;white;1;None;Bot Texture 1;_BotTexture1;white;2;None;Triplanar Sampler;False;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT3;1,1,1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;326;2389.99,-1052.729;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;372.1492,-52.63287;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;268;904.1758,355.4401;Inherit;False;Constant;_GrassColorOffset;GrassColorOffset;11;0;Create;True;0;0;False;0;0.5882353,0.572549,0.2039216,0.7764706;0.8584906,0.8203301,0,0.7764706;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;140;863.9841,-789.0641;Inherit;False;Property;_MossColor;MossColor;6;0;Create;True;0;0;False;0;0.2694077,0.572549,0.01202351,0.6627451;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;18;566.0275,-36.58213;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;343;2531.038,-548.3964;Inherit;False;342;ramp;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;237;2131.414,764.2252;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;337;2398.761,-834.0543;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCGrayscale;205;1365.803,-968.6837;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;344;2721.769,-817.5504;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;206;1111.855,-809.2986;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;959.9061,-175.7365;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;288;2394.255,701.0918;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;310;962.7675,163.9656;Inherit;False;Constant;_Color0;Color 0;10;0;Create;True;0;0;False;0;0.9811321,0.9533642,0.3378426,0.7568628;0.587,0.5741839,0.202515,0.7607843;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;155;1027.019,-568.134;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;285;2017.094,448.2388;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;331;2951.502,-829.8021;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;322;2418.669,399.5801;Inherit;False;Constant;_Float4;Float 4;14;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;323;2410.669,466.5801;Inherit;False;Property;_GrassCoverage;GrassCoverage;9;0;Create;True;0;0;False;0;0;0.8;0;0.8;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;332;1305.266,-397.0715;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;338;2178.321,-1023.271;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;252;1286.641,325.7065;Inherit;True;4;4;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;239;1570.846,337.5521;Inherit;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SmoothstepOpNode;320;2610.808,381.3029;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;276;1489.125,-656.2855;Inherit;False;Property;_SecondNrmTiling;SecondNrmTiling;3;0;Create;True;0;0;False;0;0.05,0.05;0.05,0.05;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexturePropertyNode;278;1577.191,-858.8284;Inherit;True;Property;_SecondaryNormals;SecondaryNormals;2;0;Create;True;0;0;False;0;None;None;True;bump;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.LerpOp;341;1558.099,-390.4813;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;287;1501.521,-535.1089;Inherit;False;Constant;_Vector3;Vector 3;13;0;Create;True;0;0;False;0;-0.3,0.7;-0.3,0.04;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;23;1984.232,64.09846;Inherit;True;Property;_NormalMap;NormalMap;1;1;[Normal];Create;True;0;0;False;0;-1;None;5683b6c1b2cf55c49877777f20e2b06e;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TriplanarNode;275;1765.738,-680.4227;Inherit;True;Spherical;World;True;BaseTexture;_BaseTexture;white;1;None;Mid Texture 2;_MidTexture2;white;2;None;Bot Texture 2;_BotTexture2;white;3;None;Triplanar Sampler;False;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;254;1859.351,-206.8482;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;256;2134.379,-399.5267;Inherit;True;Property;_GrassTop;GrassTop;5;0;Create;True;0;0;False;0;1;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendNormalsNode;281;2367.835,-176.2015;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3012.55,-323.3202;Float;False;True;-1;6;ASEMaterialInspector;0;0;Lambert;SM_RampShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;14.7;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;196;0;194;2
WireConnection;196;1;195;1
WireConnection;196;2;195;2
WireConnection;197;0;196;0
WireConnection;197;1;198;1
WireConnection;197;2;198;2
WireConnection;204;0;197;0
WireConnection;345;0;204;0
WireConnection;345;1;346;0
WireConnection;203;0;345;0
WireConnection;215;0;221;0
WireConnection;215;1;267;0
WireConnection;215;2;267;0
WireConnection;215;3;216;0
WireConnection;215;4;319;0
WireConnection;200;0;203;0
WireConnection;228;0;215;0
WireConnection;228;2;316;0
WireConnection;212;0;202;0
WireConnection;212;1;214;1
WireConnection;212;2;214;2
WireConnection;330;0;327;4
WireConnection;231;0;228;0
WireConnection;236;0;231;0
WireConnection;342;0;212;0
WireConnection;190;0;221;0
WireConnection;190;1;221;0
WireConnection;190;2;221;0
WireConnection;190;3;193;0
WireConnection;190;4;191;0
WireConnection;326;0;328;0
WireConnection;326;2;330;0
WireConnection;6;0;1;0
WireConnection;6;1;7;0
WireConnection;6;2;24;0
WireConnection;18;0;6;0
WireConnection;237;0;236;0
WireConnection;237;1;317;1
WireConnection;237;2;317;2
WireConnection;337;0;326;0
WireConnection;337;1;328;4
WireConnection;205;0;190;0
WireConnection;344;0;337;0
WireConnection;344;1;343;0
WireConnection;206;0;205;0
WireConnection;206;1;140;0
WireConnection;206;2;140;4
WireConnection;31;0;212;0
WireConnection;31;1;18;0
WireConnection;288;0;237;0
WireConnection;288;1;268;4
WireConnection;155;0;206;0
WireConnection;155;1;31;0
WireConnection;155;2;202;0
WireConnection;285;0;288;0
WireConnection;331;0;344;0
WireConnection;332;0;331;0
WireConnection;332;1;155;0
WireConnection;332;2;330;0
WireConnection;338;0;328;4
WireConnection;252;0;215;0
WireConnection;252;1;310;0
WireConnection;252;2;285;0
WireConnection;252;3;268;0
WireConnection;239;0;252;0
WireConnection;239;2;288;0
WireConnection;320;0;288;0
WireConnection;320;1;322;0
WireConnection;320;2;323;0
WireConnection;341;0;332;0
WireConnection;341;1;155;0
WireConnection;341;2;338;0
WireConnection;23;5;287;2
WireConnection;275;0;278;0
WireConnection;275;8;287;1
WireConnection;275;3;276;0
WireConnection;254;0;239;0
WireConnection;254;1;341;0
WireConnection;254;2;320;0
WireConnection;256;0;341;0
WireConnection;256;1;254;0
WireConnection;281;0;275;0
WireConnection;281;1;23;0
WireConnection;0;0;256;0
WireConnection;0;1;281;0
ASEEND*/
//CHKSM=F02C810A7A0322F4566CF3C87687BDA40D2D0670