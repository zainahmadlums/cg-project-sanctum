// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "sm_flame"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_Color("Color", Color) = (0,0,0,0)
		_panningdiffuse("panning diffuse", Vector) = (1,1,0,0)
		_Float1("Float 1", Float) = 1.42
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
		
		#include "UnityShaderVariables.cginc"
		
		
		struct Input
		{
			float2 uv_texcoord;
		};
		uniform sampler2D _TextureSample0;
		uniform float2 _panningdiffuse;
		uniform sampler2D _TextureSample1;
		uniform float _Float1;
		uniform float4 _Color;
		
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float mulTime26 = _Time.y * 0.5;
			float2 uv_TexCoord27 = v.texcoord.xy * float2( 0.2,-0.16 ) + ( _panningdiffuse * mulTime26 );
			float4 lerpResult8 = lerp( tex2Dlod( _TextureSample0, float4( uv_TexCoord27, 0, 0.0) ) , tex2Dlod( _TextureSample1, float4( uv_TexCoord27, 0, 0.0) ) , float4( 0.678,0.678,0.678,0 ));
			float grayscale37 = Luminance(lerpResult8.rgb);
			float outlineVar = ( grayscale37 * _Float1 ).r;
			v.vertex.xyz += ( v.normal * outlineVar );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			float mulTime26 = _Time.y * 0.5;
			float2 uv_TexCoord27 = i.uv_texcoord * float2( 0.2,-0.16 ) + ( _panningdiffuse * mulTime26 );
			float4 lerpResult8 = lerp( tex2D( _TextureSample0, uv_TexCoord27 ) , tex2D( _TextureSample1, uv_TexCoord27 ) , float4( 0.678,0.678,0.678,0 ));
			float grayscale37 = Luminance(lerpResult8.rgb);
			float4 temp_output_12_0 = ( grayscale37 + _Color );
			o.Emission = temp_output_12_0.rgb;
		}
		ENDCG
		

		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 2.5
		#pragma surface surf Unlit keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _TextureSample0;
		uniform float2 _panningdiffuse;
		uniform sampler2D _TextureSample1;
		uniform float4 _Color;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 temp_output_17_0 = 0;
			v.vertex.xyz += temp_output_17_0;
			v.normal = temp_output_17_0;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float mulTime26 = _Time.y * 0.5;
			float2 uv_TexCoord27 = i.uv_texcoord * float2( 0.2,-0.16 ) + ( _panningdiffuse * mulTime26 );
			float4 lerpResult8 = lerp( tex2D( _TextureSample0, uv_TexCoord27 ) , tex2D( _TextureSample1, uv_TexCoord27 ) , float4( 0.678,0.678,0.678,0 ));
			float grayscale37 = Luminance(lerpResult8.rgb);
			float4 temp_output_12_0 = ( grayscale37 + _Color );
			o.Emission = temp_output_12_0.rgb;
			o.Alpha = 1;
			float3 temp_output_17_0 = half3(0,0,0);
			clip( temp_output_17_0.x - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17900
-1281;497;1030;491;708.8749;-28.44179;1.272797;True;False
Node;AmplifyShaderEditor.Vector2Node;28;-1288.531,-167.2627;Float;False;Property;_panningdiffuse;panning diffuse;4;0;Create;True;0;0;False;0;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;26;-1272.39,-33.1891;Inherit;False;1;0;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-1101.232,-162.1262;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;27;-979.9905,-207.8141;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.2,-0.16;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-724.1068,-43.72683;Inherit;True;Property;_TextureSample1;Texture Sample 1;2;0;Create;True;0;0;False;0;-1;36be8d528a4fa024faa4680d7658642c;36be8d528a4fa024faa4680d7658642c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-715.9864,-235.4795;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;-1;f7e96904e8667e1439548f0f86389447;e28dc97a9541e3642a48c0e3886688c5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;8;-343.2322,-237.6699;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0.678,0.678,0.678,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCGrayscale;37;-44.47081,-232.5501;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;13;-46.09004,-158.9982;Inherit;False;Property;_Color;Color;3;0;Create;True;0;0;False;0;0,0,0,0;1,0.5861652,0.1839623,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;43;-339.3937,437.7345;Inherit;False;Property;_Float1;Float 1;5;0;Create;True;0;0;False;0;1.42;-0.35;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-68.69376,308.4345;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;1.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;12;204.4923,-183.4847;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OutlineNode;17;201.4623,206.9544;Inherit;False;0;True;None;0;0;Front;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;16;584.5739,-226.7987;Float;False;True;-1;1;ASEMaterialInspector;0;0;Unlit;sm_flame;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;False;0;True;TransparentCutout;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;29;0;28;0
WireConnection;29;1;26;0
WireConnection;27;1;29;0
WireConnection;3;1;27;0
WireConnection;2;1;27;0
WireConnection;8;0;2;0
WireConnection;8;1;3;0
WireConnection;37;0;8;0
WireConnection;42;0;37;0
WireConnection;42;1;43;0
WireConnection;12;0;37;0
WireConnection;12;1;13;0
WireConnection;17;0;12;0
WireConnection;17;1;42;0
WireConnection;16;2;12;0
WireConnection;16;10;17;0
WireConnection;16;11;17;0
WireConnection;16;12;17;0
ASEEND*/
//CHKSM=56274C1C3636B099617F174E56BCED4634620428