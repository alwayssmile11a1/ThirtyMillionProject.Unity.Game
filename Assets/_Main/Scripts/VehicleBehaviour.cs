using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D;

public class VehicleBehaviour : MonoBehaviour {

    public LayerMask bounceLayerMask;
    public LayerMask deadLayerMask;

    //public float force = 5f;
    public float speed = 10f;
    //public float maxSpeed = 10f;

    public float raycastDistance = 0.2f;

    public Transform[] rayOrigins;

    private Rigidbody m_RigidBody;

    //private float m_Horizontal;
    //private float m_Vertical;
    //private Vector3 m_ForceVector;

    private RaycastHit infoHit = new RaycastHit();

	void Awake () {
        m_RigidBody = GetComponent<Rigidbody>();
	}


    private void Update()
    {
        //m_Horizontal = Input.GetAxis("Horizontal");
        //m_Vertical = Input.GetAxis("Vertical");

        //m_ForceVector = new Vector3(0, 0, m_Vertical);


        for (int i = 0; i < rayOrigins.Length; i++)
        {
            Debug.DrawRay(rayOrigins[i].position, transform.forward * raycastDistance, Color.red);

            if (Physics.Raycast(rayOrigins[i].position, transform.forward, out infoHit, raycastDistance, bounceLayerMask.value))
            {
               

                ChangeDirection(infoHit.normal);
                infoHit = new RaycastHit();
                break;
            }
        }

    }

    void FixedUpdate()
    {

        //m_RigidBody.AddForce(transform.forward * force);

        Vector3 direction = transform.forward.normalized;

        m_RigidBody.velocity = new Vector3(direction.x * speed, m_RigidBody.velocity.y, direction.z * speed);

        //m_RigidBody.MoveRotation(m_RigidBody.rotation * Quaternion.Euler(0, m_Horizontal * 2, 0));

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

        float angle = Vector3.SignedAngle(surfaceNormal, -transform.forward, Vector3.up);

        //m_RigidBody.rotation =Quaternion.LookRotation(surfaceNormal, Vector3.up);

        transform.rotation = Quaternion.LookRotation(Quaternion.Euler(0, -angle, 0) * surfaceNormal);

    }

}
