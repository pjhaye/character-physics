using System;
using System.Numerics;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace CharacterPhysics
{
    public interface ICharacterBody
    {
        public event EventHandler StartedTouchingGround;
        public event EventHandler StoppedTouchingGround;
        public event EventHandler<CharacterBodyHitInfo> HitCollider; 

        public Vector3 Up { get; }
        public Vector3 Forward { get; }
        public Vector3 Right { get; }
        public Vector3 WorldTop { get; }
        public Vector3 WorldBottom { get; }
        public Vector3 WorldCenter { get; }
        public Vector3 WorldPosition { get; set; }
        public Quaternion WorldRotation { get; set; }
        public Vector3 WorldEulerAngles { get; set; }
        public float Height { get; }
        public Vector3 Velocity { get; set; }
        public bool IsTouchingGround { get; }

        public void MoveBy(Vector3 movement);
        public void MoveTo(Vector3 position);
        public void TeleportTo(Vector3 position);
    }
}