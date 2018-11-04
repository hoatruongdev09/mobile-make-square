using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;

public class OptionControl : MonoBehaviour {
    public static readonly string OPTION_STRING = "OptionString";
    private static readonly string DEFAULT_OPTION = "Music:1;Sound:1;ControlType:1;PixelType:1;Theme:1";
    public bool isMusic;
    public bool isSound;

    public ControlType controlType;
    public PixelType pixelType;
    public Theme theme;

    public Sprite crnPointerPixel;
    public Sprite crnDownSpeedPixel;
    public Sprite crnPixel;
    [Header ("Sprite Options")]
    public Sprite circlePointerPixel;
    public Sprite circlePixel;
    public Sprite squarePointerPixel;
    public Sprite squarePixel;
    public Sprite roundedSquarePixel;
    public Sprite roundedPixel;

    public Sprite squareDownSpeedPixel;
    public Sprite roundedSquareDownSpeedPixel;
    public Sprite circleDownSpeedPixel;
    private List<Option> listOption;

    public enum ControlType { swipe, drag };
    public enum PixelType { cirle, square, roundedSquare };
    public enum Theme { dark, light };
    public static OptionControl Instance { get; private set; }
    private void OnEnable () {
        LoadOption ();
    }
    private void Awake () {
        Instance = this;
    }
    public void LoadOption () {
        if (PlayerPrefs.HasKey (OPTION_STRING)) {
            listOption = ParseOptions (PlayerPrefs.GetString (OPTION_STRING));
        } else {
            listOption = ParseOptions (DEFAULT_OPTION);
        }
        foreach (Option op in listOption) {
            if (op.GetKey ().Equals ("Music")) {
                isMusic = op.GetBoolValue ();
                continue;
            } else if (op.GetKey ().Equals ("Sound")) {
                isSound = op.GetBoolValue ();
                continue;
            } else if (op.GetKey ().Equals ("ControlType")) {
                controlType = GetControlType (op.GetNormalValue ());
                continue;
            } else if (op.GetKey ().Equals ("PixelType")) {
                pixelType = GetPixelType (op.GetNormalValue ());
                SetPixelType ();
                continue;
            } else if (op.GetKey ().Equals ("Theme")) {
                theme = GetTheme (op.GetNormalValue ());
                if (Themes.Instance == null) {
                    GetComponent<Themes> ().SetTheme (theme);
                } else {
                    Themes.Instance.SetTheme (theme);
                }
            }

        }
    }
    public void ChangeOptionByTag (string tag, int value) {
        switch (tag) {
            case "Music":
                ChangeOption_Music (value);
                return;
            case "Sound":
                ChangeOption_Sound (value);
                return;
            case "ControlType":
                ChangeOption_ControlType (value);
                return;
            case "Theme":
                ChangeOption_Theme (value);
                return;
            case "PixelType":
                ChangeOption_PixelType (value);
                return;
            default:
                Debug.LogError ("Wrong tag");
                return;
        }
    }
    private void ChangeOption_Music (int value) {
        isMusic = value == 1;
        int opIndex = GetOptionByTag ("Music");
        if (opIndex != -1) {
            listOption[opIndex].SetValue (value);
            if (value == 0) {
                SoundsControl.Instance.StopMusic ();
            }
        }
    }
    private void ChangeOption_Sound (int value) {
        isSound = value == 1;
        int opIndex = GetOptionByTag ("Sound");
        if (opIndex != -1) {
            listOption[opIndex].SetValue (value);
        }
    }
    private void ChangeOption_Theme (int value) {
        //Debug.Log("theme " + value);
        theme = GetTheme (value);
        Themes.Instance.SetTheme (theme);
        int opIndex = GetOptionByTag ("Theme");
        if (opIndex != -1) {
            listOption[opIndex].SetValue (value);
        }
    }
    private void ChangeOption_ControlType (int value) {
        //Debug.Log("Control " + value);
        controlType = GetControlType (value);
        int opIndex = GetOptionByTag ("ControlType");
        if (opIndex != -1) {
            listOption[opIndex].SetValue (value);
        }
    }
    private void ChangeOption_PixelType (int value) {
        pixelType = GetPixelType (value);
        int opIndex = GetOptionByTag ("PixelType");
        SetPixelType ();
        if (opIndex != -1) {
            listOption[opIndex].SetValue (value);
        }
    }
    public void SaveOption () {
        string saveString = "";
        foreach (Option op in listOption) {
            saveString += op.GetKey () + ":" + op.GetNormalValue () + ";";
        }
        saveString = saveString.Trim (';');
        PlayerPrefs.SetString (OPTION_STRING, saveString);
    }
    private void SetPixelType () {
        switch (pixelType) {
            case PixelType.cirle:
                crnPixel = circlePixel;
                crnPointerPixel = circlePointerPixel;
                crnDownSpeedPixel = circleDownSpeedPixel;
                return;
            case PixelType.square:
                crnPixel = squarePixel;
                crnPointerPixel = squarePointerPixel;
                crnDownSpeedPixel = squareDownSpeedPixel;
                return;
            default:
                crnPixel = roundedPixel;
                crnPointerPixel = roundedSquarePixel;
                crnDownSpeedPixel = roundedSquareDownSpeedPixel;
                return;

        }
    }

    public Theme GetTheme (int index) {
        switch (index) {
            case 1:
                return Theme.light;
            default:
                return Theme.dark;
        }
    }

    public PixelType GetPixelType (int index) {
        switch (index) {
            case 1:
                return PixelType.square;
            case 2:
                return PixelType.roundedSquare;
            default:
                return PixelType.cirle;
        }
    }
    public ControlType GetControlType (int index) {
        switch (index) {
            case 1:
                return ControlType.drag;
            default:
                return ControlType.swipe;
        }
    }
    public List<Option> ParseOptions (string optionString) {
        List<Option> list = new List<Option> ();
        string[] arr = optionString.Split (';');
        foreach (string st in arr) {
            string[] spArr = st.Split (':');
            list.Add (new Option (spArr[0], int.Parse (spArr[1])));
        }
        return list;
    }
    public int GetOptionByTag (string _tag) {
        for (int i = 0; i < listOption.Count; i++) {
            if (listOption[i].GetKey ().Equals (_tag))
                return i;
        }
        return -1;
    }
    public int GetOptionValueByTag (string _tag) {
        foreach (Option op in listOption) {
            if (op.GetKey ().Equals (_tag)) {
                // Debug.Log(op.GetKey() + ":" + op.GetNormalValue());
                return op.GetNormalValue ();
            }
        }
        return 0;
    }

}
public class Option {
    public string Key { get; private set; }
    public int Value { get; private set; }
    public Option (string key, int value) {
        Key = key;
        Value = value;
    }
    public void SetValue (int value) {
        Value = value;
    }
    public string GetKey () {
        return Key;
    }
    public bool GetBoolValue () {
        return Value == 1;
    }
    public int GetNormalValue () {
        return Value;
    }
}