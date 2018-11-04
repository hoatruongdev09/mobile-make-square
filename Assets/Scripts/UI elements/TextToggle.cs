using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TextToggle : MonoBehaviour {
    public string optionTag;
    public string[] texts;
    public Color[] textColors;

    private int currentIndex;
    private void Start() {
        SetStatus(OptionControl.Instance.GetOptionValueByTag(optionTag));
    }
    public void OnToggleClick() {
        currentIndex++;
        if (currentIndex >= texts.Length) {
            currentIndex = 0;
        }
        Text childText = GetComponentInChildren<Text>();
        childText.text = texts[currentIndex];
        OptionControl.Instance.ChangeOptionByTag(optionTag, currentIndex);
        StopCoroutine("ChangeTextColor");
        StartCoroutine(ChangeTextColor(childText, textColors[currentIndex]));
    }

    private IEnumerator ChangeTextColor(Text text, Color color) {
        float time = 0;
        while (true) {
            text.color = Color.Lerp(text.color, color, 2 * Time.deltaTime);
            if (time > 1)
                break;
            time += Time.deltaTime;
            yield return null;
        }
    }
    public void SetStatus(int index) {
        Text childText = GetComponentInChildren<Text>();
        currentIndex = index;
        childText.text = texts[currentIndex];        
        childText.color = textColors[currentIndex];
    }
}
