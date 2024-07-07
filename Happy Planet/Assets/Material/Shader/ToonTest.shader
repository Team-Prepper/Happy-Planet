Shader "Custom/ToonTest"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_Color("Main Tex Color", Color) = (1,1,1,1)
		_BumpMap("NormalMap", 2D) = "bump" {}

		_OutLineColor("Outline Color", Color) = (0, 0, 0, 0)
		_OutLineWidth("Outline Bold", Range(0, .1)) = 0.1

		_Band_Tex("Band LUT", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
			LOD 200

			cull front
			CGPROGRAM
			#pragma surface surf NoLight vertex:vert noshadow noambient
			#pragma target 3.0

			float4 _OutLineColor;
			float _OutLineWidth;

			void vert(inout appdata_full v) {
				v.vertex.xyz += v.normal.xyz * _OutLineWidth;
			}

			struct Input
			{
				float4 color;
			};

			void surf(Input IN, inout SurfaceOutput o)
			{

			}

			float4 LightingNoLight(SurfaceOutput s, float3 lightDir, float atten) {
				return float4(_OutLineColor.rgb, 1);
			}
			ENDCG

		cull back    //! 2Pass는 뒷면을 그리지 않는다.
		CGPROGRAM

		#pragma surface surf _BandedLighting    //! 커스텀 라이트 사용

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_Band_Tex;
			float2 uv_BumpMap;
		};

		struct SurfaceOutputCustom        //! Custom SurfaceOutput 구조체, BandLUT 텍스처를 넣기 위해 만듬
		{
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			half Specular;
			fixed Gloss;
			fixed Alpha;

			float3 BandLUT;
		};

		sampler2D _MainTex;
		sampler2D _Band_Tex;
		sampler2D _BumpMap;

		float4 _Color;

		void surf(Input IN, inout SurfaceOutputCustom o)
		{
			float4 fMainTex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = fMainTex.rgb;
			o.Alpha = 1.0f;

			float4 fBandLUT = tex2D(_Band_Tex, IN.uv_Band_Tex);
			o.BandLUT = fBandLUT.rgb;

			float3 fNormalTex = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Normal = fNormalTex;
		}

		//! 커스텀 라이트 함수
		float4 Lighting_BandedLighting(SurfaceOutputCustom s, float3 lightDir, float3 viewDir, float atten)
		{
			//! BandedDiffuse 조명 처리 연산
			float3 fBandedDiffuse;
			float fNDotL = dot(s.Normal, lightDir) * 0.5f + 0.5f;    //! Half Lambert 공식

			//! 0~1로 이루어진 fNDotL값을 3개의 값으로 고정함 <- Banded Lighting 작업
			//float fBandNum = 3.0f;
			//fBandedDiffuse = ceil(fNDotL * fBandNum) / fBandNum;             

			//! BandLUT 텍스처의 UV 좌표에 0~1로 이루어진 NDotL값을 넣어서 음영 색을 가져온다.
			fBandedDiffuse = tex2D(_Band_Tex, float2(fNDotL, 0.5f)).rgb;

			float3 fSpecularColor;
			float3 fHalfVector = normalize(lightDir + viewDir);
			float fHDotN = saturate(dot(fHalfVector, s.Normal));
			float fPowedHDotN = pow(fHDotN, 500.0f);

			//! smoothstep
			float fSpecularSmooth = smoothstep(0.005, 0.01f, fPowedHDotN);
			fSpecularColor = fSpecularSmooth * 1.0f;

			//! 최종 컬러 출력
			float4 fFinalColor;
			fFinalColor.rgb = ((s.Albedo * _Color) + fSpecularColor) *
								 fBandedDiffuse * _LightColor0.rgb * atten;
			fFinalColor.a = s.Alpha;

			return fFinalColor;
		}

		ENDCG
	}
}