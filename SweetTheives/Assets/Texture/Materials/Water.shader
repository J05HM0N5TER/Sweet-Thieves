// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Water"
{
	Properties
	{
		_wavetile("wave tile", Float) = 1
		_WaveSpeed("Wave Speed", Float) = 1
		_WaveHeight("Wave Height", Float) = 1
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_watercolour("water colour", Color) = (0,0,0,0)
		_topcolour("top colour", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform float _WaveHeight;
		uniform float _WaveSpeed;
		uniform float _wavetile;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float4 _watercolour;
		uniform float4 _topcolour;


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


		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			float4 temp_cast_4 = (1.0).xxxx;
			return temp_cast_4;
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float temp_output_8_0 = ( _Time.y * _WaveSpeed );
			float2 _WaveDirection = float2(0,1);
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float4 appendResult11 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 worldSpace12 = appendResult11;
			float4 WaveTileUV25 = ( ( worldSpace12 * float4( float2( 0.1,1 ), 0.0 , 0.0 ) ) * _wavetile );
			float2 panner5 = ( temp_output_8_0 * _WaveDirection + WaveTileUV25.xy);
			float simplePerlin2D2 = snoise( panner5 );
			float2 temp_cast_2 = (_WaveDirection.x).xx;
			float2 panner29 = ( temp_output_8_0 * temp_cast_2 + ( WaveTileUV25 * float4( 0,0,0,0 ) ).xy);
			float simplePerlin2D30 = snoise( panner29 );
			float temp_output_32_0 = ( simplePerlin2D2 + simplePerlin2D30 );
			float3 WaveHeight39 = ( ( float3(0,0.2,0) * _WaveHeight ) * temp_output_32_0 );
			v.vertex.xyz += WaveHeight39;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			o.Normal = tex2D( _TextureSample0, uv_TextureSample0 ).rgb;
			float temp_output_8_0 = ( _Time.y * _WaveSpeed );
			float2 _WaveDirection = float2(0,1);
			float3 ase_worldPos = i.worldPos;
			float4 appendResult11 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 worldSpace12 = appendResult11;
			float4 WaveTileUV25 = ( ( worldSpace12 * float4( float2( 0.1,1 ), 0.0 , 0.0 ) ) * _wavetile );
			float2 panner5 = ( temp_output_8_0 * _WaveDirection + WaveTileUV25.xy);
			float simplePerlin2D2 = snoise( panner5 );
			float2 temp_cast_3 = (_WaveDirection.x).xx;
			float2 panner29 = ( temp_output_8_0 * temp_cast_3 + ( WaveTileUV25 * float4( 0,0,0,0 ) ).xy);
			float simplePerlin2D30 = snoise( panner29 );
			float temp_output_32_0 = ( simplePerlin2D2 + simplePerlin2D30 );
			float WavePattern36 = temp_output_32_0;
			float4 lerpResult47 = lerp( _watercolour , _topcolour , WavePattern36);
			float4 albedo53 = lerpResult47;
			o.Albedo = albedo53.rgb;
			o.Smoothness = 0.9;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16800
202;73;1298;653;328.1102;743.3976;1.43766;True;False
Node;AmplifyShaderEditor.CommentaryNode;14;-3672.15,-531.3724;Float;False;821.0002;323.9999;Comment;3;11;10;12;WorldSpace UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;10;-3622.15,-481.3723;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;11;-3286.149,-465.3724;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-3083.727,-491.7302;Float;True;worldSpace;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;19;-2315.951,-714.0406;Float;False;Constant;_Vector0;Vector 0;0;0;Create;True;0;0;False;0;0.1,1;0.25,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;15;-2330.284,-948.9085;Float;True;12;worldSpace;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-2025.74,-815.6999;Float;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-1737.169,-685.0018;Float;False;Property;_wavetile;wave tile;0;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-1708.082,-878.5793;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;35;-2677.476,132.2117;Float;False;1499.541;1235.049;The Pattern;12;34;6;9;8;4;33;28;5;29;2;32;30;Wave Pattern;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;25;-1488.653,-862.7214;Float;True;WaveTileUV;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-2629.476,1044.212;Float;True;Property;_WaveSpeed;Wave Speed;1;0;Create;True;0;0;False;0;1;0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;34;-2117.477,1140.212;Float;True;25;WaveTileUV;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleTimeNode;6;-2629.476,804.2117;Float;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-2085.477,980.2117;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;4;-2533.476,484.2117;Float;True;Constant;_WaveDirection;Wave Direction;0;0;Create;True;0;0;False;0;0,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-2373.478,820.2117;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;28;-2533.476,180.2117;Float;True;25;WaveTileUV;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PannerNode;29;-1941.475,692.2117;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;5;-2053.476,420.2117;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;30;-1637.475,708.2117;Float;True;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;2;-1733.475,388.2117;Float;True;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;32;-1413.475,580.2117;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;41;-1251.488,-918.6652;Float;False;1100.354;642.887;Comment;5;23;37;24;38;39;UV height;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector3Node;23;-1201.488,-868.6652;Float;True;Constant;_WaveUp;Wave Up;2;0;Create;True;0;0;False;0;0,0.2,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;37;-1182.292,-533.7781;Float;True;Property;_WaveHeight;Wave Height;2;0;Create;True;0;0;False;0;1;0.74;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;36;-1026.359,577.473;Float;True;WavePattern;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;45;364.3228,-829.8622;Float;False;Property;_watercolour;water colour;4;0;Create;True;0;0;False;0;0,0,0,0;0.06407974,0.7559434,0.9056604,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-906.8194,-714.1119;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;46;365.8391,-604.8118;Float;False;Property;_topcolour;top colour;5;0;Create;True;0;0;False;0;0,0,0,0;0,0.5921569,0.9568627,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;49;357.4673,-404.1634;Float;True;36;WavePattern;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-565.2983,-638.3414;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;47;730.0634,-526.9414;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;53;1041.299,-492.0209;Float;False;albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;39;-314.955,-646.7651;Float;True;WaveHeight;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;40;345.9949,464.2323;Float;True;39;WaveHeight;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;44;215.1817,-54.53122;Float;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;False;0;None;9c975b0b2a6d04343ab7ddd778edd417;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;22;391.5289,706.1056;Float;True;Constant;_Tesellation;Tesellation;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;380.8962,146.7336;Float;True;Constant;_Float0;Float 0;3;0;Create;True;0;0;False;0;0.9;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;52;665.929,-207.7637;Float;True;53;albedo;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;772.6982,40.59881;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;11;0;10;1
WireConnection;11;1;10;3
WireConnection;12;0;11;0
WireConnection;16;0;15;0
WireConnection;16;1;19;0
WireConnection;20;0;16;0
WireConnection;20;1;21;0
WireConnection;25;0;20;0
WireConnection;33;0;34;0
WireConnection;8;0;6;0
WireConnection;8;1;9;0
WireConnection;29;0;33;0
WireConnection;29;2;4;1
WireConnection;29;1;8;0
WireConnection;5;0;28;0
WireConnection;5;2;4;0
WireConnection;5;1;8;0
WireConnection;30;0;29;0
WireConnection;2;0;5;0
WireConnection;32;0;2;0
WireConnection;32;1;30;0
WireConnection;36;0;32;0
WireConnection;24;0;23;0
WireConnection;24;1;37;0
WireConnection;38;0;24;0
WireConnection;38;1;32;0
WireConnection;47;0;45;0
WireConnection;47;1;46;0
WireConnection;47;2;49;0
WireConnection;53;0;47;0
WireConnection;39;0;38;0
WireConnection;0;0;52;0
WireConnection;0;1;44;0
WireConnection;0;4;42;0
WireConnection;0;11;40;0
WireConnection;0;14;22;0
ASEEND*/
//CHKSM=7D7D273042B1F0A0FD7F1D2227C8A57D4ECEACD6