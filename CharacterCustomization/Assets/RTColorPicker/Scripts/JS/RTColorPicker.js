import System;
import System.Globalization;

//Public Variables
//--------------------------------------------------------------------------------
var skin : GUISkin;
var WindowTitle : String = "RTColorPicker";
var WindowID : int = 1;

//Show Toggles
var ShowColorSelection : boolean = true;
var ShowHSVGroup : boolean = true;
var ShowRGBGroup : boolean = true;
var ShowAlphaSlider : boolean = true;
var ShowHexField : boolean = true;
var ShowColorsPanel : boolean = false;
var ShowTooltips : boolean = true;

//Allowables
var AllowWindowDrag : boolean = true;
var AllowEyeDropper : boolean = true;
var AllowWheelBoxToggle : boolean = true;
var AllowOrientationChange : boolean = true;
var AllowColorPanel : boolean = true;
var AllowCustomColors : boolean = true;

var DefaultColorsFile : TextAsset;
var MaxCustomColors : int = 24;

var ColorWheelBtnStyle : GUIStyle;
var ColorBoxBtnStyle : GUIStyle;
var EyeDropperBtnStyle : GUIStyle;
var OrientationBtnStyle : GUIStyle;
var ColorPanelBtnStyle : GUIStyle;
var GroupToggleBtnStyle : GUIStyle;
var ColorBtnStyle : GUIStyle;
var ToolTipStyle : GUIStyle;
var ToolTipWordwrapStyle : GUIStyle;

var ColorWheelTex : Texture2D;
var ColorWheelBG : Texture2D;

var SampleColorTexFull : Texture2D;
var SampleColorTexHalf : Texture2D;
var SampleColorAlphaBG : Texture2D;

var ColorBoxPickerOverlay : Texture2D;
var SelectedCustomColorHilight : Texture2D;
var HMeterTex : Texture2D;
var HMeterSelector : Texture2D;
var GradientBG : Texture2D;

var ColorBoxZoomGrid : Texture2D;
var ColorWheelZoomGrid : Texture2D;

var ArrowRightIcon : Texture2D;
var ArrowDownIcon : Texture2D;
var CursorTexture : Texture2D;

enum CPMODE{ COLORWHEEL, COLORBOX }
enum CPORIENTATION{ VERTICAL, HORIZONTAL }

//Private Variables
//--------------------------------------------------------------------------------

private var _showCP : boolean = false;
private var ZoomedTex : Texture2D;

//Color Values
private var _hsvColor : HSVColor = new HSVColor();
private var _rgbColor : RGBColor = new RGBColor(1, 1, 1, 1);
private var _oldRGBColor : RGBColor = new RGBColor(1, 1, 1, 1);
private var _hexStr : String = "";

//Component Rects
private var _cpWinVerticalRect : Rect = Rect(5, 5, 200, 400);
private var _cpWinHorizontalRect : Rect = Rect(5, 5, 400, 225);
private var _cpWinRect : Rect = Rect(5, 5, 200, 400);
private var _cpHSliderVerticalRect = Rect(170, 23, 20, 154); //Vertical Hue Slider
private var _cpColorSelectRect : Rect = Rect(10, 23, 154, 154); 
private var _wheelCursorPos : Vector2 = Vector2.zero;
private var _colorPickBoxTintColor : Color = Color.white;

private var _cpMode : CPMODE = CPMODE.COLORBOX;
private var _cpOrientation : CPORIENTATION = CPORIENTATION.VERTICAL;

private var _defaultColors : Color[];
private var _customColors : ArrayList;
private var _customColorButtonRect : Rect = Rect(0, 0, 1, 1);
private var _customColorsDirty : boolean = false; //This is set to true when custom colors are altered
private var _customColorsUpdateTime : float = 0.0;
private var _selectedCustomColorIndex : int = -1;

private var _showHSVSliders : boolean = true;
private var _showRGBSliders : boolean = true;

private var _draggingHMeter : boolean = false;
private var _draggingColorCursor : boolean = false;
private var _eyeDropperActive : boolean = false;

private var _cpScrollVec : Vector2 = Vector2.zero;
private var _cpScrollHeight : int = 0;
private var _mousePosition : Vector2 = Vector2.zero;

private var _tipText : String = "";

private var _activeColor : Color = Color.black;
private var _activeRGBColor : RGBColor = null;
private var _activeMaterial : Material = null;
private var _activeLight : Light = null;
private var _activeCamera : Camera = null;

//The tag string is passed in to the callback functions
private var _colorTag : String = "";

//This callback function is called every time the color changed
//See DemoUI script for usage.
private var _colorChangeCallback : Function = null;

//Called when clicking cancel, pressing escape or clicking the X to close
private var _colorCancelCallback : Function = null;

//Called when clicking the Ok button
private var _colorOkCallback : Function = null;

//Called when custom colors have changed and need saving
private var _customColorsSaveCallback : Function = null;

//Called when the custom colors have finished loading after
//calling the LoadCustomColors function
private var _customColorsLoadedCallback : Function;

function Start(){
	//Default Values
	_hsvColor.SetValues(0, 1.0, 1.0);
	_colorPickBoxTintColor = new HSVColor(_hsvColor.h, 1.0, 1.0).ToRGBColor(null).ToUnityColor();
	_rgbColor = _hsvColor.ToRGBColor(_rgbColor);
	_hexStr = _rgbColor.ToHexStr();
	
	//Load Default Colors
	LoadDefaultColors();	
	
	//Load Custom Colors --  This basically ensures the _custumColors
	//Variable isn't null
	LoadCustomColors("");	
	
	//Create Eye Dropper Zoom Texture
	ZoomedTex = new Texture2D(11, 11, TextureFormat.ARGB32, false);
	ZoomedTex.filterMode = FilterMode.Point;
}//Start

function Show(c : Color, colorTag : String, changeCallback : Function){	
	if(changeCallback != null){
		InitFromColor(c);
		_activeRGBColor = new RGBColor(c);		
		_colorTag = colorTag;
		_colorChangeCallback = changeCallback;
		_showCP = true;
	}//if
	else{
		ErrorLog("Unable to show RTColorPicker. Passed in OnChange Callback is null.");
	}//else
}//Show

function Show(c : RGBColor){
	ClearActiveColors();
	
	_activeRGBColor = c;
	
	InitFromColor(_activeColor);
	_showCP = true;
}//Show

function Show(m : Material){
	ClearActiveColors();
	_activeMaterial = m;
	
	if(_activeMaterial != null){
		InitFromColor(_activeMaterial.color);
		_showCP = true;
	}//if
}//Show

function Show(l : Light){
	ClearActiveColors();
	_activeLight = l;
	if(_activeLight != null){
		InitFromColor(_activeLight.color);
		_showCP = true;
	}//if
}//Show

function Show(cam : Camera){
	ClearActiveColors();
	_activeCamera = cam;
	if(_activeCamera != null){	
		InitFromColor(_activeCamera.backgroundColor);
		_showCP = true;
	}//if
}//Show

function Hide(){
	_showCP = false;
	ClearActiveColors();
}//Hide

function IsOpen(){
	return _showCP;
}//IsOpen

function GetCurrentRGBColor(){	
	return _rgbColor;
}//GetCurrentColor

function GetCurrentUnityColor(){
	if(_rgbColor == null)
		return Color.black;
		
	return _rgbColor.ToUnityColor();
}//GetCurrentUnityColor

function SetPos(x : int, y : int){
	_cpWinRect.x = x;
	_cpWinRect.y = y;
}//SetPosition

function SetMode(mode : CPMODE){
	_cpMode = mode;
}//SetMode

function SetCallbackFunc(funcName : String, func : Function){
	funcName = funcName.ToLower();	
	var FuncAssigned : boolean = false;
	
	switch(funcName){
		case "onok":
			_colorOkCallback = func;
			FuncAssigned = true;
		break;
		
		case "oncancel":
			_colorCancelCallback = func;
			FuncAssigned = true;
		break;
		
		case "oncustomcolorsave":
			_customColorsSaveCallback = func;
			FuncAssigned = true;			
		break;
		
		case "oncustomcolorloaded":
			_customColorsLoadedCallback = func;
			FuncAssigned = true;
		break;
	}//switch
	
	if(!FuncAssigned){
		ErrorLog("Error Setting Callback Function. Please ensure you've called SetCallbackFunc with the correct function name and a valid function delegate.");
	}//if
}//SetCallbackFunc

function Update(){
	if(!_showCP)
		return;
		
	_mousePosition = Vector2(Input.mousePosition.x, Input.mousePosition.y);
	_mousePosition.y = Screen.height - _mousePosition.y;
	
	var correctedMP : Vector2 = _mousePosition;
	if(_draggingHMeter){
		correctedMP.x = correctedMP.x - (_cpWinRect.x + _cpHSliderVerticalRect.x);
		correctedMP.y = Mathf.Clamp(correctedMP.y - (_cpWinRect.y + _cpHSliderVerticalRect.y), 0, _cpHSliderVerticalRect.height);		
		
		_hsvColor.h = (correctedMP.y / _cpHSliderVerticalRect.height) * 359;	
		_rgbColor = _hsvColor.ToRGBColor(_rgbColor);		
		_hexStr = _rgbColor.ToHexStr();
		_colorPickBoxTintColor = new HSVColor(_hsvColor.h, 1.0, 1.0).ToRGBColor(null).ToUnityColor();
		AssignColor(_rgbColor);
		
		if(Input.GetMouseButtonUp(0))
			_draggingHMeter = false;
	}//if
	
	if(_draggingColorCursor){
		correctedMP.x = Mathf.Clamp(correctedMP.x - (_cpWinRect.x + _cpColorSelectRect.x), 0, _cpColorSelectRect.width);
		correctedMP.y = Mathf.Clamp(correctedMP.y - (_cpWinRect.y + _cpColorSelectRect.y), 0, _cpColorSelectRect.height);
		
		if(_cpMode == CPMODE.COLORBOX){
			_hsvColor.s = correctedMP.x / _cpColorSelectRect.width;
			_hsvColor.v = (_cpColorSelectRect.height - correctedMP.y) / _cpColorSelectRect.height;
		}//if
		else{
			var distance = Mathf.Clamp(Vector2.Distance(correctedMP, Vector2(_cpColorSelectRect.width/2, _cpColorSelectRect.width/2)), 0, _cpColorSelectRect.width/2);
			correctedMP.x = ((correctedMP.x / _cpColorSelectRect.width) * 2) - 1;
			correctedMP.y = ((correctedMP.y / _cpColorSelectRect.width) * 2) - 1;
			
			_hsvColor.h = Mathf.Atan2(-correctedMP.y, correctedMP.x);
			if(_hsvColor.h < 0) _hsvColor.h += Mathf.PI*2;
			_hsvColor.h =  _hsvColor.h * Mathf.Rad2Deg;
						
			_hsvColor.s = distance / (_cpColorSelectRect.width/2);
		}//else
		
		_rgbColor = _hsvColor.ToRGBColor(_rgbColor);
		_hexStr = _rgbColor.ToHexStr();
		_colorPickBoxTintColor = new HSVColor(_hsvColor.h, 1.0, 1.0).ToRGBColor(null).ToUnityColor();
		AssignColor(_rgbColor);		
		
		if(Input.GetMouseButtonUp(0))
			_draggingColorCursor = false;
	}//if
	
	if(_eyeDropperActive){
		UpdateZoomedTexture();

		if(Input.GetMouseButtonDown(0)){
			_rgbColor.FromUnityColor(ZoomedTex.GetPixel(5, 5));
			//_rgbColor.a = 1.0;
			_hsvColor = _rgbColor.ToHSV();
			_hexStr = _rgbColor.ToHexStr();
			_colorPickBoxTintColor = new HSVColor(_hsvColor.h, 1.0, 1.0).ToRGBColor(null).ToUnityColor();
			AssignColor(_rgbColor);
			_eyeDropperActive = false;
		}//if
	}//if
	
	//Close ColorPicker if Escape is pressed
	if(Input.GetKeyUp(KeyCode.Escape)){
		AssignColor(_oldRGBColor);
		
		if(_colorCancelCallback != null){
			try{
				_colorCancelCallback(_oldRGBColor, _rgbColor);
			}//try
			catch(e){
				ErrorLog("Unable to call Cancel function delegate. Please ensure it is defined properly. :: " + e.ToString());
			}//catch
		}//if
		
		Hide();	
	}//if
	
	if(_customColorsDirty && _customColorsUpdateTime > 1.5){		
		_customColorsUpdateTime = 0.0;
		_customColorsDirty = false;
		
		if(_customColorsSaveCallback != null){
			try{
				_customColorsSaveCallback(BuildCustomColorFileContent());
			}//try
			catch(e){
				ErrorLog("Unable to call OnCustomColorsSave function delegate. Please ensure it is defined properly. :: " + e.ToString());
			}//catch
		}//if
	}//if
	else{
		_customColorsUpdateTime += Time.deltaTime;
	}//else
}//Update

function UpdateZoomedTexture(){
	yield WaitForEndOfFrame();
	ZoomedTex.ReadPixels(Rect(_mousePosition.x, Screen.height - _mousePosition.y, ZoomedTex.width, ZoomedTex.height), 0, 0);
	var idx : int = 0;
	
	//If we're in color wheel mode let's make the zoomed texture look like a circle instead of a square
	//by setting pixel opacity to 0 based on distance from center of the texture
	if(_cpMode == CPMODE.COLORWHEEL){
		var pixels : Color[] = ZoomedTex.GetPixels();
		for(var x : int = 0; x < ZoomedTex.width; x++){
			for(var y : int = 0; y < ZoomedTex.height; y++){
				if(Mathf.Round(Mathf.Abs(Vector2.Distance(Vector2(ZoomedTex.width/2, ZoomedTex.height/2), Vector2(x,y)))) > ZoomedTex.width/2){
					pixels[idx].a = 0.0;
				}//if
				
				idx++;
			}//for			
		}//for
		
		ZoomedTex.SetPixels(pixels);
	}//if
	
	ZoomedTex.Apply();
}//UpdateZoomedTexture

function OnGUI(){
	if(!_showCP)
		return;	
	
	if(skin != null)
		GUI.skin = skin;
	
	
	
	_cpWinRect.width = _cpOrientation == CPORIENTATION.VERTICAL ? _cpWinVerticalRect.width : _cpWinHorizontalRect.width;
	_cpWinRect.height = _cpOrientation == CPORIENTATION.VERTICAL ? _cpWinVerticalRect.height : _cpWinHorizontalRect.height;
	
	if(ShowColorsPanel){
		var PanelRect : Rect;
		if(_cpOrientation == CPORIENTATION.VERTICAL){
			PanelRect = Rect(_cpWinRect.x + _cpWinRect.width-3, _cpWinRect.y+10, 200, _cpWinRect.height-20);
			if(PanelRect.x + PanelRect.width > Screen.width)
				PanelRect.x = _cpWinRect.x - (PanelRect.width - 3);
			DrawColorsPanel(PanelRect);
		}//if
		else{
			PanelRect = Rect(_cpWinRect.x + 10, (_cpWinRect.y + _cpWinRect.height)-3, _cpWinRect.width-20, 160);
			if(PanelRect.y + PanelRect.height > Screen.height)
				PanelRect.y = _cpWinRect.y - (PanelRect.height - 3);
			DrawColorsPanel(PanelRect);
		}//else
	}//if
	
	_cpWinRect = GUI.Window(WindowID, _cpWinRect, DrawColorPickerWindow, WindowTitle);
	
	//Reset Selected Custom Color if clicked outside the Main ColorPicker area
	if(!_customColorButtonRect.Contains(Event.current.mousePosition) && 
		!_cpWinRect.Contains(Event.current.mousePosition) &&
		Event.current.type == EventType.MouseDown){
		_selectedCustomColorIndex = -1;
	}//if
	
	//Handle Tooltips
	if(ShowTooltips){
		if(_tipText != "")
			GUI.tooltip = _tipText;
			
		FloatTip();
	}//if
}//OnGUI

function DrawColorsPanel(groupRect : Rect){
	GUI.Box(groupRect, "Colors");	
	
	GUI.BeginGroup(groupRect);
		_cpScrollVec = GUI.BeginScrollView(Rect(10, 20, groupRect.width-15, groupRect.height-55), _cpScrollVec, Rect(0, 0, groupRect.width-30, _cpScrollHeight));
		GUI.Label(Rect(10, 0, groupRect.width, 20), "Default Colors");
		
		var XOffset : int = 10; 		
		_cpScrollHeight = 0;
		var numColumns : int = 6;
		if(_cpOrientation == CPORIENTATION.HORIZONTAL)
			numColumns = 12;
			
		for(var dfIdx : int = 0; dfIdx < _defaultColors.Length; dfIdx++){
			if((dfIdx % numColumns) == 0){
				XOffset = 10;
				_cpScrollHeight += 25;
			}//if
			GUI.color = _defaultColors[dfIdx];
				if(GUI.Button(Rect(XOffset, _cpScrollHeight, 20, 20), "", ColorBtnStyle)){
				//if(ColorSampleBox(Rect(XOffset, _cpScrollHeight, 20, 20), new RGBColor(_defaultColors[dfIdx]), true)){		
					_rgbColor.FromUnityColor(_defaultColors[dfIdx]);
					_hsvColor = _rgbColor.ToHSV();
					_colorPickBoxTintColor = new HSVColor(_hsvColor.h, 1.0, 1.0).ToRGBColor(null).ToUnityColor();
					_hexStr = _rgbColor.ToHexStr();					
				}//if
			GUI.color = Color.white;
			
			XOffset += 25;
		}//for
		
		_cpScrollHeight += 30;
		
		if(AllowCustomColors){
			XOffset = 10;
			GUI.Label(Rect(10, _cpScrollHeight, groupRect.width-20, 20), "Custom Colors");
			
			if(_customColors.Count == 0){
				_cpScrollHeight += 20;
				GUI.Label(Rect(20, _cpScrollHeight, groupRect.width-20 ,20), "None");
				
			}//if
			else{
				for(var ccIdx : int = 0; ccIdx < _customColors.Count; ccIdx++){
					if((ccIdx % numColumns) == 0){
						XOffset = 10;
						_cpScrollHeight += 25;
					}//if
					
					_customColorButtonRect = Rect(XOffset, _cpScrollHeight, 20, 20);
					GUI.color = _customColors[ccIdx];
					
					if(GUI.Button(_customColorButtonRect, "", ColorBtnStyle)){
					//if(ColorSampleBox(_customColorButtonRect, new RGBColor(_customColors[ccIdx]), true)){		
						_rgbColor.FromUnityColor(_customColors[ccIdx]);
						_hsvColor = _rgbColor.ToHSV();
						_colorPickBoxTintColor = new HSVColor(_hsvColor.h, 1.0, 1.0).ToRGBColor(null).ToUnityColor();
						_hexStr = _rgbColor.ToHexStr();
						_selectedCustomColorIndex = ccIdx;
					}//if
					GUI.color = Color.white;
					
					if(ccIdx == _selectedCustomColorIndex){
						GUI.DrawTexture(_customColorButtonRect, SelectedCustomColorHilight);
					}//if
					
					XOffset += 25;
				}//for
			}//else
		}//if
		_cpScrollHeight += 25;
		GUI.EndScrollView();
		
		if(_selectedCustomColorIndex == -1){
			GUI.enabled = false;
		}//if
		
		if(AllowCustomColors){
			if(GUI.Button(Rect(groupRect.width-172, groupRect.height-25, 90, 18), "Remove Color")){
				_customColors.RemoveAt(_selectedCustomColorIndex);
				_selectedCustomColorIndex = -1;
				
				if(_customColorsSaveCallback != null){
					try{
						_customColorsSaveCallback(BuildCustomColorFileContent());
					}//try
					catch(e){
						ErrorLog("Unable to call OnCustomColorsSave function delegate. Please ensure it is defined properly. :: " + e.ToString());
					}//catch
				}//if
			}//if
			GUI.enabled = true;
			
			var AddColorTip = "Add Custom Color";
			if(_customColors.Count >= MaxCustomColors){		
				GUI.enabled = false;
				AddColorTip = "Maximum Custom Colors Reached";
			}//if
			
			if(GUI.Button(Rect(groupRect.width-80, groupRect.height-25, 70, 18), GUIContent("Add Color", AddColorTip))){
				_customColors.Add(_rgbColor.ToUnityColor());
				
				if(_customColorsSaveCallback != null){
					try{
						_customColorsSaveCallback(BuildCustomColorFileContent());
					}//try
					catch(e){
						ErrorLog("Unable to call OnCustomColorsSave function delegate. Please ensure it is defined properly. :: " + e.ToString());
					}//catch
				}//if
			}//if
		}//if
		GUI.enabled = true;
	GUI.EndGroup();
	
	_tipText = GUI.tooltip;
}//DrawColorsBox

function DrawColorPickerWindow(winID : int){
	if(GUI.Button(Rect(_cpWinRect.width - 20, 2, 16, 16), "x")){
		AssignColor(_oldRGBColor);
		
		if(_colorCancelCallback != null){
			try{
				_colorCancelCallback(_oldRGBColor, _rgbColor);
			}//try
			catch(e){
				ErrorLog("Unable to call Cancel function delegate. Please ensure it is defined properly. :: " + e.ToString());
			}//catch
		}//if
		
		Hide();		
	}//if
	
	var ContentYOffset : int = 22;
	if(ShowColorSelection){
		switch(_cpMode){
			case CPMODE.COLORWHEEL:
				DrawWheelColorPicker();
			break;
			
			case CPMODE.COLORBOX:
				DrawBoxColorPicker();
			break;
		}//switch
		
		ContentYOffset += _cpColorSelectRect.x + _cpColorSelectRect.height;
	}//if
	
	ContentYOffset += DrawButtonRow(Rect(10, ContentYOffset, 180, 20));
	
	
	if(_cpOrientation == CPORIENTATION.VERTICAL){		
		//ContentYOffset += 50;
		
		//HSV Controls
		if(ShowHSVGroup){
			_showHSVSliders = DrawToggleGroupButton(Rect(10, ContentYOffset, _cpWinRect.width-20, 16), "HSV", _showHSVSliders);
			ContentYOffset += 16;
			ContentYOffset += DrawHSVControls(Rect(10, ContentYOffset, _cpWinRect.width-20, 60));
		}//if
		
		//RGBControls
		if(ShowRGBGroup){
			_showRGBSliders = DrawToggleGroupButton(Rect(10, ContentYOffset, _cpWinRect.width-20, 16), "RGB", _showRGBSliders);
			ContentYOffset += 16;
			ContentYOffset += DrawRGBControls(Rect(10, ContentYOffset, _cpWinRect.width-20, 60));
		}//if
		
		//Alpha/Opacity Slider
		if(ShowAlphaSlider){
			ContentYOffset += DrawAlphaSlider(Rect(10, ContentYOffset, _cpWinRect.width-20, 20));
		}//if
		
		//Ok/Cancel Button
		if(GUI.Button(Rect(_cpWinRect.width-40, ContentYOffset+4, 30, 18), "Ok")){
			if(_colorOkCallback != null){
				try{
					_colorOkCallback(_oldRGBColor, _rgbColor);
				}//try
				catch(e){
					ErrorLog("Unable to call Ok function delegate. Please ensure it is defined properly. :: " + e.ToString());
				}//catch
			}//if
			
			Hide();
		}//if
		
		if(GUI.Button(Rect(_cpWinRect.width-90, ContentYOffset+4, 50, 18), "Cancel")){
			AssignColor(_oldRGBColor);
			
			if(_colorCancelCallback != null){
				try{
					_colorCancelCallback(_oldRGBColor, _rgbColor);
				}//try
				catch(e){
					ErrorLog("Unable to call Cancel function delegate. Please ensure it is defined properly. :: " + e.ToString());
				}//catch
			}//if
			
			Hide();
		}//if
		
		//Hex Text Field
		DrawHexControl(Rect(10, ContentYOffset+4, 95, 20));
		_cpWinVerticalRect.height = ContentYOffset + 30;
	}//if
	else{
		ContentYOffset = 22;
		if(ShowHSVGroup){
			_showHSVSliders = DrawToggleGroupButton(Rect(200, ContentYOffset, _cpWinRect.width-210, 16), "HSV", _showHSVSliders);
			ContentYOffset += 16;
			ContentYOffset += DrawHSVControls(Rect(200, ContentYOffset, _cpWinRect.width-210, 60));
		}//if
		
		//RGBControls
		if(ShowRGBGroup){
			_showRGBSliders = DrawToggleGroupButton(Rect(200, ContentYOffset, _cpWinRect.width-210, 16), "RGB", _showRGBSliders);
			ContentYOffset += 16;
			ContentYOffset += DrawRGBControls(Rect(200, ContentYOffset, _cpWinRect.width-210, 60));
		}//if
		
		//Alpha/Opacity Slider
		if(ShowAlphaSlider){
			ContentYOffset += DrawAlphaSlider(Rect(200, ContentYOffset, _cpWinRect.width-210, 20));
		}//if
		
		//Ok/Cancel Button
		if(GUI.Button(Rect(_cpWinRect.width-40, _cpWinRect.height-28, 30, 18), "Ok")){
			if(_colorOkCallback != null){
				try{
					_colorOkCallback(_oldRGBColor, _rgbColor);
				}//try
				catch(e){
					ErrorLog("Unable to call Ok function delegate. Please ensure it is defined properly. :: " + e.ToString());
				}//catch
			}//if
			
			Hide();
		}//if
		
		if(GUI.Button(Rect(_cpWinRect.width-90, _cpWinRect.height-28, 50, 18), "Cancel")){
			AssignColor(_oldRGBColor);
			
			if(_colorCancelCallback != null){
				try{
					_colorCancelCallback(_oldRGBColor, _rgbColor);
				}//try
				catch(e){
					ErrorLog("Unable to call Cancel function delegate. Please ensure it is defined properly. :: " + e.ToString());
				}//catch
			}//if
		
			Hide();
		}//if
		
		//Hex Text Field
		ContentYOffset += DrawHexControl(Rect(200, ContentYOffset+4, 95, 20));
		_cpWinRect.height = ContentYOffset + 10;
	}//else
		
	if(AllowWindowDrag)
		GUI.DragWindow(Rect(0, 0, _cpWinRect.width-20, 20));
	
	if(ShowTooltips && GUI.tooltip != ""){
		GUI.Label(Rect(_cpColorSelectRect.x, _cpColorSelectRect.y + _cpColorSelectRect.height-20, _cpColorSelectRect.width, 20), GUI.tooltip, ToolTipStyle);		
	}//if	
}//ColorPickerWindow

function DrawWheelColorPicker(){
	if(!_eyeDropperActive){
		GUI.DrawTexture(_cpColorSelectRect, ColorWheelBG);
		GUI.color.a = _hsvColor.v;
			GUI.DrawTexture(_cpColorSelectRect, ColorWheelTex);
		GUI.color.a = 1.0;
		
		_wheelCursorPos.x = Mathf.Cos(_hsvColor.h*(3.14/180));
		_wheelCursorPos.y = -Mathf.Sin(_hsvColor.h*(3.14/180));
		_wheelCursorPos *= _hsvColor.s;
		GUI.DrawTexture(Rect((_cpColorSelectRect.x + (_cpColorSelectRect.width/2) + (_wheelCursorPos.x * _cpColorSelectRect.width/2)) - CursorTexture.width/2,
								(_cpColorSelectRect.y + (_cpColorSelectRect.height/2) + (_wheelCursorPos.y * _cpColorSelectRect.width/2)) - CursorTexture.width/2,
								CursorTexture.width, CursorTexture.width), CursorTexture);
								
		if(_cpColorSelectRect.Contains(Event.current.mousePosition)){
			if(Event.current.type == EventType.MouseDown){
				_draggingColorCursor = true;
			}//if
		}//if
	}//if
	else{
		GUI.DrawTexture(_cpColorSelectRect, ZoomedTex);	
		GUI.DrawTexture(_cpColorSelectRect, ColorWheelZoomGrid);
	}//else
	
	GUI.DrawTexture(_cpHSliderVerticalRect, HMeterTex);
	if(_cpHSliderVerticalRect.Contains(Event.current.mousePosition)){
		if(Event.current.type == EventType.MouseDown){
			_draggingHMeter = true;
		}//if
	}//if
	
	GUI.DrawTexture(Rect(_cpHSliderVerticalRect.x, (_cpHSliderVerticalRect.y + ((_hsvColor.h / 359.0) * _cpHSliderVerticalRect.height)) - HMeterSelector.height/2, 20, 9), HMeterSelector);	
}//DrawColorWheelPicker

function DrawBoxColorPicker(){	
	//Box Color Selection Area
	if(!_eyeDropperActive){
		GUI.DrawTexture(_cpColorSelectRect, GradientBG);	
		GUI.color = _colorPickBoxTintColor;
		GUI.DrawTexture(_cpColorSelectRect, ColorBoxPickerOverlay);
		GUI.color = Color.white;
	
		GUI.DrawTexture(Rect((_cpColorSelectRect.x + (_hsvColor.s * _cpColorSelectRect.width)) - CursorTexture.width/2, 
								(_cpColorSelectRect.y + ((1 - _hsvColor.v) * _cpColorSelectRect.height)) - CursorTexture.width/2,
								CursorTexture.width, CursorTexture.width), CursorTexture);
								
		if(_cpColorSelectRect.Contains(Event.current.mousePosition)){
			if(Event.current.type == EventType.MouseDown){
				_draggingColorCursor = true;
			}//if
		}//if
	}//if
	else{		
		GUI.DrawTexture(_cpColorSelectRect, ZoomedTex);	
		GUI.DrawTexture(_cpColorSelectRect, ColorBoxZoomGrid);	
	}//else	
	
	//Vertical Hue Slider
	GUI.DrawTexture(_cpHSliderVerticalRect, HMeterTex);	

	if(_cpHSliderVerticalRect.Contains(Event.current.mousePosition)){
		if(Event.current.type == EventType.MouseDown){
			_draggingHMeter = true;
		}//if
	}//if
	
	GUI.DrawTexture(Rect(_cpHSliderVerticalRect.x, (_cpHSliderVerticalRect.y + ((_hsvColor.h / 359.0) * _cpHSliderVerticalRect.height)) - HMeterSelector.height/2, 20, 9), HMeterSelector);	
}//DrawBoxColorPicker

function DrawButtonRow(groupRect : Rect){
	GUI.BeginGroup(groupRect);
		
		var ButtonXOffset = 0;
		if(AllowWheelBoxToggle){
			if(GUI.Button(Rect(0, 2, 15, 15), GUIContent("", "Toggle Color Wheel"), ColorWheelBtnStyle)){
				SetMode(CPMODE.COLORWHEEL);
			}//if
		
			if(GUI.Button(Rect(20, 2, 15, 15), GUIContent("", "Toggle Color Box"), ColorBoxBtnStyle)){
				SetMode(CPMODE.COLORBOX);
			}//if
			
			ButtonXOffset = 40;
		}//if		
		
		if(AllowOrientationChange){
			if(GUI.Button(Rect(ButtonXOffset, 2, 15, 15), GUIContent("", "Toggle Orientation"), OrientationBtnStyle)){
				if(_cpOrientation == CPORIENTATION.VERTICAL)
					_cpOrientation = CPORIENTATION.HORIZONTAL;
				else
					_cpOrientation = CPORIENTATION.VERTICAL;
			}//if
		
			ButtonXOffset += 20;
		}//if
		
		if(AllowColorPanel){
			if(GUI.Button(Rect(ButtonXOffset, 2, 15, 15), GUIContent("", "Toggle Colors Panel"), ColorPanelBtnStyle)){
				ShowColorsPanel = !ShowColorsPanel;
			}//if
		}//if
		
		if(AllowEyeDropper){
			if(GUI.Button(Rect(groupRect.width-80, 2, 15, 15), GUIContent("", "Sample Color"), EyeDropperBtnStyle)){
				_eyeDropperActive = true;
			}//if
		}//if				
		
		ColorSampleBox(Rect(groupRect.width-60, 0, 30, 20), _oldRGBColor, false);
		ColorSampleBox(Rect(groupRect.width-30, 0, 30, 20), _rgbColor, false);		
	GUI.EndGroup();
	
	return groupRect.height;
}//DrawButtonRow

function DrawHSVControls(groupRect : Rect){
	GUI.BeginGroup(groupRect);		
		if(_showHSVSliders){
			_hsvColor.h = FloatValueSlider(Rect(0, 0, groupRect.width, 18), _hsvColor.h, "H:", 0, 359, 1);
			_hsvColor.s = FloatValueSlider(Rect(0, 20, groupRect.width, 18), _hsvColor.s, "S:", 0, 1.0, 100);
			_hsvColor.v = FloatValueSlider(Rect(0, 40, groupRect.width, 18), _hsvColor.v, "V:", 0, 1.0, 100);
		
			if(GUI.changed){
				GUI.changed = false;
				_colorPickBoxTintColor = new HSVColor(_hsvColor.h, 1.0, 1.0).ToRGBColor(null).ToUnityColor();
				_rgbColor = _hsvColor.ToRGBColor(_rgbColor);
				_hexStr = _rgbColor.ToHexStr();
				AssignColor(_rgbColor);
			}//if
		}//if
		else{
			groupRect.height = 0;
		}//else
	GUI.EndGroup();
	
	return groupRect.height;
}//DrawHSVControls

function DrawRGBControls(groupRect : Rect){
	GUI.BeginGroup(groupRect);
		if(_showRGBSliders){
			_rgbColor.r = FloatValueSlider(Rect(0, 0, groupRect.width, 18), _rgbColor.r, "R:", 0, 1, 255);
			_rgbColor.g = FloatValueSlider(Rect(0, 20, groupRect.width, 18), _rgbColor.g, "G:", 0, 1, 255);
			_rgbColor.b = FloatValueSlider(Rect(0, 40, groupRect.width, 18), _rgbColor.b, "B:", 0, 1, 255);			
	
			if(GUI.changed){
				GUI.changed = false;
				_hsvColor = _rgbColor.ToHSV();
				_hexStr = _rgbColor.ToHexStr();
				_colorPickBoxTintColor = new HSVColor(_hsvColor.h, 1.0, 1.0).ToRGBColor(null).ToUnityColor();
				AssignColor(_rgbColor);
			}//if
		}//if
		else{
			groupRect.height = 0;
		}//else	
	GUI.EndGroup();
	
	return groupRect.height;
}//DrawRGBControls

function DrawAlphaSlider(groupRect : Rect){
	GUI.BeginGroup(groupRect);
		_rgbColor.a = FloatValueSlider(Rect(0, 0, groupRect.width, 18), _rgbColor.a, "A:", 0, 1, 100);
		
		if(GUI.changed)
			AssignColor(_rgbColor);
	GUI.EndGroup();
	
	return groupRect.height;
}//DrawAlphaSlider

function DrawHexControl(groupRect : Rect){
	GUI.BeginGroup(groupRect);
	if(ShowHexField){
		GUI.Label(Rect(0, 0, 25, 18), "Hex:");
		_hexStr = GUI.TextField(Rect(28, 0, groupRect.width-28, 18), _hexStr);
	}//if
	else
		groupRect.height = 0;
	
	GUI.EndGroup();
	
	if(GUI.changed){		
		if(_hexStr != ""){
			//Trim Hex String to correct length
			if(_hexStr[0] == "#" && _hexStr.Length > 7)
				_hexStr = _hexStr.Substring(0, 7);
			else if(_hexStr[0] != "#" && _hexStr.Length > 6)
				_hexStr = _hexStr.Substring(0, 6);
			
			_rgbColor.FromHexStr(_hexStr);
			_hsvColor = _rgbColor.ToHSV();		
			_colorPickBoxTintColor = new HSVColor(_hsvColor.h, 1.0, 1.0).ToRGBColor(null).ToUnityColor();
			AssignColor(_rgbColor);
		}//if
	}//if
	
	return groupRect.height;
}//DrawHexControl

function DrawToggleGroupButton(rect : Rect, label : String, toggleVal : boolean){
	if(toggleVal){
		if(GUI.Button(rect, GUIContent(label, ArrowDownIcon), GroupToggleBtnStyle)){
			toggleVal = !toggleVal;
		}//if
	}//if
	else{
		if(GUI.Button(rect, GUIContent(label, ArrowRightIcon), GroupToggleBtnStyle)){
			toggleVal = !toggleVal;
		}//if
	}//else
	
	return toggleVal;
}//DrawToggleGroupButton

//--Private Functions----------------------------------------------------------------
//-------------------------------------------------------------------------------------------
private function InitFromColor(c : Color){
	_oldRGBColor.FromUnityColor(c);
	_rgbColor.FromUnityColor(c);
	_hsvColor = _rgbColor.ToHSV();
	_hexStr = _rgbColor.ToHexStr();
	_colorPickBoxTintColor = new HSVColor(_hsvColor.h, 1.0, 1.0).ToRGBColor(null).ToUnityColor();
}//InitFromColor

private function LoadDefaultColors(){
	if(DefaultColorsFile == null){
		//If no color file is assigned then build up some default
		//colors
		_defaultColors = new Color[24];
		var shadeIndex : float = 1.0;
		var HDeg : int = 0;
		for(var dfIdx : int = 0; dfIdx < _defaultColors.Length; dfIdx++){
			if((shadeIndex % 5) == 0){
				HDeg += 60;
				shadeIndex = 1.0;
			}//if		
		
			_defaultColors[dfIdx] = new HSVColor(HDeg, shadeIndex/4, shadeIndex/4).ToRGBColor(null).ToUnityColor();
		
			shadeIndex++;
		}//for
	}//if
	else{
		if(DefaultColorsFile.text != ""){
			var colorArray : ArrayList = ParseColorFileContent(DefaultColorsFile.text);
			if(colorArray != null && colorArray.Count != 0)
				_defaultColors = colorArray.ToArray(typeof(Color));
		}//if
	}//else
}//LoadDefaultColors

function LoadCustomColors(dataStr : String){	
	if(dataStr != ""){		
		_customColors = ParseColorFileContent(dataStr);		
	}//if
	else{
		if(_customColors == null)
			_customColors = new ArrayList();
	}//else
	
	if(_customColorsLoadedCallback != null){
		try{
			_customColorsLoadedCallback();
		}//try
		catch(e){
			ErrorLog("Unable to call OnLoadCustomColors function delegate. Please ensure it is defined properly. :: " + e.ToString());
		}//catch
	}//if
}//LoadCustomColors

private function ParseColorFileContent(ColorFile : String){	
	var FileLines = ColorFile.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);	
	
	var Colors : ArrayList = new ArrayList();
	var RGBLineColor : RGBColor = new RGBColor();
	for(var LineIndex : int = 0; LineIndex < FileLines.Length; LineIndex++){
		RGBLineColor.FromHexStr(FileLines[LineIndex].Trim());
		Colors.Add(RGBLineColor.ToUnityColor());	
	}//for	
	
	return Colors;
}//String

private function BuildCustomColorFileContent(){
	var FileContent : String = "";
	
	for(var cc : Color in _customColors){
		FileContent += new RGBColor(cc).ToHexStr() + Environment.NewLine;
	}//for
	
	return FileContent;
}//BuildCustomColorFileContent

private function AssignColor(rgbColor : RGBColor){
	if(_activeRGBColor != null){
		if(_colorChangeCallback != null){
			try{
				_colorChangeCallback(rgbColor, _colorTag);
			}//try
			catch(e){				
				ErrorLog("Unable to call OnChange function delegate. Please ensure it is defined properly. :: " + e.ToString());			
			}//catch
		}//if
	}//if
	else if(_activeMaterial != null){
		if(_activeMaterial.HasProperty("_Color"))
			_activeMaterial.color = rgbColor.ToUnityColor();
	}//else if
	else if(_activeLight != null){
		_activeLight.color = rgbColor.ToUnityColor();
		_activeLight.intensity = rgbColor.a;
	}//else if
	else if(_activeCamera != null){
		_activeCamera.backgroundColor = rgbColor.ToUnityColor();
	}//else if
	
	if(_selectedCustomColorIndex >= 0 && _selectedCustomColorIndex < _customColors.Count){
		_customColors[_selectedCustomColorIndex] = rgbColor.ToUnityColor();
		_customColorsDirty = true;
	}//if
}//AssignColor

private function ClearActiveColors(){
	_activeRGBColor = null;
	_activeMaterial = null;
	_activeLight = null;
	_activeCamera = null;
}//ClearActiveColors

//--Custom GUI Components----------------------------------------------------
//-------------------------------------------------------------------------------------------

function FloatValueSlider(rect : Rect, value : float, label : String, minVal : float, maxVal : float, displayMax : float) : float{
	var textFieldWidth = 30;
	var labelWidth : int = GUI.skin.GetStyle("label").CalcSize(GUIContent(label)).x;
	var sliderWidth : int = rect.width-(labelWidth+textFieldWidth)-6;
	
	GUI.BeginGroup(rect);
		GUI.Label(Rect(0, 0, labelWidth, rect.height), label);				
		value = GUI.HorizontalSlider(Rect(labelWidth+3, 4, sliderWidth-3, rect.height), value, minVal, maxVal);
		
		var tFieldValue : float = Mathf.Round(value*displayMax);
		var valStrRect : Rect = Rect(labelWidth+sliderWidth+3, 0, textFieldWidth, rect.height);
		var valStr = GUI.TextField(valStrRect, tFieldValue.ToString());
		
		if(GUI.changed){		
			try{				
				value = float.Parse(valStr)/displayMax;				
			}
			catch(err){
				value = 0;
			}
			
		}//if		
	GUI.EndGroup();
	
	
	
	return value;
}//FloatValueSlider

function FloatTip(){
	if(GUI.tooltip != ""){		
		var textSize = ToolTipStyle.CalcSize(GUIContent(GUI.tooltip));
		var adjustedMouseX = _mousePosition.x;
		if(textSize.x < 150){
			adjustedMouseX -= textSize.x / 2;
			GUI.Label(Rect(adjustedMouseX, _mousePosition.y - (textSize.y + 4), textSize.x, textSize.y), GUI.tooltip, ToolTipStyle);
		}//if
		else{
			adjustedMouseX -= 75;
			var LabelHeight = ToolTipWordwrapStyle.CalcHeight(GUIContent(GUI.tooltip), 150);
			GUI.Label(Rect(adjustedMouseX, _mousePosition.y - (LabelHeight + 4), 150, LabelHeight), GUI.tooltip, ToolTipWordwrapStyle);
		}//else
		
		GUI.tooltip = "";		
	}//if	
}//FloatTip

function ColorSampleBox(rect : Rect, color : Color, isButton){
	return ColorSampleBox(rect, new RGBColor(color), isButton);
}//ColorSampleBox

function ColorSampleBox(rect : Rect, rgbColor : RGBColor, isButton : boolean){
	var ButtonPressed : boolean = false;
	
	GUI.DrawTexture(rect, SampleColorAlphaBG);
	GUI.color = rgbColor.ToUnityColor();		
			GUI.DrawTexture(rect, SampleColorTexFull);
	GUI.color = Color.white;
		
	//Old Color without Alpha
	GUI.color = rgbColor.ToUnityColorWithAlpha(1.0);
		if(isButton){
			GUI.color.a = 0.0;
				ButtonPressed = GUI.Button(rect, "");
			GUI.color.a = 1.0;
		}//if
		
		GUI.DrawTexture(rect, SampleColorTexHalf);
	GUI.color = Color.white;
	
	return ButtonPressed;
}//ColorSampleBox

//--Utility---------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------
private function ErrorLog(logTxt : String){
	if(logTxt != "")
		Debug.LogError("RTColorPicker Error :: " + logTxt);
}//ErrorLog
