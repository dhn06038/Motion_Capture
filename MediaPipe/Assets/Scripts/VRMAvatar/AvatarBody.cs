using System;
using UnityEngine;

namespace VRMAvatar
{
    public class AvatarBody : MonoBehaviour
    {
        private AvatarHandLeft lefthand;

        private AvatarHandRight righthand;

        private Animator AvatarAnimator;

        private GameObject baseObject;

        public BonePoint[] avatarBonePoint;

        private Vector3 initPosition;

        private Vector3 forwardUpperVec;

        private Vector3 forwardLowerVec;

        private Vector3 upperVec;

        private Vector3 downVec;

        private Vector3 rightUpperVec;

        private Vector3 rightLowerVec;

        private Vector3 lShldrBendF = Vector3.forward;

        private Vector3 lForearmBendF = Vector3.forward;

        private Vector3 rShldrBendF = Vector3.forward;

        private Vector3 rForearmBendF = Vector3.forward;

        private float EstimatedScore;

        private float EstimatedThreshold = 0.3f;

        private float FootCheckThreshold = 0.2f;

        private float tallHead;

        private float tallHeadNeck;

        private float tallNeckSpine;

        private float tallSpineCrotch;

        private float tallThigh;

        private float tallShin;

        private float prevTallHead;

        private float prevTallHeadNeck;

        private float prevTallNeckSpine;

        private float prevTallSpineCrotch;

        private float prevTallThigh;

        private float prevTallShin;

        private float VisibleThreshold = 0.05f;

        private float waistTilt;

        private float centerTall = 336f;

        private float tall = 336f;

        private float prevTall = 336f;

        private float centerHeadSize = 89.6f;

        private float headSize = 89.6f;

        private float prevHeadSize = 89.6f;

        private float hypotheticalCamera = 3f;

        private float DistanceToPerson = 3f;

        private float ZMovementSensitivity = 0.5f;

        private float bottomThreshold = -180f;

        private bool LockFoot;

        private bool LockLegs;

        private bool LockHand;

        private Vector3 movementSenstivity = new Vector3(0.01f, 0.01f, 0.01f);

        private bool PoorLowerBodyMode = true;

        public MoCapMode mocapmode;

        public int Sensitivity = 5;

        private float hipHeight = 1f;

        private float baseX = 0.5f;

        private float baseY = 0.5f;

        private float spanX = 2f;

        private float spanZ = 5f;

        [HideInInspector]
        public bool received;

        [HideInInspector]
        public bool validInput;

        private bool calibrate;

        private float pitchdeg;

        private float pitchdeglower;

        private float pitchdegupper;

        private float yawdeg;

        private Transform root;

        private Transform spine;

        private Transform chest;

        private Transform neck;

        private Transform lscapular;

        private Transform lshoulder;

        private Transform lelbow;

        private Transform lhand;

        private Transform rscapular;

        private Transform rshoulder;

        private Transform relbow;

        private Transform rhand;

        private Transform lhip;

        private Transform lknee;

        private Transform lfoot;

        private Transform ltoe;

        private Transform rhip;

        private Transform rknee;

        private Transform rfoot;

        private Transform rtoe;

        private Transform head;

        private Quaternion root_offset;

        private Quaternion spine_offset;

        private Quaternion chest_offset;

        private Quaternion lscapular_offset;

        private Quaternion lshoulder_offset;

        private Quaternion lelbow_offset;

        private Quaternion lhand_offset;

        private Quaternion rscapular_offset;

        private Quaternion rshoulder_offset;

        private Quaternion relbow_offset;

        private Quaternion rhand_offset;

        private Quaternion lhip_offset;

        private Quaternion lknee_offset;

        private Quaternion lfoot_offset;

        private Quaternion ltoe_offset;

        private Quaternion rhip_offset;

        private Quaternion rknee_offset;

        private Quaternion rfoot_offset;

        private Quaternion rtoe_offset;

        private Quaternion head_offset;

        [HideInInspector]
        public Vector3[] PoseLandmarks = new Vector3[33];

        [HideInInspector]
        public float[] PoseVisibilities = new float[33];

        [HideInInspector]
        public Vector3[] WorldPoseLandmarks = new Vector3[33];

        [HideInInspector]
        public float[] WorldPoseVisibilities = new float[33];

        [HideInInspector]
        public Vector3[] faceLandmarks = new Vector3[478];

        [HideInInspector]
        public FilterManager fm;

        private int[] facelandmarkInUse = new int[4] { 21, 172, 251, 397 };

        private bool resuming = true;

        private float SendTimeLeft = 60f;

        public string ErrorMessage { get; private set; }

        private void Awake()
        {
            float num = 0.005f;
            fm = base.gameObject.GetComponent<FilterManager>();
            righthand = base.gameObject.GetComponent<AvatarHandRight>();
            lefthand = base.gameObject.GetComponent<AvatarHandLeft>();
            AvatarAnimator = base.gameObject.GetComponent<Animator>();
            movementSenstivity = new Vector3(num, num, num);
            if (AvatarAnimator != null)
            {
                Animator avatarAnimator = AvatarAnimator;
                AvatarAnimator = null;
                MapToAvatarBone(avatarAnimator);
            }

            root = AvatarAnimator.GetBoneTransform(HumanBodyBones.Hips);
            lhip = AvatarAnimator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            lknee = AvatarAnimator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            lfoot = AvatarAnimator.GetBoneTransform(HumanBodyBones.LeftFoot);
            ltoe = AvatarAnimator.GetBoneTransform(HumanBodyBones.LeftToes);
            rhip = AvatarAnimator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
            rknee = AvatarAnimator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
            rfoot = AvatarAnimator.GetBoneTransform(HumanBodyBones.RightFoot);
            rtoe = AvatarAnimator.GetBoneTransform(HumanBodyBones.RightToes);
        }

        private void Update()
        {
            PoseUpdate();
        }

        private bool MapToAvatarBone(Animator animator)
        {
            lelbow = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            relbow = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            neck = animator.GetBoneTransform(HumanBodyBones.Neck);
            head = animator.GetBoneTransform(HumanBodyBones.Head);
            ErrorMessage = "";
            if (animator != null && animator.isHuman)
            {
                if (AvatarAnimator != null)
                {
                    SetInitTPose();
                }

                AvatarAnimator = animator;
                if (AvatarAnimator.gameObject == null)
                {
                    ErrorMessage = "Animator gameObject is null.";
                    return false;
                }

                baseObject = AvatarAnimator.gameObject;
                if (DistanceToPerson == 0f)
                {
                    centerTall = 336f;
                    centerHeadSize = 44.8f;
                }
                else
                {
                    centerTall = DistanceToPerson * 336f / hypotheticalCamera;
                    centerHeadSize = DistanceToPerson * 44.8f / hypotheticalCamera;
                }

                tall = centerTall;
                prevTall = centerTall;
                headSize = centerHeadSize;
                prevHeadSize = centerHeadSize;
                avatarBonePoint = new BonePoint[32];
                for (int i = 0; i < 32; i++)
                {
                    avatarBonePoint[i] = new BonePoint();
                    avatarBonePoint[i].Enabled = true;
                    avatarBonePoint[i].Lock = false;
                    avatarBonePoint[i].Error = 0;
                }

                try
                {
                    avatarBonePoint[0].Transform = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
                    avatarBonePoint[1].Transform = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
                    avatarBonePoint[2].Transform = animator.GetBoneTransform(HumanBodyBones.RightHand);
                    avatarBonePoint[3].Transform = animator.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);
                    avatarBonePoint[4].Transform = animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
                    if (avatarBonePoint[3].Transform == null || avatarBonePoint[4].Transform == null)
                    {
                        avatarBonePoint[2].Enabled = false;
                        avatarBonePoint[3].Enabled = false;
                        avatarBonePoint[4].Enabled = false;
                    }

                    avatarBonePoint[29].Transform = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
                    avatarBonePoint[29].Enabled = false;
                    avatarBonePoint[5].Transform = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
                    avatarBonePoint[6].Transform = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                    avatarBonePoint[7].Transform = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                    avatarBonePoint[8].Transform = animator.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);
                    avatarBonePoint[9].Transform = animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
                    if (avatarBonePoint[8].Transform == null || avatarBonePoint[9].Transform == null)
                    {
                        avatarBonePoint[7].Enabled = false;
                        avatarBonePoint[8].Enabled = false;
                        avatarBonePoint[9].Enabled = false;
                    }

                    avatarBonePoint[30].Transform = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
                    avatarBonePoint[30].Enabled = false;
                    avatarBonePoint[11].Transform = animator.GetBoneTransform(HumanBodyBones.LeftEye);
                    avatarBonePoint[13].Transform = animator.GetBoneTransform(HumanBodyBones.RightEye);
                    avatarBonePoint[15].Transform = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
                    avatarBonePoint[16].Transform = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
                    avatarBonePoint[17].Transform = animator.GetBoneTransform(HumanBodyBones.RightFoot);
                    avatarBonePoint[18].Transform = animator.GetBoneTransform(HumanBodyBones.RightToes);
                    if (avatarBonePoint[18].Transform == null)
                    {
                        avatarBonePoint[18].Enabled = false;
                    }

                    avatarBonePoint[19].Transform = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
                    avatarBonePoint[20].Transform = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
                    avatarBonePoint[21].Transform = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
                    avatarBonePoint[22].Transform = animator.GetBoneTransform(HumanBodyBones.LeftToes);
                    if (avatarBonePoint[22].Transform == null)
                    {
                        avatarBonePoint[22].Enabled = false;
                    }

                    avatarBonePoint[23].Transform = animator.GetBoneTransform(HumanBodyBones.Spine);
                    avatarBonePoint[24].Transform = animator.GetBoneTransform(HumanBodyBones.Hips);
                    avatarBonePoint[25].Transform = animator.GetBoneTransform(HumanBodyBones.Head);
                    avatarBonePoint[26].Transform = animator.GetBoneTransform(HumanBodyBones.Neck);
                    avatarBonePoint[27].Transform = animator.GetBoneTransform(HumanBodyBones.Spine);
                    avatarBonePoint[31].Transform = animator.GetBoneTransform(HumanBodyBones.Chest);
                    if (avatarBonePoint[31].Transform == null)
                    {
                        avatarBonePoint[31].Enabled = false;
                    }

                    Transform transform = avatarBonePoint[24].Transform;
                    Transform transform2 = avatarBonePoint[15].Transform;
                    if (transform.position.y <= transform2.position.y && Mathf.Abs(transform.position.y - transform2.position.y) < 0.1f)
                    {
                        transform2.position = new Vector3(transform2.position.x, transform.position.y - 0.01f, transform2.position.z);
                    }

                    Transform transform3 = avatarBonePoint[19].Transform;
                    if (transform.position.y <= transform3.position.y && Mathf.Abs(transform.position.y - transform3.position.y) < 0.1f)
                    {
                        transform3.position = new Vector3(transform3.position.x, transform.position.y - 0.01f, transform3.position.z);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                    ErrorMessage = "Failed to set the bone.\r\n" + ex.Message;
                    return false;
                }

                avatarBonePoint[0].Child = avatarBonePoint[1];
                avatarBonePoint[1].Child = avatarBonePoint[2];
                if (avatarBonePoint[29].Enabled)
                {
                    avatarBonePoint[29].Child = avatarBonePoint[0];
                }

                avatarBonePoint[5].Child = avatarBonePoint[6];
                avatarBonePoint[6].Child = avatarBonePoint[7];
                if (avatarBonePoint[30].Enabled)
                {
                    avatarBonePoint[30].Child = avatarBonePoint[5];
                }

                avatarBonePoint[15].Child = avatarBonePoint[16];
                avatarBonePoint[16].Child = avatarBonePoint[17];
                avatarBonePoint[17].Child = avatarBonePoint[18];
                avatarBonePoint[17].Parent = avatarBonePoint[16];
                avatarBonePoint[19].Child = avatarBonePoint[20];
                avatarBonePoint[20].Child = avatarBonePoint[21];
                avatarBonePoint[21].Child = avatarBonePoint[22];
                avatarBonePoint[21].Parent = avatarBonePoint[20];
                if (avatarBonePoint[31].Enabled)
                {
                    avatarBonePoint[27].Child = avatarBonePoint[31];
                    avatarBonePoint[31].Child = avatarBonePoint[26];
                }
                else
                {
                    avatarBonePoint[27].Child = avatarBonePoint[26];
                }

                avatarBonePoint[26].Child = avatarBonePoint[25];
                try
                {
                    Vector3 forward = TriangleNormal(avatarBonePoint[24].Transform.position, avatarBonePoint[19].Transform.position, avatarBonePoint[15].Transform.position);
                    Vector(avatarBonePoint[26].Transform.position, avatarBonePoint[23].Transform.position);
                    BonePoint[] array = avatarBonePoint;
                    foreach (BonePoint bonePoint in array)
                    {
                        if (bonePoint.Transform != null)
                        {
                            bonePoint.InitRotation = bonePoint.Transform.rotation;
                            bonePoint.InitLocalRotation = bonePoint.Transform.localRotation;
                            if (bonePoint.Parent != null && bonePoint.Parent.Transform != null && bonePoint.Child.Transform != null)
                            {
                                Vector3 forward2 = bonePoint.Parent.Transform.position - bonePoint.Transform.position;
                                bonePoint.Inverse = GetInverse(bonePoint, bonePoint.Child, forward2);
                                bonePoint.InverseRotation = bonePoint.Inverse * bonePoint.InitRotation;
                            }
                            else if (bonePoint.Child != null && bonePoint.Child.Transform != null)
                            {
                                bonePoint.Inverse = GetInverse(bonePoint, bonePoint.Child, forward);
                                bonePoint.InverseRotation = bonePoint.Inverse * bonePoint.InitRotation;
                            }
                        }
                    }

                    BonePoint bonePoint2 = avatarBonePoint[24];
                    initPosition = baseObject.transform.position;
                    initPosition = bonePoint2.Transform.localPosition;
                    bonePoint2.Inverse = Quaternion.Inverse(Quaternion.LookRotation(forward));
                    bonePoint2.InverseRotation = bonePoint2.Inverse * bonePoint2.InitRotation;
                    BonePoint bonePoint3 = avatarBonePoint[25];
                    bonePoint3.InitRotation = avatarBonePoint[25].Transform.rotation;
                    Vector3 forward3 = new Vector3(0f, 0f, 0.05f);
                    bonePoint3.Inverse = Quaternion.Inverse(Quaternion.LookRotation(forward3));
                    bonePoint3.InverseRotation = bonePoint3.Inverse * bonePoint3.InitRotation;
                    BonePoint bonePoint4 = avatarBonePoint[7];
                    if (bonePoint4.Enabled)
                    {
                        Vector3 upwards = TriangleNormal(bonePoint4.Transform.position, avatarBonePoint[9].Transform.position, avatarBonePoint[8].Transform.position);
                        bonePoint4.InitRotation = bonePoint4.Transform.rotation;
                        bonePoint4.Inverse = Quaternion.Inverse(Quaternion.LookRotation(bonePoint4.Transform.position - avatarBonePoint[9].Transform.position, upwards));
                        bonePoint4.InverseRotation = bonePoint4.Inverse * bonePoint4.InitRotation;
                        BonePoint bonePoint5 = avatarBonePoint[6];
                        BonePoint bonePoint6 = avatarBonePoint[5];
                        bonePoint5.InitRotation = bonePoint5.Transform.rotation;
                        bonePoint5.Inverse = Quaternion.Inverse(Quaternion.LookRotation(bonePoint5.Transform.position - bonePoint4.Transform.position, upwards));
                        bonePoint5.InverseRotation = bonePoint5.Inverse * bonePoint5.InitRotation;
                        upwards = TriangleNormal(bonePoint6.Transform.position, bonePoint5.Transform.position, avatarBonePoint[23].Transform.position);
                        bonePoint6.InitRotation = bonePoint6.Transform.rotation;
                        bonePoint6.Inverse = Quaternion.Inverse(Quaternion.LookRotation(bonePoint6.Transform.position - bonePoint4.Transform.position, upwards));
                        bonePoint6.InverseRotation = bonePoint6.Inverse * bonePoint6.InitRotation;
                    }

                    BonePoint bonePoint7 = avatarBonePoint[2];
                    if (bonePoint7.Enabled)
                    {
                        Vector3 upwards2 = TriangleNormal(bonePoint7.Transform.position, avatarBonePoint[4].Transform.position, avatarBonePoint[3].Transform.position);
                        bonePoint7.InitRotation = bonePoint7.Transform.rotation;
                        bonePoint7.Inverse = Quaternion.Inverse(Quaternion.LookRotation(bonePoint7.Transform.position - avatarBonePoint[4].Transform.position, upwards2));
                        bonePoint7.InverseRotation = bonePoint7.Inverse * bonePoint7.InitRotation;
                        BonePoint bonePoint8 = avatarBonePoint[1];
                        BonePoint bonePoint9 = avatarBonePoint[0];
                        bonePoint8.InitRotation = bonePoint8.Transform.rotation;
                        bonePoint8.Inverse = Quaternion.Inverse(Quaternion.LookRotation(bonePoint8.Transform.position - bonePoint7.Transform.position, upwards2));
                        bonePoint8.InverseRotation = bonePoint8.Inverse * bonePoint8.InitRotation;
                        upwards2 = TriangleNormal(bonePoint9.Transform.position, bonePoint8.Transform.position, avatarBonePoint[23].Transform.position);
                        bonePoint9.InitRotation = bonePoint9.Transform.rotation;
                        bonePoint9.Inverse = Quaternion.Inverse(Quaternion.LookRotation(bonePoint9.Transform.position - bonePoint7.Transform.position, upwards2));
                        bonePoint9.InverseRotation = bonePoint9.Inverse * bonePoint9.InitRotation;
                    }
                }
                catch (Exception ex2)
                {
                    Debug.Log(ex2);
                    ErrorMessage = "Failed to set the bone.\r\n" + ex2.Message;
                    return false;
                }

                return true;
            }

            ErrorMessage = "Animator is not human.";
            return false;
        }

        private void SetInitTPose()
        {
            baseObject.transform.position = initPosition;
            initTPose(24);
            initTPose(27);
            if (avatarBonePoint[31].Enabled)
            {
                initTPose(31);
            }

            initTPose(26);
            initTPose(25);
            if (avatarBonePoint[30].Enabled)
            {
                initTPose(30);
            }

            if (avatarBonePoint[29].Enabled)
            {
                initTPose(29);
            }

            initTPose(5);
            initTPose(6);
            initTPose(7);
            initTPose(0);
            initTPose(1);
            initTPose(2);
            initTPose(19);
            initTPose(20);
            initTPose(21);
            initTPose(15);
            initTPose(16);
            initTPose(17);
        }

        private void initTPose(int index)
        {
            if (avatarBonePoint[index].Transform != null)
            {
                avatarBonePoint[index].Transform.rotation = avatarBonePoint[index].InitRotation;
            }
        }

        public void ResetCalibration()
        {
            pitchdeglower = 0f;
            pitchdegupper = 0f;
            yawdeg = 0f;
        }

        public void PoseUpdate()
        {
            if (resuming)
            {
                SendTimeLeft -= Time.deltaTime;
                if (SendTimeLeft < 0f)
                {
                    resuming = false;
                    SendTimeLeft = 10f;
                    Debug.Log("Stop");
                }

                _ = Time.time;
                if (calibrate)
                {
                    Vector3 vector = (WorldPoseLandmarks[11] + WorldPoseLandmarks[12]) / 2f;
                    Vector3 vector2 = (WorldPoseLandmarks[23] + WorldPoseLandmarks[24]) / 2f;
                    _ = WorldPoseLandmarks[12] - WorldPoseLandmarks[11];
                    pitchdeg = Vector3.SignedAngle(Vector3.up, vector - vector2, Vector3.left) + 180f;
                    pitchdeglower = pitchdeg + 10f;
                    pitchdegupper = pitchdeg + 5f;
                    yawdeg = 180f - Vector3.SignedAngle(Vector3.right, WorldPoseLandmarks[12] - WorldPoseLandmarks[11], Vector3.up);
                    calibrate = false;
                }

                if (validInput)
                {
                    for (int i = 0; i < 23; i++)
                    {
                        WorldPoseLandmarks[i] = rotatePointAroundAxis(rotatePointAroundAxis(WorldPoseLandmarks[i], pitchdegupper, Vector3.right), yawdeg, Vector3.up);
                    }

                    for (int j = 23; j < 33; j++)
                    {
                        WorldPoseLandmarks[j] = rotatePointAroundAxis(rotatePointAroundAxis(WorldPoseLandmarks[j], pitchdeglower, Vector3.right), yawdeg, Vector3.up);
                    }

                    for (int k = 0; k < 33; k++)
                    {
                        WorldPoseLandmarks[k] = fm.UpdatePos(0, k, WorldPoseLandmarks[k]);
                    }

                    for (int l = 0; l < 4; l++)
                    {
                        faceLandmarks[facelandmarkInUse[l]] = fm.UpdatePos(-1, facelandmarkInUse[l], faceLandmarks[facelandmarkInUse[l]]);
                    }
                }

                avatarBonePoint[0].Pos3D = Utils.scaling(WorldPoseLandmarks[12]);
                avatarBonePoint[0].Score3D = WorldPoseVisibilities[12];
                avatarBonePoint[1].Pos3D = Utils.scaling(WorldPoseLandmarks[14]);
                avatarBonePoint[1].Score3D = WorldPoseVisibilities[14];
                avatarBonePoint[2].Pos3D = Utils.scaling(WorldPoseLandmarks[16]);
                avatarBonePoint[2].Score3D = WorldPoseVisibilities[16];
                avatarBonePoint[3].Pos3D = Utils.scaling(WorldPoseLandmarks[22]);
                avatarBonePoint[3].Score3D = WorldPoseVisibilities[22];
                avatarBonePoint[4].Pos3D = Utils.scaling(WorldPoseLandmarks[18]);
                avatarBonePoint[4].Score3D = WorldPoseVisibilities[18];
                avatarBonePoint[5].Pos3D = Utils.scaling(WorldPoseLandmarks[11]);
                avatarBonePoint[5].Score3D = WorldPoseVisibilities[11];
                avatarBonePoint[6].Pos3D = Utils.scaling(WorldPoseLandmarks[13]);
                avatarBonePoint[6].Score3D = WorldPoseVisibilities[13];
                avatarBonePoint[7].Pos3D = Utils.scaling(WorldPoseLandmarks[15]);
                avatarBonePoint[7].Score3D = WorldPoseVisibilities[15];
                avatarBonePoint[8].Pos3D = Utils.scaling(WorldPoseLandmarks[21]);
                avatarBonePoint[8].Score3D = WorldPoseVisibilities[21];
                avatarBonePoint[9].Pos3D = Utils.scaling(WorldPoseLandmarks[17]);
                avatarBonePoint[9].Score3D = WorldPoseVisibilities[17];
                avatarBonePoint[10].Pos3D = Utils.scaling(WorldPoseLandmarks[7]);
                avatarBonePoint[10].Score3D = WorldPoseVisibilities[7];
                avatarBonePoint[11].Pos3D = Utils.scaling(WorldPoseLandmarks[2]);
                avatarBonePoint[11].Score3D = WorldPoseVisibilities[2];
                avatarBonePoint[12].Pos3D = Utils.scaling(WorldPoseLandmarks[8]);
                avatarBonePoint[12].Score3D = WorldPoseVisibilities[8];
                avatarBonePoint[13].Pos3D = Utils.scaling(WorldPoseLandmarks[5]);
                avatarBonePoint[13].Score3D = WorldPoseVisibilities[5];
                avatarBonePoint[14].Pos3D = Vector3.Lerp((Utils.scaling(WorldPoseLandmarks[2]) + Utils.scaling(WorldPoseLandmarks[5])) / 2f, Utils.scaling(WorldPoseLandmarks[0]), 0.7f);
                avatarBonePoint[14].Score3D = WorldPoseVisibilities[0];
                avatarBonePoint[15].Pos3D = Utils.scaling(WorldPoseLandmarks[24]);
                avatarBonePoint[15].Score3D = WorldPoseVisibilities[24];
                avatarBonePoint[16].Pos3D = Utils.scaling(WorldPoseLandmarks[26]);
                avatarBonePoint[16].Score3D = WorldPoseVisibilities[26];
                avatarBonePoint[17].Pos3D = Utils.scaling(WorldPoseLandmarks[28]);
                avatarBonePoint[17].Score3D = WorldPoseVisibilities[28];
                avatarBonePoint[18].Pos3D = Utils.scaling(WorldPoseLandmarks[32]);
                avatarBonePoint[18].Score3D = WorldPoseVisibilities[32];
                avatarBonePoint[19].Pos3D = Utils.scaling(WorldPoseLandmarks[23]);
                avatarBonePoint[19].Score3D = WorldPoseVisibilities[23];
                avatarBonePoint[20].Pos3D = Utils.scaling(WorldPoseLandmarks[25]);
                avatarBonePoint[20].Score3D = WorldPoseVisibilities[25];
                avatarBonePoint[21].Pos3D = Utils.scaling(WorldPoseLandmarks[27]);
                avatarBonePoint[21].Score3D = WorldPoseVisibilities[27];
                avatarBonePoint[22].Pos3D = Utils.scaling(WorldPoseLandmarks[31]);
                avatarBonePoint[22].Score3D = WorldPoseVisibilities[31];
                Vector3 a = Vector3.Lerp(Utils.scaling(WorldPoseLandmarks[23]), Utils.scaling(WorldPoseLandmarks[24]), 0.5f);
                Vector3 b = Vector3.Lerp(Utils.scaling(WorldPoseLandmarks[11]), Utils.scaling(WorldPoseLandmarks[12]), 0.5f);
                avatarBonePoint[23].Pos3D = Vector3.Lerp(a, b, 0.4f);
                avatarBonePoint[23].Score3D = WorldPoseVisibilities[23];
                CalculateAvatarBones();
                for (int m = 0; m < 24; m++)
                {
                    avatarBonePoint[m].Visibled = ((!(avatarBonePoint[m].Score3D < VisibleThreshold)) ? true : false);
                }

                avatarBonePoint[25].Visibled = ((avatarBonePoint[11].Visibled && avatarBonePoint[13].Visibled) ? true : false);
                avatarBonePoint[26].Visibled = ((avatarBonePoint[5].Visibled && avatarBonePoint[0].Visibled) ? true : false);
                avatarBonePoint[27].Visibled = avatarBonePoint[23].Visibled;
                avatarBonePoint[24].Visibled = avatarBonePoint[23].Visibled;
                avatarBonePoint[31].Visibled = avatarBonePoint[23].Visibled;
                avatarBonePoint[28].Visibled = avatarBonePoint[23].Visibled;
                avatarBonePoint[30].Visibled = avatarBonePoint[5].Visibled;
                avatarBonePoint[29].Visibled = avatarBonePoint[0].Visibled;
                if (avatarBonePoint[10].Visibled && avatarBonePoint[12].Visibled)
                {
                    tallHead = Vector3.Distance(avatarBonePoint[10].Pos3D, avatarBonePoint[12].Pos3D);
                }
                else
                {
                    tallHead = prevTallHead;
                }

                if (avatarBonePoint[25].Visibled && avatarBonePoint[26].Visibled)
                {
                    tallHeadNeck = Vector3.Distance(avatarBonePoint[25].Pos3D, avatarBonePoint[26].Pos3D);
                }
                else
                {
                    tallHeadNeck = prevTallHeadNeck;
                }

                if (avatarBonePoint[26].Visibled && avatarBonePoint[27].Visibled)
                {
                    tallNeckSpine = Vector3.Distance(avatarBonePoint[26].Pos3D, avatarBonePoint[27].Pos3D);
                }
                else
                {
                    tallNeckSpine = prevTallNeckSpine;
                }

                float num = 0f;
                float num2 = 0f;
                num = ((!avatarBonePoint[20].Visibled || !avatarBonePoint[21].Visibled || avatarBonePoint[20].Lock || avatarBonePoint[21].Lock) ? prevTallThigh : Vector3.Distance(avatarBonePoint[20].Pos3D, avatarBonePoint[21].Pos3D));
                num2 = ((!avatarBonePoint[16].Visibled || !avatarBonePoint[17].Visibled || avatarBonePoint[16].Lock || avatarBonePoint[17].Lock) ? prevTallThigh : Vector3.Distance(avatarBonePoint[16].Pos3D, avatarBonePoint[17].Pos3D));
                tallShin = (num2 + num) / 2f;
                float num3 = 0f;
                float num4 = 0f;
                num3 = ((!avatarBonePoint[15].Visibled || !avatarBonePoint[16].Visibled || avatarBonePoint[15].Lock || avatarBonePoint[16].Lock) ? prevTallThigh : Vector3.Distance(avatarBonePoint[15].Pos3D, avatarBonePoint[16].Pos3D));
                num4 = ((!avatarBonePoint[19].Visibled || !avatarBonePoint[20].Visibled || avatarBonePoint[19].Lock || avatarBonePoint[20].Lock) ? prevTallThigh : Vector3.Distance(avatarBonePoint[19].Pos3D, avatarBonePoint[20].Pos3D));
                tallThigh = (num3 + num4) / 2f;
                if (avatarBonePoint[15].Visibled && avatarBonePoint[19].Visibled && avatarBonePoint[27].Visibled && !avatarBonePoint[15].Lock && !avatarBonePoint[19].Lock)
                {
                    Vector3 b2 = (avatarBonePoint[15].Pos3D + avatarBonePoint[19].Pos3D) / 2f;
                    tallSpineCrotch = Vector3.Distance(avatarBonePoint[27].Pos3D, b2);
                }
                else
                {
                    tallSpineCrotch = prevTallSpineCrotch;
                }

                float num5 = tallHeadNeck + tallNeckSpine + tallSpineCrotch + (tallThigh + tallShin);
                tall = num5 * 0.5f + prevTall * 0.5f;
                _ = tall / centerTall;
                _ = DistanceToPerson;
                prevTall = tall;
                prevTallHead = tallHead * 0.3f + prevTallHead * 0.7f;
                prevTallHeadNeck = tallHeadNeck * 0.3f + prevTallHeadNeck * 0.7f;
                prevTallNeckSpine = prevTallNeckSpine * 0.3f + tallNeckSpine * 0.7f;
                prevTallSpineCrotch = prevTallSpineCrotch * 0.3f + tallSpineCrotch * 0.7f;
                prevTallThigh = prevTallThigh * 0.3f + tallThigh * 0.7f;
                prevTallShin = prevTallShin * 0.3f + tallShin * 0.7f;
                float num6 = 0f;
                int num7 = 0;
                num6 += avatarBonePoint[11].Score3D;
                num6 += avatarBonePoint[13].Score3D;
                num6 += avatarBonePoint[10].Score3D;
                num6 += avatarBonePoint[12].Score3D;
                num6 += avatarBonePoint[14].Score3D;
                num7 += 5;
                num6 += avatarBonePoint[5].Score3D;
                num6 += avatarBonePoint[0].Score3D;
                num6 += avatarBonePoint[23].Score3D;
                num7 += 3;
                if (!PoorLowerBodyMode)
                {
                    num6 += avatarBonePoint[19].Score3D;
                    num7++;
                    num6 += avatarBonePoint[15].Score3D;
                    num7++;
                    if (!avatarBonePoint[20].Lock)
                    {
                        num6 += avatarBonePoint[20].Score3D;
                        num7++;
                    }

                    if (!avatarBonePoint[16].Lock)
                    {
                        num6 += avatarBonePoint[16].Score3D;
                        num7++;
                    }

                    if (!avatarBonePoint[21].Lock)
                    {
                        num6 += avatarBonePoint[21].Score3D;
                        num7++;
                    }

                    if (!avatarBonePoint[17].Lock)
                    {
                        num6 += avatarBonePoint[17].Score3D;
                        num7++;
                    }

                    if (!avatarBonePoint[22].Lock)
                    {
                        num6 += avatarBonePoint[22].Score3D;
                        num7++;
                    }

                    if (!avatarBonePoint[18].Lock)
                    {
                        num6 += avatarBonePoint[18].Score3D;
                        num7++;
                    }

                    EstimatedScore = num6 / (float)num7;
                    if (EstimatedScore < EstimatedThreshold)
                    {
                        return;
                    }
                }
                else
                {
                    EstimatedScore = num6 / (float)num7;
                    if (EstimatedScore < EstimatedThreshold)
                    {
                        return;
                    }
                }

                BonePoint bonePoint = avatarBonePoint[24];
                forwardUpperVec = TriangleNormal(avatarBonePoint[23].Pos3D, avatarBonePoint[0].Pos3D, avatarBonePoint[5].Pos3D);
                forwardLowerVec = TriangleNormal(avatarBonePoint[23].Pos3D, avatarBonePoint[19].Pos3D, avatarBonePoint[15].Pos3D);
                upperVec = Vector(26, 23);
                rightUpperVec = Vector3.Cross(upperVec, forwardUpperVec);
                downVec = Vector(28, 23);
                rightLowerVec = Vector3.Cross(forwardLowerVec, downVec);
                if (forwardLowerVec != Vector3.zero)
                {
                    bonePoint.Transform.rotation = Quaternion.LookRotation(forwardLowerVec, -downVec) * bonePoint.InverseRotation;
                }

                if (Vector3.Angle(rightUpperVec, rightLowerVec) < 100f && Vector3.Angle(upperVec, downVec) > 10f)
                {
                    if (avatarBonePoint[31].Enabled)
                    {
                        LookAt(27, 31, forwardUpperVec);
                        Vector3 upwords = TriangleNormal(avatarBonePoint[31].Pos3D, avatarBonePoint[0].Pos3D, avatarBonePoint[5].Pos3D);
                        LookAt(31, 26, upwords);
                    }
                    else
                    {
                        LookAt(27, 26, forwardUpperVec);
                    }

                    Vector3 vector3 = TriangleNormal(avatarBonePoint[14].Pos3D, avatarBonePoint[12].Pos3D, avatarBonePoint[10].Pos3D);
                    _ = avatarBonePoint[26].Pos3D - avatarBonePoint[23].Pos3D;
                    if (Vector3.Angle(vector3, upperVec) < 45f)
                    {
                        LookAt(26, 25, forwardUpperVec);
                        BonePoint bonePoint2 = avatarBonePoint[25];
                        Vector3 vector4 = avatarBonePoint[14].Pos3D - bonePoint2.Pos3D;
                        if (Vector3.Angle(vector4, forwardUpperVec) < 60f)
                        {
                            bonePoint2.Transform.rotation = Quaternion.LookRotation(vector4, vector3) * bonePoint2.InverseRotation;
                        }
                    }

                    BonePoint bonePoint3 = avatarBonePoint[7];
                    if (bonePoint3.Enabled)
                    {
                        if (avatarBonePoint[30].Enabled)
                        {
                            LookAt(30, 5, forwardUpperVec);
                        }

                        Vector3 vector5 = TriangleNormal(bonePoint3.Pos3D, avatarBonePoint[9].Pos3D, avatarBonePoint[8].Pos3D);
                        if (GetVectorAngle(5, 6, 23) > 5f)
                        {
                            lShldrBendF = TriangleNormal(5, 6, 23);
                        }

                        LookAt(5, 6, lShldrBendF);
                        float vectorAngle = GetVectorAngle(6, 7, 5);
                        if (vectorAngle > 5f)
                        {
                            lForearmBendF = TriangleNormal(6, 7, 5);
                        }

                        if (vectorAngle < 20f)
                        {
                            LookAt(6, 7, vector5);
                        }
                        else if (vectorAngle < 90f)
                        {
                            float num8 = (vectorAngle - 20f) / 70f;
                            Vector3 upwords2 = vector5 * (1f - num8) + lForearmBendF * num8;
                            LookAt(6, 7, upwords2);
                        }
                        else
                        {
                            LookAt(6, 7, lForearmBendF);
                        }

                        LookAt(7, 9, vector5);
                    }
                    else
                    {
                        LookAt(5, 6, forwardUpperVec);
                        LookAt(6, 7, forwardUpperVec);
                    }

                    BonePoint bonePoint4 = avatarBonePoint[2];
                    if (bonePoint4.Enabled)
                    {
                        if (avatarBonePoint[29].Enabled)
                        {
                            LookAt(29, 0, forwardUpperVec);
                        }

                        Vector3 vector6 = TriangleNormal(bonePoint4.Pos3D, avatarBonePoint[4].Pos3D, avatarBonePoint[3].Pos3D);
                        if (GetVectorAngle(0, 1, 23) > 5f)
                        {
                            rShldrBendF = TriangleNormal(0, 1, 23);
                        }

                        LookAt(0, 1, rShldrBendF);
                        float vectorAngle2 = GetVectorAngle(1, 2, 0);
                        if (vectorAngle2 > 5f)
                        {
                            rForearmBendF = TriangleNormal(1, 2, 0);
                        }

                        if (vectorAngle2 < 20f)
                        {
                            LookAt(1, 2, vector6);
                        }
                        else if (vectorAngle2 < 90f)
                        {
                            float num9 = (vectorAngle2 - 20f) / 70f;
                            Vector3 upwords3 = vector6 * (1f - num9) + rForearmBendF * num9;
                            LookAt(1, 2, upwords3);
                        }
                        else
                        {
                            LookAt(1, 2, rForearmBendF);
                        }

                        LookAt(2, 4, vector6);
                    }
                    else
                    {
                        LookAt(0, 1, forwardUpperVec);
                        LookAt(1, 2, forwardUpperVec);
                    }
                }

                lefthand.rotFromArm = lelbow.rotation;
                righthand.rotFromArm = relbow.rotation;
                Vector3 vector7 = (Utils.scalingface(faceLandmarks[21]) + Utils.scalingface(faceLandmarks[251])) / 2f - (Utils.scalingface(faceLandmarks[172]) + Utils.scalingface(faceLandmarks[397])) / 2f;
                Vector3 forward = TriangleNormal(Utils.scalingface(faceLandmarks[21]), Utils.scalingface(faceLandmarks[251]), (Utils.scalingface(faceLandmarks[172]) + Utils.scalingface(faceLandmarks[397])) / 2f);
                if (vector7 != Vector3.zero)
                {
                    Quaternion b3 = Quaternion.LookRotation(forward, vector7);
                    float t = Utils.Remap(Vector3.Distance(Utils.scalingface(faceLandmarks[21]), Utils.scalingface(faceLandmarks[172])), 3f, 8f);
                    Quaternion b4 = Quaternion.Lerp(avatarBonePoint[25].Transform.rotation, b3, t);
                    neck.rotation = Quaternion.Lerp(Quaternion.Euler(0f, 0f, 0f), b4, 0.3f);
                    head.rotation = Quaternion.Lerp(Quaternion.Euler(0f, 0f, 0f), b4, 0.7f);
                }

                if (!PoorLowerBodyMode)
                {
                    LegRotate(19, 20, 21, 22);
                    LegRotate(15, 16, 17, 18);
                }

                if (LowerBodyVisible() && mocapmode != MoCapMode.upperbody)
                {
                    Vector3 vector8 = (PoseLandmarks[23] + PoseLandmarks[24]) / 2f;
                    if (mocapmode == MoCapMode.flat)
                    {
                        root.localPosition = new Vector3((baseX - vector8.x) * spanX * 0.8f * hipHeight, 0.8787938f, (vector8.y - baseY) * spanZ * 0.8f * hipHeight + 0.002470553f);
                    }
                    else
                    {
                        root.localPosition = new Vector3((baseX - vector8.x) * 2f, (baseY - vector8.y) * 1.6f + 0.8787938f, 0.002470553f);
                    }
                }
                else
                {
                    root.localPosition = new Vector3(0f, 0.8787938f, 0.002470553f);
                    lhip.rotation = Quaternion.Euler(0f, 0f, 0f);
                    lknee.rotation = Quaternion.Euler(0f, 0f, 0f);
                    lfoot.rotation = Quaternion.Euler(0f, 0f, 0f);
                    ltoe.rotation = Quaternion.Euler(0f, 0f, 0f);
                    rhip.rotation = Quaternion.Euler(0f, 0f, 0f);
                    rknee.rotation = Quaternion.Euler(0f, 0f, 0f);
                    rfoot.rotation = Quaternion.Euler(0f, 0f, 0f);
                    rtoe.rotation = Quaternion.Euler(0f, 0f, 0f);
                }

                validInput = false;
            }
            else
            {
                SendTimeLeft -= Time.deltaTime;
                if (SendTimeLeft < 0f)
                {
                    resuming = true;
                    SendTimeLeft = 60f;
                    Debug.Log("Resume");
                }
            }
        }

        private void CalculateAvatarBones()
        {
            avatarBonePoint[28].Pos3D = (avatarBonePoint[15].Pos3D + avatarBonePoint[19].Pos3D) / 2f;
            avatarBonePoint[24].Pos3D = (avatarBonePoint[23].Pos3D + avatarBonePoint[28].Pos3D) / 2f;
            Vector3 forward = TriangleNormal(avatarBonePoint[24].Pos3D, avatarBonePoint[19].Pos3D, avatarBonePoint[15].Pos3D);
            Vector3 rhs = TriangleNormal(avatarBonePoint[23].Pos3D, avatarBonePoint[0].Pos3D, avatarBonePoint[5].Pos3D);
            Vector3 vector = avatarBonePoint[23].Pos3D - avatarBonePoint[28].Pos3D;
            Vector3 lhs = (avatarBonePoint[0].Pos3D + avatarBonePoint[5].Pos3D) / 2f - avatarBonePoint[23].Pos3D;
            if (avatarBonePoint[5].Pos3D.y > -56f && avatarBonePoint[0].Pos3D.y > -56f && avatarBonePoint[19].Pos3D.y < -112f && avatarBonePoint[15].Pos3D.y < -112f)
            {
                PoorLowerBodyMode = true;
            }
            else
            {
                PoorLowerBodyMode = false;
            }

            CheckLeg(19, 20, 21, 5, forward);
            CheckLeg(15, 16, 17, 0, forward);
            if (avatarBonePoint[30].Enabled && avatarBonePoint[29].Enabled)
            {
                Vector3 vector2 = (avatarBonePoint[12].Pos3D + avatarBonePoint[10].Pos3D) / 2f;
                Vector3 vector3 = (avatarBonePoint[0].Pos3D + avatarBonePoint[5].Pos3D) / 2f;
                Vector3 value = vector2 - avatarBonePoint[23].Pos3D;
                Vector3 rhs2 = vector3 - avatarBonePoint[23].Pos3D;
                Vector3 vector4 = Vector3.Normalize(value);
                Vector3 vector5 = avatarBonePoint[23].Pos3D + vector4 * Vector3.Dot(vector4, rhs2);
                Vector3 vector6 = (vector3 - vector5) / 2f;
                avatarBonePoint[26].Pos3D = vector5 + vector6;
                avatarBonePoint[25].Pos3D = vector2;
                float num = (avatarBonePoint[0].Pos3D - avatarBonePoint[5].Pos3D).magnitude / 4f;
                Vector3 normalized = Vector3.Cross(lhs, rhs).normalized;
                avatarBonePoint[29].Pos3D = avatarBonePoint[26].Pos3D + normalized * num;
                avatarBonePoint[30].Pos3D = avatarBonePoint[26].Pos3D - normalized * num;
            }
            else
            {
                avatarBonePoint[26].Pos3D = (avatarBonePoint[0].Pos3D + avatarBonePoint[5].Pos3D) / 2f;
                avatarBonePoint[25].Pos3D = (avatarBonePoint[12].Pos3D + avatarBonePoint[10].Pos3D) / 2f;
                avatarBonePoint[29].Pos3D = (avatarBonePoint[26].Pos3D + avatarBonePoint[0].Pos3D) / 2f;
                avatarBonePoint[30].Pos3D = (avatarBonePoint[26].Pos3D + avatarBonePoint[5].Pos3D) / 2f;
            }

            avatarBonePoint[27].Pos3D = avatarBonePoint[23].Pos3D;
            lhs = avatarBonePoint[26].Pos3D - avatarBonePoint[23].Pos3D;
            Vector3 vector7 = (vector.normalized + lhs.normalized * 2f).normalized * (lhs / 2f).magnitude;
            avatarBonePoint[31].Pos3D = avatarBonePoint[23].Pos3D + vector7;
        }

        private void CheckLeg(int piThighBend, int piShin, int piFoot, int piShldrBend, Vector3 forward)
        {
            BonePoint bonePoint = avatarBonePoint[piThighBend];
            BonePoint bonePoint2 = avatarBonePoint[piShin];
            BonePoint bonePoint3 = avatarBonePoint[piFoot];
            BonePoint child = bonePoint3.Child;
            BonePoint bonePoint4 = avatarBonePoint[piShldrBend];
            if (bonePoint2.Score3D < FootCheckThreshold && bonePoint.Score3D > FootCheckThreshold)
            {
                Vector3 vector = bonePoint.Pos3D - bonePoint4.Pos3D;
                if ((bonePoint.Pos3D + vector).y < bottomThreshold)
                {
                    child.Lock = true;
                    bonePoint3.Lock = true;
                    bonePoint2.Lock = true;
                    bonePoint.Lock = true;
                }
                else
                {
                    child.Lock = LockFoot || LockLegs;
                    bonePoint3.Lock = LockFoot || LockLegs;
                    bonePoint2.Lock = LockLegs;
                    bonePoint.Lock = LockLegs;
                }

                return;
            }

            child.Lock = LockFoot || LockLegs;
            bonePoint3.Lock = LockFoot || LockLegs;
            bonePoint2.Lock = LockLegs;
            bonePoint.Lock = LockLegs;
            if (bonePoint3.Score3D < FootCheckThreshold && bonePoint2.Score3D > FootCheckThreshold)
            {
                Vector3 vector2 = bonePoint2.Pos3D - bonePoint.Pos3D;
                if ((bonePoint2.Pos3D + vector2).y < bottomThreshold)
                {
                    child.Lock = true;
                    bonePoint3.Lock = true;
                    bonePoint2.Lock = true;
                }
                else
                {
                    child.Lock = LockFoot || LockLegs;
                    bonePoint3.Lock = LockFoot || LockLegs;
                    bonePoint2.Lock = LockLegs;
                }
            }

            if (child != null && child.Score3D < FootCheckThreshold && bonePoint3.Score3D > FootCheckThreshold)
            {
                Vector3 vector3 = bonePoint2.Pos3D - bonePoint.Pos3D;
                if ((bonePoint2.Pos3D + vector3).y < bottomThreshold)
                {
                    child.Lock = true;
                    bonePoint3.Lock = true;
                }
                else
                {
                    child.Lock = LockFoot || LockLegs;
                    bonePoint3.Lock = LockFoot || LockLegs;
                }
            }
        }

        private void LegRotate(int thighBend, int shin, int foot, int toe)
        {
            if (!avatarBonePoint[thighBend].Lock)
            {
                Vector3 pos3D = avatarBonePoint[thighBend].Pos3D;
                Vector3 pos3D2 = avatarBonePoint[shin].Pos3D;
                float num = 0f - (rightLowerVec.x * pos3D.x + rightLowerVec.y * pos3D.y + rightLowerVec.z * pos3D.z);
                float num2 = (0f - (rightLowerVec.x * pos3D2.x + rightLowerVec.y * pos3D2.y + rightLowerVec.z * pos3D2.z + num)) / (rightLowerVec.x * rightLowerVec.x + rightLowerVec.y * rightLowerVec.y + rightLowerVec.z * rightLowerVec.z);
                Vector3 upwords = Vector3.Cross(pos3D2 + num2 * rightLowerVec - pos3D, rightLowerVec);
                LookAt(thighBend, shin, upwords);
                if (!avatarBonePoint[shin].Lock)
                {
                    float vectorAngle = GetVectorAngle(shin, foot, thighBend);
                    if (vectorAngle < 20f)
                    {
                        LookAt(shin, foot, forwardLowerVec);
                    }
                    else if (vectorAngle >= 20f && vectorAngle < 40f)
                    {
                        float num3 = (vectorAngle - 20f) / 20f;
                        Vector3 upwords2 = forwardLowerVec * (1f - num3) + (Vector(shin, foot) + Vector(shin, thighBend)) * num3;
                        LookAt(shin, foot, upwords2);
                    }
                    else
                    {
                        LookAt(shin, foot, Vector(shin, foot) + Vector(shin, thighBend));
                    }

                    if (!avatarBonePoint[foot].Lock)
                    {
                        LookAt(foot, toe, Vector(shin, foot));
                    }
                    else
                    {
                        SetDefaultRotation(foot, toe, forwardLowerVec);
                    }
                }
                else
                {
                    SetDefaultRotation(shin, foot, forwardLowerVec);
                    SetDefaultRotation(foot, toe, forwardLowerVec);
                }
            }
            else
            {
                SetDefaultRotation(thighBend, shin, forwardLowerVec);
                SetDefaultRotation(shin, foot, forwardLowerVec);
                SetDefaultRotation(foot, toe, forwardLowerVec);
            }
        }

        private void SetDefaultRotation(int root, int child, Vector3 forward)
        {
            if (avatarBonePoint[root].Transform != null)
            {
                avatarBonePoint[root].Transform.localRotation = Quaternion.Lerp(avatarBonePoint[root].Transform.localRotation, avatarBonePoint[root].InitLocalRotation, 0.05f);
            }
        }

        private void LookAt(int index, int childIndex, Vector3 upwords)
        {
            BonePoint bonePoint = avatarBonePoint[index];
            if (bonePoint.Transform != null)
            {
                BonePoint bonePoint2 = avatarBonePoint[childIndex];
                bonePoint.Transform.rotation = Quaternion.LookRotation(bonePoint.Pos3D - bonePoint2.Pos3D, upwords) * bonePoint.InverseRotation;
                Quaternion.Slerp(Quaternion.LookRotation(bonePoint.Pos3D - bonePoint2.Pos3D, upwords) * bonePoint.InverseRotation, bonePoint.Transform.rotation, Time.deltaTime);
            }
        }

        private Vector3 Vector(Vector3 a, Vector3 b)
        {
            return a - b;
        }

        private Vector3 Vector(int a, int b)
        {
            return avatarBonePoint[a].Pos3D - avatarBonePoint[b].Pos3D;
        }

        private float GetVectorAngle(int a, int b, int c)
        {
            return Vector3.Angle(Vector(a, b), Vector(c, a));
        }

        private Vector3 TriangleNormal(int a, int b, int c)
        {
            return TriangleNormal(avatarBonePoint[a].Pos3D, avatarBonePoint[b].Pos3D, avatarBonePoint[c].Pos3D);
        }

        private Vector3 TriangleNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 lhs = a - b;
            Vector3 rhs = a - c;
            Vector3 result = Vector3.Cross(lhs, rhs);
            result.Normalize();
            return result;
        }

        private Quaternion GetInverse(BonePoint p1, BonePoint p2, Vector3 forward)
        {
            _ = p1.Transform.position - p2.Transform.position == Vector3.zero;
            return Quaternion.Inverse(Quaternion.LookRotation(p1.Transform.position - p2.Transform.position, forward));
        }

        public void SetMoCapMode(MoCapMode mode)
        {
            mocapmode = mode;
            if (mocapmode == MoCapMode.upperbody)
            {
                fm.UpdateUpperbodyParams();
            }
            else
            {
                fm.RestoreUpperbodyParams();
            }
        }

        private void calibrateHip(int CalibrateStatus = 0)
        {
            if (LowerBodyVisible())
            {
                Vector3 vector = (PoseLandmarks[23] + PoseLandmarks[24]) / 2f;
                baseX = vector.x;
                baseY = vector.y;
                spanX = 2f;
                spanZ = 5f;
            }
            else
            {
                baseX = 0.5f;
                baseY = 0.5f;
            }
        }

        private bool LowerBodyVisible()
        {
            return (double)(PoseVisibilities[23] + PoseVisibilities[24]) > 1.9;
        }

        private Vector3 rotatePointAroundAxis(Vector3 point, float angle, Vector3 axis)
        {
            return Quaternion.AngleAxis(angle, axis) * point;
        }

        public void Calibrate(int CalibrateStatus = 0)
        {
            if (CalibrateStatus == 0)
            {
                calibrate = true;
            }

            calibrateHip(CalibrateStatus);
        }

        public void InitFilter(int sensitivity)
        {
            fm.init(sensitivity);
        }

        public void SetSensitivity(int sensitivity)
        {
            fm.UpdateParams(sensitivity);
            if (mocapmode == MoCapMode.upperbody)
            {
                fm.UpdateUpperbodyParams();
            }
        }
    }
}