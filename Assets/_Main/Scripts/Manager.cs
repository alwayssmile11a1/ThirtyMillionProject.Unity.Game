using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Manager : MonoBehaviour {

    public static Manager Instance
    {
        get
        {
            if (m_Instance != null) return m_Instance;

            m_Instance = FindObjectOfType<Manager>();

            if (m_Instance != null) return m_Instance;

            //create new
            GameObject gameObject = new GameObject("GameManager");
            gameObject = Instantiate(gameObject);
            m_Instance = gameObject.AddComponent<Manager>();

            return m_Instance;
        }

    }

    [System.Serializable]
    public class Path
    {
        public GameObject spawnObject;
        public Transform spawnTransform;
        public Collider destination;
    }

    [Header("Camera")]
    public CinemachineVirtualCamera virtualMainCamera;
    public CinemachineVirtualCamera wholeSceneVirtualCamera;

    [Header("Canvas")]
    public Canvas startCanvas;
    public Canvas endCanvas;  

    public Path[] paths;


    private static Manager m_Instance;
    private List<MoveableBehaviour> moveables = new List<MoveableBehaviour>();
    private int m_CurrentIndex;
    private MoveableBehaviour m_CurrentMoveable;

    // Use this for initialization
    void Awake()
    {

        if (Instance != this)
        {
            Destroy(gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.W))
        {
            wholeSceneVirtualCamera.enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            wholeSceneVirtualCamera.enabled = false;
        }


        if (Input.GetKeyDown(KeyCode.F))
        {
            StartPath();

        }


        if(Input.GetKeyDown(KeyCode.C))
        {
            EndPath();
            SpawnNewMovable();
        }


        //if (LineDrawer.Instance.GetCurrentLine()!=null)
        //{
        //    LineDrawer.Instance.AddDrawingPoint();
        //}


        //if (Input.GetMouseButton(0))
        //{
        //    LineDrawer.Instance.AddDrawingPoint();

        //}


        //if (Input.GetMouseButtonUp(0))
        //{
        //    LineDrawer.Instance.EndDrawing();
        //}


    }

    public void SpawnNewMovable()
    {
        if (m_CurrentIndex >= paths.Length) return;

        //Instatiate new moveable
        m_CurrentMoveable = Instantiate(paths[m_CurrentIndex].spawnObject, paths[m_CurrentIndex].spawnTransform.position, paths[m_CurrentIndex].spawnTransform.rotation).GetComponent<MoveableBehaviour>();
        m_CurrentMoveable.destinationCollider = paths[m_CurrentIndex].destination;

        //Setup canvases
        startCanvas.gameObject.SetActive(true);
        startCanvas.transform.position = m_CurrentMoveable.transform.position + m_CurrentMoveable.transform.forward * 2;
        endCanvas.gameObject.SetActive(true);
        endCanvas.transform.position = paths[m_CurrentIndex].destination.transform.position;

        //Set follow point
        virtualMainCamera.Follow = m_CurrentMoveable.transform;

        //Add to list 
        moveables.Add(m_CurrentMoveable);

        m_CurrentIndex++;
    }



    private void SwitchToWholeSceneCamera()
    {
        wholeSceneVirtualCamera.enabled = true;
    }

    private void SwitchBackToOriginalCamera()
    {
        wholeSceneVirtualCamera.enabled = false;
    }

    private void PlayBackOtherMovables()
    {
        for (int i = 0; i < moveables.Count - 1; i++)
        {
            moveables[i].gameObject.SetActive(true);
            moveables[i].PlayBack();
        }
    }

    private void EndPlayBack()
    {
        for (int i = 0; i < moveables.Count; i++)
        {
            moveables[i].Stop();
        }
    }

    public void StartPath()
    {
        startCanvas.gameObject.SetActive(false);
        endCanvas.gameObject.SetActive(false);

        if (m_CurrentMoveable == null) return;

        //Set path to move
        m_CurrentMoveable.lineRenderer = LineDrawer.Instance.GetLines()[LineDrawer.Instance.GetLines().Count - 1].lineRenderer;
        m_CurrentMoveable.SaveRecord();

        m_CurrentMoveable.PlayBack();

        //Play back Other
        PlayBackOtherMovables();
        
        SwitchBackToOriginalCamera();
    }

    public void EndPath()
    {
        SwitchToWholeSceneCamera();
        EndPlayBack();
        m_CurrentMoveable = null;
    }

}
