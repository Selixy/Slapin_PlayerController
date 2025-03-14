using UnityEngine;
using Physics;

namespace Slapin_CharacterController
{
    public class Jump
    {
        private Physic physic;
        private S_Input input;

        private float jumpForce;
        private float airJumpForce;
        private float airJumpDelay;
        private float airJumpMax;
        private float wallJumpForce;
        private float wallJumpDuration;

        //constructor
        public Jump(Physic physic, 
                    S_Input input, 

                    // Constants
                    float jumpForce, 
                    float airJumpForce, 
                    float airJumpDelay, 
                    float airJumpMax, 
                    float wallJumpForce, 
                    float wallJumpDuration)
        {
            this.physic = physic;
            this.input = input;

            // Constants
            this.jumpForce = jumpForce;
            this.airJumpForce = airJumpForce;
            this.airJumpDelay = airJumpDelay;
            this.airJumpMax = airJumpMax;
            this.wallJumpForce = wallJumpForce;
            this.wallJumpDuration = wallJumpDuration;
        }

        // logic

        public void Update()
        {

            if (input.CurrentJumpState == BInput.Down)
            {
                if (physic.state == Physics.State.OnGround) {
                    physic.AddVelocity(new Vector2(0, jumpForce));
                }
            }
        }

    }
}
