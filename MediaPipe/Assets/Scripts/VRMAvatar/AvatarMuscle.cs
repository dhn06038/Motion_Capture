using System;
using UnityEngine;

namespace VRMAvatar
{
    public class AvatarMuscle : MonoBehaviour
    {
        private void Start()
        {
            this.poseHandler = new HumanPoseHandler(base.GetComponent<Animator>().avatar, base.transform);
        }

        private void LateUpdate()
        {
            Transform transform = base.transform;
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            HumanPose humanPose = default(HumanPose);
            this.poseHandler.GetHumanPose(ref humanPose);
            transform.SetPositionAndRotation(position, rotation);
            for (int i = 0; i < 55; i++)
            {
                if (humanPose.muscles[i] > 1f)
                {
                    humanPose.muscles[i] = 1f;
                }
                if (humanPose.muscles[i] < -1f)
                {
                    humanPose.muscles[i] = -1f;
                }
            }
            this.poseHandler.SetHumanPose(ref humanPose);
        }

        private HumanPoseHandler poseHandler;
    }
}
