using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D;

public class MoveableBehaviour : MonoBehaviour {

    public LayerMask deadLayerMask;

    public float speed = 10f;
    public float rotationSpeed = 5f;

    public float raycastDistance = 0.2f;

    [HideInInspector]
    public LineRenderer path;

    [HideInInspector]
    public Collider destinationCollider;

    private Rigidbody m_RigidBody;

    private bool m_IsPlayingBack = false;

    private Vector3[] m_RecordPositions;


    private Vector3 m_OriginalPosition;
    private Quaternion m_OriginalRotation;

    private bool m_IsEnded;

    void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_OriginalPosition = transform.position;
        m_OriginalRotation = transform.rotation;

    }


    private void Update()
    {

        if (m_IsEnded) return;

        if (m_IsPlayingBack) return;

    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        //m_RigidBody.AddForce(transform.forward * 20);
        //m_RigidBody.MoveRotation(m_RigidBody.rotation * Quaternion.Euler(0, m_Horizontal * 2, 0));

        Vector3 direction = transform.forward.normalized;
        m_RigidBody.velocity = new Vector3(direction.x * speed, m_RigidBody.velocity.y, direction.z * speed);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (deadLayerMask.Contains(collision.gameObject))
        {
            transform.position = m_OriginalPosition;
            transform.rotation = m_OriginalRotation;
            m_RigidBody.velocity = Vector3.zero;
        }
        
    }

    

    private void End()
    {
        //if (!m_IsEnded)
        //{
        //    GameCycleManager.Instance.EndPath();
        //    gameObject.layer = LayerMask.NameToLayer("NoCollider");
        //    m_IsEnded = true;
        //}

        gameObject.SetActive(false);
    }


    private IEnumerator InternalPlayBack()
    {
        m_IsPlayingBack = true;

        transform.position = m_OriginalPosition;
        transform.rotation = m_OriginalRotation;


        int i = 0;

        while (i < m_RecordPositions.Length)
        {

            Vector3 from = m_RecordPositions[i];
            from.y = 0;
            Vector3 to = transform.position;
            to.y = 0;

            Vector3 direction = (from - to);

            if ((direction).sqrMagnitude <= 5f)
            {
                i++;

                if (i == m_RecordPositions.Length) break;

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), Time.deltaTime* rotationSpeed);

            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), Time.deltaTime * rotationSpeed);
            }


            yield return null;
        }

        m_IsPlayingBack = false;
    }

    public void PlayBack()
    {
        m_RecordPositions = new Vector3[path.positionCount];
        path.GetPositions(m_RecordPositions);


        StartCoroutine(InternalPlayBack());
    }





}
