        using System.Collections.Generic;
        using System.Collections;
        using UnityEngine;

        public class Pattern : MonoBehaviour {

            public float speed;
            public bool isAd;
            public bool isSlowdown;
            public float slowdownChance = 10;
            public float adChance = 5;
            public bool isNextAd;
            public bool isNextSlowdown;
            public List<Vector2Int> pixels;
            public List<Vector2Int> pregeneratedPixels;
            private float timeCounter;
            private PatternGenerator patternGenerator;
            public static Pattern Instance { get; private set; }
            private float originSpeed;
            public bool isGenerating;
            public int moveStepCount;
            private void Awake () {

                patternGenerator = new PatternGenerator ();
                originSpeed = speed;
                //pixels = patternGenerator.GetPattern();
                //pregeneratedPixels = patternGenerator.GetRawPattern();
            }
            private void Start () {
                Instance = this;
                //ScreenManager.Instance.UpdateSmallScreen();
            }
            private void Update () {

                //GameControl.DebugLog ("Is gameover: " + GameControl.isGameOver + "; IsClearing square: " + PointerControl.Instance.GetIsClearingSquare () + "\nIs Bomb selected: " + BombControl.isBombSelected + "; Is ScreenEffect: " + ScreenManager.isScreenEffect);
                if (!GameControl.isGameOver && !PointerControl.Instance.GetIsClearingSquare () && !BombControl.isBombSelected && !ScreenManager.isScreenEffect && !GameControl.stopMovingPattern && !GameControl.isPause) {

                    Updating ();

                }
            }
            private void GeneratingEffect () {
                StartCoroutine (GeneratingEffect (2));
            }
            private IEnumerator GeneratingEffect (float time) {
                List<Pixel> listP = ScreenManager.Instance.GetListPixel (pixels);
                float timeCounter = 0;
                Color crntColor = ScreenManager.Instance.color_bombActive;
                while (true) {
                    timeCounter += Time.deltaTime;
                    crntColor = Color.Lerp (crntColor, ScreenManager.Instance.color_pixelActive, Time.deltaTime);
                    ScreenManager.Instance.UpdateColorRange (listP, crntColor);
                    if (timeCounter >= time) {
                        ScreenManager.Instance.UpdateScreen ();
                        //isGenerating = false;
                        break;
                    }
                    yield return null;
                }
            }
            private void Updating () {
                if (timeCounter < speed) {
                    timeCounter += Time.deltaTime;
                } else {
                    timeCounter = 0;
                    if (GameControl.isTutor && !GameControl.isGameOver) {
                        GameControl.DebugLog ("Turorial move");
                        TutorialMove ();
                    }
                    if (!GameControl.isGameOver && !GameControl.isTutor) {
                        GameControl.DebugLog ("Normal move");
                        Move ();
                    }

                }
            }
            private void TutorialMove () {
                if (moveStepCount == 2 && GameControl.attachTime == 0) {
                    UIControl.Instance.UpdateTutorText ();
                }
                if (moveStepCount == 7 && GameControl.attachTime == 0) {
                    UIControl.Instance.UpdateTutorText ();
                }

                if (moveStepCount == 12 && GameControl.attachTime == 1) {
                    UIControl.Instance.UpdateTutorText ();
                    GameControl.stopMovingPattern = true;
                    GameControl.lockHorizonLeft = false;
                    UIControl.Instance.ShowImageDragLeft (true);
                    moveStepCount = 100;
                }
                if (moveStepCount == 8 && GameControl.attachTime == 2) {
                    UIControl.Instance.UpdateTutorText ();
                    moveStepCount = 100;
                    GameControl.stopMovingPattern = true;
                    GameControl.lockRotate = false;
                    GameControl.needRotate = true;
                    UIControl.Instance.ShowImageTap (true);
                }
                if (pixels.Count == 0) {
                    GeneratePattern ();
                }
                if (CanMove () && !GameControl.stopMovingPattern) {
                    for (int i = 0; i < pixels.Count; i++)
                        pixels[i] = Move (pixels[i], Vector2Int.down);
                    ScreenManager.Instance.UpdateScreen ();
                    moveStepCount++;
                    GameControl.DebugLog ("update screen here - tutorial move");
                }
            }
            private void Move () {
                if (pixels.Count == 0) {
                    GeneratePattern ();
                }
                if (CanMove ()) {
                    for (int i = 0; i < pixels.Count; i++){
                        pixels[i] = Move (pixels[i], Vector2Int.down);
                        if(isAd){
                            ScreenManager.Instance.SetPixelActiveStatus(pixels[i],Pixel.ActiveState.ad);
                        }else if(isSlowdown){
                            ScreenManager.Instance.SetPixelActiveStatus(pixels[i],Pixel.ActiveState.slowdown);
                        }else{
                            ScreenManager.Instance.SetPixelActiveStatus(pixels[i],Pixel.ActiveState.normal);
                        }
                    }
                    ScreenManager.Instance.UpdateScreen ();
                    GameControl.DebugLog ("update screen here - normal move");
                } //} else {
                //    GameControl.GameOver();
                //}

            }

            public void OnTutorialStart () {
                patternGenerator = new PatternGenerator ();
                pixels = patternGenerator.SetPosition (patternGenerator.GetFirtPattern ());
                pregeneratedPixels = patternGenerator.GetSecondPattern ();
                ScreenManager.Instance.UpdateSmallScreen ();
            }
            public void TutorialNextPattern () {
                moveStepCount = 0;
                pregeneratedPixels = patternGenerator.GetThirdPattern ();
                ScreenManager.Instance.UpdateSmallScreen ();
                ScreenManager.Instance.UpdateScreen ();
            }
            public void TutorialGeneratePattern () {
                pixels = patternGenerator.SetPosition (pregeneratedPixels);
                TutorialNextPattern ();
            }

            public void OnGameStart () {
                ResetSpeed();
                patternGenerator = new PatternGenerator ();
                pixels = patternGenerator.GetPattern ();
                NextPattern ();
                //pregeneratedPixels = patternGenerator.GetRawPattern ();
                //isGenerating = true;
                ScreenManager.Instance.UpdateSmallScreen ();
                //GeneratingEffect();
            }
            public void NextPattern () {
                pregeneratedPixels = patternGenerator.GetRawPattern ();
                ScreenManager.Instance.UpdateSmallScreen ();
                StartCoroutine(Shaking.Instance.ShakeIt(0.25f,0.1f,ScreenManager.SmallScreenHolder.transform,Vector2.right));
                GameControl.DebugLog ("update screen here - next pattern");
            }
            public void GeneratePattern () {
                pixels = null;
                pixels = patternGenerator.SetPosition (pregeneratedPixels);
                if(isNextAd){
                    isAd = true;
                    isNextAd = false;
                    isSlowdown = false;
                }else if(isNextSlowdown){
                    isSlowdown = true;
                    isAd = false;
                    isNextSlowdown = false;
                }else{
                    isAd = isSlowdown = isNextAd = isNextSlowdown = false;
                }
                NextPattern ();
                if(Random.Range(0f,100f) <  adChance){
                    isNextAd = true;
                }
                if(Random.Range(0f,100f) < slowdownChance && ScoreManager.Instance.GetSpeedLevel() > 1){
                    isNextSlowdown = true;
                }
                
                if(isNextAd && isNextSlowdown){
                    isNextSlowdown = false;
                }
                ScreenManager.Instance.UpdateSmallScreen ();
            }
            public void MakeAttach () {
                if (!GameControl.isGameOver && !GameControl.isTutor)
                    AttachToPointer ();
                else
                    TutorialAttachToPointer ();
            }
          
            private void TutorialAttachToPointer () {
                GameControl.DebugLog ("Attach ");
                PointerControl.Instance.Attach (pixels);
                SoundsControl.Instance.PlaySoundEffect (2);
                ScreenManager.Instance.UpdateScreen ();
                TutorialGeneratePattern ();
                if (GameControl.attachTime == 0) {
                    UIControl.Instance.UpdateTutorText ();
                    GameControl.lockHorizonRight = false;
                    GameControl.stopMovingPattern = true;
                    UIControl.Instance.ShowImageDragRight (true);
                }
                if (GameControl.attachTime == 1) {
                    UIControl.Instance.UpdateTutorText ();
                    GameControl.lockHorizonLeft = GameControl.lockHorizonRight = true;
                    GameControl.lockRotate = true;
                    GameControl.stopMovingPattern = false;
                    UIControl.Instance.ShowImageDragLeft (false);
                }
                if (GameControl.attachTime == 2) {
                    UIControl.Instance.UpdateTutorText ();
                    //Debug.Log ("End tutor here");
                    //GameControl.Instance.stopMovingPattern = true;
                    StartCoroutine (DelayToEndTutor (2));
                    //GameControl.TutorialOver ();
                }
                GameControl.attachTime += 1;
                GameControl.DebugLog ("update screen here - attach to pointer tutorial");

            }
            private IEnumerator DelayToEndTutor (float time) {
                yield return new WaitForSeconds (time);
                GameControl.TutorialOver ();
            }
            private void AttachToPointer () {
                PointerControl.Instance.Attach (pixels);
                ScoreManager.Instance.SetAdScored(isAd);
                ScoreManager.Instance.SetSlowdownScored(isSlowdown);
                GameControl.DebugLog ("update screen here - attach to pointer normal");
                SoundsControl.Instance.PlaySoundEffect (2);
                ScreenManager.Instance.UpdateScreen ();

                GeneratePattern ();
                //pixels = patternGenerator.SetPosition(pregeneratedPixels);
                //pregeneratedPixels = patternGenerator.GetRawPattern();
                //ScreenManager.Instance.UpdateSmallScreen();
            }

            private Vector2Int Move (Vector2Int pixel, Vector2Int direct) {
                return pixel + direct;
            }
            private bool CanMove () {
                foreach (Vector2Int v in pixels) {
                    if(isAd || isSlowdown){
                        if((v+new Vector2Int(0,-1)).y < -3){
                            GeneratePattern();
                            return false;
                        }
                    }
                    if (CanAttach ()) {
//                        Debug.Log ("can attach");
                        if (!GameControl.isTutor)
                            AttachToPointer ();
                        else
                            TutorialAttachToPointer ();
                        return false;
                    }
                    if (!CanMove (v, new Vector2Int (0, -1)) && !isAd && !isSlowdown) {
                        GameControl.GameOver ();
                        return false;
                    }
                }
                return true;
            }
            private bool CanAttach () {
                foreach (Vector2Int v in pixels) {
                    if (Move (v, Vector2Int.down) == PointerControl.Instance.pointerPixel)
                        return true;
                    else if (PointerControl.Instance.attachedPixel.Contains (Move (v, Vector2Int.down)))
                        return true;
                }
                return false;
            }
            private bool CanMove (Vector2Int pixel, Vector2Int direct) {
                Vector2Int afterMove = pixel + direct;
                return (afterMove.y >= 0);
            }
            public void ResetSpeed () {
                speed = originSpeed;
            }
        }