using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ForSplash : MonoBehaviour {

	public bool isSplashMain = false;
	public Image background;
	public Color dColor;
	public Color wColor;
	public Image banner;

	public Sprite dBanner;
	public Sprite wBanner;
	private void Awake(){
		if(!isSplashMain)
			return;
		if(DateTime.Now.Hour > 18){
			PlayerPrefs.SetString (OptionControl.OPTION_STRING, "Music:1;Sound:1;ControlType:1;PixelType:1;Theme:0");
			background.color = dColor;
			banner.sprite = dBanner;
		}else{
			background.color = wColor;
			banner.sprite = wBanner;
			PlayerPrefs.SetString (OptionControl.OPTION_STRING, "Music:1;Sound:1;ControlType:1;PixelType:1;Theme:1");
		}
	}
	private void Start(){
		
		if(isSplashMain){
			StartCoroutine(StartSplash());
		}
		
	}
	public void DestroySplash(){
		Destroy(gameObject.GetComponent<ForSplash>());
		this.gameObject.SetActive(false);
	}
	private IEnumerator StartSplash(){
		yield return new WaitForSeconds(1);
		UnityEngine.SceneManagement.SceneManager.LoadScene(1);
	}
}
