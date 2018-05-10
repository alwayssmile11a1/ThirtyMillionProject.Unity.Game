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
    public LineRenderer lineRenderer;

    [HideInInspector]
    public Collider destinationCollider;


    private Vector3[] m_RecordPositions;
    private Queue<Vector3> m_LinePositions;
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


    public void FixedUpdate()
    {

        if (!m_IsPlayingBack) return;

        if (m_RigidBody.velocity.sqrMagnitude < maxSpeed * maxSpeed)
        {
            m_RigidBody.AddForce(transform.forward.normalized * force);
        }
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

            if ((direction).sqrMagnitude <= maxDistanceCheck * maxDistanceCheck)
            {
                i++;

                if(m_LinePositions.Count>0)
                {
                    m_LinePositions.Dequeue();
                    lineRenderer.SetPositions(m_LinePositions.ToArray());
                }

                if (i == m_RecordPositions.Length) break;
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
        StopAllCoroutines();
        StartCoroutine(InternalPlayBack());
    }


    public void Stop()
    {
        StopAllCoroutines();
        m_IsPlayingBack = false;
        m_RigidBody.velocity = Vector3.zero;
        transform.position = m_OriginalPosition;
        transform.rotation = m_OriginalRotation;
    }

    public void SaveRecord()
    {
        m_RecordPositions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(m_RecordPositions);
        m_LinePositions = new Queue<Vector3>(m_RecordPositions);

    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawSphere(transform.position, maxDistanceCheck);
    }


}
