using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ImageToggle : MonoBehaviour {

    public string optionTag;
    public Sprite[] images;
    public bool changeColor;
    public Color[] colors;

    private int currentIndex;
    private void Start() {
        SetStatus(OptionControl.Instance.GetOptionValueByTag(optionTag));
    }
    public void OnToggleClick() {
        currentIndex++;
        if (currentIndex >= images.Length) {
            currentIndex = 0;
        }
        Image image = GetComponent<Image>();
        image.sprite = images[currentIndex];
        OptionControl.Instance.ChangeOptionByTag(optionTag, currentIndex);
        if (changeColor) {
            StopCoroutine("ChangeImageColor");
            StartCoroutine(ChangeImageColor(image, colors[currentIndex]));
        }
    }
    private IEnumerator ChangeImageColor(Image image, Color color) {
        float time = 0;
        while (true) {
            image.color = Color.Lerp(image.color, color, 20 * Time.deltaTime);
            if (time > 0.5f) {
                image.color = color;
                break;
            }
            time += Time.deltaTime;
            yield return null;
        }
    }
    public void SetStatus(int index) {
        Image image = GetComponent<Image>();
        currentIndex = index;
        image.sprite = images[currentIndex];
        if (changeColor)
            image.color = colors[currentIndex];
    }
}
