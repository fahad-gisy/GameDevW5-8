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
    [SerializeField] private float minRotationDistance = 30f;
    [SerializeField] private float maxRotationDistance = 330f;

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

    private ZombieMovement ZombieMovement;
    private ZombieState zombieState = ZombieState.Wait; //normal state
    private bool isWaiting = false;
    private Coroutine waitingCoroutine;
    //
    private bool isWandering = false;
    private Coroutine wanderingCoroutine;
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
                if (ChasePlayer())
                {
                    InitChase();
                }
                ZombieWait();
                break;
            case ZombieState.Wander:
                if (ChasePlayer())
                {
                    InitChase();
                }
                ZombieWander();
                break;
            case ZombieState.Chase:
                if (PlayerDistance <= startAttackDistance)
                {
                    ZombieMovement.IsMoving = false;
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
        StopCoroutine(wanderingCoroutine);
        StopCoroutine(waitingCoroutine);
        isWaiting = false;
        isWandering = false;
        ZombieMovement.IsMoving = true;
        zombieState = ZombieState.Chase;
    }

    private void ZombieWait()
    {
        if (!isWaiting)
        {// if waiting > false go wandering > between max and min Time
            waitingCoroutine = StartCoroutine((string)GoWandering()); 
        }
    }

     IEnumerable GoWandering()
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
             isWandering = true;
             transform.Rotate(0f,Random.Range(minRotationDistance,maxRotationDistance),0f);//wandring by rotating the zombie in Y Axis
             waitingCoroutine = StartCoroutine((string)WalkTime());
         }
     }

     IEnumerable WalkTime()
     {
         ZombieMovement.IsMoving = true;
         yield return new WaitForSeconds(Random.Range(minRotationDistance,maxRotationDistance));
         ZombieMovement.IsMoving = false;
         isWaiting = false;
         zombieState = ZombieState.Wait;
         isWandering = false;
     }

    private void ZombieChase()
    {
        if (ChasePlayer())
        {
            transform.LookAt(playerTransfomr);
        }
        else
        {
            ZombieMovement.IsMoving = false;
            zombieState = ZombieState.Wait;
        }
    }

    private void ZombieAttack()
    {
        if (canAttack)
        {
            //attack code
            Debug.Log("Attacked the player");
            StartCoroutine((string)AttackCoolDown());
        }
        else
        {
            if (PlayerDistance > stopAttackDistance)
            {
                InitChase();
            }
        }
    }

    IEnumerable AttackCoolDown()
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

