using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerControl : MonoBehaviour {

    public Vector2Int pointerPixel;
    public List<Vector2Int> attachedPixel;
    public bool isClearingSquare;
    public static PointerControl Instance { get; private set; }
    public float currentDegree;
    private void Start () {
        Instance = this;
    }
    public bool GetIsClearingSquare () {
        return isClearingSquare;
    }

    #region relate to basic move
    public void MovePointer (Vector2Int direct) {
        if (CanMove (direct)) {
            pointerPixel = MovePixel (pointerPixel, direct);
            for (int i = 0; i < attachedPixel.Count; i++)
                attachedPixel[i] = MovePixel (attachedPixel[i], direct);

            ScreenManager.Instance.UpdateScreen ();

            GameControl.DebugLog ("update screen here - Move Pointer");
            //Debug.Log(PointerScreenPosition());
         }//else{
        //     StartCoroutine(Shaking.Instance.ShakeIt(0.5f,.05f,ScreenManager.ScreenHolder.transform));
        // }
    }
    public void OnGameStart () {
        if (attachedPixel.Count > 0)
            attachedPixel.Clear ();
        currentDegree = 0;
        isClearingSquare = false;
    }
    public void Rotate () {
        //  Debug.Log("Xoay "+ ScreenManager.isScreenEffect);
        if (ScreenManager.isScreenEffect){
            Shaking.Instance.VibrateIt();
            StartCoroutine(Shaking.Instance.ShakeIt(0.5f,.1f,ScreenManager.ScreenHolder.transform, Vector2.right));
            return;
        }
        if (CanRotate ()) {
            for (int i = 0; i < attachedPixel.Count; i++)
                attachedPixel[i] = Rotate90 (pointerPixel, attachedPixel[i]);
            AddDegree ();
            ScreenManager.Instance.UpdateScreen ();
            GameControl.DebugLog ("update screen here - rotate");
        }else{
            StartCoroutine(Shaking.Instance.ShakeIt(0.5f,.1f,ScreenManager.ScreenHolder.transform,Vector2.right));
            Shaking.Instance.VibrateIt();
        }
    }
    private void AddDegree () {
        currentDegree += 90;
        if (currentDegree >= 360)
            currentDegree -= 360;
    }
    private bool CanMove (Vector2Int direct) {
        bool canMove = CanMove (pointerPixel, direct);
        if (!canMove){
            StartCoroutine(Shaking.Instance.ShakeIt(0.5f,.1f,ScreenManager.ScreenHolder.transform,Vector2.right));
            Shaking.Instance.VibrateIt();
            return canMove;
        }
        foreach (Vector2Int v in attachedPixel) {
            canMove = CanMove (v, direct);
            if (!canMove){
                StartCoroutine(Shaking.Instance.ShakeIt(0.5f,.1f,ScreenManager.ScreenHolder.transform, Vector2.right));
                 Shaking.Instance.VibrateIt();
                return false;
            }
        }
        if (CheckForAttach (direct)) {
            Pattern.Instance.MakeAttach ();
            return false;
        }
        return canMove;
    }
    private bool CanRotate () {
        bool canRotate = true;
        foreach (Vector2Int v in attachedPixel) {
            canRotate = CanRotate (pointerPixel, v);
            if (!canRotate)
                break;
        }
        return canRotate;
    }

    private Vector2Int MovePixel (Vector2Int pixel, Vector2Int direct) {
        return pixel + direct;
    }

    private bool CanMove (Vector2Int pixel, Vector2Int direct) {
        Vector2Int afterMove = pixel + direct;
        return afterMove.x >= 0 && afterMove.x < ScreenManager.Instance.screenSize.x && afterMove.y >= 0 && afterMove.y < ScreenManager.Instance.screenSize.y;
    }
    private bool CanRotate (Vector2Int anchor, Vector2Int pixel) {
        Vector2Int afterMove = Rotate90 (anchor, pixel);
        bool phase1 = afterMove.x >= 0 && afterMove.x < ScreenManager.Instance.screenSize.x && afterMove.y >= 0 && afterMove.y < ScreenManager.Instance.screenSize.y;
        bool phase2 = ScreenManager.Instance.GetPixelStatus (afterMove) != Pixel.Status.active;
        return phase1 && phase2;
    }
    private Vector2Int Rotate90 (Vector2Int anchor, Vector2Int pixel) {
        Vector2Int result = Vector2Int.zero;
        Vector3Int[] mat = Rotate90Mat (new Vector3Int (anchor.x, anchor.y, 1));
        result.x = pixel.x * mat[0].x + pixel.y * mat[1].x + 1 * mat[2].x;
        result.y = pixel.x * mat[0].y + pixel.y * mat[1].y + 1 * mat[2].y;
        return result;
    }
    private Vector3Int[] Rotate90Mat (Vector3Int anchor) {
        Vector3Int[] mat = new Vector3Int[3];
        mat[0] = new Vector3Int (0, 1, 0);
        mat[1] = new Vector3Int (-1, 0, 0);
        mat[2] = new Vector3Int (anchor.y + anchor.x, -anchor.x + anchor.y, 1);
        return mat;
    }
    #endregion
    #region relate to Attach job
    public void Attach (List<Vector2Int> pixels) {
        if (CanAttach (pixels)) {
            foreach (Vector2Int v in pixels)
                attachedPixel.Add (v);
        } else {
            foreach (Vector2Int v in pixels)
                attachedPixel.Add (v);
            ScreenManager.Instance.UpdateScreen ();
            GameControl.GameOver ();
        }
        StartCoroutine (DelayToClearSquare ());

    }
    private bool CanAttach (List<Vector2Int> pixels) {
        foreach (Vector2Int v in pixels) {
            if (v.y >= ScreenManager.Instance.screenSize.y)
                return false;
        }
        return true;
    }
    private bool CheckForAttach (Vector2Int direct) {
        if (Pattern.Instance.pixels.Contains (MovePixel (pointerPixel, direct)))
            return true;
        foreach (Vector2Int v in attachedPixel)
            if (Pattern.Instance.pixels.Contains (MovePixel (v, direct)))
                return true;
        return false;
    }
    #endregion
    #region Relate to ClearSquare job
    private void ClearSquare () {
        isClearingSquare = true;
        int layer = 0;
        List<List<Vector2Int>> squares = GetSquaresByLayer (out layer);
        if (squares.Count == 2) {
           // Debug.Log ("Score 2 squares");
            GameControl.Instance.GetComponent<ServicesControl> ().playGameServices.UnlockAchievement (GPGSIds.achievement_simple);
        } else if (squares.Count == 3) {
            GameControl.Instance.GetComponent<ServicesControl> ().playGameServices.UnlockAchievement (GPGSIds.achievement_crazy);
        } else if (squares.Count == 4) {
            GameControl.Instance.GetComponent<ServicesControl> ().playGameServices.UnlockAchievement (GPGSIds.achievement_savage);
        }

        List<Vector2Int> square = new List<Vector2Int> (); // GetSquareByLayer(2);
        foreach (List<Vector2Int> sq in squares)
            square.AddRange (sq);
        int scoreTmp = 0;
        if (square.Count == 0) {
            isClearingSquare = false;
            scoreTmp = 0;
            ScoreManager.Instance.SetSlowdownScored(false);
            return;
        }
        #region add score 
        scoreTmp += square.Count * ScoreManager.Instance.GetSpeed() * squares.Count;
        ScoreManager.Instance.AddScore (scoreTmp);
        int levelPX = GetLevelAttached(layer);
        ScreenManager.Instance.ClearSquareEffect (ScreenManager.Instance.GetWorldPosition (pointerPixel),ScreenManager.Instance.color_pixelAttachedLevel[levelPX],ScreenManager.Instance.color_pixelAttachedLevel[levelPX-1]);
        ScreenManager.Instance.ShowScoreEffect(pointerPixel);
        #endregion
        float deleteTime = 0.8f / square.Count;
        SoundsControl.Instance.PlaySoundEffect (0);
        // for (int i = 0; i < square.Count; i++) {
        //     StartCoroutine (DelayToDeletePixel (i * deleteTime, square[i]));
        // }
        StartCoroutine(DelayToDeletePixel(square,deleteTime));
        StartCoroutine (DelayToReallocatePixels (0.5f, /*squares.Count */ layer, squares.Count));
        StartCoroutine (DelayToDeleteIsoLatePixels (1.6f));
    } 
    private int GetLevelAttached (int max) {
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
    private IEnumerator DelayToDeletePixel(List<Vector2Int> pixels ,float time){
        yield return new WaitForEndOfFrame();
        foreach(Vector2Int v in pixels){
            attachedPixel.Remove(v);
        }
        ScreenManager.Instance.UpdateScreen();
    }
    private IEnumerator DelayToDeletePixel (float time, Vector2Int v) {
        yield return new WaitForSeconds (time);
        attachedPixel.RemoveAt (attachedPixel.FindIndex (x => x == v));
        ScreenManager.Instance.UpdateScreen ();

        GameControl.DebugLog ("update screen here - Delay to deletePixel");
    }
    public Vector2 PointerScreenPosition () {
        Vector3 worldPos = ScreenManager.Instance.GetWorldPosition (pointerPixel);
        RectTransform canvasRect = GameObject.Find ("Canvas").GetComponent<RectTransform> ();
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint (worldPos);
        Vector2 screenPosition = new Vector2 ((canvasRect.sizeDelta.x * viewportPosition.x) - canvasRect.sizeDelta.x * 0.5f, (canvasRect.sizeDelta.y * viewportPosition.y) - canvasRect.sizeDelta.y * 0.5f);
        return screenPosition;
    }
    private void DelayToReallocatePixels (/*float time, */ List<Vector2Int> pixel, Vector2Int direct) {
        //yield return new WaitForSeconds (time);
        SoundsControl.Instance.PlaySoundEffect (3);
        ReallocateAttachedPixel (pixel, direct);
        ScreenManager.Instance.UpdateScreen ();
        GameControl.DebugLog ("update screen here - Delay to reallocate");
    }
    private IEnumerator DelayToReallocatePixels (float time, int layer, int step) {
        yield return new WaitForSeconds (time);
        List<Vector2Int>[] attPixel = GetAttachPixelByLayer (layer);
        if(attPixel[0].Count != 0){
            DelayToReallocatePixels (attPixel[0], Vector2Int.down * step);
            yield return new WaitForSeconds(.2f);
        }
        if(attPixel[1].Count !=0){
            DelayToReallocatePixels (attPixel[1], Vector2Int.left * step);
            yield return new WaitForSeconds(.2f);
        }
        if(attPixel[2].Count != 0){
            DelayToReallocatePixels (attPixel[2], Vector2Int.up * step);
            yield return new WaitForSeconds(.2f);
        }
        if(attPixel[3].Count != 0){
            DelayToReallocatePixels (attPixel[3], Vector2Int.right * step);
            yield return new WaitForSeconds(.2f);
        }
        yield return new WaitForSeconds (0.1f);
        DeleteResidualAttachedPixels ();

    }
    private void ReallocateAttachedPixel (List<Vector2Int> pixels, Vector2Int direct) {
        int count = 0;
        for (int i = 0; i < attachedPixel.Count; i++) {
            if (count == pixels.Count)
                break;
            if (pixels.Contains (attachedPixel[i])) {
                count++;
                attachedPixel[i] += direct;
            }
        }
    }
    private void DeleteResidualAttachedPixels () {
        try {
            for (int i = 0; i < attachedPixel.Count - 1; i++) {
                for (int j = i + 1; j < attachedPixel.Count; j++) {
                    if (attachedPixel[i] == attachedPixel[j])
                        attachedPixel.RemoveAt (j);
                }
            }
        } catch (Exception e) {
            GameControl.DebugLog (e.Message);
        }
    }
    private List<Vector2Int>[] GetAttachPixelByLayer (int layer) {
        List<Vector2Int>[] tmp = new List<Vector2Int>[4];
        tmp[0] = GetTopAttachPixels (layer);
        tmp[1] = GetRightAttachPixels (layer);
        tmp[2] = GetBottomAttachPixels (layer);
        tmp[3] = GetLeftAttachPixels (layer);
        return tmp;
    }
    private List<Vector2Int> GetRightAttachPixels (int layer) {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        foreach (Vector2Int tmp in attachedPixel) {
            Vector2Int v = tmp - pointerPixel;
            if (v.y >= -layer && v.y <= layer && v.x > layer)
                pixels.Add (tmp);
        }
        return pixels;
    }
    private List<Vector2Int> GetLeftAttachPixels (int layer) {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        foreach (Vector2Int tmp in attachedPixel) {
            Vector2Int v = tmp - pointerPixel;
            if (v.y >= -layer && v.y <= layer && v.x < -layer)
                pixels.Add (tmp);
        }
        return pixels;
    }
    private List<Vector2Int> GetTopAttachPixels (int layer) {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        foreach (Vector2Int tmp in attachedPixel) {
            Vector2Int v = tmp - pointerPixel;
            if (v.x >= -layer && v.x <= layer && v.y > layer)
                pixels.Add (tmp);
        }
        return pixels;
    }
    private List<Vector2Int> GetNeightbor (Vector2Int pos) {
        List<Vector2Int> list = new List<Vector2Int> ();
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (!(x == 0 && y == 0)) {
                    Vector2Int tmp = pos + new Vector2Int(x,y);
                    if (attachedPixel.Contains (tmp))
                        list.Add (tmp);
                }
            }
        }
        return list;
    }
    private List<Vector2Int> GetIsolatedGroup3(){
        List<Vector2Int> attach = new List<Vector2Int>();
        attach.AddRange(attachedPixel);
       // LogList(attach);
        List<Vector2Int> iso = IsolatedGroup();
        foreach(Vector2Int v in iso){
            attach.Remove(v);
        }
       // Debug.Log(attach.Count);
        //LogList(iso);
        //LogList(attach);
        return attach;
    }
    private void LogList(List<Vector2Int> list){
        string log = "Loglist: ";
        foreach(Vector2Int v in list){
            log+=(v+"; ");
        }
        Debug.Log(log);
    }
    private List<Vector2Int> IsolatedGroup(){
        List<Vector2Int> close = new List<Vector2Int>();
        List<Vector2Int> neighbor = new List<Vector2Int>();
        neighbor = GetNeightbor(pointerPixel);
        close.Add(pointerPixel);
        while(neighbor.Count > 0){
            if(!close.Contains(neighbor[0])){
                neighbor.AddRange(GetNeightbor(neighbor[0]));
                close.Add(neighbor[0]);
            }
            neighbor.RemoveAt(0);
        }
        close.Remove(pointerPixel);
        return close;
    }
    private List<Vector2Int> GetIsolatedGroup2 () {
        List<Vector2Int> group = GetNeightbor (pointerPixel);
        List<Vector2Int> unAttached = new List<Vector2Int> ();
        unAttached.AddRange (attachedPixel);
        foreach (Vector2Int v in group) {
            unAttached.Remove (v);
        }
        int maxLayer = FindMaxLayer ();
        if (maxLayer != -1) {
            for (int i = 1; i < maxLayer; i++) {
                foreach (Vector2Int v in GetLayerPixel (i + 1)) {
                    foreach (Vector2Int v1 in GetNeightbor (v)) {
                        if (group.Contains (v1)) {
                            group.Add (v);
                            unAttached.Remove (v);
                            break;
                        }
                    }
                }
            }
        }
        return unAttached;

    }
    private List<List<Vector2Int>> GetIsolatedGroup () {
        // Debug.Log("max level: " + FindMaxLevel());
        List<List<Vector2Int>> list = new List<List<Vector2Int>> ();
        List<Vector2Int> attched = new List<Vector2Int> ();
        attched.AddRange (attachedPixel);
        attched = ListSort (attched);
        while (attched.Count > 0) {
            List<Vector2Int> pixels = new List<Vector2Int> ();
            foreach (Vector2Int v in attched.ToArray ()) {
                if (pixels.Count == 0) {
                    pixels.Add (v);
                    attched.Remove (v);
                    continue;
                }
                for (int x = -1; x < 2; x++)
                    for (int y = -1; y < 2; y++) {
                        if (pixels.Contains (v)) {
                            break;
                        } else {
                            if (pixels.Contains (v + new Vector2Int (x, y))) {
                                pixels.Add (v);
                                attched.Remove (v);
                                break;
                            }
                        }
                    }
            }
            bool isConnect = false;
            for (int x = -1; x < 2; x++) {
                for (int y = -1; y < 2; y++) {
                    if (pixels.Contains (pointerPixel + new Vector2Int (x, y))) {
                        isConnect = true;
                        break;
                    }
                }
                if (isConnect)
                    break;
            }
            if (!isConnect)
                list.Add (pixels);
        }
        return list;
    }
    private List<Vector2Int> ListSort (List<Vector2Int> list) {
        for (int i = 0; i < list.Count - 1; i++) {
            int iMinX = FindMinX (list, i + 1);
            if (list[i].x < list[iMinX].x) {
                Swap (list, i, iMinX);
            } else if (list[i].x == list[iMinX].x) {
                if (list[i].y < list[iMinX].y)
                    Swap (list, i, iMinX);
            }
        }
        return list;
    }
    private void Swap (List<Vector2Int> l, int o1, int o2) {
        Vector2Int tmp = l[o1];
        l[o1] = l[o2];
        l[o2] = tmp;
    }

    private int FindMinX (List<Vector2Int> l, int startIndex) {
        int iMin = startIndex;
        for (int i = startIndex; i < l.Count; i++) {
            if (l[iMin].x > l[i].x) {
                iMin = i;
            }
        }
        return iMin;
    }
    private int FindMinY (List<Vector2Int> l, int startIndex) {
        int imin = startIndex;
        for (int i = startIndex; i < l.Count; i++) {
            if (l[imin].y > l[i].y) {
                imin = i;
            }
        }
        return imin;
    }
    private int FindMaxX (List<Vector2Int> l, int startIndex) {
        int iMax = startIndex;
        for (int i = startIndex + 1; i < l.Count; i++) {
            if (l[startIndex].x < l[i].x)
                iMax = i;
        }
        return iMax;
    }
    private int FindMaxY (List<Vector2Int> l, int startIndex) {
        int iMin = startIndex;
        for (int i = startIndex + 1; i < l.Count; i++) {
            if (l[startIndex].y < l[i].y)
                iMin = i;
        }
        return iMin;
    }
    private List<Vector2Int> GetBottomAttachPixels (int layer) {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        foreach (Vector2Int tmp in attachedPixel) {
            Vector2Int v = tmp - pointerPixel;
            if (v.x >= -layer && v.x <= layer && v.y < -layer)
                pixels.Add (tmp);
        }
        return pixels;
    }
    private List<List<Vector2Int>> GetSquaresByLayer (out int layer) {
        int lv = 0;
        List<List<Vector2Int>> tmp = new List<List<Vector2Int>> ();
        int maxLayer = FindMaxLayer ();
        if (maxLayer == -1){
            layer = lv;
            return tmp;
        }
        for (int i = 0; i < maxLayer; i++) {
            List<Vector2Int> listPixel = GetSquareByLayer (i + 1);
            if (listPixel != null){
                lv = i + 1;
                tmp.Add (listPixel);
            }
        }
        layer = lv;
        return tmp;
    }
    private int FindMaxLayer () {
        if (attachedPixel.Count == 0)
            return -1;
        int x, y;
        x = attachedPixel[0].x;
        y = attachedPixel[0].y;
        for (int i = 1; i < attachedPixel.Count; i++) {
            if (x < attachedPixel[i].x)
                x = attachedPixel[i].x;
            if (y < attachedPixel[i].y)
                y = attachedPixel[i].y;
        }
        return x > y ? x : y;
    }
    private List<Vector2Int> GetLayerPixel (int layer) {
        List<Vector2Int> square = new List<Vector2Int> ();
        Vector2Int pos;
        for (int x = -layer; x <= layer; x++) {
            pos = (pointerPixel + new Vector2Int (x, layer));
            if (ScreenManager.Instance.GetPixelStatus (pos) == Pixel.Status.attached)
                square.Add (pos);
        }
        for (int y = layer - 1; y >= -layer; y--) {
            pos = (pointerPixel + new Vector2Int (layer, y));
            if (ScreenManager.Instance.GetPixelStatus (pos) == Pixel.Status.attached)
                square.Add (pos);
        }
        for (int x = layer - 1; x >= -layer; x--) {
            pos = (pointerPixel + new Vector2Int (x, -layer));
            if (ScreenManager.Instance.GetPixelStatus (pos) == Pixel.Status.attached)
                square.Add (pos);
        }
        for (int y = -layer + 1; y <= layer - 1; y++) {
            pos = (pointerPixel + new Vector2Int (-layer, y));
            if (ScreenManager.Instance.GetPixelStatus (pos) == Pixel.Status.attached)
                square.Add (pos);
        }
        return square;
    }
    private List<Vector2Int> GetSquareByLayer (int layer) {
        List<Vector2Int> square = new List<Vector2Int> ();
        Vector2Int pos;
        for (int x = -layer; x <= layer; x++) {
            pos = (pointerPixel + new Vector2Int (x, layer));
            if (ScreenManager.Instance.GetPixelStatus (pos) == Pixel.Status.attached)
                square.Add (pos);
            else {
                return null;
            }
        }
        for (int y = layer - 1; y >= -layer; y--) {
            pos = (pointerPixel + new Vector2Int (layer, y));
            if (ScreenManager.Instance.GetPixelStatus (pos) == Pixel.Status.attached)
                square.Add (pos);
            else
                return null;
        }
        for (int x = layer - 1; x >= -layer; x--) {
            pos = (pointerPixel + new Vector2Int (x, -layer));
            if (ScreenManager.Instance.GetPixelStatus (pos) == Pixel.Status.attached)
                square.Add (pos);
            else
                return null;
        }
        for (int y = -layer + 1; y <= layer - 1; y++) {
            pos = (pointerPixel + new Vector2Int (-layer, y));
            if (ScreenManager.Instance.GetPixelStatus (pos) == Pixel.Status.attached)
                square.Add (pos);
            else
                return null;
        }
        return square;
    }
    private List<Vector2Int> GetSquare () {
        List<Vector2Int> square = new List<Vector2Int> ();
        Vector2Int pos;
        for (int x = -1; x <= 1; x++) {
            pos = (pointerPixel + new Vector2Int (x, 1));
            if (ScreenManager.Instance.GetPixelStatus (pos) == Pixel.Status.attached)
                square.Add (pointerPixel + new Vector2Int (x, 1));
            else {
                return null;
            }
        }
        for (int y = 0; y >= -1; y--) {
            pos = (pointerPixel + new Vector2Int (1, y));
            if (ScreenManager.Instance.GetPixelStatus (pos) == Pixel.Status.attached)
                square.Add (pointerPixel + new Vector2Int (1, y));
            else
                return null;
        }
        for (int x = 0; x >= -1; x--) {
            pos = (pointerPixel + new Vector2Int (x, -1));
            if (ScreenManager.Instance.GetPixelStatus (pos) == Pixel.Status.attached)
                square.Add (pointerPixel + new Vector2Int (x, -1));
            else
                return null;
        }
        pos = (pointerPixel + new Vector2Int (-1, 0));
        if (ScreenManager.Instance.GetPixelStatus (pos) == Pixel.Status.attached)
            square.Add (pointerPixel + new Vector2Int (-1, 0));
        else
            return null;
        return square;
    }
   
    private IEnumerator DelayToClearSquare () {
        yield return new WaitForEndOfFrame ();
        ClearSquare ();
    }
    #endregion
    #region relate to delete isolated pixel
    private IEnumerator DelayToDeleteIsoLatePixels (float time) {
        yield return new WaitForSeconds (time);
        //DeleteIsolatedPixels();
        //Debug.Log(listGroup.Count);
        //foreach (List<Vector2Int> l in listGroup) {
        //    Debug.Log("group : ");
        //    foreach (Vector2Int v in l) {
        //        Debug.Log("Delete" + v);
        //        attachedPixel.Remove(v);
        //    }
        //}
        
        List<Vector2Int> listGroup = GetIsolatedGroup3 ();
        foreach (Vector2Int v in listGroup) {
            attachedPixel.Remove (v);
            GameControl.DebugLog ("delete " + v);
        }
        ScreenManager.Instance.UpdateScreen ();
        GameControl.DebugLog ("update screen here - Delay to delete isolate");
        StartCoroutine (DelayToClearSquare ());
        //isClearingSquare = false;
        //scoreTmp = 0;
    }
    private void DeleteIsolatedPixels () {
        List<Vector2Int> blPixels = BottomLeftPixels ();
        List<Vector2Int> tlPixels = TopLeftPixels ();
        List<Vector2Int> trPixels = TopRightPixels ();
        List<Vector2Int> brPixels = BottomRightPixels ();
        //Debug.Log("pixel:");
        //foreach (Vector2Int v in blPixels)
        //    Debug.Log(v);
        //Debug.Log("edge");
        //foreach (Vector2Int v in GetBottomLeftEdge())
        //    Debug.Log(v);

        if (CheckPixelsIsolate (blPixels, GetBottomLeftEdge ()))
            DeletePixelContainsIn (blPixels);
        if (CheckPixelsIsolate (tlPixels, GetTopLeftEdge ()))
            DeletePixelContainsIn (tlPixels);
        if (CheckPixelsIsolate (trPixels, GetTopRightEdge ()))
            DeletePixelContainsIn (trPixels);
        if (CheckPixelsIsolate (brPixels, GetBottomRightEdge ()))
            DeletePixelContainsIn (brPixels);

    }
    private bool CheckPixelsIsolate (List<Vector2Int> pixels, List<Vector2Int> edge) {
        foreach (Vector2Int v in pixels) {
            if (!CheckPixelIsolate (v, edge))
                return false;
        }
        return true;
    }
    private bool CheckPixelIsolate (Vector2Int pixel, List<Vector2Int> edge) {
        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++) {
                if (edge.Contains (pixel + new Vector2Int (x, y)))
                    return false;
            }
        return true;
    }
    private void DeletePixelContainsIn (List<Vector2Int> pixels) {
        foreach (Vector2Int v in pixels) {
            attachedPixel.RemoveAt (attachedPixel.FindIndex (x => x == v));
        }
    }
    private List<Vector2Int> GetBottomLeftEdge () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        foreach (Vector2Int tmp in attachedPixel) {
            Vector2Int v = tmp - pointerPixel;
            if (v.x <= -1 && v.y == -1)
                pixels.Add (tmp);
            else if (v.y <= -1 && v.x == -1)
                pixels.Add (tmp);
        }
        return pixels;
    }
    private List<Vector2Int> GetTopLeftEdge () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        foreach (Vector2Int tmp in attachedPixel) {
            Vector2Int v = tmp - pointerPixel;
            if (v.x <= -1 && v.y == 1)
                pixels.Add (tmp);
            else if (v.y >= 1 && v.x == -1)
                pixels.Add (tmp);
        }
        return pixels;
    }
    private List<Vector2Int> GetTopRightEdge () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        foreach (Vector2Int tmp in attachedPixel) {
            Vector2Int v = tmp - pointerPixel;
            if (v.x >= 1 && v.y == 1)
                pixels.Add (tmp);
            else if (v.y >= 1 && v.x == 1)
                pixels.Add (tmp);
        }
        return pixels;
    }
    private List<Vector2Int> GetBottomRightEdge () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        foreach (Vector2Int tmp in attachedPixel) {
            Vector2Int v = tmp - pointerPixel;
            if (v.x >= 1 && v.y == -1)
                pixels.Add (tmp);
            else if (v.y <= -1 && v.x == 1)
                pixels.Add (tmp);
        }
        return pixels;
    }
    private List<Vector2Int> BottomLeftPixels () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        foreach (Vector2Int tmp in attachedPixel) {
            Vector2Int v = tmp - pointerPixel;
            if (v.x < -1 && v.y < -1)
                pixels.Add (tmp);
        }
        return pixels;
    }
    private List<Vector2Int> TopLeftPixels () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        foreach (Vector2Int tmp in attachedPixel) {
            Vector2Int v = tmp - pointerPixel;
            if (v.x < -1 && v.y > 1)
                pixels.Add (tmp);
        }
        return pixels;
    }
    private List<Vector2Int> TopRightPixels () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        foreach (Vector2Int tmp in attachedPixel) {
            Vector2Int v = tmp - pointerPixel;
            if (v.x > 1 && v.y > 1)
                pixels.Add (tmp);
        }
        return pixels;
    }
    private List<Vector2Int> BottomRightPixels () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        foreach (Vector2Int tmp in attachedPixel) {
            Vector2Int v = tmp - pointerPixel;
            if (v.x > 1 && v.y < -1)
                pixels.Add (tmp);
        }
        return pixels;
    }
    #endregion

}