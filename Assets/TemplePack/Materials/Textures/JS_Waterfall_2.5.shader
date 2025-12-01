// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "sm_waterfall"
{
	Properties
	{
		_mask1("mask1", 2D) = "white" {}
		_power("power", Float) = 0.5
		_1("1", Color) = (0,0,0,0)
		_2("2", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform float _power;
		uniform sampler2D _mask1;
		uniform float4 _1;
		uniform float4 _2;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_TexCoord287 = i.uv_texcoord * float2( 2,2 );
			float2 panner253 = ( 1.0 * _Time.y * float2( 0,0.3 ) + uv_TexCoord287);
			float4 tex2DNode4 = tex2D( _mask1, panner253 );
			float4 lerpResult303 = lerp( _1 , _2 , float4( 0,0,0,0 ));
			float4 clampResult311 = clamp( ( ( _power * tex2DNode4 ) * lerpResult303 ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			o.Emission = saturate( clampResult311 ).rgb;
			float grayscale377 = Luminance(tex2DNode4.rgb);
			o.Alpha = ( ( grayscale377 + grayscale377 ) * ( 1.0 - i.vertexColor.r ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17900
-1479;-1544;1286;893;950.9373;1100.801;1;True;False
Node;AmplifyShaderEditor.Vector2Node;375;-595.8188,-760.2001;Inherit;False;Constant;_tiling;tiling;7;0;Create;True;0;0;False;0;2,2;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;373;-205.4481,-467.8391;Inherit;False;Constant;_speed;speed;7;0;Create;True;0;0;False;0;0,0.3;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;287;-321.792,-719.0265;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;253;101.9542,-700.7809;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;13;474.6431,-672.302;Inherit;False;Property;_power;power;3;0;Create;True;0;0;False;0;0.5;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;307;164.4704,112.8291;Inherit;False;Property;_2;2;5;0;Create;True;0;0;False;0;0,0,0,0;0.3435898,0.8871795,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;134.3591,-433.986;Inherit;True;Property;_mask1;mask1;0;0;Create;True;0;0;False;0;-1;None;8dea97ae406f4914da2413e91e5e2d26;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;306;174.4654,-143.2585;Inherit;False;Property;_1;1;4;0;Create;True;0;0;False;0;0,0,0,0;0,0.4727891,0.51,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;303;583.9624,109.3609;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;749.1586,-456.8474;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;308;813.9548,-204.9077;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCGrayscale;377;616.4836,412.6555;Inherit;True;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;381;789.5789,633.5698;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;311;1047.27,106.0925;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;380;875.3781,403.4699;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;383;1031.378,625.7699;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;259;-4072.578,364.41;Float;False;Property;_NoiseScaleMaster;Noise Scale Master;2;0;Create;True;0;0;False;0;1;-0.99;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;257;-4072.536,266.839;Float;False;Property;_NoiseSpeedMaster;Noise Speed Master;1;0;Create;True;0;0;False;0;1;-0.64;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;372;52.09354,-871.8256;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;276;-3815.044,370.7397;Inherit;False;NoiseScaleMaster;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;351;1258.065,202.6304;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;382;1104.178,420.3698;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;275;-3814.044,266.7397;Inherit;False;NoiseSpeedMaster;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1480.462,251.2248;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;sm_waterfall;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;2;False;-1;0;False;-1;True;-10;False;-1;-1;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;19.8;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;287;0;375;0
WireConnection;253;0;287;0
WireConnection;253;2;373;0
WireConnection;4;1;253;0
WireConnection;303;0;306;0
WireConnection;303;1;307;0
WireConnection;12;0;13;0
WireConnection;12;1;4;0
WireConnection;308;0;12;0
WireConnection;308;1;303;0
WireConnection;377;0;4;0
WireConnection;311;0;308;0
WireConnection;380;0;377;0
WireConnection;380;1;377;0
WireConnection;383;0;381;1
WireConnection;276;0;259;0
WireConnection;351;0;311;0
WireConnection;382;0;380;0
WireConnection;382;1;383;0
WireConnection;275;0;257;0
WireConnection;0;2;351;0
WireConnection;0;9;382;0
ASEEND*/
//CHKSM=F64F1AE8074BA11B81887001305536FA01AE95F3