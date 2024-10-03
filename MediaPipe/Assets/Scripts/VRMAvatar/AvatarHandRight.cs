using UnityEngine;

namespace VRMAvatar
{
    public class AvatarHandRight : MonoBehaviour
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

        private Vector3 lthumb1 = new Vector3(5f, 18f, -30f);

        private Vector3 lthumb2 = new Vector3(0f, 40f, 0f);

        private Vector3 lthumb3 = new Vector3(0f, 20f, 0f);

        private Vector3 lindex1 = new Vector3(0f, 0f, -70f);

        private Vector3 lindex2 = new Vector3(0f, 0f, -70f);

        private Vector3 lindex3 = new Vector3(0f, 0f, -80f);

        private Vector3 lmiddle1 = new Vector3(0f, 0f, -70f);

        private Vector3 lmiddle2 = new Vector3(0f, 0f, -70f);

        private Vector3 lmiddle3 = new Vector3(0f, 0f, -80f);

        private Vector3 lring1 = new Vector3(-5f, 0f, -70f);

        private Vector3 lring2 = new Vector3(0f, 0f, -70f);

        private Vector3 lring3 = new Vector3(0f, 0f, -80f);

        private Vector3 lpinky1 = new Vector3(-10f, 0f, -70f);

        private Vector3 lpinky2 = new Vector3(0f, 0f, -70f);

        private Vector3 lpinky3 = new Vector3(0f, 0f, -80f);

        private Animator animator;

        private Quaternion rotindex;

        private Quaternion rotmid;

        private Quaternion rotring;

        private Quaternion rotpinky;

        private GameObject indexR;

        private GameObject middleR;

        private GameObject ringR;

        private GameObject pinkyR;

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
            spawnPoint = Utils.getChildGameObject(base.gameObject, "index1.R").transform;
            indexR = new GameObject("indexR");
            indexR.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            indexR.transform.parent = spawnPoint.parent.transform;
            spawnPoint = Utils.getChildGameObject(base.gameObject, "middle1.R").transform;
            middleR = new GameObject("middleR");
            middleR.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            middleR.transform.parent = spawnPoint.parent.transform;
            spawnPoint = Utils.getChildGameObject(base.gameObject, "ring1.R").transform;
            ringR = new GameObject("ringR");
            ringR.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            ringR.transform.parent = spawnPoint.parent.transform;
            spawnPoint = Utils.getChildGameObject(base.gameObject, "pinky1.R").transform;
            pinkyR = new GameObject("pinkyR");
            pinkyR.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            pinkyR.transform.parent = spawnPoint.parent.transform;
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
                    filteredlandmarks[i] = fm.UpdatePos(2, i, landmarks[i]);
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
                lerp = fm.UpdateLerp(1, 0, l);
                Vector3 forward = Utils.scaling(filteredlandmarks[9]) - Utils.scaling(filteredlandmarks[0]);
                Vector3 upwards = Utils.TriangleNormal(Utils.scaling(filteredlandmarks[0]), Utils.scaling(filteredlandmarks[17]), Utils.scaling(filteredlandmarks[5]));
                Quaternion quaternion = Quaternion.LookRotation(forward, upwards);
                rot = quaternion;
                Quaternion quaternion2 = rot * Quaternion.Euler(-180f, -90f, 0f);
                animator.GetBoneTransform(HumanBodyBones.RightHand).transform.rotation = Quaternion.Lerp(rotFromArm.normalized, quaternion2.normalized, lerp);
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
                lerp = fm.UpdateLerp(1, 0, l * depre);
            }

            Quaternion rotation = rotindex * Quaternion.Euler(90f, 0f, -90f);
            indexR.transform.rotation = rotation;
            Quaternion quaternion3 = Quaternion.Inverse(indexR.transform.rotation) * animator.GetBoneTransform(HumanBodyBones.RightHand).transform.rotation;
            float num = Utils.Remap(0f - quaternion3.x, -0.05f, 0.05f, -15f, 0f);
            lindex1 = new Vector3(0f, num * lerp, -70f * lindex * lerp);
            inout = 1.5f - Utils.Remap(0f - quaternion3.x, -0.05f, 0.05f, 0f, 1.5f);
            rotation = rotmid * Quaternion.Euler(90f, 0f, -90f);
            middleR.transform.rotation = rotation;
            float num2 = Utils.Remap(0f - (Quaternion.Inverse(middleR.transform.rotation) * animator.GetBoneTransform(HumanBodyBones.RightHand).transform.rotation).x, -0.02f, 0.08f, -10f, 10f);
            lmiddle1 = new Vector3(0f, num2 * lerp, -70f * lmiddle * lerp);
            rotation = rotring * Quaternion.Euler(90f, 0f, -90f);
            ringR.transform.rotation = rotation;
            float num3 = Utils.Remap(0f - (Quaternion.Inverse(ringR.transform.rotation) * animator.GetBoneTransform(HumanBodyBones.RightHand).transform.rotation).x, 0f, 0.1f, -10f, 10f);
            lring1 = new Vector3(0f, num3 * lerp, -70f * lring * lerp);
            rotation = rotpinky * Quaternion.Euler(90f, 0f, -90f);
            pinkyR.transform.rotation = rotation;
            float num4 = Utils.Remap(0f - (Quaternion.Inverse(pinkyR.transform.rotation) * animator.GetBoneTransform(HumanBodyBones.RightHand).transform.rotation).x, 0.05f, 0.15f, 0f, 25f);
            lpinky1 = new Vector3(0f, num4 * lerp, -70f * lpinky * lerp);
            animator.GetBoneTransform(HumanBodyBones.RightThumbProximal).transform.localRotation = Quaternion.Euler(lthumb1 * lthumb * lerp);
            animator.GetBoneTransform(HumanBodyBones.RightThumbIntermediate).transform.localRotation = Quaternion.Euler(lthumb2 * lthumb * lerp);
            animator.GetBoneTransform(HumanBodyBones.RightThumbDistal).transform.localRotation = Quaternion.Euler(lthumb3 * lthumb * lerp);
            animator.GetBoneTransform(HumanBodyBones.RightIndexProximal).transform.localRotation = Quaternion.Euler(lindex1);
            animator.GetBoneTransform(HumanBodyBones.RightIndexIntermediate).transform.localRotation = Quaternion.Euler(lindex2 * lindex * lerp);
            animator.GetBoneTransform(HumanBodyBones.RightIndexDistal).transform.localRotation = Quaternion.Euler(lindex3 * lindex * lerp);
            animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal).transform.localRotation = Quaternion.Euler(lmiddle1);
            animator.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate).transform.localRotation = Quaternion.Euler(lmiddle2 * lmiddle * lerp);
            animator.GetBoneTransform(HumanBodyBones.RightMiddleDistal).transform.localRotation = Quaternion.Euler(lmiddle3 * lmiddle * lerp);
            animator.GetBoneTransform(HumanBodyBones.RightRingProximal).transform.localRotation = Quaternion.Euler(lring1);
            animator.GetBoneTransform(HumanBodyBones.RightRingIntermediate).transform.localRotation = Quaternion.Euler(lring2 * lring * lerp);
            animator.GetBoneTransform(HumanBodyBones.RightRingDistal).transform.localRotation = Quaternion.Euler(lring3 * lring * lerp);
            animator.GetBoneTransform(HumanBodyBones.RightLittleProximal).transform.localRotation = Quaternion.Euler(lpinky1);
            animator.GetBoneTransform(HumanBodyBones.RightLittleIntermediate).transform.localRotation = Quaternion.Euler(lpinky2 * lpinky * lerp);
            animator.GetBoneTransform(HumanBodyBones.RightLittleDistal).transform.localRotation = Quaternion.Euler(lpinky3 * lpinky * lerp);
            received = false;
            validInput = false;
        }
    }
}