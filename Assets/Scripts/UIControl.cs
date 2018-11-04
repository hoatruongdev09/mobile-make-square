using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour {

    private UIElement elements;
    private ScoreManager scoreManager;

    private bool isButtonPlayClicked;
    private int page = 0;
    private bool isCapture;
    public static UIControl Instance { get; private set; }
    private void Awake () {
        elements = GetComponent<UIElement> ();
        scoreManager = GetComponent<ScoreManager> ();
        Instance = this;
    }
    private void Start () {
        isButtonPlayClicked = false;
        OnInitGame ();
    }
    private void OnInitGame () {
        elements.gamePlayPanel.SetActive (false);
        elements.gameoverPanel.SetActive (false);
        elements.optionPanel.SetActive (false);
        elements.shopPanel.SetActive (false);
        elements.tutorPanel.SetActive (false);

        elements.titlePanel.SetActive (true);
        elements.titlePanel.GetComponent<AnimationControl> ().PlayClip (0);
        UpdateTitleHighScoreText (ScoreManager.Instance.GetHighScore ());
    }
    private void OnGameStart () {
        UpdateBombText (ScoreManager.Instance.GetBombAmount ());
        UpdateSwapText (ScoreManager.Instance.GetSwapTurn ());
        UpdateHighScoreText (0);
        UpdateScoreText (ScoreManager.Instance.GetScore ());
        UpdateSpeedText (ScoreManager.Instance.GetSpeed ().ToString());
        UpdateTimeText (ScoreManager.Instance.GetTimeRemain ());
        UpdateGamePlayHighScoreText (ScoreManager.Instance.GetHighScore ());
    }
    public void EndTutor () {
        StartCoroutine (WaitToEndTutor (1));
    }
    private IEnumerator WaitToEndTutor (float time) {
        yield return new WaitForSeconds (1);
        elements.tutorPanel.GetComponent<AnimationControl> ().PlayClip (2);
        //screen fade out;
        yield return new WaitForSeconds (3);
        elements.tutorPanel.GetComponent<AnimationControl> ().PlayClip (1);
        yield return new WaitForSeconds (time + .2f);
        elements.titlePanel.SetActive (true);
        elements.tutorPanel.SetActive (false);
        elements.titlePanel.GetComponent<AnimationControl> ().PlayClip (0);
        SoundsControl.Instance.FadeOutEffect ();
    }
    public void ShowGameoverUI () {
        StartCoroutine (WaitShowGameOverUI (1));
    }
    private IEnumerator WaitShowGameOverUI (float time) {
        yield return new WaitForSeconds (1);
        elements.gamePlayPanel.GetComponent<AnimationControl> ().PlayClip (1);
        elements.text_GameoverScore.text = ScoreManager.Instance.GetScore ().ToString ();
        // GameControl.Instance.GetComponent<ServicesControl> ().playGameServices.AchievementScore (scoreManager.GetScore ());
        elements.text_GameoverHighScore.text = ScoreManager.Instance.GetHighScore ().ToString ();
        yield return new WaitForSeconds (time + .2f);
        elements.gameoverPanel.SetActive (true);
        elements.gameoverPanel.GetComponent<AnimationControl> ().PlayClip (0);
        SoundsControl.Instance.FadeOutEffect ();
    }
    public void CreatePopUp (string text, Color backgroudColor, Vector2 position, Vector2 destiny) {
        PopupControl popup = Instantiate (elements.popupPrefab, GameObject.Find ("Canvas").transform);
        //Debug.Log(popup.name);
        popup.SetPoppup (text, backgroudColor, position, destiny);
    }
    public void UpdateTimeText (float time) {
        elements.text_Time.text = string.Format ("<color=#ffa502>Time:</color> {0}", (int) time);
        //if (time > 10)
        //    elements.text_Time.text = string.Format("<color=#ffa502>Time:</color> <color=#2F3542>{0}</color>", (int)time);
        //else
        //    elements.text_Time.text = string.Format("<color=#ffa502>Time:</color> <color=#ff6348>{0}</color>", (int)time);
    }
    public void ShowImageDragLeft (bool active) {
        elements.image_DragLeft.gameObject.SetActive (active);
        elements.image_Fader.gameObject.SetActive (active);
        elements.text_Instruction.gameObject.SetActive (active);
        elements.text_Instruction.text = "Drag to the left on screen.";
    }
    public void ShowImageDragRight (bool active) {
        elements.image_DragRight.gameObject.SetActive (active);
        elements.image_Fader.gameObject.SetActive (active);
        elements.text_Instruction.gameObject.SetActive (active);
        elements.text_Instruction.text = "Drag to the right on screen.";
    }
    public void ShowImageTap (bool active) {
        elements.image_Tap.gameObject.SetActive (active);
        elements.image_Fader.gameObject.SetActive (active);
        elements.text_Instruction.gameObject.SetActive (active);
        elements.text_Instruction.text = "Tap anywhere on screen.";
    }
    public void UpdateTutorText () {
        elements.text_Tutor.text = TutorControl.Instance.GetNextText ();
    }
    public void DisableButtonPurchaseAd () {
        elements.button_purchaseAd.GetComponentInChildren<Text> ().text = "Purchased";
    }
    public void UpdateSwapText (int swap) {
        elements.text_SwapAmount.text = swap.ToString ();
    }
    public void UpdateScoreText (int score) {
        elements.text_Score.text = score.ToString ();
    }
    public void UpdateHighScoreText (int hiScore) {
        elements.text_Highscore.text = hiScore.ToString ();
    }
    public void UpdateTitleHighScoreText (int hiScore) {
        elements.text_TitleHighScore.text = hiScore.ToString ();
    }
    public void UpdateSpeedText (string speed) {
        elements.text_Speed.text = "<color=#ffa502>Speed:</color> " + speed;
    }
    public void UpdateBombText (int bomb) {
        elements.text_BombAmount.text = bomb.ToString ();
    }
    public void UpdateLoginStatusText (string text) {
        elements.text_loginStatus.text = text;
    }
    public void UpdateGamePlayHighScoreText (int score) {
        elements.text_Highscore.text = score.ToString ();
    }
    public void Button_Swap () {
        if(GameControl.isPause)
            return;
        ButtonJobs ();
        if (ScoreManager.Instance.GetSwapTurn () > 0) {
            Pattern.Instance.NextPattern ();
            ScoreManager.Instance.SetSwapTurn (ScoreManager.Instance.GetSwapTurn () - 1);
            UpdateSwapText (ScoreManager.Instance.GetSwapTurn ());
        }else{
            ShowImageForShop();
        }
    }
    public void Button_YesQuit(){
        Application.Quit();
    }
    public void Button_NoQuit(){
        elements.image_Confirm.gameObject.SetActive(false);
    }
    public void Button_Quit(){
        elements.image_Confirm.gameObject.SetActive(true);
    }
    public void Button_SignIn () {
        GetComponent<ServicesControl> ().playGameServices.SignIn ();
    }
    public void Button_ShowAchievement () {
        if(GetComponent<ServicesControl>().playGameServices.IsSignedId())
            GetComponent<ServicesControl> ().playGameServices.ShowAchivementUI ();
        else{
            ShowWarning("Achievements");
            GetComponent<ServicesControl>().playGameServices.OnAfterSignIn += AfterSignInShowAchievement;
        }
    }
    public void Button_ShowLeaderboard () {
        if(GetComponent<ServicesControl>().playGameServices.IsSignedId())
            GetComponent<ServicesControl> ().playGameServices.ShowLeaderboardUI ();
        else{
            ShowWarning("Leaderboard");
            GetComponent<ServicesControl>().playGameServices.OnAfterSignIn += AfterSignInShowLeaderBoard;
        }
    }
    public void AfterSignInShowAchievement(object sender, EventArgs args){
        GetComponent<ServicesControl> ().playGameServices.ShowAchivementUI();
    }
    public void AfterSignInShowLeaderBoard(object sender, EventArgs args){
        GetComponent<ServicesControl> ().playGameServices.ShowLeaderboardUI();
    }
    public void Button_ShowAd(){
        GameControl.Instance.GetComponent<ServicesControl>().adServices.ShowInterstial();
    }
    public void Button_Shop () {
        ButtonJobs ();
        elements.shopPanel.SetActive (true);
        elements.shopPanel.GetComponent<AnimationControl> ().PlayClip (0);
    }
    public void Button_Option () {
        ButtonJobs ();
        elements.optionPanel.SetActive (true);
        elements.optionPanel.GetComponent<AnimationControl> ().PlayClip (0);
    }
    public void Button_Back () {
        SoundsControl.Instance.PlayButtonSound (1);
        StartCoroutine (FallBack (0.5f));
    }
    public void Button_Next () {
        SoundsControl.Instance.PlayButtonSound (0);
        //StopCoroutine (GoNext (.5f));
        StartCoroutine (GoNext (.5f));
    }
    public void Button_Home () {
        ButtonJobs ();
        StartCoroutine (WaitShowTitleUI (1));
    }
    public void Button_Share(){
        StartCoroutine(CaptureImageAndShare());
    }
    public void Button_Play () {
        SoundsControl.Instance.PlayButtonSound (2);
        ShowGamePlayUI ();
    }
    public void Button_Tutorial () {
        SoundsControl.Instance.PlayButtonSound (2);
        ShowTutorialUI ();
    }
    public void Button_Buy (string id) {
        ButtonJobs ();
        GetComponent<ServicesControl> ().purchaseServices.BuyProductId (id);
    }
    public void Button_Login () {
        GameControl.Instance.GetComponent<ServicesControl> ().playGameServices.SignIn ();
    }
    public void Button_RateUs(){
         GameControl.Instance.GetComponent<ServicesControl> ().playGameServices.Rate ();
    }
    public void Button_TitleNothank(){
        elements.image_Warning.gameObject.SetActive(false);
        GetComponent<ServicesControl>().playGameServices.OnAfterSignIn -= AfterSignInShowAchievement;
        GetComponent<ServicesControl>().playGameServices.OnAfterSignIn -= AfterSignInShowLeaderBoard;
    }
    public void Button_Pause(){
        GameControl.isPause = !GameControl.isPause;

        if(GameControl.isPause){
            elements.image_Hider.gameObject.SetActive(true);
            elements.image_Pause.sprite = Themes.Instance.sprite_unpause;
            SoundsControl.Instance.FadeOutEffect();
        }else{
            elements.image_Hider.gameObject.SetActive(false);
            elements.image_Pause.sprite = Themes.Instance.sprite_pause;
            SoundsControl.Instance.FadeInEffect();
        }
    }
    public void Button_Unpause(){
        GameControl.isPause = false;
        elements.image_Hider.gameObject.SetActive(false);
        elements.image_Pause.sprite = Themes.Instance.sprite_pause;
        SoundsControl.Instance.FadeInEffect();
    }
    public void Button_Feedback(){
         GameControl.Instance.GetComponent<ServicesControl> ().playGameServices.FeedBack ();
    }
    public void Button_NoThanks(){
        elements.image_Forshop.gameObject.SetActive(false);
    }
    public void Button_WatchAd(){
        GameControl.Instance.GetComponent<ServicesControl>().adServices.ShowRewardedVideo();
    }
    private void ButtonJobs () {
        SoundsControl.Instance.PlayButtonSound (0);
    }

    public void Button_Bomb () {
        if(GameControl.isPause)
            return;
        if (!ScreenManager.isScreenEffect) {
//            Debug.Log("Bomb amount: " + scoreManager.GetBombAmount());
            if (ScoreManager.Instance.GetBombAmount () > 0) {
                if (!BombControl.isBombSelected) {
                    SoundsControl.Instance.PlayButtonSound (0);
                } else {
                    SoundsControl.Instance.PlayButtonSound (1);
                }
                BombControl.isBombSelected = !BombControl.isBombSelected;
                BombControl.Instance.InitPixels ();
                ScreenManager.Instance.InitBombStateEffect (PointerControl.Instance.pointerPixel);
            } else {
                BombControl.isBombSelected = false;
                ShowImageForShop();
            }
            
            //ScreenManager.Instance.UpdateScreen();
            GameControl.DebugLog ("update screen here - button bomb");
        }
    }
    private void ShowWarning(string text){
        elements.text_Warning.text = "Sign in to show " + text + " ?";
        elements.image_Warning.gameObject.SetActive(true);
    }
    public void ShakeCup(){
        StartCoroutine(Shaking.Instance.ShakeItUI(2,100f,elements.image_Cup.rectTransform));
    }
    private void ShowImageForShop(){
        elements.image_Forshop.gameObject.SetActive(true);
        Button_Pause();
    }
    public void PageValueChange (Vector2 value) {
        if (value.x < 0.5f) {
            elements.text_nextButton.text = "Next";
            page = 0;
            return;
        }
        if (value.x > 0.5f) {
            elements.text_nextButton.text = "Previous";
            page = 1;
            return;
        }
    }
    private IEnumerator GoNext (float time) {
        float couting = 0;
        if (page == 0) {
            page = 1;
        } else {
            page = 0;
        }
        int nextpos = page * (-1080);
        while (couting < time + 2) {
            if (couting >= time) {
                elements.go_ViewContent.anchoredPosition = new Vector2 (nextpos, 0);
                //elements.text_nextButton.text = (page == 1) ? "Previous" : "Next";
                break;
            }
            elements.go_ViewContent.anchoredPosition = Vector2.Lerp (elements.go_ViewContent.anchoredPosition, new Vector2 (nextpos, 0), 8 * Time.smoothDeltaTime);
            couting += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator FallBack (float time) {
        if (elements.optionPanel.activeSelf) {
            elements.optionPanel.GetComponent<AnimationControl> ().PlayClip (1);
            OptionControl.Instance.SaveOption ();
        }
        if (elements.shopPanel.activeSelf) {
            elements.shopPanel.GetComponent<AnimationControl> ().PlayClip (1);
        }
        yield return new WaitForSeconds (time);
        elements.optionPanel.SetActive (false);
        elements.shopPanel.SetActive (false);

        //elements.titlePanel.SetActive (true);
    }
    private IEnumerator WaitShowTitleUI (float time) {
        elements.gameoverPanel.GetComponent<AnimationControl> ().PlayClip (2);
        yield return new WaitForSeconds (time);
        elements.gameoverPanel.SetActive (false);
        elements.tutorPanel.SetActive (false);
        elements.titlePanel.SetActive (true);
        elements.titlePanel.GetComponent<AnimationControl> ().PlayClip (0);
        UpdateTitleHighScoreText (ScoreManager.Instance.GetHighScore ());
    }
    private void ShowGamePlayUI () {
        StartCoroutine (WaitShowGameplayUI (1));
    }
    public void ShowTutorialUI () {
        StartCoroutine (WaitShowTutorialUI (1));
    }
    private IEnumerator WaitShowTutorialUI (float time) {
        if (!isButtonPlayClicked) {
            isButtonPlayClicked = true;
            if (elements.gameoverPanel.activeSelf) {
                elements.gameoverPanel.GetComponent<AnimationControl> ().PlayClip (1);
            }
            if (elements.titlePanel.activeSelf) {
                // play animation;
                elements.titlePanel.GetComponent<AnimationControl> ().PlayClip (1);
            }
            yield return new WaitForSeconds (time + 0.5f);
            elements.gameoverPanel.SetActive (false);
            elements.titlePanel.SetActive (false);
            elements.gamePlayPanel.SetActive (false);
            elements.tutorPanel.SetActive (true);
            elements.tutorPanel.GetComponent<AnimationControl> ().PlayClip (0);
            ScreenManager.Instance.ResetScreen ();
            GameControl.StartTutor ();
            PointerControl.Instance.OnGameStart ();
            Pattern.Instance.OnTutorialStart ();
            SoundsControl.Instance.FadeInEffect ();
            //scoreManager.ResetScore ();
            //OnGameStart ();
            //GameControl.StartGame ();
            isButtonPlayClicked = false;
        }
    }
    private IEnumerator WaitShowGameplayUI (float time) {
        if (!isButtonPlayClicked) {
            isButtonPlayClicked = true;
            if (elements.gameoverPanel.activeSelf) {
                elements.gameoverPanel.GetComponent<AnimationControl> ().PlayClip (1);
            }
            if (elements.titlePanel.activeSelf) {
                // play animation;
                elements.titlePanel.GetComponent<AnimationControl> ().PlayClip (1);
            }
            yield return new WaitForSeconds (time + 0.5f);
            elements.gameoverPanel.SetActive (false);
            elements.titlePanel.SetActive (false);
            elements.gamePlayPanel.SetActive (true);
            elements.gamePlayPanel.GetComponent<AnimationControl> ().PlayClip (0);
            //yield return new WaitForSeconds(1);
            ScreenManager.Instance.ResetScreen ();
            PointerControl.Instance.OnGameStart ();
            Pattern.Instance.OnGameStart ();
            ScoreManager.Instance.ResetScore ();
            OnGameStart ();
            GameControl.StartGame ();
            SoundsControl.Instance.FadeInEffect ();
            isButtonPlayClicked = false;
        }
    }

    private  IEnumerator CaptureImageAndShare() {
        //Texture2D screenshotTexture = null;
        string fileName = "screenshot.png";
        
        yield return new WaitForEndOfFrame();
        if (!isCapture) {
            isCapture = true;
            ScreenCapture.CaptureScreenshot(fileName);
            //screenshotTexture = ScreenCapture.CaptureScreenshotAsTexture();
        }
        yield return new WaitForEndOfFrame();
        //new NativeShare().AddFile(Application.persistentDataPath + "/" + fileName).SetSubject("#MakeSquares #Share").SetTitle("Can you beat my scores ?").SetText("#NowPlaying #MakeSquares #MyHighestScore #"+scoreManager.GetHighScore()).Share();
        StartCoroutine(ShareScreenshot(Application.persistentDataPath + "/" + fileName));
        isCapture = false;
    }
    private IEnumerator ShareScreenshot(string destination) {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSecondsRealtime(0.3f);

        if (!Application.isEditor) {
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "Make Square - Origin");
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TITLE"), "Come and beat my score");    
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "#MyScore #MakeSquare #NowPlaying");
            intentObject.Call<AndroidJavaObject>("setType", "text/plain");
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"),uriObject);
            intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser",
                intentObject, "Choose platform");
            currentActivity.Call("startActivity", chooser);

            yield return new WaitForSecondsRealtime(1);
        }
        Debug.Log("done");
    }
}