using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternGenerator {
    private void LogList (List<Vector2Int> list) {
        string log = "";
        foreach (Vector2Int v in list)
            log += v + ";";
        Debug.Log (log);
    }

    public List<Vector2Int> GetPattern () {
        List<Vector2Int> tmp = new List<Vector2Int> ();
        tmp = GenerateForm ();
        return SetPosition (tmp);
    }

    public List<Vector2Int> GetRawPattern () {
        return GenerateForm ();
    }
    public List<Vector2Int> SetPosition (List<Vector2Int> pixels) {

        int distanceTop = 2 - CalculateTop (pixels);
        for (int i = 0; i < pixels.Count; i++) {
            pixels[i] += new Vector2Int (9, 21 + (distanceTop));
        }
        return pixels;
    }
    #region  For tutorial
    public List<Vector2Int> GetFirtPattern () {
        List<Vector2Int> tmp = new List<Vector2Int> ();
        tmp.Add(new Vector2Int(1,0));
        tmp.Add(new Vector2Int(2,0));
        return tmp;
    }
    public List<Vector2Int> GetSecondPattern () {
        List<Vector2Int> tmp = new List<Vector2Int> ();
        tmp = Move (Rotate90 (ThreeTiles1 ()), Vector2Int.left);
        return tmp;
    }
    public List<Vector2Int> GetThirdPattern () {
        List<Vector2Int> tmp = new List<Vector2Int> ();
        tmp = Move (ThreeTiles2 (), Vector2Int.right);
        return tmp;
    }
    #endregion
    private int CalculateTop (List<Vector2Int> pixels) {
        int yMax = 0;
        foreach (Vector2Int v in pixels) {
            if (v.y > yMax) {
                yMax = v.y;
            }
        }
        return yMax;
    }

    private List<Vector2Int> GenerateForm () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        int tileNum = Random.Range (2, 6);
        switch (tileNum) {
            case 2:
                tileNum = Random.Range (0, 4);
                pixels = Get2CellShape(tileNum);
                break;
            case 3:
                tileNum = Random.Range (0, 6);
                pixels = Get3CellShape(tileNum);
                break;
            case 4:
                tileNum = Random.Range (0, 15);
                pixels = Get4CellShape(tileNum);
                break;
            case 5:
                tileNum = Random.Range (0, 12);
                pixels = Get5CellShape(tileNum);
                break;
            default:
                Debug.Log (tileNum);
                break;
        }
        return RandomRotate (pixels);
    }
    private List<Vector2Int> Get2CellShape (int index) {
        switch (index) {
            case 1:
                return TwoTile1 ();
            case 2:
                return TwoTile2 ();
            case 3:
                return TwoTile3 ();
            default:
                return TwoTile ();
        }
    }
    private List<Vector2Int> Get3CellShape (int index) {
        switch (index) {
            case 1:
                return ThreeTiles1 ();
            case 2:
                return ThreeTiles2 ();
            case 3:
                return ThreeTiles3();
            case 4: 
                return ThreeTiles4();
            case 5:
                return ThreeTiles5();
            default:
                return ThreeTiles();
        }
    }
    private List<Vector2Int> Get4CellShape(int index){
        switch(index){
            case 1: 
                return FourTiles2();
            case 2: 
                return FourTiles3();
            case 3: 
                return FourTiles4();
            case 4: 
                return FourTiles5();
            case 6: 
                return FourTiles6();
            case 7: 
                return FourTiles7();
            case 8: 
                return FourTiles8();
            case 9: 
                return FourTiles9();
            case 10: 
                return FourTiles10();
            case 11: 
                return FourTiles11();
            case 12: 
                return FourTiles12();
            case 13: 
                return FourTiles13();
            case 14: 
                return FourTiles14();
            default:
                return FourTiles1();
            
        }
    }
    private List<Vector2Int> Get5CellShape(int index){
        switch(index){
            case 1:
                return FiveTile2();
            case 2:
                return FiveTile3();
            case 3:
                return FiveTile4();
            case 4:
                return FiveTile5();
            case 5:
                return FiveTile6();
            case 6:
                return FiveTile7();
            case 7:
                return FiveTile8();
            case 9:
                return FiveTile9();
            case 10:
                return FiveTile10();
            case 11:
                return FiveTile11();
            default:
                return FiveTile1();
             
        }
    }
    private Vector2Int MinPixel (List<Vector2Int> pixels) {
        Vector2Int min = new Vector2Int ();
        min = pixels[0];
        for (int i = 1; i < pixels.Count; i++) {
            if (min.x > pixels[i].x)
                min.x = pixels[i].x;
            if (min.y > pixels[i].y)
                min.y = pixels[i].y;
        }
        return min;
    }

    private List<Vector2Int> RandomRotate (List<Vector2Int> pixels) {
        List<Vector2Int> tmp = new List<Vector2Int> ();
        int rotateTime = Random.Range (0, 4);
        for (int i = 0; i <= rotateTime; i++)
            tmp = Rotate90 (pixels);
        return tmp;
    }
    private List<Vector2Int> FiveTile1 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 0));
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (1, 2));
        pixels.Add (new Vector2Int (2, 2));
        return pixels;
    }
    private List<Vector2Int> FiveTile2 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 0));
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (1, 2));
        pixels.Add (new Vector2Int (2, 1));
        return pixels;
    }
    private List<Vector2Int> FiveTile3 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (1, 0));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (1, 2));
        pixels.Add (new Vector2Int (2, 1));
        return pixels;
    }
    private List<Vector2Int> FiveTile4 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (1, 2));
        pixels.Add (new Vector2Int (2, 1));
        pixels.Add (new Vector2Int (2, 0));
        return pixels;
    }
    private List<Vector2Int> FiveTile5 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (0, 2));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (2, 1));
        pixels.Add (new Vector2Int (2, 0));
        return pixels;
    }
    private List<Vector2Int> FiveTile6 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 0));
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (2, 1));
        pixels.Add (new Vector2Int (2, 2));
        return pixels;
    }
    private List<Vector2Int> FiveTile7 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (2, 0));
        pixels.Add (new Vector2Int (2, 1));
        pixels.Add (new Vector2Int (2, 2));
        return pixels;
    }
    private List<Vector2Int> FiveTile8 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 0));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (2, 0));
        pixels.Add (new Vector2Int (2, 1));
        pixels.Add (new Vector2Int (2, 2));
        return pixels;
    }
    private List<Vector2Int> FiveTile9 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 0));
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (2, 0));
        pixels.Add (new Vector2Int (2, 1));
        return pixels;
    }
    private List<Vector2Int> FiveTile10 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 0));
        pixels.Add (new Vector2Int (0, 2));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (2, 0));
        pixels.Add (new Vector2Int (2, 2));
        return pixels;
    }
    private List<Vector2Int> FiveTile11 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 0));
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (0, 2));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (2, 0));
        return pixels;
    }

    private List<Vector2Int> FourTiles1 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 0));
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (1, 0));
        pixels.Add (new Vector2Int (1, 1));
        return pixels;
    }
    private List<Vector2Int> FourTiles2 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (1, 0));
        pixels.Add (new Vector2Int (2, 1));
        return pixels;
    }
    private List<Vector2Int> FourTiles3 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (2, 1));
        pixels.Add (new Vector2Int (2, 0));
        return pixels;
    }
    private List<Vector2Int> FourTiles4 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (2, 1));
        pixels.Add (new Vector2Int (0, 0));
        return pixels;
    }
    private List<Vector2Int> FourTiles5 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 0));
        pixels.Add (new Vector2Int (1, 0));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (2, 1));
        return pixels;
    }
    private List<Vector2Int> FourTiles6 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (1, 0));
        pixels.Add (new Vector2Int (2, 0));
        return pixels;
    }
    private List<Vector2Int> FourTiles7 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (0, 2));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (2, 1));
        return pixels;
    }
    private List<Vector2Int> FourTiles8 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (0, 2));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (2, 0));
        return pixels;
    }
    private List<Vector2Int> FourTiles9 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 0));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (2, 1));
        pixels.Add (new Vector2Int (2, 2));
        return pixels;
    }
    private List<Vector2Int> FourTiles10 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (1, 0));
        pixels.Add (new Vector2Int (2, 2));
        return pixels;
    }
    private List<Vector2Int> FourTiles11 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 0));
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (1, 0));
        pixels.Add (new Vector2Int (1, 2));
        return pixels;
    }
    private List<Vector2Int> FourTiles12 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (1, 0));
        pixels.Add (new Vector2Int (1, 2));
        pixels.Add (new Vector2Int (2, 0));
        pixels.Add (new Vector2Int (2, 1));
        return pixels;
    }
    private List<Vector2Int> FourTiles13 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 0));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (2, 0));
        pixels.Add (new Vector2Int (2, 2));
        return pixels;
    }
    private List<Vector2Int> FourTiles14 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (1, 0));
        pixels.Add (new Vector2Int (1, 2));
        pixels.Add (new Vector2Int (2, 0));
        return pixels;
    }
    private List<Vector2Int> FourTiles15 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 0));
        pixels.Add (new Vector2Int (1, 0));
        pixels.Add (new Vector2Int (1, 2));
        pixels.Add (new Vector2Int (2, 1));
        return pixels;
    }

    private List<Vector2Int> ThreeTiles () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 0));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (2, 2));
        return pixels;
    }
    private List<Vector2Int> ThreeTiles1 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (2, 1));
        return pixels;
    }
    private List<Vector2Int> ThreeTiles2 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (1, 0));
        return pixels;
    }
    private List<Vector2Int> ThreeTiles3 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 0));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (0, 2));
        return pixels;
    }
    private List<Vector2Int> ThreeTiles4 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (2, 2));
        return pixels;
    }
    private List<Vector2Int> ThreeTiles5 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (1, 1));
        pixels.Add (new Vector2Int (2, 0));
        return pixels;
    }
    private List<Vector2Int> TwoTile () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (1, 1));
        return pixels;
    }
    private List<Vector2Int> TwoTile1 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 0));
        pixels.Add (new Vector2Int (0, 1));
        return pixels;
    }
    private List<Vector2Int> TwoTile2 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 1));
        pixels.Add (new Vector2Int (0, 2));
        return pixels;
    }
    private List<Vector2Int> TwoTile3 () {
        List<Vector2Int> pixels = new List<Vector2Int> ();
        pixels.Add (new Vector2Int (0, 0));
        pixels.Add (new Vector2Int (1, 1));
        return pixels;
    }
    private List<Vector2Int> Rotate90 (List<Vector2Int> pixels) {
        if (!CanRotate (pixels)) {
            Debug.Log ("Cant rotate");
            return pixels;
        }
        for (int i = 0; i < pixels.Count; i++) {
            pixels[i] = CalculatePosition (new Vector2Int (1, 1), pixels[i]);
        }
        return pixels;
    }
    private bool CanRotate (List<Vector2Int> pixels) {
        foreach (Vector2Int v in pixels) {
            Vector2Int postmp = CalculatePosition (new Vector2Int (1, 1), v);
            if (postmp.x < 0 || postmp.y < 0 || postmp.x > 3 - 1 || postmp.y > 3 - 1) {
                return false;
            }
        }
        return true;
    }
    private List<Vector2Int> Move (List<Vector2Int> pixels, Vector2Int direct) {
        for (int i = 0; i < pixels.Count; i++) {
            pixels[i] += direct;
        }
        return pixels;
    }
    private Vector2Int CalculatePosition (Vector2Int anchor, Vector2Int point) {
        Vector2Int result = Vector2Int.zero;
        Vector3Int[] mat = Rotate90Mat (new Vector3Int (anchor.x, anchor.y, 1));
        result.x = point.x * mat[0].x + point.y * mat[1].x + 1 * mat[2].x;
        result.y = point.x * mat[0].y + point.y * mat[1].y + 1 * mat[2].y;
        return result;
    }
    private Vector3Int[] Rotate90Mat (Vector3Int anchor) {
        Vector3Int[] mat = new Vector3Int[3];
        mat[0] = new Vector3Int (0, 1, 0);
        mat[1] = new Vector3Int (-1, 0, 0);
        mat[2] = new Vector3Int (anchor.y + anchor.x, -anchor.x + anchor.y, 1);
        return mat;
    }
}