using UnityEngine;

namespace CharacterPhysics
{
    public class CharacterBodyHitInfo
    {
        public Collider Collider;
        public Vector3 Normal;
        public Vector3 Point;
        public float Distance;
        public Vector3 MovementDirection;
    }
}