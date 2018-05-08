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

    //private float m_Horizontal;
    //private float m_Vertical;
    //private Vector3 m_ForceVector;
    //public float maxSpeed = 10f;
    //public float force = 5f;

    class RecordInfo
    {
        public float timer;
        public Vector3 velocity;
        public Quaternion rotation;
    }


    private Rigidbody m_RigidBody;

    private bool m_IsPlayingBack = false;

    private List<RecordInfo> m_Record = new List<RecordInfo>();

    private float m_Timer;

    private Vector3 m_OriginalPosition;
    private Quaternion m_OriginalRotation;

	void Awake () {
        m_RigidBody = GetComponent<Rigidbody>();
        m_OriginalPosition = transform.position;
        m_OriginalRotation = transform.rotation;
    }


    private void Update()
    {
        //m_Horizontal = Input.GetAxis("Horizontal");
        //m_Vertical = Input.GetAxis("Vertical");

        //m_ForceVector = new Vector3(0, 0, m_Vertical);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveRecord();
            StartCoroutine(PlayBack());
        }


        if (m_IsPlayingBack) return;

        m_Timer += Time.deltaTime;

        CheckForBounce();

    }

    void FixedUpdate()
    {
        //if (m_IsPlayingBack) return;

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
            transform.position = new Vector3(5.35f, 1.2f, -22.38f);
        }

        //ChangeDirection(collision.contacts[0].normal);
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

        RecordInfo info = new RecordInfo();
        info.timer = m_Timer;
        info.rotation = transform.rotation;
        m_Record.Add(info);

        m_Timer = 0;
    }

    private IEnumerator PlayBack()
    {
        m_IsPlayingBack = true;

        transform.position = m_OriginalPosition;
        transform.rotation = m_OriginalRotation;


        int i = 0;

        while (i < m_Record.Count)
        {

            if (m_Record[i].timer > 0)
            {
                m_Record[i].timer -= Time.deltaTime;

                transform.rotation = m_Record[i].rotation;

                if (m_Record[i].timer <= 0)
                {
                    i++;
                }

            }

            yield return null;
        }



        m_IsPlayingBack = false;
    }



}
