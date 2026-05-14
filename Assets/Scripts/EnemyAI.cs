using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // ── States ────────────────────────────────────────────────────────────────
    private enum State { PATROL, CHASE, ATTACK }
    private State currentState = State.PATROL;

    // ── Inspector fields ──────────────────────────────────────────────────────
    [Header("Patrol")]
    public Transform[] waypoints;
    public float patrolSpeed            = 3.5f;
    public float waypointReachedDistance = 1.5f;

    [Header("Detection")]
    public float chaseRange  = 14f;
    public float attackRange =  8f;   // shoot from distance
    public float loseRange   = 20f;

    [Header("Chase")]
    public float chaseSpeed = 7f;

    [Header("Shooting")]
    public GameObject projectilePrefab;  // assign in Inspector (or created at runtime)
    public Transform  firePoint;         // optional muzzle transform
    public float      fireRate   = 1.2f; // shots per second
    public float      projectileSpeed  = 14f;
    public float      projectileDamage = 15f;

    // ── Private references ────────────────────────────────────────────────────
    private NavMeshAgent agent;
    private Transform    player;
    private PlayerHealth playerHealth;
    private int          waypointIndex = 0;
    private float        fireCooldown  = 0f;

    // ─────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        var playerGO = GameObject.FindWithTag("Player");
        if (playerGO != null)
        {
            player       = playerGO.transform;
            playerHealth = playerGO.GetComponent<PlayerHealth>();
        }
        else
        {
            Debug.LogWarning("[EnemyAI] No GameObject with tag 'Player' found.");
        }

        if (!agent.isOnNavMesh)
            Debug.LogError("[EnemyAI] " + gameObject.name + " is NOT on the NavMesh. Bake the NavMesh.");

        // Snap any waypoints that aren't on the NavMesh to the nearest valid point
        SnapWaypointsToNavMesh();

        EnterPatrol();
    }

    private void SnapWaypointsToNavMesh()
    {
        if (waypoints == null) return;
        foreach (var wp in waypoints)
        {
            if (wp == null) continue;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(wp.position, out hit, 3f, NavMesh.AllAreas))
                wp.position = hit.position;
            else
                Debug.LogWarning("[EnemyAI] Waypoint " + wp.name + " could not be snapped to NavMesh.");
        }
    }

    private void Update()
    {
        if (player == null) return;

        fireCooldown -= Time.deltaTime;

        float dist = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.PATROL: UpdatePatrol(dist); break;
            case State.CHASE:  UpdateChase(dist);  break;
            case State.ATTACK: UpdateAttack(dist); break;
        }
    }

    // ── PATROL ────────────────────────────────────────────────────────────────

    private void EnterPatrol()
    {
        currentState    = State.PATROL;
        agent.speed     = patrolSpeed;
        agent.isStopped = false;
        GoToNextWaypoint();
    }

    private void UpdatePatrol(float dist)
    {
        if (dist <= chaseRange) { EnterChase(); return; }

        if (!agent.pathPending && agent.remainingDistance <= waypointReachedDistance)
            GoToNextWaypoint();
    }

    private void GoToNextWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        agent.SetDestination(waypoints[waypointIndex].position);
        waypointIndex = (waypointIndex + 1) % waypoints.Length;
    }

    // ── CHASE ─────────────────────────────────────────────────────────────────

    private void EnterChase()
    {
        currentState    = State.CHASE;
        agent.speed     = chaseSpeed;
        agent.isStopped = false;
    }

    private void UpdateChase(float dist)
    {
        if (dist <= attackRange) { EnterAttack(); return; }
        if (dist >= loseRange)   { EnterPatrol(); return; }

        agent.SetDestination(player.position);
    }

    // ── ATTACK ────────────────────────────────────────────────────────────────
    // Enemy stops and shoots projectiles at the player.

    private void EnterAttack()
    {
        currentState    = State.ATTACK;
        agent.isStopped = true;
    }

    private void UpdateAttack(float dist)
    {
        if (dist > attackRange * 1.2f) { EnterChase(); return; }

        // Always face the player
        Vector3 lookTarget = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookTarget);

        // Shoot on cooldown
        if (fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1f / fireRate;
        }
    }

    private void Shoot()
    {
        Vector3 origin = firePoint != null ? firePoint.position : transform.position + transform.forward * 0.6f + Vector3.up * 1f;
        Vector3 dir    = (player.position + Vector3.up * 0.8f - origin).normalized;

        if (projectilePrefab != null)
        {
            var go = Instantiate(projectilePrefab, origin, Quaternion.LookRotation(dir));
            var proj = go.GetComponent<EnemyProjectile>();
            if (proj != null)
            {
                proj.speed  = projectileSpeed;
                proj.damage = projectileDamage;
                proj.Init(dir);
            }
        }
        else
        {
            // Fallback: create a primitive sphere projectile at runtime
            CreateRuntimeProjectile(origin, dir);
        }
    }

    private void CreateRuntimeProjectile(Vector3 origin, Vector3 dir)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "EnemyBullet";
        go.transform.position   = origin;
        go.transform.localScale = Vector3.one * 0.25f;

        // Red material
        var rend = go.GetComponent<Renderer>();
        if (rend != null)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            if (mat.shader.name == "Hidden/InternalErrorShader")
                mat = new Material(Shader.Find("Standard"));
            mat.color = Color.red;
            rend.material = mat;
        }

        // Make it a trigger so it passes through environment triggers properly
        var col = go.GetComponent<SphereCollider>();
        if (col != null) col.isTrigger = true;

        // Add a kinematic Rigidbody so trigger detection works
        var rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        var proj = go.AddComponent<EnemyProjectile>();
        proj.speed  = projectileSpeed;
        proj.damage = projectileDamage;
        proj.Init(dir);
    }

    // ── Gizmos ────────────────────────────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, loseRange);
    }
}
