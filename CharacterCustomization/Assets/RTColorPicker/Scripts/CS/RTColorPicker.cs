using UnityEngine;
using System;
using System.Globalization;
using System.Collections;

//namespace RTColorPickerCS{
	public enum CPMODE{ COLORWHEEL, COLORBOX }
	public enum CPORIENTATION{ VERTICAL, HORIZONTAL }
	
	public class RTColorPicker : MonoBehaviour {
		//Public Variables
		//--------------------------------------------------------------------------------
		public GUISkin skin;
		public string WindowTitle = "RTColorPicker";
		public int WindowID = 1;

		//Show Toggles
		public bool ShowColorSelection = true;
		public bool ShowHSVGroup = true;
		public bool ShowRGBGroup = true;
		public bool ShowAlphaSlider = true;
		public bool ShowHexField = true;
		public bool ShowColorsPanel = false;
		public bool ShowTooltips = true;

		//Allowables
		public bool AllowWindowDrag = true;
		public bool AllowEyeDropper = true;
		public bool AllowWheelBoxToggle = true;
		public bool AllowOrientationChange = true;
		public bool AllowColorPanel = true;
		public bool AllowCustomColors = true;

		public TextAsset DefaultColorsFile;
		public int MaxCustomColors = 24;

		public GUIStyle ColorWheelBtnStyle;
		public GUIStyle ColorBoxBtnStyle;
		public GUIStyle EyeDropperBtnStyle;
		public GUIStyle OrientationBtnStyle;
		public GUIStyle ColorPanelBtnStyle;
		public GUIStyle GroupToggleBtnStyle;
		public GUIStyle ColorBtnStyle;
		public GUIStyle ToolTipStyle;
		public GUIStyle ToolTipWordwrapStyle;

		public Texture2D ColorWheelTex;
		public Texture2D ColorWheelBG;

		public Texture2D SampleColorTexFull;
		public Texture2D SampleColorTexHalf;
		public Texture2D SampleColorAlphaBG;

		public Texture2D ColorBoxPickerOverlay;
		public Texture2D SelectedCustomColorHilight;
		public Texture2D HMeterTex;
		public Texture2D HMeterSelector;
		public Texture2D GradientBG;

		public Texture2D ColorBoxZoomGrid;
		public Texture2D ColorWheelZoomGrid;

		public Texture2D ArrowRightIcon;
		public Texture2D ArrowDownIcon;
		public Texture2D CursorTexture;

		
		
		//Private Variables
		//--------------------------------------------------------------------------------

		private bool _showCP = false;
		private Texture2D ZoomedTex;

		//Color Values
		private HSVColor _hsvColor = new HSVColor();
		private RGBColor _rgbColor = new RGBColor(1, 1, 1, 1);
		private RGBColor _oldRGBColor = new RGBColor(1, 1, 1, 1);
		private string _hexStr = "";

		//Component Rects
		private Rect _cpWinVerticalRect = new Rect(5, 5, 200, 400);
		private Rect _cpWinHorizontalRect = new Rect(5, 5, 400, 225);
		private Rect _cpWinRect = new Rect(5, 5, 200, 400);
		private Rect _cpHSliderVerticalRect = new Rect(170, 23, 20, 154); //Vertical Hue Slider
		private Rect _cpColorSelectRect = new Rect(10, 23, 154, 154); 
		private Vector2 _wheelCursorPos = Vector2.zero;
		private Color _colorPickBoxTintColor = Color.white;

		private CPMODE _cpMode = CPMODE.COLORWHEEL;
		private CPORIENTATION _cpOrientation = CPORIENTATION.VERTICAL;

		private Color[] _defaultColors;
		private ArrayList _customColors;
		private Rect _customColorButtonRect = new Rect(0, 0, 1, 1);
		private bool _customColorsDirty = false; //This is set to true when custom colors are altered
		private float _customColorsUpdateTime = 0.0f;
		private int _selectedCustomColorIndex = -1;

		private bool _showHSVSliders = true;
		private bool _showRGBSliders = true;

		private bool _draggingHMeter = false;
		private bool _draggingColorCursor = false;
		private bool _eyeDropperActive = false;

		private Vector2 _cpScrollVec = Vector2.zero;
		private int _cpScrollHeight = 0;
		private Vector2 _mousePosition = Vector2.zero;

		private string _tipText = "";

		private Color _activeColor = Color.black;
		private RGBColor _activeRGBColor = null;
		private Material _activeMaterial = null;
		private Light _activeLight = null;
		private Camera _activeCamera = null;

		//The tag string is passed in to the callback functions
		private string _colorTag = "";
		
		//This callback function is called every time the color changes
		//See DemoUI script for usage.
		public delegate void ColorChangeEventHandler(RGBColor rgbColor, string Tag);
		public event ColorChangeEventHandler OnColorChangeEvent;
		
		//Called when clicking cancel, pressing escape or clicking the X to close the color picker interface
		public delegate void ColorCancelEventHandler(RGBColor oldColor, RGBColor newColor);
		public event ColorCancelEventHandler OnColorCancelEvent;
		
		//Called when clicking the Ok button
		public delegate void ColorOKEventHandler(RGBColor oldColor, RGBColor newColor);
		public event ColorOKEventHandler OnColorOKEvent;
		
		//Called when custom colors have changed and need saving
		public delegate void CustomColorSaveEventHandler(string FileContent);
		public event CustomColorSaveEventHandler OnCustomColorSaveEvent;
		
		//Called when the custom colors have finished loading after
		//calling the LoadCustomColors function
		public delegate void CustomColorLoadedEventHandler();
		public event CustomColorLoadedEventHandler OnCustomColorLoadedEvent;
		
		void Start(){
			//Default Values
			_hsvColor.SetValues(0.0f, 1.0f, 1.0f);
			_colorPickBoxTintColor = new HSVColor(_hsvColor.h, 1.0f, 1.0f).ToRGBColor(null).ToUnityColor();
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
		
		public void Show(Color c, string colorTag){				
			InitFromColor(c);
			_activeRGBColor = new RGBColor(c);		
			_colorTag = colorTag;			
			_showCP = true;			
		}//Show

		public void Show(RGBColor c){
			ClearActiveColors();
			
			_activeRGBColor = c;			
			InitFromColor(_activeColor);
			
			_showCP = true;
		}//Show

		public void Show(Material m){
			ClearActiveColors();
			_activeMaterial = m;
			
			if(_activeMaterial != null){
				InitFromColor(_activeMaterial.color);
				_showCP = true;
			}//if
		}//Show

		public void Show(Light l){
			ClearActiveColors();
			_activeLight = l;
			if(_activeLight != null){
				InitFromColor(_activeLight.color);
				_showCP = true;
			}//if
		}//Show

		public void Show(Camera cam){
			ClearActiveColors();
			_activeCamera = cam;
			if(_activeCamera != null){	
				InitFromColor(_activeCamera.backgroundColor);
				_showCP = true;
			}//if
		}//Show
		
		public void Hide(){
			_showCP = false;
			ClearActiveColors();
		}//Hide

		public bool IsOpen(){
			return _showCP;
		}//IsOpen

		public RGBColor GetCurrentRGBColor(){	
			return _rgbColor;
		}//GetCurrentColor

		public Color GetCurrentUnityColor(){
			if(_rgbColor == null)
			return Color.black;
			
			return _rgbColor.ToUnityColor();
		}//GetCurrentUnityColor

		public void SetPos(int x, int y){
			_cpWinRect.x = x;
			_cpWinRect.y = y;
		}//SetPosition

		public void SetMode(CPMODE mode){
			_cpMode = mode;
		}//SetMode
		
		void Update(){
			if(!_showCP)
			return;
			
			_mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			_mousePosition.y = Screen.height - _mousePosition.y;
			
			Vector2 correctedMP = _mousePosition;
			if(_draggingHMeter){
				correctedMP.x = correctedMP.x - (_cpWinRect.x + _cpHSliderVerticalRect.x);
				correctedMP.y = Mathf.Clamp(correctedMP.y - (_cpWinRect.y + _cpHSliderVerticalRect.y), 0, _cpHSliderVerticalRect.height);		
				
				_hsvColor.h = (correctedMP.y / _cpHSliderVerticalRect.height) * 359;	
				_rgbColor = _hsvColor.ToRGBColor(_rgbColor);		
				_hexStr = _rgbColor.ToHexStr();
				_colorPickBoxTintColor = new HSVColor(_hsvColor.h, 1.0f, 1.0f).ToRGBColor(null).ToUnityColor();
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
					float distance = Mathf.Clamp(Vector2.Distance(correctedMP, new Vector2(_cpColorSelectRect.width/2, _cpColorSelectRect.width/2)), 0, _cpColorSelectRect.width/2);
					correctedMP.x = ((correctedMP.x / _cpColorSelectRect.width) * 2) - 1;
					correctedMP.y = ((correctedMP.y / _cpColorSelectRect.width) * 2) - 1;
					
					_hsvColor.h = Mathf.Atan2(-correctedMP.y, correctedMP.x);
					if(_hsvColor.h < 0) _hsvColor.h += Mathf.PI*2;
					_hsvColor.h =  _hsvColor.h * Mathf.Rad2Deg;
					
					_hsvColor.s = distance / (_cpColorSelectRect.width/2);
				}//else
				
				_rgbColor = _hsvColor.ToRGBColor(_rgbColor);
				_hexStr = _rgbColor.ToHexStr();
				_colorPickBoxTintColor = new HSVColor(_hsvColor.h, 1.0f, 1.0f).ToRGBColor(null).ToUnityColor();
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
					_colorPickBoxTintColor = new HSVColor(_hsvColor.h, 1.0f, 1.0f).ToRGBColor(null).ToUnityColor();
					AssignColor(_rgbColor);
					_eyeDropperActive = false;
				}//if
			}//if
			
			//Close ColorPicker if Escape is pressed
			if(Input.GetKeyUp(KeyCode.Escape)){
				AssignColor(_oldRGBColor);				
				OnColorCancelEvent(_oldRGBColor, _rgbColor);
				Hide();	
			}//if
			
			if(_customColorsDirty && _customColorsUpdateTime > 1.5f){		
				_customColorsUpdateTime = 0.0f;
				_customColorsDirty = false;
				OnCustomColorSaveEvent(BuildCustomColorFileContent());
			}//if
			else{
				_customColorsUpdateTime += Time.deltaTime;
			}//else			
		}//Update
		
		public void UpdateZoomedTexture(){
			//return WaitForEndOfFrame();
			ZoomedTex.ReadPixels(new Rect(_mousePosition.x, Screen.height - _mousePosition.y, ZoomedTex.width, ZoomedTex.height), 0, 0);
			int idx = 0;
			
			//If we're in color wheel mode let's make the zoomed texture look like a circle instead of a square
			//by setting pixel opacity to 0 based on distance from center of the texture
			if(_cpMode == CPMODE.COLORWHEEL){
				Color[] pixels = ZoomedTex.GetPixels();
				for(int x = 0; x < ZoomedTex.width; x++){
					for(int y = 0; y < ZoomedTex.height; y++){
						if(Mathf.Round(Mathf.Abs(Vector2.Distance(new Vector2(ZoomedTex.width/2, ZoomedTex.height/2), new Vector2(x,y)))) > ZoomedTex.width/2){
							pixels[idx].a = 0.0f;
						}//if
						
						idx++;
					}//for			
				}//for
				
				ZoomedTex.SetPixels(pixels);
			}//if
			
			ZoomedTex.Apply();
		}//UpdateZoomedTexture
		
		void OnGUI(){
			if(!_showCP)
			return;	
			
			if(skin != null)
			GUI.skin = skin;
			
			_cpWinRect.width = _cpOrientation == CPORIENTATION.VERTICAL ? _cpWinVerticalRect.width : _cpWinHorizontalRect.width;
			_cpWinRect.height = _cpOrientation == CPORIENTATION.VERTICAL ? _cpWinVerticalRect.height : _cpWinHorizontalRect.height;
			
			if(ShowColorsPanel){
				Rect PanelRect;
				if(_cpOrientation == CPORIENTATION.VERTICAL){
					PanelRect = new Rect(_cpWinRect.x + _cpWinRect.width-3, _cpWinRect.y+10, 200, _cpWinRect.height-20);
					if(PanelRect.x + PanelRect.width > Screen.width)
					PanelRect.x = _cpWinRect.x - (PanelRect.width - 3);
					DrawColorsPanel(PanelRect);
				}//if
				else{
					PanelRect = new Rect(_cpWinRect.x + 10, (_cpWinRect.y + _cpWinRect.height)-3, _cpWinRect.width-20, 160);
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
		
		public void DrawColorsPanel(Rect groupRect){
			GUI.Box(groupRect, "Colors");	
			
			GUI.BeginGroup(groupRect);
			_cpScrollVec = GUI.BeginScrollView(new Rect(10, 20, groupRect.width-15, groupRect.height-55), _cpScrollVec, new Rect(0, 0, groupRect.width-30, _cpScrollHeight));
			GUI.Label(new Rect(10, 0, groupRect.width, 20), "Default Colors");
			
			int XOffset = 10; 		
			_cpScrollHeight = 0;
			int numColumns = 6;
			if(_cpOrientation == CPORIENTATION.HORIZONTAL)
			numColumns = 12;
			
			for(int dfIdx = 0; dfIdx < _defaultColors.Length; dfIdx++){
				if((dfIdx % numColumns) == 0){
					XOffset = 10;
					_cpScrollHeight += 25;
				}//if
				GUI.color = _defaultColors[dfIdx];
				if(GUI.Button(new Rect(XOffset, _cpScrollHeight, 20, 20), "", ColorBtnStyle)){
					//if(ColorSampleBox(Rect(XOffset, _cpScrollHeight, 20, 20), new RGBColor(_defaultColors[dfIdx]), true)){		
					_rgbColor.FromUnityColor(_defaultColors[dfIdx]);
					_hsvColor = _rgbColor.ToHSV();
					_colorPickBoxTintColor = new HSVColor(_hsvColor.h, 1.0f, 1.0f).ToRGBColor(null).ToUnityColor();
					_hexStr = _rgbColor.ToHexStr();					
				}//if
				GUI.color = Color.white;
				
				XOffset += 25;
			}//for
			
			_cpScrollHeight += 30;
			
			if(AllowCustomColors){
				XOffset = 10;
				GUI.Label(new Rect(10, _cpScrollHeight, groupRect.width-20, 20), "Custom Colors");
				
				if(_customColors.Count == 0){
					_cpScrollHeight += 20;
					GUI.Label(new Rect(20, _cpScrollHeight, groupRect.width-20 ,20), "None");
					
				}//if
				else{
					for(int ccIdx = 0; ccIdx < _customColors.Count; ccIdx++){
						if((ccIdx % numColumns) == 0){
							XOffset = 10;
							_cpScrollHeight += 25;
						}//if
						
						_customColorButtonRect = new Rect(XOffset, _cpScrollHeight, 20, 20);
						GUI.color = (Color)_customColors[ccIdx];
						
						if(GUI.Button(_customColorButtonRect, "", ColorBtnStyle)){
							//if(ColorSampleBox(_customColorButtonRect, new RGBColor(_customColors[ccIdx]), true)){		
							_rgbColor.FromUnityColor((Color)_customColors[ccIdx]);
							_hsvColor = _rgbColor.ToHSV();
							_colorPickBoxTintColor = new HSVColor(_hsvColor.h, 1.0f, 1.0f).ToRGBColor(null).ToUnityColor();
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
				if(GUI.Button(new Rect(groupRect.width-172, groupRect.height-25, 90, 18), "Remove Color")){
					_customColors.RemoveAt(_selectedCustomColorIndex);
					_selectedCustomColorIndex = -1;
					OnCustomColorSaveEvent(BuildCustomColorFileContent());
				}//if
				GUI.enabled = true;
				
				string AddColorTip = "Add Custom Color";
				if(_customColors.Count >= MaxCustomColors){		
					GUI.enabled = false;
					AddColorTip = "Maximum Custom Colors Reached";
				}//if
				
				if(GUI.Button(new Rect(groupRect.width-80, groupRect.height-25, 70, 18), new GUIContent("Add Color", AddColorTip))){
					_customColors.Add(_rgbColor.ToUnityColor());
					OnCustomColorSaveEvent(BuildCustomColorFileContent());
				}//if
			}//if
			GUI.enabled = true;
			GUI.EndGroup();
			
			_tipText = GUI.tooltip;
		}//DrawColorsPanel
		
		public void DrawColorPickerWindow(int winID){
			if(GUI.Button(new Rect(_cpWinRect.width - 20, 2, 16, 16), "x")){
				AssignColor(_oldRGBColor);
				OnColorCancelEvent(_oldRGBColor, _rgbColor);
				Hide();		
			}//if
			
			int ContentYOffset = 22;
			if(ShowColorSelection){
				switch(_cpMode){
				case CPMODE.COLORWHEEL:
					DrawWheelColorPicker();
					break;
					
				case CPMODE.COLORBOX:
					DrawBoxColorPicker();
					break;
				}//switch
				
				ContentYOffset += (int)_cpColorSelectRect.x + (int)_cpColorSelectRect.height;
			}//if
			
			ContentYOffset += DrawButtonRow(new Rect(10, ContentYOffset, 180, 20));
			
			
			if(_cpOrientation == CPORIENTATION.VERTICAL){		
				//ContentYOffset += 50;
				
				//HSV Controls
				if(ShowHSVGroup){
					_showHSVSliders = DrawToggleGroupButton(new Rect(10, ContentYOffset, _cpWinRect.width-20, 16), "HSV", _showHSVSliders);
					ContentYOffset += 16;
					ContentYOffset += DrawHSVControls(new Rect(10, ContentYOffset, _cpWinRect.width-20, 60));
				}//if
				
				//RGBControls
				if(ShowRGBGroup){
					_showRGBSliders = DrawToggleGroupButton(new Rect(10, ContentYOffset, _cpWinRect.width-20, 16), "RGB", _showRGBSliders);
					ContentYOffset += 16;
					ContentYOffset += DrawRGBControls(new Rect(10, ContentYOffset, _cpWinRect.width-20, 60));
				}//if
				
				//Alpha/Opacity Slider
				if(ShowAlphaSlider){
					ContentYOffset += DrawAlphaSlider(new Rect(10, ContentYOffset, _cpWinRect.width-20, 20));
				}//if
				
				//Ok/Cancel Button
				if(GUI.Button(new Rect(_cpWinRect.width-40, ContentYOffset+4, 30, 18), "Ok")){
					OnColorOKEvent(_oldRGBColor, _rgbColor);					
					Hide();
				}//if
				
				if(GUI.Button(new Rect(_cpWinRect.width-90, ContentYOffset+4, 50, 18), "Cancel")){
					AssignColor(_oldRGBColor);
					OnColorCancelEvent(_oldRGBColor, _rgbColor);
					Hide();
				}//if
				
				//Hex Text Field
				DrawHexControl(new Rect(10, ContentYOffset+4, 95, 20));
				_cpWinVerticalRect.height = ContentYOffset + 30;
			}//if
			else{
				ContentYOffset = 22;
				if(ShowHSVGroup){
					_showHSVSliders = DrawToggleGroupButton(new Rect(200, ContentYOffset, _cpWinRect.width-210, 16), "HSV", _showHSVSliders);
					ContentYOffset += 16;
					ContentYOffset += DrawHSVControls(new Rect(200, ContentYOffset, _cpWinRect.width-210, 60));
				}//if
				
				//RGBControls
				if(ShowRGBGroup){
					_showRGBSliders = DrawToggleGroupButton(new Rect(200, ContentYOffset, _cpWinRect.width-210, 16), "RGB", _showRGBSliders);
					ContentYOffset += 16;
					ContentYOffset += DrawRGBControls(new Rect(200, ContentYOffset, _cpWinRect.width-210, 60));
				}//if
				
				//Alpha/Opacity Slider
				if(ShowAlphaSlider){
					ContentYOffset += DrawAlphaSlider(new Rect(200, ContentYOffset, _cpWinRect.width-210, 20));
				}//if
				
				//Ok/Cancel Button
				if(GUI.Button(new Rect(_cpWinRect.width-40, _cpWinRect.height-28, 30, 18), "Ok")){
					OnColorOKEvent(_oldRGBColor, _rgbColor);					
					Hide();
				}//if
				
				if(GUI.Button(new Rect(_cpWinRect.width-90, _cpWinRect.height-28, 50, 18), "Cancel")){
					AssignColor(_oldRGBColor);
					OnColorCancelEvent(_oldRGBColor, _rgbColor);
					Hide();
				}//if
				
				//Hex Text Field
				ContentYOffset += DrawHexControl(new Rect(200, ContentYOffset+4, 95, 20));
				_cpWinRect.height = ContentYOffset + 10;
			}//else
			
			if(AllowWindowDrag)
			GUI.DragWindow(new Rect(0, 0, _cpWinRect.width-20, 20));
			
			if(ShowTooltips && GUI.tooltip != ""){
				GUI.Label(new Rect(_cpColorSelectRect.x, _cpColorSelectRect.y + _cpColorSelectRect.height-20, _cpColorSelectRect.width, 20), GUI.tooltip, ToolTipStyle);		
			}//if	
		}//ColorPickerWindow
		
		void DrawWheelColorPicker(){
			if(!_eyeDropperActive){
				GUI.DrawTexture(_cpColorSelectRect, ColorWheelBG);
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, _hsvColor.v);
				GUI.DrawTexture(_cpColorSelectRect, ColorWheelTex);
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1.0f);
				
				_wheelCursorPos.x = Mathf.Cos(_hsvColor.h*(3.14f/180.0f));
				_wheelCursorPos.y = -Mathf.Sin(_hsvColor.h*(3.14f/180.0f));
				_wheelCursorPos *= _hsvColor.s;
				GUI.DrawTexture(new Rect((_cpColorSelectRect.x + (_cpColorSelectRect.width/2) + (_wheelCursorPos.x * _cpColorSelectRect.width/2)) - CursorTexture.width/2,
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
			
			GUI.DrawTexture(new Rect(_cpHSliderVerticalRect.x, (_cpHSliderVerticalRect.y + ((_hsvColor.h / 359.0f) * _cpHSliderVerticalRect.height)) - HMeterSelector.height/2, 20, 9), HMeterSelector);	
		}//DrawColorWheelPicker
		
		void DrawBoxColorPicker(){	
			//Box Color Selection Area
			if(!_eyeDropperActive){
				GUI.DrawTexture(_cpColorSelectRect, GradientBG);	
				GUI.color = _colorPickBoxTintColor;
				GUI.DrawTexture(_cpColorSelectRect, ColorBoxPickerOverlay);
				GUI.color = Color.white;
				
				GUI.DrawTexture(new Rect((_cpColorSelectRect.x + (_hsvColor.s * _cpColorSelectRect.width)) - CursorTexture.width/2, 
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
			
			GUI.DrawTexture(new Rect(_cpHSliderVerticalRect.x, (_cpHSliderVerticalRect.y + ((_hsvColor.h / 359.0f) * _cpHSliderVerticalRect.height)) - HMeterSelector.height/2, 20, 9), HMeterSelector);	
		}//DrawBoxColorPicker
		
		int DrawButtonRow(Rect groupRect){
			GUI.BeginGroup(groupRect);
			
			int ButtonXOffset = 0;
			if(AllowWheelBoxToggle){
				if(GUI.Button(new Rect(0, 2, 15, 15), new GUIContent("", "Toggle Color Wheel"), ColorWheelBtnStyle)){
					SetMode(CPMODE.COLORWHEEL);
				}//if
				
				/*if(GUI.Button(new Rect(20, 2, 15, 15), new GUIContent("", "Toggle Color Box"), ColorBoxBtnStyle)){
					SetMode(CPMODE.COLORBOX);
				}//if*/
				
				ButtonXOffset = 40;
			}//if		
			
			if(AllowOrientationChange){
				if(GUI.Button(new Rect(ButtonXOffset, 2, 15, 15), new GUIContent("", "Toggle Orientation"), OrientationBtnStyle)){
					if(_cpOrientation == CPORIENTATION.VERTICAL)
					_cpOrientation = CPORIENTATION.HORIZONTAL;
					else
					_cpOrientation = CPORIENTATION.VERTICAL;
				}//if
				
				ButtonXOffset += 20;
			}//if
			
			if(AllowColorPanel){
			if(GUI.Button(new Rect(20, 2, 15, 15), new GUIContent("", "Toggle Colors Panel"), ColorPanelBtnStyle)){
					ShowColorsPanel = !ShowColorsPanel;
				}//if
			}//if
			
			if(AllowEyeDropper){
				if(GUI.Button(new Rect(groupRect.width-80, 2, 15, 15), new GUIContent("", "Sample Color"), EyeDropperBtnStyle)){
					_eyeDropperActive = true;
				}//if
			}//if				
			
			ColorSampleBox(new Rect(groupRect.width-60, 0, 30, 20), _oldRGBColor, false);
			ColorSampleBox(new Rect(groupRect.width-30, 0, 30, 20), _rgbColor, false);		
			GUI.EndGroup();
			
			return (int)groupRect.height;
		}//DrawButtonRow
		
		int DrawHSVControls(Rect groupRect){
			GUI.BeginGroup(groupRect);		
			if(_showHSVSliders){
				_hsvColor.h = FloatValueSlider(new Rect(0, 0, groupRect.width, 18), _hsvColor.h, "H:", 0, 359, 1);
				_hsvColor.s = FloatValueSlider(new Rect(0, 20, groupRect.width, 18), _hsvColor.s, "S:", 0, 1, 100);
				_hsvColor.v = FloatValueSlider(new Rect(0, 40, groupRect.width, 18), _hsvColor.v, "V:", 0, 1, 100);
				
				if(GUI.changed){
					GUI.changed = false;
					_colorPickBoxTintColor = new HSVColor(_hsvColor.h, 1.0f, 1.0f).ToRGBColor(null).ToUnityColor();
					_rgbColor = _hsvColor.ToRGBColor(_rgbColor);
					_hexStr = _rgbColor.ToHexStr();
					AssignColor(_rgbColor);
				}//if
			}//if
			else{
				groupRect.height = 0;
			}//else
			GUI.EndGroup();
			
			return (int)groupRect.height;
		}//DrawHSVControls
		
		int DrawRGBControls(Rect groupRect){
			GUI.BeginGroup(groupRect);
			if(_showRGBSliders){
				_rgbColor.r = FloatValueSlider(new Rect(0, 0, groupRect.width, 18), _rgbColor.r, "R:", 0, 1, 255);
				_rgbColor.g = FloatValueSlider(new Rect(0, 20, groupRect.width, 18), _rgbColor.g, "G:", 0, 1, 255);
				_rgbColor.b = FloatValueSlider(new Rect(0, 40, groupRect.width, 18), _rgbColor.b, "B:", 0, 1, 255);			
				
				if(GUI.changed){
					GUI.changed = false;
					_hsvColor = _rgbColor.ToHSV();
					_hexStr = _rgbColor.ToHexStr();
					_colorPickBoxTintColor = new HSVColor(_hsvColor.h, 1.0f, 1.0f).ToRGBColor(null).ToUnityColor();
					AssignColor(_rgbColor);
				}//if
			}//if
			else{
				groupRect.height = 0;
			}//else	
			GUI.EndGroup();
			
			return (int)groupRect.height;
		}//DrawRGBControls
		
		int DrawAlphaSlider(Rect groupRect){
			GUI.BeginGroup(groupRect);
			_rgbColor.a = FloatValueSlider(new Rect(0, 0, groupRect.width, 18), _rgbColor.a, "A:", 0, 1, 100);
			
			if(GUI.changed)
			AssignColor(_rgbColor);
			GUI.EndGroup();
			
			return (int)groupRect.height;
		}//DrawAlphaSlider
		
		int DrawHexControl(Rect groupRect){
			GUI.BeginGroup(groupRect);
			if(ShowHexField){
				GUI.Label(new Rect(0, 0, 25, 18), "Hex:");
				_hexStr = GUI.TextField(new Rect(28, 0, groupRect.width-28, 18), _hexStr);
			}//if
			else
			groupRect.height = 0;
			
			GUI.EndGroup();
			
			if(GUI.changed){		
				if(_hexStr != ""){
					//Trim Hex String to correct length
					if(_hexStr[0] == '#' && _hexStr.Length > 7)
					_hexStr = _hexStr.Substring(0, 7);
					else if(_hexStr[0] != '#' && _hexStr.Length > 6)
					_hexStr = _hexStr.Substring(0, 6);
					
					_rgbColor.FromHexStr(_hexStr);
					_hsvColor = _rgbColor.ToHSV();		
					_colorPickBoxTintColor = new HSVColor(_hsvColor.h, 1.0f, 1.0f).ToRGBColor(null).ToUnityColor();
					AssignColor(_rgbColor);
				}//if
			}//if
			
			return (int)groupRect.height;
		}//DrawHexControl
		
		bool DrawToggleGroupButton(Rect rect, string label, bool toggleVal){
			if(toggleVal){
				if(GUI.Button(rect, new GUIContent(label, ArrowDownIcon), GroupToggleBtnStyle)){
					toggleVal = !toggleVal;
				}//if
			}//if
			else{
				if(GUI.Button(rect, new GUIContent(label, ArrowRightIcon), GroupToggleBtnStyle)){
					toggleVal = !toggleVal;
				}//if
			}//else
			
			return toggleVal;
		}//DrawToggleGroupButton
		
		public void LoadCustomColors(string dataStr){	
			if(dataStr != ""){		
				_customColors = ParseColorFileContent(dataStr);		
			}//if
			else{
				if(_customColors == null)
				_customColors = new ArrayList();
			}//else
			
			OnCustomColorLoadedEvent();			
		}//LoadCustomColors
		
		//--Private Functions----------------------------------------------------------------
		//-------------------------------------------------------------------------------------------
		private void InitFromColor(Color c){
			_oldRGBColor.FromUnityColor(c);
			_rgbColor.FromUnityColor(c);
			_hsvColor = _rgbColor.ToHSV();
			_hexStr = _rgbColor.ToHexStr();
			_colorPickBoxTintColor = new HSVColor(_hsvColor.h, 1.0f, 1.0f).ToRGBColor(null).ToUnityColor();
		}//InitFromColor
		
		private void LoadDefaultColors(){
			if(DefaultColorsFile == null){
				//If no color file is assigned then build up some default
				//colors
				_defaultColors = new Color[24];
				float shadeIndex = 1.0f;
				int HDeg = 0;
				for(int dfIdx = 0; dfIdx < _defaultColors.Length; dfIdx++){
					if((shadeIndex % 5) == 0){
						HDeg += 60;
						shadeIndex = 1.0f;
					}//if		
					
					_defaultColors[dfIdx] = new HSVColor(HDeg, shadeIndex/4, shadeIndex/4).ToRGBColor(null).ToUnityColor();
					
					shadeIndex++;
				}//for
			}//if
			else{
				if(DefaultColorsFile.text != ""){
					ArrayList colorArray = ParseColorFileContent(DefaultColorsFile.text);
					if(colorArray != null && colorArray.Count != 0)
					_defaultColors = colorArray.ToArray(typeof(Color)) as Color[];
				}//if
			}//else
		}//LoadDefaultColors
		
		private ArrayList ParseColorFileContent(string ColorFile){	
			char[] delimiters = new char[] { '\r', '\n' };
			string[] FileLines = ColorFile.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);	
			
			ArrayList Colors = new ArrayList();
			RGBColor RGBLineColor = new RGBColor();
			for(int LineIndex = 0; LineIndex < FileLines.Length; LineIndex++){
				RGBLineColor.FromHexStr(FileLines[LineIndex].Trim());
				Colors.Add(RGBLineColor.ToUnityColor());	
			}//for	
			
			return Colors;
		}//ParseColorFileContent
		
		private string BuildCustomColorFileContent(){
			string FileContent = "";
			
			foreach(Color cc in _customColors){
				FileContent += new RGBColor(cc).ToHexStr() + Environment.NewLine;
			}//for
			
			return FileContent;
		}//BuildCustomColorFileContent
		
		private void AssignColor(RGBColor rgbColor){
			if(_activeRGBColor != null){
				OnColorChangeEvent(rgbColor, _colorTag);				
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

		private void ClearActiveColors(){
			_activeRGBColor = null;
			_activeMaterial = null;
			_activeLight = null;
			_activeCamera = null;
		}//ClearActiveColors
		
		//--Custom GUI Components----------------------------------------------------
		//-------------------------------------------------------------------------------------------

		float FloatValueSlider(Rect rect, float value, string label, float minVal, float maxVal, float displayMax){
			int textFieldWidth = 30;
			float labelWidth = GUI.skin.GetStyle("label").CalcSize(new GUIContent(label)).x;
			float sliderWidth = rect.width-(labelWidth+textFieldWidth)-6;
			
			GUI.BeginGroup(rect);
			GUI.Label(new Rect(0, 0, labelWidth, rect.height), label);				
			value = GUI.HorizontalSlider(new Rect(labelWidth+3, 4, sliderWidth-3, rect.height), value, minVal, maxVal);
			
			float tFieldValue = Mathf.Round(value*displayMax);
			Rect valStrRect = new Rect(labelWidth+sliderWidth+3, 0, textFieldWidth, rect.height);
			string valStr = GUI.TextField(valStrRect, tFieldValue.ToString());
			
			if(GUI.changed){		
				try{				
					value = float.Parse(valStr)/displayMax;				
				}
				catch(Exception err){
					value = 0.0f;
				}
				
			}//if		
			
			GUI.EndGroup();
			
			return value;
		}//FloatValueSlider

		void FloatTip(){
			if(GUI.tooltip != ""){		
				Vector2 textSize = ToolTipStyle.CalcSize(new GUIContent(GUI.tooltip));
				float adjustedMouseX = _mousePosition.x;
				if(textSize.x < 150){
					adjustedMouseX -= textSize.x / 2.0f;
					GUI.Label(new Rect(adjustedMouseX, _mousePosition.y - (textSize.y + 4), textSize.x, textSize.y), GUI.tooltip, ToolTipStyle);
				}//if
				else{
					adjustedMouseX -= 75;
					float LabelHeight = ToolTipWordwrapStyle.CalcHeight(new GUIContent(GUI.tooltip), 150.0f);
					GUI.Label(new Rect(adjustedMouseX, _mousePosition.y - (LabelHeight + 4), 150, LabelHeight), GUI.tooltip, ToolTipWordwrapStyle);
				}//else
				
				GUI.tooltip = "";		
			}//if	
		}//FloatTip

		public bool ColorSampleBox(Rect rect, Color color, bool isButton){
			return ColorSampleBox(rect, new RGBColor(color), isButton);
		}//ColorSampleBox

		public bool ColorSampleBox(Rect rect, RGBColor rgbColor, bool isButton){
			bool ButtonPressed = false;
			
			GUI.DrawTexture(rect, SampleColorAlphaBG);
			GUI.color = rgbColor.ToUnityColor();		
			GUI.DrawTexture(rect, SampleColorTexFull);
			GUI.color = Color.white;
			
			//Old Color without Alpha
			GUI.color = rgbColor.ToUnityColorWithAlpha(1.0f);
			if(isButton){
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.0f);
				ButtonPressed = GUI.Button(rect, "");
				GUI.color =  new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1.0f);
			}//if
			
			GUI.DrawTexture(rect, SampleColorTexHalf);
			GUI.color = Color.white;
			
			return ButtonPressed;
		}//ColorSampleBox

		//--Utility---------------------------------------------------------------------------------
		//-------------------------------------------------------------------------------------------
		private void ErrorLog(string logTxt){
			if(logTxt != "")
				Debug.LogError("RTColorPicker Error :: " + logTxt);
		}//ErrorLog
		
	}
//}
