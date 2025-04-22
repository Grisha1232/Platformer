using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DefaultAttack : MonoBehaviour {

    /// <summary>
    /// Скорость преследования
    /// </summary>
    [SerializeField] protected float chaseSpeed = 5f;

    /// <summary>
    /// Радиус обнаружения игрока
    /// </summary>
    [SerializeField] protected float aggroRange = 10f;

    /// <summary>
    /// Количество урона
    /// </summary>
    [SerializeField] protected float damage = 1.0f;

    /// <summary>
    /// Дальность атаки
    /// </summary>
    [SerializeField] protected float attackRange = 1.0f; 

    /// <summary>
    /// Время перезарядки атаки
    /// </summary>
    [SerializeField] protected float attackCooldown = 2.0f;  

    /// <summary>
    /// Множитель урона при спец приемах
    /// </summary>
    [SerializeField] protected float multiplayerDamage = 2.0f;
    [SerializeField] protected Transform attackTransform;
    [SerializeField] protected LayerMask attackableLayer;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected LayerMask platformLayer;

    /// <summary>
    /// Находится ли враг в атакующем режиме
    /// </summary>
    [HideInInspector] public bool isAttackMode {get; set;} = false;

    private BoxCollider2D boxCollider;
    protected Rigidbody2D body;
    protected Animator animator;
    protected Transform player;
    protected float attackTimeCounter = 9f;

    
    protected bool shouldBeDamaging {get; set;}= true; 

    protected bool isJumping = false;
    protected bool jumpingPhase1 = false;
    protected bool jumpingPhase2 = false;
    protected float counterAfterBug = 0;
    protected int currentPointToFollow;
    [SerializeField] protected float jumpForce = 10f;
    protected List<Vector3Int> pathToFollow;

    private Collider2D platformCollider;

    protected void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        boxCollider = GetComponent<BoxCollider2D>();
    }

    abstract protected IEnumerator Attack();
    abstract public void ChaseToAttack();

    public virtual bool PlayerInAggroRange() {
        isAttackMode = Vector2.Distance(transform.position, player.position) <= aggroRange;
        return isAttackMode;
    }

    public virtual bool PlayerInAttackRange() {
        var hits = Physics2D.CircleCast(attackTransform.position, attackRange, transform.right, 0f, attackableLayer);
        return hits.collider != null;
    }
    
    protected void Flip() {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    
    protected void UpdatePath() {
        if (isJumping) {
            return;
        }
        if (Pathfinder.instance == null) {
            Debug.LogError("Pathfinder.instance = null");
            return;
        }

        if (Pathfinder.instance.getPathLength2(transform.position) >= 30) {
            return;
        }
        var path = Pathfinder.instance.getNextThreeTiles(transform.position);
        if (path == null || path.Count == 0) {
            return;
        }
        pathToFollow = path;
    }

    
    protected void followPath() {

        if (pathToFollow == null) {
            return;
        }

        if (pathToFollow.Count == 0) {
            return;
        }

        if (Pathfinder.instance.getPathLength2(transform.position) <= 1) {
            // animator.SetFloat("Speed", 0);
            return;
        }
        if (!isGrounded()) {
            return;
        }

        // animator.SetFloat("Speed", chaseSpeed);

        // Получаем текущую и следующую точку пути
        Vector3 currentPoint = pathToFollow[0] + new Vector3(0.5f, 0.8f);
        Vector3 afterNextPoint = pathToFollow[2] + new Vector3(0.5f, 0.8f);

        float dir = Mathf.Sign(currentPoint.x - afterNextPoint.x);
        if (Mathf.Abs(currentPoint.y - afterNextPoint.y) == 0 && dir == Mathf.Sign(transform.localScale.x)) {
            Flip();
        } 
       
        if (afterNextPoint.y - currentPoint.y > 0) {
            Jump(afterNextPoint);
        } else if (afterNextPoint.y - currentPoint.y == 0) {
            // Двигаемся к следующей точке
            // Debug.Log("moving from " + transform.position + " towards " + afterNextPoint);
            currentPoint.y = transform.position.y;
            transform.position = Vector2.MoveTowards(transform.position, currentPoint, chaseSpeed * Time.deltaTime);
        } else if (afterNextPoint.y - currentPoint.y < 0) {
            // Двигаемся к следующей точке
            // Debug.Log("moving from " + transform.position + " towards " + afterNextPoint);
            transform.position = Vector2.MoveTowards(transform.position, currentPoint, chaseSpeed * Time.deltaTime);
            
            DisableCollisionWithPlatforms();
            StartCoroutine(MoveDownPlatform());
        }
    }

    private IEnumerator MoveDownPlatform() {
        yield return new WaitUntil( () => {
            var collider = Physics2D.OverlapBox(boxCollider.bounds.center, boxCollider.bounds.size * 1.1f, 0f, layerMask: platformLayer);
            return collider == null;
        });

        EnableCollisionWithPlatforms();

    }

    private void DisableCollisionWithPlatforms() {
        if (platformCollider == null) {
            return;
        }

        Physics2D.IgnoreCollision(platformCollider, boxCollider, true);
    }

    private void EnableCollisionWithPlatforms() {

        Physics2D.IgnoreCollision(platformCollider, boxCollider, false);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMaskToLayerIndex(platformLayer)) {
            platformCollider = collision.collider;
        }
    }

    private int LayerMaskToLayerIndex(LayerMask layerMask)
    {
        int layerIndex = (int)Mathf.Log(layerMask.value, 2);
        return layerIndex;
    }
    
    protected void Jump(Vector2 to) {

        if (!isGrounded()) {
            return;
        }

        Vector2 startPos = transform.position;
        Vector2 endPos = to;
        Vector2 midPos = new Vector2( (startPos.x + endPos.x) / 2f, (startPos.y + endPos.y) / 2f + Mathf.Abs(startPos.y - endPos.y));
        
        float x1 = startPos.x, y1 = startPos.y;
        float x2 = endPos.x, y2 = endPos.y;
        float x3 = midPos.x, y3 = midPos.y;

       // Решаем систему уравнений для a, b, c
        float[,] matrix = {
            { x1 * x1, x1, 1 },
            { x2 * x2, x2, 1 },
            { x3 * x3, x3, 1 }
        };

        float[] rhs = { y1, y2, y3 };

        
        // Находим вершину параболы
        float h;
        float k;

        if (!SolveSystem(matrix, rhs, out float a, out float b, out float c)) {
            h = midPos.x;
            k = midPos.y;
            Debug.LogError("Точки не лежат на одной параболе!");
        } else {
            h = -b / (2 * a);
            k = c - (b * b) / (4 * a);
        }
        

        float flightTime;
        Vector2 initialVelocity;

        CalculateFlightParameters(startPos, endPos, new Vector2(h, k), out flightTime, out initialVelocity);

        // // Время полета
        // float t = Mathf.Sqrt(2 * Mathf.Abs(deltaY) / g);

        // // Начальная скорость
        // float vx = deltaX / t;
        // float vy = Mathf.Abs(deltaY) / t + 0.5f * g * t;

        // Применяем скорость к Rigidbody2D
        body.velocity = initialVelocity;

        // Анимация прыжка
        // animator.SetTrigger("Jump");
    }

     private void CalculateFlightParameters(Vector2 startPoint, Vector2 endPoint, Vector2 dropPoint, out float flightTime, out Vector2 initialVelocity) {
        float g = Mathf.Abs(Physics2D.gravity.y) * body.gravityScale;

        // Время подъема до точки, где тело начало падать
        float t_d = Mathf.Sqrt(2 * (dropPoint.y - startPoint.y) / g);

        // Общее время полета (время подъема + время падения)
        flightTime = 2 * t_d;

        // Горизонтальная составляющая начальной скорости
        float v0x = (endPoint.x - startPoint.x) / flightTime;

        // Вертикальная составляющая начальной скорости
        float v0y = g * t_d;

        // Начальная скорость (в виде Vector2)
        initialVelocity = new Vector2(v0x, v0y);
    }

    private bool SolveSystem(float[,] matrix, float[] rhs, out float a, out float b, out float c) {
        a = b = c = 0;

        // Определитель матрицы
        float det = matrix[0, 0] * (matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1])
                  - matrix[0, 1] * (matrix[1, 0] * matrix[2, 2] - matrix[1, 2] * matrix[2, 0])
                  + matrix[0, 2] * (matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0]);

        if (Mathf.Abs(det) < 0.0001f)
        {
            return false; // Система не имеет решения
        }

        // Находим a, b, c по правилу Крамера
        float detA = rhs[0] * (matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1])
                   - matrix[0, 1] * (rhs[1] * matrix[2, 2] - matrix[1, 2] * rhs[2])
                   + matrix[0, 2] * (rhs[1] * matrix[2, 1] - matrix[1, 1] * rhs[2]);

        float detB = matrix[0, 0] * (rhs[1] * matrix[2, 2] - matrix[1, 2] * rhs[2])
                   - rhs[0] * (matrix[1, 0] * matrix[2, 2] - matrix[1, 2] * matrix[2, 0])
                   + matrix[0, 2] * (matrix[1, 0] * rhs[2] - rhs[1] * matrix[2, 0]);

        float detC = matrix[0, 0] * (matrix[1, 1] * rhs[2] - rhs[1] * matrix[2, 1])
                   - matrix[0, 1] * (matrix[1, 0] * rhs[2] - rhs[1] * matrix[2, 0])
                   + rhs[0] * (matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0]);

        a = detA / det;
        b = detB / det;
        c = detC / det;

        return true;
    }

    protected bool isGrounded() {
        RaycastHit2D groundHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.25f, groundLayer | platformLayer);
        return groundHit.collider != null;
    }
    

}