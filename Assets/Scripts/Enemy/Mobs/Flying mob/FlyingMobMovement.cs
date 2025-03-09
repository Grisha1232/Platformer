using UnityEngine;
 
 public class FlyingMobMovement : DefaultMovement
 {
    [Header("Flying Settings")]
    [SerializeField] private float chaseSpeed = 2f;
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float verticalOffset = 2f;
    [SerializeField] private float stoppingDistance = 1f;
    // [SerializeField] protected float patrolSpeed = 5f; // изменить бы скороть патрулирования
    
    [HideInInspector] public bool isInShadow { get; private set; } = false;
 
    private Collider2D platformCollider;
     private void Awake()
     {
         base.Start();
         attackScript = GetComponent<FlyingMobAttack>();
     }

    private new void Start() {
        
    }

    protected override void Update()
     {
         if (attackScript.PlayerInAggroRange())
         {
             ChasePlayer();
             ToggleIndicators(false);
         }
         else
         {
             Patrol();
             ToggleIndicators(true);
         }
     }
 
     protected override void Patrol()
     {
         Vector2 targetPosition = initialPosition + new Vector3(
             Mathf.PingPong(Time.time * patrolSpeed, patrolRange * 3) - patrolRange,
             Mathf.Sin(Time.time) * 4f
         );
 
         MoveTowards(targetPosition, patrolSpeed);
         
         if ((targetPosition - (Vector2)transform.position).x > 0 != (transform.localScale.x > 0))
         {
             Flip();
         }
     }
 
     private void ChasePlayer()
     {
         Vector2 targetPosition = (Vector2)player.position + new Vector2(0, verticalOffset);
         
         if (Vector2.Distance(transform.position, targetPosition) > stoppingDistance)
         {
             MoveTowards(targetPosition, chaseSpeed);
         }
         else
         {
             body.velocity = Vector2.zero;
             animator.SetFloat("Speed", 0);
         }
 
         Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
         if (direction.x > 0 != (transform.localScale.x > 0))
         {
             Flip();
         }
     }
 
     private void MoveTowards(Vector2 target, float speed)
     {
         Vector2 direction = (target - (Vector2)transform.position).normalized;
         body.velocity = direction * speed;
         animator.SetFloat("Speed", speed);
     }
 
     private void ToggleIndicators(bool isPatrolling)
     {
        //  indicatorPatrol.SetActive(isPatrolling);
        //  indicatorAttack.SetActive(!isPatrolling);
        //  indicatorShadow.SetActive(isInShadow);
     }

     private void OnCollisionEnter2D(Collision2D collision) {
        Debug.Log(LayerMaskToLayerIndex(LayerMask.NameToLayer("Platform")) + " " + collision.gameObject);
        if (collision.gameObject.name == "Platforms") {
            platformCollider = collision.collider;
            Debug.Log("disabling");
            Physics2D.IgnoreCollision(boxCollider, collision.collider, true);
        }
    }

    private int LayerMaskToLayerIndex(LayerMask layerMask)
    {
        int layerIndex = (int)Mathf.Log(layerMask.value, 2);
        return layerIndex;
    }
 
     void Flip()
     {
         Vector3 scale = transform.localScale;
         scale.x *= -1;
         transform.localScale = scale;
     }
 
     private void OnDrawGizmosSelected()
     {
         Gizmos.color = Color.red;
         Gizmos.DrawWireSphere(transform.position, chaseRange);
         Gizmos.color = Color.blue;
         Gizmos.DrawWireSphere(initialPosition, 0.5f);
     }
 }