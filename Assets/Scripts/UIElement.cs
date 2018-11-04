using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIElement : MonoBehaviour {
    [Header ("Panel")]
    public GameObject titlePanel;
    public GameObject gamePlayPanel;
    public GameObject gameoverPanel;
    public GameObject optionPanel;
    public GameObject shopPanel;
    public GameObject tutorPanel;
    [Header ("Game play")]
    public Text text_Score;
    public Text text_Highscore;
    public Text text_BombAmount;
    public Text text_SwapAmount;
    public Text text_Time;
    public Text text_Speed;
    public Image image_Cup;
    public Image image_Bomb;
    public Image image_Swap;
    public Image image_Hider;
    public Image image_Forshop;
    public Image image_Pause;
    [Header ("Gameover panel")]
    public Text text_GameoverScore;
    public Text text_GameoverHighScore;
    [Header ("Title panel")]
    public Text text_TitleHighScore;
    public Text text_Warning;
    public Image image_Warning;
    public Image image_Confirm;
    [Header ("Prefabs")]
    public PopupControl popupPrefab;
    [Header ("Shop Panel")]
    public GameObject button_purchaseAd;
    [Header ("Tutorial Panel")]
    public Text text_Tutor;
    public Text text_Instruction;
    public Image image_Tap;
    public Image image_DragLeft;
    public Image image_DragRight;
    public Image image_Fader;
    [Header ("Option panel")]
    public RectTransform go_ViewContent;
    public Text text_loginStatus;
    public Text text_nextButton;
}