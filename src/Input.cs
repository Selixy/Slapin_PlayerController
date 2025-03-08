using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Slapin_CharacterController
{
    public class S_Input
    {
        // Actions directement récupérées depuis l'asset
        private InputAction jump;
        private InputAction sprint;
        private InputAction dash;
        private InputAction atk;
        private InputAction move;
        private InputAction look;
        private InputAction mouseDelta;


        // Propriétés en lecture seule pour connaître l'état actuel des entrées
        public bool IsGamepadMode { get; private set; } = false;
        // États des entrées
        public BInput CurrentJumpState { get; private set; } = BInput.Released;
        public BInput CurrentSprintState { get; private set; } = BInput.Released;
        public BInput CurrentDashState { get; private set; } = BInput.Released;
        public BInput CurrentAtkState { get; private set; } = BInput.Released;
        public Vector2 CurrentMoveState { get; private set; } = Vector2.zero;
        public Vector2 CurrentLookState {
            get {
                if (IsGamepadMode) {
                    return JoystickLookState;
                } else {
                    return MouseLookState;
                }
            }
        }

        // references
        private MonoBehaviour coroutineOwner;
        private GameObject Player;

        // Position du joueur
        private Vector2 PlayerPosition {
            get {
                Vector3 pos = Player.transform.position;
                return new Vector2(pos.x, pos.y);
            }
        }

        // Look state
        private Vector2 JoystickLookState = Vector2.zero;
        private Vector2 MouseLookState {
            get {
                // Conversion de la position de la souris en coordonnées monde
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                // Calcul du vecteur de regard
                Vector2 lookVector = PlayerPosition - new Vector2(mouseWorldPosition.x, mouseWorldPosition.y);
                // Normalisation du vecteur de regard
                lookVector.Normalize();
                return lookVector;
            }
        }


        // Constructeur
        public S_Input(MonoBehaviour owner, GameObject Player)
        {
            this.coroutineOwner = owner;
            this.Player = Player;

            // Chargement direct de l'InputActionAsset depuis le dossier Resources
            InputActionAsset A_Input = Resources.Load<InputActionAsset>("InputSystem_Actions");

            // Récupération des actions par leur nom (assurez-vous que ces noms correspondent à ceux définis dans l'asset)
            this.jump = A_Input.FindAction("Player/Jump");
            this.sprint = A_Input.FindAction("Player/Sprint");
            this.dash = A_Input.FindAction("Player/Dash");
            this.atk = A_Input.FindAction("Player/Atk");
            this.move = A_Input.FindAction("Player/Move");
            this.look = A_Input.FindAction("Player/Look");
            this.mouseDelta = A_Input.FindAction("Player/MouseDelta");

            // Abonnement aux événements
            // Jump
            this.jump.started  += OnJumpStarted;
            this.jump.performed += OnJumpPerformed;
            this.jump.canceled  += OnJumpCanceled;

            // Sprint
            this.sprint.started  += OnSprintStarted;
            this.sprint.performed += OnSprintPerformed;
            this.sprint.canceled  += OnSprintCanceled;

            // Dash
            this.dash.started  += OnDashStarted;
            this.dash.performed += OnDashPerformed;
            this.dash.canceled  += OnDashCanceled;

            // Atk
            this.atk.started  += OnAtkStarted;
            this.atk.performed += OnAtkPerformed;
            this.atk.canceled  += OnAtkCanceled;

            // Move
            this.move.performed += OnMovePerformed;
            this.move.canceled  += OnMoveCanceled;

            // Look
            this.look.started += OnLookStarted;
            this.look.performed += OnLookPerformed;
            this.look.canceled  += OnLookCanceled;
            this.mouseDelta.started += OnMouseDeltaStarted;
        }

        // Destructor
        ~S_Input()
        {
            // Désabonnement des événements
            // Jump
            this.jump.started  -= OnJumpStarted;
            this.jump.performed -= OnJumpPerformed;
            this.jump.canceled  -= OnJumpCanceled;

            // Sprint
            this.sprint.started  -= OnSprintStarted;
            this.sprint.performed -= OnSprintPerformed;
            this.sprint.canceled  -= OnSprintCanceled;

            // Dash
            this.dash.started  -= OnDashStarted;
            this.dash.performed -= OnDashPerformed;
            this.dash.canceled  -= OnDashCanceled;

            // Atk
            this.atk.started  -= OnAtkStarted;
            this.atk.performed -= OnAtkPerformed;
            this.atk.canceled  -= OnAtkCanceled;

            // Move
            this.move.performed -= OnMovePerformed;
            this.move.canceled  -= OnMoveCanceled;

            // Look
            this.look.started -= OnLookStarted;
            this.look.performed -= OnLookPerformed;
            this.look.canceled  -= OnLookCanceled;
            this.mouseDelta.started -= OnMouseDeltaStarted;
        }


        // Jump
        // Méthode appelée lorsque le bouton de saut est initialement pressé
        private void OnJumpStarted(InputAction.CallbackContext context)
        {
            CurrentJumpState = BInput.Down;
            Debug.Log("Jump started: " + CurrentJumpState);
        }

        // Méthode appelée lorsque le bouton de saut est maintenu
        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            CurrentJumpState = BInput.Pressed;
            Debug.Log("Jump performed: " + CurrentJumpState);
        }

        // Méthode appelée lorsque le bouton de saut est relâché
        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            CurrentJumpState = BInput.Up;
            Debug.Log("Jump canceled: " + CurrentJumpState);
            // Démarrer la coroutine pour réinitialiser l'état la frame suivante
            coroutineOwner.StartCoroutine(ResetJumpImputCoroutine());
        }

        // Coroutine qui réinitialise l'état de saut après une frame
        private IEnumerator ResetJumpImputCoroutine()
        {
            yield return null;
            CurrentJumpState = BInput.Released;
            Debug.Log("Jump state reset to Released");
        }



        // Sprint
        // Méthode appelée lorsque le bouton de sprint est initialement pressé
        private void OnSprintStarted(InputAction.CallbackContext context)
        {
            CurrentSprintState = BInput.Down;
            Debug.Log("Sprint started: " + CurrentSprintState);
        }

        // Méthode appelée lorsque le bouton de sprint est maintenu
        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            CurrentSprintState = BInput.Pressed;
            Debug.Log("Sprint performed: " + CurrentSprintState);
        }

        // Méthode appelée lorsque le bouton de sprint est relâché
        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            CurrentSprintState = BInput.Up;
            Debug.Log("Sprint canceled: " + CurrentSprintState);
            // Démarrer la coroutine pour réinitialiser l'état la frame suivante
            coroutineOwner.StartCoroutine(ResetSprintImputCoroutine());
        }

        // Coroutine qui réinitialise l'état de sprint après une frame
        private IEnumerator ResetSprintImputCoroutine()
        {
            yield return null;
            CurrentSprintState = BInput.Released;
            Debug.Log("Sprint state reset to Released");
        }



        // Dash
        // Méthode appelée lorsque le bouton de dash est initialement pressé
        private void OnDashStarted(InputAction.CallbackContext context)
        {
            CurrentDashState = BInput.Down;
            Debug.Log("Dash started: " + CurrentDashState);
        }

        // Méthode appelée lorsque le bouton de dash est maintenu
        private void OnDashPerformed(InputAction.CallbackContext context)
        {
            CurrentDashState = BInput.Pressed;
            Debug.Log("Dash performed: " + CurrentDashState);
        }

        // Méthode appelée lorsque le bouton de dash est relâché
        private void OnDashCanceled(InputAction.CallbackContext context)
        {
            CurrentDashState = BInput.Up;
            Debug.Log("Dash canceled: " + CurrentDashState);
            // Démarrer la coroutine pour réinitialiser l'état la frame suivante
            coroutineOwner.StartCoroutine(ResetDashImputCoroutine());
        }

        // Coroutine qui réinitialise l'état de dash après une frame
        private IEnumerator ResetDashImputCoroutine()
        {
            yield return null;
            CurrentDashState = BInput.Released;
            Debug.Log("Dash state reset to Released");
        }

        // Atk
        // Méthode appelée lorsque le bouton d'attaque est initialement pressé
        private void OnAtkStarted(InputAction.CallbackContext context)
        {
            CurrentAtkState = BInput.Down;
            Debug.Log("Atk started: " + CurrentAtkState);
        }

        // Méthode appelée lorsque le bouton d'attaque est maintenu
        private void OnAtkPerformed(InputAction.CallbackContext context)
        {
            CurrentAtkState = BInput.Pressed;
            Debug.Log("Atk performed: " + CurrentAtkState);
        }

        // Méthode appelée lorsque le bouton d'attaque est relâché
        private void OnAtkCanceled(InputAction.CallbackContext context)
        {
            CurrentAtkState = BInput.Up;
            Debug.Log("Atk canceled: " + CurrentAtkState);
            // Démarrer la coroutine pour réinitialiser l'état la frame suivante
            coroutineOwner.StartCoroutine(ResetAtkImputCoroutine());
        }

        // Coroutine qui réinitialise l'état d'attaque après une frame
        private IEnumerator ResetAtkImputCoroutine()
        {
            yield return null;
            CurrentAtkState = BInput.Released;
            Debug.Log("Atk state reset to Released");
        }



        // Move
        // Méthode appelée lorsque le déplacement est effectué
        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            CurrentMoveState = context.ReadValue<Vector2>();
            Debug.Log("Move performed: " + CurrentMoveState);
        }

        // Méthode appelée lorsque le déplacement est annulé (relâché)
        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            CurrentMoveState = Vector2.zero;
            Debug.Log("Move canceled: " + CurrentMoveState);
        }


        // Look
        // Méthode appelée lorsque le déplacement du joystick (R) est initialement pressé
        private void OnLookStarted(InputAction.CallbackContext context)
        {
            IsGamepadMode = true;
            Debug.Log("Look started: Gamepad mode activated");
        }

        // Méthode appelée lorsque le déplacement du joystick (R) est effectué
        private void OnLookPerformed(InputAction.CallbackContext context)
        {
            JoystickLookState = context.ReadValue<Vector2>();
            Debug.Log("Look performed: " + JoystickLookState);
        }

        // Méthode appelée lorsque le déplacement du joystick (R) est annulé (relâché)
        private void OnLookCanceled(InputAction.CallbackContext context)
        {
            JoystickLookState = Vector2.zero;
            Debug.Log("Look canceled: " + JoystickLookState);
        }

        // MouseDelta
        // Méthode appelée lorsque la souris commence à se déplacer
        private void OnMouseDeltaStarted(InputAction.CallbackContext context)
        {
            if (IsGamepadMode) {
                IsGamepadMode = false;
                Debug.Log("MouseDelta started: Mouse mode activated");
            }
        }
    }
}