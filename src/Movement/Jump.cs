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
        private int   airJumpMax;
        private float wallJumpForce;
        private float wallJumpDuration;
        private float wallUpFactor;
        private bool  stayJump;

        private int airJumpCount;
        private bool IsJumping = false;
        //constructor
        public Jump(Physic physic, 
                    S_Input input, 
                    Walk walk,
                    GameObject gameObject,

                    // Constants
                    float jumpForce, 
                    float airJumpForce, 
                    float airJumpDelay, 
                    int   airJumpMax, 
                    float wallJumpForce, 
                    float wallJumpDuration,
                    float wallUpFactor,
                    bool  stayJump)
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

            if (physic.state != Physics.State.InAir) {
                IsJumping = false;
            } else {
                if (IsJumping) {
                    physic.MultiplyHorizontalVelocity(0.997f);
                }
            }

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
            } else if (input.CurrentJumpState == BInput.Up) {
                StopJump();
            }
        }

        private void JumpOnGround()
        {
            IsJumping = true;
            Vector2 velocity = physic.velocity;
            velocity.y = jumpForce;
            velocity.x = physic.velocity.x * 1.7f;
            physic.SetVelocity(velocity);
        }

        private void StopJump()
        {
            float velocity = physic.velocity.y;
            if (velocity > 0f && IsJumping) {
                physic.SetVerticalVelocity(velocity * 0.5f);
                //IsJumping = false;
            }
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
            if (walk.WallJumpFactor != 1f) {
                if (physic.state == Physics.State.OnGround || physic.state == Physics.State.OnWall && walk.WallJumpFactor > 0.5f) {
                    walk.WallJumpFactor = 1f;
                } else {
                    walk.WallJumpFactor += Time.deltaTime / wallJumpDuration;
                    walk.WallJumpFactor = Mathf.Clamp(walk.WallJumpFactor, 0f, 1f);
                }
            }
        }
    }
}
