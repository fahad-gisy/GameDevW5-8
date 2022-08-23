using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAi : MonoBehaviour
{    [Header("Waiting Settings")] // waiting time by second > random between 2.5,4.0
    [SerializeField] private float minWaitTime = 2.5f;
    [SerializeField] private float maxWaitTime = 4.0f;

    [Header("Wandering Settings")] //rotation > max,min , > min,max > Time
    [SerializeField] private float minWanderTime = 5f;
    [SerializeField] private float maxWanderTime = 10f;
    // [SerializeField] private float minRotationDistance = 30f;
    // [SerializeField] private float maxRotationDistance = 330f;
    [SerializeField] private float minDistance = 30f;
    [SerializeField] private float maxDistance = 330f;
    

    [Header("Chase Settings")] 
    [SerializeField] private float playerStartChaseDistance = 10f;
    [SerializeField] private float playerStopChaseDistance = 15f;
    
    
    [Header("Attack Settings")]
    [SerializeField] private float startAttackDistance = 3f;
    [SerializeField] private float stopAttackDistance = 5f;
    [SerializeField] private float attackCoolDown = 1.5f;

    [Header("Zombie's Health Settings")] 
    [SerializeField] private float zombieHealth = 100;
    
    
    
    public enum ZombieState
    {
        Wait,
        Wander,
        Chase,
        Attack,
        Death,
    }

   [SerializeField] private ZombieMovement ZombieMovement;
    private ZombieState zombieState = ZombieState.Wait; //normal state
    private bool isWaiting = false;
    private Coroutine _waitingCoroutine;
    //
    private bool isWandering = false;
    private Coroutine _wanderingCoroutine;
    //
    private Transform playerTransfomr;
    private bool isChasing = false;
    //
    private bool canAttack = true;
    void Start()
    {
        ZombieMovement = GetComponent<ZombieMovement>();
        playerTransfomr = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        switch (zombieState)
        {
            case ZombieState.Wait:
                ZombieWait();
                if (ChasePlayer())
                {
                    InitChase();
                }
                break;
            case ZombieState.Wander:
                ZombieWander();
                if (ChasePlayer())
                {
                    InitChase();
                }
                break;
            case ZombieState.Chase:
                if (PlayerDistance <= startAttackDistance)
                {
                    // ZombieMovement.IsMoving = false;
                    zombieState = ZombieState.Attack;
                }
                ZombieChase();
                break;
            case ZombieState.Attack:
                ZombieAttack();
                break;
            case  ZombieState.Death:
                ZombieDeath();
                break;
            default:
                break;
            
        }
        //DEATH
        if (zombieHealth <= 0)
        {
            zombieState = ZombieState.Death;
        }
    }

    private Vector3 PlayerPosition
    {//getting the position of our player so we can make the zombie look at hin and start chasing him
        get { return playerTransfomr.position; }
    }

    
    private bool ChasePlayer()
    {
        if (isChasing)
        {
            if (PlayerDistance <= playerStopChaseDistance)
            {
                return true;
            }
            else
            {
                isChasing = false;
                return false;
            }
            
        }
        else
        {
            if (PlayerDistance <= playerStartChaseDistance)
            {
                isChasing = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }

    private float PlayerDistance
    {//getting the zombie n the player's distance positions
        get { return Vector3.Distance(transform.position, PlayerPosition); }
    }

    private void InitChase()
    {
        StopCoroutine(WalkTime());
        StopCoroutine(GoWandering());
        isWaiting = false;
        isWandering = false;
        // ZombieMovement.IsMoving = true;
        zombieState = ZombieState.Chase;
    }

    private void ZombieWait()
    {
        if (!isWaiting)
        {// if waiting > false go wandering > between max and min Time
            
            _waitingCoroutine = StartCoroutine(GoWandering());
        }
    }

    IEnumerator GoWandering()
    {
        isWaiting = true; 
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        zombieState = ZombieState.Wander;
        isWaiting = false;
    }

     private void ZombieWander()
     {
         if (!isWandering)
         {
             Debug.Log("TryingWandering");
             isWandering = true;
             // transform.Rotate(0f,Random.Range(minRotationDistance,maxRotationDistance),0f);//wandring by rotating the zombie in Y Axis
             float currentX = transform.position.x;
             float currentZ = transform.position.z;
             float targetX = currentX + Random.Range(minDistance, maxDistance);
             float targetZ = currentZ + Random.Range(minDistance, maxDistance);
             //we use the terrain to find the Y
             float targetY = Terrain.activeTerrain.SampleHeight(new Vector3(targetX,0f,targetZ));
             ZombieMovement.SetDest(new Vector3(targetX,targetY,targetZ));
             Debug.Log("Finding new path");
             _waitingCoroutine = StartCoroutine(WalkTime());
         }

         // if (ZombieMovement.hasArrived && isWandering)
         // {
         //     Debug.Log("Arrived WAITING?");
         //     isWandering = false;
         //     zombieState = ZombieState.Wait;
         // }
     }

     IEnumerator WalkTime()
     {
         // ZombieMovement.IsMoving = true;
         yield return new WaitForSeconds(Random.Range(minWanderTime,maxWanderTime));
         // ZombieMovement.IsMoving = false;
         Debug.Log("STOP AND WAIT");
         // ZombieMovement.SetDest(transform.position);
         isWaiting = false;
         zombieState = ZombieState.Wait;
         isWandering = false;
     }

    private void ZombieChase()
    {
        if (ChasePlayer())
        {
            transform.LookAt(playerTransfomr);
            ZombieMovement.SetDest(PlayerPosition);
        }
        else
        {
            // ZombieMovement.IsMoving = false;
            // zombieState = ZombieState.Wait;
            ZombieMovement.SetDest(transform.position);
        }
    }

    private void ZombieAttack()
    {
        if (canAttack)
        {
            //attack code
            Debug.Log("Attacked the player");
            StartCoroutine(AttackCoolDown());
        }
        else
        {
            if (PlayerDistance > stopAttackDistance)
            {
                InitChase();
            }
        }
    }

    IEnumerator AttackCoolDown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCoolDown);
        canAttack = true;
    }

    private void ZombieDeath()
    {
        Destroy(gameObject);
    }
}

