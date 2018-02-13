using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace StatsCompiler
{
	public class ResearchInfo : BuildableInfo
	{
		public int ResearchTime;
			
		public ResearchInfo( ):base()
		{	
		}

        public override int CompareTo(object o)
        {
            ResearchInfo toCompare = o as ResearchInfo;
            if (toCompare != null)
            {
                foreach (RequirementInfo req in this.Requirements.Values)
                {
                    if (req is RequiredResearch)
                    {
                        RequiredResearch reqRes = (RequiredResearch)req;
                        if (Translation.Translate(reqRes.Research) == toCompare.Name)
                            return 1;
                    }
                }
                foreach (RequirementInfo req in toCompare.Requirements.Values)
                {
                    if (req is RequiredResearch)
                    {
                        RequiredResearch reqRes = (RequiredResearch)req;
                        if (Translation.Translate(reqRes.Research) == this.Name)
                            return -1;
                    }
                }

            }
            return base.CompareTo(o);
        }
    }
		
}