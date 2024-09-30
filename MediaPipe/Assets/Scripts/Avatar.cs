using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviour
{
    public Transform headBone;
    public Transform leftShoulderBone;
    public Transform rightShoulderBone;
    public Transform leftElbowBone;
    public Transform rightElbowBone;
    public Transform leftHandBone;
    public Transform rightHandBone;
    public Transform leftHipBone;
    public Transform rightHipBone;
    public Transform leftKneeBone;
    public Transform rightKneeBone;
    public Transform leftFootBone;
    public Transform rightFootBone;
    public Transform spineBone;
    public Transform neckBone;

    private Vector3[] poseLandmarks = MotionCapture.poseLandmarks;

    // Update is called once per frame
    void Update()
    {
        headBone.position = ConvertLandmarkToUnity(poseLandmarks[0]);  // ÄÚ
        leftShoulderBone.position = ConvertLandmarkToUnity(poseLandmarks[9]); // ¿ÞÂÊ ¾î±ú
        rightShoulderBone.position = ConvertLandmarkToUnity(poseLandmarks[10]); // ¿À¸¥ÂÊ ¾î±ú
        leftElbowBone.position = ConvertLandmarkToUnity(poseLandmarks[11]); // ¿ÞÂÊ ÆÈ²ÞÄ¡
        rightElbowBone.position = ConvertLandmarkToUnity(poseLandmarks[12]); // ¿À¸¥ÂÊ ÆÈ²ÞÄ¡
        leftHandBone.position = ConvertLandmarkToUnity(poseLandmarks[13]); // ¿ÞÂÊ ¼Õ¸ñ
        rightHandBone.position = ConvertLandmarkToUnity(poseLandmarks[14]); // ¿À¸¥ÂÊ ¼Õ¸ñ
        leftHipBone.position = ConvertLandmarkToUnity(poseLandmarks[19]); // ¿ÞÂÊ °ñ¹Ý
        rightHipBone.position = ConvertLandmarkToUnity(poseLandmarks[20]); // ¿À¸¥ÂÊ °ñ¹Ý
        leftKneeBone.position = ConvertLandmarkToUnity(poseLandmarks[21]); // ¿ÞÂÊ ¹«¸­
        rightKneeBone.position = ConvertLandmarkToUnity(poseLandmarks[22]); // ¿À¸¥ÂÊ ¹«¸­
        leftFootBone.position = ConvertLandmarkToUnity(poseLandmarks[23]); // ¿ÞÂÊ ¹ß¸ñ
        rightFootBone.position = ConvertLandmarkToUnity(poseLandmarks[24]); // ¿À¸¥ÂÊ ¹ß¸ñ
        spineBone.position = ConvertLandmarkToUnity(poseLandmarks[29]); // Ã´Ãß Áß½É
        neckBone.position = ConvertLandmarkToUnity(poseLandmarks[30]); // ¸ñ
    }

    Vector3 ConvertLandmarkToUnity(Vector3 landmark)
    {
        // ÁÂÇ¥ º¯È¯ ¹× ½ºÄÉÀÏ¸µ
        float scaleFactor = 1.0f;
        return new Vector3(landmark.x * scaleFactor, landmark.y * scaleFactor, -landmark.z * scaleFactor);
    }
}
