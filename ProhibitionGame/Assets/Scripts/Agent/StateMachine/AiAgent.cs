using UnityEngine;
using UnityEngine.AI;
using Hearing;

//Head of the enemyAI
public class AiAgent : MonoBehaviour, IHear
{
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public AiStateMachine stateMachine;
    [HideInInspector] public Transform playerTransform;
    [HideInInspector] public Animator animator;
    [HideInInspector] public PlayerHealth playerHealth;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public PlayerFootStepSystem playerFootStepSystem;
    [HideInInspector] public PlayerCamera playerCamera;
    [HideInInspector] public Camera mainCamera;

    [HideInInspector] public bool isListening;

    public AiStateId initialState;

    [Space(15f)]
    public LayerMask groundLayers;
    public AiAgentConfig config;
    public Transform lookPoint;

    [Header("Sensors")]
    public AiSensor mainSensor;
    public AiSensor backSensor;
    public AiSensor attackSensor;

    [SerializeField] private bool enableHearing = true;


    protected virtual void Start()
    {
        mainCamera = Camera.main;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = playerTransform.gameObject.GetComponent<PlayerHealth>();
        playerMovement = playerTransform.gameObject.GetComponent<PlayerMovement>();
        playerCamera = playerTransform.gameObject.GetComponentInChildren<PlayerCamera>();
        playerFootStepSystem = playerTransform.gameObject.GetComponentInChildren<PlayerFootStepSystem>();

        stateMachine = new AiStateMachine(this);
        stateMachine.RegisterState(new AiChasePlayerState());
        stateMachine.RegisterState(new AiPatrolState());
        stateMachine.RegisterState(new AiAttackState());
        stateMachine.RegisterState(new AiLaughingState());
        stateMachine.RegisterState(new AiStunned());
        stateMachine.RegisterState(new AiAFK());
        stateMachine.ChangeState(initialState);
    }

    private void Update()
    {
        stateMachine.Update();
        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude); //Set enemy animator speed accordingly to navmesh speed
    }

    public void RespondToSound(Sound sound) // Method that implement functional of IHear interface, recognize sounds and do stuff with it depending on which type of sound agent have heard
    {
        if (isListening && enableHearing)
        {
            if(sound.soundType == Sound.SoundType.Chasing)
            {
                navMeshAgent.SetDestination(sound.pos);
            }
            else if(sound.soundType == Sound.SoundType.Interesting)
            {
                navMeshAgent.SetDestination(sound.pos);
            }
        }
    }
}