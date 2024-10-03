using System.Collections.Generic;
using UnityEngine;

namespace VRMAvatar
{
    public class FaceCapResult : MonoBehaviour
    {
        public Dictionary<string, float> values = new Dictionary<string, float>();

        public void Start()
        {
            for (int i = 0; i < LiveLinkTrackingData.Names.Length; i++)
            {
                values.Add(LiveLinkTrackingData.Names[i], 0f);
            }
        }
    }
}
