using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
    public static readonly string HIGH_SCORE_KEY = "HighScoreKey";
    public static readonly string SWAP_SHAPE_KEY = "SwapShapeKey";
    public static readonly string BOMB_AMOUNT_KEY = "BombAmountKey";
    public static readonly string PLAY_TIME_KEY = "PlayTimeKey";
    private int score = 0;
    private int highScore;
    private float timePlayed = 90;
    public int speedLevel = 1;
    private int swapTurns;
    private int bombAmount;
    public int nextScoreToSpeed;
    public float nextTimeToSpeed;
    private bool breakHighscore;
    public static ScoreManager Instance { get; private set; }
    private bool isAdScored;
    private bool isSlowdownScored;
    private int tempScore = 7;
    private int tempTime = 45;
    private void OnEnable () {
        InitScore ();
    }
    private void Awake () {
        Instance = this;
    }

    private void Update () {
        //Debug.Log(timeRemain);
        if (!PointerControl.Instance.GetIsClearingSquare () && !BombControl.isBombSelected && !ScreenManager.isScreenEffect && !GameControl.isPause && !GameControl.isGameOver && !GameControl.isTutor) { //&& GameControl.mode == GameControl.GameMode.timeup) {
            timePlayed += Time.deltaTime;
            UIControl.Instance.UpdateTimeText (timePlayed);
            if(timePlayed >= nextTimeToSpeed){
                speedLevel += 1;
                Pattern.Instance.speed -= 0.1f;
                tempTime = 20 + 4 * speedLevel;
                nextTimeToSpeed = timePlayed + tempTime * speedLevel;
                StartCoroutine(Wait2ShowSpeedPopUp(2f));
            }
        } //else {
            //if (timeRemain <= 0 && GameControl.mode == GameControl.GameMode.timeup) {
            //    uIControl.UpdateTimeText(timeRemain);
            //    GameControl.GameOver();
            //}
        //}
    }
    private void InitScore () {
        if (!PlayerPrefs.HasKey (HIGH_SCORE_KEY)) {
            highScore = 0;
            PlayerPrefs.SetInt (HIGH_SCORE_KEY, highScore);
        } else {
            highScore = PlayerPrefs.GetInt (HIGH_SCORE_KEY);
        }
        if (!PlayerPrefs.HasKey (SWAP_SHAPE_KEY)) {
            swapTurns = 10;
            PlayerPrefs.SetInt (SWAP_SHAPE_KEY, swapTurns);
        } else {
            swapTurns = PlayerPrefs.GetInt (SWAP_SHAPE_KEY);
        }
        if (!PlayerPrefs.HasKey (BOMB_AMOUNT_KEY)) {
            bombAmount = 5;
            PlayerPrefs.SetInt (BOMB_AMOUNT_KEY, bombAmount);
        } else {
            bombAmount = PlayerPrefs.GetInt (BOMB_AMOUNT_KEY);
        }
        nextScoreToSpeed = 50;
        nextTimeToSpeed = 60;
    }
    public void SetSpeed () {
        if(speedLevel > 1 && isSlowdownScored){
            speedLevel-=1;
            Pattern.Instance.speed += .1f;
            tempScore = 1 + speedLevel;
            nextScoreToSpeed = score + tempScore * speedLevel * 8;
            StopCoroutine(Wait2ShowSlowDownPopUp(2f));
            StartCoroutine(Wait2ShowSlowDownPopUp(2f));
            return;
        }
        if (score >= nextScoreToSpeed && speedLevel<10) {
            speedLevel += 1;
            Pattern.Instance.speed -= 0.1f;
            tempScore = 1 + speedLevel;
            nextScoreToSpeed = score + tempScore * speedLevel * 8;
            StopCoroutine (Wait2ShowSpeedPopUp (2f));
            StartCoroutine (Wait2ShowSpeedPopUp (2f));
        }

    }
    private IEnumerator Wait2ShowSlowDownPopUp(float time){
        yield return new WaitForSeconds(time);
        SoundsControl.Instance.PlaySoundEffect(7);
        UIControl.Instance.UpdateSpeedText(speedLevel.ToString());
        UIControl.Instance.CreatePopUp ("Slow down!", Color.white, PointerControl.Instance.PointerScreenPosition () + new Vector2 (-250, 40), new Vector2(0,-50));
    }
    private IEnumerator Wait2ShowSpeedPopUp (float time) {
        yield return new WaitForSeconds (time);
        SoundsControl.Instance.PlaySoundEffect (6);
        if(speedLevel >= 10){
            UIControl.Instance.UpdateSpeedText ("Max!");
        }else{
            UIControl.Instance.UpdateSpeedText (speedLevel.ToString());
        }
        UIControl.Instance.CreatePopUp ("Speed up!", Color.white, PointerControl.Instance.PointerScreenPosition () + new Vector2 (250, -10), new Vector2(0,50));
    }
    public void SetHighScore () {
        if (highScore < score) {
            highScore = score;
            PlayerPrefs.SetInt (HIGH_SCORE_KEY, highScore);
            GameControl.Instance.GetComponent<ServicesControl>().playGameServices.SubmitScoreLeaderboard(highScore);
        }
    }
    public int GetHighScore () {
        return highScore;
    }
    public int GetScore () {
        return score;
    }
    public int GetSpeed () {
        return speedLevel;
    }
    public float GetTimeRemain () {
        return timePlayed;
    }
    public void ResetScore () {
        score = 0;
        timePlayed = 0;
        speedLevel = 1;
        tempScore = 7;
        tempTime = 45;
        nextScoreToSpeed = 50;
        nextTimeToSpeed = 60;
        breakHighscore = false;
        ResetFlag();
    }
    public void AddScore (int score) {
        this.score += score;
        BreakHighScore(this.score);
        GameControl.Instance.GetComponent<ServicesControl> ().playGameServices.AchievementScore (score);
        UIControl.Instance.UpdateScoreText (this.score);
        UIControl.Instance.CreatePopUp ("+" + score, Color.white, PointerControl.Instance.PointerScreenPosition () + new Vector2 (250, -10), new Vector2(0, 60));
        SetSpeed ();
        isAdScored = isSlowdownScored = false;
    }
    private void BreakHighScore(int score){
        if(score > highScore && highScore != 0 && !breakHighscore){
            breakHighscore = true;
            UIControl.Instance.ShakeCup();
            ScreenManager.Instance.NewHighScoreEffect(new Vector3(11.5f,36),new Color(1,0.28f,0.35f,1),new Color(0.18f,0.53f,0.87f,1));
            // do shit for this
        }
    }
    public void AddTime (float time) {
        this.timePlayed += time;
    }
    public void SetBombAmount (int amount) {
        bombAmount = amount;
        PlayerPrefs.SetInt (BOMB_AMOUNT_KEY, bombAmount);
    }
    public void AddBombAmount (int amount) {
        bombAmount += amount;
        PlayerPrefs.SetInt (BOMB_AMOUNT_KEY, bombAmount);
    }
    public void AddSwapTurn (int amount) {
        swapTurns += amount;
        PlayerPrefs.SetInt (SWAP_SHAPE_KEY, swapTurns);
    }
    public int GetBombAmount () {
        return bombAmount;
    }
    public void SetSwapTurn (int turns) {
        swapTurns = turns;
        PlayerPrefs.SetInt (SWAP_SHAPE_KEY, swapTurns);
    }
    public int GetSwapTurn () {
        return swapTurns;
    }
   

    public void SetAdScored(bool active){
        if(isAdScored)
            return;
        isAdScored = active;
    }
    public void SetSlowdownScored(bool active){
        isSlowdownScored = active;
    }
    public void ResetFlag(){
        isAdScored = false;
        isSlowdownScored = false;
    }
    public bool GetAdScored(){
        return isAdScored;
    }
    public int GetSpeedLevel(){
        return speedLevel;
    }
}