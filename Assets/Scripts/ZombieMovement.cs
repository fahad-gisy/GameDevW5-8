using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieMovement : MonoBehaviour
{
    //this will not be a singleton
    //each zombie we make will have it's own movement script

    //we'll enable and disable this with our ZombieAI script.
    public bool IsMoving = false;
    //this will control the zombie movement speed.
    [SerializeField] private float MoveSpeed = 5f;

    //our rigidbody for movement
    public CharacterController charController;
    public NavMeshAgent _agent;
    // public bool hasArrived = false;
    void Start()
    {
        charController = GetComponent<CharacterController>();
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //if we're not moving, then exit the update
        // if (!IsMoving)//false
        // {
        //     Debug.Log("Away");
        //     //exit the Update
        //     return;
        // }
        //
        // Debug.Log("Close");
        // //move the zombie forward in the z axis only
        // charController.Move(transform.TransformDirection(Vector3.forward) * MoveSpeed * Time.deltaTime);

        // if (transform.position.magnitude >= _agent.destination.magnitude)
        // {
        //     Debug.Log("Arrived");
        //     hasArrived = true;
        // }

    }

    public void SetDest(Vector3 dest)
    {
        _agent.SetDestination(dest);
    }
}
