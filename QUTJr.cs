using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;
using UnityEngine.UI;

public class QUTJr : MonoBehaviour
{
    public GameObject child;
    public Vector3 jointLocation;
    public Vector3 jointOffset;
    public float angle;
    public float lastAngle;
    public Vector3[] limbVertexLocations;
    public Vector3 vertexColour;
    private Vector3 toGroundVector = new Vector3(0f, -1.75f, 0f);
    public Mesh mesh;
    public Material material;
    public Vector3 offset;

    public bool moveRight;
    public bool moveLeft;
    public bool moveUp;

    public float timer;

    void Awake()
    {
        // Draw the limb
        DrawLimb();
    }

    void Start()
    {
        if (child != null)
        {
            child.GetComponent<QUTJr>().MoveByOffset(jointOffset);
        }
        moveRight = true;
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

        // Translate to sit on the ground line
        Matrix3x3 meshToOrigin = IGB283Transform.Translate(-offset);
        Matrix3x3 meshToGround = IGB283Transform.Translate(toGroundVector);
        // Move the object to the ground, then back to the origin
        Matrix3x3 avatarToGround = meshToGround * meshToOrigin;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = avatarToGround.MultiplyPoint(vertices[i]);
        }
        mesh.vertices = vertices;
        // Apply the transformation to the joint
        jointLocation = avatarToGround.MultiplyPoint(jointLocation);
    }

    void Update()
    {
        lastAngle = angle;
        if ((Mathf.Round(Time.fixedTime)) % 2 != 0)
        {
            angle -= 0.0005f;
        }
        else
        {
            angle += 0.0005f;
        }


        if (child != null)
        {
            child.GetComponent<QUTJr>().RotateAroundPoint(jointLocation, angle, lastAngle);
        }

        // matrices to make the mesh oscilate
        Matrix3x3 meshToOrigin = IGB283Transform.Translate(-offset);
        Matrix3x3 chaChaRealSmooth = IGB283Transform.Translate(new Vector3(0f, 0f, 0f)) * meshToOrigin;

        //Controls the character
        if (Input.GetKey("d"))
        {
            moveRight = true;
            moveLeft = false;
        }
        else if (Input.GetKey("a"))
        {
            moveLeft = true;
            moveRight = false;
        }
        else if (Input.GetKey("w"))
        {
            //Matrix3x3 oneHopThisTime = IGB283Transform.Translate(new Vector3());
            //chaChaRealSmooth = oneHopThisTime * meshToOrigin;
            moveUp = true;
        }


        if (moveRight == true && moveLeft == false)
        {
            Matrix3x3 takeItBackNowYall = IGB283Transform.Translate(new Vector3(0.01f, 0f, 0.1f));
            chaChaRealSmooth = takeItBackNowYall * meshToOrigin;
        }
        else if(moveLeft == true && moveRight == false)
        {
            Matrix3x3 toTheLeft = IGB283Transform.Translate(new Vector3(-0.01f, 0f, 0f));
            chaChaRealSmooth = toTheLeft * meshToOrigin;
        }
        
        if (moveUp == true)
        {
            timer += Time.deltaTime;
            if (timer >= 0 && timer <= 0.1 && moveRight == true)
            {
                //child.GetComponent<QUTJr>().MoveByOffset(new Vector3(0.0f, 0.025f, 0f));
                Matrix3x3 up = IGB283Transform.Translate(new Vector3(0.01f, 0.05f, 0f));
                chaChaRealSmooth = up * meshToOrigin;
            }
            else if (timer >= 0.1 && timer <= 0.2 && moveRight == true)
            {
                Matrix3x3 up = IGB283Transform.Translate(new Vector3(0.01f, -0.05f, 0f));
                chaChaRealSmooth = up * meshToOrigin;

            }
            if (timer >= 0 && timer <= 0.1 && moveLeft == true)
            {
                //child.GetComponent<QUTJr>().MoveByOffset(new Vector3(0.0f, 0.025f, 0f));
                Matrix3x3 up = IGB283Transform.Translate(new Vector3(-0.01f, 0.05f, 0f));
                chaChaRealSmooth = up * meshToOrigin;
            }
            else if (timer >= 0.1 && timer <= 0.2 && moveLeft == true)
            {
                Matrix3x3 up = IGB283Transform.Translate(new Vector3(-0.01f, -0.05f, 0f));
                chaChaRealSmooth = up * meshToOrigin;

            }
            else if (timer >=0.2f || jointLocation.y <= -1.8f)
            {
                moveUp = false;
                timer = 0;
                
            }

        }
        

        

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = chaChaRealSmooth.MultiplyPoint(vertices[i]);
        }

        
        mesh.vertices = vertices;

        // Apply the transformation to the joint
        jointLocation = chaChaRealSmooth.MultiplyPoint(jointLocation);

        mesh.RecalculateBounds();
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

        if (child != null)
        {
            child.GetComponent<QUTJr>().MoveByOffset(offset);
        }
    }

    public void RotateAroundPoint(Vector3 point, float angle, float lastAngle)
    {
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

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = M.MultiplyPoint(vertices[i]);
        }
        mesh.vertices = vertices;
        // Apply the transformation to the joint
        jointLocation = M.MultiplyPoint(jointLocation);

        // Apply the transformation to the children
        if (child != null)
        {
            child.GetComponent<QUTJr>().RotateAroundPoint(point, angle, lastAngle);
        }
    }
}
