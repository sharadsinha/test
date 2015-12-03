using UnityEngine;
using System.Collections;

public class Constants {

	#region Scene Names
	
	public const string MARKERLESS_SCENE_NAME = "MarkerlessScene";
	public const string QR_SCENE_NAME = "QRScene";
	public const string MAIN_MENU_SCENE_NAME = "MainMenuScene";
	public const string OPTIONS_SCENE_NAME = "OptionsScene";
	public const string MEMENTOS_SCENE_NAME = "MementosScene";
	public const string LOADING_SCENE_NAME = "LoadingScene";
	public const string SELFIE_SCENE_NAME = "SelfieScene";
	public const string WIKI_SCENE_NAME = "WikiScene";
	public const string SPLASH_SCENE_NAME = "SplashScene";
	
	#endregion
	
	#region Global Values
	
	public const float SCENE_TRANSITION_TIME = 0.3f;
	public static Vector2 DEFAULT_CAMERA_DIMENTIONS = new Vector2(640, 480);
	
	#endregion

	#region Title Keys
	
	public const string INFO_TITLE_KEY = "INFO";
	public const string MEMENTOS_TITLE_KEY = "MEMENTOS";
	public const string SCAN_TITLE_KEY = "SCAN";
	public const string SELFIE_TITLE_KEY = "SELFIE";
	public const string SELFIE_SHARE_TITLE_KEY = "SELFIE_SHARE";
	public const string VIEW_TITLE_KEY = "VIEW";

	#endregion

	#region Localized String Keys

	public const string UNDETECTED_INSTRUCTIONS_KEY = "UNDETECTED_INSTRUCTIONS";
	public const string LOST_TRACKING_INSTRUCTIONS_KEY = "LOST_TRACKING_INSTRUCTIONS";
	public const string INVALID_QR_CODE_INSTRUCTIONS_KEY = "INVALID_QR_CODE_INSTRUCTIONS";
	public const string SELFIE_IMAGE_SHARE_SUBJECT_KEY = "SELFIE_IMAGE_SHARE_SUBJECT";
	public const string SELFIE_IMAGE_SHARE_MESSAGE_KEY = "SELFIE_IMAGE_SHARE_MESSAGE";

	#endregion
	
	#region Tutorial Keys
	
	public const string TUTORIAL_1_KEY = "TUTORIAL_1_KEY";
	public const string TUTORIAL_2_KEY = "TUTORIAL_2_KEY";
	public const string TUTORIAL_3_KEY = "TUTORIAL_3_KEY";
	public const string SELFIE_TUTORIAL_KEY = "SELFIE_TUTORIAL_KEY";
	public const string MEMENTOES_TUTORIAL_KEY = "MEMENTOES_TUTORIAL_KEY";
	public const string MARKERLESS_TUTORIAL_KEY = "MARKERLESS_TUTORIAL_KEY"; 
	
	#endregion
}
