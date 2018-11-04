using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaking : MonoBehaviour {
	public static Shaking Instance {get; private set;}

	private bool isShakeMain;
	private bool isShakeSub;
	private void Start(){
		Instance = this;
	}
	public IEnumerator ShakeItUI(float duration, float magnitude, RectTransform rect){
		if(!isShakeSub){
			isShakeSub = true;
			Vector2 originalPos = rect.anchoredPosition;
			float elapsed = .0f;
			while(elapsed < duration){
				float x = Random.Range(-1f,1f) * magnitude;
				float y = Random.Range(-1f,1f) * magnitude;
				rect.anchoredPosition += new Vector2(x,y);
			//Debug.Log("Shake: + " + rect.anchoredPosition);
				elapsed += Time.deltaTime;
				yield return null;
			}
			rect.anchoredPosition = originalPos;
			isShakeSub = false;
		}
	}
	public IEnumerator ShakeIt(float duration, float magnitude, Transform trans, Vector2 dir){
		//Debug.Log("Shake !!");
		//VibrateIt();
		if(!isShakeMain){
		//	Debug.Log("Shake !!");
			isShakeMain = true;
			Vector3 originalPos = trans.position;
			float elapsed = .0f;
			while(elapsed < duration){
				float x = Random.Range(-dir.x,dir.x) * magnitude;
				float y = Random.Range(-dir.y,dir.y) * magnitude;
				trans.position += new Vector3(x,y, originalPos.z);
				elapsed += Time.deltaTime;
				yield return null;
			}
			trans.position = originalPos;
			isShakeMain = false;
		}
	}
	public void VibrateIt(){
		if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer){
			Handheld.Vibrate();
		}
		
	}
}
