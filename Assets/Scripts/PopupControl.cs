using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PopupControl : MonoBehaviour {

    public Image background;
    public Text text;
    private RectTransform rect;
    private Vector2 goPos;
    private void Awake() {
        rect = GetComponent<RectTransform>();
        background.color = new Color(background.color.r, background.color.g, background.color.b, 0);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
    }
    public void SetPoppup(string text, Color backgroundColor, Vector2 position, Vector2 destiny) {
        if (GameControl.isDebugLog)
            Debug.Log("pop set");
        this.text.text = text;
        this.rect.anchoredPosition = position;
        goPos = rect.anchoredPosition + destiny;
        //this.background.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 0);
        
        if(position.y<goPos.y){
            StartCoroutine(PoppingUp());
        }else{
            StartCoroutine(PoppingDown());
        }
    }

    private void PopUp() {
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, goPos, 4 * Time.deltaTime);
        text.color = Color.Lerp(text.color, new Color(text.color.r, text.color.g, text.color.b, 1), 10 * Time.deltaTime);
        background.color = Color.Lerp(background.color, new Color(background.color.r, background.color.g, background.color.b, 1), 10 * Time.deltaTime);

    }
   
    private IEnumerator PoppingDown(){
  //      Debug.Log("Pop down");
        while(true){
            if(rect.anchoredPosition.y > goPos.y + 0.1f){
                PopUp();
            }else{
                StartCoroutine(FadeDown());
                break;
            }
            yield return null;
        }
    }
    private IEnumerator PoppingUp() {
//        Debug.Log("Pop up");
        while (true) {
            if (rect.anchoredPosition.y < goPos.y - 0.1f) {
                PopUp();
            } else {
                StartCoroutine(FadeDown());
                break;
            }
            yield return null;
        }
    }
    private IEnumerator FadeDown() {
        while (true) {
            if (background.color.a > 0.001f) {
                text.color = Color.Lerp(text.color, new Color(text.color.r, text.color.g, text.color.b, 0), 15 * Time.deltaTime);
                background.color = Color.Lerp(background.color, new Color(background.color.r, background.color.g, background.color.b, 0), 15 * Time.deltaTime);
            } else {
                Destroy(gameObject);
                break;
            }
            yield return null;
        }
    }

}
