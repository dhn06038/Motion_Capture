using System;
using UnityEngine;

namespace VRMAvatar
{
    [Serializable]
    [CreateAssetMenu(fileName = "BlendShapeMappings", menuName = "Dollars/Face Capture Mappings")]
    public class BlendShapeMappings : ScriptableObject
    {
        [SerializeField]
        private Mapping[] m_Mappings = new Mapping[0];

        public Mapping[] mappings => m_Mappings;

        private BlendShapeMappings()
        {
            m_Mappings = new Mapping[LiveLinkTrackingData.Names.Length];
            for (int i = 0; i < LiveLinkTrackingData.Names.Length; i++)
            {
                m_Mappings[i].from = LiveLinkTrackingData.Names[i];
            }
        }
    }
}
