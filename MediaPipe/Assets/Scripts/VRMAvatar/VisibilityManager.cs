using System;
using UnityEngine;

namespace VRMAvatar
{
    public class VisibilityManager : MonoBehaviour
    {
        private void Start()
        {
            this.fm = base.gameObject.GetComponent<FilterManager>();
            this.rh = base.gameObject.GetComponent<AvatarHandRight>();
            this.lh = base.gameObject.GetComponent<AvatarHandLeft>();
        }

        private void Update()
        {
            if (!this.received)
            {
                return;
            }
            if (this.valid)
            {
                for (int i = 0; i < 33; i++)
                {
                    this.FilteredWorldPoseVisibilities[i] = this.WorldPoseVisibilities[i];
                    this.FilteredWorldPoseVisibilities[i] = this.fm.UpdateLF(i, this.FilteredWorldPoseVisibilities[i]);
                }
            }
            else
            {
                for (int j = 0; j < 33; j++)
                {
                    this.FilteredWorldPoseVisibilities[j] = Mathf.Clamp01(this.fm.UpdateLF(j, this.FilteredWorldPoseVisibilities[j] * this.FilteredWorldPoseVisibilities[j]));
                }
            }
            this.LerpRightHand = this.rh.lerp;
            this.LerpLeftHand = this.lh.lerp;
            this.VisibilityNose = this.FilteredWorldPoseVisibilities[0];
            this.VisibilityLeftHand = this.FilteredWorldPoseVisibilities[15];
            this.VisiblityRightHand = this.FilteredWorldPoseVisibilities[16];
            this.VisibilityHip = (this.FilteredWorldPoseVisibilities[23] + this.FilteredWorldPoseVisibilities[24]) / 2f;
            this.VisibilityLeftLeg = this.FilteredWorldPoseVisibilities[27];
            this.VisibilityRightLeg = this.FilteredWorldPoseVisibilities[28];
            this.valid = false;
            this.received = false;
        }

        [HideInInspector]
        public float VisibilityNose;

        [HideInInspector]
        public float VisibilityLeftHand;

        [HideInInspector]
        public float VisiblityRightHand;

        [HideInInspector]
        public float VisibilityHip;

        [HideInInspector]
        public float VisibilityLeftLeg;

        [HideInInspector]
        public float VisibilityRightLeg;

        [HideInInspector]
        public float LerpLeftHand;

        [HideInInspector]
        public float LerpRightHand;

        private FilterManager fm;

        private AvatarHandRight rh;

        private AvatarHandLeft lh;

        [HideInInspector]
        public float[] WorldPoseVisibilities = new float[33];

        [HideInInspector]
        public float[] FilteredWorldPoseVisibilities = new float[33];

        [HideInInspector]
        public bool received;

        [HideInInspector]
        public bool valid;
    }
}
