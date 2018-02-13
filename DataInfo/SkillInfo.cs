using System;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace StatsCompiler
{
	
	public enum AreaEffects
	{
		None,
		Point,
		Circle,
		Squad,
		Pie,
	}

	public class SkillInfo : BuildableInfo
	{
		public string ActivationType;
		public double RechargeTime;
		public double Duration;
        public bool GlobalTimer;
        public double Radius;
		public double Range;
        public double Refresh;
        public double Delay;
        public bool Track;
		public bool TargetGround;		
					
		public double MaxDamage;
		public double MinDamage;
		public double MinDamageValue;
		public double MoraleDamage;
        public double MinForce;
        public double MaxForce;
		public Hashtable Filter;
		public string AOEFilter;
		public AreaEffects AreaType;
		public UnitInfo Spawn;
        public Hashtable ArmorPiercingValues;
        public double BasePiercing;
        public SkillInfo ParentSkill;
        public SkillInfo RootParentSkill
        {
            get
            {
                if (ParentSkill == null)
                    return this;
               return ParentSkill.RootParentSkill;
            }
        }
        public SkillInfo ChildSkill;
        public bool IsChild;

        public bool IsNeeded
        {
            get
            {
                if (IsValid)
                    return true;
                if (ChildSkill != null)
                    return ChildSkill.IsNeeded;
                return IsValid;
            }
        }
        public bool IsValid
        {
            get 
            {
                if (DataDumper.UnnecessaryAbilities.Contains(LuaName))
                    return false;
                
                if (Modifiers != null)
                    foreach (ModifierInfo modInfo in Modifiers.Values)
                    {
                        if (modInfo != null && modInfo.IsValid)                        
                            return true;                        
                    }
                if (ArmorPiercingValues.Count > 0 || MoraleDamage > 0)
                    return true;
                return false;
            }
        }

        public bool IsValidDamage()
        {
            
            if (MoraleDamage>0)
                return true;
            if (ArmorPiercingValues.Count==0)
                return false;
            if (MaxDamage == 0)
                return false;
            else
                foreach (ArmorPiercing ar in ArmorPiercingValues.Values)
                    if (ar.PiercingValue > 0)
                        return true;
            return false;
        }

		public SkillInfo( ):base()
		{
			Filter=new Hashtable();
            ArmorPiercingValues = new Hashtable();
		}
		
	
		public string GetActivationType()
		{
			if (ActivationType == "tp_ability_activation_toggled")
				return "Toggled Ability";
			if (ActivationType == "tp_ability_activation_timed")
				return "Activated Ability";
			if (ActivationType == "tp_ability_activation_targeted")
				return "Target Ability";			
			return "Passive Ability";
		}

		private static string FormatSeparator(int current,int count)
		{
			if (current == count-2)
				return " and ";
			if (current == count-1)
				return ".";
			if (current < count-1)
				return ", ";
			return "";
		}

        public static AreaEffects GetAreaType(string AOEType)
        {
            if (AOEType == "tp_area_effect_circle")
                return AreaEffects.Circle;
            if (AOEType == "tp_area_effect_pie")
                return AreaEffects.Pie;
            if (AOEType == "tp_area_effect_point")
                return AreaEffects.Point;
            if (AOEType == "tp_area_effect_squad")
                return AreaEffects.Squad;
            return AreaEffects.None;
                
        }
        public string FormatAlliance()
		{
			if (AOEFilter == "tp_area_filter_allied")
				return "Allied ";
			if (AOEFilter == "tp_area_filter_enemy")
				return "Enemy ";
			if (AOEFilter == "tp_area_filter_own")
				return "Friendly "; 			
			return "";
		}
		public string FormatFilter()
		{
			string output="";
			string temp="";            
          
			if (this.Radius > 0)
			{
			}
			
			int count=0;
            foreach (ArmorTypes ar in Filter.Values)
            {
                if (ar >= ArmorTypes.tp_infantry_low && ar <= ArmorTypes.tp_infantry_heavy_high)
                {
                    count++;
                    temp += Translation.Translate(ar.ToString())+"*";
                }
            }
			if (count >= 6)
				output += "Infantry*";
			else if (count > 0)
				output += temp;
			temp="";
			count=0;
            foreach (ArmorTypes ar in Filter.Values)
            {
                if (ar >= ArmorTypes.tp_monster_low && ar <= ArmorTypes.tp_monster_high)
                {
                    count++;
                    temp += Translation.Translate(ar.ToString())+"*";
                }
            }
			if (count == 3)
				output += "Monsters*";
			else if (count > 0)
				output += temp;
			temp="";
			count=0;
            foreach (ArmorTypes ar in Filter.Values)
            {
                if (ar == ArmorTypes.tp_air_low || (ar >= ArmorTypes.tp_vehicle_low && ar <= ArmorTypes.tp_vehicle_high))
                {
                    count++;
                    temp += Translation.Translate(ar.ToString())+"*";
                }
            }
			if (count == 4)
				output += "Vehicles*";
			else if (count > 0)
				output += temp;
			temp="";
			count=0;
            foreach (ArmorTypes ar in Filter.Values)
            {
                if (ar >= ArmorTypes.tp_building_low && ar <= ArmorTypes.tp_building_high)
                {
                    count++;
                    temp += Translation.Translate(ar.ToString())+"*";
                }
            }
			if (count == 3)
				output += "Buildings*";
			else if (count > 0)
				output += temp;
            ArrayList filtersList = new ArrayList(Filter.Values);
			if (filtersList.Contains(ArmorTypes.tp_commander))
				output+= "Commanders*";
			
			output = Regex.Replace(output, @"\*(?<word>[^\*]*)\*$" , @" and ${word}" );			
			output = Regex.Replace(output,@"\*$",@"");
			output = Regex.Replace(output,@"\*",@", ");

			
			if (this.Radius > 0)
			{
				if (ActivationType == "tp_ability_activation_targeted")
					output+= " in the area of effect.";
				else
					output+= " in the surrounding area.";	
			}
			else if (Filter.Count>0)
			{				                
				if (ActivationType != "tp_ability_activation_targeted")
                    output += " in the squad.";
			}

			return output;
		}

        public bool CheckLoop(string child)
        {
            if (child == this.LuaName)
                return false;
            if (IsChild)
                return ParentSkill.CheckLoop(child);
            return true;
        }
        
        public void Parse(string lua)
        {
            Parse(lua, false, false);
        }
		public void Parse(string lua, bool inherited, bool isChild)
		{
            
            if (lua != "" && !lua.EndsWith(".lua") && !lua.EndsWith(".nil"))
                lua += ".lua";
            IsChild = isChild;
            string path = DataPath.GetPath("abilities\\"+lua);
            
			if (path != "")
			{
                
				StreamReader file = new StreamReader(File.OpenRead(path),System.Text.Encoding.ASCII);
				file.BaseStream.Seek(0, SeekOrigin.Begin);

				string s = file.ReadToEnd();
			
				file.Close();

                #region CHILD ABILITY

                if (ChildSkill==null)
                {
                    Match mtc = Regex.Match(s, @"GameData\[""child_ability_name""\]\s=\s""(?<child>.*)""");
                    if (mtc.Success)
                    {
                        string child = mtc.Groups["child"].Value;

                        if (child != "" && CheckLoop(child))
                        {
                            ChildSkill = new SkillInfo();
                            ChildSkill.Parent = Parent;
                            ChildSkill.LuaName = child;
                            ChildSkill.Name = child;
                            ChildSkill.ParentSkill = this;

                            ChildSkill.Parse(child, false, true);
                        }
                    }
                }
                #endregion

                #region INHERITANCE
				Match mc = Regex.Match( s , @"GameData\s=\sInherit\(\[\[abilities\\(?<inherit>.*)\]\]\)" );
				if (mc.Success)
				{
					Group grp = mc.Groups["inherit"];
                    if (grp.Success && grp.Value != "")
                    {
                        Parse(grp.Value, true, isChild);
                    }
				}
				#endregion
			
				double numValue = 0.0;
				string stringValue = "";


                if (LuaParser.ReadNumericValue(s, @"GameData\[""refresh_time""\]\s=\s(?<refresh>.*)", out numValue))
                    Refresh = numValue;
                
				if (LuaParser.ReadStringValue(s,@"GameData\[""activation""\]\s=\sReference\(\[\[type_abilityactivation\\(?<actvationType>.*)\.lua\]\]\)",out stringValue))
					ActivationType = stringValue;
				if (LuaParser.ReadNumericValue(s,@"GameData\[""duration_time""\]\s=\s(?<duration>.*)",out numValue))
					Duration = numValue;
                if (LuaParser.ReadNumericValue(s, @"GameData\[""initial_delay_time""\]\s=\s(?<duration>.*)", out numValue))
                    Delay = numValue;
				if (LuaParser.ReadNumericValue(s,@"GameData\[""recharge_time""\]\s=\s(?<duration>.*)",out numValue))
					RechargeTime = numValue;
                if (Regex.Match(s, @"GameData\[""recharge_timer_global""\]\s=\strue").Success)
                    GlobalTimer = true;
                if (LuaParser.ReadNumericValue(s,@"GameData\[""fire_cost""\]\[""requisition""\]\s=\s(.*)",out numValue))
					RequisitionCost = (int)numValue; 
				if (LuaParser.ReadNumericValue(s,@"GameData\[""fire_cost""\]\[""power""\]\s=\s(.*)",out numValue))
					PowerCost = (int)numValue; 
				if (LuaParser.ReadNumericValue(s,@"GameData\[""area_effect""\]\[""area_effect_information""\]\[""radius""\]\s=\s(.*)",out numValue))
					Radius = numValue; 
				
				if (LuaParser.ReadNumericValue(s,@"GameData\[""range""\]\s=\s(.*)",out numValue))
					Range = numValue; 
				
				if (LuaParser.ReadNumericValue(s,@"GameData\[""time_cost""\]\[""cost""\]\[""power""\]\s=\s(.*)",out numValue))
					PowerCost = (int)numValue; 		
				if (LuaParser.ReadNumericValue(s,@"GameData\[""time_cost""\]\[""time_seconds""\]\s=\s(.*)",out numValue))
					RechargeTime = (int)numValue;
				
				if (LuaParser.ReadStringValue(s,@"GameData\[""area_effect""\]\[""area_effect_information""\]\[""area_type""\]\s=\sReference\(\[\[type_areaeffect\\(.*)\.lua\]\]\)",out stringValue ))
					AreaType= GetAreaType( stringValue);
                
                if (LuaParser.ReadStringValue(s, @"GameData\[""area_effect""\]\[""area_effect_information""\]\[""filter_type""\]\s=\sReference\(\[\[type_areafilter\\(.*)\.lua\]\]\)", out stringValue))
                    AOEFilter = stringValue;
                
                if (Regex.Match(s, @"GameData\[""target_ground""\]\s=\strue").Success)
                    TargetGround = true;

                if (LuaParser.ReadStringValue(s, @"GameData\[""spawned_entity_name""\]\s=\s""(?<spawn>.*\.lua)""", out stringValue))
                {
                    bool loopcheck = true;
                    UnitInfo root = null;
                    if (this.RootParent is UnitInfo)
                    {
                        root = RootParent as UnitInfo;
                        string spawnName = Regex.Replace(stringValue, @".*\\\\","");
                        if (root.FileName == spawnName)
                            loopcheck = false;   
                    }
                    if (loopcheck)
                    {
                        UnitInfo uInfo = new UnitInfo();
                        LuaParser.ParseUnit(DataPath.GetPath(Regex.Replace(stringValue, @"\\\\", "\\")), uInfo);
                        Spawn = uInfo;
                    }
                }
                if (LuaParser.ReadStringValue(s, @"GameData\[""ui_info""\]\[""icon_name""\]\s=\s""(.*)""", out stringValue))
                {
                    Icon = stringValue;
                    
                }

               
                if (LuaParser.ReadNumericValue(s, @"GameData\[""area_effect""\]\[""weapon_damage""\]\[""armour_damage""\]\[""armour_piercing""\]\s=\s(.*)", out numValue))
                    BasePiercing = numValue;
                if (LuaParser.ReadNumericValue(s, @"GameData\[""area_effect""\]\[""weapon_damage""\]\[""armour_damage""\]\[""morale_damage""\]\s=\s(.*)", out numValue))
                    MoraleDamage = numValue;
                
                if (LuaParser.ReadNumericValue(s, @"\[""ui_info""\]\[""screen_name_id""\]\s=\s""\$(.*)""", out numValue))
                    UI = (int)numValue;
                
                string luaName = Regex.Replace(path, @".*\\", "");
                
                if (!inherited && !isChild)
                    Name = Translation.Translate(UI, luaName);

                               
                MatchCollection mtcs = Regex.Matches(s,@"GameData\[""area_effect""\]\[""area_effect_information""\]\[""target_filter""\]\[""(?<filter_entry>entry_[0-9][0-9])""\]\s=\sReference\(\[\[type_armour\\(?<armour_type>.*)\.lua\]\]\)");
				foreach (Match m in mtcs)
				{
					
                    ArmorTypes armor;
                    string entry = m.Groups["filter_entry"].Value;
                    string armourType = m.Groups["armour_type"].Value;
                    try
                    {
                        armor = (ArmorTypes)Enum.Parse(typeof(ArmorTypes), armourType);
                    }
                    catch { armor = ArmorTypes.unknown; }

                    if (!Filter.ContainsKey(entry))
                        Filter.Add(entry, armor);
                    else if (armor != ArmorTypes.unknown)
                        Filter[entry] = armor;
                    else Filter.Remove(entry);
                    
				}
                
                #region DPS     

                if (LuaParser.ReadNumericValue(s, @"GameData\[""area_effect""\]\[""weapon_damage""\]\[""armour_damage""\]\[""min_damage""\]\s=\s(.*)", out numValue))
                    MinDamage = numValue;
                if (LuaParser.ReadNumericValue(s, @"GameData\[""area_effect""\]\[""weapon_damage""\]\[""armour_damage""\]\[""max_damage""\]\s=\s(.*)", out numValue))
                    MaxDamage = numValue;
                if (LuaParser.ReadNumericValue(s, @"GameData\[""area_effect""\]\[""throw_data""\]\[""force_min""\]\s=\s(.*)", out numValue))
                    MinForce = numValue;
                if (LuaParser.ReadNumericValue(s, @"GameData\[""area_effect""\]\[""throw_data""\]\[""force_max""\]\s=\s(.*)", out numValue))
                    MaxForce = numValue;
                

                MatchCollection aMcs = Regex.Matches(s, @"GameData\[""area_effect""\]\[""weapon_damage""\]\[""armour_damage""\]\[""armour_piercing_types""\]\[""entry_(?<armor_entry>[0-9][0-9])""\]\[""armour_type""\]\s=\sReference\(\[\[type_armour\\(?<armor_type>.*)\.lua\]\]\)");

                foreach (Match m in aMcs) // Retrieve ArmorTypes entries
                {
                    string armor = m.Groups["armor_type"].Value;

                   
                    int armorEntry = System.Convert.ToInt32(Regex.Replace(m.Groups["armor_entry"].Value, "\b0", ""), LuaParser.NumberFormat);

                    if (!ArmorPiercingValues.Contains(armorEntry))
                    {
                        ArmorPiercing ap = new ArmorPiercing();
                        ap.Entry = armorEntry;
                        try
                        {
                            ap.ArmorType = (ArmorTypes)Enum.Parse(typeof(ArmorTypes), armor);
                        }
                        catch { ap.ArmorType = ArmorTypes.unknown; }
                        ArmorPiercingValues.Add(armorEntry, ap);
                    }
                    else
                    {
                        ArmorPiercing ap = ArmorPiercingValues[armorEntry] as ArmorPiercing;
                        ap.Entry = armorEntry;
                        try
                        {
                            ap.ArmorType = (ArmorTypes)Enum.Parse(typeof(ArmorTypes), armor);
                        }
                        catch { ap.ArmorType = ArmorTypes.unknown; }
                    }
                }

                foreach (ArmorPiercing ap in ArmorPiercingValues.Values)
                {
                    if (ap.ArmorType != ArmorTypes.unknown)
                    {
                        string subIndex = "";
                        int j = ap.Entry;
                        if (j < 10) subIndex = "0" + j.ToString();
                        else subIndex = j.ToString();
                        if (LuaParser.ReadNumericValue(s, @"GameData\[""area_effect""\]\[""weapon_damage""\]\[""armour_damage""\]\[""armour_piercing_types""\]\[""entry_+" + subIndex + @"""\]\[""armour_piercing_value""\]\s=\s(.*)", out numValue))
                            ap.PiercingValue = numValue;
                    }
                    else ap.PiercingValue = 0.0;
                }
                #endregion
        
   
				LuaParser.ParseToolTips(s,this);
				LuaParser.ParseRequirements(s,this);
                LuaParser.ParseModifiers(s, this, this.Modifiers, AreaOfEffectTypes.area_effect);
                
            }
		}
	
	}
		
}