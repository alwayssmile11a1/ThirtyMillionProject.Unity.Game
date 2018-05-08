using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D;

public class VehicleBehaviour : MonoBehaviour {

    public LayerMask bounceLayerMask;
    public LayerMask deadLayerMask;

    
    public float speed = 10f;

    public float raycastDistance = 0.2f;

    public Transform[] rayOrigins;

    [HideInInspector]
    public Collider destinationCollider;

    //class RecordInfo
    //{
    //    public float timer;
    //    public Vector3 velocity;
    //    public Quaternion rotation;
    //}
    //private List<RecordInfo> m_Record = new List<RecordInfo>();

    private Rigidbody m_RigidBody;

    private bool m_IsPlayingBack = false;


    private List<Vector3> m_RecordPositions = new List<Vector3>();

    private float m_Timer;

    private Vector3 m_OriginalPosition;
    private Quaternion m_OriginalRotation;

    private bool m_IsEnded;

	void Awake () {
        m_RigidBody = GetComponent<Rigidbody>();
        m_OriginalPosition = transform.position;
        m_OriginalRotation = transform.rotation;
      
    }


    private void Update()
    {

        if (m_IsEnded) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveRecord();
            StartCoroutine(InternalPlayBack());
        }


        if (m_IsPlayingBack) return;

        m_Timer += Time.deltaTime;

        CheckForBounce();

    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        //m_RigidBody.AddForce(transform.forward * force);
        //m_RigidBody.MoveRotation(m_RigidBody.rotation * Quaternion.Euler(0, m_Horizontal * 2, 0));

        Vector3 direction = transform.forward.normalized;
        m_RigidBody.velocity = new Vector3(direction.x * speed, m_RigidBody.velocity.y, direction.z * speed);
    }

    private void CheckForBounce()
    {
        for (int i = 0; i < rayOrigins.Length; i++)
        {
            Debug.DrawRay(rayOrigins[i].position, transform.forward * raycastDistance, Color.red);

            RaycastHit infoHit = new RaycastHit();

            if (Physics.Raycast(rayOrigins[i].position, transform.forward, out infoHit, raycastDistance, bounceLayerMask.value))
            {
                SaveRecord();
                ChangeDirection(infoHit.normal);
                break;
            }
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

        //ChangeDirection(collision.contacts[0].normal);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other == destinationCollider)
        {
            End();
        }
    }

    private void End()
    {
        if (!m_IsEnded)
        {
            SaveRecord();
            GameCycleManager.Instance.EndPath();
            gameObject.layer = LayerMask.NameToLayer("NoCollider");
            m_IsEnded = true;
        }

        gameObject.SetActive(false);
    }


    private void ChangeDirection(Vector3 surfaceNormal)
    {
        //ChangeDirection
        float angle = Vector3.SignedAngle(surfaceNormal, -transform.forward, Vector3.up);

        //transform.rotation =Quaternion.LookRotation(surfaceNormal, Vector3.up);

        transform.rotation = Quaternion.LookRotation(Quaternion.Euler(0, -angle, 0) * surfaceNormal);

    }


    private void SaveRecord()
    {

        //RecordInfo info = new RecordInfo();
        //info.timer = m_Timer;
        //info.rotation = transform.rotation;
        //m_Record.Add(info);

        //m_Timer = 0;

        m_RecordPositions.Add(transform.position);

    }

    private IEnumerator InternalPlayBack()
    {
        m_IsPlayingBack = true;

        transform.position = m_OriginalPosition;
        transform.rotation = m_OriginalRotation;


        int i = 0;

        //while (i < m_Record.Count)
        //{

        //    if (m_Record[i].timer > 0)
        //    {
        //        m_Record[i].timer -= Time.deltaTime;

        //        transform.rotation = m_Record[i].rotation;

        //        if (m_Record[i].timer <= 0)
        //        {
        //            i++;
        //        }

        //    }

        //    yield return null;
        //}

        while(i<m_RecordPositions.Count)
        {

            if((m_RecordPositions[i]-transform.position).sqrMagnitude <=0.1f)
            {
                i++;

                if (i == m_RecordPositions.Count) break;

                Vector3 direction = (m_RecordPositions[i] - transform.position).normalized;

                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            }
            else
            {
                Vector3 direction = (m_RecordPositions[i] - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }


            yield return null;
        }

        m_IsPlayingBack = false;
    }

    public void PlayBack()
    {
        StartCoroutine(InternalPlayBack());
    }

}
