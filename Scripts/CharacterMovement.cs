using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace CharacterPhysics
{
    [RequireComponent(typeof(ICharacterBody))]
    [DefaultExecutionOrder(-100)]
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField]
        private float _maxRunSpeed = 3.0f;
        [SerializeField]
        private float _acceleration = 3.0f;
        [SerializeField]
        private float _decceleration = 3.0f;
        [SerializeField]
        private float _airControl = 0.5f;
        [SerializeField]
        private float _airDecceleration = 0.0f;
        [SerializeField]
        private float _rotationSpeed = 500.0f;
        [SerializeField]
        public bool _rotateTowardAcceleration = true;
        
        private ICharacterBody _characterBody;

        private bool _acceleratedLastFrame = false;

        public float MaxRunSpeed
        {
            get => _maxRunSpeed;
            set => _maxRunSpeed = value;
        }

        public float Acceleration
        {
            get => _acceleration;
            set => _acceleration = value;
        }

        public float Decceleration
        {
            get => _decceleration;
            set => _decceleration = value;
        }

        public float AirControl
        {
            get => _airControl;
            set => _airControl = value;
        }

        public float RotationSpeed
        {
            get => _rotationSpeed;
            set => _rotationSpeed = value;
        }

        public bool RotateTowardAcceleration
        {
            get => _rotateTowardAcceleration;
            set => _rotateTowardAcceleration = value;
        }

        public float AirDecceleration
        {
            get => _airDecceleration;
            set => _airDecceleration = value;
        }

        private void OnEnable()
        {
            _characterBody = GetComponent<ICharacterBody>();
        }

        public void Launch(Vector3 force)
        {
            _characterBody.Velocity = force;
        }

        [Button]
        public void Jump(float jumpForce)
        {
            var jumpVelocity = _characterBody.Velocity;
            jumpVelocity.y = jumpForce;
            
            Launch(jumpVelocity);
        }

        private void FixedUpdate()
        {
            if (!_acceleratedLastFrame)
            {
                Decelerate(Time.deltaTime);
            }

            _acceleratedLastFrame = false;
        }

        private void Decelerate(float deltaTime)
        {
            var xzMovement = new Vector3(_characterBody.Velocity.x, 0.0f, _characterBody.Velocity.z);
            var xzSpeed = xzMovement.magnitude;
            xzSpeed -= (_characterBody.IsTouchingGround ? Decceleration : AirDecceleration) * deltaTime;
            
            if (xzSpeed < 0.0f)
            {
                xzSpeed = 0.0f;
            }
            
            xzMovement = xzMovement.normalized * xzSpeed;
            _characterBody.Velocity = new Vector3(xzMovement.x, _characterBody.Velocity.y, xzMovement.z);
        }

        public void Accelerate(Vector3 direction, float strength, float deltaTime)
        {
            if (strength == 0.0f)
            {
                return;
            }
            
            var xzMovement = new Vector3(_characterBody.Velocity.x, 0.0f, _characterBody.Velocity.z);

            if (!_characterBody.IsTouchingGround)
            {
                strength *= AirControl;
            }
            
            xzMovement += new Vector3(direction.x, 0.0f, direction.z) * (Acceleration * strength * deltaTime);

            var xzSpeed = xzMovement.magnitude;
            var xzDirection = xzMovement.normalized;

            if (xzSpeed >= MaxRunSpeed)
            {
                xzSpeed = MaxRunSpeed;
            }

            xzMovement = xzDirection * xzSpeed;

            if (xzMovement.magnitude < float.Epsilon)
            {
                return;
            }
            
            _characterBody.Velocity = new Vector3(xzMovement.x, _characterBody.Velocity.y, xzMovement.z);

            _acceleratedLastFrame = true;

            if (RotateTowardAcceleration)
            {
                var targetRotation = Quaternion.LookRotation(xzMovement.normalized);
                
                _characterBody.WorldRotation = Quaternion.RotateTowards(
                    _characterBody.WorldRotation,
                    targetRotation,
                    RotationSpeed * deltaTime);
            }
        }
    }
}
