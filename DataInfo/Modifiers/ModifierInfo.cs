using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace StatsCompiler
{
    public enum ModifierTypes
    {
        garrison_requisition_modifier,
        accuracy_moving_reduction_weapon_modifier,
        accuracy_ranged_weapon_modifier,
        accuracy_weapon_modifier,
        entity_accuracy_modifier,
        armour_modifier,
        capture_rate_squad_modifier,
        combat_melee_damage_modifier,
        cost_power_modifier,
        cost_requisition_modifier,
        cost_time_modifier,
        construction_speed_modifier,
        enable_infiltration,
        enable_squad_morale_damage,
        enable_abilities,
        enable_movement,
        enable_production,
        enable_squad_jump,
        enable_armour_2,
        enable_charge_modifiers,
        enable_melee_leap,
        enable_squad_reinforcement,
        enable_general_combat,
        health_regeneration_modifier,
        income_power_player_modifier,
        income_requisition_player_modifier,
        morale_maximum_squad_modifier,
        max_damage_weapon_modifier,
        max_leaders_squad_modifier,
        max_range_weapon_modifier,
        max_troopers_squad_modifier,
        max_squad_cap_player_modifier,
        max_support_cap_player_modifier,
        max_upgrades_squad_modifier,
        health_otherdamage_received_modifier,
        health_rangedamage_received_1_modifier,
        health_rangedamage_received_2_modifier,
        health_meleedamage_received_modifier,
        health_resurrect_modifier,
        morale_rangeddamage_received_modifier,
        morale_otherdamage_received_modifier,
        morale_meleedamage_received_modifier, 
        morale_rate_squad_modifier,
        population_cap_player_modifier,
        population_growth_rate_player_modifier,
        production_speed_modifier,
        reinforce_time_player_modifier,
        reload_time_weapon_modifier,
        repair_rate_modifier,
        research_time_player_modifier,
        setup_time_weapon_modifier,
        sight_radius_modifier,
        speed_maximum_modifier,
        squad_cap_player_modifier,
        support_cap_player_modifier,
        health_maximum_modifier,
        health_degeneration_modifier,
        melee_charge_range_modifier,
        default_weapon_modifier_hardpoint,
        special_attack_physics_mass
    }
    public enum UsageTypes
	{
		Multiply,
		Add,
		Enable,
		Percent
	}
	public enum ApplicationTypes
	{
		ApplyToEntity,
		ApplyToEntityType,
		ApplyToSquad,
		ApplyToSquadType,
		ApplyToPlayer,
		ApplyToWeaponType
	}

    public enum AreaOfEffectTypes
    {
        none,
        area_effect,
        backfire_area_effect
    }

    public class ModifierInfo:IComparable
	{
		public string Target;

        public string ModifierClass;
		public string ModifierType;
		public string Modifier;
		public string ModifierTranslation;
		public string Extension;
        public AreaOfEffectTypes AreaOfEffectType;
		public ApplicationTypes ApplicationType;
		public UsageTypes UsageType;
		        
		public bool Percent;
		public double LifeTime;
		public bool DoNotStacks;
		public BaseInfo Parent;

        private double m_Amount;
        public double Amount
        {
            get 
            {
                return m_Amount;  
            }
            set 
            { 
                m_Amount = value;
            }
        }

        public int HardPoint;

        public ModifierInfo(string modifier, string modClass, string modifierType, string translation): this(modifier,modClass, modifierType, translation, false)
		{
		}
        public ModifierInfo(string modifier, string modClass, string modifierType, string translation, bool percent)
		{
			Target = "";
            ModifierClass = modClass;
            Modifier = modifier;
			ModifierType = modifierType;
			ModifierTranslation = translation;
			Percent = percent;
            ApplicationType = ApplicationTypes.ApplyToEntity;
            UsageType = UsageTypes.Multiply;
            Amount = 1;
		}

        public bool IsValid
        {
            get
            {
                if (UsageType == UsageTypes.Add && Amount == 0)
                    return false;
                if (UsageType == UsageTypes.Multiply && Amount == 1)
                    return false;
                return true;
            }
        }
		
        public virtual string GetModTranslation()
		{
			return ModifierTranslation;
		}
		
		public static UsageTypes GetUsageType(string s)
		{
			if (s == "tp_mod_usage_addition")
				return UsageTypes.Add;
			if (s == "tp_mod_usage_multiplication")
				return UsageTypes.Multiply;
			if (s == "tp_mod_usage_enable")
				return UsageTypes.Enable;
			return UsageTypes.Percent;
		}

        public virtual string TargetName
        {
            get { return Translation.Translate(Target); }
        }

		public static ApplicationTypes GetApplicationType(string s)
		{
			if (s == "tp_mod_apply_to_entity")
				return ApplicationTypes.ApplyToEntity;
			if (s == "tp_mod_apply_to_entity_type")
				return ApplicationTypes.ApplyToEntityType;
			if (s == "tp_mod_apply_to_squad")
				return ApplicationTypes.ApplyToSquad;
			if (s == "tp_mod_apply_to_squad_type")
				return ApplicationTypes.ApplyToSquadType;
			if (s == "tp_mod_apply_to_player")
				return ApplicationTypes.ApplyToPlayer;
			
			return ApplicationTypes.ApplyToWeaponType;
		}
		
		public static ModifierInfo GetModifier(string mod, string modClass, string modType)
		{
			if (modType == "garrison_requisition_modifier")
                return new ModifierInfo(mod, modClass, modType, "requisition aquired", true); 
			if (modType == "accuracy_moving_reduction_weapon_modifier")
                return new ModifierInfo(mod, modClass, modType, "accuracy reduction on moving", false);					 
			if (modType == "accuracy_ranged_weapon_modifier")
                return new AccuracyModifier(mod, modClass, modType, "ranged accuracy");
			if (modType == "accuracy_weapon_modifier")
                return new AccuracyModifier(mod, modClass, modType, "accuracy");
			if (modType == "entity_accuracy_modifier")
                return new AccuracyModifier(mod, modClass, modType, "accuracy");
			if (modType == "armour_modifier")
                return new ModifierInfo(mod, modClass, modType, "armor strength", false);
			if (modType == "capture_rate_squad_modifier")
                return new ModifierInfo(mod, modClass, modType, "capturing rate", true);
			if (modType =="combat_melee_damage_modifier")
                return new ModifierInfo(mod, modClass, modType, "melee damage done", true);	
			if (modType =="cost_power_modifier")
                return new CostModifier(mod, modClass,"cost_power_modifier","power cost");
            if (modType == "cost_requisition_modifier")
                return new CostModifier(mod, modClass,"cost_requisition_modifier","requisition cost");
			if (modType =="cost_time_modifier")
				return new CostModifier(mod,modClass,"cost_time_modifier","build time");
			if (modType =="construction_speed_modifier")
                return new ModifierInfo(mod, modClass, modType, "construction speed", true);
			if (modType == "enable_infiltration")
				return new EnableInfiltrationModifier(mod, modClass);
			if (modType == "enable_squad_morale_damage")
                return new EnableMoraleDamageModifier(mod, modClass);
			if (modType == "enable_abilities")
                return new EnableAbilitiesModifier(mod, modClass);
			if (modType == "enable_movement")
                return new EnableMovementModifier(mod, modClass);
			if (modType == "enable_production")
                return new EnableProductionModifier(mod, modClass);
			if (modType == "enable_squad_jump")
                return new EnableSquadJumpModifier(mod, modClass);
            if (modType == "enable_armour_2")
                return new EnableArmorModifier(mod, modClass);
            if (modType == "enable_charge_modifiers")
            {
                return new EnableChargeModifier(mod, modClass);
            }
            if (modType == "health_regeneration_modifier")
                return new ModifierInfo(mod, modClass, modType, "health regeneration", false);
            if (modType == "health_resurrect_modifier")
                return new ResurrectionModifier(mod, modClass);
			if (modType =="income_power_player_modifier")
				return new ModifierInfo(mod, modClass,modType,"power income",true);
			if (modType =="income_requisition_player_modifier")
				return new ModifierInfo(mod,modClass,modType,"requisition income",true);
			if (modType =="morale_maximum_squad_modifier")
				return new ModifierInfo(mod, modClass,modType,"morale",false);
			if (modType =="max_damage_weapon_modifier")
                return new WeaponDamageModifier(mod, modClass, modType, "damage");
			if (modType =="max_leaders_squad_modifier")
                return new ModifierInfo(mod, modClass, modType, "max leaders", true);
			if (modType =="max_range_weapon_modifier")
				return new WeaponMaxRangeModifier(mod,modClass);
			if (modType =="max_troopers_squad_modifier")
				return new SquadSizeModifier(mod,modClass);
			if (modType =="health_otherdamage_received_modifier")
                return new ModifierInfo(mod, modClass, modType, "received special damage", true);
			if (modType =="health_meleedamage_received_modifier")
                return new ModifierInfo(mod, modClass, modType, "received melee damage", true);
            if (modType == "health_rangedamage_received_1_modifier")
                return new ModifierInfo(mod, modClass, modType, "received ranged damage", true);
            //if (modType == "health_rangedamage_received_2_modifier")
              //  return new ModifierInfo(mod, modClass, modType, "received ranged damage", true);
            if (modType == "morale_otherdamage_received_modifier")
                return new ModifierInfo(mod, modClass, modType, "received special morale damage", true);
            if (modType == "morale_meleedamage_received_modifier")
                return new ModifierInfo(mod, modClass, modType, "received melee morale damage", true);
            if (modType == "morale_rangeddamage_received_modifier")
                return new ModifierInfo(mod, modClass, modType, "received ranged morale damage", true);
            if (modType =="morale_rate_squad_modifier")
                return new ModifierInfo(mod, modClass, modType, "morale regeneration", true);
			if (modType =="population_cap_player_modifier")
				return new PopulationCapModifier(mod,modClass);
			if (modType =="population_growth_rate_player_modifier")
				return new PopulationGrowRateModifier(mod,modClass);
			if (modType =="production_speed_modifier")
                return new ModifierInfo(mod, modClass, modType, "production speed", true);
			if (modType =="reinforce_time_player_modifier")
                return new ModifierInfo(mod, modClass, modType, "global reinforce time", true);
			if (modType =="reload_time_weapon_modifier")
                return new WeaponReloadModifier(mod, modClass);
			if (modType =="repair_rate_modifier")
                return new ModifierInfo(mod, modClass, modType, "repair rate", true);
			if (modType =="research_time_player_modifier")
                return new ModifierInfo(mod, modClass, modType, "research time", true);
			if (modType =="setup_time_weapon_modifier")
                return new WeaponSetupModifier(mod, modClass);
			if (modType =="sight_radius_modifier")
                return new ModifierInfo(mod, modClass, modType, "sight radius", false);
			if (modType =="speed_maximum_modifier")
                return new ModifierInfo(mod, modClass, modType, "speed", true);
			if (modType =="squad_cap_player_modifier")
				return new SquadCapModifier(mod,modClass);
			if (modType =="support_cap_player_modifier")
                return new SupportCapModifier(mod, modClass);
            if (modType == "max_squad_cap_player_modifier")
                return new MaxSquadCapModifier(mod, modClass);
            if (modType == "max_support_cap_player_modifier")
                return new MaxSupportCapModifier(mod, modClass);
			if (modType == "health_maximum_modifier")
                return new ModifierInfo(mod, modClass, modType, "maximum health", false);
			if (modType == "health_degeneration_modifier")
				return new HealthDegenerationModifier(mod,modClass);
			if (modType == "melee_charge_range_modifier")
                return new ModifierInfo(mod, modClass, modType, "charge range", false);
            if (modType == "enable_melee_leap")
                return new EnableLeapModifier(mod, modClass);
            if (modType == "enable_general_combat")
                return new EnableCombat(mod, modClass);
            if (modType == "enable_squad_reinforcement")
                return new EnableReinforcement(mod, modClass);
            if (modType == "max_upgrades_squad_modifier")
                return new MaximumHeavyWeaponModifier(mod, modClass);
            if (modType == "special_attack_physics_mass")
                return new ModifierInfo(mod, modClass, modType, "Mass", false);

			Match m = Regex.Match(modType,@"default_weapon_modifier_hardpoint([0-9]+)");
			if (m.Success)
                return new WeaponModifier(mod, modClass, m.Groups[1].Value);
                                   
            m = Regex.Match(modType, @"enable_hardpoint_([0-9]+)");
            if (m.Success)
                return new EnableWeaponModifier(mod, modClass, m.Groups[1].Value);
            
            return null;			
		}

        public void Parse(string lua)
        {
            string ext = "";
            if (Extension != "")
                ext = @"\[""" + Extension + @"""\]"; 
            Match m = Regex.Match(lua, ext + @"(\[""area_effect""\]\[""weapon_damage""\])?\["""+ModifierClass+@"""\]\[""" + Modifier + @"""\](\[""modifier""\])?\[""application_type""\]\s=\sReference\(\[\[type_modifierapplicationtype\\(?<applicationType>.*).lua\]\]\)");
			if (m.Success)
				ApplicationType = GetApplicationType(m.Groups["applicationType"].Value);
            m = Regex.Match(lua, ext + @"(\[""area_effect""\]\[""weapon_damage""\])?\["""+ModifierClass+@"""\]\[""" + Modifier + @"""\](\[""modifier""\])?\[""usage_type""\]\s=\sReference\(\[\[type_modifierusagetype\\(?<usageType>.*).lua\]\]\)");
			if (m.Success)
				UsageType = GetUsageType(m.Groups["usageType"].Value);
            m = Regex.Match(lua, ext + @"(\[""area_effect""\]\[""weapon_damage""\])?\["""+ModifierClass+@"""\]\[""" + Modifier + @"""\](\[""modifier""\])?\[""target_type_name""\]\s=\s""(?<target>.*)""");
			if (m.Success)
                Target = m.Groups["target"].Value;
            m = Regex.Match(lua, ext + @"(\[""area_effect""\]\[""weapon_damage""\])?\["""+ModifierClass+@"""\]\[""" + Modifier + @"""\](\[""modifier""\])?\[""value""\]\s=\s(?<value>.*)");
            if (m.Success)   
                Amount = System.Convert.ToDouble(m.Groups["value"].Value, LuaParser.NumberFormat);
            
            m = Regex.Match(lua, ext + @"(\[""area_effect""\]\[""weapon_damage""\])?\["""+ModifierClass+@"""\]\[""" + Modifier + @"""\](\[""max_lifetime""\])\s=\s(?<lifeTime>.*)");
            if (m.Success)
            {
                try
                {
                    LifeTime = System.Convert.ToDouble(m.Groups["lifeTime"].Value, LuaParser.NumberFormat);
                }
                catch {}
            }
            m = Regex.Match(lua, ext + @"(\[""area_effect""\]\[""weapon_damage""\])?\["""+ModifierClass+@"""\]\[""" + Modifier + @"""\]\[""modifier""\]?\[""exclusive""\]\s=\strue");
			if (m.Success)
				DoNotStacks = true;
		}
        
        public virtual string GetTarget()
        {
            return GetTarget("of");
        }
        public string GetTarget(string prep)
        {
            // SQUAD
            if (Parent is SquadInfo)
            {
                if (ApplicationType == ApplicationTypes.ApplyToSquad)
                    return " "+prep+" the squad";
            }
            // UNIT
            else if (Parent is UnitInfo)
            {
                switch (ApplicationType)
                {
                    case ApplicationTypes.ApplyToSquad:
                        return " "+prep+" the squad";
                    case ApplicationTypes.ApplyToEntityType:
                        if (Target != "")
                            return prep+" " + HtmlCompiler.GetPlural(TargetName);
                        break;
                    case ApplicationTypes.ApplyToEntity:
                        if (Target != "")
                            return prep+" " + Translation.Translate( ((UnitInfo)Parent).Name );
                        break;
                }
            }
            // Abilities
            else if (Parent is SkillInfo)
            {
                SkillInfo sk = Parent as SkillInfo;
                bool aoe = (sk.Radius > 0);
                bool isTarget = (sk.ActivationType == "tp_ability_activation_targeted");

                if (isTarget)
                {
                    if (aoe)
                        return prep+" each target in the area of effect";
                    else
                        switch (ApplicationType)
                        {
                            case ApplicationTypes.ApplyToSquad:
                                return prep+" targeted squad";

                            case ApplicationTypes.ApplyToEntity:
                                return prep+" the target ";
                        }
                }
                else
                    switch (ApplicationType)
                    {              
                        case ApplicationTypes.ApplyToEntity:
                            {
                                if (aoe)
                                    return prep+" each target in the area of effect";
                                break;
                            }

                        case ApplicationTypes.ApplyToWeaponType:
                            if (Target != "")
                                return prep+" " + TargetName;
                            break;
                        case ApplicationTypes.ApplyToSquadType:
                            if (Target != "")
                                return prep+" " + TargetName;
                            break;
                        case ApplicationTypes.ApplyToEntityType:
                            if (Target != "")
                                return prep+" " + TargetName;
                            break;
                    }
            }
            else if (Target != "")	// research and others
            {
                return prep+" " + TargetName;
            }

            return "";
        }
        
        public virtual string Compile()
        {
            string output = "";

            //string action   = "";
            //string modifier = "";
            string target   = "";
            //string amount   = "";

            target = GetTarget();

            if (ApplicationType != ApplicationTypes.ApplyToEntity && !(Parent is LeaderInfo) && Parent is UnitInfo && Parent.Parent != null && Parent.Parent is SquadInfo && ((SquadInfo)Parent.Parent).MaxSquadSize > 1)
                output += "Each squad member ";

            switch (UsageType)
            {
                case UsageTypes.Enable:
                    if (Amount < 0)
                        output += "Disables ";
                    else output += "Enables ";
                    break;
                case UsageTypes.Add:
                    if (Amount >= 0.0)
                        output += "Increases the";
                    else output += "Decreases the";
                    break;
                default:
                    if (Amount - 1 > 0.0)
                        output += "Increases the";
                    else output += "Decreases the";
                    break;
            }

            output += " " + GetModTranslation();

            output += " " + GetTarget();


            if (UsageType == UsageTypes.Add)
            {
                if (Percent)
                    output += " by " + Math.Round(Math.Abs(Amount * 100),3) + "%";
                else
                    output += " by " + Math.Abs(Amount);
            }
            else if (UsageType == UsageTypes.Multiply)
                output += " by " + ((double)(Math.Round((Math.Abs(1 - Amount) * 100),3))).ToString() + "%";

            if (LifeTime > 0)
                output += " for " + LifeTime.ToString() + " seconds";

            if (DoNotStacks)
                output += " (doesn't stacks)";
            
            return output;
        }
                
        public virtual int CompareTo(object o)
		{
			if (o==null || ! (o is ModifierInfo))
				return -1;
			
			ModifierInfo toCompare = o as ModifierInfo;
			if (toCompare == null || toCompare.ModifierType == null)
				return -1;
			return ModifierType.CompareTo(toCompare.ModifierType);
			
		}
	}
		
}