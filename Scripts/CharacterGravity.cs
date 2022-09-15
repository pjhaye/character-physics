using System;
using UnityEngine;

namespace CharacterPhysics
{
    [RequireComponent(typeof(ICharacterBody))]
    [DefaultExecutionOrder(-100)]
    public class CharacterGravity : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _gravity = new Vector3(0.0f, -9.1f, 0.0f);
        [SerializeField]
        private float _restingGravity = 0.01f;
        
        private ICharacterBody _characterBody;

        public Vector3 Gravity
        {
            get
            {
                return _gravity;
            }
            set
            {
                _gravity = value;
            }
        }

        public float RestingGravity
        {
            get => _restingGravity;
            set => _restingGravity = value;
        }

        private void OnEnable()
        {
            _characterBody = GetComponent<ICharacterBody>();
        }

        private void FixedUpdate()
        {
            if (!_characterBody.IsTouchingGround)
            {
                _characterBody.Velocity += _gravity * Time.deltaTime;
            }
            else if(_characterBody.Velocity.y < 0.0f)
            {
                _characterBody.Velocity = new Vector3(_characterBody.Velocity.x, RestingGravity, _characterBody.Velocity.z);
            }
        }
    }
}