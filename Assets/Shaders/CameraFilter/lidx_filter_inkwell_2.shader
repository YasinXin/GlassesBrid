Shader "lidx/lidx_filter_inkwell_2"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_disableEffect("disableEffect",int)=0
		_contrast("contrast",float)=0.85
	}
		SubShader
	{
		// No culling or depth
		//Cull Off ZWrite Off ZTest Always
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma target 3.0
			struct appdata
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float2 texcoord : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = v.texcoord;
				return o;
			}

			uniform sampler2D _MainTex;
			uniform int _disableEffect;
			uniform float _contrast;
			float3 AdjustColorRG(float3 inputColor, float4 adjustAmount)
			{
				float3 outputColor = inputColor;


				float max_c = max(inputColor.r, inputColor.g);
				float min_c = min(inputColor.r, inputColor.g);
				float mid_c = inputColor.b;
				if (inputColor.b < min_c)
				{
					mid_c = min_c;
					min_c = inputColor.b;
				}
				if (inputColor.b > max_c)
				{
					mid_c = max_c;
					max_c = inputColor.b;
				}

				float interval_up = max_c - mid_c;
				float interval_down = mid_c - min_c;

				float redUpmax = inputColor.r;
				float redDownmax = -1.0 * (1.0 - redUpmax);
				float greenUpmax = inputColor.g;
				float greenDownmax = -1.0 * (1.0 - greenUpmax);
				float blueUpmax = inputColor.b;
				float blueDownmax = -1.0 * (1.0 - blueUpmax);
				float blackUpmax = dot(inputColor, float3(0.3, 0.6, 0.1));
				float blackDownmax = -1.0 * (1.0 - blackUpmax);

				float4 changeColor = adjustAmount;
				if (changeColor.r > redUpmax)
				{
					changeColor.r = redUpmax;
				}
				if (changeColor.r < redDownmax)
				{
					changeColor.r = redDownmax;
				}

				if (changeColor.g > greenUpmax)
				{
					changeColor.g = greenUpmax;
				}
				if (changeColor.g < greenDownmax)
				{
					changeColor.g = greenDownmax;
				}

				if (changeColor.b > blueUpmax)
				{
					changeColor.b = blueUpmax;
				}
				if (changeColor.b < blueDownmax)
				{
					changeColor.b = blueDownmax;
				}

				if (changeColor.a > blackUpmax)
				{
					changeColor.a = blackUpmax;
				}
				if (changeColor.a < blackDownmax)
				{
					changeColor.a = blackDownmax;
				}
				if ((max_c == inputColor.g&&mid_c == inputColor.b) || (max_c == inputColor.b&&mid_c == inputColor.g))
				{
					outputColor.r = outputColor.r - changeColor.r * interval_down;
					outputColor.g = outputColor.g - changeColor.g * interval_down;
					outputColor.b = outputColor.b - changeColor.b * interval_down;

					outputColor.r = outputColor.r - changeColor.a * interval_down;
					outputColor.g = outputColor.g - changeColor.a * interval_down;
					outputColor.b = outputColor.b - changeColor.a * interval_down;
				}
				else
				{
					outputColor = inputColor;
				}

				outputColor.r = clamp(outputColor.r, 0.0, 1.0);
				outputColor.g = clamp(outputColor.g, 0.0, 1.0);
				outputColor.b = clamp(outputColor.b, 0.0, 1.0);

				return outputColor;
			}

			float3 RGBToHSL(float3 color)
			{
				float3 hsl; // init to 0 to avoid warnings ? (and reverse if + remove first part)

				float fmin = min(min(color.r, color.g), color.b);    //Min. value of RGB
				float fmax = max(max(color.r, color.g), color.b);    //Max. value of RGB
				float delta = fmax - fmin;             //Delta RGB value

				hsl.z = (fmax + fmin) / 2.0; // Luminance

				if (delta == 0.0)		//This is a gray, no chroma...
				{
					hsl.x = 0.0;	// Hue
					hsl.y = 0.0;	// Saturation
				}
				else                                    //Chromatic data...
				{
					if (hsl.z < 0.5)
					{
						hsl.y = delta / (fmax + fmin);// Saturation
					}
					else
					{
						hsl.y = delta / (2.0 - fmax - fmin); // Saturation
					}
					 float deltaR = (((fmax - color.r) / 6.0) + (delta / 2.0)) / delta;
					 float deltaG = (((fmax - color.g) / 6.0) + (delta / 2.0)) / delta;
					 float deltaB = (((fmax - color.b) / 6.0) + (delta / 2.0)) / delta;

					if (color.r == fmax)
					{
						hsl.x = deltaB - deltaG; // Hue
					}
					else if (color.g == fmax)
					{
						hsl.x = (1.0 / 3.0) + deltaR - deltaB; // Hue
					}
					else if (color.b == fmax)
					{
						hsl.x = (2.0 / 3.0) + deltaG - deltaR; // Hue
					}

					if (hsl.x < 0.0)
					{
						hsl.x += 1.0; // Hue
					}
					else if (hsl.x > 1.0)
					{
						hsl.x -= 1.0; // Hue
					}
				}
				return hsl;
			}

			float HueToRGB( float f1, float f2, float hue)
			{
				if (hue < 0.0)
				{
					hue += 1.0;
				}
				else if (hue > 1.0)
				{
					hue -= 1.0;
				}
				float res;
				if ((6.0 * hue) < 1.0)
				{
					res = f1 + (f2 - f1) * 6.0 * hue;
				}
				else if ((2.0 * hue) < 1.0)
				{
					res = f2;
				}
				else if ((3.0 * hue) < 2.0)
				{
					res = f1 + (f2 - f1) * ((2.0 / 3.0) - hue) * 6.0;
				}
				else
				{
					res = f1;
				}
				return res;
			}

			float3 HSLToRGB(float3 hsl)
			{
				float3 rgb;
				if (hsl.y == 0.0)
				{
					rgb = float3(hsl.z,0.0,0.0); // Luminance
				}
				else
				{
					float f2;
					if (hsl.z < 0.5)
					{
						f2 = hsl.z * (1.0 + hsl.y);
					}
					else
					{
						f2 = (hsl.z + hsl.y) - (hsl.y * hsl.z);
					}

					float f1 = 2.0 * hsl.z - f2;

					rgb.r = HueToRGB(f1, f2, hsl.x + (1.0 / 3.0));
					rgb.g = HueToRGB(f1, f2, hsl.x);
					rgb.b = HueToRGB(f1, f2, hsl.x - (1.0 / 3.0));
				}
				return rgb;
			}

			float RGBToL(float3 color)
			{
				float fmin = min(min(color.r, color.g), color.b);    //Min. value of RGB
				float fmax = max(max(color.r, color.g), color.b);    //Max. value of RGB
				return (fmax + fmin) / 2.0; // Luminance
			}

			float4 frag(v2f i) : COLOR
			{
				float4 col;
				if (_disableEffect == 2) {
					col = float4(0, 0, 0, 1.0);
					return col;
				}
				float2 uv = i.texcoord.xy;
				if (_disableEffect == 1) {
					col =tex2D(_MainTex, uv);
					return col;
				}

				float3 sampleColor;

				float2 blurCoordinates0;
				float2 blurCoordinates1;
				float2 blurCoordinates2;
				float2 blurCoordinates3;
				float2 blurCoordinates4;
				float2 blurCoordinates5;
				float2 blurCoordinates6;
				float2 blurCoordinates7;
				float2 blurCoordinates8;
				float2 blurCoordinates9;
				float2 blurCoordinates10;
				float2 blurCoordinates11;

				float mul = 0.1;

				float mul_x = mul / 720.0;
				float mul_y = mul / 1280.0;

				blurCoordinates0 = uv + float2(0.0 * mul_x, -10.0 * mul_y);
				blurCoordinates1 = uv + float2(5.0 * mul_x, -8.0 * mul_y);
				blurCoordinates2 = uv + float2(8.0 * mul_x, -5.0 * mul_y);
				blurCoordinates3 = uv + float2(10.0 * mul_x, 0.0 * mul_y);
				blurCoordinates4 = uv + float2(8.0 * mul_x, 5.0 * mul_y);
				blurCoordinates5 = uv + float2(5.0 * mul_x, 8.0 * mul_y);
				blurCoordinates6 = uv + float2(0.0 * mul_x, 10.0 * mul_y);
				blurCoordinates7 = uv + float2(-5.0 * mul_x, 8.0 * mul_y);
				blurCoordinates8 = uv + float2(-8.0 * mul_x, 5.0 * mul_y);
				blurCoordinates9 = uv + float2(-10.0 * mul_x, 0.0 * mul_y);
				blurCoordinates10 = uv + float2(-8.0 * mul_x, -5.0 * mul_y);
				blurCoordinates11 = uv + float2(-5.0 * mul_x, -8.0 * mul_y);

				sampleColor = tex2D(_MainTex, uv).rgb * 20.0;

				sampleColor -= tex2D(_MainTex, blurCoordinates0).rgb;
				sampleColor -= tex2D(_MainTex, blurCoordinates1).rgb;
				sampleColor -= tex2D(_MainTex, blurCoordinates2).rgb;
				sampleColor -= tex2D(_MainTex, blurCoordinates3).rgb;
				sampleColor -= tex2D(_MainTex, blurCoordinates4).rgb;
				sampleColor -= tex2D(_MainTex, blurCoordinates5).rgb;
				sampleColor -= tex2D(_MainTex, blurCoordinates6).rgb;
				sampleColor -= tex2D(_MainTex, blurCoordinates7).rgb;
				sampleColor -= tex2D(_MainTex, blurCoordinates8).rgb;
				sampleColor -= tex2D(_MainTex, blurCoordinates9).rgb;
				sampleColor -= tex2D(_MainTex, blurCoordinates10).rgb;
				sampleColor -= tex2D(_MainTex, blurCoordinates11).rgb;

				sampleColor /= 8.0;

				float hue = tex2D(_MainTex, uv).r;
				hue = pow(hue, 1.2);

				float3 texel = tex2D(_MainTex, uv).rgb;
				float4 FragColor = float4(lerp(texel, sampleColor, 1.0 - hue), 1.0);

				float dist = distance(uv, float2(0.5, 0.5));
				dist = smoothstep(0.0, 1.0, dist);
				dist = pow(dist, 1.5);
				float4 darkenColor = FragColor;
				darkenColor.r = pow(darkenColor.r, 3.0);
				darkenColor.g = pow(darkenColor.g, 3.0);
				darkenColor.b = pow(darkenColor.b, 3.0);

				FragColor = lerp(FragColor, darkenColor, dist);

				float curveSpoint = 0.1;

				FragColor.r = (FragColor.r - curveSpoint)*(FragColor.r + curveSpoint - 2.0) / ((1.0 - curveSpoint)*(curveSpoint - 1.0));
				FragColor.g = (FragColor.g - curveSpoint)*(FragColor.g + curveSpoint - 2.0) / ((1.0 - curveSpoint)*(curveSpoint - 1.0));
				FragColor.b = (FragColor.b - curveSpoint)*(FragColor.b + curveSpoint - 2.0) / ((1.0 - curveSpoint)*(curveSpoint - 1.0));

				FragColor.rgb = AdjustColorRG(FragColor.rgb, float4(-0.1, -0.1, -0.4, 0.0));
				FragColor.rgb = clamp(FragColor.rgb, 0.0, 1.0);

				FragColor.r = pow(FragColor.r, 1.3);
				FragColor.g = pow(FragColor.g, 1.3);
				FragColor.b = pow(FragColor.b, 1.3);

				float3 shadowsShift = float3(0.2, 0.1, 0.0);
				float3 midtonesShift = float3(0.0, 0.0, 0.0);
				float3 highlightsShift = float3(0.0, 0.0, 0.0);
				int preserveLuminosity = 1;

				// Alternative way:
				float3 lightness = FragColor.rgb;

				float a = 0.25;
				float b = 0.333;
				float scale = 0.7;

				float3 shadows = shadowsShift * (clamp((lightness - b) / -a + 0.5, 0.0, 1.0) * scale);
				float3 midtones = midtonesShift * (clamp((lightness - b) / a + 0.5, 0.0, 1.0) * clamp((lightness + b - 1.0) / -a + 0.5, 0.0, 1.0) * scale);
				float3 highlights = highlightsShift * (clamp((lightness + b - 1.0) / a + 0.5, 0.0, 1.0) * scale);

				float3 newColor = FragColor.rgb + shadows + midtones + highlights;
				newColor = clamp(newColor, 0.0, 1.0);

				if (preserveLuminosity != 0) {
					float3 newHSL = RGBToHSL(newColor);
					float oldLum = RGBToL(FragColor.rgb);
					FragColor.rgb = HSLToRGB(float3(newHSL.x, newHSL.y, oldLum));
				}
				else {
					FragColor = float4(newColor.rgb, FragColor.w);
				}

				//float contrast = 0.85;
				return float4(((FragColor.rgb - float3(0.5,0.0,0.0)) * _contrast + float3(0.5,0.0,0.0)), FragColor.a);
			}
			ENDCG
		}
	}
}