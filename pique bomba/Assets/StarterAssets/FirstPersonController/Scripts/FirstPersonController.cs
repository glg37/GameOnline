using UnityEngine;
using Fusion;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : NetworkBehaviour
    {
        [Header("Player")]
        public float MoveSpeed = 4.0f;
        public float SprintSpeed = 6.0f;
        public float RotationSpeed = 1.0f;
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;

        [Space(10)]
        public float JumpTimeout = 0.1f;
        public float FallTimeout = 0.15f;

        [Header("Grounded")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.5f;
        public LayerMask GroundLayers;

        [Header("Camera")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 90.0f;
        public float BottomClamp = -90.0f;

        // Cinemachine
        private float _cinemachineTargetPitch;

        // Player
        private float _speed;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // Timeout
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // Components
        private CharacterController _controller;
        private StarterAssetsInputs _input;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif

        private Camera _mainCamera;

        private const float _threshold = 0.01f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        // =========================================================
        // SPAWN
        // =========================================================

        public override void Spawned()
        {
            // pega câmera principal
            _mainCamera = Camera.main;

            // se NÃO for o dono do player
            if (!Object.HasInputAuthority)
            {
                // desativa câmera
                if (_mainCamera != null)
                {
                    _mainCamera.gameObject.SetActive(false);
                }

                // desativa script
                enabled = false;

                return;
            }

            // trava cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // =========================================================
        // START
        // =========================================================

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();

#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#endif

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        // =========================================================
        // NETWORK UPDATE
        // =========================================================

        public override void FixedUpdateNetwork()
        {
            // somente dono controla
            if (!Object.HasInputAuthority)
                return;

            GroundedCheck();
            JumpAndGravity();
            Move();
            CameraRotation();
        }

        // =========================================================
        // GROUND CHECK
        // =========================================================

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(
                transform.position.x,
                transform.position.y - GroundedOffset,
                transform.position.z
            );

            Grounded = Physics.CheckSphere(
                spherePosition,
                GroundedRadius,
                GroundLayers,
                QueryTriggerInteraction.Ignore
            );
        }

        // =========================================================
        // CAMERA
        // =========================================================

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold)
            {
                float deltaTimeMultiplier =
                    IsCurrentDeviceMouse ? 1.0f : Runner.DeltaTime;

                // vertical
                _cinemachineTargetPitch +=
                    _input.look.y * RotationSpeed * deltaTimeMultiplier;

                _cinemachineTargetPitch =
                    ClampAngle(_cinemachineTargetPitch,
                    BottomClamp,
                    TopClamp);

                CinemachineCameraTarget.transform.localRotation =
                    Quaternion.Euler(
                        _cinemachineTargetPitch,
                        0.0f,
                        0.0f
                    );

                // horizontal
                transform.Rotate(
                    Vector3.up *
                    (_input.look.x * RotationSpeed * deltaTimeMultiplier)
                );
            }
        }

        // =========================================================
        // MOVEMENT
        // =========================================================

        private void Move()
        {
            float targetSpeed =
                _input.sprint ? SprintSpeed : MoveSpeed;

            if (_input.move == Vector2.zero)
                targetSpeed = 0.0f;

            float currentHorizontalSpeed =
                new Vector3(
                    _controller.velocity.x,
                    0.0f,
                    _controller.velocity.z
                ).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude =
                _input.analogMovement ? _input.move.magnitude : 1f;

            // aceleração
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(
                    currentHorizontalSpeed,
                    targetSpeed * inputMagnitude,
                    Runner.DeltaTime * SpeedChangeRate
                );

                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // direção
            Vector3 inputDirection =
                new Vector3(
                    _input.move.x,
                    0.0f,
                    _input.move.y
                ).normalized;

            if (_input.move != Vector2.zero)
            {
                inputDirection =
                    transform.right * _input.move.x +
                    transform.forward * _input.move.y;
            }

            // movimento final
            _controller.Move(
                inputDirection.normalized *
                (_speed * Runner.DeltaTime) +

                new Vector3(
                    0.0f,
                    _verticalVelocity,
                    0.0f
                ) * Runner.DeltaTime
            );
        }

        // =========================================================
        // JUMP + GRAVITY
        // =========================================================

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // pulo
                if (_input.jump &&
                    _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity =
                        Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                // cooldown pulo
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Runner.DeltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Runner.DeltaTime;
                }

                _input.jump = false;
            }

            // gravidade
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Runner.DeltaTime;
            }
        }

        // =========================================================
        // CLAMP ANGLE
        // =========================================================

        private static float ClampAngle(
            float angle,
            float min,
            float max)
        {
            if (angle < -360f)
                angle += 360f;

            if (angle > 360f)
                angle -= 360f;

            return Mathf.Clamp(angle, min, max);
        }

        // =========================================================
        // GIZMOS
        // =========================================================

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen =
                new Color(0.0f, 1.0f, 0.0f, 0.35f);

            Color transparentRed =
                new Color(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color =
                Grounded ? transparentGreen : transparentRed;

            Gizmos.DrawSphere(
                new Vector3(
                    transform.position.x,
                    transform.position.y - GroundedOffset,
                    transform.position.z
                ),
                GroundedRadius
            );
        }
    }
}