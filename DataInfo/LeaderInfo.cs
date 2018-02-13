using System;
using System.Collections;
using System.Text;

namespace StatsCompiler
{
    public class LeaderInfo : UnitInfo
    {
       
        public LeaderInfo(): base()
        {
        }

        public override bool IsIndipendant()
        {
            return false;
        }
    }
}
