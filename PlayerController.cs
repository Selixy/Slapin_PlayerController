using UnityEngine;
using UnityEngine.InputSystem;
using Physics;

namespace Slapin_CharacterController
{
    public class Sl_CharacterController : MonoBehaviour
    {
        private S_Input input;
        private Physic physic;
        private Slapin_States states;
        private Walk walk;
        private Jump jump;
        private Dash dash;
        
        public Vector2 SpawnPosition;
        public Vector2 velocity { get{ return physic.velocity; } }

        [Header("Movement Settings")]
        [SerializeField] private float walkAcceleration = 3f;
        [SerializeField] private float walkMaxSpeed = 4f;
        [SerializeField] private float sprintAcceleration = 4f;
        [SerializeField] private float sprintMaxSpeed = 8f;

        [Header("Jump Settings")]
        [SerializeField] private bool  stayJump = false;
        [SerializeField] private float jumpForce = 22f;
        [SerializeField] private float airJumpForce = 18f;
        [SerializeField] private float airJumpDelay = 0.5f;
        [SerializeField] private int   airJumpMax = 1;
        [SerializeField] private float wallJumpForce = 30f;
        [SerializeField] private float wallUpFactor = 0.65f;
        [SerializeField] private float wallJumpDuration = 0.8f;

        [Header("Dash Settings")]
        [SerializeField] private float dashDuration = 10f;
        [SerializeField] private float dashChargeTimeMin = 0.1f;
        [SerializeField] private float dashChargeTimeMax = 0.4f;
        [SerializeField] private float dashChargeMaxTimeLimit = 0.2f;
        [SerializeField] private float dashDistanceMin = 10f;
        [SerializeField] private float dashDistanceMax = 20f;
        [SerializeField] private float dashCurveExponent = 7f;
        [SerializeField] private float dashTimeScale = 0.05f;

        private void Awake()
        {
            // Création des instances de base
            physic = new Physic(gameObject, 0.0001f);
            input = new S_Input(this, gameObject);

            SpawnPosition = transform.position;
            states = gameObject.GetComponent<Slapin_States>();

            // Instances de mouvement
            walk = new Walk(physic,
                            input,
                            walkAcceleration,
                            walkMaxSpeed,
                            sprintAcceleration,
                            sprintMaxSpeed);

            // Instances de saut
            jump = new Jump(physic,
                            input,
                            walk,
                            gameObject,
                            jumpForce,
                            airJumpForce,
                            airJumpDelay,
                            airJumpMax,
                            wallJumpForce,
                            wallJumpDuration,
                            wallUpFactor,
                            stayJump);

            // Instances de dash
            dash = new Dash(physic,
                            input,
                            dashDuration,
                            dashChargeTimeMin,
                            dashChargeTimeMax,
                            dashDistanceMin,
                            dashDistanceMax,
                            dashCurveExponent,
                            dashTimeScale,
                            dashChargeMaxTimeLimit);
        }

        // Abonnement aux événements
        void OnEnable()
        {
            Slapin_States.OnDeath += OnDeath;
        }

        void OnDisable()
        {
            Slapin_States.OnDeath -= OnDeath;
        }

        void Update()
        {
            if (!states.isDead && !states.isStunned)
            {
                walk.Update();
                jump.Update();
                dash.Update();
                input.Update();
            }
            physic.Update();
        }

        void OnDeath(GameObject obj, bool isDead)
        {
            if (obj == this.gameObject)
            {
                transform.position = new Vector3(SpawnPosition.x, SpawnPosition.y, transform.position.z);
                states.Resurrect(0);
            }
        }
    }
}
