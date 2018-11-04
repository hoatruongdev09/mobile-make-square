using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixel : MonoBehaviour {

    public Vector2Int position;
    public Status status;
    public ActiveState activestatus;
    private Color currentColor;
    private SpriteRenderer spRenderer;
    public enum Status { init, disable, active, attached, pointer, bomb, bombAttached}
    public enum ActiveState{normal, ad, slowdown}
    private void Awake () {
        spRenderer = GetComponent<SpriteRenderer> ();
    }
    
    public void SetNormalStatus (Status stt) {
        status = stt;
        switch (stt) {
            case Status.init:
                ChangeColor (new Color (0, 0, 0, 0), 10);
                break;
            case Status.disable:
                activestatus = ActiveState.normal;
                ChangeColor (ScreenManager.Instance.color_pixelDiable, 10);
                break;
            case Status.active:
                ChangeRotation(0);
                switch(activestatus){
                    default:
                        ChangeColor (ScreenManager.Instance.color_pixelActive, 20);
                        spRenderer.sprite = OptionControl.Instance.crnPixel;
                        break;
                    case ActiveState.ad:
                        ChangeColor(ScreenManager.Instance.color_pixelAd,20);
                        spRenderer.sprite = OptionControl.Instance.crnPixel;
                        break;
                    case ActiveState.slowdown:
                        ChangeColor(ScreenManager.Instance.color_pixelSlowDown);
                        spRenderer.sprite = OptionControl.Instance.crnDownSpeedPixel;
                        break;
                }
                break;
            case Status.attached:
                ChangeColor (GetAttachedColor (), 20);
                break;
            case Status.pointer:
                ChangeColor (ScreenManager.Instance.color_pixelPointer, 14);
                break;
            case Status.bomb:
                ChangeColor (ScreenManager.Instance.color_bombPointer, 14);
                break;
            default:
                break;
        }
    }
    public void SetSmallScreenPixelStatus (Status stt) {
        status = stt;
        switch (stt) {
            case Status.init:
                ChangeColor (new Color (0, 0, 0, 0), 10);
                break;
            case Status.disable:
                spRenderer.sprite = OptionControl.Instance.crnPixel;
                if (BombControl.isBombSelected) {
                    ChangeColor (ScreenManager.Instance.color_bombDisable, 10);
                } else
                    ChangeColor (ScreenManager.Instance.color_pixelDiable, 10);
                //gameObject.name = "pixel" + position;
                break;
            case Status.active:
                switch(activestatus){
                    default:
                        ChangeColor (ScreenManager.Instance.color_pixelActive, 20);
                        spRenderer.sprite = OptionControl.Instance.crnPixel;
                        break;
                    case ActiveState.ad:
                        ChangeColor(ScreenManager.Instance.color_pixelAd,20);
                        spRenderer.sprite = OptionControl.Instance.crnPixel;
                        break;
                    case ActiveState.slowdown:
                        ChangeColor(ScreenManager.Instance.color_pixelSlowDown);
                        spRenderer.sprite = OptionControl.Instance.crnDownSpeedPixel;
                        break;
                }
                if (BombControl.isBombSelected)
                    ChangeColor (ScreenManager.Instance.color_bombActive, 20);
                //gameObject.name = "pixel" + position;
                break;
            case Status.attached:
                if (BombControl.isBombSelected)
                    ChangeColor (ScreenManager.Instance.color_bombActive, 20);
                else
                    ChangeColor (GetAttachedColor (), 20);
                //gameObject.name = "pixel" + position;
                break;
            case Status.pointer:
                if (BombControl.isBombSelected)
                    ChangeColor (ScreenManager.Instance.color_bombActive, 14);
                else
                    ChangeColor (ScreenManager.Instance.color_pixelPointer, 14);
                //gameObject.name = "pixel" + position;
                break;
            case Status.bomb:
                ChangeColor (ScreenManager.Instance.color_bombPointer, 14);
                break;
            default:
                break;
        }
    }
    
    public void SetStatus (Status stt) {
        //ChangeSizeEffect();
        status = stt;
        switch (stt) {
            case Status.init:
                spRenderer.sprite = OptionControl.Instance.crnPixel;
                ChangeColor (new Color (0, 0, 0, 0), 10);
                break;
            case Status.bomb:
                spRenderer.sprite = OptionControl.Instance.crnPixel;
                ChangeColor (ScreenManager.Instance.color_bombPointer, 14);
                break;
            case Status.bombAttached:
                spRenderer.sprite = OptionControl.Instance.crnPixel;
                ChangeColor (ScreenManager.Instance.color_bombAttached, 14);
                break;
            case Status.disable:
                spRenderer.sprite = OptionControl.Instance.crnPixel;
                if (BombControl.isBombSelected) {
                    ChangeColor (ScreenManager.Instance.color_bombDisable, 10);
                } else
                    ChangeColor (ScreenManager.Instance.color_pixelDiable, 10);
                //gameObject.name = "pixel" + position;
                break;
            case Status.attached:
                spRenderer.sprite = OptionControl.Instance.crnPixel;
                if (BombControl.isBombSelected)
                    ChangeColor (ScreenManager.Instance.color_bombActive, 20);
                else
                    ChangeColor (GetAttachedColor (), 20);
                //gameObject.name = "pixel" + position;
                break;
            case Status.active:
                ChangeRotation(0);
                if (BombControl.isBombSelected){
                    ChangeColor (ScreenManager.Instance.color_bombActive, 20);
                }else{
                        switch(activestatus){
                            default:
                                ChangeColor (ScreenManager.Instance.color_pixelActive, 20);
                                spRenderer.sprite = OptionControl.Instance.crnPixel;
                                break;
                            case ActiveState.ad:
                                ChangeColor(ScreenManager.Instance.color_pixelAd,20);
                                spRenderer.sprite = OptionControl.Instance.crnPixel;
                                break;
                            case ActiveState.slowdown:
                                ChangeColor(ScreenManager.Instance.color_pixelSlowDown);
                                spRenderer.sprite = OptionControl.Instance.crnDownSpeedPixel;
                                break;
                        }
                }
                //gameObject.name = "pixel" + position;
                break;

            case Status.pointer:
                spRenderer.sprite = OptionControl.Instance.crnPointerPixel;
                ChangeRotation (PointerControl.Instance.currentDegree);
                if (BombControl.isBombSelected)
                    ChangeColor (ScreenManager.Instance.color_bombActive, 14);
                else
                    ChangeColor (ScreenManager.Instance.color_pixelPointer, 14);
                //gameObject.name = "pixel" + position;
                break;

            default:
                break;
        }
    }
    public void ChangeColor (Color color) {
        spRenderer.color = currentColor = color;
    }
    public void ChangeScale(Vector3 scale){
        transform.localScale = scale;
    }
    private Color GetAttachedColor () {
        return ScreenManager.Instance.color_pixelAttachedLevel[GetLevelAttached ()];
    }
    private int GetLevelAttached () {
        Vector2Int distance = (position - PointerControl.Instance.pointerPixel);
        int x = Mathf.Abs (distance.x);
        int y = Mathf.Abs (distance.y);
        int max = x > y ? x : y;
        if (max > 4) {
            max = max % 4;
        }
        if (max % 4 == 0)
            return 0;
        else if (max % 3 == 0)
            return 3;
        else if (max % 2 == 0)
            return 2;
        else //if (max % 1 == 0)
            return 1;
        //return 0;

    }
    public void ChangeSizeEffect(Vector3 size){
        StartCoroutine(ChangeSizeEffect(size,4));
    }
    public void ChangeColorEffect(Color color){
        StartCoroutine(ChangeColorEffect(color,4));
    }
    private IEnumerator ChangeColorEffect(Color color, float speed){
        spRenderer.color = color;
        while(true){
            if (Mathf.Abs (currentColor.r - spRenderer.color.r) <= 0.01f &&
                Mathf.Abs (currentColor.g - spRenderer.color.g) <= 0.01f &&
                Mathf.Abs (currentColor.b - spRenderer.color.b) <= 0.01f) {
                spRenderer.color = currentColor;
                break;
            }
            spRenderer.color = Color.Lerp (spRenderer.color, currentColor, speed * Time.deltaTime);
            yield return null;
        }
    }
    private IEnumerator ChangeSizeEffect(Vector3 startSize,float speed){
        transform.localScale = startSize;
        while(true){
            if(Mathf.Abs(1 - transform.localScale.x)  < 0.01f){
                transform.localScale = Vector3.one;
                break;
            }
            transform.localScale = Vector3.Lerp(transform.localScale,Vector3.one,speed * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }
    private void ChangeColor (Color color, float speed) {
        currentColor = color;
        StartCoroutine (ChangeColor (speed));
    }
    private void ChangeRotation (float angle) {
        transform.rotation = Quaternion.Euler (0, 0, angle);
    }
    private IEnumerator ChangeColor (float speed) {
        while (true) {
            if (Mathf.Abs (currentColor.r - spRenderer.color.r) <= 0.01f &&
                Mathf.Abs (currentColor.g - spRenderer.color.g) <= 0.01f &&
                Mathf.Abs (currentColor.b - spRenderer.color.b) <= 0.01f) {
                spRenderer.color = currentColor;
                break;
            }
            spRenderer.color = Color.Lerp (spRenderer.color, currentColor, speed * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }
}