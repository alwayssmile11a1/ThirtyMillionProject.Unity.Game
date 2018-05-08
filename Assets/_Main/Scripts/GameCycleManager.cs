using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameCycleManager : MonoBehaviour {


    public static GameCycleManager Instance
    {
        get
        {
            if (m_Instance != null) return m_Instance;

            m_Instance = FindObjectOfType<GameCycleManager>();

            if (m_Instance != null) return m_Instance;

            GameObject gameObject = new GameObject("GameManager");
            gameObject = Instantiate(gameObject);
            m_Instance = gameObject.AddComponent<GameCycleManager>();
           

            return m_Instance;
        }
            
    }

    private static GameCycleManager m_Instance;

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

    public List<VehicleBehaviour> vehicles = new List<VehicleBehaviour>();


    private int m_CurrentIndex;

    // Use this for initialization
    void Awake () {

        if(Instance!=this)
        {
            Destroy(gameObject);
        }

    }
	
	// Update is called once per frame
	void Update () {
	    
        if(Input.GetKeyDown(KeyCode.W))
        {
            wholeSceneVirtualCamera.enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            wholeSceneVirtualCamera.enabled = false;
        }


        if(Input.GetKeyDown(KeyCode.F))
        {
            SpawnNewPath();

            SwitchBackToOriginalCamera();

        }

    }

    public void SpawnNewPath()
    {
        if (m_CurrentIndex >= paths.Length) return;


        VehicleBehaviour vehicle = Instantiate(paths[m_CurrentIndex].spawnObject, paths[m_CurrentIndex].spawnTransform.position, paths[m_CurrentIndex].spawnTransform.rotation).GetComponent<VehicleBehaviour>();


        vehicle.destinationCollider = paths[m_CurrentIndex].destination;


        vehicles.Add(vehicle);

        virtualMainCamera.Follow = vehicle.transform;


        PlayBackOtherVehicles();

    }



    private void SwitchToWholeSceneCamera()
    {
        wholeSceneVirtualCamera.enabled = true;
    }

    private void SwitchBackToOriginalCamera()
    {
        wholeSceneVirtualCamera.enabled = false;
    }
    
    private void PlayBackOtherVehicles()
    {
        for (int i = 0; i < vehicles.Count - 1; i++)
        {
            vehicles[i].gameObject.SetActive(true);
            vehicles[i].PlayBack();
        }
    }

    private void EndPlayBack()
    {
        for (int i = 0; i < vehicles.Count - 1; i++)
        {
            vehicles[i].gameObject.SetActive(false);
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
