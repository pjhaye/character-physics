using UnityEngine;

namespace CharacterPhysics
{
    public struct CharacterMovementResult
    {
        public bool DidHitSurface;
        public Vector3 HitSurfaceNormal;
        public Vector3 HitSurfacePosition;
    }
}