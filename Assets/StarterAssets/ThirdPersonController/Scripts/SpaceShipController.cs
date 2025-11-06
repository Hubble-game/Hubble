using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class SpaceshipController : MonoBehaviour
    {
        [Header("Spaceship Settings")]
        public float MoveSpeed = 10f;
        public float RotationSpeed = 90f; // degrés/seconde
        public float Acceleration = 5f;
        public float Fuel = 100f;
        public float FuelConsumptionRate = 5f; // unité par seconde quand on pousse

        [Header("FX Settings")]
        public ParticleSystem MainThrusterFX;
        public ParticleSystem LeftThrusterFX;
        public ParticleSystem RightThrusterFX;
        public float MaxFXIntensity = 5f;
    // Sync settings for lateral thruster FX
    [Tooltip("How much turning affects lateral thruster intensity (0 = no turn influence, 1 = full)")]
    public float TurnFXWeight = 0.5f;
    [Tooltip("Adds a small shared pulse to both thrusters so their visual timing stays in sync")]
    public float PulseSpeed = 6f;
    [Tooltip("Pulse amplitude (0 = no pulse). Applied multiplicatively to final intensity.")]
    public float PulseAmount = 0.12f;
    [Header("Manual Particle Sync")]
    [Tooltip("When enabled, forces both lateral ParticleSystems to the same internal simulation time each frame.")]
    public bool ManualParticleSync = false;
    [Tooltip("Duration of the particle cycle in seconds used to compute simulation time (simTime = Time.time % ParticleCycle)")]
    public float ParticleCycle = 1f;

        private float _currentSpeed = 0f;
        private CharacterController _controller;
        private StarterAssetsInputs _input;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#endif

            // Ensure thruster FX aren't playing at startup and use deterministic seeds
            if (LeftThrusterFX)
            {
                LeftThrusterFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                LeftThrusterFX.useAutoRandomSeed = false;
                var e = LeftThrusterFX.emission; e.rateOverTime = 0f;
                LeftThrusterFX.randomSeed = 123456u;
            }

            if (RightThrusterFX)
            {
                RightThrusterFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                RightThrusterFX.useAutoRandomSeed = false;
                var e2 = RightThrusterFX.emission; e2.rateOverTime = 0f;
                RightThrusterFX.randomSeed = 123456u;
            }
        }

        private void Update()
        {
            HandleMovement();
            HandleFX();
        }

        private void HandleMovement()
        {
            if (Fuel <= 0f) return;

            // Input : avant/arrière
            float forwardInput = _input.move.y;
            float turnInput = _input.move.x;

            // Accélération douce
            float targetSpeed = forwardInput * MoveSpeed;
            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.deltaTime * Acceleration);

            // Rotation sur Y (pas de tilt)
            if (Mathf.Abs(turnInput) > 0.1f)
                transform.Rotate(Vector3.up, turnInput * RotationSpeed * Time.deltaTime);

            // Déplacement avant/arrière
            Vector3 move = transform.forward * _currentSpeed * Time.deltaTime;
            _controller.Move(move);

            // Consommation carburant
            if (Mathf.Abs(forwardInput) > 0.1f || Mathf.Abs(turnInput) > 0.1f)
                Fuel -= FuelConsumptionRate * Time.deltaTime;

            Fuel = Mathf.Max(Fuel, 0f);
        }

        private void HandleFX()
        {
            if (!MainThrusterFX && !LeftThrusterFX && !RightThrusterFX) return;

            float forwardInput = _input.move.y;
            float turnInput = _input.move.x;

            // Main thruster FX removed: we only use lateral thrusters for visual feedback
            // (intentionally not using MainThrusterFX for flame output)

            // FX latéraux (rotation)
            // Combine forward/back input (activates both lateral thrusters) with turning input
            float forwardContribution = Mathf.Clamp01(Mathf.Abs(forwardInput));
            float leftTurnContribution = Mathf.Max(0f, turnInput) * TurnFXWeight;   // positive turn -> left thruster
            float rightTurnContribution = Mathf.Max(0f, -turnInput) * TurnFXWeight; // negative turn -> right thruster

            // Shared pulse so both thrusters animate in phase (keeps timing synchronized)
            float pulse = 1f;
            if (PulseAmount > 0f && PulseSpeed > 0f)
            {
                pulse += PulseAmount * Mathf.Sin(Time.time * PulseSpeed);
                pulse = Mathf.Max(0f, pulse); // avoid negative multiplier
            }

            float leftTotal = Mathf.Clamp01(forwardContribution + leftTurnContribution) * pulse;
            float rightTotal = Mathf.Clamp01(forwardContribution + rightTurnContribution) * pulse;

            // Threshold under which we consider FX should stop
            const float FX_THRESHOLD = 0.02f;

            // Update emission rates (constant curves)
            if (LeftThrusterFX)
            {
                var emission = LeftThrusterFX.emission;
                emission.rateOverTime = leftTotal * MaxFXIntensity;
            }

            if (RightThrusterFX)
            {
                var emission = RightThrusterFX.emission;
                emission.rateOverTime = rightTotal * MaxFXIntensity;
            }

            // Play both thrusters together to keep them in phase visually when either should start
            bool leftShouldPlay = leftTotal > FX_THRESHOLD;
            bool rightShouldPlay = rightTotal > FX_THRESHOLD;
            bool anyShouldPlay = leftShouldPlay || rightShouldPlay;

            // If either should play and none are playing yet, start both at the same frame
            if (anyShouldPlay && !( (LeftThrusterFX && LeftThrusterFX.isPlaying) || (RightThrusterFX && RightThrusterFX.isPlaying) ))
            {
                // If manual sync is enabled, pre-simulate both systems to the same simTime before playing
                if (ManualParticleSync && ParticleCycle > 0f)
                {
                    float simTime = Time.time % ParticleCycle;
                    if (LeftThrusterFX) LeftThrusterFX.Simulate(simTime, true, true);
                    if (RightThrusterFX) RightThrusterFX.Simulate(simTime, true, true);
                    if (LeftThrusterFX) LeftThrusterFX.Play(false);
                    if (RightThrusterFX) RightThrusterFX.Play(false);
                }
                else
                {
                    if (LeftThrusterFX) LeftThrusterFX.Play(true);
                    if (RightThrusterFX) RightThrusterFX.Play(true);
                }
            }

            // If neither should play, stop both (so they stop together)
            if (!anyShouldPlay)
            {
                if (LeftThrusterFX && LeftThrusterFX.isPlaying) LeftThrusterFX.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                if (RightThrusterFX && RightThrusterFX.isPlaying) RightThrusterFX.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }

            // (manual sync handled when starting playback to avoid extra per-frame Simulate calls)
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
        }
    }
}
