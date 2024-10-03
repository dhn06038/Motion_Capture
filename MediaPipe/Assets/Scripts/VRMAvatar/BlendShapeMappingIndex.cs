using System.Collections.Generic;

namespace VRMAvatar
{
    public class BlendShapeMappingIndex
    {
        public Dictionary<string, int> morphs;

        public BlendShapeMappingIndex()
        {
            morphs = new Dictionary<string, int>();
        }
    }
}