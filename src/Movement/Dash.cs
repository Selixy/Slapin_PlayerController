using UnityEngine;
using Physics;

namespace Slapin_CharacterController
{
    public class Dash
    {
        // Références
        private Physic physic;
        private S_Input input;

        // Constants
        private float dashDuration;
        private float dashChargeTimeMin;
        private float dashChargeTimeMax;
        private float dashDistanceMin;
        private float dashDistanceMax;
        private static float dashCurveExponent = 8f;

        // Variables internes
        private float dashing = 0f;
        private bool isDashing = false;
        private bool isCharging = false;
        private float chargingFactor = 0f;
        private Vector2 dashDirection;
        private float dashDistance;

        // Constructeur
        public Dash(Physic physic,
                    S_Input input,
                    float dashDuration,
                    float dashChargeTimeMin,
                    float dashChargeTimeMax,
                    float dashDistanceMin,
                    float dashDistanceMax)
        {
            this.physic = physic;
            this.input = input;

            this.dashDuration = dashDuration;
            this.dashChargeTimeMin = dashChargeTimeMin;
            this.dashChargeTimeMax = dashChargeTimeMax;
            this.dashDistanceMin = dashDistanceMin;
            this.dashDistanceMax = dashDistanceMax;

            // S'abonner à l'événement bonk de Move
            Move.Impact += HandleImpactMessage;
        }

        // Méthode de gestion du message "bonk"
        private void HandleImpactMessage(GameObject gameObject, Vector2 velocity, RaycastHit2D hit)
        {
            if (gameObject == physic.gameObject) {
                if (isDashing) {
                    dashDirection = velocity.normalized;
                }
            }
        }

        private Vector2 GetDashDirectionInput()
        {
            return;
        }

        public void Update()
        {
            if (dashing == 0f)
            {
                if (input.CurrentDashState == BInput.Down)
                {
                    isCharging = true;
                    chargingFactor = 0f;
                }
                else if (isCharging)
                {
                    StartDash();
                    if (input.CurrentDashState == BInput.Up && chargingFactor >= dashChargeTimeMin) {
                        dashing = dashDuration;
                    }
                }
            }
            else
            {
                InDash();
            }
        }

        private void StartDash()
        {
            dashDirection = input.CurrentLookState;
            dashDistance = Mathf.Lerp(dashDistanceMin, dashDistanceMax, chargingFactor - dashChargeTimeMin);
            chargingFactor += Time.deltaTime;
            chargingFactor = Mathf.Clamp(chargingFactor, 0f, dashChargeTimeMax);
        }

        private void InDash()
        {
            if (dashDuration == 0f) {
                Debug.LogWarning("Dash duration is 0");
                return;
            }

            float elapsed = dashDuration - dashing;
            float f = elapsed / dashDuration;

            float instantaneousVelocityMagnitude = dashDistance * dashCurveExponent * Mathf.Pow(1f - f, dashCurveExponent - 1) / dashDuration;
            Vector2 velocity = dashDirection * instantaneousVelocityMagnitude;
            physic.SetVelocity(velocity);

            dashing -= Time.deltaTime;
            isDashing = dashing > 0f;
            if (dashing <= 0f) {
                EndDash();
            }
        }

        private void EndDash()
        {
            dashing = 0f;
            chargingFactor = 0f;
            dashDirection = Vector2.zero;
        }
    }
}
