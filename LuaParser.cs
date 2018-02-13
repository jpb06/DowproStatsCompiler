using System;
using System.Collections;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;


namespace StatsCompiler
{
    public sealed class LuaParser
	{
        public static NumberFormatInfo NumberFormat;
        public static readonly LuaParser instance = new LuaParser();
        private LuaParser()
        {
            NumberFormat = new NumberFormatInfo(); 
            NumberFormat.NumberDecimalSeparator = ".";                                  
        }

        		
		/// <summary>
		/// Search for a number which match given criteria, return true if the the criteria is matched. The value is stored in the Value 
		/// </summary>
		public static bool ReadNumericValue(string s,string criteria, out double Value)
		{
			Value = 0.0;
			Match mc = Regex.Match(s,criteria);
			if (mc.Success)
			{
				Group grp = mc.Groups[1];
				if (grp.Success)
				{
                    try
                    {
                        Value = Math.Round(System.Convert.ToDouble(grp.Value, NumberFormat), 2);
                    }
                    catch{}				
                    return true;
				}
			}			
			return false;
		}
		public static bool ReadStringValue(string s,string criteria, out string Value)
		{
			Value ="";
			Match mc = Regex.Match(s,criteria);
			if (mc.Success)
			{
				Group grp = mc.Groups[1];
				if (grp.Success)
				{
					Value = grp.Value;
					return true;
				}
			}
			return false;
		}
		public static string ReadStringValue(string s,string criteria)
		{
			Match mc = Regex.Match(s,criteria);
			if (mc.Success)
			{
				Group grp = mc.Groups[1];
				if (grp.Success)
					return grp.Value;
			}
			return "";
		}
        public static bool ReadBooleanValue(string s, string criteria, out bool Value)
        {
            string val = "";
            Value = false;
            Match mc = Regex.Match(s, criteria);
            if (mc.Success)
            {
                Group grp = mc.Groups[1];
                if (grp.Success)
                {
                    val = grp.Value.Trim().ToLower();
                    if (val == "true")
                        Value = true;
                    
                    return true;
                }
            }
            return false;
        }
    #region parsing functions

		public static void ParseSquad(string path, SquadInfo info)
		{
			if (path!=null &&path != "")
			{
				StreamReader file = new StreamReader(File.OpenRead(path),System.Text.Encoding.ASCII);
				file.BaseStream.Seek(0, SeekOrigin.Begin);

				string s = file.ReadToEnd();

				file.Close();
			#region INHERITANCE						
				Match mc = Regex.Match( s , @"GameData\s=\sInherit\(\[\[(?<inherit>.*)\]\]\)" );
				Group grp = mc.Groups["inherit"];
				if (grp.Success)
					ParseSquad(DataPath.GetPath(grp.Value),info);
				
			#endregion
				double numValue=0.0;		
			#region SQUAD STATS
			
                if (ReadNumericValue(s,@"GameData\[""squad_reinforce_ext""\]\[""cost""\]\[""cost""\]\[""requisition""\]\s=\s(.*)",out numValue))
					info.RequisitionCost = (int)numValue ;
				if (ReadNumericValue(s,@"GameData\[""squad_reinforce_ext""\]\[""cost""\]\[""cost""\]\[""power""\]\s=\s(.*)",out numValue))
					info.PowerCost= (int)numValue;
				if (ReadNumericValue(s,@"GameData\[""squad_reinforce_ext""\]\[""cost""\]\[""time_seconds""\]\s=\s(.*)",out numValue))
					info.ReinforceTime= (int)numValue;
                if (ReadNumericValue(s, @"GameData\[""squad_reinforce_ext""\]\[""in_combat_time_multiplier""\]\s=\s(.*)", out numValue))
                    info.InCombatTimeMultiplier = numValue;
                if (ReadNumericValue(s,@"GameData\[""squad_reinforce_ext""\]\[""cost""\]\[""cost""\]\[""population""\]\s=\s(.*)",out numValue))
					info.ReinforcePop= (int)numValue;
				if (ReadNumericValue(s,@"GameData\[""squad_cap_ext""\]\[""squad_cap_usage""\]\s=\s(.*)",out numValue))
					info.SquadCapUsage= (int)numValue;
				if (ReadNumericValue(s,@"GameData\[""squad_cap_ext""\]\[""support_cap_usage""\]\s=\s(.*)",out numValue))
					info.SupportCapUsage = (int)numValue; 
				if (ReadNumericValue(s,@"GameData\[""squad_requirement_ext""\]\[""requirements""\]\["".*""\]\[""max_squad_cap""\]\s=\s(.*)",out numValue))
					info.HardCap= (int)numValue; 
				if (ReadNumericValue(s,@"GameData\[""squad_loadout_ext""\]\[""unit_min""\]\s=\s(.*)",out numValue))
					info.StartingSquadSize = (int)numValue;
				if (ReadNumericValue(s,@"GameData\[""squad_loadout_ext""\]\[""unit_max""\]\s=\s(.*)",out numValue))
					info.MaxSquadSize= (int)numValue;
				if (ReadNumericValue(s,@"GameData\[""squad_reinforce_ext""\]\[""max_upgrades""\]\s=\s(.*)",out numValue))
					info.MaxWeapons	= (int)numValue;	
				if (ReadNumericValue(s,@"GameData\[""squad_leader_ext""\]\[""max_leaders""\]\s=\s(.*)",out numValue))
					info.MaxLeaders	= (int)numValue;
				if (ReadNumericValue(s,@"GameData\[""squad_morale_ext""\]\[""max""\]\s=\s(.*)",out numValue))
					info.Morale	= (int)numValue;
				if (ReadNumericValue(s,@"GameData\[""squad_morale_ext""\]\[""rate_per_second""\]\s=\s(.*)",out numValue))
					info.MoraleRegen = numValue;															
				 
				if (ReadNumericValue(s,@"GameData\[""squad_ui_ext""\]\[""ui_info""\]\[""screen_name_id""\]\s=\s""\$(.*)""",out numValue))
					info.UI = (int)numValue;
				
                if (Regex.Match(s,@"GameData\[""squad_can_attach_ext""\]\s=\sReference\(\[\[sbpextensions\\squad_can_attach_ext.lua\]\]\)").Success )
                    info.CanBeAttached = true;
                string luaName = Regex.Replace(path,@".*\\","");
				info.Name= Translation.Translate(info.UI,luaName);


			# endregion
			
			#region UNITS STATS
				
				string unitLua = ReadStringValue(s,@"GameData\[""squad_loadout_ext""\]\[""trooper_base""\]\[""type""\]\s=\s""(?<unitPath>.*)""");
			
				unitLua = unitLua.Replace("\\\\","\\");

                if (unitLua != "")
                {
                    string unitLuaPath = DataPath.GetPath(unitLua);

                    if (unitLuaPath != "")
                    {
                        info.Unit = new UnitInfo();
                        info.Unit.Parent = info;
                        ParseUnit(unitLuaPath, info.Unit);
                        Match km = Regex.Match(unitLua, @".*\\(?<unit>.*)\.\w+");
                        if (km.Success)
                            if (!DataDumper.Units.Contains(km.Groups["unit"].Value))
                                DataDumper.Units.Add(km.Groups["unit"].Value, info.Unit);
                    }
                }
               
						
			#endregion
			
			#region LEADERS STATS 
				MatchCollection matches = Regex.Matches(s,@"GameData\[""squad_leader_ext""\]\[""(?<leader>.*)""\]\[""leader""\]\[""type""\]\s=\s""(?<leaderPath>.*)""");
				if (matches.Count>0)
					info.Leaders=new ArrayList();
				foreach (Match m in matches)
				{
					if (m.Success)
					{
						grp = m.Groups["leaderPath"];
						if (grp.Success)
						{
							string leaderLuaPath = DataPath.GetPath(grp.Value);
						
							if (leaderLuaPath != "")
							{
								UnitInfo leaderInfo = new LeaderInfo();
								leaderInfo.Parent=info;
								info.Leaders.Add(leaderInfo);
							
								ParseUnit(leaderLuaPath, leaderInfo);
							
								Match km = Regex.Match(grp.Value,@".*\\(?<unit>.*)\.\w+");
								if (km.Success)
									if (!DataDumper.Units.Contains(km.Groups["unit"].Value))
										DataDumper.Units.Add(km.Groups["unit"].Value,leaderInfo);
							
								string leaderGr = m.Groups["leader"].Value;
								if (ReadNumericValue(s,@"GameData\[""squad_leader_ext""\]\["""+leaderGr+@"""\]\[""cost_time""\]\[""cost""\]\[""requisition""\]\s=\s(.*)",out numValue))
									leaderInfo.RequisitionCost= (int)numValue;
								if (ReadNumericValue(s,@"GameData\[""squad_leader_ext""\]\["""+leaderGr+@"""\]\[""cost_time""\]\[""cost""\]\[""power""\]\s=\s(.*)",out numValue))
									leaderInfo.PowerCost = (int)numValue;
								if (ReadNumericValue(s,@"GameData\[""squad_leader_ext""\]\["""+leaderGr+@"""\]\[""cost_time""\]\[""time_seconds""\]\s=\s(.*)",out numValue))
									leaderInfo.BuildTime = (int)numValue;							
							}
						}
					}
				}
			#endregion
			
				ParseToolTips(s,info);
				ParseRequirements(s,info,"squad_requirement_ext");
                ParseModifiers(s, info,info.Modifiers, "squad_modifier_apply_ext");
                ParseInfiltration(s, info.Infiltration);
                ParseJumps(s, info.Jumps);
                #region Extensions
                ParseEnemyPossession(s, info);
                ParseLightningField(s, info);
                ParseFear(s, info);
                ParseDance(s, info);
                ParseRampage(s, info);
                ParseCannibalize(s, info);
                #endregion
            }

			
		}
		public static void ParseUnit(string luaPath,UnitInfo info)
		{
			StreamReader file = new StreamReader(File.OpenRead(luaPath),System.Text.Encoding.ASCII);
			file.BaseStream.Seek(0, SeekOrigin.Begin);

			string s = file.ReadToEnd();
			file.Close();

			#region INHERITANCE
			
			Match mc = Regex.Match( s , @"GameData\s=\sInherit\(\[\[(?<inherit>.*)\]\]\)" );
			if (mc.Success)
			{
				
				Group grp = mc.Groups["inherit"];
				if (grp.Success && grp.Value!="")
				{
					string unitLua = grp.Value;
					string unitLuaPath = DataPath.GetPath(grp.Value);

					if (unitLuaPath != "")
						ParseUnit(unitLuaPath,info);
				}
			}			
			#endregion
			
			double numValue=0.0;

			#region UNIT STATS							
            
            if (ReadNumericValue(s, @"GameData\[""mob_ext""\]\[""mob_value""\]\s=\s(.*)", out numValue))
                info.MobValue = (int)numValue;								
			if (ReadNumericValue(s,@"GameData\[""cost_ext""\]\[""time_cost""\]\[""cost""\]\[""requisition""\]\s=\s(.*)",out numValue))
				info.RequisitionCost= (int)numValue;
			if (ReadNumericValue(s,@"GameData\[""cost_ext""\]\[""time_cost""\]\[""cost""\]\[""power""\]\s=\s(.*)",out numValue))
				info.PowerCost = (int)numValue;
			if (ReadNumericValue(s,@"GameData\[""cost_ext""\]\[""time_cost""\]\[""time_seconds""\]\s=\s(.*)",out numValue))
				info.BuildTime = numValue;
			if (ReadNumericValue(s,@"GameData\[""cost_ext""\]\[""time_cost""\]\[""cost""\]\[""population""\]\s=\s(.*)",out numValue))
				info.PopCost = (int)numValue;
			if (ReadNumericValue(s,@"GameData\[""health_ext""\]\[""hitpoints""\]\s=\s(.*)",out numValue))
				info.HitPoints = (int)numValue; 			
			if (ReadNumericValue(s,@"GameData\[""sight_ext""\]\[""sight_radius""\]\s=\s(.*)",out numValue))
				info.Sight = numValue;
			if (ReadNumericValue(s,@"GameData\[""sight_ext""\]\[""keen_sight_radius""\]\s=\s(.*)",out numValue))
				info.KeenSight = numValue;
			if (ReadNumericValue(s,@"GameData\[""special_attack_physics_ext""\]\[""mass""\]\s=\s(.*)",out numValue))
				info.Mass =	 numValue; 
			if (ReadNumericValue(s,@"GameData\[""moving_ext""\]\[""speed_max""\]\s=\s(.*)",out numValue))
				info.Speed=	 numValue;
            if (Regex.Match(s, @"GameData\[""moving_ext""\]\[""air_unit""\]\s=\strue").Success)
                info.CanFly = true;
            
			if (ReadNumericValue(s,@"GameData\[""health_ext""\]\[""regeneration_rate""\]\s=\s(.*)",out numValue))
				info.HitsRegen  = numValue;
            if (ReadNumericValue(s, @"GameData\[""health_ext""\]\[""get_back_up_chance""\]\s=\s(.*)", out numValue))
                info.RessurectChance = numValue;
            if (ReadNumericValue(s, @"GameData\[""health_ext""\]\[""get_back_up_health_percent""\]\s=\s(.*)", out numValue))
                info.ResurrectHps = numValue;
			if (ReadNumericValue(s,@"GameData\[""morale_add_ext""\]\[""inc_morale_max""\]\s=\s(.*)",out numValue))
				info.Morale = numValue;
			if (ReadNumericValue(s,@"GameData\[""morale_add_ext""\]\[""inc_morale_rate""\]\s=\s(.*)",out numValue))
				info.MoraleRegen  = numValue;
			if (ReadNumericValue(s,@"GameData\[""melee_ext""\]\[""charge_range""\]\s=\s(.*)",out numValue))
				info.ChargeRange  = numValue;
            if (ReadNumericValue(s, @"GameData\[""squad_hold_ext""\]\[""nr_available_spots""\]\s=\s(.*)", out numValue))
                info.TransportSlots = (int)numValue;

			string strValue="";
			if (ReadStringValue(s,@"GameData\[""type_ext""\]\[""type_armour""\]\s=\sReference\(\[\[type_armour\\(.*).lua\]\]\)",out strValue))
				info.ArmorType = (ArmorTypes)Enum.Parse(typeof(ArmorTypes),strValue);
			
			if (ReadNumericValue(s,@"\[""ui_ext""\]\[""ui_info""\]\[""screen_name_id""\]\s=\s""\$(.*)""",out numValue))
				info.UI = (int)numValue;
            if (ReadStringValue(s, @"GameData\[""ui_ext""\]\[""ui_info""\]\[""icon_name""\]\s=\s""(.*)""", out strValue))
                info.Icon = strValue;
			string luaName = Regex.Replace(luaPath,@".*\\","");
            info.FileName = luaName;
			info.Name= Translation.Translate(info.UI,luaName);

			#endregion
			
			#region SKILLS
			
			ParseSkills(s,info);
			
			
		
			#endregion

            #region Extensions
            
            ParsePossession(s, info);
            ParseEntrench(s, info);
            ParseHarvest(s, info);
            ParseDirectSpawn(s, info);
            #endregion 

            #region WEAPONS

            #region OldWeapons

            foreach (int hardpoint in new Hashtable(info.WeaponHardPoints).Keys)
            {
                WeaponHardPointInfo whpInfo = (WeaponHardPointInfo)info.WeaponHardPoints[hardpoint];
                foreach (WeaponInfo weaponInfo in new ArrayList( whpInfo.Weapons))
                {
                    string hp = "";
                    if (hardpoint < 10)
                        hp = "0" + hardpoint.ToString();
                    else hp = hardpoint.ToString();

                    string ind = "";
                    if (weaponInfo.WeaponIndex < 10)
                        ind = "0" + weaponInfo.WeaponIndex.ToString();
                    else ind = weaponInfo.WeaponIndex.ToString();
    
                    if (Regex.Match(s, @"GameData\[""combat_ext""\]\[""hardpoints""\]\[""hardpoint_(?<hardpoint>"+ hp  +@")""\]\[""weapon_table""\]\[""weapon_(?<weaponIndex>"+ind+@")""\]\[""weapon""\]\s=\s(.*)").Success)
                        whpInfo.Weapons.Remove(weaponInfo);
                }
                if (whpInfo.Weapons.Count == 0)
                    info.WeaponHardPoints.Remove(hardpoint);
            }


            #endregion
            
            MatchCollection mcc = Regex.Matches(s,@"GameData\[""combat_ext""\]\[""hardpoints""\]\[""hardpoint_(?<hardpoint>[0-9]?[0-9]?)""\]\[""weapon_table""\]\[""weapon_(?<weaponIndex>[0-9]?[0-9]?)""\]\[""weapon""\]\s=\s""(weapon\\\\)?(?<weaponName>.*)""");
			
			foreach (Match mt in mcc)
			{
				Group gr_name = mt.Groups["weaponName"];
                //if (Regex.Match(gr_name.Value, "(dummy)").Success || gr_name.Value == "")
                if ( gr_name.Value == "")    
                    continue;

                string weaponLua = gr_name.Value;
                if (!gr_name.Value.EndsWith(".lua") && !gr_name.Value.EndsWith(".nil"))
                    weaponLua += ".lua";

                int wpIndex = System.Convert.ToInt32(mt.Groups["weaponIndex"].Value, NumberFormat);
                int hardpoint = System.Convert.ToInt32(Regex.Replace(mt.Groups["hardpoint"].Value, "\b0", ""), NumberFormat);

                if (Regex.Match(gr_name.Value, "(_group)").Success)
                {
                    ParseWeaponGroup(weaponLua,info,hardpoint,wpIndex);
                    continue;
                }

   
                // Create the WeaponInfo
                WeaponInfo weaponInfo = new WeaponInfo();
				weaponInfo.Parent=info;
                weaponInfo.Name = Regex.Replace(gr_name.Value, @"\.\w+", "");

                weaponInfo.WeaponIndex = wpIndex;
                weaponInfo.HardPoint = hardpoint;
               
                ParseWeapon(weaponLua, weaponInfo);

                if (!weaponInfo.IsValid())
                    continue;

                if (!info.WeaponHardPoints.ContainsKey(hardpoint))
                    info.WeaponHardPoints.Add(hardpoint, new WeaponHardPointInfo());

                WeaponHardPointInfo whpInfo = info.WeaponHardPoints[hardpoint] as WeaponHardPointInfo;

                whpInfo.Weapons.Add(weaponInfo);
                whpInfo.Weapons.Sort();		
			}
			
			#endregion
			
			ParseToolTips(s,info);
			ParseRequirements(s,info);
            ParseModifiers(s, info,info.Modifiers, "modifier_apply_ext");
            ParseModifiers(s, info, info.InCombatModifiers, "melee_ext", "in_melee_modifiers"); 
		}
        public static void ParseWeaponGroup(string luaPath, BuildableInfo info, int hardPoint, int wpIndex)
        {
            string path = DataPath.GetPath("weapon\\"+luaPath);
            if (path == "")
                return; 
            StreamReader file = new StreamReader(File.OpenRead(path), System.Text.Encoding.ASCII);
            file.BaseStream.Seek(0, SeekOrigin.Begin);

            string s = file.ReadToEnd();

            file.Close();
            
            MatchCollection mcc = Regex.Matches(s, @"GameData\[""group_weapon_table""\]\[""weapon_(?<weapon_num>[0-9]?[0-9]?)""\]\[""referenced_weapon_name""\]\s=\s""(weapon\\\\)?(?<weaponName>.*)""");

            foreach (Match mt in mcc)
            {
                Group gr_name = mt.Groups["weaponName"];
               // if (Regex.Match(gr_name.Value, "(dummy)").Success || gr_name.Value == "")
                if ( gr_name.Value == "")
                    continue;

                string weaponLua = gr_name.Value;
                if (!gr_name.Value.EndsWith(".lua") && !gr_name.Value.EndsWith(".nil"))
                    weaponLua += ".lua";

                // Create the WeaponInfo
                WeaponInfo weaponInfo = new WeaponInfo();
                weaponInfo.Parent = info;
                weaponInfo.Name = Regex.Replace(gr_name.Value, @"\.\w+", "");

                weaponInfo.WeaponIndex = wpIndex;
                weaponInfo.HardPoint = hardPoint;
                
                ParseWeapon(weaponLua, weaponInfo);

                if (!weaponInfo.IsValid())
                    continue;

                if (!info.WeaponHardPoints.ContainsKey(hardPoint))
                    info.WeaponHardPoints.Add(hardPoint, new WeaponHardPointInfo());

                WeaponHardPointInfo whpInfo = info.WeaponHardPoints[hardPoint] as WeaponHardPointInfo;

                whpInfo.Weapons.Add(weaponInfo);
                whpInfo.Weapons.Sort();
            }
        }
		public static void ParseWeapon(string luaPath, WeaponInfo info)
		{
			string path = DataPath.GetPath("weapon\\"+luaPath);
            if (path != "")
            {
                StreamReader file = new StreamReader(File.OpenRead(path), System.Text.Encoding.ASCII);
                file.BaseStream.Seek(0, SeekOrigin.Begin);

                string s = file.ReadToEnd();

                file.Close();
                #region INHERITANCE
                Match mc = Regex.Match(s, @"GameData\s=\sInherit\(\[\[weapon\\(?<inherit>.*)\]\]\)");
                if (mc.Success)
                {
                    Group grp = mc.Groups["inherit"];
                    if (grp.Success && grp.Value != "")
                        ParseWeapon(grp.Value, info);
                }
                #endregion

                double numValue = 0.0;

                if (ReadNumericValue(s, @"GameData\[""cost""\]\[""cost""\]\[""requisition""\]\s=\s(.*)", out numValue))
                    info.RequisitionCost = (int)numValue;
                if (ReadNumericValue(s, @"GameData\[""cost""\]\[""cost""\]\[""power""\]\s=\s(.*)", out numValue))
                    info.PowerCost = (int)numValue;
                if (ReadNumericValue(s, @"GameData\[""cost""\]\[""time_seconds""\]\s=\s(.*)", out numValue))
                    info.BuildTime = (int)numValue;
                if (ReadNumericValue(s, @"GameData\[""accuracy""\]\s=\s(.*)", out numValue))
                    info.Accuracy = numValue;
                if (ReadNumericValue(s, @"GameData\[""accuracy_reduction_when_moving""\]\s=\s(.*)", out numValue))
                    info.AccuracyReduction = numValue;
                if (ReadNumericValue(s, @"GameData\[""area_effect""\]\[""weapon_damage""\]\[""armour_damage""\]\[""min_damage""\]\s=\s(.*)", out numValue))
                    info.MinDamage = numValue;
                if (ReadNumericValue(s, @"GameData\[""area_effect""\]\[""weapon_damage""\]\[""armour_damage""\]\[""max_damage""\]\s=\s(.*)", out numValue))
                    info.MaxDamage = numValue;
                if (ReadNumericValue(s, @"GameData\[""area_effect""\]\[""weapon_damage""\]\[""armour_damage""\]\[""min_damage_value""\]\s=\s(.*)", out numValue))
                    info.MinDamageValue = numValue;
                if (ReadNumericValue(s, @"GameData\[""area_effect""\]\[""weapon_damage""\]\[""armour_damage""\]\[""morale_damage""\]\s=\s(.*)", out numValue))
                    info.MoraleDamage = numValue;
                if (ReadNumericValue(s, @"GameData\[""min_range""\]\s=\s(.*)", out numValue))
                    info.MinRange = numValue;
                if (ReadNumericValue(s, @"GameData\[""max_range""\]\s=\s(.*)", out numValue))
                    info.MaxRange = numValue;
                if (ReadNumericValue(s, @"GameData\[""reload_time""\]\s=\s(.*)", out numValue))
                    info.ReloadTime = numValue;
                if (ReadNumericValue(s, @"GameData\[""setup_time""\]\s=\s(.*)", out numValue))
                    info.SetupTime = numValue;
                if (ReadNumericValue(s, @"GameData\[""area_effect""\]\[""area_effect_information""\]\[""radius""\]\s=\s(.*)", out numValue))
                    info.AOERadius = numValue;
                
                if (ReadNumericValue(s, @"GameData\[""area_effect""\]\[""throw_data""\]\[""force_min""\]\s=\s(.*)", out numValue))
                    info.MinForce = numValue;
                if (ReadNumericValue(s, @"GameData\[""area_effect""\]\[""throw_data""\]\[""force_max""\]\s=\s(.*)", out numValue))
                    info.MaxForce = numValue;
                


                bool boolValue;
                if (ReadBooleanValue(s,@"GameData\[""can_attack_air_units""\]\s=\s(.*)",out boolValue))
                    info.CanHitAir = boolValue;
                if (ReadBooleanValue(s,@"GameData\[""can_attack_ground_units""\]\s=\s(.*)",out boolValue))
                    info.CanHitGround = boolValue;


                if (Regex.Match(s, @"GameData\[""show_in_reinforce""\]\s=\sfalse").Success)
                    info.ShowCost = false;
              
                string strValue = "";

                if (ReadStringValue(s, @"GameData\[""ui_info""\]\[""icon_name""\]\s=\s""(.*)""", out strValue))
                    info.Icon = strValue;
                
                #region DPS
                
                MatchCollection aMcs = Regex.Matches(s, @"GameData\[""area_effect""\]\[""weapon_damage""\]\[""armour_damage""\]\[""armour_piercing_types""\]\[""entry_(?<armor_entry>[0-9][0-9])""\]\[""armour_type""\]\s=\sReference\(\[\[type_armour\\(?<armor_type>.*)\.lua\]\]\)");

                foreach (Match m in aMcs) // Retrieve ArmorTypes entries
                {
                    string armor = m.Groups["armor_type"].Value;                    
                    int armorEntry = System.Convert.ToInt32(Regex.Replace(m.Groups["armor_entry"].Value, "\b0", ""), NumberFormat);

                    if (!info.ArmorPiercingValues.Contains(armorEntry))
                    {
                        ArmorPiercing ap = new ArmorPiercing();
                        ap.Entry = armorEntry;    
                        try
                        {
                            ap.ArmorType = (ArmorTypes)Enum.Parse(typeof(ArmorTypes), armor);
                        }
                        catch { ap.ArmorType = ArmorTypes.unknown; }
                        info.ArmorPiercingValues.Add(armorEntry, ap);
                    }
                    else
                    {
                        ArmorPiercing ap = info.ArmorPiercingValues[armorEntry] as ArmorPiercing;
                        ap.Entry = armorEntry;
                        try
                        {
                            ap.ArmorType = (ArmorTypes)Enum.Parse(typeof(ArmorTypes), armor);
                        }
                        catch { ap.ArmorType = ArmorTypes.unknown; }
                    }
                }
                #endregion
                
                foreach (ArmorPiercing ap in info.ArmorPiercingValues.Values)
                {
                    if (ap.ArmorType != ArmorTypes.unknown)
                    {
                        string subIndex = "";
                        int j = ap.Entry;
                        if (j < 10) subIndex = "0" + j.ToString();
                        else subIndex = j.ToString();
                        if (ReadNumericValue(s, @"GameData\[""area_effect""\]\[""weapon_damage""\]\[""armour_damage""\]\[""armour_piercing_types""\]\[""entry_+" + subIndex + @"""\]\[""armour_piercing_value""\]\s=\s(.*)", out numValue))
                            ap.PiercingValue = numValue;
                    }
                    else ap.PiercingValue = 0.0;          
                }
 
                ParseRequirements(s, info);
            }
		}
		public static void ParseSkills(string lua, BuildableInfo info)
		{
            
            # region OLD-SKILLS
			
            if (info.Skills.Count > 0) // Re-Parse old Skills
			{
				ArrayList l = new ArrayList(info.Skills.Keys);
				foreach (string skill in l)
				{
					Match m = Regex.Match(lua,@"GameData\[""ability_ext""\]\[""abilities""\]\[""ability_" + skill + @"""\]");
                    if (m.Success)
                        info.Skills.Remove(skill);
				}
				
			}
			#endregion

            #region NEW SKILLS              
            MatchCollection mcc = Regex.Matches(lua,@"GameData\[""ability_ext""\]\[""abilities""\]\[""ability_(?<abilityIndex>[0-9]?[0-9]?)""\]\s=\s""(abilities\\\\)?(?<abilityType>.*)""");

            foreach (Match m in mcc)
            {
                Group skill = m.Groups["abilityIndex"];
                Group skillType = m.Groups["abilityType"];

                if (skill.Success && skillType.Value != "" && !info.Skills.Contains(skill.Value))
                {

                    SkillInfo newSkillInfo = new SkillInfo();
                    newSkillInfo.Parent = info;
                    newSkillInfo.FileName = skillType.Value;  
                    if (!newSkillInfo.FileName.EndsWith(".lua") && !newSkillInfo.FileName.EndsWith(".nil"))
                        newSkillInfo.FileName += ".lua";
                    newSkillInfo.LuaName = Regex.Replace(newSkillInfo.FileName, @"(\.lua)|(\.nil)", "");
                    info.Skills.Add(skill.Value, newSkillInfo);
                }
            }
            
           
			#endregion
						
			#region SKILLS PARSING
			if (info.Skills.Count > 0)
                foreach (SkillInfo skillInfo in info.Skills.Values)
                {
                    skillInfo.Parse(skillInfo.FileName);                    
                }
				
			#endregion	
		}
		public static void ParseBuilding(string path,BuildingInfo info)
		{
			StreamReader file = new StreamReader(File.OpenRead(path),System.Text.Encoding.ASCII);
			file.BaseStream.Seek(0, SeekOrigin.Begin);

			string s = file.ReadToEnd();

			file.Close();


			#region INHERITANCE
			
			Match mc = Regex.Match( s , @"GameData\s=\sInherit\(\[\[(?<inherit>.*)\]\]\)" );			
			if (mc.Success)
			{
				Group grp = mc.Groups["inherit"];
				if (grp.Success && grp.Value!="")
				{
					string unitLua = grp.Value;
					string unitLuaPath = DataPath.GetPath(grp.Value);

					if (unitLuaPath != "")
						ParseBuilding(unitLuaPath,info);
				}
				
			}			
			#endregion
		
			double numValue=0.0;

			#region BUILDING STATS							
														
			if (ReadNumericValue(s,@"GameData\[""cost_ext""\]\[""time_cost""\]\[""cost""\]\[""requisition""\]\s=\s(.*)",out numValue))
				info.RequisitionCost= (int)numValue;
			if (ReadNumericValue(s,@"GameData\[""cost_ext""\]\[""time_cost""\]\[""cost""\]\[""power""\]\s=\s(.*)",out numValue))
				info.PowerCost = (int)numValue;
			if (ReadNumericValue(s,@"GameData\[""cost_ext""\]\[""time_cost""\]\[""time_seconds""\]\s=\s(.*)",out numValue))
				info.BuildTime = (int)numValue;
			if (ReadNumericValue(s,@"GameData\[""health_ext""\]\[""hitpoints""\]\s=\s(.*)",out numValue))
				info.HitPoints = (int)numValue; 			
			if (ReadNumericValue(s,@"GameData\[""sight_ext""\]\[""sight_radius""\]\s=\s(.*)",out numValue))
				info.Sight = (int)numValue;
			if (ReadNumericValue(s,@"GameData\[""resource_ext""\]\[""requisition_per_second""\]\s=\s(.*)",out numValue))
				info.RequisitionIncome = numValue*10;
			if (ReadNumericValue(s,@"GameData\[""resource_ext""\]\[""power_per_second""\]\s=\s(.*)",out numValue))
				info.PowerIncome = numValue*10;
            if (ReadNumericValue(s, @"GameData\[""garrison_ext""\]\[""requisition_rate_multiplier""\]\s=\s(.*)", out numValue))
                info.RequisitionModifier = (numValue-1)*6;
            if (ReadNumericValue(s, @"GameData\[""squad_hold_ext""\]\[""nr_available_spots""\]\s=\s(.*)", out numValue))
                info.TransportSlots = (int)numValue;
		
			string strValue="";
			if (ReadStringValue(s,@"GameData\[""type_ext""\]\[""type_armour""\]\s=\sReference\(\[\[type_armour\\(.*).lua\]\]\)",out strValue))
                info.ArmorType = (ArmorTypes)Enum.Parse(typeof(ArmorTypes), strValue);			
			if (ReadNumericValue(s,@"GameData\[""ui_ext""\]\[""ui_info""\]\[""screen_name_id""\]\s=\s""\$(.*)""",out numValue))
				info.UI = (int)numValue;
            if (ReadStringValue(s, @"GameData\[""ui_ext""\]\[""ui_info""\]\[""icon_name""\]\s=\s""(.*)""", out strValue))
                info.Icon = strValue;
			string luaName = Regex.Replace(path,@".*\\","");
			info.Name= Translation.Translate(info.UI,luaName);

			#endregion

            #region SKILLS

            ParseSkills(s, info);

            #endregion

			#region SPAWNS
			MatchCollection mcc = Regex.Matches(s,@"GameData\[""spawner_ext""\]\[""squad_table""\]\[""squad_([0-9]?[0-9]?)""\]\s=\s"".*\\\\(?<squad>.*).lua""");
			if (mcc.Count>0)
			{
				info.BuildUnits=new ArrayList();
				
				foreach (Match mt in mcc)
				{
					Group squad = mt.Groups["squad"];
					if (squad.Success)
						info.BuildUnits.Add(squad.Value);				
				}
			}
			#endregion

			
			#region Researches
            MatchCollection mccR = Regex.Matches(s, @"GameData\[""research_ext""\]\[""research_table""\]\[""research_([0-9]?[0-9]?)""\]\s=\s""(.*\\\\)?((?<research>.*)\.lua|(?<research>.*))""");
			if (mccR.Count>0)
			{
				info.Researches=new ArrayList();
				
				foreach (Match mt in mccR)
				{
					Group research = mt.Groups["research"];
                    if (research.Success)  
                        info.Researches.Add(research.Value);  
				}
                info.Researches.Sort();
			}
			#endregion

            #region WEAPONS

            #region OldWeapons


            foreach (int hardpoint in new Hashtable(info.WeaponHardPoints).Keys)
            {
                WeaponHardPointInfo whpInfo = (WeaponHardPointInfo)info.WeaponHardPoints[hardpoint];
                foreach (WeaponInfo weaponInfo in new ArrayList(whpInfo.Weapons))
                {
                    string hp = "";
                    if (hardpoint < 10)
                        hp = "0" + hardpoint.ToString();
                    else hp = hardpoint.ToString();

                    string ind = "";
                    if (weaponInfo.WeaponIndex < 10)
                        ind = "0" + weaponInfo.WeaponIndex.ToString();
                    else ind = weaponInfo.WeaponIndex.ToString();

                    if (Regex.Match(s, @"GameData\[""combat_ext""\]\[""hardpoints""\]\[""hardpoint_(?<hardpoint>" + hp + @")""\]\[""weapon_table""\]\[""weapon_(?<weaponIndex>" + ind + @")""\]\[""weapon""\]\s=\s(.*)").Success)
                        whpInfo.Weapons.Remove(weaponInfo);
                }
                if (whpInfo.Weapons.Count == 0)
                    info.WeaponHardPoints.Remove(hardpoint);
            }

            #endregion

            MatchCollection mccW = Regex.Matches(s, @"GameData\[""combat_ext""\]\[""hardpoints""\]\[""hardpoint_(?<hardpoint>[0-9]?[0-9]?)""\]\[""weapon_table""\]\[""weapon_(?<weaponIndex>[0-9]?[0-9]?)""\]\[""weapon""\]\s=\s""(weapon\\\\)?(?<weaponName>.*)""");
            
               
            foreach (Match mt in mccW)
            {
                Group gr_name = mt.Groups["weaponName"];
                if ( gr_name.Value == "")
                    continue;

                

                string weaponLua = gr_name.Value;
                if (!gr_name.Value.EndsWith(".lua") && !gr_name.Value.EndsWith(".nil"))
                    weaponLua += ".lua";

                int wpIndex = System.Convert.ToInt32(mt.Groups["weaponIndex"].Value, NumberFormat);
                int hardpoint = System.Convert.ToInt32(Regex.Replace(mt.Groups["hardpoint"].Value, "\b0", ""), NumberFormat);

                if (Regex.Match(gr_name.Value, "(_group)").Success)
                {
                    ParseWeaponGroup(weaponLua, info, hardpoint, wpIndex);
                    continue;
                }


                // Create the WeaponInfo
                WeaponInfo weaponInfo = new WeaponInfo();
                weaponInfo.Parent = info;
                weaponInfo.Name = Regex.Replace(gr_name.Value, @"\.\w+", "");

                weaponInfo.WeaponIndex = wpIndex;
                weaponInfo.HardPoint = hardpoint;

                ParseWeapon(weaponLua, weaponInfo);

                if (!weaponInfo.IsValid())
                    continue;

                if (!info.WeaponHardPoints.ContainsKey(hardpoint))
                    info.WeaponHardPoints.Add(hardpoint, new WeaponHardPointInfo());

                WeaponHardPointInfo whpInfo = info.WeaponHardPoints[hardpoint] as WeaponHardPointInfo;

                whpInfo.Weapons.Add(weaponInfo);
                whpInfo.Weapons.Sort();
            }

            #endregion

            #region Extensions
            ParseMinefield(s, info);
            #endregion


            #region ADDONS
            info.Addons=new ArrayList();
			mcc = Regex.Matches(s,@"GameData\[""addon_ext""\]\[""addons""\]\[""addon_([0-9]?[0-9]?)""\]\s=\s""(addons\\\\)?(?<addon>.*)""");
			
			foreach (Match mt in mcc)
			{
				Group gra = mt.Groups["addon"];
				if (gra.Success)
				{
					if (Regex.Match(gra.Value,"(dummy)").Success)
						continue;
					ResearchInfo rInfo=new ResearchInfo();
					rInfo.Parent = info;
					rInfo.Name = gra.Value;
                    if (!rInfo.Name.EndsWith(".lua") && !rInfo.Name.EndsWith(".nil"))
                        rInfo.Name += ".lua";
                    
					ParseResearch(DataPath.GetPath("addons\\"+rInfo.Name),rInfo);
					info.Addons.Add(rInfo);
				}
			}
			info.Addons.Sort();
			#endregion

			ParseToolTips(s,info);
			ParseRequirements(s,info,"requirement_ext");
            ParseModifiers(s, info,info.Modifiers, "modifier_apply_ext");
        }
        public static void ParseInfiltration(string s, InfiltrationInfo info)
        {
            if (Regex.Match(s, @"GameData\[""squad_infiltration_ext""\]\s=\sReference\(\[\[sbpextensions\\squad_infiltration_ext.lua\]\]\)").Success)
                info.InfiltrationType = InfiltrationTypes.Permanent;
            if (Regex.Match(s, @"GameData\[""squad_cover_ext""\]\[""cover_light""\]\[""modifiers""\]\[""modifier_02""\]\s=\sReference\(\[\[modifiers\\enable_infiltration.lua\]\]\)").Success)
                info.InfiltrationType = InfiltrationTypes.InCover;
            ParseRequirements(s, info, "squad_infiltration_ext"); 
        }
        public static void ParseJumps(String s, JumpInfo info)
        {          
            double numValue = 0;
            if (ReadNumericValue(s, @"GameData\[""squad_jump_ext""\]\[""jump_distance_max""\]\s=\s(.*)", out numValue))
                info.JumpRange = (int)numValue;
            double maxcharge=0;
            if (ReadNumericValue(s, @"GameData\[""squad_jump_ext""\]\[""charge_max""\]\s=\s(.*)", out numValue))
                maxcharge = numValue;
            if (ReadNumericValue(s, @"GameData\[""squad_jump_ext""\]\[""charge_jump_cost_max""\]\s=\s(.*)", out numValue))
                info.Jumps = Math.Round((maxcharge / numValue),2);
            if (ReadNumericValue(s, @"GameData\[""squad_jump_ext""\]\[""charge_regeneration""\]\s=\s(.*)", out numValue))
                info.JumpRecharge = Math.Round( (maxcharge/info.Jumps)/numValue,2);
            if (info.Jumps > 0 && info.JumpType== JumpTypes.None)
                info.JumpType = JumpTypes.Jump;
            if (Regex.Match(s, @"GameData\[""squad_jump_ext""\]\[""teleport""\]\s=\strue").Success)
                info.JumpType = JumpTypes.Teleport;
            ParseRequirements(s, info, "squad_jump_ext");
        }

        

        public static void ParseMinefield(string s, BuildableInfo info)
        {           
            if (Regex.Match(s, @"GameData\[""mine_field_ext""\]\s=\sReference\(\[\[ebpextensions\\mine_field_ext\.lua\]\]\)").Success)
            {
                MinefieldInfo mInfo = null;
                if (info.Extensions.Contains("minefield"))
                    mInfo = info.Extensions["minefield"] as MinefieldInfo;
                else
                {
                    mInfo = new MinefieldInfo();
                    info.Extensions.Add("minefield", mInfo);
                }
                mInfo.Parent = info;
                
                double numValue = 0;
                
                if (ReadNumericValue(s, @"GameData\[""mine_field_ext""\]\[""area_effect""\]\[""weapon_damage""\]\[""armour_damage""\]\[""min_damage""\]\s=\s(.*)", out numValue))
                    mInfo.Weapon.MinDamage = numValue;
                if (ReadNumericValue(s, @"GameData\[""mine_field_ext""\]\[""area_effect""\]\[""weapon_damage""\]\[""armour_damage""\]\[""max_damage""\]\s=\s(.*)", out numValue))
                    mInfo.Weapon.MaxDamage = numValue;
                if (ReadNumericValue(s, @"GameData\[""mine_field_ext""\]\[""area_effect""\]\[""weapon_damage""\]\[""armour_damage""\]\[""morale_damage""\]\s=\s(.*)", out numValue))
                    mInfo.Weapon.MoraleDamage = numValue;
                if (ReadNumericValue(s, @"GameData\[""mine_field_ext""\]\[""explosion_recharge_time""\]\s=\s(.*)", out numValue))
                    mInfo.Weapon.ReloadTime = numValue;

                mInfo.Weapon.SetupTime = -1.0;
                
                if (ReadNumericValue(s, @"GameData\[""mine_field_ext""\]\[""area_effect""\]\[""area_effect_information""\]\[""radius""\]\s=\s(.*)", out numValue))
                    mInfo.Weapon.AOERadius = numValue;

                if (ReadNumericValue(s, @"GameData\[""mine_field_ext""\]\[""area_effect""\]\[""throw_data""\]\[""force_min""\]\s=\s(.*)", out numValue))
                    mInfo.Weapon.MinForce = numValue;
                if (ReadNumericValue(s, @"GameData\[""mine_field_ext""\]\[""area_effect""\]\[""throw_data""\]\[""force_max""\]\s=\s(.*)", out numValue))
                    mInfo.Weapon.MaxForce = numValue;


                mInfo.Weapon.Accuracy = 1;
                mInfo.Weapon.ShowCost = false;
                
               

                mInfo.Weapon.Icon = info.Icon;

                #region Inherith
                StreamReader file = new StreamReader(File.OpenRead(DataPath.GetPath("ebpextensions\\mine_field_ext.lua")), System.Text.Encoding.ASCII);
                file.BaseStream.Seek(0, SeekOrigin.Begin);

                string ext = file.ReadToEnd();

                file.Close();
                #endregion

                GetPiercing(ext, mInfo);
                GetPiercing(s, mInfo);
                
                mInfo.Weapon.Name = "Mine Field";
            }
        }

        public static void GetPiercing(string s,MinefieldInfo mInfo )
        {
            double numValue = 0;
            #region DPS

            MatchCollection aMcs = Regex.Matches(s, @"GameData(\[""mine_field_ext""\])?\[""area_effect""\]\[""weapon_damage""\]\[""armour_damage""\]\[""armour_piercing_types""\]\[""entry_(?<armor_entry>[0-9][0-9])""\]\[""armour_type""\]\s=\sReference\(\[\[type_armour\\(?<armor_type>.*)\.lua\]\]\)");

            foreach (Match m in aMcs) // Retrieve ArmorTypes entries
            {
                string armor = m.Groups["armor_type"].Value;
                int armorEntry = System.Convert.ToInt32(Regex.Replace(m.Groups["armor_entry"].Value, "\b0", ""), NumberFormat);

                if (!mInfo.Weapon.ArmorPiercingValues.Contains(armorEntry))
                {
                    ArmorPiercing ap = new ArmorPiercing();
                    ap.Entry = armorEntry;
                    try
                    {
                        ap.ArmorType = (ArmorTypes)Enum.Parse(typeof(ArmorTypes), armor);
                    }
                    catch { ap.ArmorType = ArmorTypes.unknown; }
                    mInfo.Weapon.ArmorPiercingValues.Add(armorEntry, ap);
                }
                else
                {
                    ArmorPiercing ap = mInfo.Weapon.ArmorPiercingValues[armorEntry] as ArmorPiercing;
                    ap.Entry = armorEntry;
                    try
                    {
                        ap.ArmorType = (ArmorTypes)Enum.Parse(typeof(ArmorTypes), armor);
                    }
                    catch { ap.ArmorType = ArmorTypes.unknown; }
                }
            }
            #endregion

            foreach (ArmorPiercing ap in mInfo.Weapon.ArmorPiercingValues.Values)
            {
                if (ap.ArmorType != ArmorTypes.unknown)
                {
                    string subIndex = "";
                    int j = ap.Entry;
                    if (j < 10) subIndex = "0" + j.ToString();
                    else subIndex = j.ToString();

                    if (ReadNumericValue(s, @"\[""area_effect""\]\[""weapon_damage""\]\[""armour_damage""\]\[""armour_piercing_types""\]\[""entry_+" + subIndex + @"""\]\[""armour_piercing_value""\]\s=\s(.*)", out numValue))
                        ap.PiercingValue = numValue;
                }
                else ap.PiercingValue = 0.0;
            }
        }

        public static void ParseDance(string s, BuildableInfo info)
        {

            if (Regex.Match(s, @"GameData\[""squad_melee_dance_ext""\]\s=\sReference\(\[\[sbpextensions\\squad_melee_dance_ext\.lua\]\]\)").Success)
            {
                DanceInfo dInfo = null;

                if (info.Extensions.ContainsKey("dance"))
                    dInfo = info.Extensions["dance"] as DanceInfo;
                else
                {
                    dInfo = new DanceInfo();
                    info.Extensions.Add("dance", dInfo);
                }
                dInfo.Parent = info;
                string stringValue = "";
                double numValue = 0;

                if (ReadNumericValue(s, @"GameData\[""squad_melee_dance_ext""\]\[""dance_duration""\]\s=\s(?<duration>.*)", out numValue))
                    dInfo.Duration = numValue;

                if (ReadNumericValue(s, @"GameData\[""squad_melee_dance_ext""\]\[""recharge_duration""\]\s=\s(?<recharge>.*)", out numValue))
                    dInfo.Recharge = numValue;

                if (ReadStringValue(s, @"GameData\[""squad_melee_dance_ext""\]\[""button_texture""\]\s=\s""(?<button_texture>.*)""", out stringValue))
                    dInfo.Icon = stringValue;

                ParseRequirements(s, dInfo, "squad_melee_dance_ext");
            }
        }

        public static void ParseRampage(string s, BuildableInfo info)
        {

            if (Regex.Match(s, @"GameData\[""squad_rampage_ext""\]\s=\sReference\(\[\[sbpextensions\\squad_rampage_ext\.lua\]\]\)").Success)
            {
                RampageInfo rInfo = null;

                if (info.Extensions.ContainsKey("rampage"))
                    rInfo = info.Extensions["rampage"] as RampageInfo;
                else
                {
                    rInfo = new RampageInfo();
                    info.Extensions.Add("rampage", rInfo);
                }
                rInfo.Parent = info;
                
                double numValue = 0;

                if (ReadNumericValue(s, @"GameData\[""squad_rampage_ext""\]\[""reload_time""\]\s=\s(?<recharge>.*)", out numValue))
                    rInfo.Recharge = numValue;

                ParseRequirements(s, rInfo, "squad_rampage_ext");
            }
        }

        public static void ParseHarvest(string s, BuildableInfo info)
        {
            if (Regex.Match(s, @"GameData\[""harvest_ext""\]\s=\sReference\(\[\[ebpextensions\\harvest_ext\.lua\]\]\)").Success)
            {
                HarvestInfo hInfo = null;

                if (info.Extensions.ContainsKey("harvest"))
                    hInfo = info.Extensions["harvest"] as HarvestInfo;
                else
                {
                    hInfo = new HarvestInfo();
                    info.Extensions.Add("harvest", hInfo);
                }
                hInfo.Parent = info;

                double numValue = 0;
                string stringValue = "";

                if (ReadNumericValue(s, @"GameData\[""harvest_ext""\]\[""spawn_slot_a_bodies_requirement""\]\s=\s(.*)", out numValue))
                    hInfo.Slot1_Bodies = (int)numValue;
                if (ReadNumericValue(s, @"GameData\[""harvest_ext""\]\[""spawn_slot_b_bodies_requirement""\]\s=\s(.*)", out numValue))
                    hInfo.Slot2_Bodies = (int)numValue;
                if (ReadNumericValue(s, @"GameData\[""harvest_ext""\]\[""spawn_slot_c_bodies_requirement""\]\s=\s(.*)", out numValue))
                    hInfo.Slot3_Bodies = (int)numValue;
                if (ReadStringValue(s, @"GameData\[""harvest_ext""\]\[""spawn_slot_a_squad""\]\s=\s""(.*)""", out stringValue))
                    hInfo.Slot1_Squad = Translation.Translate(stringValue);
                if (ReadStringValue(s, @"GameData\[""harvest_ext""\]\[""spawn_slot_b_squad""\]\s=\s""(.*)""", out stringValue))
                    hInfo.Slot2_Squad = Translation.Translate(stringValue);
                if (ReadStringValue(s, @"GameData\[""harvest_ext""\]\[""spawn_slot_c_squad""\]\s=\s""(.*)""", out stringValue))
                    hInfo.Slot3_Squad = Translation.Translate(stringValue);

                ParseRequirements(s, hInfo, "harvest_ext");
            }
        }

        public static void ParseCannibalize(string s, BuildableInfo info)
        {

            if (Regex.Match(s, @"GameData\[""squad_cannibalize_ext""\]\s=\sReference\(\[\[sbpextensions\\squad_cannibalize_ext\.lua\]\]\)").Success)
            {
                CannibalizeInfo cInfo = null;

                if (info.Extensions.ContainsKey("cannibalize"))
                    cInfo = info.Extensions["cannibalize"] as CannibalizeInfo;
                else
                {
                    cInfo = new CannibalizeInfo();
                    info.Extensions.Add("cannibalize", cInfo);
                }
                cInfo.Parent = info;

                double numValue = 0;

                if (ReadNumericValue(s, @"GameData\[""squad_cannibalize_ext""\]\[""max_cannibalism_bonus""\]\s=\s(?<duration>.*)", out numValue))
                    cInfo.MaxHP = (int)numValue;
                ParseRequirements(s, cInfo, "squad_cannibalize_ext");
            }
        }

        public static void ParseDirectSpawn(string s, BuildableInfo info)
        {
            if (Regex.Match(s, @"GameData\[""direct_spawn_ext""\]\s=\sReference\(\[\[ebpextensions\\direct_spawn_ext\.lua\]\]\)").Success)
            {
                DirectSpawnInfo dInfo = null;

                if (info.Extensions.ContainsKey("spawn"))
                    dInfo = info.Extensions["spawn"] as DirectSpawnInfo;
                else
                {
                    dInfo = new DirectSpawnInfo();
                    info.Extensions.Add("spawn", dInfo);
                }
                dInfo.Parent = info;
                string stringValue = "";
                double numValue = 0;
                if (ReadStringValue(s, @"GameData\[""direct_spawn_ext""\]\[""spawned_squad""\]\s=\s""(.*)""", out stringValue))
                    dInfo.Spawn = stringValue;

                if (ReadNumericValue(s, @"GameData\[""direct_spawn_ext""\]\[""health_cost_fraction_of_base""\]\s=\s(.*)", out numValue))
                    dInfo.HealthFraction = numValue;

                if (ReadNumericValue(s, @"GameData\[""direct_spawn_ext""\]\[""recharge_period""\]\s=\s(.*)", out numValue))
                    dInfo.Recharge = (int)numValue;
                                    
                if (ReadStringValue(s, @"GameData\[""direct_spawn_ext""\]\[""button_texture""\]\s=\s""(.*)""", out stringValue))
                    dInfo.Icon = stringValue;

                dInfo.Name = Translation.Translate(dInfo.Spawn) + " Spawn"; 

                ParseRequirements(s, dInfo, "direct_spawn_ext");
            }
        }


        public static void ParseFear(string s, BuildableInfo info)
        {
            if (Regex.Match(s, @"GameData\[""squad_fear_ext""\]\s=\sReference\(\[\[sbpextensions\\squad_fear_ext\.lua\]\]\)").Success)
            {
                FearInfo fInfo = null;
                if (info.Extensions.ContainsKey("fear"))
                    fInfo = info.Extensions["fear"] as FearInfo;
                else
                {
                    fInfo = new FearInfo();
                    info.Extensions.Add("fear", fInfo);
                }
                fInfo.Parent = info;
                string stringValue = "";
                double numValue = 0;

                if (ReadNumericValue(s, @"GameData\[""squad_fear_ext""\]\[""recharge_time""\]\s=\s(?<recharge>.*)", out numValue))
                    fInfo.RechargeTime = numValue;
                if (ReadNumericValue(s, @"GameData\[""squad_fear_ext""\]\[""area_effect""\]\[""area_effect_information""\]\[""radius""\]\s=\s(?<radius>.*)", out numValue))
                    fInfo.Radius = numValue;
                if (ReadNumericValue(s, @"GameData\[""squad_fear_ext""\]\[""effect_time""\]\s=\s(?<duration>.*)", out numValue))
                    fInfo.Duration = numValue;
                
                if (ReadStringValue(s, @"GameData\[""squad_fear_ext""\]\[""ui_icon_name""\]\s=\s""(?<icon>.*)""", out stringValue))
                    fInfo.Icon = stringValue;

                if (ReadNumericValue(s, @"GameData\[""squad_fear_ext""\]\[""ui_title""\]\s=\s""\$(?<id>.*)""", out numValue))
                    fInfo.Name = Translation.Translate((int)numValue);
                //GameData["squad_fear_ext"]["area_effect"]["weapon_damage"]["armour_damage"]["morale_damage"] = 40.00000
                if (ReadNumericValue(s, @"GameData\[""squad_fear_ext""\]\[""area_effect""\]\[""weapon_damage""\]\[""armour_damage""\]\[""morale_damage""\]\s=\s(?<morale_damage>.*)", out numValue))
                    fInfo.Weapon.MoraleDamage = numValue;

                if (ReadStringValue(s, @"\[""area_effect""\]\[""area_effect_information""\]\[""filter_type""\]\s=\sReference\(\[\[type_areafilter\\(.*).lua\]\]\)", out stringValue))
                    fInfo.AOEFilter = stringValue;

                #region Filters
                MatchCollection mtcs = Regex.Matches(s, @"\[""area_effect""\]\[""area_effect_information""\]\[""target_filter""\]\[""entry_([0-9][0-9])""\]\s=\sReference\(\[\[type_armour\\(.*)\.lua\]\]\)");
                foreach (Match m in mtcs)
                {
                    ArmorTypes armor;
                    try
                    {
                        armor = (ArmorTypes)Enum.Parse(typeof(ArmorTypes), m.Groups[2].Value);
                    }
                    catch { armor = ArmorTypes.unknown; }
                    if (!fInfo.Filter.Contains(armor))
                        fInfo.Filter.Add(armor);

                }
                #endregion

                ParseRequirements(s, fInfo, "squad_fear_ext");
                ParseModifiers(s,fInfo,fInfo.Modifiers, "squad_fear_ext","modifiers");
            }
        }
        
        public static void ParseLightningField(string s, BuildableInfo info)
        {

            if (Regex.Match(s, @"GameData\[""squad_lightning_field_ext""\]\s=\sReference\(\[\[sbpextensions\\squad_lightning_field_ext\.lua\]\]\)").Success)
            {
                LightningFieldInfo lInfo = null;
                
                if (info.Extensions.ContainsKey("lightningfield"))
                    lInfo = info.Extensions["lightningfield"] as LightningFieldInfo;
                else
                {
                    lInfo = new LightningFieldInfo();
                    info.Extensions.Add("lightningfield", lInfo);
                }
                lInfo.Parent = info;

                double numValue = 0;
                if (ReadNumericValue(s, @"GameData\[""squad_lightning_field_ext""\]\[""discharge_max_radius""\]\s=\s(?<radius>.*)", out numValue))
                    lInfo.Radius = (int)numValue;
                if (ReadNumericValue(s, @"GameData\[""squad_lightning_field_ext""\]\[""discharge_damage_ratio""\]\s=\s(?<damageRatio>.*)", out numValue))
                    lInfo.DischargeDamageRatio = numValue;
                if (ReadNumericValue(s, @"GameData\[""squad_lightning_field_ext""\]\[""recharge_max""\]\s=\s(?<max>.*)", out numValue))
                    lInfo.MaxCharge = numValue;
                if (ReadNumericValue(s, @"GameData\[""squad_lightning_field_ext""\]\[""recharge_min_fraction""\]\s=\s(?<minFraction>.*)", out numValue))
                    lInfo.RechargeMinFraction = numValue;
                if (ReadNumericValue(s, @"GameData\[""squad_lightning_field_ext""\]\[""reflection_damage_ratio""\]\s=\s(?<reflectRatio>.*)", out numValue))
                    lInfo.ReflectedDamageRatio = numValue;
                if (ReadNumericValue(s, @"GameData\[""squad_lightning_field_ext""\]\[""recharge_impact_ratio""\]\s=\s(?<impactRatio>.*)", out numValue))
                    lInfo.RechargeImpactRatio = numValue;
                
            }
        }

        public static void ParseEnemyPossession(string s, BuildableInfo info)
        {                      

            if (Regex.Match(s, @"GameData\[""squad_possess_enemy_ext""\]\s=\sReference\(\[\[sbpextensions\\squad_possess_enemy_ext\.lua\]\]\)").Success)
            {
                PossessEnemyInfo pInfo = null;
                
                if (info.Extensions.ContainsKey("enemypossession"))
                    pInfo = info.Extensions["enemypossession"] as PossessEnemyInfo;
                else
                {
                    pInfo = new PossessEnemyInfo();
                    info.Extensions.Add("enemypossession", pInfo);
                }
                pInfo.Parent = info;
                
                double numValue = 0;
                if (ReadNumericValue(s, @"GameData\[""squad_possess_enemy_ext""\]\[""possess_entity_max_radius""\]\s=\s(?<range>.*)", out numValue))
                    pInfo.Range = (int)numValue;
                if (ReadNumericValue(s, @"GameData\[""squad_possess_enemy_ext""\]\[""take_possession_duration""\]\s=\s(?<duration>.*)", out numValue))
                    pInfo.Duration = (int)numValue;
                ParseRequirements(s, pInfo, "squad_possess_enemy_ext");
            }
        }

        public static void ParsePossession(string s, BuildableInfo info)
        {
            if (Regex.Match(s, @"GameData\[""possess_ext""\]\s=\sReference\(\[\[ebpextensions\\possess_ext\.lua\]\]\)").Success)
            {
                PossessInfo pInfo = null;

                if (info.Extensions.ContainsKey("possession"))
                    pInfo = info.Extensions["possession"] as PossessInfo;
                else
                {
                    pInfo = new PossessInfo();
                    info.Extensions.Add("possession", pInfo);
                }
                pInfo.Parent = info;
                string stringValue = "";
                double numValue = 0;
                if (ReadStringValue(s, @"GameData\[""possess_ext""\]\[""squad_replacement_name""\]\s=\s""(?<possess>.*)""", out stringValue))
                    pInfo.Replacement = stringValue;
                if (ReadNumericValue(s, @"GameData\[""possess_ext""\]\[""automatic_possession_time""\]\s=\s(?<duration>.*)", out numValue))
                    pInfo.Duration = (int)numValue;

                if (ReadStringValue(s, @"GameData\[""possess_ext""\]\[""ui_info""\]\[""icon_name""\]\s=\s""(?<icon>.*)""", out stringValue))
                    pInfo.Icon = stringValue;

                if (ReadNumericValue(s, @"GameData\[""possess_ext""\]\[""ui_info""\]\[""screen_name_id""\]\s=\s""\$(?<id>.*)""", out numValue))
                    pInfo.Name = Translation.Translate((int)numValue);
 
                ParseRequirements(s, pInfo,"possess_ext");
            }
        }

        public static void ParseEntrench(string s, BuildableInfo info)
        {
            if (Regex.Match(s, @"GameData\[""entrench_ext""\]\s=\sReference\(\[\[ebpextensions\\entrench_ext\.lua\]\]\)").Success)
            {
                EntrenchInfo eInfo = null;

                if (info.Extensions.ContainsKey("entrench"))
                    eInfo = info.Extensions["entrench"] as EntrenchInfo;
                else
                {
                    eInfo = new EntrenchInfo();
                    info.Extensions.Add("entrench", eInfo);
                }
                eInfo.Parent = info;
                string stringValue = "";
                //double numValue = 0;
                string unit = "";

                if (ReadStringValue(s, @"GameData\[""entrench_ext""\]\[""entrenched_blueprint_name""\]\s=\s""(?<entrenchedUnit>.*)""", out stringValue))
                {
                    unit = stringValue;
                    string unitLua = unit.Replace("\\\\", "\\");

                    if (unitLua != "")
                    {
                        string unitLuaPath = DataPath.GetPath(unitLua);

                        if (unitLuaPath != "")
                        {
                            eInfo.EntrenchedUnit = new UnitInfo();
                            eInfo.EntrenchedUnit.Parent = info;
                            ParseUnit(unitLuaPath, eInfo.EntrenchedUnit);
                        }
                    }

                    

                }
                if (ReadStringValue(s, @"GameData\[""entrench_ext""\]\[""icon_entrench""\]\s=\s""(.*)""", out stringValue))
                    eInfo.Icon = stringValue;
                ParseModifiers(s, eInfo, eInfo.Modifiers, "entrench_ext", "entrenched_modifiers");
                ParseRequirements(s, eInfo, "entrench_ext");
            }
        }
        public static void ParseResearch(string path, ResearchInfo info)
        {
            
            if (path != null && path != "")
            {
                StreamReader file = new StreamReader(File.OpenRead(path), System.Text.Encoding.ASCII);
                file.BaseStream.Seek(0, SeekOrigin.Begin);

                string s = file.ReadToEnd();
                file.Close();

                
                #region INHERITANCE

                Match mc = Regex.Match(s, @"GameData\s=\sInherit\(\[\[(?<inherit>.*)\]\]\)");
                Group grp = mc.Groups["inherit"];
                if (grp.Success && grp.Value!="")
                {
                    try
                    {
                        ParseResearch(DataPath.GetPath(DataPath.GetCategoryPath(grp.Value,InfoTypes.Research,info.Race)), info);
                       
                    }
                    catch (Exception e)
                    {
                    }
                }
                #endregion

                double numValue = 0.0;

                #region COSTS

                if (ReadNumericValue(s, @"GameData\[""time_cost""\]\[""cost""\]\[""requisition""\]\s=\s(.*)", out numValue))
                    info.RequisitionCost = (int)numValue;
                if (ReadNumericValue(s, @"GameData\[""time_cost""\]\[""cost""\]\[""power""\]\s=\s(.*)", out numValue))
                    info.PowerCost = (int)numValue;
                if (ReadNumericValue(s, @"GameData\[""time_cost""\]\[""time_seconds""\]\s=\s(.*)", out numValue))
                    info.ResearchTime = (int)numValue;

                #endregion

                ParseToolTips(s, info);

                if (ReadNumericValue(s, @"GameData\[""ui_info""\]\[""screen_name_id""\]\s=\s""\$(.*)""", out numValue))
                    info.UI = (int)numValue;
                string luaName = Regex.Replace(path, @".*\\", "");
                info.Name = Translation.Translate(info.UI, luaName);
                string strValue = "";
                if (ReadStringValue(s, @"GameData\[""ui_info""\]\[""icon_name""\]\s=\s""(.*)""", out strValue))
                    info.Icon = strValue;

                ParseRequirements(s, info);
                ParseModifiers(s, info, info.Modifiers, "");
                
            }
        }

        public static void ParseToolTips(string s, BuildableInfo info)
        {
            MatchCollection mcc = Regex.Matches(s, @"\[""ui_info""\]\[""help_text_list""\]\[""(?<entry>text_[0-9][0-9])""\]\s=\s""\$(?<tips>.*)""");
            if (mcc.Count > 0)
            {
                foreach (Match m in mcc)
                {
                    string entry = m.Groups["entry"].Value;
                    int tips = System.Convert.ToInt32(m.Groups["tips"].Value, NumberFormat);
                    if (info.ToolTipsInfo.ContainsKey(entry))
                        info.ToolTipsInfo[entry] = tips;
                    else info.ToolTipsInfo.Add(entry, tips);
                }
            }
        }
        public static void ParseRequirements(string lua, BaseInfo info)
        {
            ParseRequirements(lua, info, "");
        }
        public static void ParseRequirements(string lua, BaseInfo info, string extension)
        {
            string ext = "";
            if (extension != "")
                ext += @"\[""" + extension + @"""\]";

            # region OLD-REQUIREMENTS
            if (info.Requirements.Count > 0) // Re-Parse old Requirements
            {
                ArrayList l = new ArrayList(info.Requirements.Values);
                foreach (RequirementInfo reqInfo in l)
                {
                    Match m = Regex.Match(lua, ext + @"\[""requirements""\]\[""" + reqInfo.Requirement + @"""\]\s=\sReference\(\[\[requirements\\(?<reqType>.*)(.lua)\]\]\)");
                    Match display = Regex.Match(lua, @"\[""" + reqInfo.Requirement + @"""\]\[""is_display_requirement""\]\s=\strue");
                    if (m.Success)
                    {
                        Group reqType = m.Groups["reqType"];
                        if (reqType.Value != reqInfo.RequirementType || display.Success) // The Requirement Type is changed or is only a display requirement
                        {
                            info.Requirements.Remove(reqInfo.Requirement);
                            continue;
                        }
                    }
                }
            }
            #endregion

            #region NEW REQUIREMENTS
            // search for new requirements
            MatchCollection mcc = Regex.Matches(lua, ext + @"\[""requirements""\]\[""(?<requirement>.*)""\]\s=\sReference\(\[\[requirements\\(?<reqType>.*)(.lua)\]\]\)");
            if (mcc.Count > 0)
            {
                foreach (Match m in mcc)
                {
                    Group req = m.Groups["requirement"];
                    Group reqType = m.Groups["reqType"];
                    Match display = Regex.Match(lua, @"\[""" + req + @"""\]\[""is_display_requirement""\]\s=\strue");


                    if (req.Success && !display.Success && !info.Requirements.Contains(req.Value))
                    {
                        RequirementInfo newReqInfo = RequirementInfo.GetRequirement(req.Value, reqType.Value);
                        if (newReqInfo != null)
                        {
                            newReqInfo.Parent = info;
                            info.Requirements.Add(req.Value, newReqInfo);
                        }
                    }
                }
            }
            #endregion

            #region REQUIREMENTS PARSING
            if (info.Requirements.Count > 0)
                foreach (RequirementInfo reqInfo in info.Requirements.Values)
                    reqInfo.Parse(lua);
            #endregion
        }

        public static void ParseModifiers(string lua, BaseInfo parent, Hashtable modifiers, AreaOfEffectTypes aoeType)
        {
            ParseModifiers(lua, parent, modifiers, "", "");            
        }

        public static void ParseModifiers(string lua, BaseInfo parent, Hashtable modifiers, string extension)
        {
            ParseModifiers(lua, parent, modifiers, extension, "");
        }

        public static void InheritFromExtension(string extPath,BaseInfo parent, Hashtable modifiers, string extension,string modifierClass)
        {
                 
            string lua = "";
            try
            {
                StreamReader file = new StreamReader(File.OpenRead(DataPath.GetPath(extPath)), System.Text.Encoding.ASCII);
                file.BaseStream.Seek(0, SeekOrigin.Begin);

                lua = file.ReadToEnd();
                file.Close();
            }
            catch { return; }

            

            string ext = "";
            if (extension != "")
                ext = @"\[""" + extension + @"""\]";
            string modClassString = @"[\w_]*modifiers[\w_]*";
            if (modifierClass != "")
                modClassString = modifierClass;



            #region EXT MODIFIERS
       
            MatchCollection mcc = Regex.Matches(lua,  @"\[""(?<modifier_class>" + modClassString + @")""\]\[""(?<modifier>[\w_]+)""\](\[""modifier""\])?\s=\sReference\(\[\[modifiers\\(?<modType>[\w_]+)(.lua)\]\]\)");

            if (mcc.Count > 0)
            {
               
                foreach (Match m in mcc)
                {

                    string modClass = m.Groups["modifier_class"].Value;
                    string mod = m.Groups["modifier"].Value;
                    string modType = m.Groups["modType"].Value;                   

                    string modID = modClass + "-" + mod;
                    if (modType != "no_modifier" && !modifiers.Contains(modID))
                    {

                        ModifierInfo newModInfo = ModifierInfo.GetModifier(mod, modClass, modType);

                        if (newModInfo != null)
                        {
                            newModInfo.Parent = parent;
                            newModInfo.Extension = "";
                            modifiers.Add(modID, newModInfo);
                        }
                    }
                }
            }
            #endregion

            #region MODIFIER PARSING
            if (modifiers.Count > 0)
                foreach (ModifierInfo modInfo in modifiers.Values)
                {
                    modInfo.Parse(lua);
                }
            #endregion
        }
  
        public static void ParseModifiers(string lua, BaseInfo parent, Hashtable modifiers, string extension, string modifierClass)
        {
            string ext = "";
            if (extension != "")
                ext = @"\[""" + extension + @"""\]";
            string modClassString = @"[\w_]*modifiers[\w_]*";
            if (modifierClass != "")
                modClassString = modifierClass;

            #region EXTENSION-MODIFIERS
            if (extension != "")
            {
                string extLua="";
                Match mext = Regex.Match(lua,@"GameData"+ext+@"\s=\sReference\(\[\[(?<ext_ref>.*)\]\]\)");
                if (mext.Success)
                {
                    extLua = mext.Groups["ext_ref"].Value;
                    InheritFromExtension(extLua, parent, modifiers, extension, modifierClass);
                }
            }
            #endregion

            # region OLD-MODIFIERS
            if (modifiers.Count > 0) // Re-Parse old Modifiers
            {
                ArrayList l = new ArrayList(modifiers.Values);
                foreach (ModifierInfo modInfo in l)
                {
                    if (modifierClass == "" || modifierClass == modInfo.ModifierClass)
                    {
                        Match m = Regex.Match(lua, ext + @"\[""" + modInfo.ModifierClass + @"""\]\[""" + modInfo.Modifier + @"""\](\[""modifier""\])?\s=\sReference\(\[\[modifiers\\(?<modType>.*)(.lua)\]\]\)");
                        if (m.Success)
                        {
                            Group modType = m.Groups["modType"];
                            if (modType.Value != modInfo.ModifierType || modType.Value == "no_modifier") // The Modifier Type has been changed or is null
                            {
                                string modID = modInfo.ModifierClass + "-" + modInfo.Modifier;
                                modifiers.Remove(modID);
                                continue;
                            }
                        }
                    }
                }
            }
            #endregion

            #region NEW MODIFIERS
 
            MatchCollection mcc = Regex.Matches(lua, ext +@"(\[""area_effect""\]\[""weapon_damage""\])?\[""(?<modifier_class>"+modClassString+@")""\]\[""(?<modifier>[\w_]+)""\](\[""modifier""\])?\s=\sReference\(\[\[modifiers\\(?<modType>[\w_]+)(.lua)\]\]\)");
           
            if (mcc.Count > 0)
            {                
                foreach (Match m in mcc)
                {
                    
                    string modClass = m.Groups["modifier_class"].Value;
                    string mod = m.Groups["modifier"].Value;
                    string modType = m.Groups["modType"].Value;


                    string modID = modClass + "-" + mod;
                    if (modType != "no_modifier" && !modifiers.Contains(modID))
                    {
                        
                        ModifierInfo newModInfo = ModifierInfo.GetModifier(mod, modClass, modType);
                        
                        if (newModInfo != null)
                        {
                            newModInfo.Parent = parent;
                            newModInfo.Extension = extension;
                            modifiers.Add(modID, newModInfo);
                        }
                    }
                }
            }
            #endregion

            #region MODIFIER PARSING
            if (modifiers.Count > 0)
                foreach (ModifierInfo modInfo in modifiers.Values)
                    modInfo.Parse(lua);

            #endregion
        }

    #endregion
    
    }
}
