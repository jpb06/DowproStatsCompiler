using System;
using System.Collections.Generic;
using System.Text;

namespace StatsCompiler
{
    public enum JumpTypes
    {
        None,
        Jump,
        Teleport
    }
    public class JumpInfo : BaseInfo
    {
        public JumpTypes JumpType;
        public double Jumps;
        public double JumpRecharge;
        public double JumpRange;
        public JumpInfo()
        {
            JumpType = JumpTypes.None;
        }
    }

}
