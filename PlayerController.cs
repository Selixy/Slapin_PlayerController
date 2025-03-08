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
        [SerializeField] private float walkAcceleration = 10f;
        [SerializeField] private float walkMaxSpeed = 5f;
        [SerializeField] private float sprintAcceleration = 20f;
        [SerializeField] private float sprintMaxSpeed = 10f;

        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private float airJumpForce = 5f;
        [SerializeField] private float airJumpDelay = 0.5f;
        [SerializeField] private float airJumpMax = 2f;
        [SerializeField] private float wallJumpForce = 10f;
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
                            jumpForce,
                            airJumpForce,
                            airJumpDelay,
                            airJumpMax,
                            wallJumpForce,
                            wallJumpDuration);

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
        }
    }
}
