 lastAngle = angle;
        if (child != null)
        {
            child.GetComponent<QUTJr>().RotateAroundPoint(jointLocation, angle, lastAngle);
        }

        //Controls the character
        if (Input.GetKey("d"))
        {
            child.GetComponent<QUTJr>().MoveByOffset(new Vector3(0.01f, 0f, 0.1f));

        }
        else if (Input.GetKey("a"))
        {
                child.GetComponent<QUTJr>().MoveByOffset(new Vector3(-0.01f, 0f, 0f));
        }
        else if (Input.GetKey("w"))
        {
            if (child != null)
            {
                child.GetComponent<QUTJr>().MoveByOffset(new Vector3(0.0f, 0.01f, 0f));
            }    
        }


        // Recalculate the bounds of the mesh
        mesh.RecalculateBounds();

    }
