using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//EEF1F3 EEF1F3 6A6263 993B4C 95636C
public class Themes : MonoBehaviour {
    public static Themes Instance { get; private set; }

    [Header ("Logo")]
    public Image logo;
    public Image logoSplash;
    public Sprite wLogo;
    public Sprite dLogo;
    [Header("Pause button")]
    public Sprite sprite_pause;
    public Sprite sprite_unpause;
    [Header ("Elements")]
    public Text[] tutorPanelTexts;
    public Text[] optionPanelTexts;
    public Text[] shopPanelTexts;
    public Text[] mainMenuTexts;
    public Text[] gameoverTexts;
    public Text[] gameplayTexts;
    public Image[] imagesNeedChange;
    public Image[] imagesRevertChange;
    [Header ("White")]
    public Color wBackgroundColor;
    public Color wLabelColor;
    [Header ("Dark")]
    public Color dBackgroundColor;
    public Color dLabelColor;

    [Header ("Pixel Theme White")]
    public Color wDisable;
    public Color wActive;
    public Color[] wAttachedLevel;
    public Color wPointer;
    public Color wBombDisable;
    public Color wBombActive;
    public Color wBombPointer;
    public Color wBombAttached;
    [Header ("Pixel Theme Dark")]
    public Color dDisable;
    public Color dActive;
    public Color[] dAttachedLevel;
    public Color dPointer;
    public Color dBombDisable;
    public Color dBombActive;
    public Color dBombPointer;
    public Color dBombAttached;


    private void OnEnable () {
        Instance = this;
    }

    public void SetTheme (OptionControl.Theme theme) {
        if (theme == OptionControl.Theme.dark) {
            SetThemeDark ();
            ScreenManager.Instance.SetPixelsColor (dDisable, dActive, dAttachedLevel, dPointer, dBombActive, dBombDisable, dBombPointer, dBombAttached);
        } else {
            SetThemeWhite ();
            ScreenManager.Instance.SetPixelsColor (wDisable, wActive, wAttachedLevel, wPointer, wBombActive, wBombDisable, wBombPointer, wBombAttached);
        }
    }

    public void SetThemeWhite () {
        logoSplash.sprite = logo.sprite = wLogo;
        StopCoroutine ("ChangeImageColor");
        StopCoroutine(ChangeTextColor(dLabelColor,1));
        StopCoroutine(ChangeImageColor(dBackgroundColor,dLabelColor,1));

        ChangeCameraColor(wBackgroundColor);
        ChangeImageColor(wBackgroundColor,wLabelColor);
        ChangeTextColor(wLabelColor);
    }
    public void SetThemeDark () {
        logoSplash.sprite =  logo.sprite = dLogo;
        StopCoroutine("ChangeCameraColor");
        StopCoroutine(ChangeImageColor(wBackgroundColor,wLabelColor,1));
        StopCoroutine(ChangeTextColor(wLabelColor,1));

        ChangeCameraColor(dBackgroundColor);
        ChangeImageColor(dBackgroundColor,dLabelColor);
        ChangeTextColor(dLabelColor);
    }
    
    private void ChangeTextColor (Color color) {
        StartCoroutine(ChangeTextColor(color,1));
    }
    private void ChangeImageColor (Color color,Color revertColor) {
        StartCoroutine(ChangeImageColor(color,revertColor,1));
    }
    private void ChangeCameraColor(Color color){
        StartCoroutine(ChangeCameraColor(color,1f));
    }
    private IEnumerator ChangeCameraColor(Color color, float time){
        float counting = 0;
        float realtime = time +.2f;
        while(counting < realtime){
            if(counting >= time){
                Camera.main.backgroundColor = color;
                break;
            }
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor,color,5*Time.deltaTime);
            counting+=Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
   
    
    private IEnumerator ChangeTextColor (Text txt, Color color, float time) {
        float counting = 0;
        while (counting < time + 0.2f) {
            if (counting >= time) {
                txt.color = color;
                counting += Time.deltaTime;
                yield return null;
            }
            txt.color = Color.Lerp (txt.color, color, 5 * Time.deltaTime);
            counting += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator ChangeImageColor(Color color,Color revertColor, float time){
        float counting = 0;
        float realtime = time + .2f;
        while(counting < realtime){
            if(counting >= time){
                foreach(Image img in imagesNeedChange){
                img.color = color;
            }
            foreach(Image img in imagesRevertChange){
                img.color = revertColor;
            }
                break;
            }
            foreach(Image img in imagesNeedChange){
                img.color = Color.Lerp(img.color,color,5*Time.deltaTime);
            }
            foreach(Image img in imagesRevertChange){
                img.color = Color.Lerp(img.color,revertColor,5*Time.deltaTime);
            }
            
            counting+=Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
    private IEnumerator ChangeTextColor(Color color, float time){
        float counting = 0;
        float realtime = time + .2f;
        while(counting < realtime){
            if(counting >= time){
                SetTextsColor(color);
                break;
            }
            SetTextsColor(Color.Lerp(optionPanelTexts[0].color,color,5*Time.deltaTime));
            counting+=Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
    private void SetTextsColor(Color color){
        foreach (Text t in optionPanelTexts) {
            t.color = color;
        }
        foreach (Text t in mainMenuTexts) {
           t.color = color;
        }
        foreach (Text t in gameoverTexts) {
            t.color = color;
        }
        foreach (Text t in gameplayTexts) {
            t.color = color;
        }
        foreach (Text t in shopPanelTexts) {
            t.color = color;
        }
        foreach (Text t in tutorPanelTexts) {
            t.color = color;
        }
    }
    private IEnumerator ChangeImageColor (Image img, Color color, float time) {
        float counting = 0;
        while (counting < time + 0.2f) {
            if (counting >= time) {
                img.color = color;
                counting += Time.deltaTime;
                yield return null;
            }
            img.color = Color.Lerp (img.color, color, 5 * Time.deltaTime);
            counting += Time.deltaTime;
            yield return null;
        }
    }

}