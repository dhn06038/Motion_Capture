using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRMAvatar
{
    public class MoCapSrc : MonoBehaviour
    {
        private void Start()
        {
            this.sync = new bool[]
            {
                this.Neck,
                this.Head,
                this.Hips,
                this.Spine,
                this.Chest,
                this.UpperChest,
                this.LeftShoulder,
                this.LeftUpperArm,
                this.LeftLowerArm,
                this.LeftHand,
                this.LeftThumb,
                this.LeftThumb,
                this.LeftThumb,
                this.LeftIndex,
                this.LeftIndex,
                this.LeftIndex,
                this.LeftMiddle,
                this.LeftMiddle,
                this.LeftMiddle,
                this.LeftRing,
                this.LeftRing,
                this.LeftRing,
                this.LeftPinky,
                this.LeftPinky,
                this.LeftPinky,
                this.RightShoulder,
                this.RightUpperArm,
                this.RightLowerArm,
                this.RightHand,
                this.RightThumb,
                this.RightThumb,
                this.RightThumb,
                this.RightIndex,
                this.RightIndex,
                this.RightIndex,
                this.RightMiddle,
                this.RightMiddle,
                this.RightMiddle,
                this.RightRing,
                this.RightRing,
                this.RightRing,
                this.RightPinky,
                this.RightPinky,
                this.RightPinky,
                this.LeftUpperLeg,
                this.LeftLowerLeg,
                this.LeftFoot,
                this.LeftToes,
                this.RightUpperLeg,
                this.RightLowerLeg,
                this.RightFoot,
                this.RightToes
            };
            this.srcModel = Utils.getChildGameObject(this.src, "robot").transform;
            this.init();
            this.StartRetargeting();
        }

        private void init()
        {
            this.srcAnimator = this.srcModel.GetComponent<Animator>();
            this.selfAnimator = base.gameObject.GetComponent<Animator>();
            this.InitBones();
            this.SetJointsInitRotation();
            this.SetInitPosition();
        }

        private void LateUpdate()
        {
            if (this.inited)
            {
                this.SetJointsRotation();
                this.SetPosition();
            }
        }

        private void getScale()
        {
            this.SetInitPosition();
        }

        private void StartRetargeting()
        {
            this.inited = true;
        }

        private void InitBones()
        {
            this.srcJoints = new List<Transform>();
            this.selfJoints = new List<Transform>();
            for (int i = 0; i < MoCapSrc.bonesToUse.Length; i++)
            {
                if (this.selfAnimator.GetBoneTransform(MoCapSrc.bonesToUse[i]) == null || this.srcAnimator.GetBoneTransform(MoCapSrc.bonesToUse[i]) == null)
                {
                    this.srcJoints.Add(null);
                    this.selfJoints.Add(null);
                }
                else
                {
                    this.srcJoints.Add(this.srcAnimator.GetBoneTransform(MoCapSrc.bonesToUse[i]));
                    this.selfJoints.Add(this.selfAnimator.GetBoneTransform(MoCapSrc.bonesToUse[i]));
                }
            }
        }

        private void SetJointsInitRotation()
        {
            this.srcInitRotation = this.srcModel.transform.rotation;
            this.selfInitRotation = base.gameObject.transform.rotation;
            this.srcJointsInitRotation = new List<Quaternion>();
            this.selfJointsInitRotation = new List<Quaternion>();
            for (int i = 0; i < MoCapSrc.bonesToUse.Length; i++)
            {
                if (this.selfAnimator.GetBoneTransform(MoCapSrc.bonesToUse[i]) == null || this.srcAnimator.GetBoneTransform(MoCapSrc.bonesToUse[i]) == null)
                {
                    this.srcJointsInitRotation.Add(Quaternion.identity);
                    this.selfJointsInitRotation.Add(Quaternion.identity);
                }
                else
                {
                    this.srcJointsInitRotation.Add(this.srcJoints[i].rotation * Quaternion.Inverse(this.srcInitRotation));
                    this.selfJointsInitRotation.Add(Quaternion.Inverse(this.selfInitRotation) * this.selfJoints[i].rotation);
                }
            }
        }

        private void SetJointsRotation()
        {
            for (int i = 0; i < MoCapSrc.bonesToUse.Length; i++)
            {
                if (this.sync[i] && !(this.selfAnimator.GetBoneTransform(MoCapSrc.bonesToUse[i]) == null) && !(this.srcAnimator.GetBoneTransform(MoCapSrc.bonesToUse[i]) == null))
                {
                    Quaternion quaternion = this.selfInitRotation;
                    quaternion *= Quaternion.Inverse(this.srcJointsInitRotation[i]) * this.srcJoints[i].rotation;
                    quaternion *= this.selfJointsInitRotation[i];
                    if (i > 5 && i <= 24)
                    {
                        this.selfJoints[i].rotation = Quaternion.Slerp(this.selfJoints[i].rotation, quaternion, this.LeftHandWeight);
                    }
                    else if (i > 24 && i <= 43)
                    {
                        this.selfJoints[i].rotation = Quaternion.Slerp(this.selfJoints[i].rotation, quaternion, this.RightHandWeight);
                    }
                    else
                    {
                        this.selfJoints[i].rotation = quaternion;
                    }
                }
            }
        }

        private void SetInitPosition()
        {
            this.srcRoot = this.srcAnimator.GetBoneTransform(0);
            this.selfRoot = this.selfAnimator.GetBoneTransform(0);
            this.srcInitPosition = this.srcRoot.localPosition;
            this.selfInitPosition = this.selfRoot.localPosition;
            Vector3 vector = Quaternion.Inverse(this.selfRoot.root.transform.rotation) * this.selfRoot.transform.position;
            Vector3 vector2 = Quaternion.Inverse(this.selfRoot.transform.root.transform.rotation) * this.selfRoot.transform.root.transform.position;
            this.scale = (vector.y - vector2.y) / this.srcRoot.localPosition.y;
        }

        private void SetPosition()
        {
            this.selfRoot.localPosition = (this.srcRoot.localPosition - this.srcInitPosition) * this.scale + this.selfInitPosition;
        }

        public GameObject src;

        [Range(0f, 1f)]
        public float LeftHandWeight = 1f;
        [Range(0f, 1f)]
        public float RightHandWeight = 1f;

        [Header("Sync Body")]
        public bool Neck = true;
        public bool Head = true;
        public bool Hips = true;
        public bool Spine = true;
        public bool Chest = true;
        public bool UpperChest = true;

        [Header("Sync Left Hand")]
        public bool LeftShoulder = true;
        public bool LeftUpperArm = true;
        public bool LeftLowerArm = true;
        public bool LeftHand = true;

        [Header("Sync Left Fingers")]
        public bool LeftThumb = true;
        public bool LeftIndex = true;
        public bool LeftMiddle = true;
        public bool LeftRing = true;
        public bool LeftPinky = true;

        [Header("Sync Right Hand")]
        public bool RightShoulder = true;
        public bool RightUpperArm = true;
        public bool RightLowerArm = true;
        public bool RightHand = true;

        [Header("Sync Right Fingers")]
        public bool RightThumb = true;
        public bool RightIndex = true;
        public bool RightMiddle = true;
        public bool RightRing = true;
        public bool RightPinky = true;

        [Header("Sync Left Leg")]
        public bool LeftUpperLeg = true;
        public bool LeftLowerLeg = true;
        public bool LeftFoot = true;
        public bool LeftToes = true;

        [Header("Sync Right Leg")]
        public bool RightUpperLeg = true;
        public bool RightLowerLeg = true;
        public bool RightFoot = true;
        public bool RightToes = true;

        private Transform srcModel;
        private Animator srcAnimator;
        private Animator selfAnimator;
        private List<Transform> srcJoints;
        private List<Transform> selfJoints;
        private Quaternion srcInitRotation;
        private Quaternion selfInitRotation;
        private List<Quaternion> srcJointsInitRotation;
        private List<Quaternion> selfJointsInitRotation;
        private Transform srcRoot;
        private Transform selfRoot;
        private Vector3 srcInitPosition;
        private Vector3 selfInitPosition;
        private float scale;
        private bool inited;
        private static HumanBodyBones[] bonesToUse = new HumanBodyBones[]
        {
            (HumanBodyBones)9,
            (HumanBodyBones)10,
            default(HumanBodyBones),
            (HumanBodyBones)7,
            (HumanBodyBones)8,
            (HumanBodyBones)54,
            (HumanBodyBones)11,
            (HumanBodyBones)13,
            (HumanBodyBones)15,
            (HumanBodyBones)17,
            (HumanBodyBones)24,
            (HumanBodyBones)25,
            (HumanBodyBones)26,
            (HumanBodyBones)27,
            (HumanBodyBones)28,
            (HumanBodyBones)29,
            (HumanBodyBones)30,
            (HumanBodyBones)31,
            (HumanBodyBones)32,
            (HumanBodyBones)33,
            (HumanBodyBones)34,
            (HumanBodyBones)35,
            (HumanBodyBones)36,
            (HumanBodyBones)37,
            (HumanBodyBones)38,
            (HumanBodyBones)12,
            (HumanBodyBones)14,
            (HumanBodyBones)16,
            (HumanBodyBones)18,
            (HumanBodyBones)39,
            (HumanBodyBones)40,
            (HumanBodyBones)41,
            (HumanBodyBones)42,
            (HumanBodyBones)43,
            (HumanBodyBones)44,
            (HumanBodyBones)45,
            (HumanBodyBones)46,
            (HumanBodyBones)47,
            (HumanBodyBones)48,
            (HumanBodyBones)49,
            (HumanBodyBones)50,
            (HumanBodyBones)51,
            (HumanBodyBones)52,
            (HumanBodyBones)53,
            (HumanBodyBones)1,
            (HumanBodyBones)3,
            (HumanBodyBones)5,
            (HumanBodyBones)19,
            (HumanBodyBones)2,
            (HumanBodyBones)4,
            (HumanBodyBones)6,
            (HumanBodyBones)20,
        };

        private bool[] sync;
    }
}
