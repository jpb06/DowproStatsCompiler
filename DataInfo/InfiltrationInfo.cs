using System;
using System.Collections;
using System.Text;

namespace StatsCompiler
{
    public enum InfiltrationTypes
    {
        None,
        InCover,
        Permanent
    }
    public class InfiltrationInfo : BaseInfo
    { 
        private InfiltrationTypes m_InfiltrationType;
        public InfiltrationTypes InfiltrationType
        {
            get { return m_InfiltrationType; }
            set { m_InfiltrationType = value; }
        }
        public InfiltrationInfo()
        { 
            InfiltrationType = InfiltrationTypes.None;
        }
    }
}
