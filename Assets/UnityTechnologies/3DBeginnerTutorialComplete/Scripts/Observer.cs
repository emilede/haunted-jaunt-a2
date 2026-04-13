using UnityEngine;
using UnityEngine.AI;

public class Observer : MonoBehaviour
{
    public enum State { Patrol, Chase }

    public Transform player;
    public GameEnding gameEnding;

    [Header("Vision")]
    public float viewAngle = 45f;
    public float viewDistance = 15f;
    public float fillRate = 0.6f;
    public float decayRate = 0.3f;

    [Header("Chase")]
    public NavMeshAgent navMeshAgent;
    public WaypointPatrol waypointPatrol;
    public float catchDistance = 1.5f;
    public float chaseSpeed = 4.5f;

    [Header("Tint")]
    public Renderer ghostRenderer;
    public Color chaseTint = Color.red;

    [Header("Blood")]
    public ParticleSystem bloodParticles;

    [Header("Audio")]
    public AudioClip chaseStinger;
    public float stingerVolume = 1f;

    State m_State = State.Patrol;
    float m_Suspicion;

    void Awake()
    {
        if (navMeshAgent == null) navMeshAgent = GetComponent<NavMeshAgent>();
        if (waypointPatrol == null) waypointPatrol = GetComponent<WaypointPatrol>();
        if (ghostRenderer == null) ghostRenderer = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        bool sees = CanSeePlayer();

        float target = sees ? 1f : 0f;
        float rate = sees ? fillRate : decayRate;
        m_Suspicion = Mathf.MoveTowards(m_Suspicion, target, rate * Time.deltaTime);

        if (m_State == State.Patrol && m_Suspicion >= 1f)
            EnterChase();

        if (m_State == State.Chase)
            TickChase();
    }

    bool CanSeePlayer()
    {
        Vector3 offset = player.position - transform.position;
        float distance = offset.magnitude;
        if (distance > viewDistance) return false;

        Vector3 toPlayer = offset / distance;
        float dot = Vector3.Dot(transform.forward, toPlayer);
        if (dot <= Mathf.Cos(viewAngle * Mathf.Deg2Rad)) return false;

        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = (player.position + Vector3.up) - origin;
        Ray ray = new Ray(origin, direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, viewDistance);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (var hit in hits)
        {
            if (hit.transform.root == transform.root) continue;
            return hit.transform.root == player.root;
        }
        return false;
    }

    void EnterChase()
    {
        m_State = State.Chase;
        if (waypointPatrol != null) waypointPatrol.enabled = false;
        if (navMeshAgent != null) navMeshAgent.speed = chaseSpeed;
        if (ghostRenderer != null) ghostRenderer.material.color = chaseTint;
        if (chaseStinger != null)
            AudioSource.PlayClipAtPoint(chaseStinger, transform.position, stingerVolume);
    }

    void TickChase()
    {
        if (navMeshAgent != null)
            navMeshAgent.SetDestination(player.position);

        if (Vector3.Distance(transform.position, player.position) <= catchDistance)
        {
            if (bloodParticles != null)
            {
                bloodParticles.transform.position = player.position + Vector3.up;
                bloodParticles.Play();
            }
            gameEnding.CaughtPlayer();
        }
    }

}
