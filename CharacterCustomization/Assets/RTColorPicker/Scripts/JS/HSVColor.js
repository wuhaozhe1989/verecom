class HSVColor{
	var h : float = 0.0;
	var s : float = 0.5;
	var v : float = 1.0;
	
	function HSVColor(){}
	function HSVColor(H : float, S : float, V : float){
		h = H; s = S; v = V;
	}//HSVColor
	
	function SetValues(H : float, S : float, V : float){
		h = H; s = S; v = V;
	}//SetValues
	
	function ToRGBColor(rgbColor : RGBColor){
		var min : float;
		var chroma : float;
		var hDash : float;
		var x : float;
		if(rgbColor == null){
			rgbColor = new RGBColor();
		}//if
		else{
			rgbColor.r = rgbColor.g = rgbColor.b = 0.0;
		}//else
		
		chroma = s * v;
		hDash = h / 60.0;
		x = chroma * (1.0 - Mathf.Abs((hDash % 2.0) - 1.0));
		
		if(hDash < 1.0){
			rgbColor.r = chroma;
			rgbColor.g = x;
		}//if
		else if(hDash < 2.0){
			rgbColor.r = x;
			rgbColor.g = chroma;
		}//else if
		else if(hDash < 3.0){
			rgbColor.g = chroma;
			rgbColor.b = x;
		}//else if
		else if(hDash < 4.0){
			rgbColor.g = x;
			rgbColor.b = chroma;
		}//else if
		else if(hDash < 5.0){
			rgbColor.r = x;
			rgbColor.b = chroma;
		}//else if
		else if(hDash < 6.0){
			rgbColor.r = chroma;
			rgbColor.b = x;
		}//else if
		
		min = v - chroma;
		
		rgbColor.r += min;
		rgbColor.g += min;
		rgbColor.b += min;	
		
		return rgbColor;//new RGBColor(color);
	}//ToRGB
}//HSVColor