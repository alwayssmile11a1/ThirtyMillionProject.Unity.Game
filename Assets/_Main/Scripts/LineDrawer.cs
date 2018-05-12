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



    public LayerMask undrawableLayerMask;

    public Line linePrefab;

    private Line m_CurrentLine;
    private List<Line> m_Lines = new List<Line>();

	// Use this for initialization
	void Awake () {
        
        if(Instance != this)
        {
            Destroy(gameObject);
        }


    }
	
    public void StartDrawing()
    {
        if(m_CurrentLine==null)
        {
            m_CurrentLine = Instantiate(linePrefab);
            m_Lines.Add(m_CurrentLine);
        }
        else
        {
            m_CurrentLine.lineRenderer.positionCount = 0;
        }

    }



    public void AddDrawingPoint()
    {

        if (m_CurrentLine == null) return;

        //Get ray from camera point to mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        //Cast ray
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 1000f))
        {
            Vector3 newPosition = hitInfo.point;
            newPosition.y = 1;

            //Check obstacles between new position and previous position 
            if (m_CurrentLine.lineRenderer.positionCount > 0)
            {
                //Vector3 direction = newPosition - m_CurrentLine.lineRenderer.GetPosition(m_CurrentLine.lineRenderer.positionCount - 1);
                if(Physics.Linecast(m_CurrentLine.lineRenderer.GetPosition(m_CurrentLine.lineRenderer.positionCount - 1), newPosition, undrawableLayerMask.value))
                {
                    Debug.Log("ds");
                    return;
                }

            }

            m_CurrentLine.AddDrawingPoint(newPosition);
        }

       
        
    }

    public void EndDrawing()
    {
        m_CurrentLine = null;

    }
    
    public List<Line> GetLines()
    {
        return m_Lines;
    }

    public void Clear()
    {
        m_Lines.Clear();
    }

    public Line GetCurrentLine()
    {
        return m_CurrentLine;
    }

}
