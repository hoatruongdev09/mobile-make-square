using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdServices : MonoBehaviour {
    private string bannerId = "ca-app-pub-9563807851794008/5771590959";
    private string interstitialId = "ca-app-pub-9563807851794008/8988904948";
    private string videoRewardId = "ca-app-pub-9563807851794008/1488445602";

    private string appId;

    private BannerView banner;
    private InterstitialAd interstitial;
    private RewardBasedVideoAd rewardedVideo;
    private RewardBasedVideoAd videoAd;
    private int showInterstialTime;
    
    public static bool isShowAd = true;

    public void Init(){
#if UNITY_ANDROID
        string appId = "ca-app-pub-9563807851794008~4432557777";
#elif UNITY_IPHONE
        string appId = "ca-app-pub-3940256099942544~1458002511";
#else
        string appId = "unexpected_platform";
#endif
        MobileAds.Initialize (appId);
        InitAds ();
        showInterstialTime = UnityEngine.Random.Range (0, 7);
    }
    public static void DisableAd () {
        isShowAd = false;
    }
    public static void EnableAd () {
        isShowAd = true;
    }
    
    public void ShowInterstial () {
        if (!isShowAd)
            return;
        if (interstitial.IsLoaded ()) {
            if (showInterstialTime == 0) {
                showInterstialTime = UnityEngine.Random.Range (0, 7);
                interstitial.Show ();
                Debug.Log ("Intersial ad show");
            }else{
                showInterstialTime--;
            }
        } else {
            Debug.Log ("Intersial ad not show");
        }
    }
    public void ShowVideoAd(){
        if(!isShowAd){
            return;
        }
        if(videoAd.IsLoaded()){
            videoAd.Show();
        }else{
            Debug.Log("Video ad not show");
        }
    }
    public void ShowRewardedVideo () {
        if (!isShowAd)
            return;
        if (rewardedVideo.IsLoaded ()) {
                rewardedVideo.Show ();
        } else {
            Debug.Log ("Rewarded video ad not show");
        }
    }

    private void RewardForPlayer () {
        ScoreManager.Instance.AddBombAmount(3);
        ScoreManager.Instance.AddSwapTurn(3);
        UIControl.Instance.UpdateBombText(ScoreManager.Instance.GetBombAmount());
        UIControl.Instance.UpdateSwapText(ScoreManager.Instance.GetSwapTurn());
    }

    private void InitAds () {
        //LoadBanner ();
        LoadInterstial ();
        LoadRewardedVideo ();
        rewardedVideo.OnAdClosed += OnRewardedVideoClosed;
        videoAd.OnAdClosed += OnVideoAdClosed;
        interstitial.OnAdClosed += OnInterstialAdClose;
    }
    private void LoadBanner () {
        banner = new BannerView (bannerId, AdSize.SmartBanner, AdPosition.Bottom);
        AdRequest request = new AdRequest.Builder ().Build ();
        banner.LoadAd (request);
    }
    private void LoadInterstial () {
        interstitial = new InterstitialAd (interstitialId);
        AdRequest request = new AdRequest.Builder ().Build ();
        interstitial.LoadAd (request);
    }
    private void LoadRewardedVideo () {
        rewardedVideo = RewardBasedVideoAd.Instance;
        videoAd = RewardBasedVideoAd.Instance;
        AdRequest request = new AdRequest.Builder ().Build ();
        AdRequest videoReqest = new AdRequest.Builder().Build();
        rewardedVideo.LoadAd (request, videoRewardId);
        videoAd.LoadAd(request,videoRewardId);
    }
    private void OnVideoAdClosed(object sender, EventArgs args){
        AdRequest videoReqest = new AdRequest.Builder().Build();
        videoAd.LoadAd(videoReqest,videoRewardId);
    }
    private void OnRewardedVideoClosed (object sender, EventArgs args) {
        LoadRewardedVideo ();
        RewardForPlayer ();
    }
    private void OnInterstialAdClose (object sender, EventArgs args) {
        AdRequest request = new AdRequest.Builder ().Build ();
        interstitial.LoadAd (request);
    }

    private void OnDisable () {
        rewardedVideo.OnAdClosed -= OnRewardedVideoClosed;
        interstitial.OnAdClosed -= OnInterstialAdClose;
    }

}