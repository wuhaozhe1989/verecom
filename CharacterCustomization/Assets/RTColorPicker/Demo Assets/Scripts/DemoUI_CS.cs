using UnityEngine;
using System;
using System.IO;
using System.Collections;

public class DemoUI_CS : MonoBehaviour {
	public GUISkin Skin;
	public GUIStyle ColorBoxBtnStyle;

	//Objects
	//public GameObject[]body;
	public GameObject Ball01;
	public GameObject Ball02;
	//public Light SpotLight1;
	//public Light SpotLight2;
	//public ParticleAnimator ParticleSystem;
	//public Camera CameraObject;
	
	private RTColorPicker ColorPicker;
	private Rect _boxRect = new Rect(0, 150, 100, 100);
	private string _customColorsFilePath;
	
	// Use this for initialization
	void Start () {
		//body=new GameObject[2];
		ColorPicker = GameObject.Find("RTColorPicker").GetComponent<RTColorPicker>();
		ColorPicker.SetPos((int)_boxRect.width+5, 20);	
		
		//Set our callback functions for RTColorPicker (Optional)
		//ColorPicker.OnColorChangeEvent += OnColorChange;
		ColorPicker.OnColorCancelEvent += OnColorPickerCancel;
		ColorPicker.OnColorOKEvent += OnColorPickerOk;
		ColorPicker.OnCustomColorSaveEvent += OnColorPickerCustomColorsSave;
		ColorPicker.OnCustomColorLoadedEvent += OnColorPickerCustomColorsLoaded;
		
		/*
		ColorPicker.SetCallbackFunc("OnOK", OnColorPickerOk);
		ColorPicker.SetCallbackFunc("OnCancel", OnColorPickerCancel);
		ColorPicker.SetCallbackFunc("OnCustomColorSave", OnColorPickerCustomColorsSave);
		ColorPicker.SetCallbackFunc("OnCustomColorLoaded", OnColorPickerCustomColorsLoaded);
		*/
		//Only Setup the following if we're not running in a Webplayer
		if (!Application.isWebPlayer) {	
			//Setup the default filepath for our custom colors	
			if(Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
				_customColorsFilePath = Environment.CurrentDirectory + "/custom_colors.txt";
			else
				_customColorsFilePath = Environment.CurrentDirectory + "\\custom_colors.txt";
			
			//Load Custom Colors File
			LoadCustomColors();
		}//if
	}//Start
	
	void OnGUI () {
		if(Skin != null)
			GUI.skin = Skin;	
		
		GUI.Label(new Rect(_boxRect.width + 10, 2, 300, 20), "Click on a Color Box To The Left To Open Color Picker");
		GUI.Box(_boxRect, "Color Chooser");
		if(ColorPicker == null){
			GUI.Label(new Rect(10, 40, _boxRect.width-80, 20), "Error Finding RTColorPicker");
		}//if
		else{
			GUI.BeginGroup(_boxRect);
			
			//Left Column 
			GUI.BeginGroup(new Rect(10, 20, _boxRect.width/2-5, 140));
			//Ball01
			GUI.Label(new Rect(0, 0, _boxRect.width-20, 18), "Face");				
			if(ColorPicker.ColorSampleBox(new Rect(0, 20, 30, 20), Ball01.transform.renderer.material.color, true)){
				//Pass in the object material. The materials main color
				//will automatically be the colorpicker target.
				ColorPicker.Show(Ball01.transform.renderer.material);


			}//if



			
			
			//SpotLight1
			/*GUI.Label(new Rect(0, 45, _boxRect.width/2-10, 20), "SpotLight 1");				
			if(ColorPicker.ColorSampleBox(new Rect(0, 65, 30, 20), SpotLight1.color, true)){
				//Pass in the Light Object. The light color will be the
				//colorpicker target. The Alpha value of the color will
				//adjust the light intensity.
				ColorPicker.Show(SpotLight1);
			}//if*/				
			
			//Camera Color
			/*GUI.Label(new Rect(0, 90, _boxRect.width/2-10, 18), "Camera");								
			if(ColorPicker.ColorSampleBox(new Rect(0, 110, 30, 20), CameraObject.backgroundColor, true)){
				//Pass in the Camera Object. The camera background color
				//will be the colorpicker target.
				ColorPicker.Show(CameraObject);
			}//if*/				
			
			GUI.EndGroup();
			
			//Right Column
			GUI.BeginGroup(new Rect(_boxRect.width/2, 20, _boxRect.width/2-5, 140));
			//Ball02
			GUI.Label(new Rect(0, 0, _boxRect.width-20, 18), "Ball 2");			
			if(ColorPicker.ColorSampleBox(new Rect(0, 20, 30, 20), Ball02.transform.renderer.material.color, true)){
				//Pass in the object material. The materials main color
				//will automatically be the colorpicker target.
				ColorPicker.Show(Ball02.transform.renderer.material);
			}//if
			
			//SpotLight2
			/*GUI.Label(new Rect(0, 45, _boxRect.width/2-10, 20), "SpotLight 1");				
			if(ColorPicker.ColorSampleBox(new Rect(0, 65, 30, 20), SpotLight2.color, true)){
				//Pass in the Light Object. The light color will be the
				//colorpicker target. The Alpha value of the color will
				//adjust the light intensity.
				ColorPicker.Show(SpotLight2);
			}//if*/			
			
			//Ambient Light
			//GUI.Label(new Rect(0, 90, _boxRect.width/2-10, 20), "Ambient Light");
			if(ColorPicker.ColorSampleBox(new Rect(0, 110, 30, 20), RenderSettings.ambientLight, true)){
				//Here we pass in the color, target string, and a callback
				//function (defined below)
				//The callback function will be called everytime the color changes on the colorpicker
				ColorPicker.Show(RenderSettings.ambientLight, "ambient_light");//, OnColorChange);
			}//if				
			
			GUI.EndGroup();
			
			//Particle Colors
			//Iterator over the Particle Animator colorAnimations array
			GUI.BeginGroup(new Rect(10, 160, _boxRect.width - 20, 45));
			int ButtonXOffset = 0;
			int pcIdx = 0;
			
			/*GUI.Label(new Rect(0, 0, _boxRect.width-20, 20), "Particle Colors");				
			foreach(Color pc in ParticleSystem.colorAnimation){					
				if(ColorPicker.ColorSampleBox(new Rect(ButtonXOffset, 20, 30, 20), pc, true)){					
					ColorPicker.Show(pc, "pColor_" + pcIdx);//, OnColorChange);
				}//if				
				
				ButtonXOffset += 35;
				pcIdx++;
			}//for*/
			GUI.EndGroup();
			
			//GUI.BeginGroup(new Rect(10, 210, _boxRect.width-20, 140));
			//GUI.Label(new Rect(0, 0, _boxRect.width, 20), "RTColorPicker Options");
			//ColorPicker.AllowWindowDrag = GUI.Toggle(new Rect(0, 20, _boxRect.width/2-10, 20), ColorPicker.AllowWindowDrag, "Can Drag");
			//ColorPicker.ShowColorSelection = GUI.Toggle(new Rect(_boxRect.width/2, 20, _boxRect.width/2-10, 20), ColorPicker.ShowColorSelection, "Color Select");
			
			//ColorPicker.ShowHSVGroup = GUI.Toggle(new Rect(0, 40, _boxRect.width/2-10, 20), ColorPicker.ShowHSVGroup, "HSV Group");
			//ColorPicker.ShowRGBGroup = GUI.Toggle(new Rect(_boxRect.width/2, 40, _boxRect.width/2-10, 20), ColorPicker.ShowRGBGroup, "RGB Group");
			
			//ColorPicker.ShowAlphaSlider = GUI.Toggle(new Rect(0, 60, _boxRect.width/2-10, 20), ColorPicker.ShowAlphaSlider, "Alpha Slider");
			//ColorPicker.AllowEyeDropper = GUI.Toggle(new Rect(_boxRect.width/2, 60, _boxRect.width/2-10, 20), ColorPicker.AllowEyeDropper, "Eye Dropper");
			
			//ColorPicker.ShowHexField = GUI.Toggle(new Rect(0, 80, _boxRect.width/2-10, 20), ColorPicker.ShowHexField, "Hex Field");
			//ColorPicker.AllowWheelBoxToggle = GUI.Toggle(new Rect(_boxRect.width/2, 80, _boxRect.width/2-10, 20), ColorPicker.AllowWheelBoxToggle, "Select Btns");
			
			//ColorPicker.AllowOrientationChange = GUI.Toggle(new Rect(0, 100, _boxRect.width/2-10, 20), ColorPicker.AllowOrientationChange, "Orientation Btn");
			//ColorPicker.AllowColorPanel = GUI.Toggle(new Rect(_boxRect.width/2, 100, _boxRect.width/2-10, 20), ColorPicker.AllowColorPanel, "Color Panel");
			
			//ColorPicker.AllowCustomColors = GUI.Toggle(new Rect(0, 120, _boxRect.width/2-10, 20), ColorPicker.AllowCustomColors, "Custom Colors");
			//ColorPicker.ShowTooltips = GUI.Toggle(new Rect(_boxRect.width/2, 120, _boxRect.width/2-10, 20), ColorPicker.ShowTooltips, "Tooltips");
			GUI.EndGroup();
			
			//GUI.EndGroup();
		}//else
		
	}//OnGUI
	
	/*In this example we'll be loading a custom colors file from the local system. This is only 
	possible if we are not in the WebPlayer. Webplayer games could load the Custom Colors
	using WWW or getting the data from a external javascript call.

	The file we are loading is saved in the OnColorPickercustomColorsSave function below*/
	void LoadCustomColors(){
		if (!Application.isWebPlayer) {		
			if(File.Exists(_customColorsFilePath)){
				StreamReader SR = File.OpenText(_customColorsFilePath);
				string Line = SR.ReadLine();
				string FileContent = "";
				while(Line != null){
					//Add the Line Terminator as the LoadCustomColors
					//function splits the values by line terminator
					FileContent += Line + Environment.NewLine;
					Line = SR.ReadLine();
				}//while
				
				SR.Close();
				
				ColorPicker.LoadCustomColors(FileContent);			
			}//if		
		}//if
	}//LoadCustomColors
	
	//RTColorPicker Callbacks -----------------------------------------------------------------------
	//------------------------------------------------------------------------------------------------------------

	//We pass this function in the RTColorPicker Show function for dealing
	//with standard unity colors. When passed in this function will get called
	//any time the color changes in the RTColorPicker window. 
	//rgbColor: The passed back color from the colorpicker as an RGBColor object
	//tagStr: This is the string that was passed in to the show function. We can
	//use this to handle multiple colors with the same callback function as shown
	//below
	/*void OnColorChange(RGBColor rgbColor, string tagStr){
		//Handle "ambient_light" tag
		if(tagStr == "ambient_light"){
			RenderSettings.ambientLight = rgbColor.ToUnityColor();		
		}//if
		else{	
			//Handle Particle Color Changes
			int pcIdx = 0;
			Color[] pColors = ParticleSystem.colorAnimation;
			foreach(Color pc in pColors){
				if(tagStr == "pColor_" + pcIdx){
					pColors[pcIdx] = rgbColor.ToUnityColor();
					ParticleSystem.colorAnimation = pColors;
				}//if
				
				pcIdx++;
			}//for
		}//else	
	}//OnColorChange*/

	//Callback function when the RTColorPicker cancel or x button is clicked to
	//close the picker. 
	//oldColor: The color that was first sent to the ColorPicker before changes
	//newColor: The new color that was selected. This is not assigned to the target by the color
	//picker when cancel or x is clicked. This function can over-ride that behavior if desired by
	//manually applying the new color.
	void OnColorPickerCancel(RGBColor oldColor, RGBColor newColor){
		Debug.Log("RTColorPicker Cancel/X or Escape pressed");
	}//OnColorPickerCancel

	//Callback function when the RTColorPicker Ok button is clicked.
	//oldColor: The color that was first sent to the ColorPicker before changes
	//newcolor: The color that was selected and applied to the target of the Color Picker
	void OnColorPickerOk(RGBColor oldColor, RGBColor newColor){
		Debug.Log("RTColorPicker Ok clicked");
	}//OnColorPickerOk

	//Callback function when a custom color has been added or removed. This does not
	//get called if Custom Colors are disabled.
	//FileContent: This is a string containing the data that should be saved
	void OnColorPickerCustomColorsSave(string FileContent){
		/*In this example we'll be saving the content to a file which can only be done
		if we're either in the editor or a stand alone application. WebPlayer games
		can save this on a server using WWW or an external javascript call.*/
	/*	if (!Application.isWebPlayer) {
			StreamWriter SW = File.CreateText(_customColorsFilePath);
			SW.Write(FileContent);
			SW.Close();
		}//if*/
	}//OnColorPickerCustomColorsSave

	//Callback function when the RTColorPicker.LoadCustomColors function completes
	//successfully
	void OnColorPickerCustomColorsLoaded(){
		Debug.Log("RTColorPicker Custom Colors Loaded.");
	}//OnColorPickerCustomColorsLoad
}
