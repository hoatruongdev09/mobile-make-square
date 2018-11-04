using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour {
    public static GameControl Instance { get; private set; }
    public static bool isGameOver;
    public static bool isPause;
    public static bool isTutor;
    public static bool lockHorizonLeft, lockHorizonRight, lockRotate, stopMovingPattern, needRotate;
    public static int attachTime = 0;
    public static int moveStepCount = 0;
    public static bool isDebugLog;
    public bool log;
    public static GameMode mode;
    private PointerControl pointerControl;
    private BombControl bombControl;
    private static UIControl uiControl;
    private static SwipeControl swipeControl;
    private static DragControl dragControl;
    private static KeyBoardControl keyboardControl;
    public enum GameMode { classic, timeup }

    private void OnEnable () {
        Instance = this;
    }

    private void Awake () {
        pointerControl = GetComponent<PointerControl> ();
        bombControl = GetComponent<BombControl> ();
        dragControl = GetComponent<DragControl> ();
        swipeControl = GetComponent<SwipeControl> ();
        keyboardControl = GetComponent<KeyBoardControl> ();
        uiControl = GetComponent<UIControl> ();
        isDebugLog = log;
    }
    private void Start () {
        mode = GameMode.classic;
        isGameOver = true;
        StartCoroutine(FirstPlay(2));
        // uiControl.Button_Play();
    }
    private IEnumerator FirstPlay(float time){
        int playtime = PlayerPrefs.GetInt(ScoreManager.PLAY_TIME_KEY);
        yield return new WaitForSecondsRealtime(time);
        if(playtime == 0){
            UIControl.Instance.ShowTutorialUI();
        }
    }
    private void Update () {
        //Debug.Log (stopMovingPattern + " " + isGameOver + " " + isTutor);
        if (!isGameOver && !pointerControl.GetIsClearingSquare () && !ScreenManager.isScreenEffect && !isPause) {
            if (isTutor) {
                if (moveStepCount == 3) {
                    UIControl.Instance.UpdateTutorText ();
                    stopMovingPattern = false;
                    lockHorizonLeft = lockHorizonRight = true;
                    UIControl.Instance.ShowImageDragRight (false);
                    moveStepCount = 4;
                }
                keyboardControl.KeyboardControlForTutorial (lockHorizonLeft, lockHorizonRight, lockRotate);
                if (OptionControl.Instance.controlType == OptionControl.ControlType.drag)
                    dragControl.TouchControlForTutorial (lockHorizonLeft, lockHorizonRight, lockRotate);
                else if (OptionControl.Instance.controlType == OptionControl.ControlType.swipe) {
                    swipeControl.TouchControlForTutorial (lockHorizonLeft, lockHorizonRight, lockRotate);
                }
            } else {
                keyboardControl.KeyboardControl ();
                //singleSwipeControl.TouchControl();
                if (OptionControl.Instance.controlType == OptionControl.ControlType.drag)
                    dragControl.TouchControl ();
                else if (OptionControl.Instance.controlType == OptionControl.ControlType.swipe) {
                    swipeControl.TouchControl ();
                }
            }
        }
    }

    public static void StartTutor () {
        moveStepCount = 0;
        isTutor = true;
        isGameOver = false;
        isPause = false;
        lockHorizonLeft = lockHorizonRight = lockRotate = true;
        needRotate = stopMovingPattern = false;
        attachTime = 0;
        TutorControl.Instance.OnTutor ();
        UIControl.Instance.UpdateTutorText ();
    }
    public static void StartGame () {
        isTutor = false;
        isPause = false;
        isGameOver = false;
        stopMovingPattern = false;
    }
    public static void TutorialOver () {
        isGameOver = true;
        isTutor = false;
        stopMovingPattern = true;
        ScreenManager.Instance.EndGameEffect (new Vector2Int (21 / 2, 21 / 2));
        uiControl.EndTutor ();
        PlayerPrefs.SetInt(ScoreManager.PLAY_TIME_KEY,1);
    }
    public static void GameOver () {
        isGameOver = true;
        Debug.Log("show :" + ScoreManager.Instance.GetAdScored());
        if(ScoreManager.Instance.GetAdScored()){
            Debug.Log("Show video ad");
            ServicesControl.Instance.adServices.ShowVideoAd();
        }else{      
            Debug.Log("Show interstial ad");      
            ServicesControl.Instance.adServices.ShowInterstial ();
        }
        ScreenManager.Instance.EndGameEffect (new Vector2Int (21 / 2, 21 / 2));
        ScoreManager.Instance.SetHighScore ();
        uiControl.ShowGameoverUI ();
        DebugLog ("game over");
    }
    public static void DebugLog (string text) {
        if (isDebugLog) {
            Debug.Log ("GameControl log: " + text);
        }
    }
}