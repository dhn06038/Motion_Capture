using System.Collections.Generic;
using UnityEngine;

namespace VRMAvatar
{
    public class FilterManager : MonoBehaviour
    {
        private KalmanFilter[] _kalmanFilters = new KalmanFilter[33];

        private KalmanFilter.Measurement<Vector3>[] measurements = new KalmanFilter.Measurement<Vector3>[33];

        private KalmanFilter[] _LHkalmanFilters = new KalmanFilter[21];

        private KalmanFilter.Measurement<Vector3>[] LHmeasurements = new KalmanFilter.Measurement<Vector3>[21];

        private KalmanFilter[] _RHkalmanFilters = new KalmanFilter[21];

        private KalmanFilter.Measurement<Vector3>[] RHmeasurements = new KalmanFilter.Measurement<Vector3>[21];

        private KalmanFilter[] _FacekalmanFilters = new KalmanFilter[478];

        private KalmanFilter.Measurement<Vector3>[] Facemeasurements = new KalmanFilter.Measurement<Vector3>[478];

        private KalmanFilter[] _LerpLHkalmanFilters = new KalmanFilter[10];

        private KalmanFilter.Measurement<float>[] LerpLHmeasurements = new KalmanFilter.Measurement<float>[10];

        private KalmanFilter[] _LerpRHkalmanFilters = new KalmanFilter[10];

        private KalmanFilter.Measurement<float>[] LerpRHmeasurements = new KalmanFilter.Measurement<float>[10];

        private KalmanFilter[] _visibilitykalmanFilters = new KalmanFilter[33];

        private KalmanFilter.Measurement<float>[] visibilitymeasurements = new KalmanFilter.Measurement<float>[33];

        private KalmanFilter[] _bskalmanFilters = new KalmanFilter[52];

        private KalmanFilter.Measurement<float>[] bsHmeasurements = new KalmanFilter.Measurement<float>[52];

        private int LowPassFilterNOrder = 10;

        private float LowPassFilterSmooth = 0.1f;

        private List<LowPassFilter> lowPassFilter = new List<LowPassFilter>();

        private List<LowPassFilter> lowPassHandFilter = new List<LowPassFilter>();

        private float filterFrequency = 120f;

        private float filterMinCutoff = 1f;

        private float filterBeta = 1f;

        private float filterDcutoff = 10f;

        private List<OneEuroFilter<Quaternion>> rotationFilter = new List<OneEuroFilter<Quaternion>>();

        private List<OneEuroFilter<Vector2>> positionFilter = new List<OneEuroFilter<Vector2>>();

        private float upperbodyR;

        private float getSensitivityParams(int sensi)
        {
            return sensi switch
            {
                5 => 0.0015f,
                4 => 0.0045f,
                3 => 0.0135f,
                2 => 0.04f,
                1 => 0.12f,
                _ => 0.0015f,
            };
        }

        public void init(int sensi)
        {
            float sensitivityParams = getSensitivityParams(sensi);
            float q = 0.001f;
            upperbodyR = sensitivityParams;
            for (int i = 0; i < 33; i++)
            {
                _kalmanFilters[i] = new KalmanFilter(q, sensitivityParams);
                measurements[i] = new KalmanFilter.Measurement<Vector3>();
            }

            for (int j = 0; j < 21; j++)
            {
                _LHkalmanFilters[j] = new KalmanFilter(q, sensitivityParams);
                LHmeasurements[j] = new KalmanFilter.Measurement<Vector3>();
            }

            for (int k = 0; k < 21; k++)
            {
                _RHkalmanFilters[k] = new KalmanFilter(q, sensitivityParams);
                RHmeasurements[k] = new KalmanFilter.Measurement<Vector3>();
            }

            for (int l = 0; l < 478; l++)
            {
                _FacekalmanFilters[l] = new KalmanFilter(q, sensitivityParams);
                Facemeasurements[l] = new KalmanFilter.Measurement<Vector3>();
            }

            for (int m = 0; m < 9; m++)
            {
                _LerpLHkalmanFilters[m] = new KalmanFilter(q, sensitivityParams);
                LerpLHmeasurements[m] = new KalmanFilter.Measurement<float>();
            }

            for (int n = 0; n < 9; n++)
            {
                _LerpRHkalmanFilters[n] = new KalmanFilter(q, sensitivityParams);
                LerpRHmeasurements[n] = new KalmanFilter.Measurement<float>();
            }

            for (int num = 0; num < 33; num++)
            {
                _visibilitykalmanFilters[num] = new KalmanFilter(q, 0.5f);
                visibilitymeasurements[num] = new KalmanFilter.Measurement<float>();
            }

            InitLowPassFilter();
            for (int num2 = 0; num2 < 50; num2++)
            {
                rotationFilter.Add(new OneEuroFilter<Quaternion>(filterFrequency, filterMinCutoff, filterBeta, filterDcutoff));
            }

            for (int num3 = 0; num3 < 52; num3++)
            {
                positionFilter.Add(new OneEuroFilter<Vector2>(filterFrequency, filterMinCutoff, filterBeta, filterDcutoff));
            }
        }

        private void InitLowPassFilter()
        {
            lowPassFilter.Clear();
            for (int i = 0; i < 33; i++)
            {
                lowPassFilter.Add(new LowPassFilter(LowPassFilterNOrder, LowPassFilterSmooth));
            }

            lowPassHandFilter.Clear();
            for (int j = 0; j < 33; j++)
            {
                lowPassHandFilter.Add(new LowPassFilter(LowPassFilterNOrder, LowPassFilterSmooth));
            }
        }

        public Quaternion UpdateHandRotationOneEuro(int index, Quaternion q)
        {
            return rotationFilter[index].Filter(q);
        }

        public void UpdateUpperbodyParams()
        {
            _kalmanFilters[11].paramR = 0.15f;
            _kalmanFilters[12].paramR = 0.15f;
            _kalmanFilters[23].paramR = 0.15f;
            _kalmanFilters[24].paramR = 0.15f;
        }

        public void RestoreUpperbodyParams()
        {
            _kalmanFilters[11].paramR = upperbodyR;
            _kalmanFilters[12].paramR = upperbodyR;
            _kalmanFilters[23].paramR = upperbodyR;
            _kalmanFilters[24].paramR = upperbodyR;
        }

        public float UpdateLF(int index, float v)
        {
            return lowPassFilter[index].CorrectAndPredict(v);
        }

        public float UpdateBSOneEuro(int index, float v)
        {
            return positionFilter[index].Filter(new Vector2(v, 0f)).x;
        }

        public void UpdateParams(int sensi)
        {
            float sensitivityParams = getSensitivityParams(sensi);
            float paramQ = 0.001f;
            upperbodyR = sensitivityParams;
            for (int i = 0; i < 33; i++)
            {
                _kalmanFilters[i].paramQ = paramQ;
                _kalmanFilters[i].paramR = sensitivityParams;
            }

            for (int j = 0; j < 21; j++)
            {
                _LHkalmanFilters[j].paramQ = paramQ;
                _LHkalmanFilters[j].paramR = sensitivityParams;
            }

            for (int k = 0; k < 21; k++)
            {
                _RHkalmanFilters[k].paramQ = paramQ;
                _RHkalmanFilters[k].paramR = sensitivityParams;
            }

            for (int l = 0; l < 478; l++)
            {
                _FacekalmanFilters[l].paramQ = paramQ;
                _FacekalmanFilters[l].paramR = sensitivityParams;
            }

            for (int m = 0; m < 9; m++)
            {
                _LerpLHkalmanFilters[m].paramQ = paramQ;
                _LerpLHkalmanFilters[m].paramR = sensitivityParams;
            }

            for (int n = 0; n < 9; n++)
            {
                _LerpRHkalmanFilters[n].paramQ = paramQ;
                _LerpRHkalmanFilters[n].paramR = sensitivityParams;
            }
        }

        public Vector3 UpdatePos(int part, int index, Vector3 pos)
        {
            switch (part)
            {
                case -1:
                    Facemeasurements[index].SetObservedValue(pos);
                    _FacekalmanFilters[index].KalmanUpdate(Facemeasurements[index]);
                    return Facemeasurements[index].x;
                case 0:
                    measurements[index].SetObservedValue(pos);
                    _kalmanFilters[index].KalmanUpdate(measurements[index]);
                    return measurements[index].x;
                case 1:
                    LHmeasurements[index].SetObservedValue(pos);
                    _LHkalmanFilters[index].KalmanUpdate(LHmeasurements[index]);
                    return LHmeasurements[index].x;
                default:
                    RHmeasurements[index].SetObservedValue(pos);
                    _RHkalmanFilters[index].KalmanUpdate(RHmeasurements[index]);
                    return RHmeasurements[index].x;
            }
        }

        public float UpdateLerp(int part, int index, float lerp)
        {
            switch (part)
            {
                case 0:
                    LerpLHmeasurements[index].SetObservedValue(lerp);
                    _LerpLHkalmanFilters[index].KalmanUpdate(LerpLHmeasurements[index]);
                    return LerpLHmeasurements[index].x;
                case -1:
                    visibilitymeasurements[index].SetObservedValue(lerp);
                    _visibilitykalmanFilters[index].KalmanUpdate(visibilitymeasurements[index]);
                    return visibilitymeasurements[index].x;
                default:
                    LerpRHmeasurements[index].SetObservedValue(lerp);
                    _LerpRHkalmanFilters[index].KalmanUpdate(LerpRHmeasurements[index]);
                    return LerpRHmeasurements[index].x;
            }
        }
    }
}