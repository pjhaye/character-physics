using System;
using UnityEngine;
using UnityEngine.Events;

namespace CharacterPhysics
{
    [RequireComponent(typeof(ICharacterBody))]
    public class CharacterBodyEventListener : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent _startedTouchingGround;
        [SerializeField]
        private UnityEvent _stoppedTouchingGround;
        [SerializeField]
        private UnityEvent<CharacterBodyHitInfo> _hitCollider;
        
        private ICharacterBody _characterBody;
        
        private void OnEnable()
        {
            _characterBody = GetComponent<ICharacterBody>();

            if (_characterBody != null)
            {
                _characterBody.StartedTouchingGround += OnStartTouchingGround;
                _characterBody.StoppedTouchingGround += OnStopTouchingGround;
                _characterBody.HitCollider += OnHitCollider;
            }
        }
        
        private void OnDisable()
        {
            if (_characterBody != null)
            {
                _characterBody.StartedTouchingGround -= OnStartTouchingGround;
                _characterBody.StoppedTouchingGround -= OnStopTouchingGround;
                _characterBody.HitCollider -= OnHitCollider;
            }
        }

        private void OnStartTouchingGround(object sender, EventArgs e)
        {
            _startedTouchingGround?.Invoke();
        }

        private void OnStopTouchingGround(object sender, EventArgs e)
        {
            _stoppedTouchingGround?.Invoke();
        }
        
        private void OnHitCollider(object sender, CharacterBodyHitInfo e)
        {
            _hitCollider?.Invoke(e);
        }
    }
}