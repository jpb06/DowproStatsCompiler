using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace StatsCompiler
{
	public class GlobalRequiredAddon : RequirementInfo
	{
		public override int Priority{get{return 4;}}
		public string AddOn;
		
		public override string Format()
		{
			return Translation.Translate(AddOn);
		} 
		public override void Parse(string lua)
		{
			Match m = Regex.Match(lua,@"\[""requirements""\]\["""+Requirement+@"""\]\[""global_addon_name""\]\s=\s""(.*\\\\(?<addon>.*).lua|(?<addon>.*))""");
			if (m.Success)
				AddOn = m.Groups["addon"].Value;
		}		
		public GlobalRequiredAddon(string req):base(req)
		{	
			AddOn="";
			RequirementType="global_required_addon";
		}
	}
    public class RequiredMobValue : RequirementInfo
    {
        public override int Priority { get { return 4; } }
        public int MobRequired;
        public int ProximityRequired;
        public bool SquadActivated;

        public override string Format()
        {            
            if (SquadActivated)
                return "A Mob Value of " + MobRequired + " in a radius of " + ProximityRequired; 
            return "A Mob Value of " + MobRequired + " and a Big Mek or Warboss, in a radius of " + ProximityRequired; 

        }
        public override void Parse(string lua)
        {
            double numValue = 0.0;
            string stringValue="";
            if (LuaParser.ReadNumericValue(lua, @"\[""mobvalue_required""\]\s=\s(?<value>.*)", out numValue))
                MobRequired = System.Convert.ToInt32(numValue);
            if (LuaParser.ReadNumericValue(lua, @"\[""proximity_required""\]\s=\s(?<value>.*)", out numValue))
                ProximityRequired = System.Convert.ToInt32(numValue);
            if (LuaParser.ReadStringValue(lua, @"\[""squad_activated""\]\s=\s(true)", out stringValue))
                SquadActivated = true;
            
        }
        public RequiredMobValue(string req): base(req)
        {
            RequirementType = "required_mobvalue";
        }
    }	
	
	public class GlobalRequiredAddonExclusive : RequirementInfo
	{
		public string AddOn;
		
		public override string Format()
		{
			return "Mutually exclusive with "+Translation.Translate(AddOn);
		} 
		public override void Parse(string lua)
		{
			Match m = Regex.Match(lua,@"\[""requirements""\]\["""+Requirement+@"""\]\[""global_mutually_exclusive_with""\]\s=\s"".*\\\\(.*).lua""");
			if (m.Success)
				AddOn = m.Groups[1].Value;
		}		
		public GlobalRequiredAddonExclusive(string req):base(req)
		{
			AddOn="";
			RequirementType="global_required_addon_exclusive";
		}
	}
		
	
	public class LocalRequiredAddon : RequirementInfo
	{
		public override int Priority{get{return 4;}}
		public string AddOn;
        public bool Replace;
		
		public override string Format()
		{
            if (Replace)
			    return  Translation.Translate(AddOn)+" (Replaced)";
            return Translation.Translate(AddOn);
		} 
		public override void Parse(string lua)
		{
			Match m = Regex.Match(lua,@"\[""requirements""\]\["""+Requirement+@"""\]\[""addon_name""\]\s=\s"".*\\\\(.*).lua""");
			if (m.Success)
				AddOn = m.Groups[1].Value;
            m = Regex.Match(lua,@"\[""requirements""\]\["""+Requirement+@"""\]\[""replace_when_done""\]\s=\strue");
            if (m.Success)
                Replace = true;
		}		
		public LocalRequiredAddon(string req):base(req)
		{
			AddOn="";
			RequirementType="local_required_addon";
		}
	}

	
	public class LocalRequiredAddonExclusive : RequirementInfo
	{
		public string AddOn;
		
		public override string Format()
		{
			return "Mutually exclusive with "+Translation.Translate(AddOn);
		} 
		public override void Parse(string lua)
		{
			Match m = Regex.Match(lua,@"\[""requirements""\]\["""+Requirement+@"""\]\[""mutually_exclusive_with""\]\s=\s"".*\\\\(.*).lua""");
			if (m.Success)
				AddOn = m.Groups[1].Value;
		}		
		public LocalRequiredAddonExclusive(string req):base(req)
		{
			AddOn="";
			RequirementType="local_required_addon_exclusive";
		}
	}
		
	
	public class RequiredCap : RequirementInfo
	{
		public override int Priority{get{return 0;}}
		public int Cap;
		
		public override string Format()
		{
			return "Limit: "+ Cap.ToString();
		} 
		public override void Parse(string lua)
		{
			Match m = Regex.Match(lua,@"\[""requirements""\]\["""+Requirement+@"""\]\[""max_cap""\]\s=\s(.*)");
			if (m.Success)
				Cap = (int)System.Convert.ToDouble(m.Groups[1].Value,LuaParser.NumberFormat);

		}		
		
		
		public RequiredCap(string req):base(req)
		{	
			Cap=0;
			RequirementType="required_cap";
		}
	}
	
	
	public class RequiredCumulativeSquadCap : RequirementInfo
	{
		public override int Priority{get{return 1;}}
		public int Cap;
		public string[] Squads;		
		
		public override string Format()
		{
			string output="";
			for (int i=0; i< Squads.Length; i++)
			{
				output += Translation.Translate(Squads[i]);
				if (i== Squads.Length-2)
					output+= " and ";
				else if (i == Squads.Length-1)
					output+=".";
				else output+=", ";
			}
			return "Shared Cap of "+Cap+" with "+output;
		} 
		
		public override void Parse(string lua)
		{
			Match m = Regex.Match(lua,@"\[""requirements""\]\["""+Requirement+@"""\]\[""max_cumulative_squad_cap""\]\s=\s(.*)");
			if (m.Success)
				Cap = (int)System.Convert.ToDouble(m.Groups[1].Value,LuaParser.NumberFormat);

			MatchCollection mcc = Regex.Matches(lua,@"GameData\[""squad_requirement_ext""\]\[""requirements""\]\["""+Requirement+@"""\]\[""squad_table""\]\[""squad_.+""\]\s=\s"".*\\\\(.*).lua""");
			if (mcc.Count>0)
			{
				Squads=new string[mcc.Count];
				int i=0;
				foreach(Match s in mcc)
					Squads[i]=s.Groups[1].Value;				
			}
		}		
		
		
		public RequiredCumulativeSquadCap(string req):base(req)
		{	
			Cap=1;
			Squads= new string[]{""};
			RequirementType="required_cumulative_cap";
		}
	}
		

	public class RequiredHealth : RequirementInfo
	{
		public override int Priority{get{return 1;}}
		public double HealthFraction;
			
		public override string Format()
		{
			return "More than "+HealthFraction+"% of the total Health";
		} 
		public override void Parse(string lua)
		{
			Match m = Regex.Match(lua,@"\[""requirements""\]\["""+Requirement+@"""\]\[""min_health_fraction""\]\s=\s(.*)");
			if (m.Success)
				HealthFraction =100*System.Convert.ToDouble(m.Groups[1].Value,LuaParser.NumberFormat);
		}		
		
		
		
		public RequiredHealth(string req):base(req)
		{
			RequirementType="required_health";
		}
	}

	
	public class RequiredResearch : RequirementInfo
	{
		public string Research;
        public bool MustNotBeCompleted;
		
		public override string Format()
		{
			return Translation.Translate(Research)+ (MustNotBeCompleted?" must not be complete":"");
		} 
		
        public override void Parse(string lua)
		{
			Match m = Regex.Match(lua,@"\[""requirements""\]\["""+Requirement+@"""\]\[""research_name""\]\s=\s""(.*\\\\)?(?<research>.*)(\.lua)?""");
			if (m.Success)
				Research = m.Groups["research"].Value;
            m = Regex.Match(lua, @"\[""requirements""\]\[""" + Requirement + @"""\]\[""research_must_not_be_complete""\]\s=\strue");
            if (m.Success)
                MustNotBeCompleted = true;
        }		
		
		
		public RequiredResearch(string req):base(req)
		{
			Research="";
			RequirementType="required_research";            
		}
	}


	public class RequiredSquad : RequirementInfo
	{
		public override int Priority{get{return 5;}}
		public string Squad;
		public int Count;
		
		public override string Format()
		{
            if (Count > 1)
                return Count.ToString() + " " + HtmlCompiler.GetPlural(Translation.Translate(Squad));
            return Translation.Translate(Squad);
		} 
		public override void Parse(string lua)
		{
			Match m = Regex.Match(lua,@"\[""requirements""\]\["""+Requirement+@"""\]\[""squad_name""\]\s=\s"".*\\\\(.*)\.lua""");
			if (m.Success)
				Squad = m.Groups[1].Value;

			m = Regex.Match(lua,@"\[""requirements""\]\["""+Requirement+@"""\]\[""min_count""\]\s=\s(.*)");
			
			if (m.Success)
				Count = (int)System.Convert.ToDouble(m.Groups[1].Value,LuaParser.NumberFormat);
		}		
	
		
		public RequiredSquad(string req):base(req)
		{
			Count=1;
			Squad="";
			RequirementType="required_squad";
		}
	}

	
	public class RequiredSquadCap : RequirementInfo
	{
		public override int Priority{get{return 0;}}
		public int SquadCap;
			
		public override string Format()
		{
			return "Limit: "+SquadCap.ToString();
		} 
		public override void Parse(string lua)
		{
			Match m = Regex.Match(lua,@"\[""requirements""\]\["""+Requirement+@"""\]\[""max_squad_cap""\]\s=\s(.*)");
			if (m.Success)
				SquadCap =(int)System.Convert.ToDouble(m.Groups[1].Value,LuaParser.NumberFormat);
		}
		
		
		public RequiredSquadCap(string req):base(req)
		{
			SquadCap=1;
			RequirementType="required_squad_cap";
		}
	}

    public class RequiredOwnership : RequirementInfo
    {
        public override int Priority { get { return 2; } }
        public string OwnName;
        public int Count;

        public override string Format()
        {
            string output = "";
            if (Count > 1)
            {
                output += Count.ToString() + " ";
                output += HtmlCompiler.GetPlural(Translation.Translate(OwnName));
            }
            else output = Translation.Translate(OwnName);
            return output;
        }
        public override void Parse(string lua)
        {
            Match m = Regex.Match(lua, @"\[""requirements""\]\[""" + Requirement + @"""\]\[""own_name""\]\s=\s""(.*\\\\)?(?<own>.*)(\.lua)?""");
            if (m.Success)
                OwnName = m.Groups["own"].Value;
            m = Regex.Match(lua, @"\[""requirements""\]\[""" + Requirement + @"""\]\[""owned_count""\]\s=\s(?<count>.*)");
            if (m.Success)
                Count = (int)System.Convert.ToDouble(m.Groups["count"].Value, LuaParser.NumberFormat);

        }


        public RequiredOwnership(string req): base(req)
        {
            OwnName = "";
            RequirementType = "required_ownership";
        }
    }
	
	public class RequiredStructure : RequirementInfo
	{
		public override int Priority{get{return 2;}}
		public string Structure;
		
		public override string Format()
		{
			
			return Translation.Translate(Structure);
		} 
		public override void Parse(string lua)
		{            
            Match m = Regex.Match(lua,@"\[""requirements""\]\["""+Requirement+@"""\]\[""structure_name""\]\s=\s""(.*\\\\)?(?<structure>.*)(\.lua)?""");
			if (m.Success)
				Structure = m.Groups["structure"].Value;
		}		
		
		
		public RequiredStructure(string req):base(req)
		{	
			Structure="";
			RequirementType="required_structure";
		}
	}

    public class RequiredStructureEither : RequirementInfo
    {
        public override int Priority { get { return 2; } }
        public string Structure1;
        public string Structure2;

        public override string Format()
        {
            return "Either "+ Translation.Translate(Structure1)+" or "+Translation.Translate(Structure2);
        }
        public override void Parse(string lua)
        {
            Match m = Regex.Match(lua, @"\[""requirements""\]\[""" + Requirement + @"""\]\[""structure_name_either""\]\s=\s""(.*\\\\)?(?<either>.*)(\.lua)?""");
            if (m.Success)
                Structure1 = m.Groups["either"].Value;
            m = Regex.Match(lua, @"\[""requirements""\]\[""" + Requirement + @"""\]\[""structure_name_or""\]\s=\s"".*\\\\(?<or>.*)\.lua""");
            if (m.Success)
                Structure2 = m.Groups["or"].Value;
        }


        public RequiredStructureEither(string req): base(req)
        {
            RequirementType = "required_structure";
        }
    }
	
	public class RequiredStructureRatio : RequirementInfo
	{
		public override int Priority{get{return 3;}}
		public string Structure;
		public int Count;
		
		public override string Format()
		{
            if (Count > 1)
                return Count.ToString() + " " + HtmlCompiler.GetPlural(Translation.Translate(Structure));
            return Translation.Translate(Structure);
		} 
		public override void Parse(string lua)
		{
			Match m = Regex.Match(lua,@"\[""requirements""\]\["""+Requirement+@"""\]\[""required_structure_name""\]\s=\s"".*\\\\(.*)\.lua""");
			if (m.Success)
				Structure = m.Groups[1].Value;
			m = Regex.Match(lua,@"\[""requirements""\]\["""+Requirement+@"""\]\[""required_structure_count""\]\s=\s(.*)");
			if (m.Success)
                Count = (int)System.Convert.ToDouble(m.Groups[1].Value, LuaParser.NumberFormat);
		}		
		
		
		public RequiredStructureRatio(string req):base(req)
		{	
			Structure="";
			Count=1;
			RequirementType="required_structure_ratio";
		}
	}
		
	
	public class RequiredTotalPop : RequirementInfo
	{
		public override int Priority{get{return 1;}}
		public int TotalPop;
			
		public override string Format()
		{
			return "Population: "+TotalPop.ToString();
		} 
		public override void Parse(string lua)
		{
			Match m = Regex.Match(lua,@"\[""requirements""\]\["""+Requirement+@"""\]\[""population_required""\]\s=\s(.*)");
			if (m.Success)
                TotalPop = (int)System.Convert.ToDouble(m.Groups[1].Value, LuaParser.NumberFormat);
		}		
		
		
		
		public RequiredTotalPop(string req):base(req)
		{
			TotalPop=0;
			RequirementType="required_total_pop";
		}
	}


	

}