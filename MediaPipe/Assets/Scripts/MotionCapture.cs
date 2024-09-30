using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionCapture : MonoBehaviour
{
    // Start is called before the first frame update
    public UDPReceive udpReceive;
    public GameObject[] landMarks;

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

    Vector3[] poseLandmarks;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        string data = udpReceive.data;
        data = data.Remove(0, 1);
        data = data.Remove(data.Length - 1, 1);

        string[] points = data.Split(',');
        print(points[0]);

        //0        1*3      2*3
        //x1,y1,z1,x2,y2,z2,x3,y3,z3

        for (int i = 0; i < 33; i++)
        {

            float x = 7 - float.Parse(points[i * 3]) / 100;
            float y = float.Parse(points[i * 3 + 1]) / 100;
            float z = float.Parse(points[i * 3 + 2]) / 100;

            poseLandmarks[i] = new Vector3(x, y, z);

            landMarks[i].transform.localPosition = new Vector3(x, y, z);
        }

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