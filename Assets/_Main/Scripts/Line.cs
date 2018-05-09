using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {

    [HideInInspector]
    public LineRenderer lineRenderer;


    private float distanceBetweenLinePosition = 1f;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }



    public void AddDrawingPoint(Vector3 position)
    {
        if (lineRenderer.positionCount == 0)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(0, position);
        }
        else
        {
            if ((lineRenderer.GetPosition(lineRenderer.positionCount - 1) - position).sqrMagnitude >= distanceBetweenLinePosition * distanceBetweenLinePosition)
            {
                //m_LineRenderer.positionCount = 2;
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);
            }
        }




    }


}
