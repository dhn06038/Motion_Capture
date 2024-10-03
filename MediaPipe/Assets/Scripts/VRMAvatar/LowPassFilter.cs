using UnityEngine;

namespace VRMAvatar
{

    public class LowPassFilter
    {
        private float[] values = new float[10];

        private int NOrderLPF = 7;

        private float Smooth = 0.9f;

        private int effectiveCount;

        public LowPassFilter(int order, float smooth)
        {
            NOrderLPF = Mathf.Min(order, 10);
            Smooth = smooth;
        }

        public float CorrectAndPredict(float kp)
        {
            values[0] = kp;
            for (int i = 1; i < NOrderLPF; i++)
            {
                values[i] = values[i] * (1f - Smooth) + values[i - 1] * Smooth;
            }

            values[0] = values[0] * (1f - Smooth) + values[NOrderLPF - 1] * Smooth;
            if (effectiveCount < 10)
            {
                effectiveCount++;
                return kp;
            }

            return values[0];
        }
    }
}
