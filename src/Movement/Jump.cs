using UnityEngine;
using Physics;

namespace Slapin_CharacterController
{
    public class Jump
    {
        private Physic physic;
        private S_Input input;
        private Walk walk;
        private GameObject gameObject;

        private float jumpForce;
        private float airJumpForce;
        private float airJumpDelay;
        private int airJumpMax;
        private float wallJumpForce;
        private float wallJumpDuration;
        private float wallUpFactor;
        private bool stayJump;

        private int airJumpCount;

        //constructor
        public Jump(Physic physic, 
                    S_Input input, 
                    Walk walk,
                    GameObject gameObject,

                    // Constants
                    float jumpForce, 
                    float airJumpForce, 
                    float airJumpDelay, 
                    int airJumpMax, 
                    float wallJumpForce, 
                    float wallJumpDuration,
                    float wallUpFactor,
                    bool stayJump)
        {
            this.physic = physic;
            this.input = input;
            this.walk = walk;
            this.gameObject = gameObject;

            // Constants
            this.jumpForce = jumpForce;
            this.airJumpForce = airJumpForce;
            this.airJumpDelay = airJumpDelay;
            this.airJumpMax = airJumpMax;
            this.wallJumpForce = wallJumpForce;
            this.wallJumpDuration = wallJumpDuration;
            this.wallUpFactor = wallUpFactor;
            this.stayJump = stayJump;
        }

        // logic

        public void Update()
        {
            ResetAirJumpCount();
            ResetWallJumpFactor();

            if (input.CurrentJumpState == BInput.Down) {
                if (physic.state == Physics.State.OnGround) {
                    JumpOnGround();
                } else if (physic.state == Physics.State.OnWall) {
                    JumpOnWall();
                } else {
                    JumpInAir();
                }
            } else if (input.CurrentJumpState == BInput.Pressed && stayJump) {
                if (physic.state == Physics.State.OnGround) {
                    JumpOnGround();
                }
            }
        }

        private void JumpOnGround()
        {
            physic.SetVerticalVelocity(jumpForce);
        }

        private void JumpInAir()
        {
            if (airJumpCount < airJumpMax) {
                physic.SetVerticalVelocity(airJumpForce);
                airJumpCount++;
            }
        }

        public void ResetAirJumpCount()
        {
            if (physic.state == Physics.State.OnGround && airJumpCount != 0) {
                airJumpCount = 0;
            }
        }

        public void AddAirJump()
        {
            if (airJumpCount > 0) {
                airJumpCount--;
            }
        }

        public Vector2 GetNormalWall()
        {
            if (physic.state == Physics.State.OnWall) {
                foreach (Physics.CollisionData collision in physic.collisionBuffer.CollisionList) {
                    if (Mathf.Abs(collision.Normal.x) > 0.5f) {
                        return collision.Normal;
                    }
                    Debug.Log("No wall collision found");
                }
            }
            Debug.Log("Is not on wall");
            return Vector2.zero;
        }

        private void JumpOnWall()
        {
            Vector2 vectorJump = Vector2.Lerp(GetNormalWall(), Vector2.up, wallUpFactor);
            physic.SetVelocity(vectorJump * wallJumpForce);
            walk.WallJumpFactor = 0f;
        }

        public void ResetWallJumpFactor()
        {
            if (walk.WallJumpFactor < 1f) {
                walk.WallJumpFactor += Time.deltaTime / wallJumpDuration;
                walk.WallJumpFactor = Mathf.Clamp(walk.WallJumpFactor, 0f, 1f);
            }
        }

    }
}
