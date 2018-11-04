using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using System;
using UnityEngine.SocialPlatforms;

public class PlayGameServices : MonoBehaviour {
	public static readonly string SIGN_IN_TIME = "SignInTime";
	public event EventHandler<EventArgs> OnAfterSignIn;
	public void Init(){
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder ().EnableSavedGames ().Build ();

		PlayGamesPlatform.DebugLogEnabled = true;
		PlayGamesPlatform.InitializeInstance (config);
		PlayGamesPlatform.Activate ();
		if(PlayerPrefs.GetInt(SIGN_IN_TIME) != 0)
			SignIn();
	}
	public void Rate(){
		Application.OpenURL("https://play.google.com/store/apps/details?id=com.hoatruongdev09.makesquare");
	}
	public void FeedBack(){
		Application.OpenURL("https://play.google.com/store/apps/details?id=com.hoatruongdev09.makesquare");
	}
	public void SignIn () {
		if (Social.localUser.authenticated) {
			PlayGamesPlatform.Instance.SignOut ();
			UIControl.Instance.UpdateLoginStatusText ("Login");
			PlayerPrefs.SetInt(SIGN_IN_TIME,0);
		} else {
			Social.localUser.Authenticate (OnSignInCallback);
		}
	}
	public void AchievementScore (int score) {
		if (!Social.localUser.authenticated)
			return;
		PlayGamesPlatform.Instance.IncrementAchievement (GPGSIds.achievement_newbie, score, OnIncrementAchievement);
		PlayGamesPlatform.Instance.IncrementAchievement (GPGSIds.achievement_pro, score, OnIncrementAchievement);
		PlayGamesPlatform.Instance.IncrementAchievement (GPGSIds.achievement_expert, score, OnIncrementAchievement);
	}
	public void UnlockAchievement (string ID) {
		if (!Social.localUser.authenticated)
			return;
		Social.ReportProgress (ID, 100, OnUnlockAchievement);
	}
	public void SubmitScoreLeaderboard(int score){
		if(!Social.localUser.authenticated)
			return;
		Social.ReportScore(score, GPGSIds.leaderboard_leaderboard,OnReportScoreCallback);
	}
	private void OnUnlockAchievement (bool success) {
		if (success) {
			Debug.Log ("Achivement unlocked");
		} else {
			Debug.Log ("Achievement not unlocked");
		}
	}
	private void OnIncrementAchievement (bool success) {
		if (success) {
			Debug.Log ("Increment success");
		} else {
			Debug.Log ("Increment fail");
		}
	}
	public void ShowLeaderboardUI () {
		PlayGamesPlatform.Instance.ShowLeaderboardUI ();
	}
	public void ShowAchivementUI () {
		PlayGamesPlatform.Instance.ShowAchievementsUI ();
	}
	public bool IsSignedId(){
		return Social.localUser.authenticated;
	}
	private void OnReportScoreCallback(bool success){
		if(success){
			Debug.Log("Report score success");
		}else{
			Debug.Log("Report score failed");
		}
	}
	private void OnSignInCallback (bool success) {
		if (success) {
			Debug.Log ("Sign in success: " + Social.localUser.id);
			UIControl.Instance.UpdateLoginStatusText ("Logout");
			((GooglePlayGames.PlayGamesPlatform) Social.Active).SetGravityForPopups (Gravity.BOTTOM);
			this.OnAfterSignIn += (sender, args) =>
            {
                if (this.OnAfterSignIn != null)
                {
                    this.OnAfterSignIn(this, args);
					OnAfterSignIn -= UIControl.Instance.AfterSignInShowAchievement;
					OnAfterSignIn -= UIControl.Instance.AfterSignInShowLeaderBoard;
                }
            };
			// if(OnAfterSignIn != null){
			// 	OnAfterSignIn(object,args);
			// 	OnAfterSignIn = null;
			// }
			PlayerPrefs.SetInt(SIGN_IN_TIME,1);
		} else {
			Debug.Log ("Sign in failed");
			UIControl.Instance.UpdateLoginStatusText ("Login");
		}
	}
}