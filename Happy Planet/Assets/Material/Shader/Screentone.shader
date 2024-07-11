Shader "Tutorial/40_DitheredLighting" {
	//show values to edit in inspector
	Properties{
		_Color("Tint", Color) = (0, 0, 0, 1)
		_MainTex("Texture", 2D) = "white" {}
		[HDR] _Emission("Emission", color) = (0,0,0)

		_HalftonePattern("Halftone Pattern", 2D) = "white" {}

        _RemapInputMin ("Remap input min value", Range(0, 1)) = 0
        _RemapInputMax ("Remap input max value", Range(0, 1)) = 1
        _RemapOutputMin ("Remap output min value", Range(0, 1)) = 0
        _RemapOutputMax ("Remap output max value", Range(0, 1)) = 1
		
//Outline
        [KeywordEnum(NML,POS)] _OUTLINE("OUTLINE MODE", Float) = 0
        _Outline_Width ("Outline_Width", Float ) = 0
        _Farthest_Distance ("Farthest_Distance", Float ) = 100
        _Nearest_Distance ("Nearest_Distance", Float ) = 0.5
        _Outline_Sampler ("Outline_Sampler", 2D) = "white" {}
        _Outline_Color ("Outline_Color", Color) = (0.5,0.5,0.5,1)
        [Toggle(_)] _Is_BlendBaseColor ("Is_BlendBaseColor", Float ) = 0
        [Toggle(_)] _Is_LightColor_Outline ("Is_LightColor_Outline", Float ) = 1
        //v.2.0.4
        [Toggle(_)] _Is_OutlineTex ("Is_OutlineTex", Float ) = 0
        _OutlineTex ("OutlineTex", 2D) = "white" {}
        //Offset parameter
        _Offset_Z ("Offset_Camera_Z", Float) = 0
        //v.2.0.4.3 Baked Nrmal Texture for Outline
        [Toggle(_)] _Is_BakedNormal ("Is_BakedNormal", Float ) = 0
        _BakedNormal ("Baked Normal for Outline", 2D) = "white" {}
        //GI Intensity
        _GI_Intensity ("GI_Intensity", Range(0, 1)) = 0
        //For VR Chat under No effective light objects
        _Unlit_Intensity ("Unlit_Intensity", Range(0.001, 4)) = 1
        //v.2.0.5 
        [Toggle(_)] _Is_Filter_LightColor ("VRChat : SceneLights HiCut_Filter", Float ) = 0
        //Built-in Light Direction
        [Toggle(_)] _Is_BLD ("Advanced : Activate Built-in Light Direction", Float ) = 0
        _Offset_X_Axis_BLD (" Offset X-Axis (Built-in Light Direction)", Range(-1, 1)) = -0.05
        _Offset_Y_Axis_BLD (" Offset Y-Axis (Built-in Light Direction)", Range(-1, 1)) = 0.09
        [Toggle(_)] _Inverse_Z_Axis_BLD (" Inverse Z-Axis (Built-in Light Direction)", Float ) = 1
	}
	SubShader{
		//the material is completely non-transparent and is rendered at the same time as the other opaque geometry
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry"}
		Pass {
            Name "Outline"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            //#pragma fragmentoption ARB_precision_hint_fastest
            //#pragma multi_compile_shadowcaster
            //#pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal vulkan xboxone ps4 switch
            #pragma target 3.0
            //V.2.0.4
            #pragma multi_compile _IS_OUTLINE_CLIPPING_NO 
            #pragma multi_compile _OUTLINE_NML _OUTLINE_POS
            //アウトライン処理は以下のUCTS_Outline.cgincへ.
            #include "UCTS_Outline.cginc"
            ENDCG
        }
		CGPROGRAM

		//the shader is a surface shader, meaning that it will be extended by unity in the background to have fancy lighting and other features
		//our surface shader function is called surf and we use our custom lighting model
		//fullforwardshadows makes sure unity adds the shadow passes the shader might need
		#pragma surface surf Halftone fullforwardshadows
		#pragma target 3.0

        //basic properties
		sampler2D _MainTex;
		fixed4 _Color;
		half3 _Emission;

        //shading properties
		sampler2D _HalftonePattern;
		float4 _HalftonePattern_ST;

        ///remapping values
        float _RemapInputMin;
        float _RemapInputMax;
        float _RemapOutputMin;
        float _RemapOutputMax;

        //struct that holds information that gets transferred from surface to lighting function
		struct HalftoneSurfaceOutput {
			fixed3 Albedo;
			float2 ScreenPos;
			half3 Emission;
			fixed Alpha;
			fixed3 Normal;
		};

        // This function remaps values from a input to a output range
        float map(float input, float inMin, float inMax, float outMin,  float outMax)
        {
            //inverse lerp with input range
            float relativeValue = (input - inMin) / (inMax - inMin);
            //lerp with output range
            return lerp(outMin, outMax, relativeValue);
        }

		//our lighting function. Will be called once per light
		float4 LightingHalftone(HalftoneSurfaceOutput s, float3 lightDir, float atten) {
			//how much does the normal point towards the light?
			float towardsLight = dot(s.Normal, lightDir);
			//remap the value from -1 to 1 to between 0 and 1
			towardsLight = towardsLight * 0.5 + 0.5;
			//combine shadow and light and clamp the result between 0 and 1
			float lightIntensity = saturate(towardsLight * atten).r;

			//get halftone comparison value
            float halftoneValue = tex2D(_HalftonePattern, s.ScreenPos).r;

            //make lightness binary between fully lit and fully shadow based on halftone pattern (with a bit of antialiasing between)
            halftoneValue = map(halftoneValue, _RemapInputMin, _RemapInputMax, _RemapOutputMin, _RemapOutputMax);
            float halftoneChange = fwidth(halftoneValue) * 0.5;
			lightIntensity = smoothstep(halftoneValue - halftoneChange, halftoneValue + halftoneChange, lightIntensity);

			//combine the color
			float4 col;
			//intensity we calculated previously, diffuse color, light falloff and shadowcasting, color of the light
			col.rgb = lightIntensity * s.Albedo * _LightColor0.rgb;
			//in case we want to make the shader transparent in the future - irrelevant right now
			col.a = s.Alpha;

			return col;
		}

		//input struct which is automatically filled by unity
		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
		};

		//the surface shader function which sets parameters the lighting function then uses
		void surf(Input i, inout HalftoneSurfaceOutput o) {
			//set surface colors
			fixed4 col = tex2D(_MainTex, i.uv_MainTex);
			col *= _Color;
			o.Albedo = col.rgb;

			o.Emission = _Emission;

            //setup screenspace UVs for lighing function
			float aspect = _ScreenParams.x / _ScreenParams.y;
			o.ScreenPos = i.screenPos.xy / i.screenPos.w;
			o.ScreenPos = TRANSFORM_TEX(o.ScreenPos, _HalftonePattern);
			o.ScreenPos.x = o.ScreenPos.x * aspect;
		}
		ENDCG
	}
		FallBack "Standard"
}