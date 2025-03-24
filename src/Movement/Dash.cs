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
        private float dashCurveExponent;
        private float dashTimeScale;
        private float dashChargeMaxTimeLimit;

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
                    float dashDistanceMax,
                    float dashCurveExponent,
                    float dashTimeScale,
                    float dashChargeMaxTimeLimit)
        {
            this.physic = physic;
            this.input = input;

            this.dashDuration = dashDuration;
            this.dashChargeTimeMin = dashChargeTimeMin;
            this.dashChargeTimeMax = dashChargeTimeMax;
            this.dashDistanceMin = dashDistanceMin;
            this.dashDistanceMax = dashDistanceMax;
            this.dashCurveExponent = dashCurveExponent;
            this.dashTimeScale = dashTimeScale;

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



        public void Update()
        {
            if (dashing == 0f) {
                if (input.CurrentDashState == BInput.Down) {
                    isCharging = true;
                    chargingFactor = 0f;
                    Time.timeScale = dashTimeScale;
                }
                else if (isCharging) {
                    if (input.CurrentDashState == BInput.Pressed) {
                        ChargingDash();
                    }
                    else if (input.CurrentDashState == BInput.Up) {
                        Time.timeScale = 1f;
                        if (chargingFactor >= dashChargeTimeMin) {
                            dashing = dashDuration;
                        }
                    }
                }
            }
            else {
                InDash();
            }
        }

        public void BreakCharge()
        {
            chargingFactor = 0f;
            Time.timeScale = 1f;
            isCharging = false;
            dashing = 0f;
            isDashing = false;
        }

        private void ChargingDash()
        {
            dashDirection = input.CurrentLookState;
            dashDistance = Mathf.Lerp(dashDistanceMin, dashDistanceMax, chargingFactor - dashChargeTimeMin);
            // Si le temps est divisé par 0, on ne charge pas
            if (dashTimeScale != 0f) {
                chargingFactor += Time.deltaTime / dashTimeScale;
            } else {
                Debug.LogWarning("Dash time scale is 0");
            }
            chargingFactor = Mathf.Clamp(chargingFactor, 0f, dashChargeTimeMax);

            // Si la charge dure trop longtemps, on arrête la charge
            if (chargingFactor >= dashChargeMaxTimeLimit + dashChargeTimeMax) {
                BreakCharge();
            }
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
