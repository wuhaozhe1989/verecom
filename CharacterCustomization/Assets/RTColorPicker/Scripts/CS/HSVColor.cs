using UnityEngine;
using System.Collections;

//namespace RTColorPickerCS{
	public class HSVColor {
		public float h, s, v;
		
		public HSVColor(){
			h = 0.0f; s = 0.5f; v = 1.0f;
		}//HSVColor
		
		public HSVColor(float H, float S, float V){
			h = H; v = V; s = S;
		}//HSVColor
		
		public void SetValues(float H, float S, float V){
			h = H; s = S; v = V;
		}//SetValues
		
		public RGBColor ToRGBColor(RGBColor rgbColor){
			float min, chroma, hDash, x;
			
			if(rgbColor == null){
				rgbColor = new RGBColor();
			}//if
			else{
				rgbColor.r = rgbColor.g = rgbColor.b = 0.0f;
			}//else
			
			chroma = s * v;
			hDash = h / 60.0f;
			x = chroma * (1.0f - Mathf.Abs((hDash % 2.0f) - 1.0f));
			
			if(hDash < 1.0f){
				rgbColor.r = chroma;
				rgbColor.g = x;
			}//if
			else if(hDash < 2.0f){
				rgbColor.r = x;
				rgbColor.g = chroma;
			}//else if
			else if(hDash < 3.0f){
				rgbColor.g = chroma;
				rgbColor.b = x;
			}//else if
			else if(hDash < 4.0f){
				rgbColor.g = x;
				rgbColor.b = chroma;
			}//else if
			else if(hDash < 5.0f){
				rgbColor.r = x;
				rgbColor.b = chroma;
			}//else if
			else if(hDash < 6.0f){
				rgbColor.r = chroma;
				rgbColor.b = x;
			}//else if
			
			min = v - chroma;
			
			rgbColor.r += min;
			rgbColor.g += min;
			rgbColor.b += min;	
			
			return rgbColor;
		}//ToRGB
		
	}
//}
