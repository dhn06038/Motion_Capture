using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;
using Mediapipe.Unity;

public class AvatarController : MonoBehaviour
{
    // VRM ���� �� ����
    public Transform Neck;
    public Transform Head;
    public Transform Hips;
    public Transform Spine;
    public Transform Chest;
    public Transform UpperChest;
    public Transform LeftShoulder;
    public Transform LeftUpperArm;
    public Transform LeftLowerArm;
    public Transform LeftHand;
    public Transform RightShoulder;
    public Transform RightUpperArm;
    public Transform RightLowerArm;
    public Transform RightHand;
    public Transform LeftUpperLeg;
    public Transform LeftLowerLeg;
    public Transform LeftFoot;
    public Transform RightUpperLeg;
    public Transform RightLowerLeg;
    public Transform RightFoot;
    // �հ��� ���� �߰�
    public Transform LeftThumb1;
    public Transform LeftThumb2;
    public Transform LeftThumb3;
    public Transform LeftThumb4;
    public Transform LeftIndex1;
    public Transform LeftIndex2;
    public Transform LeftIndex3;
    public Transform LeftIndex4;
    public Transform LeftMiddle1;
    public Transform LeftMiddle2;
    public Transform LeftMiddle3;
    public Transform LeftMiddle4;
    public Transform LeftRing1;
    public Transform LeftRing2;
    public Transform LeftRing3;
    public Transform LeftRing4;
    public Transform LeftPinky1;
    public Transform LeftPinky2;
    public Transform LeftPinky3;
    public Transform LeftPinky4;

    public Transform RightThumb1;
    public Transform RightThumb2;
    public Transform RightThumb3;
    public Transform RightThumb4;
    public Transform RightIndex1;
    public Transform RightIndex2;
    public Transform RightIndex3;
    public Transform RightIndex4;
    public Transform RightMiddle1;
    public Transform RightMiddle2;
    public Transform RightMiddle3;
    public Transform RightMiddle4;
    public Transform RightRing1;
    public Transform RightRing2;
    public Transform RightRing3;
    public Transform RightRing4;
    public Transform RightPinky1;
    public Transform RightPinky2;
    public Transform RightPinky3;
    public Transform RightPinky4;

    public float widthScale = 1.0f;
    public float heightScale = 1.0f;
    public float depthScale = 1.0f;
    public void UpdateAvatar(NormalizedLandmarkList poseLandmarks, NormalizedLandmarkList leftHandLandmarks, NormalizedLandmarkList rightHandLandmarks, NormalizedLandmarkList faceLandmarks)
    {
        UpdateTorso(poseLandmarks);
        UpdateArms(poseLandmarks);
        UpdateHands(leftHandLandmarks, rightHandLandmarks);
        UpdateFace(faceLandmarks);
        UpdateLegs(poseLandmarks);
    }

    private void UpdateTorso(NormalizedLandmarkList poseLandmarks)
    {
        // Hips, Spine, Chest, Neck, Head ������Ʈ
        // �� ���� ��ġ�� ȸ���� ����Ͽ� ����
    }

    private void UpdateArms(NormalizedLandmarkList poseLandmarks)
    {
        // ���� �� ������Ʈ
        Vector3 leftShoulderPos = MediaPipeLandmarkToUnityWorld(poseLandmarks.Landmark[11]);
        Vector3 leftElbowPos = MediaPipeLandmarkToUnityWorld(poseLandmarks.Landmark[13]);
        Vector3 leftWristPos = MediaPipeLandmarkToUnityWorld(poseLandmarks.Landmark[15]);

        UpdateBoneRotation(this.LeftUpperArm, leftShoulderPos, leftElbowPos);
        UpdateBoneRotation(this.LeftLowerArm, leftElbowPos, leftWristPos);

        // ������ �� ������Ʈ (������ ���)
    }

    private void UpdateHands(NormalizedLandmarkList leftHandLandmarks, NormalizedLandmarkList rightHandLandmarks)
    {
        if (leftHandLandmarks != null)
        {
            UpdateLeftHand(leftHandLandmarks);
        }
        if (rightHandLandmarks != null)
        {
            UpdateRightHand(rightHandLandmarks);
        }
    }

    private void UpdateLeftHand(NormalizedLandmarkList handLandmarks)
    {
        // �հ��� �� ������Ʈ
        // �����հ���
        Vector3 thumbCMC = MediaPipeLandmarkToUnityWorld(handLandmarks.Landmark[1]);
        Vector3 thumbMCP = MediaPipeLandmarkToUnityWorld(handLandmarks.Landmark[2]);
        Vector3 thumbIP = MediaPipeLandmarkToUnityWorld(handLandmarks.Landmark[3]);
        Vector3 thumbTip = MediaPipeLandmarkToUnityWorld(handLandmarks.Landmark[4]);

        UpdateBoneRotation(this.LeftThumb1, thumbCMC, thumbMCP);
        UpdateBoneRotation(this.LeftThumb2, thumbMCP, thumbIP);
        UpdateBoneRotation(this.LeftThumb3, thumbIP, thumbTip);

        // �ٸ� �հ����鵵 ������Ʈ
    }

    private void UpdateRightHand(NormalizedLandmarkList handLandmarks)
    {
        // �հ��� �� ������Ʈ
        // �����հ���
        Vector3 thumbCMC = MediaPipeLandmarkToUnityWorld(handLandmarks.Landmark[1]);
        Vector3 thumbMCP = MediaPipeLandmarkToUnityWorld(handLandmarks.Landmark[2]);
        Vector3 thumbIP = MediaPipeLandmarkToUnityWorld(handLandmarks.Landmark[3]);
        Vector3 thumbTip = MediaPipeLandmarkToUnityWorld(handLandmarks.Landmark[4]);

        UpdateBoneRotation(this.LeftThumb1, thumbCMC, thumbMCP);
        UpdateBoneRotation(this.LeftThumb2, thumbMCP, thumbIP);
        UpdateBoneRotation(this.LeftThumb3, thumbIP, thumbTip);

        // �ٸ� �հ����鵵 ������Ʈ
    }

    private void UpdateFace(NormalizedLandmarkList faceLandmarks)
    {
        // �� ǥ�� �� �ü� ������Ʈ
        // ���� ������ ���� Ȱ���Ͽ� ǥ�� ����
    }

    private void UpdateLegs(NormalizedLandmarkList poseLandmarks)
    {
        // �ٸ� �� ������Ʈ
        // ���� �ٸ�
        Vector3 leftHipPos = MediaPipeLandmarkToUnityWorld(poseLandmarks.Landmark[23]);
        Vector3 leftKneePos = MediaPipeLandmarkToUnityWorld(poseLandmarks.Landmark[25]);
        Vector3 leftAnklePos = MediaPipeLandmarkToUnityWorld(poseLandmarks.Landmark[27]);

        UpdateBoneRotation(this.LeftUpperLeg, leftHipPos, leftKneePos);
        UpdateBoneRotation(this.LeftLowerLeg, leftKneePos, leftAnklePos);

        // ������ �ٸ� ������Ʈ (������ ���)
    }

    private Vector3 MediaPipeLandmarkToUnityWorld(NormalizedLandmark landmark)
    {
        float x = (landmark.X - 0.5f) * widthScale;
        float y = (landmark.Y - 0.5f) * heightScale;
        float z = -landmark.Z * depthScale;
        return new Vector3(x, y, z);
    }

    private void UpdateBoneRotation(Transform bone, Vector3 startPoint, Vector3 endPoint)
    {
        Vector3 direction = endPoint - startPoint;
        if (direction.sqrMagnitude > 0.0f)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            bone.rotation = rotation;
        }
    }
}
