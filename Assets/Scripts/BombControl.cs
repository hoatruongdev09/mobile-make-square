using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombControl : MonoBehaviour {

    public static bool isBombSelected;
    public List<Vector2Int> pixels;
    public Vector2Int centerPixel;
    public static BombControl Instance { get; private set; }
    private PointerControl pointer;
    private Pattern pattern;

    private void Awake() {
        Instance = this;
        pointer = GetComponent<PointerControl>();
        pattern = GetComponent<Pattern>();
    }

    public void InitPixels() {
        pixels = new List<Vector2Int>();
        for (int x = -1; x < 2; x++) {
            for (int y = -1; y < 2; y++) {
                if (x == 0 && y == 0)
                    centerPixel = (pointer.pointerPixel + new Vector2Int(x, y));
                pixels.Add(pointer.pointerPixel + new Vector2Int(x, y));
            }
        }
    }
    public void DeletePixels() {
        if (ScreenManager.isScreenEffect)
            return;
        foreach (Vector2Int v in pixels) {
            pointer.attachedPixel.Remove(pointer.attachedPixel.Find(x => x == v));
            pattern.pixels.Remove(pattern.pixels.Find(x => x == v));
        }
        ScoreManager.Instance.SetBombAmount(ScoreManager.Instance.GetBombAmount() - 1);
        FindObjectOfType<UIControl>().UpdateBombText(ScoreManager.Instance.GetBombAmount());
        isBombSelected = false;
        SoundsControl.Instance.PlaySoundEffect(5);
        ScreenManager.Instance.BombExpodeEffect(centerPixel);
        StartCoroutine(Shaking.Instance.ShakeIt(0.2f,.05f,ScreenManager.ScreenHolder.transform, Vector2.one));
        Shaking.Instance.VibrateIt();
        //ScreenManager.Instance.UpdateScreen();
    }
    public void DeactiveBombMode() {
        isBombSelected = false;
        ScreenManager.Instance.UpdateScreen();
    }
    public void Move(Vector2Int dir) {
        if (CanMove(dir)) {
            centerPixel = Move(centerPixel, dir);
            for (int i = 0; i < pixels.Count; i++)
                pixels[i] = Move(pixels[i], dir);
            ScreenManager.Instance.UpdateScreen();
            GameControl.DebugLog("update screen here - Move bomb");
        }else{
            StartCoroutine(Shaking.Instance.ShakeIt(0.5f,.1f,ScreenManager.ScreenHolder.transform, Vector2.right));
            Shaking.Instance.VibrateIt();
        }
    }
    private Vector2Int Move(Vector2Int pixel, Vector2Int direct) {
        return pixel + direct;
    }
    private bool CanMove(Vector2Int dir) {
        foreach (Vector2Int v in pixels) {
            if (!CanMove(v, dir))
                return false;
        }
        return true;
    }

    private bool CanMove(Vector2Int pixel, Vector2Int direct) {
        Vector2Int afterMove = pixel + direct;
        return (afterMove.y >= 0) && (afterMove.y < ScreenManager.Instance.screenSize.y) && (afterMove.x >= 0) && (afterMove.x < ScreenManager.Instance.screenSize.x);
    }
}
