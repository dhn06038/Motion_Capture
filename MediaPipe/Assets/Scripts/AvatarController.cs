using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;
using Mediapipe.Unity;

public class AvatarController : MonoBehaviour
{
    // VRM 모델의 본 참조
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
    // 손가락 본들 추가
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
        // Hips, Spine, Chest, Neck, Head 업데이트
        // 각 본의 위치와 회전을 계산하여 적용
    }

    private void UpdateArms(NormalizedLandmarkList poseLandmarks)
    {
        /*// 왼쪽 팔 업데이트
        Vector3 leftShoulderPos = MediaPipeLandmarkToUnityWorld(poseLandmarks.Landmark[11]);
        Vector3 leftElbowPos = MediaPipeLandmarkToUnityWorld(poseLandmarks.Landmark[13]);
        Vector3 leftWristPos = MediaPipeLandmarkToUnityWorld(poseLandmarks.Landmark[15]);

        UpdateBoneRotation(this.LeftUpperArm, leftShoulderPos, leftElbowPos);
        UpdateBoneRotation(this.LeftLowerArm, leftElbowPos, leftWristPos);

        // 오른쪽 팔 업데이트 (동일한 방식)*/
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
        // 엄지손가락 업데이트
        UpdateFinger(
        handLandmarks,
        this.LeftThumb1, this.LeftThumb2, this.LeftThumb3, this.LeftThumb4,
        1, 2, 3, 4);

        // 검지손가락 업데이트
        UpdateFinger(
            handLandmarks,
            this.LeftIndex1, this.LeftIndex2, this.LeftIndex3, this.LeftIndex4,
            5, 6, 7, 8);

        // 중지손가락 업데이트
        UpdateFinger(
            handLandmarks,
            this.LeftMiddle1, this.LeftMiddle2, this.LeftMiddle3, this.LeftMiddle4,
            9, 10, 11, 12);

        // 약지손가락 업데이트
        UpdateFinger(
            handLandmarks,
            this.LeftRing1, this.LeftRing2, this.LeftRing3, this.LeftRing4,
            13, 14, 15, 16);

        // 새끼손가락 업데이트
        UpdateFinger(
            handLandmarks,
            this.LeftPinky1, this.LeftPinky2, this.LeftPinky3, this.LeftPinky4,
            17, 18, 19, 20);
    }

    private void UpdateRightHand(NormalizedLandmarkList handLandmarks)
    {
        // 손가락 본 업데이트
        // 엄지손가락
    }
    private void UpdateFinger(
    NormalizedLandmarkList handLandmarks,
    Transform bone1, Transform bone2, Transform bone3, Transform boneEnd,
    int index1, int index2, int index3, int indexTip)
    {
        Vector3 point1 = GetWorldPosition(handLandmarks.Landmark[index1]);
        Vector3 point2 = GetWorldPosition(handLandmarks.Landmark[index2]);
        Vector3 point3 = GetWorldPosition(handLandmarks.Landmark[index3]);
        Vector3 pointTip = GetWorldPosition(handLandmarks.Landmark[indexTip]);

        // Bone1: Joint1 -> Joint2
        UpdateBoneRotation(bone1, point1, point2);

        // Bone2: Joint2 -> Joint3
        UpdateBoneRotation(bone2, point2, point3);

        // Bone3: Joint3 -> Tip
        UpdateBoneRotation(bone3, point3, pointTip);

        float lengthFactor = (pointTip - point3).magnitude;

        // BoneEnd: Tip -> Extended Point
        Vector3 direction = (pointTip - point3).normalized;
        Vector3 endPoint = pointTip + direction * lengthFactor;
        UpdateBoneRotation(boneEnd, pointTip, endPoint);
    }

    private void UpdateFace(NormalizedLandmarkList faceLandmarks)
    {
        // 얼굴 표정 및 시선 업데이트
        // 블렌드 쉐이프 등을 활용하여 표정 제어
    }

    private void UpdateLegs(NormalizedLandmarkList poseLandmarks)
    {
        /*// 다리 본 업데이트
        // 왼쪽 다리
        Vector3 leftHipPos = MediaPipeLandmarkToUnityWorld(poseLandmarks.Landmark[23]);
        Vector3 leftKneePos = MediaPipeLandmarkToUnityWorld(poseLandmarks.Landmark[25]);
        Vector3 leftAnklePos = MediaPipeLandmarkToUnityWorld(poseLandmarks.Landmark[27]);

        UpdateBoneRotation(this.LeftUpperLeg, leftHipPos, leftKneePos);
        UpdateBoneRotation(this.LeftLowerLeg, leftKneePos, leftAnklePos);

        // 오른쪽 다리 업데이트 (동일한 방식)*/
    }

    private Vector3 GetWorldPosition(NormalizedLandmark landmark)
    {
        // MediaPipe 좌표계를 유니티 월드 좌표계로 변환하는 함수
        float x = (landmark.X - 0.5f) * widthScale;
        float y = (0.5f - landmark.Y) * heightScale; // y축 반전
        float z = -landmark.Z * depthScale;
        return new Vector3(x, y, z);
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
