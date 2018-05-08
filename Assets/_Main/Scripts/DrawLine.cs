using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour {


    private LineRenderer m_LineRenderer;
    private BoxCollider m_BoxCollider;

	// Use this for initialization
	void Awake () {
        m_LineRenderer = GetComponent<LineRenderer>();

        m_BoxCollider = GetComponentInChildren<BoxCollider>();

    }
	

	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            StartDrawing();
        }


        if(Input.GetMouseButton(0))
        {
            AddDrawingPoint();

        }


        if(Input.GetMouseButtonUp(0))
        {
            EndDrawing();
        }



	}


    private void StartDrawing()
    {
        m_LineRenderer.positionCount = 0;
        m_LineRenderer.enabled = true;
         
    }



    private void AddDrawingPoint()
    {
        //Get ray from camera point to mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Cast ray
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 100f))
        {
            Vector3 newPosition = hitInfo.point;
            newPosition.y = 1;

            if (m_LineRenderer.positionCount == 0)
            {
                m_LineRenderer.positionCount++;
                m_LineRenderer.SetPosition(0, newPosition);
            }
            else
            {
                if ((m_LineRenderer.GetPosition(m_LineRenderer.positionCount - 1) - newPosition).sqrMagnitude >= 0.5f)
                {
                    m_LineRenderer.positionCount = 2;
                    m_LineRenderer.SetPosition(m_LineRenderer.positionCount - 1, newPosition);
                }
            }
        }

       
        
    }

    private void EndDrawing()
    {
        if (m_LineRenderer.positionCount < 2) return;

        //Debug.Log((m_LineRenderer.GetPosition(0) + m_LineRenderer.GetPosition(1)) / 2);
        
        //Rotate box collider
        Vector3 direction = m_LineRenderer.GetPosition(0) - m_LineRenderer.GetPosition(1);
        m_BoxCollider.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        //Set size
        m_BoxCollider.size = new Vector3(1, 1, direction.magnitude);

        //Set position
        m_BoxCollider.transform.position = (m_LineRenderer.GetPosition(0) + m_LineRenderer.GetPosition(1)) / 2;

    }
    


}
