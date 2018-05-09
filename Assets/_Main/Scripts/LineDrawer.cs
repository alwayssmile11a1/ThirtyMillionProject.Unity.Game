using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour {

    public static LineDrawer Instance
    {
        get
        {
            if (m_Instance != null) return m_Instance;

            m_Instance = FindObjectOfType<LineDrawer>();

            if (m_Instance != null) return m_Instance;

            //create new
            GameObject gameObject = new GameObject("LineDrawer");
            gameObject = Instantiate(gameObject);
            m_Instance = gameObject.AddComponent<LineDrawer>();

            return m_Instance;
        }

    }

    private static LineDrawer m_Instance;


    public Line linePrefab;
    //private BoxCollider m_BoxCollider;

    private Line m_CurrentLine;
    private List<Line> m_Lines = new List<Line>();

	// Use this for initialization
	void Awake () {
        
        if(Instance != this)
        {
            Destroy(gameObject);
        }

        //m_BoxCollider = GetComponentInChildren<BoxCollider>();

    }
	

	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            StartDrawing();
        }


        if (Input.GetMouseButton(0))
        {
            AddDrawingPoint();

        }


        if (Input.GetMouseButtonUp(0))
        {
            EndDrawing();
        }



    }


    public void StartDrawing()
    {
        m_CurrentLine = Instantiate(linePrefab);
        m_Lines.Add(m_CurrentLine);
    }



    public void AddDrawingPoint()
    {
        //Get ray from camera point to mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Cast ray
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 1000f))
        {
            Vector3 newPosition = hitInfo.point;
            newPosition.y = 1;

            m_CurrentLine.AddDrawingPoint(newPosition);
        }

       
        
    }

    public void EndDrawing()
    {

        //m_CurrentLine = null;

        //if (m_LineRenderer.positionCount < 2) return;

        ////Debug.Log((m_LineRenderer.GetPosition(0) + m_LineRenderer.GetPosition(1)) / 2);
        
        ////Rotate box collider
        //Vector3 direction = m_LineRenderer.GetPosition(0) - m_LineRenderer.GetPosition(1);
        //m_BoxCollider.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        ////Set size
        //m_BoxCollider.size = new Vector3(1, 1, direction.magnitude);

        ////Set position
        //m_BoxCollider.transform.position = (m_LineRenderer.GetPosition(0) + m_LineRenderer.GetPosition(1)) / 2;

    }
    
    public List<Line> GetLines()
    {
        return m_Lines;
    }

}
