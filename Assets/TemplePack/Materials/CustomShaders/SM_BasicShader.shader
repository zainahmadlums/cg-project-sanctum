// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SM_BasicShader"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_NormalMap("NormalMap", 2D) = "bump" {}
		_VertexOffset("VertexOffset", Float) = 0
		_ColorOffset("ColorOffset", Color) = (1,1,1,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		
		
		
		struct Input
		{
			float2 uv_texcoord;
		};
		uniform float _VertexOffset;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float outlineVar = _VertexOffset;
			v.vertex.xyz += ( v.normal * outlineVar );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			o.Emission = ( tex2D( _TextureSample0, uv_TextureSample0 ) * float4( 0.5660378,0.5660378,0.5660378,0 ) ).rgb;
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 2.0
		#pragma surface surf Lambert keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			INTERNAL_DATA
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float4 _ColorOffset;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += 0;
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float3 tex2DNode23 = UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) );
			o.Normal = tex2DNode23;
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float4 tex2DNode35 = tex2D( _TextureSample0, uv_TextureSample0 );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float fresnelNdotV32 = dot( normalize( tex2DNode23 ), ase_worldlightDir );
			float fresnelNode32 = ( -0.11 + 0.2 * pow( max( 1.0 - fresnelNdotV32 , 0.0001 ), 1.99 ) );
			float smoothstepResult38 = smoothstep( 0.0 , 0.4 , ( 1.0 - saturate( fresnelNode32 ) ));
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			o.Albedo = ( _ColorOffset * ( ( tex2DNode35 * smoothstepResult38 * ase_lightColor ) + tex2DNode35 ) ).rgb;
			o.Specular = tex2DNode35.r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17900
-1582;107;1286;882;1413.81;954.981;1.851543;True;False
Node;AmplifyShaderEditor.SamplerNode;23;-808.5892,-703.5406;Inherit;True;Property;_NormalMap;NormalMap;1;0;Create;True;0;0;False;0;-1;None;5a9549082f7322847beb39e414c93b9d;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;32;-396.9369,-679.571;Inherit;True;Standard;WorldNormal;LightDir;True;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;-0.11;False;2;FLOAT;0.2;False;3;FLOAT;1.99;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;34;45.55222,-659.4429;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;33;-113.0623,-581.2505;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;40;109.5197,-243.0498;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SmoothstepOpNode;38;75.90741,-488.0819;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.4;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;35;413.6466,-637.3696;Inherit;True;Property;_TextureSample1;Texture Sample 1;0;0;Create;True;0;0;False;0;-1;None;32f1cd703dbbd794a999df2e6b5522db;True;0;False;white;Auto;False;Instance;1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-237.1238,-75.78426;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;-1;None;0a08cb0b1d44a624b906bfd64cc63d0c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;324.1943,-360.7256;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;5;357.4367,336.179;Inherit;False;Property;_VertexOffset;VertexOffset;2;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;44;840.8206,-788.2927;Inherit;False;Property;_ColorOffset;ColorOffset;3;0;Create;True;0;0;False;0;1,1,1,0;0.6698113,0.6698113,0.6698113,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;39;532.8589,-325.8974;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;378.3105,136.1782;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.5660378,0.5660378,0.5660378,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;849.9208,-378.7927;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;45;779.7209,-594.5927;Inherit;False;Property;_Float1;Float 1;4;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;587.9083,5.614868;Inherit;False;Constant;_Float0;Float 0;3;0;Create;True;0;0;False;0;3.19;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OutlineNode;19;589.4633,174.7412;Inherit;False;0;True;None;0;0;Front;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;903,-209;Float;False;True;-1;0;ASEMaterialInspector;0;0;Lambert;SM_BasicShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;32;0;23;0
WireConnection;34;0;32;0
WireConnection;33;0;34;0
WireConnection;38;0;33;0
WireConnection;37;0;35;0
WireConnection;37;1;38;0
WireConnection;37;2;40;0
WireConnection;39;0;37;0
WireConnection;39;1;35;0
WireConnection;22;0;1;0
WireConnection;43;0;44;0
WireConnection;43;1;39;0
WireConnection;19;0;22;0
WireConnection;19;1;5;0
WireConnection;0;0;43;0
WireConnection;0;1;23;0
WireConnection;0;3;35;0
WireConnection;0;11;19;0
ASEEND*/
//CHKSM=9216DF3342FE8DAFA2D8F73FDF69479D5808D47C