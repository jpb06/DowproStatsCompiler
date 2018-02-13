using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace StatsCompiler
{
	
	public class BuildingInfo : BuildableInfo
	{
		public int HitPoints;
		public int Sight;
		public int BuildTime;		
		public double PowerIncome;
		public double RequisitionIncome;
        public double RequisitionModifier;
		public ArrayList Addons;
		public ArrayList Researches;
		public ArrayList BuildUnits;
		
		public BuildingInfo():base()
		{
		}

        public override int CompareTo(object o)
        {
            if (!(o is BuildingInfo))
                return -1;

            BuildingInfo toCompare = o as BuildingInfo;

            if (Race.CompareTo(toCompare.Race) < 0)
                return -1;
            if (Race.CompareTo(toCompare.Race) > 0)
                return 1;
            
           
            
            int comp = this.ArmorType.CompareTo(toCompare.ArmorType);
                if (comp != 0)
                    return comp;
            
            return Name.CompareTo(toCompare.Name);
        }
	}
}