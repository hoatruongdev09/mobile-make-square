using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardControl : MonoBehaviour {

	private BombControl bombControl;
	private PointerControl pointerControl;
	private void Start () {
		bombControl = GetComponent<BombControl> ();
		pointerControl = GetComponent<PointerControl> ();
	}

	public void KeyboardControlForTutorial (bool lockHorizonLeft, bool lockHorizonRight, bool lockRotate) {
		if (Input.GetKeyDown (KeyCode.LeftArrow) && !lockHorizonLeft) {
			if (BombControl.isBombSelected)
				bombControl.Move (new Vector2Int (-1, 0));
			else
				pointerControl.MovePointer (new Vector2Int (-1, 0));
		} else if (Input.GetKeyDown (KeyCode.RightArrow) && !lockHorizonRight) {
			GameControl.moveStepCount++;
			if (BombControl.isBombSelected)
				bombControl.Move (new Vector2Int (1, 0));
			else
				pointerControl.MovePointer (new Vector2Int (1, 0));
		}
		if (Input.GetKeyDown (KeyCode.Space) && !lockRotate) {
			if (GameControl.needRotate) {
				GameControl.needRotate = false;
				GameControl.lockRotate = true;
				GameControl.stopMovingPattern = false;
				UIControl.Instance.ShowImageTap (false);
			}
			if (BombControl.isBombSelected)
				bombControl.DeletePixels ();
			else
				pointerControl.Rotate ();
		}
	}
	public void KeyboardControl () {
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			if (BombControl.isBombSelected)
				bombControl.Move (new Vector2Int (-1, 0));
			else
				pointerControl.MovePointer (new Vector2Int (-1, 0));
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			if (BombControl.isBombSelected)
				bombControl.Move (new Vector2Int (1, 0));
			else
				pointerControl.MovePointer (new Vector2Int (1, 0));
		} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
			if (BombControl.isBombSelected)
				bombControl.Move (new Vector2Int (0, -1));
			else
				pointerControl.MovePointer (new Vector2Int (0, -1));
		} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
			if (BombControl.isBombSelected)
				bombControl.Move (new Vector2Int (0, 1));
			else
				pointerControl.MovePointer (new Vector2Int (0, 1));
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (BombControl.isBombSelected)
				bombControl.DeletePixels ();
			else
				pointerControl.Rotate ();
		}
	}
}