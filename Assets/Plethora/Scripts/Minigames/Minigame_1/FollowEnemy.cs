using Boom.Patterns.Broadcasts;
using Boom.Utility;
using PlethoraV2.Minigames;
using PlethoraV2.Mono;
using PlethoraV2.Patterns.RuntimeSet;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using PlethoraV2.Utility;

[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(HealthComponent))]
public class FollowEnemy : MonoBehaviour
{
    [SerializeField] GameObject rotatingPart;
    [SerializeField] GameObject asset;
    [SerializeField] Collider bodyCollider;

    [SerializeField, ShowOnly] Vector3 walkPoint;
    [SerializeField, ShowOnly] bool walkPointSet;
    [SerializeField] float walkPointRange;

    [SerializeField] float timeBetweenAttacks;
    [SerializeField] bool alreadyAttacked;

    [SerializeField] float fovAngleChance = 360;
    [SerializeField] float fovAngleAttack = 90;

    [SerializeField] float sightRange = 20;
    [SerializeField] float attackRange = 5;

    [SerializeField] float waitTimeForNextAttack = 2f;
    [SerializeField, ShowOnly] float currentWaitTimeForNextAttack = 0;

    [SerializeField] float attackPower = 20;

    [SerializeField] BaseMinigameManager.ScoreType onDeathPoints;

    int speedAnimHash;
    int attackAnimHash;

    [SerializeField, ShowOnly] Transform target;
    [SerializeField, ShowOnly] bool playerInSightRange;
    [SerializeField, ShowOnly] bool playerInAttackRange;
    [SerializeField, ShowOnly] bool attaking;

    [SerializeField] private int requiredFramesTargetCheck = 9;

    private int frameCount = 0;

    private NavMeshAgent agent;
    private HealthComponent healthComponent;

    BaseMinigameManager.GameState gameState;

    [SerializeField, ShowOnly] ConfigurableCharacter configurableCharacter;
    [SerializeField, ShowOnly] MonoDictionaryEvent animatorEventSystem;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        healthComponent = GetComponent<HealthComponent>();

        healthComponent.OnRevive.AddListener(OnReviveHandler);
        healthComponent.OnDeath.AddListener(OnDeathHandler);

        BroadcastState.Register<BaseMinigameManager.MinigameState>(StateChangeHandler, new BroadcastState.BroadcastSetting() { invokeOnRegistration = true });

        speedAnimHash = Animator.StringToHash("Speed");
        attackAnimHash = Animator.StringToHash("Action1"); //Attack

        if (TryGetComponent<ConfigurableCharacter>(out configurableCharacter) == false)
        {
            Debug.LogError($"Character doesn't not have compoment of type {nameof(ConfigurableCharacter)}");
            return;
        }
        configurableCharacter.OnSkinChange.AddListener(OnSkinChangeHandler);

        if (asset.TryGetComponent<MonoDictionaryEvent>(out animatorEventSystem) == false)
        {
            Debug.LogError($"Character doesn't not have compoment of type {nameof(MonoDictionaryEvent)}");
            return;
        }

        animatorEventSystem.AddListener("onAttackEnd", EndAttackHandler);

    }

    private void OnSkinChangeHandler(GameObject arg0)
    {
    }

    private void Start()
    {
        agent.enabled = true;
    }



    private void OnDestroy()
    {
        healthComponent.OnRevive.RemoveListener(OnReviveHandler);

        healthComponent.OnDeath.RemoveListener(OnDeathHandler);

        BroadcastState.Unregister<BaseMinigameManager.MinigameState>(StateChangeHandler);

        if(animatorEventSystem)
            animatorEventSystem.RemoveListener("onAttackEnd", EndAttackHandler);
    }

    private void StateChangeHandler(BaseMinigameManager.MinigameState state)
    {
        gameState = state.gameState;
    }

    private void Update()
    {
        ++frameCount;

        if (asset.gameObject.activeSelf == false) asset.gameObject.SetActive(true);
        if (rotatingPart.gameObject.activeSelf == false) rotatingPart.gameObject.SetActive(true);

        if (configurableCharacter.Animator == null) return;


        if (frameCount % 1 == 0)
        {
            if (gameState != BaseMinigameManager.GameState.Playing)
            {
                configurableCharacter.Animator.SetFloat(speedAnimHash, 0);
                if(agent.isOnNavMesh && agent.enabled) agent.isStopped = true;
                return;
            }

            configurableCharacter.Animator.SetFloat(speedAnimHash, Vector3.Distance(transform.position, agent.destination) > 0.25f && !attaking ? 1 : 0);


            target = RuntimeSet.FindClosest(RuntimeSet.Group.Player, RuntimeSet.Channel.A, transform, sightRange, fovAngleChance);

            playerInSightRange = target;

            if (!playerInSightRange)
            {
                return;
            }

            playerInAttackRange = target.IsInView(transform, attackRange, fovAngleAttack);

            if (agent.enabled == false) return;

            if (!playerInAttackRange)
            {
                if(!attaking) SetDestination(target);
            }
            else
            {
                DoAttack();
                transform.rotation = transform.rotation.RotateSlerp((target.position - transform.position).normalized, 5);
            }
        }
    }

    private void SetDestination(Transform target)
    {

        agent.SetDestination(target.position);
    }

    private void DoAttack()
    {
        if (attaking) return;
        attaking = true;
        configurableCharacter.Animator.SetBool(attackAnimHash, true);
    }




    private void OnDrawGizmos()
    {
        if (agent == null) return;
        if (agent.enabled == false) return;

        // Set up the gizmo color for field of view visualization
        Gizmos.color = Color.yellow;
        // Draw the field of view using a wire sphere
        Gizmos.DrawWireSphere(transform.position, sightRange);
        // Draw the field of view angle using lines
        DrawFieldOfView(sightRange, fovAngleChance, Color.yellow);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        DrawFieldOfView(attackRange, fovAngleAttack, Color.blue);
    }

    void DrawFieldOfView(float range, float fovAngle, Color color)
    {
        
        Vector3 fovLine1 = Quaternion.Euler(0, fovAngle / 2, 0) * transform.forward * range;
        Vector3 fovLine2 = Quaternion.Euler(0, -fovAngle / 2, 0) * transform.forward * range;

        Gizmos.color = color;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);
    }

    private void OnReviveHandler()
    {
        agent.enabled = true;
        asset.SetActive(true);
    }
    private void OnDeathHandler()
    {
        bodyCollider.enabled = false;
        agent.enabled = false;
        //asset.SetActive(false);
        configurableCharacter.Animator.enabled = false;
        rotatingPart.transform.localScale = rotatingPart.transform.localScale.MulY(.15f);

        CoroutineManagerUtil.DelayAction(() =>
        {
            Destroy(gameObject);
            //asset.SetActive(false);
        },2, transform);

        Broadcast.Invoke(new BaseMinigameManager.AddScore(onDeathPoints));
    }

    private void EndAttackHandler()
    {
        configurableCharacter.Animator.SetBool(attackAnimHash, false);

        Attack();
    }

    private void Attack()
    {
        if (gameState != BaseMinigameManager.GameState.Playing) return;

        StartCoroutine(WaitForNextAttackRoutine());


        if (target.IsInView(transform, attackRange, fovAngleAttack) == false) return;

        if (target.gameObject.TryGetComponent<HealthComponent>(out var component) == false)
        {
            $"Target of name: {target.gameObject.name} is missing a {typeof(HealthComponent)}".Warning(GetType().Name);
            return;
        }

        component.TakeDamage(attackPower);
    }

    IEnumerator WaitForNextAttackRoutine()
    {
        currentWaitTimeForNextAttack = waitTimeForNextAttack;

        while (currentWaitTimeForNextAttack > 0)
        {
            currentWaitTimeForNextAttack -= Time.deltaTime;

            yield return null;
        }

        attaking = false;
    }

    public void EditForce(float power)
    {
        this.attackPower = power;
    }
    public void EditSpeed(float speed)
    {
        this.agent.speed = speed;
    }
}