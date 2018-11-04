using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectControl : MonoBehaviour {

	private	void Start () {
		Debug.Log("Aspect: " + Camera.main.aspect);
		if(Camera.main.aspect == .5f){
			Camera.main.orthographicSize = 26;
		}else if(Camera.main.aspect == 9f/16f){
			Camera.main.orthographicSize = 23;
		}else{
			Camera.main.orthographicSize = 23;
		}
	}


}
