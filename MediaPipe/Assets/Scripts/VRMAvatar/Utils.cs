using UnityEngine;

namespace VRMAvatar
{
    public class Utils
    {
        private static Vector3 sca = new Vector3(-1f, -1f, -1f);

        private static Vector3 scaface = new Vector3(-50f, -50f, -50f);

        public static GameObject getChildGameObject(GameObject fromGameObject, string withName)
        {
            Transform[] componentsInChildren = fromGameObject.transform.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (Transform transform in componentsInChildren)
            {
                if (transform.gameObject.name == withName)
                {
                    return transform.gameObject;
                }
            }

            return null;
        }

        public static float Remap(float from, float fromMin, float fromMax, float toMin = 0f, float toMax = 1f)
        {
            if (from > fromMax)
            {
                from = fromMax;
            }

            if (from < fromMin)
            {
                from = fromMin;
            }

            float num = from - fromMin;
            float num2 = fromMax - fromMin;
            if (num2 == 0f)
            {
                if (from < fromMin)
                {
                    return 0f;
                }

                return 1f;
            }

            float num3 = num / num2;
            return (toMax - toMin) * num3 + toMin;
        }

        public static Vector3 TriangleNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 lhs = a - b;
            Vector3 rhs = a - c;
            Vector3 result = Vector3.Cross(lhs, rhs);
            result.Normalize();
            return result;
        }

        public static Vector3 scaling(Vector3 pos)
        {
            return new Vector3(pos.x * sca.x, pos.y * sca.y, pos.z * sca.z);
        }

        public static Vector3 scalingface(Vector3 pos)
        {
            return new Vector3(pos.x * scaface.x, pos.y * scaface.y, pos.z * scaface.z);
        }
    }
}
