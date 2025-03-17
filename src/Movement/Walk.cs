using UnityEngine;
using Physics;

namespace Slapin_CharacterController
{
    public class Walk
    {
        // Variables
        private Physic physic;
        private S_Input input;
        private float sprintAcceleration;
        private float sprintMaxSpeed;
        private float walkAcceleration;
        private float walkMaxSpeed;

        // Variables Statiques
        private static float inertiaFactor = 0.1f;
        private static float inertiaFactorAir = 0.4f;
        private static float airAdherence = 0.5f;

        // Variables accessibles
        public float WallJumpFactor = 1f;

        // Constructeur
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

        // Update
        public void Update()
        {
            if (input.CurrentMoveState != Vector2.zero) {
                if (input.CurrentSprintState == BInput.Pressed) {
                    Run(input.CurrentMoveState, sprintAcceleration, sprintMaxSpeed);
                }
                else {
                    Run(input.CurrentMoveState, walkAcceleration, walkMaxSpeed);
                }
            }
        }

        // Fonction qui retourne l'adhérence en fonction de l'état du joueur
        private float GetAderence()
        {
            // Si le joueur est sur le sol, on calcule l'adhérence
            if (physic.state == Physics.State.OnGround) {
                float Aderence = 0f;
                foreach (Physics.CollisionData collision in physic.collisionBuffer.CollisionList) {
                    if (collision.Normal.y > 0) {
                        Aderence += collision.Adherence;
                    }
                }
                return Aderence;
            } else {
                return airAdherence;
            }
        }

        private float GetInertiaFactor()
        {
            if (physic.state == Physics.State.InAir) {
                return inertiaFactorAir;
            } else {
                return inertiaFactor;
            }
        }

        // Fonction qui calcule la vitesse
        private void Run(Vector2 vectorAxis, float acceleration, float maxSpeed)
        {
            // Si le joueur ne bouge pas ou si la vitesse est supérieure à la vitesse maximale, on ne fait rien
            if (vectorAxis.x == 0 ||
                (Mathf.Abs(physic.velocity.x) > maxSpeed &&
                Mathf.Sign(physic.velocity.x) == Mathf.Sign(vectorAxis.x) &&
                physic.velocity.x != 0))
            {
                return;
            }

            // On initialise la vitesse, la direction et son amplitude
            float directionAmplitude = vectorAxis.magnitude * WallJumpFactor;
            float direction = Mathf.Sign(vectorAxis.x);

            // On calcule l'accélération en fonction de l'état du joueur
            if (Mathf.Sign(physic.velocity.x) != direction) {
                acceleration *= GetInertiaFactor() * GetAderence();
            } else {
                acceleration *= GetAderence();
            }


            // Calcul de l'accélération à ajouter en fonction de la direction et de l'accélération
            float addVelocity = direction * acceleration * directionAmplitude * Time.deltaTime * 50f;

            // On calcule la nouvelle vitesse potentielle
            float currentVelocity = physic.velocity.x;
            float newVelocity = currentVelocity + addVelocity;

            // Si la nouvelle vitesse dépasse la vitesse maximale, on la plafonne
            if (Mathf.Abs(newVelocity) > maxSpeed) {
                newVelocity = maxSpeed * Mathf.Sign(newVelocity);
                addVelocity = newVelocity - currentVelocity;
            }

            // On applique la vitesse calculée
            physic.AddVelocity(new Vector2(addVelocity, 0));
        }
    }
}
