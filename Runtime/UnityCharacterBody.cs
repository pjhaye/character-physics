using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterPhysics
{
    [RequireComponent(typeof(CharacterController))]
    public class UnityCharacterBody : MonoBehaviour, ICharacterBody
    {
        public event EventHandler StartedTouchingGround;
        public event EventHandler StoppedTouchingGround;
        public event EventHandler<CharacterBodyHitInfo> HitCollider;

        public Vector3 Up => transform.up;
        
        public Vector3 Forward => transform.forward;
        
        public Vector3 Right => transform.right;

        public Vector3 WorldTop
        {
            get
            {
                return _characterController.transform.TransformPoint(
                       (_characterController.center +
                       Vector3.up * _characterController.height * 0.5f));
            }
        }

        public Vector3 WorldBottom
        {
            get
            {
                return _characterController.transform.TransformPoint(
                       (_characterController.center +
                        -Vector3.up * _characterController.height * 0.5f));
            }
        }

        public Vector3 WorldCenter
        {
            get
            {
                return _characterController.transform.TransformPoint(
                       (_characterController.center));
            }
        }

        public Vector3 WorldPosition
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }

        public Quaternion WorldRotation
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }

        public Vector3 WorldEulerAngles
        {
            get => transform.eulerAngles;
            set => transform.eulerAngles = value;
        }


        public float Height
        {
            get
            {
                return _characterController.height;
            }
        }

        public float Radius
        {
            get
            {
                return _characterController.radius;
            }
        }

        public Vector3 Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }

        [SerializeField]
        private float _groundCheckStartHeight = 0.15f;
        [SerializeField]
        private float _groundCheckDistance = 0.25f;
        
        private Vector3 _velocity;
        
        private CharacterController _characterController;

        private List<Vector3> _movementsThisFrame = new List<Vector3>();
        private bool _isTouchingGround = false;

        public bool IsTouchingGround
        {
            get
            {
                return _isTouchingGround;
            }
        }
        
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void FixedUpdate()
        {
            var movementVelocity = Velocity;
            
            var didStickToSlope = false;
            if (IsTouchingGround && movementVelocity.y < 0.0f)
            {
                var raycast = new Ray(WorldBottom + Up * _groundCheckStartHeight, -Up);
                if (Physics.Raycast(raycast, out var raycastHit, _groundCheckStartHeight + _groundCheckDistance))
                {
                    if (raycastHit.point.y < WorldBottom.y)
                    {
                        movementVelocity.y = 0.0f;
                        var slopeAngle = Vector3.Angle(Up, raycastHit.normal);
                        var cross = Vector3.Cross(movementVelocity.normalized, Up).normalized;
                        movementVelocity = Quaternion.AngleAxis(slopeAngle, cross) * movementVelocity;
                        if (movementVelocity.y > 0.0f)
                        {
                            movementVelocity.y *= -1.0f;
                        }
                        
                        didStickToSlope = true;
                    }
                }
            }

            MoveBy(movementVelocity * Time.deltaTime);
            
            var totalMovement = Vector3.zero;
            foreach (var movement in _movementsThisFrame)
            {
                totalMovement += movement;
            }
            
            _movementsThisFrame.Clear();

            var wasTouchingGround = IsTouchingGround;
            
            var collisionFlags = _characterController.Move(totalMovement);
            if ((collisionFlags & CollisionFlags.Below) != 0 || didStickToSlope)
            {
                _isTouchingGround = true;
                if (!wasTouchingGround)
                {
                    StartedTouchingGround?.Invoke(this, null);
                }
            }
            else
            {
                _isTouchingGround = false;
                if (wasTouchingGround)
                {
                    StoppedTouchingGround?.Invoke(this, null);
                }
            }

            if ((collisionFlags & CollisionFlags.Above) != 0 && Velocity.y > 0.0f)
            {
                Velocity = new Vector3(Velocity.x, 0.0f, Velocity.z);
            }
        }

        public void MoveBy(Vector3 movement)
        {
            _movementsThisFrame.Add(movement);
        }

        public void MoveTo(Vector3 position)
        {
            _movementsThisFrame.Add(position - WorldPosition);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            HitCollider?.Invoke(this, new CharacterBodyHitInfo()
            {
                Collider = hit.collider,
                Normal = hit.normal,
                Point = hit.point,
                Distance = hit.moveLength, 
                MovementDirection = hit.moveDirection
            });
        }

        public void TeleportTo(Vector3 position)
        {
            _movementsThisFrame.Clear();
            WorldPosition = position;
        }
    }
}