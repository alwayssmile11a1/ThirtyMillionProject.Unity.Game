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
		
        if(Input.GetMouseButtonDown(0))
        {
            StartDrawing();
        }


        if(Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, 100f))
            {
                Vector3 newPosition = hitInfo.point;
                newPosition.y = 1;
                AddDrawingPoint(newPosition);
            }


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



    private void AddDrawingPoint(Vector3 position)
    {
        //Debug.Log(position);

        if(m_LineRenderer.positionCount==0)
        {
            m_LineRenderer.positionCount++;
            m_LineRenderer.SetPosition(0, position);
        }
        else
        {
            if ((m_LineRenderer.GetPosition(m_LineRenderer.positionCount - 1) - position).sqrMagnitude >=0.5f)
            {
                m_LineRenderer.positionCount = 2;
                m_LineRenderer.SetPosition(m_LineRenderer.positionCount - 1, position);
            }
        }


       
        
    }

    private void EndDrawing()
    {
        if (m_LineRenderer.positionCount < 2) return;

        Debug.Log((m_LineRenderer.GetPosition(0) + m_LineRenderer.GetPosition(1)) / 2);


        

        Vector3 direction = m_LineRenderer.GetPosition(0) - m_LineRenderer.GetPosition(1);

        m_BoxCollider.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);



        m_BoxCollider.size = new Vector3(1, 1, direction.magnitude);

        m_BoxCollider.transform.position = (m_LineRenderer.GetPosition(0) + m_LineRenderer.GetPosition(1)) / 2;

    }
    


}
