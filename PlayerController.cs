using UnityEngine;
using UnityEngine.InputSystem;
using Physics;

namespace Slapin_CharacterController
{
    public class CharacterController : MonoBehaviour
    {
        private S_Input input;
        private Physic physic;
        private Walk walk;
        private Jump jump;
        private Dash dash;

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
        [SerializeField] private float wallJumpDuration = 0.5f;

        [Header("Dash Settings")]
        [SerializeField] private float dashSpeed = 10f;
        [SerializeField] private float dashDistance = 10f;



        private void Awake()
        {
            // Création des instances
            // instances de base
            physic = new Physic(gameObject);
            input = new S_Input(this, gameObject);

            // instances de mouvement
            walk = new Walk(physic,
                            input,

                            walkAcceleration,
                            walkMaxSpeed,
                            sprintAcceleration,
                            sprintMaxSpeed);

            // instances de saut
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

            // instances de dash
            dash = new Dash(physic,
                            input,
                            dashSpeed,
                            dashDistance);
        }

        void Start()
        {
        }

        void Update()
        {
            // Run
            walk.Update();
            // Jump
            jump.Update();
            // Dash
            dash.Update();

            // Update
            physic.Update();
            input.Update();
        }
    }
}
