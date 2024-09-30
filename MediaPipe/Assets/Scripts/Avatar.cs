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
        headBone.position = ConvertLandmarkToUnity(poseLandmarks[0]);  // ��
        leftShoulderBone.position = ConvertLandmarkToUnity(poseLandmarks[9]); // ���� ���
        rightShoulderBone.position = ConvertLandmarkToUnity(poseLandmarks[10]); // ������ ���
        leftElbowBone.position = ConvertLandmarkToUnity(poseLandmarks[11]); // ���� �Ȳ�ġ
        rightElbowBone.position = ConvertLandmarkToUnity(poseLandmarks[12]); // ������ �Ȳ�ġ
        leftHandBone.position = ConvertLandmarkToUnity(poseLandmarks[13]); // ���� �ո�
        rightHandBone.position = ConvertLandmarkToUnity(poseLandmarks[14]); // ������ �ո�
        leftHipBone.position = ConvertLandmarkToUnity(poseLandmarks[19]); // ���� ���
        rightHipBone.position = ConvertLandmarkToUnity(poseLandmarks[20]); // ������ ���
        leftKneeBone.position = ConvertLandmarkToUnity(poseLandmarks[21]); // ���� ����
        rightKneeBone.position = ConvertLandmarkToUnity(poseLandmarks[22]); // ������ ����
        leftFootBone.position = ConvertLandmarkToUnity(poseLandmarks[23]); // ���� �߸�
        rightFootBone.position = ConvertLandmarkToUnity(poseLandmarks[24]); // ������ �߸�
        spineBone.position = ConvertLandmarkToUnity(poseLandmarks[29]); // ô�� �߽�
        neckBone.position = ConvertLandmarkToUnity(poseLandmarks[30]); // ��
    }

    Vector3 ConvertLandmarkToUnity(Vector3 landmark)
    {
        // ��ǥ ��ȯ �� �����ϸ�
        float scaleFactor = 1.0f;
        return new Vector3(landmark.x * scaleFactor, landmark.y * scaleFactor, -landmark.z * scaleFactor);
    }
}
