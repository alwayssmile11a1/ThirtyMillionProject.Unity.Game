using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D;

public class MoveableBehaviour : MonoBehaviour {

    public LayerMask deadLayerMask;

    public float maxSpeed = 10f;
    public float rotationSpeed = 5f;
    public float maxDistanceCheck = 7f;
    public float force = 15f; 

    [HideInInspector]
    public Vector3[] recordPositions;

    [HideInInspector]
    public Collider destinationCollider;

    private Rigidbody m_RigidBody;

    private bool m_IsPlayingBack = false;



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
        if (m_RigidBody.velocity.sqrMagnitude < maxSpeed * maxSpeed)
        {
            m_RigidBody.AddForce(transform.forward.normalized * force);
        }

        //Vector3 direction = transform.forward.normalized;

        //Vector3 desiredVelocity = new Vector3(direction.x * speed, m_RigidBody.velocity.y, direction.z * speed);

        //m_RigidBody.velocity = Vector3.Slerp(m_RigidBody.velocity, desiredVelocity,Time.deltaTime * 4f);

        //m_RigidBody.velocity = new Vector3(direction.x * speed, m_RigidBody.velocity.y, direction.z * speed);
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

        while (i < recordPositions.Length)
        {

            Vector3 from = recordPositions[i];
            from.y = 0;
            Vector3 to = transform.position;
            to.y = 0;

            Vector3 direction = (from - to);

            if ((direction).sqrMagnitude <= maxDistanceCheck * maxDistanceCheck)
            {
                i++;

                if (i == recordPositions.Length) break;

            }


            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), Time.deltaTime * rotationSpeed);

            ////OR use cross product
            //float rotateAmount = Vector3.Cross(direction, transform.forward).y;
            //m_RigidBody.angularVelocity = new Vector3(0, -rotateAmount * rotationSpeed, 0);



            yield return null;
        }

        m_IsPlayingBack = false;
    }

    public void PlayBack()
    {
        StartCoroutine(InternalPlayBack());
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawSphere(transform.position, maxDistanceCheck);
    }


}
