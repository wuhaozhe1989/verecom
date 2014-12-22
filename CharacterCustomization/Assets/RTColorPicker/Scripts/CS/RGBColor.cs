using UnityEngine;
using System.Collections;
using System.Globalization;

//namespace RTColorPickerCS{
	
	public class RGBColor {
		public float r, g, b, a;
		
		//Store a Unity Color internally so that we're not creating
		//new color objects everytime we use ToUnityColor functions
		private Color _unityColor;
		
		public RGBColor(){
			r = g = b = 0.0f;
			a = 1.0f;
		}//RGBColor
		
		public RGBColor(float R, float G, float B){
			r = R; g = G; b = B; a = 1.0f;
			_unityColor = new Color(r, g, b, a);
		}//RGBColor
		
		public RGBColor(float R, float G, float B, float A){
			r = R; g = G; b = B; a = A;
			_unityColor = new Color(r, g, b, a);
		}//RGBColor
		
		public RGBColor(Color c){
			r = c.r; g = c.g; b = c.b; a = c.a;
			_unityColor = c;
		}//RGBColor
		
		//Returns a Unity Color object
		public Color ToUnityColor(){
			_unityColor.r = r; _unityColor.g = g;
			_unityColor.b = b; _unityColor.a = a;
			
			return _unityColor;
		}//ToUnityColor
		
		//Returns a Unity Color object
		//Pass in value is used to over-ride the curent alpha value in the color. This is used in the color picker 
		//with an alpha of 1.0 so that the alpha doesn't effect our main sample colors
		public Color ToUnityColorWithAlpha(float A){
			_unityColor.r = r; _unityColor.g = g;
			_unityColor.b = b; _unityColor.a = A;
			
			return _unityColor;
		}//ToUnityColorWithAlpha
		
		public void FromUnityColor(Color c){
			r = c.r; g = c.g; b = c.b; a = c.a;
			_unityColor = c;
		}//FromUnityColor
		
		public HSVColor ToHSV(){
			float min = Mathf.Min(Mathf.Min(r, g), b);
			float max = Mathf.Max(Mathf.Max(r, g), b);
			float chroma = max - min;
			HSVColor hsv = new HSVColor();
			
			if(chroma != 0){
				if(r == max){
					hsv.h = (g - b) / chroma;
					if(hsv.h < 0){
						hsv.h += 6.0f;
					}//if
				}//if
				else if(g == max){
					hsv.h = ((b - r) / chroma) + 2.0f;
				}//else if
				else{
					hsv.h = ((r - g) / chroma) + 4.0f;
				}//else
				
				hsv.h *= 60.0f;
				hsv.s = chroma / max;
			}//if
			else{
				hsv.h = 0.0f;
				hsv.s = 0.0f;			
			}//else
			
			hsv.v = max;
			
			return hsv;
		}//ToHSV
		
		public string ToHexStr(){
			int intR = Mathf.RoundToInt(r * 255);
			string hR = intR.ToString("X2");		
			
			int intG = Mathf.RoundToInt(g * 255);
			string hG = intG.ToString("X2");		
			
			int intB = Mathf.RoundToInt(b * 255);
			string hB = intB.ToString("X2");		
			
			return "#" + hR + hG + hB;
		}//ToHexStr
		
		public void FromHexStr(string HexStr){
			if(HexStr == "" || HexStr == "#"){
				r = g = b = 0.0f;
				return;
			}//if
			
			//Strip out the hash sign if it's there
			if(HexStr[0] == '#'){
				HexStr = HexStr.Substring(1, HexStr.Length-1);
			}//if
			
			if(HexStr.Length > 6){
				HexStr = HexStr.Substring(0, 6);
			}//if
			
			if(HexStr.Length % 2 != 0 && HexStr.Length != 3){
				HexStr += "0";
			}//if	
			
			int RedChannel = 0;
			int GreenChannel = 0;
			int BlueChannel = 0;
			if(HexStr.Length == 2){		
				if(!int.TryParse(HexStr, NumberStyles.HexNumber, null, out RedChannel)){ RedChannel = 0; }//if		
				r = (float)RedChannel / 255.0f;
				//return Color(RedChannel / 255.0, 0, 0, 1);
			}//if
			else if(HexStr.Length == 3){
				if(!int.TryParse(HexStr.Substring(0, 2), NumberStyles.HexNumber, null, out RedChannel)){ RedChannel = 0; }//if
				r = (float)RedChannel / 255.0f;
				g = (float)RedChannel / 255.0f;
				b = (float)RedChannel / 255.0f;
				//return Color(RedChannel / 255.0, RedChannel / 255.0, RedChannel / 255.0, 1);
			}//else if
			else if(HexStr.Length == 4){
				if(!int.TryParse(HexStr.Substring(0, 2), NumberStyles.HexNumber, null, out RedChannel)){ RedChannel = 0; }//if
				if(!int.TryParse(HexStr.Substring(2, 2), NumberStyles.HexNumber, null, out GreenChannel)){ GreenChannel = 0; }//if
				r = (float)RedChannel / 255.0f;
				g = (float)RedChannel / 255.0f;
				//return Color(RedChannel / 255.0, GreenChannel / 255.0, 0, 1);
			}//if
			else if(HexStr.Length == 6){
				if(!int.TryParse(HexStr.Substring(0, 2), NumberStyles.HexNumber, null, out RedChannel)){ RedChannel = 0; }//if
				if(!int.TryParse(HexStr.Substring(2, 2), NumberStyles.HexNumber, null, out GreenChannel)){ GreenChannel = 0; }//if		
				if(!int.TryParse(HexStr.Substring(4, 2), NumberStyles.HexNumber, null, out BlueChannel)){ BlueChannel = 0; }//if		
				r = (float)RedChannel / 255.0f;
				g = (float)GreenChannel / 255.0f;
				b = (float)BlueChannel / 255.0f; 
				//return Color(RedChannel / 255.0, GreenChannel / 255.0, BlueChannel / 255.0, 1);
			}//if
		}//FromHexStr
		
	}
//}
