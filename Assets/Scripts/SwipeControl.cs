using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeControl : MonoBehaviour {

    private Vector2 fp, lp;
    public float dragDisX, dragDisY;

    private PointerControl pointerControl;
    private BombControl bombControl;
    private void Awake () {
        pointerControl = GetComponent<PointerControl> ();
        bombControl = GetComponent<BombControl> ();
        dragDisX = Screen.width / 70;
        dragDisY = dragDisX;

    }
    // private void Start () {
    //     if (GameControl.isDebugLog)
    //         Debug.Log ("DragDisX : " + dragDisX + " DragDisY" + dragDisY);
    // }

    public void TouchControlForTutorial (bool lockHorizonLeft, bool lockHorizonRight, bool lockRotate) {
        foreach (Touch touch in Input.touches) {
            // if (touch.position.y < 400 && touch.position.y > 50) {
            if (touch.phase == TouchPhase.Began) {
                fp = lp = touch.position;
                //Debug.Log(fp);
            }
            if (touch.phase == TouchPhase.Ended) {
                lp = touch.position;

                float xDis = Mathf.Abs (lp.x - fp.x);
                float yDis = Mathf.Abs (lp.y - fp.y);
                if (xDis > dragDisX || yDis > dragDisY) {
                    Vector2 dir = lp - fp;
                    if (BombControl.isBombSelected)
                        bombControl.Move (GetDirectionSwipe (dir));
                    else {
                        Vector2Int swipeDirect = GetDirectionSwipe (dir);
                        if (swipeDirect.x < 0 && !lockHorizonLeft) {
                            pointerControl.MovePointer (swipeDirect);
                        }
                        if (swipeDirect.x > 0 && !lockHorizonRight) {
                            GameControl.moveStepCount++;
                            pointerControl.MovePointer (swipeDirect);
                        }
                    }
                    //pointerControl.MovePointer (GetDirectionSwipe (dir));
                } else {
                    if (!lockRotate) {
                        if (GameControl.needRotate) {
                            GameControl.needRotate = false;
                            GameControl.lockRotate = true;
                            GameControl.stopMovingPattern = false;
                            UIControl.Instance.ShowImageTap (false);
                        }
                        if (BombControl.isBombSelected)
                            bombControl.DeletePixels ();
                        else {
                            pointerControl.Rotate ();
                        }
                    }
                }
            }
            //}
        }
    }
    public void TouchControl () {
        foreach (Touch touch in Input.touches) {
            // if (touch.position.y < 400 && touch.position.y > 50) {
            if (touch.phase == TouchPhase.Began) {
                fp = lp = touch.position;
                //Debug.Log(fp);
            }
            if (touch.phase == TouchPhase.Ended) {
                lp = touch.position;

                float xDis = Mathf.Abs (lp.x - fp.x);
                float yDis = Mathf.Abs (lp.y - fp.y);
                if (xDis > dragDisX || yDis > dragDisY) {
                    Vector2 dir = lp - fp;
                    if (BombControl.isBombSelected)
                        bombControl.Move (GetDirectionSwipe (dir));
                    else
                        pointerControl.MovePointer (GetDirectionSwipe (dir));
                } else {
                    if (BombControl.isBombSelected)
                        bombControl.DeletePixels ();
                    else
                        pointerControl.Rotate ();
                }
            }
            //}
        }
    }
    private Vector2Int GetDirectionSwipe (Vector2 v) {
        float absX = Mathf.Abs (v.x);
        float absY = Mathf.Abs (v.y);
        if (absX >= absY) {
            if (v.x < 0) {
                return new Vector2Int (-1, 0);
            } else {
                return new Vector2Int (1, 0);
            }
        } else {
            if (v.y < 0) {
                return new Vector2Int (0, -1);
            } else {
                return new Vector2Int (0, 1);
            }

        }
    }
}