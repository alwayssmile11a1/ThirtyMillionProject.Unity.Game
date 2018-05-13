using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D;

public class MoveableBehaviour : MonoBehaviour {

    public LayerMask hurtLayerMask;

    public float maxSpeed = 10f;
    public float rotationSpeed = 5f;
    public float maxDistanceCheck = 0.2f;
    public float force = 15f;


    private Vector3[] m_Recorded;

    private Rigidbody m_RigidBody;
    private bool m_IsPlaying;
    private bool m_IsPlayingBack;
    private Vector3 m_OriginalPosition;
    private Quaternion m_OriginalRotation;
    private bool m_IsEnded;
    private int m_CurrentIndex;

    private TrailRenderer m_TrailRenderer;

    void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_TrailRenderer = GetComponentInChildren<TrailRenderer>();

        m_OriginalPosition = transform.position;
        m_OriginalRotation = transform.rotation;

       
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
        
            m_IsEnded = true;

            SaveRecord();

            m_RigidBody.velocity = Vector3.zero;

            Debug.Log(m_TrailRenderer.positionCount);

            PlayBack();
        }

        if(m_IsPlayingBack)
        {
            InternalPlayBack();
        }


        if (m_IsEnded) return;

        if (!m_IsPlaying) return;

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            //Get ray from camera point to mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#else
        if (Input.touches.Length > 0)
        { 
            //Get ray from camera point to mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                 
#endif

            //Cast ray
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 1000f))
            {
                Vector3 from = hitInfo.point;
                from.y = 0;
                Vector3 to = transform.position;
                to.y = 0;
                Vector3 direction = (from - to).normalized;

                RotateTo(direction, Time.deltaTime * rotationSpeed);


            }
        }



    }

    public void FixedUpdate()
    {

        if (!m_IsPlaying /*&& !m_IsPlayingBack*/) return;


        if (m_RigidBody.velocity.sqrMagnitude < maxSpeed * maxSpeed)
        {
            m_RigidBody.AddForce(transform.forward.normalized * force);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hurtLayerMask.Contains(collision.gameObject))
        {
            if (Vector3.Dot(transform.forward, collision.contacts[0].normal) <= 0)
            {
                m_RigidBody.AddForce(collision.contacts[0].normal * 3, ForceMode.Impulse);
            }

        }

    }



    private void End()
    {
        if (!m_IsEnded)
        {
            //GameCycleManager.Instance.EndPath();
            //gameObject.layer = LayerMask.NameToLayer("NoCollider");
            m_IsEnded = true;
        }

        gameObject.SetActive(false);
    }


    private void InternalPlayBack()
    {
        if (m_CurrentIndex < m_Recorded.Length)
        {
            Vector3 from = m_Recorded[m_CurrentIndex];
            from.y = 0;
            Vector3 to = transform.position;
            to.y = 0;

            Vector3 direction = (from - to).normalized;

            if ((transform.position - m_Recorded[m_CurrentIndex]).sqrMagnitude < maxDistanceCheck * maxDistanceCheck)
            {
                m_CurrentIndex++;

                if (m_CurrentIndex == m_Recorded.Length)
                { 

                    m_IsPlayingBack = false;
                    return;
                }
               

               
            }

            RotateTo(direction, Time.deltaTime * rotationSpeed);

            Vector2 moveDirection = transform.forward;

            m_RigidBody.velocity = transform.forward.normalized * maxSpeed/1.3f;

        }

    }

    private void RotateTo(Vector3 direction, float speed)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), speed);
    }


    public void PlayBack()
    {
        if (m_TrailRenderer.positionCount > 0)
        {
            SaveRecord();
            m_TrailRenderer.time = 0;
        }
        transform.position = m_OriginalPosition;
        transform.rotation = m_OriginalRotation;
        m_CurrentIndex = 0;
        m_IsPlayingBack = true;



    }


    public void Stop()
    {

        m_IsPlaying = false;
        m_IsPlayingBack = false;
        m_IsEnded = true;
        m_RigidBody.velocity = Vector3.zero;
        transform.position = m_OriginalPosition;
        transform.rotation = m_OriginalRotation;
    }

    public void Play()
    {
        m_IsPlaying = true;
    }

    public void SaveRecord()
    {

        m_Recorded = new Vector3[m_TrailRenderer.positionCount];
        m_TrailRenderer.GetPositions(m_Recorded);

    }




}
