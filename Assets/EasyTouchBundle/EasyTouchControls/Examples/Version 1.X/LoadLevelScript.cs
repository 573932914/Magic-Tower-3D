﻿using UnityEngine;
using System.Collections;
#if UNITY_2019
using UnityEngine.SceneManagement;
#endif

public class LoadLevelScript : MonoBehaviour {
	
	public void LoadMainMenu(){
#if UNITY_2019
        SceneManager.LoadScene( "MainMenu");
#else
        Application.LoadLevel( "MainMenu");
		#endif
	}
	
	public void LoadJoystickEvent(){
#if UNITY_2019
        SceneManager.LoadScene( "Joystick-Event-Input");
		#else
		Application.LoadLevel( "Joystick-Event-Input");
		#endif
	}
	
	public void LoadJoysticParameter(){
#if UNITY_2019
        SceneManager.LoadScene("Joystick-Parameter");
		#else
		Application.LoadLevel("Joystick-Parameter");
		#endif
	}
	
	public void LoadDPadEvent(){
#if UNITY_2019
        SceneManager.LoadScene("DPad-Event-Input");
		#else
		Application.LoadLevel("DPad-Event-Input");
		#endif
	}
	
	public void LoadDPadClassicalTime(){
#if UNITY_2019
        SceneManager.LoadScene("DPad-Classical-Time");
		#else
		Application.LoadLevel("DPad-Classical-Time");
		#endif
	}
	
	public void LoadTouchPad(){
#if UNITY_2019
        SceneManager.LoadScene("TouchPad-Event-Input");
		#else
		Application.LoadLevel ("TouchPad-Event-Input");
		#endif
	}
	
	public void LoadButton(){
#if UNITY_2019
        SceneManager.LoadScene("Button-Event-Input");
		#else
		Application.LoadLevel("Button-Event-Input");
		#endif
	}
	
	public void LoadFPS(){
#if UNITY_2019
        SceneManager.LoadScene("FPS_Example");
		#else
		Application.LoadLevel("FPS_Example");
		#endif
	}
	
	public void LoadThird(){
#if UNITY_2019
        SceneManager.LoadScene("ThirdPerson+Jump");
		#else
		Application.LoadLevel("ThirdPerson+Jump");
		#endif
	}
	
	public void LoadThirddungeon(){
#if UNITY_2019
        SceneManager.LoadScene("ThirdPersonDungeon+Jump");
		#else
		Application.LoadLevel("ThirdPersonDungeon+Jump");
		#endif
	}
}
