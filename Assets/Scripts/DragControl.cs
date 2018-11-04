using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragControl : MonoBehaviour {

    private Vector2 fPoint, lPoint;
    public float deltaTouch = 0.2f;
    public float distanceSwiped = 0;
    private PointerControl pointerControl;
    private BombControl bombControl;
    private float limitBottom;
    private float limitTop;
    private void Awake () {
        pointerControl = GetComponent<PointerControl> ();
        bombControl = GetComponent<BombControl> ();
        deltaTouch = Screen.width / 15;
        limitBottom = Screen.height / 5.7f;
        limitTop = Screen.height / 1.4f;

    }
    private void Start () {
        if (GameControl.isDebugLog)
            Debug.Log (Screen.height);
    }

    public void TouchControlForTutorial (bool lockHorizonLeft, bool lockHorizonRight, bool lockRotate) {
        foreach (Touch touch in Input.touches) {
            if (touch.position.y >= limitBottom && touch.position.y <= limitTop) {
                if (touch.phase == TouchPhase.Began) {
                    fPoint = lPoint = touch.position;
                    distanceSwiped = 0;
                } else if (touch.phase == TouchPhase.Moved) {
                    lPoint = touch.position;
                    distanceSwiped += Vector2.Distance (lPoint, fPoint);
                    if (Vector2.Distance (lPoint, fPoint) >= deltaTouch) {
                        if (BombControl.isBombSelected)
                            bombControl.Move (GetDirectionSwipe (lPoint - fPoint));
                        else {
                            Vector2Int dragDirect = GetDirectionSwipe (lPoint - fPoint);
                            if (dragDirect.x < 0 && !lockHorizonLeft) {
                                pointerControl.MovePointer (dragDirect);
                            }
                            if (dragDirect.x > 0 && !lockHorizonRight) {
                                GameControl.moveStepCount++;
                                pointerControl.MovePointer (dragDirect);
                            }
                        }
                        fPoint = lPoint;
                    }

                }
                /*else if (touch.phase == TouchPhase.Canceled) {
                               Debug.Log("stattionary");
                               fPoint = lPoint = touch.position;
                           }*/
                else if (touch.phase == TouchPhase.Ended) {
                    if (distanceSwiped <= 8) {
                        if (!lockRotate) {
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
                }
            }
        }
    }
    public void TouchControl () {

        foreach (Touch touch in Input.touches) {
            if (touch.position.y >= limitBottom && touch.position.y <= limitTop) {
                if (touch.phase == TouchPhase.Began) {
                    fPoint = lPoint = touch.position;
                    distanceSwiped = 0;
                } else if (touch.phase == TouchPhase.Moved) {
                    lPoint = touch.position;
                    distanceSwiped += Vector2.Distance (lPoint, fPoint);
                    if (Vector2.Distance (lPoint, fPoint) >= deltaTouch) {
                        if (BombControl.isBombSelected)
                            bombControl.Move (GetDirectionSwipe (lPoint - fPoint));
                        else
                            pointerControl.MovePointer (GetDirectionSwipe (lPoint - fPoint));
                        fPoint = lPoint;
                    }

                }
                /*else if (touch.phase == TouchPhase.Canceled) {
                               Debug.Log("stattionary");
                               fPoint = lPoint = touch.position;
                           }*/
                else if (touch.phase == TouchPhase.Ended) {
                   // Debug.Log ("Lift");
                    if (distanceSwiped <= 8) {
                        if (BombControl.isBombSelected)
                            bombControl.DeletePixels ();
                        else
                            pointerControl.Rotate ();
                    }

                }
            }
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