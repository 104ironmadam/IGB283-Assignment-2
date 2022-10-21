using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGB283Transform : MonoBehaviour
{
    // This class is used to detail the data for the shape tranformations

     public static Matrix3x3 Scale(float sx, float sy)
    {
        Matrix3x3 matrix = new Matrix3x3();
        matrix.SetRow(0, new Vector3(sx, 0.0f, 0.0f));
        matrix.SetRow(1, new Vector3(0.0f, sy, 0.0f));
        matrix.SetRow(2, new Vector3(0.0f, 0.0f, 1.0f));

        return matrix;
    }

    public static Matrix3x3 Translate(Vector3 objectOffset)
    {
        Matrix3x3 matrix = new Matrix3x3();

        matrix.SetRow(0, new Vector3(1, 0, objectOffset.x));
        matrix.SetRow(1, new Vector3(0, 1, objectOffset.y));
        matrix.SetRow(2, new Vector3(0, 0, 1));

        return matrix;
    }
    
    public static Matrix3x3 Rotate(float angle)
    {
        Matrix3x3 matrix = new Matrix3x3();

        matrix.SetRow(0, new Vector3(Mathf.Cos(angle), -Mathf.Sin(angle), 0));
        matrix.SetRow(1, new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0));
        matrix.SetRow(2, new Vector3(0.0f, 0.0f, 1.0f));

        return matrix;
    }
}
