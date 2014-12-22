
class RGBColor{
	var r : float = 0.0;
	var g : float = 0.0;
	var b : float = 0.0;
	var a : float = 1.0;
	
	//Store a Unity Color internally so that we're not creating
	//new color objects everytime we use ToUnityColor functions
	private var _unityColor : Color;
	
	function RGBColor(){}
	function RGBColor(R : float, G : float, B : float){
		r = R; g = G; b = B; a = 1.0;
		_unityColor = new Color(r, g, b, a);
	}//RGBColor
	
	function RGBColor(R : float, G : float, B : float, A : float){
		r = R; g = G; b = B; a = A;
		_unityColor = new Color(r, g, b, a);
	}//RGBColor
	
	function RGBColor(c : Color){
		r = c.r; g = c.g; b = c.b; a = c.a;
		_unityColor = c;
	}//RGBColor
	
	//Returns a Unity Color object
	function ToUnityColor(){
		_unityColor.r = r; _unityColor.g = g;
		_unityColor.b = b; _unityColor.a = a;
		
		return _unityColor;
	}//ToUnityColor
	
	//Returns a Unity Color object
	//Pass in value is used to over-ride the curent alpha value in the color. This is used in the color picker 
	//with an alpha of 1.0 so that the alpha doesn't effect our main sample colors
	function ToUnityColorWithAlpha(A : float){
		_unityColor.r = r; _unityColor.g = g;
		_unityColor.b = b; _unityColor.a = A;
		
		return _unityColor;
	}//ToUnityColorWithAlpha
	
	function FromUnityColor(c : Color){
		r = c.r; g = c.g; b = c.b; a = c.a;
		_unityColor = c;
	}//FromUnityColor
	
	function ToHSV(){
		var min : float = Mathf.Min(Mathf.Min(r, g), b);
		var max : float = Mathf.Max(Mathf.Max(r, g), b);
		var chroma : float = max - min;
		var hsv : HSVColor = new HSVColor();
		
		if(chroma != 0){
			if(r == max){
				hsv.h = (g - b) / chroma;
				if(hsv.h < 0){
					hsv.h += 6.0;
				}//if
			}//if
			else if(g == max){
				hsv.h = ((b - r) / chroma) + 2.0;
			}//else if
			else{
				hsv.h = ((r - g) / chroma) + 4.0;
			}//else
			
			hsv.h *= 60.0;
			hsv.s = chroma / max;
		}//if
		else{
			hsv.h = 0.0;
			hsv.s = 0.0;			
		}//else
		
		hsv.v = max;
		
		return hsv;
	}//ToHSV
	
	function ToHexStr(){
		var intR : int = r * 255;
		var hR = intR.ToString("X2");		
		
		var intG : int = g * 255;
		var hG = intG.ToString("X2");		
		
		var intB : int = b * 255;
		var hB = intB.ToString("X2");		
		
		return "#" + hR + hG + hB;
	}//ToHexStr
	
	function FromHexStr(HexStr : String){
		if(HexStr == "" || HexStr == "#"){
			r = g = b = 0.0;
			return;
		}//if
		
		//Strip out the hash sign if it's there
		if(HexStr[0] == '#'){
			HexStr = HexStr.Substring(1, HexStr.length-1);
		}//if
		
		if(HexStr.length > 6){
			HexStr = HexStr.Substring(0, 6);
		}//if
		
		if(HexStr.length % 2 != 0 && HexStr.length != 3){
			HexStr += "0";
		}//if	
		
		var RedChannel : int = 0;
		var GreenChannel : int = 0;
		var BlueChannel : int = 0;
		if(HexStr.length == 2){		
			if(!int.TryParse(HexStr, NumberStyles.HexNumber, null, RedChannel)){ RedChannel = 0; }//if		
			r = RedChannel / 255.0;
			//return Color(RedChannel / 255.0, 0, 0, 1);
		}//if
		else if(HexStr.length == 3){
			if(!int.TryParse(HexStr.Substring(0, 2), NumberStyles.HexNumber, null, RedChannel)){ RedChannel = 0; }//if
			r = RedChannel / 255.0;
			g = RedChannel / 255.0;
			b = RedChannel / 255.0;
			//return Color(RedChannel / 255.0, RedChannel / 255.0, RedChannel / 255.0, 1);
		}//else if
		else if(HexStr.length == 4){
			if(!int.TryParse(HexStr.Substring(0, 2), NumberStyles.HexNumber, null, RedChannel)){ RedChannel = 0; }//if
			if(!int.TryParse(HexStr.Substring(2, 2), NumberStyles.HexNumber, null, GreenChannel)){ GreenChannel = 0; }//if
			r = RedChannel / 255.0;
			g = RedChannel / 255.0;
			//return Color(RedChannel / 255.0, GreenChannel / 255.0, 0, 1);
		}//if
		else if(HexStr.length == 6){
			if(!int.TryParse(HexStr.Substring(0, 2), NumberStyles.HexNumber, null, RedChannel)){ RedChannel = 0; }//if
			if(!int.TryParse(HexStr.Substring(2, 2), NumberStyles.HexNumber, null, GreenChannel)){ GreenChannel = 0; }//if		
			if(!int.TryParse(HexStr.Substring(4, 2), NumberStyles.HexNumber, null, BlueChannel)){ BlueChannel = 0; }//if		
			r = RedChannel / 255.0;
			g = GreenChannel / 255.0;
			b = BlueChannel / 255.0; 
			//return Color(RedChannel / 255.0, GreenChannel / 255.0, BlueChannel / 255.0, 1);
		}//if
		
		
	}//FromHexStr
}//RGBColor