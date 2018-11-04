using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matrix {

    /// <summary>
    /// số hàng 
    /// </summary>
    public int row { get; private set; }
    /// <summary>
    /// số cột
    /// </summary>
    public int collumn { get; private set; }

    public int[,] matrix { get; private set; }
    public Matrix(int row, int collumn) {
        this.row = Mathf.Abs(row);
        this.collumn = Mathf.Abs(collumn);
        this.matrix = new int[this.row, this.collumn];
    }
    public Matrix(int[,] matrix) {
        this.row = matrix.GetLength(1);
        this.collumn = matrix.GetLength(2);
        this.matrix = matrix;
    }
    public void SetMatrix(int[,] matrix) {
        for (int x = 0; x < matrix.GetLength(0); x++)
            for (int y = 0; y < matrix.GetLength(1); y++)
                this.matrix[x, y] = matrix[x, y];
    }

    public static Matrix operator *(Matrix mat1, Matrix mat2) {
        Matrix result = new Matrix(mat1.collumn, mat2.row);
        for (int x = 0; x < result.row; x++)
            for (int y = 0; y < result.collumn; y++) {
                result.matrix[x, y] = SumOfRowCollumn(mat1, mat2, x, y);
            }
        return result;
    }
    private static int SumOfRowCollumn(Matrix mat1, Matrix mat2, int row, int col) {
        List<int> r = new List<int>();
        List<int> c = new List<int>();
        for(int i = 0;i< mat1.collumn;i++) {
            r.Add(mat1.matrix[i, row]);
            c.Add(mat2.matrix[col, i]);
        }


        return 0;
    }
}
