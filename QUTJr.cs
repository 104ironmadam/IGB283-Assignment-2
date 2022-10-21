using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;
using UnityEngine.UI;

public class QUTJr : MonoBehaviour
{
    public GameObject child;
    // public GameObject control;
    public Vector3 jointLocation;
    public Vector3 jointOffset;
    public float angle;
    public float lastAngle;
    public Vector3[] limbVertexLocations;
    public Mesh mesh;
    public Material material;
    
    void Awake() {
        // Draw the limb
        DrawLimb();
    }

    void Start()
    {
        // Move the child to the joint location
        if (child != null) {
            child.GetComponent<QUTJr>().MoveByOffset(jointOffset);
        }
    }

    private void DrawLimb() {
        mesh = gameObject.AddComponent<MeshFilter>().mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;        

        mesh.vertices = new Vector3[]
        {
            limbVertexLocations[0],
            limbVertexLocations[1],
            limbVertexLocations[2],
            limbVertexLocations[3]
        };

        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        Vector3[] vertices = mesh.vertices;

        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = new Color(0.0f , 0.0f, 1.0f, 1.0f);
        }

        mesh.colors = colors;
    }

    // Update is called once per frame
    void Update()
    {
        lastAngle = angle;
        /* if (control != null) {
            angle = control.GetComponent<Slider>().value;
        }
        */
        if (child != null) {
            child.GetComponent<QUTJr>().RotateAroundPoint(
            jointLocation, angle, lastAngle);
        }
        mesh.RecalculateBounds();
    }

    public void MoveByOffset (Vector3 offset) {
        // FInd the translation matrix
        Matrix3x3 T = IGB283Transform.Translate(jointOffset);
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = T.MultiplyPoint(vertices[i]);
        }

        mesh.vertices = vertices;
        jointLocation = T.MultiplyPoint(jointLocation);

        if (child != null) {
            child.GetComponent<QUTJr>().MoveByOffset(jointOffset);
        }
    }
    
    public void RotateAroundPoint (Vector3 point, float angle, float lastAngle) {
        // Move the point to the origin
        Matrix3x3 T1 = IGB283Transform.Translate(-point);
        // Undo the last rotation
        Matrix3x3 R1 = IGB283Transform.Rotate(-lastAngle);
        // Move the point back to the original position
        Matrix3x3 T2 = IGB283Transform.Translate(point);
        // Perform the new rotation
        Matrix3x3 R2 = IGB283Transform.Rotate(angle);
        // The final translation matrix
        Matrix3x3 M = T2 * R2 * R1 * T1;

        // Move the mesh
        Vector3[] vertices = mesh.vertices;
        for(int i = 0; i < vertices.Length; i++) {
            vertices[i] = M.MultiplyPoint(vertices[i]);
        }
        mesh.vertices = vertices;
        // Apply the transformation to the joint
        jointLocation = M.MultiplyPoint(jointLocation);

        // Apply the transformation to the children
        if (child != null) {
            child.GetComponent<QUTJr>().RotateAroundPoint(point, angle, lastAngle);
        }
    }     
    
}