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

    public CinemachineVirtualCamera virtualMainCamera;
    public CinemachineVirtualCamera wholeSceneVirtualCamera;

    [System.Serializable]
    public class Path
    {
        public GameObject spawnObject;
        public Transform spawnTransform;
        public Collider destination;
    }

    public Path[] paths;



    private static Manager m_Instance;
    private List<MoveableBehaviour> moveables = new List<MoveableBehaviour>();
    private int m_CurrentIndex;

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
            SpawnNewMovable();

            SwitchBackToOriginalCamera();

        }

    }

    public void SpawnNewMovable()
    {
        if (m_CurrentIndex >= paths.Length) return;

        //Instatiate new moveable
        MoveableBehaviour moveable = Instantiate(paths[m_CurrentIndex].spawnObject, paths[m_CurrentIndex].spawnTransform.position, paths[m_CurrentIndex].spawnTransform.rotation).GetComponent<MoveableBehaviour>();
        moveable.destinationCollider = paths[m_CurrentIndex].destination;
        moveable.path = LineDrawer.Instance.GetLines()[LineDrawer.Instance.GetLines().Count - 1].lineRenderer;
        moveable.PlayBack();

        //Set follow point
        virtualMainCamera.Follow = moveable.transform;
        
        //Play back Other
        PlayBackOtherMovables();

        //Add to list 
        moveables.Add(moveable);
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
        for (int i = 0; i < moveables.Count; i++)
        {
            moveables[i].gameObject.SetActive(true);
            moveables[i].PlayBack();
        }
    }

    private void EndPlayBack()
    {
        for (int i = 0; i < moveables.Count - 1; i++)
        {
            moveables[i].gameObject.SetActive(false);
        }
    }

    public void StartPath()
    {
        SwitchBackToOriginalCamera();
    }

    public void EndPath()
    {
        SwitchToWholeSceneCamera();
        EndPlayBack();
        m_CurrentIndex++;
    }

}
