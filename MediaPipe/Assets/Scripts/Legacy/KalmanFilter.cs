using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KalmanFilter : MonoBehaviour
{
    private float Q;  // Process noise covariance
    private float R;  // Measurement noise covariance
    private float P;  // Estimation error covariance
    private float K;  // Kalman gain
    private float X;  // State
    public KalmanFilter(float processNoise, float measurementNoise, float estimationError, float initialValue)
    {
        Q = processNoise;
        R = measurementNoise;
        P = estimationError;
        X = initialValue;
    }

    public float Update(float measurement)
    {
        // Prediction update
        P = P + Q;

        // Measurement update
        K = P / (P + R);  // Kalman gain
        X = X + K * (measurement - X);  // Update state
        P = (1 - K) * P;  // Update estimation error covariance

        return X;  // Return filtered value
    }
}
