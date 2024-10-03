using UnityEngine;

namespace VRMAvatar
{
    public class AvatarHandLeft : MonoBehaviour
    {
        private Quaternion rot;

        private float lthumb;

        private float lindex;

        private float lmiddle;

        private float lring;

        private float lpinky;

        [HideInInspector]
        public float lerp;

        [HideInInspector]
        public bool received;

        [HideInInspector]
        public bool validInput;

        private Vector3 lthumb1 = new Vector3(-5f, -18f, 30f);

        private Vector3 lthumb2 = new Vector3(0f, -40f, 0f);

        private Vector3 lthumb3 = new Vector3(0f, -20f, 0f);

        private Vector3 lindex1 = new Vector3(0f, 0f, 70f);

        private Vector3 lindex2 = new Vector3(0f, 0f, 70f);

        private Vector3 lindex3 = new Vector3(0f, 0f, 80f);

        private Vector3 lmiddle1 = new Vector3(0f, 0f, 70f);

        private Vector3 lmiddle2 = new Vector3(0f, 0f, 70f);

        private Vector3 lmiddle3 = new Vector3(0f, 0f, 80f);

        private Vector3 lring1 = new Vector3(-5f, 0f, 70f);

        private Vector3 lring2 = new Vector3(0f, 0f, 70f);

        private Vector3 lring3 = new Vector3(0f, 0f, 80f);

        private Vector3 lpinky1 = new Vector3(-10f, 0f, 70f);

        private Vector3 lpinky2 = new Vector3(0f, 0f, 70f);

        private Vector3 lpinky3 = new Vector3(0f, 0f, 80f);

        private Animator animator;

        private Quaternion rotindex;

        private Quaternion rotmid;

        private Quaternion rotring;

        private Quaternion rotpinky;

        private GameObject indexL;

        private GameObject middleL;

        private GameObject ringL;

        private GameObject pinkyL;

        [HideInInspector]
        public Quaternion rotFromArm;

        private FilterManager fm;

        private Transform spawnPoint;

        [HideInInspector]
        public float thumb;

        [HideInInspector]
        public float index;

        [HideInInspector]
        public float middle;

        [HideInInspector]
        public float ring;

        [HideInInspector]
        public float pinky;

        [HideInInspector]
        public float inout;

        [HideInInspector]
        public Vector3[] landmarks = new Vector3[21];

        private Vector3[] filteredlandmarks = new Vector3[21];

        private float l;

        private float depre = 1f;

        private void Start()
        {
            animator = GetComponent<Animator>();
            fm = GetComponent<FilterManager>();
            spawnPoint = Utils.getChildGameObject(base.gameObject, "index1.L").transform;
            indexL = new GameObject("indexL");
            indexL.transform.position = spawnPoint.position;
            indexL.transform.rotation = spawnPoint.rotation;
            indexL.transform.parent = spawnPoint.parent.transform;
            spawnPoint = Utils.getChildGameObject(base.gameObject, "middle1.L").transform;
            middleL = new GameObject("middleL");
            middleL.transform.position = spawnPoint.position;
            middleL.transform.rotation = spawnPoint.rotation;
            middleL.transform.parent = spawnPoint.parent.transform;
            spawnPoint = Utils.getChildGameObject(base.gameObject, "ring1.L").transform;
            ringL = new GameObject("ringL");
            ringL.transform.position = spawnPoint.position;
            ringL.transform.rotation = spawnPoint.rotation;
            ringL.transform.parent = spawnPoint.parent.transform;
            spawnPoint = Utils.getChildGameObject(base.gameObject, "pinky1.L").transform;
            pinkyL = new GameObject("pinkyL");
            pinkyL.transform.position = spawnPoint.position;
            pinkyL.transform.rotation = spawnPoint.rotation;
            pinkyL.transform.parent = spawnPoint.parent.transform;
            for (int i = 0; i < 21; i++)
            {
                landmarks[i] = default(Vector3);
                filteredlandmarks[i] = default(Vector3);
            }
        }

        private void Update()
        {
            if (!received)
            {
                return;
            }

            if (validInput)
            {
                depre = 1f;
                for (int i = 0; i < 21; i++)
                {
                    filteredlandmarks[i] = fm.UpdatePos(1, i, landmarks[i]);
                }

                thumb = Mathf.Acos(Vector3.Dot(Vector3.Normalize(filteredlandmarks[3] - filteredlandmarks[2]), Vector3.Normalize(filteredlandmarks[2] - filteredlandmarks[0])));
                index = Vector3.Angle(filteredlandmarks[7] - filteredlandmarks[6], filteredlandmarks[6] - filteredlandmarks[5]);
                middle = Vector3.Angle(filteredlandmarks[11] - filteredlandmarks[10], filteredlandmarks[10] - filteredlandmarks[9]);
                ring = Vector3.Angle(filteredlandmarks[15] - filteredlandmarks[14], filteredlandmarks[14] - filteredlandmarks[13]);
                pinky = Vector3.Angle(filteredlandmarks[19] - filteredlandmarks[18], filteredlandmarks[18] - filteredlandmarks[17]);
                lthumb = Utils.Remap(thumb, 0.2f, 0.5f);
                lindex = Utils.Remap(index, 0f, 100f);
                lmiddle = Utils.Remap(middle, 0f, 100f);
                lring = Utils.Remap(ring, 0f, 100f);
                lpinky = Utils.Remap(pinky, 0f, 100f);
                l = Utils.Remap(Vector3.Distance(filteredlandmarks[9], filteredlandmarks[0]), 0.015f, 0.05f);
                lerp = fm.UpdateLerp(0, 0, l);
                Vector3 forward = Utils.scaling(filteredlandmarks[0]) - Utils.scaling(filteredlandmarks[9]);
                Vector3 upwards = Utils.TriangleNormal(Utils.scaling(filteredlandmarks[0]), Utils.scaling(filteredlandmarks[17]), Utils.scaling(filteredlandmarks[5]));
                Quaternion quaternion = Quaternion.LookRotation(forward, upwards);
                rot = quaternion;
                Quaternion quaternion2 = rot * Quaternion.Euler(0f, -90f, 0f);
                animator.GetBoneTransform(HumanBodyBones.LeftHand).transform.rotation = Quaternion.Lerp(rotFromArm.normalized, quaternion2.normalized, lerp);
                quaternion = Quaternion.LookRotation(Utils.scaling(filteredlandmarks[6]) - Utils.scaling(filteredlandmarks[5]), upwards);
                rotindex = quaternion;
                quaternion = Quaternion.LookRotation(Utils.scaling(filteredlandmarks[10]) - Utils.scaling(filteredlandmarks[9]), upwards);
                rotmid = quaternion;
                quaternion = Quaternion.LookRotation(Utils.scaling(filteredlandmarks[14]) - Utils.scaling(filteredlandmarks[13]), upwards);
                rotring = quaternion;
                quaternion = Quaternion.LookRotation(Utils.scaling(filteredlandmarks[18]) - Utils.scaling(filteredlandmarks[17]), upwards);
                rotpinky = quaternion;
            }
            else
            {
                depre = Mathf.Clamp01(depre - 0.1f);
                lerp = fm.UpdateLerp(0, 0, l * depre);
            }

            Quaternion rotation = rotindex * Quaternion.Euler(-90f, 0f, 90f);
            indexL.transform.rotation = rotation;
            Quaternion quaternion3 = Quaternion.Inverse(indexL.transform.rotation) * animator.GetBoneTransform(HumanBodyBones.LeftHand).transform.rotation;
            float num = Utils.Remap(0f - quaternion3.y, -0.05f, 0.05f, 0f, 15f);
            Vector3 euler = new Vector3(0f, num * lerp, 70f * lindex * lerp);
            inout = Utils.Remap(0f - quaternion3.y, -0.05f, 0.05f, 0f, 1.5f);
            rotation = rotmid * Quaternion.Euler(-90f, 0f, 90f);
            middleL.transform.rotation = rotation;
            float num2 = Utils.Remap(0f - (Quaternion.Inverse(middleL.transform.rotation) * animator.GetBoneTransform(HumanBodyBones.LeftHand).transform.rotation).y, -0.08f, 0.02f, -10f, 10f);
            Vector3 euler2 = new Vector3(0f, num2 * lerp, 70f * lmiddle * lerp);
            rotation = rotring * Quaternion.Euler(-90f, 0f, 90f);
            ringL.transform.rotation = rotation;
            float num3 = 0f - Utils.Remap((Quaternion.Inverse(ringL.transform.rotation) * animator.GetBoneTransform(HumanBodyBones.LeftHand).transform.rotation).y, 0f, 0.1f, -10f, 10f);
            Vector3 euler3 = new Vector3(0f, num3 * lerp, 70f * lring * lerp);
            rotation = rotpinky * Quaternion.Euler(-90f, 0f, 90f);
            pinkyL.transform.rotation = rotation;
            float num4 = 0f - Utils.Remap((Quaternion.Inverse(pinkyL.transform.rotation) * animator.GetBoneTransform(HumanBodyBones.LeftHand).transform.rotation).y, 0.05f, 0.15f, 0f, 25f);
            Vector3 euler4 = new Vector3(0f, num4 * lerp, 70f * lpinky * lerp);
            animator.GetBoneTransform(HumanBodyBones.LeftThumbProximal).transform.localRotation = Quaternion.Euler(lthumb1 * lthumb * lerp);
            animator.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate).transform.localRotation = Quaternion.Euler(lthumb2 * lthumb * lerp);
            animator.GetBoneTransform(HumanBodyBones.LeftThumbDistal).transform.localRotation = Quaternion.Euler(lthumb3 * lthumb * lerp);
            animator.GetBoneTransform(HumanBodyBones.LeftIndexProximal).transform.localRotation = Quaternion.Euler(euler);
            animator.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate).transform.localRotation = Quaternion.Euler(lindex2 * lindex * lerp);
            animator.GetBoneTransform(HumanBodyBones.LeftIndexDistal).transform.localRotation = Quaternion.Euler(lindex3 * lindex * lerp);
            animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal).transform.localRotation = Quaternion.Euler(euler2);
            animator.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate).transform.localRotation = Quaternion.Euler(lmiddle2 * lmiddle * lerp);
            animator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal).transform.localRotation = Quaternion.Euler(lmiddle3 * lmiddle * lerp);
            animator.GetBoneTransform(HumanBodyBones.LeftRingProximal).transform.localRotation = Quaternion.Euler(euler3);
            animator.GetBoneTransform(HumanBodyBones.LeftRingIntermediate).transform.localRotation = Quaternion.Euler(lring2 * lring * lerp);
            animator.GetBoneTransform(HumanBodyBones.LeftRingDistal).transform.localRotation = Quaternion.Euler(lring3 * lring * lerp);
            animator.GetBoneTransform(HumanBodyBones.LeftLittleProximal).transform.localRotation = Quaternion.Euler(euler4);
            animator.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate).transform.localRotation = Quaternion.Euler(lpinky2 * lpinky * lerp);
            animator.GetBoneTransform(HumanBodyBones.LeftLittleDistal).transform.localRotation = Quaternion.Euler(lpinky3 * lpinky * lerp);
            received = false;
            validInput = false;
        }
    }
}
