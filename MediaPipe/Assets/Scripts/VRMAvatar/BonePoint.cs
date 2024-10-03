using UnityEngine;

namespace VRMAvatar
{
    public class BonePoint
    {
        public Vector3 Pos3D;

        public float Score3D;

        public int Error;

        public bool Enabled;

        public bool Visibled;

        public bool Lock;

        public Transform Transform;

        public Quaternion InitRotation;

        public Quaternion InitLocalRotation;

        public Quaternion Inverse;

        public Quaternion InverseRotation;

        public Quaternion calcuRotation;

        public BonePoint Child;

        public BonePoint Parent;
    }
}