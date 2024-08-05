using System.Collections.Generic;
using UnityEngine;

public class MatrixComparer
{
    public const float MatrixEqualityThreshold = 0.01f;

    public static bool Equals(Matrix4x4 a, Matrix4x4 b) =>
        MatrixDifference(a, b) < MatrixEqualityThreshold;

    public static float MatrixDifference(Matrix4x4 a, Matrix4x4 b)
    {
        var totalDifference = 0f;
        for (var i = 0; i < 9; i++)
            totalDifference += Mathf.Abs(a[i] - b[i]);
        return totalDifference;
    }
}