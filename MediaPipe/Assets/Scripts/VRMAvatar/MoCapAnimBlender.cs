using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRMAvatar
{
    public class MoCapAnimBlender : MonoBehaviour
    {
        private void Start()
        {
            this.srcModel = GameObject.Find("robotDAC").transform;
            this.anim = GameObject.Find("robotAnim").transform;
            this.dac = this.srcModel.GetComponent<AvatarBody>();
            this.hip = Utils.getChildGameObject(this.srcModel.gameObject, "hips").transform;
            this.vm = this.srcModel.GetComponent<VisibilityManager>();
            this.rh = this.srcModel.GetComponent<AvatarHandRight>();
            this.lh = this.srcModel.GetComponent<AvatarHandLeft>();
            this.fm = this.srcModel.GetComponent<FilterManager>();
            this.init();
        }
        private void init()
        {
            this.srcAnimator = this.srcModel.GetComponent<Animator>();
            this.selfAnimator = base.gameObject.GetComponent<Animator>();
            this.srcAnimAnimator = this.anim.GetComponent<Animator>();
            this.InitBones();
            this.SetJointsInitRotation();
            this.SetInitPosition();
            this.inited = true;
        }
        private void LateUpdate()
        {
            if (this.inited)
            {
                this.NoseVisibility = this.vm.FilteredWorldPoseVisibilities[0];
                this.NoseWeight = Utils.Remap(this.vm.FilteredWorldPoseVisibilities[0], this.NoseVisibilityWeight.x, this.NoseVisibilityWeight.y, 0f, 1f);
                this.LeftHandVisibility = this.vm.FilteredWorldPoseVisibilities[15];
                this.RightHandVisibility = this.vm.FilteredWorldPoseVisibilities[16];
                this.LeftHandWeight = Mathf.Max(Utils.Remap(this.LeftHandVisibility, this.HandVisibilityWeight.x, this.HandVisibilityWeight.y, 0f, 1f), this.vm.LerpLeftHand);
                this.RightHandWeight = Mathf.Max(Utils.Remap(this.RightHandVisibility, this.HandVisibilityWeight.x, this.HandVisibilityWeight.y, 0f, 1f), this.vm.LerpRightHand);
                this.HipVisibility = (this.vm.FilteredWorldPoseVisibilities[23] + this.vm.FilteredWorldPoseVisibilities[24]) / 2f;
                this.HipWeight = Utils.Remap((this.vm.FilteredWorldPoseVisibilities[23] + this.vm.FilteredWorldPoseVisibilities[24]) / 2f, this.HipVisibilityWeight.x, this.HipVisibilityWeight.y, 0f, 1f);
                this.LeftFootVisibility = this.vm.FilteredWorldPoseVisibilities[27];
                this.RightFootVisibility = this.vm.FilteredWorldPoseVisibilities[28];
                this.LegWeight = Utils.Remap((this.vm.FilteredWorldPoseVisibilities[27] + this.vm.FilteredWorldPoseVisibilities[28]) / 2f, this.LegVisibilityWeight.x, this.LegVisibilityWeight.y, 0f, 1f);
                this.SetJointsRotation();
                this.SetPosition();
            }
        }

        private void InitBones()
        {
            this.srcJoints = new List<Transform>();
            this.srcAnimJoints = new List<Transform>();
            this.selfJoints = new List<Transform>();
            for (int i = 0; i < MoCapAnimBlender.bonesToUse.Length; i++)
            {
                if (this.selfAnimator.GetBoneTransform(MoCapAnimBlender.bonesToUse[i]) == null || this.srcAnimator.GetBoneTransform(MoCapAnimBlender.bonesToUse[i]) == null)
                {
                    this.srcJoints.Add(null);
                    this.selfJoints.Add(null);
                    this.srcAnimJoints.Add(null);
                }
                else
                {
                    this.srcJoints.Add(this.srcAnimator.GetBoneTransform(MoCapAnimBlender.bonesToUse[i]));
                    this.selfJoints.Add(this.selfAnimator.GetBoneTransform(MoCapAnimBlender.bonesToUse[i]));
                    this.srcAnimJoints.Add(this.srcAnimAnimator.GetBoneTransform(MoCapAnimBlender.bonesToUse[i]));
                }
            }
        }

        private void SetJointsInitRotation()
        {
            this.srcInitRotation = this.srcModel.transform.rotation;
            this.selfInitRotation = base.gameObject.transform.rotation;
            this.srcJointsInitRotation = new List<Quaternion>();
            this.selfJointsInitRotation = new List<Quaternion>();
            this.currentJointsRotation = new List<Quaternion>();
            for (int i = 0; i < MoCapAnimBlender.bonesToUse.Length; i++)
            {
                if (this.selfAnimator.GetBoneTransform(MoCapAnimBlender.bonesToUse[i]) == null || this.srcAnimator.GetBoneTransform(MoCapAnimBlender.bonesToUse[i]) == null)
                {
                    this.srcJointsInitRotation.Add(Quaternion.identity);
                    this.selfJointsInitRotation.Add(Quaternion.identity);
                    this.currentJointsRotation.Add(Quaternion.identity);
                }
                else
                {
                    this.srcJointsInitRotation.Add(this.srcJoints[i].rotation * Quaternion.Inverse(this.srcInitRotation));
                    this.selfJointsInitRotation.Add(Quaternion.Inverse(this.selfInitRotation) * this.selfJoints[i].rotation);
                    this.currentJointsRotation.Add(Quaternion.Inverse(this.selfInitRotation) * this.selfJoints[i].rotation);
                }
            }
        }

        private void SetJointsRotation()
        {
            for (int i = 0; i < MoCapAnimBlender.bonesToUse.Length; i++)
            {
                if (!(this.selfAnimator.GetBoneTransform(MoCapAnimBlender.bonesToUse[i]) == null) && !(this.srcAnimator.GetBoneTransform(MoCapAnimBlender.bonesToUse[i]) == null))
                {
                    Quaternion quaternion = this.selfInitRotation;
                    quaternion *= Quaternion.Inverse(this.srcJointsInitRotation[i]) * this.srcJoints[i].rotation;
                    quaternion *= this.selfJointsInitRotation[i];
                    quaternion = this.srcJoints[i].localRotation;
                    if (this.hip && (this.HipWeight > 0f || this.NoseWeight > 0f))
                    {
                        float num = Mathf.Min(Mathf.Abs(this.hip.rotation.eulerAngles.y), 360f - Mathf.Abs(this.hip.rotation.eulerAngles.y));
                        float num2 = Mathf.Min(Mathf.Abs(this.hip.rotation.eulerAngles.z), 360f - Mathf.Abs(this.hip.rotation.eulerAngles.z));
                        float from = Mathf.Max(num, num2);
                        this.hipRotationWeight = Utils.Remap(from, 0f, 20f, 0f, 1f);
                    }
                    else
                    {
                        this.hipRotationWeight = 0f;
                    }
                    Quaternion quaternion2;
                    if (i > 5 && i <= 9)
                    {
                        quaternion2 = Quaternion.Slerp(this.srcAnimJoints[i].localRotation, quaternion, Mathf.Max(this.LeftHandWeight, this.hipRotationWeight));
                    }
                    else if (i > 9 && i <= 24)
                    {
                        quaternion2 = Quaternion.Slerp(this.srcAnimJoints[i].localRotation, quaternion, Mathf.Min(this.lh.lerp, Mathf.Max(this.LeftHandWeight, this.hipRotationWeight)));
                    }
                    else if (i > 24 && i <= 28)
                    {
                        quaternion2 = Quaternion.Slerp(this.srcAnimJoints[i].localRotation, quaternion, Mathf.Max(this.RightHandWeight, this.hipRotationWeight));
                    }
                    else if (i > 28 && i <= 43)
                    {
                        quaternion2 = Quaternion.Slerp(this.srcAnimJoints[i].localRotation, quaternion, Mathf.Min(this.rh.lerp, Mathf.Max(this.RightHandWeight, this.hipRotationWeight)));
                    }
                    else if (i > 43)
                    {
                        quaternion2 = Quaternion.Slerp(this.srcAnimJoints[i].localRotation, quaternion, Mathf.Max(this.NoseWeight, this.hipRotationWeight));
                    }
                    else if (i == 2)
                    {
                        quaternion2 = Quaternion.Slerp(this.srcAnimJoints[i].localRotation, quaternion, Mathf.Max(this.NoseWeight, this.hipRotationWeight));
                    }
                    else
                    {
                        quaternion2 = Quaternion.Slerp(this.srcAnimJoints[i].localRotation, quaternion, Mathf.Max(this.NoseWeight, this.hipRotationWeight));
                    }
                    this.selfJoints[i].localRotation = Quaternion.Slerp(this.currentJointsRotation[i], quaternion2, this.rotationFilter);
                    this.currentJointsRotation[i] = this.selfJoints[i].localRotation;
                }
            }
        }

        public void SetInitPosition()
        {
            this.srcRoot = this.srcAnimator.GetBoneTransform(0);
            this.selfRoot = this.selfAnimator.GetBoneTransform(0);
            this.srcInitPosition = this.srcRoot.localPosition;
            this.selfInitPosition = this.selfRoot.localPosition;
            this.currentPosition = this.selfRoot.localPosition;
            this.scale = (this.selfRoot.position.y - this.selfRoot.parent.position.y) / (this.srcRoot.position.y - this.srcRoot.parent.position.y);
        }

        private void SetPosition()
        {
            if (this.dac.mocapmode == MoCapMode.upperbody)
            {
                this.HipWeight = 0f;
            }
            Vector3 vector = (this.srcRoot.localPosition - this.srcInitPosition) * this.scale * this.HipWeight;
            this.selfRoot.localPosition = (this.currentPosition - this.selfInitPosition) * this.positionFilter + vector * (1f - this.positionFilter) + this.selfInitPosition;
            this.currentPosition = this.selfRoot.localPosition;
        }

        private Transform srcModel;

        private Transform anim;

        private Transform hip;

        private Animator srcAnimator;

        private Animator srcAnimAnimator;

        private Animator selfAnimator;

        private List<Transform> srcJoints;

        private List<Transform> srcAnimJoints;

        private List<Transform> selfJoints;

        private Quaternion srcInitRotation;

        private Quaternion selfInitRotation;

        private List<Quaternion> srcJointsInitRotation;

        private List<Quaternion> selfJointsInitRotation;

        private List<Quaternion> currentJointsRotation;

        private Transform srcRoot;

        private Transform selfRoot;

        private Vector3 srcInitPosition;

        private Vector3 selfInitPosition;

        private Vector3 currentPosition;

        private float scale;

        private bool inited;

        private VisibilityManager vm;

        private AvatarBody dac;

        private AvatarHandRight rh;

        private AvatarHandLeft lh;

        private float positionFilter = 0.7f;

        private float rotationFilter = 0.7f;

        [Header("Visibility")]
        public float NoseVisibility;

        public float LeftHandVisibility;

        public float RightHandVisibility;

        public float HipVisibility;

        public float LeftFootVisibility;

        public float RightFootVisibility;

        [Header("Visibility Weight")]
        public Vector2 NoseVisibilityWeight = new Vector2(0.4f, 0.7f);

        public Vector2 HandVisibilityWeight = new Vector2(0.4f, 0.7f);

        public Vector2 HipVisibilityWeight = new Vector2(0.4f, 0.7f);

        public Vector2 LegVisibilityWeight = new Vector2(0.4f, 0.7f);

        [HideInInspector]
        public float LeftHandWeight;

        [HideInInspector]
        public float RightHandWeight;

        [HideInInspector]
        public float HipWeight;

        [HideInInspector]
        public float NoseWeight;

        [HideInInspector]
        public float LegWeight;

        [HideInInspector]
        public float hipRotationWeight;

        private FilterManager fm;

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
    }
}