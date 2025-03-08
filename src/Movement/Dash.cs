using Physics;

namespace Slapin_CharacterController
{
    public class Dash
    {
        private Physic physic;
        private S_Input input;
        private float dashSpeed;
        private float dashDistance;
        public Dash(Physic physic, 
                    S_Input input, 

                    // Constants
                    float dashSpeed, 
                    float dashDistance)
        {
            this.physic = physic;
            this.input = input;

            // Constants
            this.dashSpeed = dashSpeed;
            this.dashDistance = dashDistance;
        }

        public void Update()
        {

        }

    }
}
