using UnityEngine;

namespace VRMAvatar
{
    public class AvatarFace : MonoBehaviour
    {
        [HideInInspector]
        public FaceCapResult fcr;

        [HideInInspector]
        public bool received;

        [HideInInspector]
        public bool validInput;

        private FilterManager fm;

        public DominantEye dominantEye;

        [HideInInspector]
        public float[] bsv = new float[52];

        private float[] init_bs = new float[52];

        private float strength = 1f;

        private int[] bsmapping = new int[51]
        {
            9, 11, 13, 15, 17, 19, 21, 10, 12, 14,
            16, 18, 20, 22, 23, 24, 26, 25, 27, 32,
            38, 33, 39, 44, 45, 30, 31, 28, 29, 46,
            47, 40, 41, 42, 43, 36, 37, 34, 35, 48,
            49, 1, 2, 3, 4, 5, 6, 7, 8, 50,
            51
        };

        private void Start()
        {
            fm = base.gameObject.GetComponent<FilterManager>();
            fcr = base.gameObject.GetComponent<FaceCapResult>();
        }

        public void Calibrate()
        {
            for (int i = 0; i < bsmapping.Length; i++)
            {
                init_bs[i] = bsv[i];
            }
        }

        public void ResetCalibration()
        {
            for (int i = 0; i < bsmapping.Length; i++)
            {
                init_bs[i] = 0f;
            }
        }

        public void SetStrength(float s)
        {
            strength = s;
        }

        public void SetDominantEye(DominantEye de)
        {
            dominantEye = de;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Alpha1))
            {
                Calibrate();
            }

            if (!received)
            {
                return;
            }

            for (int i = 0; i < bsmapping.Length; i++)
            {
                if (validInput)
                {
                    fcr.values[LiveLinkTrackingData.Names[i]] = Mathf.Clamp01((bsv[bsmapping[i]] - init_bs[bsmapping[i]]) / (100f - init_bs[bsmapping[i]]) * 100f * strength);
                    fcr.values[LiveLinkTrackingData.Names[i]] = fm.UpdateBSOneEuro(i, fcr.values[LiveLinkTrackingData.Names[i]]);
                }
                else
                {
                    fcr.values[LiveLinkTrackingData.Names[i]] = fm.UpdateBSOneEuro(i, 0f);
                }
            }

            if (dominantEye == DominantEye.left)
            {
                fcr.values["EyeLookUpRight"] = fcr.values["EyeLookUpLeft"];
                fcr.values["EyeLookDownRight"] = fcr.values["EyeLookDownLeft"];
                fcr.values["EyeLookInRight"] = fcr.values["EyeLookOutLeft"];
                fcr.values["EyeLookOutRight"] = fcr.values["EyeLookInLeft"];
            }
            else if (dominantEye == DominantEye.right)
            {
                fcr.values["EyeLookUpLeft"] = fcr.values["EyeLookUpRight"];
                fcr.values["EyeLookDownLeft"] = fcr.values["EyeLookDownRight"];
                fcr.values["EyeLookOutLeft"] = fcr.values["EyeLookInRight"];
                fcr.values["EyeLookInLeft"] = fcr.values["EyeLookOutRight"];
            }

            received = false;
            validInput = false;
        }
    }
}