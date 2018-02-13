using System;
using System.Collections;

namespace StatsCompiler
{
	public abstract class RequirementInfo:IComparable
	{
		public virtual int Priority{get{return 10;}}
		public abstract string Format(); 
		public abstract void Parse(string lua);
		public string Requirement;
		public string RequirementType;
		public BaseInfo Parent;
		public RequirementInfo(string requirement)
		{
			Requirement = requirement;
		}
		public static RequirementInfo GetRequirement(string req,string reqType)
		{
			if (reqType == "global_required_addon")
				return new GlobalRequiredAddon(req);
			if (reqType == "global_required_addon_exclusive")
				return new GlobalRequiredAddonExclusive(req);
			if (reqType == "local_required_addon")
				return new LocalRequiredAddon(req);
			if (reqType == "local_required_addon_exclusive")
				return new LocalRequiredAddonExclusive(req);
			if (reqType == "required_cap")
				return new RequiredCap(req);
			if (reqType == "required_cumulative_cap")
				return new RequiredCumulativeSquadCap(req);
			if (reqType == "required_health")
				return new RequiredHealth(req);
			if (reqType == "required_research")
				return new RequiredResearch(req);
			if (reqType == "required_squad")
				return new RequiredSquad(req);
			if (reqType == "required_squad_cap")
				return new RequiredSquadCap(req);
			if (reqType == "required_structure")
				return new RequiredStructure(req);
			if (reqType == "required_structure_ratio")
				return new RequiredStructureRatio(req);
            if (reqType == "required_structure_either")
                return new RequiredStructureEither(req);
			if (reqType == "required_total_pop")
				return new RequiredTotalPop(req);
            if (reqType == "required_ownership")
                return new RequiredOwnership(req);
            if (reqType == "required_mobvalue")
                return new RequiredMobValue(req);           
			
			return null;
		}
		
		public virtual int CompareTo(object o)
		{
			if (o==null || ! (o is RequirementInfo))
				return -1;
			
			RequirementInfo toCompare = o as RequirementInfo;

			if (Priority > toCompare.Priority)
				return 1;
			else if (Priority < toCompare.Priority)
				return -1;
			return 0;
			
		}
	}

}