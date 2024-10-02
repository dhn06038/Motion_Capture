using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class MotionCaptureAvatar : MonoBehaviour
{
    // Start is called before the first frame update
    public UDPReceive udpReceive;

    float scale_ratio = 0.001f;
    float heal_position = 0.05f; 
    float head_angle = 15f;

    Vector3[] poseLandmarks = new Vector3[33];

    Animator anim;
    float play_time;

    Transform[] bone_t;

    Vector3 init_position;
    int bone_num = 17;
    Quaternion[] init_rot;
    Quaternion[] init_inv;

    Vector3[] now_pos = new Vector3[17];

    int[] bones = new int[10] { 1, 2, 4, 5, 7, 8, 11, 12, 14, 15 };
    int[] child_bones = new int[10] { 2, 3, 5, 6, 8, 10, 12, 13, 15, 16 };
    void Start()
    {
        anim = GetComponent<Animator>();
        play_time = 0;
        GetInitInfo();
    }

    void GetInitInfo()
    {
        bone_t = new Transform[bone_num];
        init_rot = new Quaternion[bone_num];
        init_inv = new Quaternion[bone_num];

        bone_t[0] = anim.GetBoneTransform(HumanBodyBones.Hips);
        bone_t[1] = anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        bone_t[2] = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        bone_t[3] = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        bone_t[4] = anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        bone_t[5] = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        bone_t[6] = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        bone_t[7] = anim.GetBoneTransform(HumanBodyBones.Spine);
        bone_t[8] = anim.GetBoneTransform(HumanBodyBones.Neck);
        bone_t[10] = anim.GetBoneTransform(HumanBodyBones.Head);
        bone_t[11] = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        bone_t[12] = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        bone_t[13] = anim.GetBoneTransform(HumanBodyBones.LeftHand);
        bone_t[14] = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
        bone_t[15] = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
        bone_t[16] = anim.GetBoneTransform(HumanBodyBones.RightHand);

        print(bone_t[0]);
        Vector3 init_forward = TriangleNormal(bone_t[7].position, bone_t[4].position, bone_t[1].position);
        init_inv[0] = Quaternion.Inverse(Quaternion.LookRotation(init_forward));

        init_position = bone_t[0].position;
        init_rot[0] = bone_t[0].rotation;
        for (int i = 0; i < bones.Length; i++)
        {
            int b = bones[i];
            int cb = child_bones[i];

            init_rot[b] = bone_t[b].rotation;
            init_inv[b] = Quaternion.Inverse(Quaternion.LookRotation(bone_t[b].position - bone_t[cb].position, init_forward));
        }
    }

    Vector3 TriangleNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 d1 = a - b;
        Vector3 d2 = a - c;

        Vector3 dd = Vector3.Cross(d1, d2);
        dd.Normalize();

        return dd;
    }

    // Update is called once per frame
    void Update()
    {
        string data = udpReceive.data;
        data = data.Remove(0, 1);
        data = data.Remove(data.Length - 1, 1);

        string[] points = data.Split(',');

        //0        1*3      2*3
        //x1,y1,z1,x2,y2,z2,x3,y3,z3

        for (int i = 0; i < 33; i++)
        {

            float x = 7 - float.Parse(points[i * 3]) / 100;
            float y = float.Parse(points[i * 3 + 1]) / 100;
            float z = -float.Parse(points[i * 3 + 2]) / 100;

            poseLandmarks[i] = new Vector3(x, y, z);
            print(poseLandmarks[i]);
        }


        now_pos[0] = (poseLandmarks[23] + poseLandmarks[24]) * 0.5f;  // Hips
        now_pos[1] = poseLandmarks[24];  // RightUpperLeg
        now_pos[2] = poseLandmarks[26];  // RightLowerLeg
        now_pos[3] = poseLandmarks[28];  // RightFoot
        now_pos[4] = poseLandmarks[23];  // LeftUpperLeg
        now_pos[5] = poseLandmarks[25];  // LeftLowerLeg
        now_pos[6] = poseLandmarks[27];  // LeftFoot

        // Spine
        now_pos[7] = ((poseLandmarks[11] + poseLandmarks[12]) * 0.5f + now_pos[0]) * 0.5f;  // Spine

        // Neck
        now_pos[8] = (poseLandmarks[0] + (poseLandmarks[11] + poseLandmarks[12]) * 0.5f) * 0.5f;  // Neck

        now_pos[10] = poseLandmarks[0];  // Head
        now_pos[11] = poseLandmarks[11];  // LeftUpperArm
        now_pos[12] = poseLandmarks[13];  // LeftLowerArm
        now_pos[13] = poseLandmarks[15];  // LeftHand
        now_pos[14] = poseLandmarks[12];  // RightUpperArm
        now_pos[15] = poseLandmarks[14];  // RightLowerArm
        now_pos[16] = poseLandmarks[16];  // RightHand

        Vector3 pos_forward = TriangleNormal(now_pos[7], now_pos[4], now_pos[1]);
        bone_t[0].position = now_pos[0] * scale_ratio + new Vector3(init_position.x, heal_position, init_position.z);
        bone_t[0].rotation = Quaternion.LookRotation(pos_forward) * init_inv[0] * init_rot[0];
        
        for (int i = 0; i < bones.Length; i++)
        {
            int b = bones[i];
            int cb = child_bones[i];
            bone_t[b].rotation = Quaternion.LookRotation(now_pos[b] - now_pos[cb], pos_forward) * init_inv[b] * init_rot[b];
        }

        bone_t[8].rotation = Quaternion.AngleAxis(head_angle, bone_t[11].position - bone_t[14].position) * bone_t[8].rotation;
    }
}