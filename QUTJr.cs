using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;
using UnityEngine.UI;

public class QUTJr : MonoBehaviour
{


    public GameObject child;
    public GameObject control;
    public Vector3 jointLocation;
    public Vector3 jointOffset;
    public float angle;
    public float lastAngle;
    public Vector3[] limbVertexLocations;
    public Vector3 vertexColour;
    public Mesh mesh;

    public Material material;

    public bool right;
    public bool left;

    public int jumpTimer;
    public bool jumpBool;

    float timer;

    void Awake()
    {
        // Draw the limb
        DrawLimb();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (child != null)
        {
            child.GetComponent<QUTJr>().MoveByOffset(jointOffset);
        }
        right = true;
    }

    private void DrawLimb()
    {

        mesh = gameObject.AddComponent<MeshFilter>().mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;

        mesh.vertices = new Vector3[]
        {
            limbVertexLocations[0],
            limbVertexLocations[1],
            limbVertexLocations[2],
            limbVertexLocations[3]
        };

        Vector3[] vertices = mesh.vertices;
        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = new Color(vertexColour.x, vertexColour.y, vertexColour.z, 1f);
        }
        mesh.colors = colors;

        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
    }

    // Update is called once per frame
    void Update()
    {



        lastAngle = angle;
        /*
        if (child != null)
        {
            child.GetComponent<QUTJr>().RotateAroundPoint(
            jointLocation, angle, lastAngle);
        }
        */
        //Controls the character
        if (Input.GetKey("d"))
        {
            right = true;
            left = false;

        }
        if (Input.GetKey("a"))
        {
            left = true;
            right = false;
        }
        if (Input.GetKey("w"))
        {
            jumpBool = true;
            
        }
        

        if (right == true && left == false)
        {
            moveRight();
        }
        else if (left == true && right == false)
        {
            moveLeft();
        }
        // Recalculate the bounds of the mesh

        if (jumpBool == true)
        {
            timer += Time.deltaTime;
            if (timer > 0 && timer <0.2)
            {
                child.GetComponent<QUTJr>().MoveByOffset(new Vector3(0.0f, 0.025f, 0f));
            }
            else if(timer > 0.2 && timer < 0.4)
            {
                child.GetComponent<QUTJr>().MoveByOffset(new Vector3(0.0f, -0.025f, 0f));

            }
            else if(timer > 0.5)
            {
                timer = 0;
                jumpBool = false;
            }

        }
        

        mesh.RecalculateBounds();
        Debug.Log(timer);

    }

    public void moveRight()
    {
        child.GetComponent<QUTJr>().MoveByOffset(new Vector3(0.01f, 0f, 0f));
    }

    public void moveLeft()
    {
        child.GetComponent<QUTJr>().MoveByOffset(new Vector3(-0.01f, 0f, 0f));
    }




    public void MoveByOffset(Vector3 offset)
    {
        Matrix3x3 T = IGB283Transform.Translate(offset);

        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = T.MultiplyPoint(vertices[i]);
        }
        mesh.vertices = vertices;


        jointLocation = T.MultiplyPoint(jointLocation);

        /*
        if (child != null)
        {
            child.GetComponent<QUTJr>().MoveByOffset(offset);
        }
        */
    }

    public void RotateAroundPoint(Vector3 point, float angle, float lastAngle)
    {
        // Move the point to the origin
        Matrix3x3 T1 = IGB283Transform.Translate(-point);
        // Undo the last rotation
        Matrix3x3 R1 = IGB283Transform.Rotate(-lastAngle);
        // Move the point back to the oritinal position
        Matrix3x3 T2 = IGB283Transform.Translate(point);
        // Perform the new rotation
        Matrix3x3 R2 = IGB283Transform.Rotate(angle);
        // The final translation matrix
        Matrix3x3 M = T2 * R2 * R1 * T1;

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = M.MultiplyPoint(vertices[i]);
        }
        mesh.vertices = vertices;
        // Apply the transformation to the joint
        jointLocation = M.MultiplyPoint(jointLocation);


        if (child != null)
        {
            child.GetComponent<QUTJr>().RotateAroundPoint(point, angle, lastAngle);
        }

    }
}
