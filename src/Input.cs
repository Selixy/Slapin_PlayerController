using UnityEngine;
using UnityEngine.InputSystem;

namespace Slapin_CharacterController
{
    public class S_Input
    {
        // Actions récupérées depuis l'asset
        private InputAction jump;
        private InputAction sprint;
        private InputAction dash;
        private InputAction atk;
        private InputAction move;
        private InputAction look;
        private InputAction mouseDelta;


        // Propriété en lecture seule pour le mode gamepad
        public bool IsGamepadMode { get; private set; } = false;

        // États des entrées (chaque bouton est un enum BInput : Down, Pressed, Up, Released)
        public BInput CurrentJumpState { get; private set; } = BInput.Released;
        public BInput CurrentSprintState { get; private set; } = BInput.Released;
        public BInput CurrentDashState { get; private set; } = BInput.Released;
        public BInput CurrentAtkState { get; private set; } = BInput.Released;
        public Vector2 CurrentMoveState { get; private set; } = Vector2.zero;
        public Vector2 CurrentLookState
        {
            get { return IsGamepadMode ? JoystickLookState : MouseLookState; }
        }

        // Références
        private MonoBehaviour coroutineOwner;
        private GameObject Player;

        // Position du joueur
        private Vector2 PlayerPosition
        {
            get
            {
                Vector3 pos = Player.transform.position;
                return new Vector2(pos.x, pos.y);
            }
        }

        // Look state pour le joystick
        private Vector2 JoystickLookState = Vector2.zero;
        // Look state pour la souris (calculé à la volée)
        private Vector2 MouseLookState
        {
            get
            {
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 lookVector = PlayerPosition - new Vector2(mouseWorldPosition.x, mouseWorldPosition.y);
                lookVector.Normalize();
                return lookVector;
            }
        }

        // Constructeur
        public S_Input(MonoBehaviour owner, GameObject Player)
        {
            this.coroutineOwner = owner;
            this.Player = Player;

            // Chargement de l'InputActionAsset depuis Resources
            InputActionAsset A_Input = Resources.Load<InputActionAsset>("InputSystem_Actions");

            // Récupération des actions par leur nom (doivent correspondre aux noms définis dans l'asset)
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
            this.jump.canceled  += OnJumpCanceled;

            // Sprint
            this.sprint.started  += OnSprintStarted;
            this.sprint.canceled  += OnSprintCanceled;

            // Dash
            this.dash.started  += OnDashStarted;
            this.dash.canceled  += OnDashCanceled;

            // Atk
            this.atk.started  += OnAtkStarted;
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

        // Destructeur : désabonnement des événements
        ~S_Input()
        {
            // Jump
            this.jump.started  -= OnJumpStarted;
            this.jump.canceled  -= OnJumpCanceled;

            // Sprint
            this.sprint.started  -= OnSprintStarted;
            this.sprint.canceled  -= OnSprintCanceled;

            // Dash
            this.dash.started  -= OnDashStarted;
            this.dash.canceled  -= OnDashCanceled;

            // Atk
            this.atk.started  -= OnAtkStarted;
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



        // --------------INPUTS CALLBACKS--------------

        // --- JUMP ---
        private void OnJumpStarted(InputAction.CallbackContext context)
        {
            CurrentJumpState = BInput.Down;
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            CurrentJumpState = BInput.Up;
        }

        // --- SPRINT ---
        private void OnSprintStarted(InputAction.CallbackContext context)
        {
            CurrentSprintState = BInput.Down;
        }

        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            CurrentSprintState = BInput.Up;
        }

        // --- DASH ---
        private void OnDashStarted(InputAction.CallbackContext context)
        {
            CurrentDashState = BInput.Down;
        }

        private void OnDashCanceled(InputAction.CallbackContext context)
        {
            CurrentDashState = BInput.Up;
        }

        // --- ATK ---
        private void OnAtkStarted(InputAction.CallbackContext context)
        {
            CurrentAtkState = BInput.Down;
        }
        private void OnAtkCanceled(InputAction.CallbackContext context)
        {
            CurrentAtkState = BInput.Up;
        }


        // --- MOVE ---
        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            CurrentMoveState = context.ReadValue<Vector2>();
        }
        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            CurrentMoveState = Vector2.zero;
        }

        // --- LOOK ---
        private void OnLookStarted(InputAction.CallbackContext context)
        {
            IsGamepadMode = true;
        }
        private void OnLookPerformed(InputAction.CallbackContext context)
        {
            JoystickLookState = context.ReadValue<Vector2>();
        }
        private void OnLookCanceled(InputAction.CallbackContext context)
        {
            JoystickLookState = Vector2.zero;
        }

        // --- MOUSE DELTA ---
        private void OnMouseDeltaStarted(InputAction.CallbackContext context)
        {
            if (IsGamepadMode)
            {
                IsGamepadMode = false;
            }
        }



        // --------------INPUTS UPDATE--------------

        // Méthode Update pour mettre à jour les états des boutons
        public void Update()
        {
            // Réinitialisation des états des boutons
            if (CurrentJumpState == BInput.Up) {
                CurrentJumpState = BInput.Released;
            }
            if (CurrentSprintState == BInput.Up) {
                CurrentSprintState = BInput.Released;
            }
            if (CurrentDashState == BInput.Up)
            {
                CurrentDashState = BInput.Released;
            }
            if (CurrentAtkState == BInput.Up) {
                CurrentAtkState = BInput.Released;
            }


            if (CurrentJumpState == BInput.Down) {
                CurrentJumpState = BInput.Pressed;
            }
            if (CurrentSprintState == BInput.Down) {
                CurrentSprintState = BInput.Pressed;
            }
            if (CurrentDashState == BInput.Down) {
                CurrentDashState = BInput.Pressed;
            }
            if (CurrentAtkState == BInput.Down) {
                CurrentAtkState = BInput.Pressed;
            }

        }
    }
}
