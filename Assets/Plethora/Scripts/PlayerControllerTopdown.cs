namespace PlethoraV2.Controllers {

    using Boom.Patterns.Broadcasts;
    using Boom.Utility;
    using PlethoraV2.Input;
    using PlethoraV2.Utility;
    using UnityEngine;
    using UnityEngine.AI;
    using Boom;
    using UnityEngine.Events;
    using PlethoraV2.Minigames;

    public class PlayerControllerTopdown : MonoBehaviour
    {

        [SerializeField] float WalkSpeed = 6;
        [SerializeField] float AttackWalkSpeed = 3;
        [SerializeField] float AttackRotSpeed = 5;

        [SerializeField, ShowOnly] Vector3 walkDir;
        [SerializeField, ShowOnly] Vector3 newPosition;
        [SerializeField, ShowOnly] bool isWalking;
        [field: SerializeField] public Transform RotatingPart { get; private set; }
        [SerializeField] Transform asset;

        [SerializeField, ShowOnly] Transform target;
        [SerializeField, ShowOnly] bool isExecutingAction;
        Vector3 nextLookAtDirection;

        BaseMinigameManager.GameState gameState;
        [SerializeField, ShowOnly] bool onSlope;

        int speedAnimHash;
        int actionAnimHash;

        public ConfigurableCharacter ConfigurableCharacter { get; private set; }
        MonoDictionaryEvent animatorEventSystem;

        public UnityEvent OnEndActionHandler { get; private set; } = new();
        public Vector3 FacingDirection { get; private set; } = default;

        private void Awake()
        {
            speedAnimHash = Animator.StringToHash("Speed");
            actionAnimHash = Animator.StringToHash("Action"); //Attack or any other action

            BroadcastState.Register<BaseMinigameManager.MinigameState>(StateChangeHandler, new BroadcastState.BroadcastSetting() { invokeOnRegistration = true });

            if (TryGetComponent<ConfigurableCharacter>(out var configurableCharacter) == false)
            {
                Debug.LogError($"Character doesn't not have compoment of type {nameof(ConfigurableCharacter)}");
                return;
            }

            ConfigurableCharacter = configurableCharacter;
            ConfigurableCharacter.OnSkinChange.AddListener(OnSkinChangeHandler);

            if (asset.TryGetComponent<MonoDictionaryEvent>(out animatorEventSystem) == false)
            {
                Debug.LogError($"Character doesn't not have compoment of type {nameof(MonoDictionaryEvent)}");
                return;
            }
        }

        private void OnSkinChangeHandler(GameObject arg0)
        {
            animatorEventSystem.AddListener("onActionEnd", OnActionEndHandler);
        }

        private void Start()
        {
            InputMaster.Enable();            
        }

        private void OnDestroy()
        {
            animatorEventSystem.RemoveListener("onActionEnd", OnActionEndHandler);

            BroadcastState.Unregister<BaseMinigameManager.MinigameState>(StateChangeHandler);
        }

        private void StateChangeHandler(BaseMinigameManager.MinigameState state)
        {
            gameState = state.gameState;

            if(gameState == BaseMinigameManager.GameState.GameOverFailure) RotatingPart.gameObject.SetActive(false);
        }


        void Update()
        {

            if(asset.gameObject.activeSelf == false) asset.gameObject.SetActive(true);
            if (RotatingPart.gameObject.activeSelf == false) RotatingPart.gameObject.SetActive(true);

            FacingDirection = RotatingPart.forward;

            if ((gameState != BaseMinigameManager.GameState.Playing && MinigameDevelopmentManager.ForceEnablePlayerControl == false) && BaseMinigameManager.MinigameManagerExist)
            {
                ConfigurableCharacter.Animator.SetFloat(speedAnimHash, 0);
                return;
            }

            ConfigurableCharacter.Animator.SetFloat(speedAnimHash, walkDir.sqrMagnitude > 0.01f? 1 : 0);


            WalkHandler();

            if (InputMaster.IsButtonPressed(InputMaster.InputType.Jump) && isExecutingAction == false)
            {

                isExecutingAction = true;
                ConfigurableCharacter.Animator.SetBool(actionAnimHash, true);
            }

            //

            RotatingPartHandler();
        }

        private void RotatingPartHandler()
        {
            if (RotatingPart == null) return;

            var lookAtDir = InputMaster.Movement.XZ3().sqrMagnitude;
            var changeDirection = lookAtDir > 0.01f;

            if (changeDirection)
            {
                if (isExecutingAction)
                {
                    RotatingPart.rotation = RotatingPart.rotation.RotateSlerp(nextLookAtDirection, AttackRotSpeed);
                }
                else
                {
                    RotatingPart.LookAt(transform.position.Y(RotatingPart.position.y) + nextLookAtDirection);

                }
            }

            nextLookAtDirection = InputMaster.Movement.XZ3();
        }

        private void WalkHandler()
        {
            walkDir = InputMaster.Movement.XZ3();
            var walkDirSqrMagnitud = walkDir.sqrMagnitude;

            isWalking = walkDirSqrMagnitud > 0.01f;

            if (isWalking)
            {
                //check if desired position produces a valid movement
                newPosition = transform.position + walkDir * Time.deltaTime * (isExecutingAction == false? WalkSpeed : AttackWalkSpeed);
                NavMeshHit hit;

                bool isValid = NavMesh.SamplePosition(newPosition, out hit, 1f, NavMesh.AllAreas);

                if (isValid)
                {
                    var hitPosition = hit.position;
                    //check if it's enough movement
                    if ((transform.position - hitPosition).magnitude >= .02f)
                    {
                        hit = default;
                        bool isValidSecondHit = NavMesh.SamplePosition(newPosition + RotatingPart.forward * 0.05f, out hit, 1f, NavMesh.AllAreas);

                        onSlope = isValidSecondHit && Mathf.Abs(hitPosition.y - hit.position.y) > 0.01f;

                        if (onSlope)
                        {
                            transform.position = hitPosition;

                        }
                        else
                        {
                            transform.position = hitPosition.Y(transform.position.y);
                        }
                    }
                }
                else { }

            }
        }


        private void OnActionEndHandler()
        {
            isExecutingAction = false;

            ConfigurableCharacter.Animator.SetBool(actionAnimHash, false);

            OnEndActionHandler.Invoke();
        }
    }
}