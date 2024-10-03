using System;
using UnityEngine;

namespace VRMAvatar
{
    [Serializable]
    public struct Mapping
    {
        [SerializeField]
        private string m_From;

        [SerializeField]
        private string m_To;

        public string from
        {
            get
            {
                return m_From;
            }
            set
            {
                m_From = value;
            }
        }

        public string to => m_To;

        public Mapping(string from, string to)
        {
            m_From = from;
            m_To = to;
        }
    }
}
