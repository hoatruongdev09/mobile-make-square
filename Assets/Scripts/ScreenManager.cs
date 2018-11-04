using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour {

    //1.15
    //camera (10.9,-13)
    #region VARIABLEs
    [Header ("Colors")]
    public Color color_pixelAd;
    public Color color_pixelSlowDown;
    public Color color_pixelDiable;
    public Color color_pixelActive;
    public Color[] color_pixelAttachedLevel;
    public Color color_pixelPointer;
    public Color color_bombDisable;
    public Color color_bombActive;
    public Color color_bombPointer;
    public Color color_bombAttached;
    [Header ("Options")]
    public Vector2Int screenSize;
    private Pixel currentPixelPrefabs;
    [Header ("Effect")]
    private GameObject clearSquare;
    private GameObject newHighScore;

    public static GameObject ScreenHolder;
    public static GameObject SmallScreenHolder;

    public static bool isScreenEffect;
    public static ScreenManager Instance { get; set; }
    private Pixel[, ] screen;
    private Pixel[, ] smallScreen;

    private PointerControl gameControl;

    #endregion
    private void OnEnable () {
        Instance = this;
    }
    private void Awake () {
        gameControl = GetComponent<PointerControl> ();
        ScreenHolder = new GameObject ("PixelHolder");
        SmallScreenHolder = new GameObject ("SmallScreen");
        currentPixelPrefabs = (Pixel) Resources.Load ("Pixel", typeof (Pixel));

        clearSquare = (GameObject) Resources.Load ("ClearSquareEffect", typeof (GameObject));
        newHighScore = (GameObject) Resources.Load ("HighScoreEffect", typeof (GameObject));

        //InitSmallScreen();
        //InitScreen();
    }
     private void Update(){
        if(Input.GetKeyDown(KeyCode.P)){
            ShowScoreEffect(PointerControl.Instance.pointerPixel);
        }
    }
    #region Ultilites
    public void ResetScreen () {
        InitSmallScreen ();
        InitScreen ();
    }
    public void OnGameOver () {
        Destroy (ScreenHolder);
        Destroy (SmallScreenHolder);
        ScreenHolder = new GameObject ("PixelHolder");
        SmallScreenHolder = new GameObject ("SmallScreen");
        //SoundsControl.Instance.FadeOutEffect ();
    }
    public Vector3 GetWorldPosition (Vector2Int gridPos) {
        //Debug.Log(screen[gridPos.x, gridPos.y].transform.position);
        return screen[gridPos.x, gridPos.y].transform.position;
    }

    public List<Pixel> GetListPixel (List<Vector2Int> pos) {
        List<Pixel> list = new List<Pixel> ();
        foreach (Vector2Int v in pos) {
            list.Add (screen[v.x, v.y]);
        }
        return list;
    }
    public void SetPixelStatus(Vector2Int pos, Pixel.Status stt){
        if(pos.x >=0 && pos.y >=0 && Mathf.Abs(pos.x) < screenSize.x && Mathf.Abs(pos.y) < screenSize.y){
            screen[pos.x,pos.y].status = stt;
        }
    }
    public Pixel.Status GetPixelStatus (Vector2Int position) {
        if (position.x < 0 || position.x >= screenSize.x || position.y < 0 || position.y >= screenSize.y)
            return Pixel.Status.disable;
        return screen[position.x, position.y].status;
    }
    #endregion
    #region UPDATING SCREEN
    public void UpdateColorRange (List<Vector2Int> listV, Color color) {
        foreach (Pixel pi in GetListPixel (listV)) {
            pi.ChangeColor (color);
        }
    }
    public void UpdateColorRange (List<Pixel> listP, Color color) {
        foreach (Pixel pi in listP) {
            pi.ChangeColor (color);
        }
    }
    public void UpdateScreen (List<Vector2Int> area) {
        if (BombControl.isBombSelected) {
            BombScreenUpdate (area);
        } else {
            UpdateMainScreen (area);
        }
    }
    public void SetPixelActiveStatus(Vector2Int pos, Pixel.ActiveState activestate){
        if(pos.y<screenSize.y && pos.y>=0)
            screen[pos.x,pos.y].activestatus = activestate;
    }
    public void BombScreenUpdate (List<Vector2Int> area) {
        Pixel pi;
        foreach (Vector2Int v in area) {
            if (Mathf.Abs (v.x) < screenSize.x && Mathf.Abs (v.y) < screenSize.y && v.x >= 0 && v.y >= 0) {
                pi = screen[v.x, v.y];
                bool isBombGroup = BombControl.Instance.pixels.Contains (pi.position);
                bool isAttachedPixel = gameControl.attachedPixel.Contains (pi.position);
                bool isPointerPixel = pi.position == gameControl.pointerPixel;
                bool isPatternPixel = Pattern.Instance.pixels.Contains (pi.position);

                if (isBombGroup && isAttachedPixel) {
                    pi.SetStatus (Pixel.Status.bombAttached);
                } else if (isBombGroup && isPointerPixel) {
                    pi.SetStatus (Pixel.Status.bombAttached);
                } else if (isBombGroup && isPatternPixel) {
                    pi.SetStatus (Pixel.Status.bombAttached);
                } else if (isBombGroup) {
                    pi.SetStatus (Pixel.Status.bomb);
                } else if (isAttachedPixel) {
                    pi.SetStatus (Pixel.Status.attached);
                } else if (isPatternPixel) {
                    pi.SetStatus (Pixel.Status.active);
                } else if (isPointerPixel) {
                    pi.SetStatus (Pixel.Status.pointer);
                } else {
                    pi.SetStatus (Pixel.Status.disable);
                }
            }
        }
    }
    public void UpdateMainScreen (List<Vector2Int> area) {
        foreach (Vector2Int v in area) {
            if (Mathf.Abs (v.x) < screenSize.x && Mathf.Abs (v.y) < screenSize.y && v.x >= 0 && v.y >= 0)
                SetPixelNormalStatusByCurrent (screen[v.x, v.y]);
        }
    }
    public void UpdateScreen () {
        if (BombControl.isBombSelected) {
            BombScreenUpdate ();
        } else {
            UpdateMainScreen ();
        }
    }
    private void UpdateMainScreen () {
        foreach (Pixel pi in screen) {
            SetPixelStatusByCurrent (pi);
        }
    }
    public void UpdateSmallScreen () {
        foreach (Pixel pi in smallScreen) {
            if (Pattern.Instance.pregeneratedPixels.Contains (pi.position)) {
                if(Pattern.Instance.isNextAd){
                    pi.activestatus = Pixel.ActiveState.ad;
                }else if(Pattern.Instance.isNextSlowdown){
                    pi.activestatus = Pixel.ActiveState.slowdown;
                }else{
                    pi.activestatus = Pixel.ActiveState.normal;
                }
                pi.SetSmallScreenPixelStatus (Pixel.Status.active);
            } else {
                pi.SetSmallScreenPixelStatus (Pixel.Status.disable);
            }
        }
    }
    public void SetPixelsColor (Color disable, Color active, Color[] attachedLevel, Color pointer, Color bombActive, Color bombDisable, Color bombPointer, Color bombAttached) {
        color_pixelDiable = disable;
        color_pixelActive = active;
        color_pixelAttachedLevel = attachedLevel;
        color_pixelPointer = pointer;
        color_bombDisable = bombDisable;
        color_bombActive = bombActive;
        color_bombPointer = bombPointer;
        color_bombAttached = bombAttached;
    }
    private void BombScreenUpdate () {
        foreach (Pixel pi in screen) {
            bool isBombGroup = BombControl.Instance.pixels.Contains (pi.position);
            bool isAttachedPixel = gameControl.attachedPixel.Contains (pi.position);
            bool isPointerPixel = pi.position == gameControl.pointerPixel;
            bool isPatternPixel = Pattern.Instance.pixels.Contains (pi.position);

            if (isBombGroup && isAttachedPixel) {
                pi.SetStatus (Pixel.Status.bombAttached);
            } else if (isBombGroup && isPointerPixel) {
                pi.SetStatus (Pixel.Status.bombAttached);
            } else if (isBombGroup && isPatternPixel) {
                pi.SetStatus (Pixel.Status.bombAttached);
            } else if (isBombGroup) {
                pi.SetStatus (Pixel.Status.bomb);
            } else if (isAttachedPixel) {
                pi.SetStatus (Pixel.Status.attached);
            } else if (isPatternPixel) {
                pi.SetStatus (Pixel.Status.active);
            } else if (isPointerPixel) {
                pi.SetStatus (Pixel.Status.pointer);
            } else {
                pi.SetStatus (Pixel.Status.disable);
            }
        }
    }

    private void SetPixelStatusByCurrent (Pixel pi) {
        if (pi.position == gameControl.pointerPixel) {
            pi.SetStatus (Pixel.Status.pointer);
        } else if (gameControl.attachedPixel.Contains (pi.position)) {
            pi.SetStatus (Pixel.Status.attached);
        } else if (Pattern.Instance.pixels.Contains (pi.position)) {
            pi.SetStatus (Pixel.Status.active);
        } else {
            pi.SetStatus (Pixel.Status.disable);
        }
    }
    #endregion

    #region INITIATION
    //private void InitPixelType() {
    //    switch (OptionControl.Instance.pixelType) {
    //        case OptionControl.PixelType.cirle:
    //            currentPixelPrefabs = circlePixelPrefabs;
    //            break;
    //        case OptionControl.PixelType.roundedSquare:
    //            currentPixelPrefabs = squareRoundedPixelPrefabs;
    //            break;
    //        default:
    //            currentPixelPrefabs = squarePixelPrefabs;
    //            break;
    //    }
    //    //currentPixelPrefabs = squareRoundedPixelPrefabs;
    //}
    private void InitScreen () {
        //InitPixelType();
        screen = new Pixel[screenSize.x, screenSize.y];

        for (int y = 0; y < screenSize.y; y++) {
            for (int x = 0; x < screenSize.x; x++) {
                Pixel go = Instantiate (currentPixelPrefabs);
                go.transform.position = new Vector3 (x * 1.15f, y * 1.15f);
                go.transform.parent = ScreenHolder.transform;
                go.position = new Vector2Int (x, y);
                go.SetStatus (Pixel.Status.init); //  
                //go.name = "(" + x + ";" + y + ")";
                screen[x, y] = go;
            }
        }

        gameControl.pointerPixel = new Vector2Int (screenSize.x / 2, screenSize.x / 2);
        screen[gameControl.pointerPixel.x, gameControl.pointerPixel.y].SetStatus (Pixel.Status.pointer);
        StartGameEffect (gameControl.pointerPixel);
        //EndGameEffect(gameControl.pointerPixel);
    }
    private void InitSmallScreen () {
        //25.5
        // InitPixelType();
        smallScreen = new Pixel[3, 3];
        for (int x = 0; x < 3; x++) {
            for (int y = 0; y < 3; y++) {
                Pixel pi = Instantiate (currentPixelPrefabs);
                pi.transform.position = new Vector3 (x * 1.15f + 10.35f, y * 1.15f + 25.5f);
                pi.position = new Vector2Int (x, y);
                pi.transform.parent = SmallScreenHolder.transform;
                pi.SetStatus (Pixel.Status.disable);
                smallScreen[x, y] = pi;
            }
        }
    }
    #endregion

    #region EFFECTs
    public void ClearSquareEffect (Vector3 pos, Color start, Color end) {
        GameObject go = Instantiate (clearSquare, pos, clearSquare.transform.rotation);
        ParticleSystem ps = go.GetComponent<ParticleSystem>();
        var mainModule = ps.main;
        mainModule.startColor = new ParticleSystem.MinMaxGradient(start, end);
        Destroy (go, 3);
    }
    public void NewHighScoreEffect (Vector3 pos, Color start, Color end) {
        GameObject go = Instantiate (newHighScore, pos, newHighScore.transform.rotation);
        ParticleSystem ps = go.GetComponent<ParticleSystem>();
        var mainModule = ps.main;
        mainModule.startColor = new ParticleSystem.MinMaxGradient(start, end);
        Destroy (go, 3);
    }
    public void BombExpodeEffect (Vector2Int center) {
        // int stepTop = (screenSize.x - center.x < screenSize.y - center.y ? screenSize.x - center.x : screenSize.y - center.y);
        // int stepBot = (center.x < center.y ? center.x : center.y);
        // int step = 47 - (stepTop < stepBot ? stepTop : stepBot);
        // for (int i = 3; i <= step; i++) {
        StartCoroutine (BombExplodeEffect (center));
        //}
    }
    private IEnumerator BombExplodeEffect (Vector2Int center) {
        int stepTop = (screenSize.x - center.x < screenSize.y - center.y ? screenSize.x - center.x : screenSize.y - center.y);
        int stepBot = (center.x < center.y ? center.x : center.y);
        int step = 47 - (stepTop < stepBot ? stepTop : stepBot);
        isScreenEffect = true;
        for (int i = 3; i <= step; i++) {
            yield return new WaitForSeconds (0.025f);
            if (i == step) {
                isScreenEffect = false;
                BombControl.Instance.DeactiveBombMode ();
            }
            List<Vector2Int> pixelsEffected = GetListEffect (center, i);
            pixelsEffected.AddRange (GetListEffect (center, i - 1));
            pixelsEffected.AddRange (GetListEffect (center, i - 2));
            List<Vector2Int> pixelNormal = GetListEffect (center, i - 3);
            BombExpodeSceenUpdate (pixelsEffected, pixelNormal);
            // foreach (Pixel pi in screen) {
            //     if (pixelsEffected.Contains (pi.position)) {
            //         pi.SetStatus (Pixel.Status.bomb);
            //     }
            //     if (pixelNormal.Contains (pi.position)) {
            //         SetPixelNormalStatusByCurrent (pi);
            //     }
            // }
        }

    }
    private void BombExpodeSceenUpdate (List<Vector2Int> effect, List<Vector2Int> normal) {
        foreach (Vector2Int v in effect) {
            if (Mathf.Abs (v.x) < screenSize.x && Mathf.Abs (v.y) < screenSize.y && v.x >= 0 && v.y >= 0) {
                screen[v.x, v.y].SetStatus (Pixel.Status.bomb);
            }
        }
        foreach (Vector2Int v in normal) {
            if (Mathf.Abs (v.x) < screenSize.x && Mathf.Abs (v.y) < screenSize.y && v.x >= 0 && v.y >= 0) {
                SetPixelStatusByCurrent (screen[v.x, v.y]);
            }
        }
    }
    public void StartGameEffect (Vector2Int center) {
        StartCoroutine (FadeOutEffect (center, Pixel.Status.disable));
        // int stepTop = (screenSize.x - center.x < screenSize.y - center.y ? screenSize.x - center.x : screenSize.y - center.y);
        // int stepBot = (center.x < center.y ? center.x : center.y);
        // int step = 30 - (stepTop < stepBot ? stepTop : stepBot);
        // for (int i = 1; i <= step; i++) {
        //     StartCoroutine (FadeOutEffect (center, i, step, Pixel.Status.disable));
        // }
    }
    public void ShowScoreEffect(Vector2Int center){
        StartCoroutine(ScoreEffect(center));
    }
     private IEnumerator ScoreEffect(Vector2Int center){
        int stepTop = (screenSize.x - center.x < screenSize.y - center.y ? screenSize.x - center.x : screenSize.y - center.y);
        int stepBot = (center.x < center.y ? center.x : center.y);
        int step = 5;//47 - (stepTop < stepBot ? stepTop : stepBot);
        List<Vector2Int> pixelsEffected = new List<Vector2Int>();
        for (int i = 1; i <= step; i++) {
            yield return new WaitForSeconds (0.025f);
            pixelsEffected = GetListEffect (center, i);
            UpdateScoreEffect(pixelsEffected, new Vector3(1.1f,1.1f,1));
        }
    }
    private void UpdateScoreEffect(List<Vector2Int> area, Vector3 scale){
         foreach (Vector2Int v in area) {
            if (Mathf.Abs (v.x) < screenSize.x && Mathf.Abs (v.y) < screenSize.y && v.x >= 0 && v.y >= 0){
                screen[v.x, v.y].ChangeSizeEffect(scale);
                screen[v.x, v.y].ChangeColorEffect(new Color(0,0,0,0));
            }
        }
    }
    private IEnumerator FadeOutEffect (Vector2Int center, Pixel.Status status2Change) {
        int stepTop = (screenSize.x - center.x < screenSize.y - center.y ? screenSize.x - center.x : screenSize.y - center.y);
        int stepBot = (center.x < center.y ? center.x : center.y);
        int step = 30 - (stepTop < stepBot ? stepTop : stepBot);
        isScreenEffect = true;
        for (int i = 1; i <= step; i++) {
            yield return new WaitForSeconds (0.025f);
            List<Vector2Int> listEffect = GetListEffect (center, i);
            if (i == step) {
                isScreenEffect = false;
                // start the game
            }
            UpdateScreen (listEffect, status2Change);
        }

    }
    public void EndGameEffect (Vector2Int center) {
        StartCoroutine (FadeInEffect (center,Pixel.Status.init));
        // int stepTop = (screenSize.x - center.x < screenSize.y - center.y ? screenSize.x - center.x : screenSize.y - center.y);
        // int stepBot = (center.x < center.y ? center.x : center.y);
        // int step = 30 - (stepTop < stepBot ? stepTop : stepBot);
        // for (int i = step; i >= 0; i--)
        //    StartCoroutine (FadeInEffect (center, i, step, Pixel.Status.init));
    }
   
    private IEnumerator FadeInEffect (Vector2Int center, Pixel.Status status2Change) {
        int stepTop = (screenSize.x - center.x < screenSize.y - center.y ? screenSize.x - center.x : screenSize.y - center.y);
        int stepBot = (center.x < center.y ? center.x : center.y);
        int step = 30 - (stepTop < stepBot ? stepTop : stepBot);
        isScreenEffect = true;
        for (int i = step; i >= 0; i--) {
            List<Vector2Int> listEffect = GetListEffect (center, i);
            yield return new WaitForSeconds (0.025f);
            if (i == 0) {
                isScreenEffect = false;
                OnGameOver ();
            }
            UpdateScreen (listEffect, status2Change);
        }

    }
    private List<Vector2Int> GetListEffect_Round(Vector2Int center, int step){
        List<Vector2Int> pixels = new List<Vector2Int>();
        Vector2Int point = new Vector2Int(step-1,0);
        Vector2Int dPoint = new Vector2Int(1,1);
        int err = dPoint.x - (step << 1);
        while(point.x >= point.y){
            pixels.Add(new Vector2Int(center.x + point.x, center.y + point.y));
            pixels.Add(new Vector2Int(center.x + point.y, center.y + point.x));
            pixels.Add(new Vector2Int(center.x - point.y, center.y + point.x));
            pixels.Add(new Vector2Int(center.x - point.x, center.y + point.y));

            pixels.Add(new Vector2Int(center.x - point.x, center.y - point.y));
            pixels.Add(new Vector2Int(center.x - point.y, center.y - point.x));
            pixels.Add(new Vector2Int(center.x + point.y, center.y - point.x));
            pixels.Add(new Vector2Int(center.x + point.x, center.y - point.y));

            if(err <= 0){
                point.y++;
                err += dPoint.y;
                dPoint.y += 2;
            }else{
                point.x--;
                dPoint.x += 2;
                err += dPoint.x - (step<<1);
            }
        }
        return pixels;
    }
    private List<Vector2Int> GetListEffect (Vector2Int center, int step) {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        for (int i = 0; i <= step; i++) {
            pixels.Add (center + new Vector2Int (i, step - i));
            //Debug.Log(center + new Vector2Int(i, step - i));
            pixels.Add (center + new Vector2Int (-i, step - i));
            pixels.Add (center + new Vector2Int (-i, -(step - i)));
            pixels.Add (center + new Vector2Int (i, -(step - i)));
            //Debug.Log("Step " + step + " :" + center + new Vector2Int(i, step - i));
        }

        return pixels;
    }
    public void InitBombStateEffect (Vector2Int center) {
        // int stepTop = (screenSize.x - center.x < screenSize.y - center.y ? screenSize.x - center.x : screenSize.y - center.y);
        // int stepBot = (center.x < center.y ? center.x : center.y);
        // int step = 47 - (stepTop < stepBot ? stepTop : stepBot);
        if (BombControl.isBombSelected)
            //for (int i = step; i >= 0; i--)
            StartCoroutine (FadeInBombEffect (center));
        else
            StartCoroutine (FadeOutBombEffect (center));
    }
    private IEnumerator FadeOutBombEffect (Vector2Int center) {
        int stepTop = (screenSize.x - center.x < screenSize.y - center.y ? screenSize.x - center.x : screenSize.y - center.y);
        int stepBot = (center.x < center.y ? center.x : center.y);
        int step = 47 - (stepTop < stepBot ? stepTop : stepBot);

        isScreenEffect = true;
        for (int i = 1; i <= step; i++) {
            yield return null;
            if (i == step) {
                isScreenEffect = false;
                UpdateScreen ();
                // start the game
            }
            List<Vector2Int> listEffect = GetListEffect (center, i);
            UpdateScreen (listEffect);
            // foreach (Pixel pi in screen) {
            //     if (listEffect.Contains (pi.position))
            //         SetPixelStatusByCurrent (pi);
            // }
        }

    }
    private IEnumerator FadeInBombEffect (Vector2Int center) {
        int stepTop = (screenSize.x - center.x < screenSize.y - center.y ? screenSize.x - center.x : screenSize.y - center.y);
        int stepBot = (center.x < center.y ? center.x : center.y);
        int step = 47 - (stepTop < stepBot ? stepTop : stepBot);
        isScreenEffect = true;
        for (int i = step; i >= 0; i--) {
            yield return null;
            if (i == 0) {
                isScreenEffect = false;
                UpdateScreen ();
                // start the game
            }
            List<Vector2Int> listEffect = GetListEffect (center, i);
            UpdateScreen (listEffect);
            // foreach (Pixel pi in screen) {
            //     if (listEffect.Contains (pi.position))
            //         SetPixelStatusByCurrent (pi);
            // }
        }

    }
    private IEnumerator FadeInEffect (Vector2Int center, int step, int maxStep, Pixel.Status status2Change) {
        List<Vector2Int> listEffect = GetListEffect (center, step);
        isScreenEffect = true;
        yield return new WaitForSeconds (0.05f * (maxStep - step));
        if (step == 0) {
            isScreenEffect = false;
            OnGameOver ();
        }
        UpdateScreen (listEffect, status2Change);
    }
    private IEnumerator FadeOutEffect (Vector2Int center, int step, int maxStep, Pixel.Status status2Change) {
        List<Vector2Int> listEffect = GetListEffect (center, step);
        isScreenEffect = true;
        yield return new WaitForSeconds (0.05f * step);
        if (step == maxStep) {
            isScreenEffect = false;
            // start the game
        }
        UpdateScreen (listEffect, status2Change);
    }
    private void UpdateScreen (List<Vector2Int> area, Pixel.Status status) {
        foreach (Vector2Int v in area) {
            if (Mathf.Abs (v.x) < screenSize.x && Mathf.Abs (v.y) < screenSize.y && v.x >= 0 && v.y >= 0)
                screen[v.x, v.y].SetStatus (status);
        }
    }

    private void SetPixelNormalStatusByCurrent (Pixel pi) {
        if (pi.position == gameControl.pointerPixel) {
            pi.SetNormalStatus (Pixel.Status.pointer);
        } else if (gameControl.attachedPixel.Contains (pi.position)) {
            pi.SetNormalStatus (Pixel.Status.attached);
        } else if (Pattern.Instance.pixels.Contains (pi.position)) {
            pi.SetNormalStatus (Pixel.Status.active);
        } else {
            pi.SetNormalStatus (Pixel.Status.disable);
        }
    }
    #endregion
}