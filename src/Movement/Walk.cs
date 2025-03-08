using UnityEngine;
using Physics;

namespace Slapin_CharacterController
{
    public class Walk
    {
        private Physic physic;
        private S_Input input;
        private float sprintAcceleration;
        private float sprintMaxSpeed;
        private float walkAcceleration;
        private float walkMaxSpeed;



        public Walk(Physic physic,
                    S_Input input,

                    // Constants
                    float walkAcceleration,
                    float walkMaxSpeed,
                    float sprintAcceleration,
                    float sprintMaxSpeed)
        {
            this.physic = physic;
            this.input = input;

            // Constants
            this.sprintAcceleration = sprintAcceleration;
            this.sprintMaxSpeed = sprintMaxSpeed;
            this.walkAcceleration = walkAcceleration;
            this.walkMaxSpeed = walkMaxSpeed;
        }


        public void Update()
        {
            if (input.CurrentMoveState != Vector2.zero)
            {
                if (input.CurrentSprintState == BInput.Pressed) {
                    Run(input.CurrentMoveState, sprintAcceleration, sprintMaxSpeed);
                }
                else {
                    Run(input.CurrentMoveState, walkAcceleration, walkMaxSpeed);
                }
            }
        }

        public void Run(Vector2 directionAxis, float acceleration, float maxSpeed)
        {
            float directionAmplitude = directionAxis.magnitude;
            if (directionAmplitude == 0) {
                return;
            }
            float direction = Mathf.Sign(directionAxis.x);

            if (Mathf.Abs(physic.velocity.x) <= maxSpeed || Mathf.Sign(physic.velocity.x) != direction) {
                float velocity = direction * acceleration * directionAmplitude;
                velocity = Mathf.Clamp(velocity, -maxSpeed, maxSpeed);
                physic.AddVelocity(new Vector2(velocity, 0));
                Debug.Log("Velocity: " + velocity);
            }
        }
    }
}
