using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace StatsCompiler
{
	public class ReinforceTimePlayerModifier:ModifierInfo
    {
        public ReinforceTimePlayerModifier(string mod, string modClass): base(mod, modClass, "reinforce_time_player_modifier", "global reinforce time",true)
        {
            UsageType = UsageTypes.Add;
        }
    }
    public class ResearchTimePlayerModifier : ModifierInfo
    {
        public ResearchTimePlayerModifier(string mod, string modClass)
            : base(mod, modClass, "research_time_player_modifier", "research time", true)
        {
            UsageType = UsageTypes.Add;
        }
    }
    public class AccuracyModifier:ModifierInfo
	{
		public AccuracyModifier(string mod, string modClass,string modType,string valueName):base(mod,modClass,modType,valueName,true)
		{
			ApplicationType = ApplicationTypes.ApplyToWeaponType;
		}
	}

	public class SquadSizeModifier:ModifierInfo
	{
		public SquadSizeModifier(string mod, string modClass):base(mod,modClass,"max_troopers_squad_modifier","maximum members",false)
		{
			UsageType = UsageTypes.Add;
            ApplicationType = ApplicationTypes.ApplyToSquadType;
            Amount = 0;
		}

	}

    public class MaximumHeavyWeaponModifier : ModifierInfo
    {
        public MaximumHeavyWeaponModifier(string mod, string modClass)
            : base(mod, modClass, "max_upgrades_squad_modifier", "maximum number of Heavy Weapons", false)
        {
            UsageType = UsageTypes.Add;
            Amount = 0;
        }

    }  

	public class SquadCapModifier:ModifierInfo
	{
		public SquadCapModifier(string mod, string modClass):base(mod,modClass,"squad_cap_player_modifier","Squad Cap",false)
		{
			UsageType = UsageTypes.Add;
			ApplicationType = ApplicationTypes.ApplyToPlayer;
            Amount = 0;
		}
	}

    public class SupportCapModifier : ModifierInfo
    {
        public SupportCapModifier(string mod, string modClass): base(mod, modClass, "support_cap_player_modifier", "Support Cap", false)
        {
            UsageType = UsageTypes.Add;
            ApplicationType = ApplicationTypes.ApplyToPlayer;
            Amount = 0;
        }
    }
    public class MaxSquadCapModifier : ModifierInfo
    {
        public MaxSquadCapModifier(string mod, string modClass)
            : base(mod, modClass, "max_squad_cap_player_modifier", "Maximum Squad Cap", false)
        {
            UsageType = UsageTypes.Add;
            ApplicationType = ApplicationTypes.ApplyToPlayer;
        }
    }
    public class MaxSupportCapModifier : ModifierInfo
    {
        public MaxSupportCapModifier(string mod, string modClass)
            : base(mod, modClass, "max_support_cap_player_modifier", "Maximum Support Cap", false)
        {
            UsageType = UsageTypes.Add;
            ApplicationType = ApplicationTypes.ApplyToPlayer;
        }
    }
	public class WeaponMaxRangeModifier:ModifierInfo
	{
		public WeaponMaxRangeModifier(string mod, string modClass):base(mod,modClass,"max_range_weapon_modifier","maximum range",false)
		{
		}
        public override string TargetName
        {
            get { return Translation.TransOwner(Target); }
        }
        
    }
    public class WeaponSetupModifier : ModifierInfo
    {
        public WeaponSetupModifier(string mod, string modClass)
            : base(mod, modClass, "setup_time_weapon_modifier", "set-up time", false)
        {
        }
        public override string TargetName
        {
            get { return Translation.TransOwner(Target); }
        }

    }
    public class WeaponReloadModifier : ModifierInfo
    {
        public WeaponReloadModifier(string mod, string modClass)
            : base(mod, modClass, "reload_time_weapon_modifier", "reload time", false)
        {
        }
        public override string TargetName
        {
            get { return Translation.TransOwner(Target); }
        }

    }
	public class EnableMovementModifier:ModifierInfo
	{
		public EnableMovementModifier(string mod, string modClass):base(mod,modClass,"enable_movement","",false)
		{
			UsageType = UsageTypes.Enable;
		}

		public override string Compile()
		{
			string output ="";
			if (Amount < 0)
				output += "Disables all  movements "+GetTarget();
			if (LifeTime > 0)
				output += " for "+LifeTime.ToString()+" seconds.";
			return output;
		}

	}	
	public class EnableProductionModifier:ModifierInfo
	{
		public EnableProductionModifier(string mod, string modClass):base(mod,modClass,"enable_production","production",false)
		{
			UsageType = UsageTypes.Enable;
		}
        public override string GetTarget()
        {
            return GetTarget("for");
        }
	}
   
    public class ResurrectionModifier : ModifierInfo
    {
        public ResurrectionModifier(string mod, string modClass)
            : base(mod, modClass, "health_resurrect_modifier", "Resurrect", false)
        {
            
        }
        public override string Compile()
        {
            if (UsageType == UsageTypes.Enable)                
                return "Resurrects dead units";
            return "";
        }
    }

    public class EnableCombat : ModifierInfo
    {
        public EnableCombat(string mod, string modClass)
            : base(mod, modClass, "enable_combat", "Combat", false)
        {
            UsageType = UsageTypes.Enable;
        }
        public override string GetTarget()
        {
            return GetTarget("for");
        }
    }

    public class EnableReinforcement : ModifierInfo
    {
        public EnableReinforcement(string mod, string modClass)
            : base(mod, modClass, "enable_combat", "reinforcement", false)
        {
            UsageType = UsageTypes.Enable;
        }
        public override string GetTarget()
        {
            return GetTarget("for");
        }
    }

	public class EnableSquadJumpModifier:ModifierInfo
	{
		public EnableSquadJumpModifier(string mod, string modClass):base(mod,modClass,"enable_squad_jump","Jumps",false)
		{
			UsageType = UsageTypes.Enable;
		}
        public override string GetTarget()
        {
            return GetTarget("for");
        }
	}

    public class CostModifier : ModifierInfo
    {
        public CostModifier(string mod, string modClass, string modType, string translation ):base(mod,modClass,modType,translation,false)
        {
            ApplicationType = ApplicationTypes.ApplyToEntityType;
            UsageType = UsageTypes.Multiply;
        }

        public override string Compile()
        {
            if (Parent != null)
            {
                string target = "";
                string action = "Increases ";
                bool further = false;
                string amount = "";
                bool stacks  = !DoNotStacks;

                switch (UsageType)
                {
                    case UsageTypes.Add:
                        if (Amount < 0)
                            action = "Decreases";
                        break;
                    case UsageTypes.Multiply:
                        if (Amount < 1)
                            action = "Decreases";
                        break;
                }
                               
                double mul = 1.0;

                if (Parent is SquadInfo || Parent is UnitInfo)
                {
                    SquadInfo squad = Parent is UnitInfo ? (SquadInfo)((UnitInfo)Parent).Parent : (SquadInfo)Parent;
                    bool isSquad = squad.MaxSquadSize > 1;

                    if (TargetName == squad.Unit.Name)
                        further = true;

                    if (isSquad)
                    {                         
                        target = TargetName + " squads";
                        mul = squad.StartingSquadSize;
                    }
                    else target = HtmlCompiler.GetPlural(TargetName);
                    
                }
                else if (Parent is ResearchInfo)
                {
                    target = TargetName;
                }
                else if (Parent is BuildingInfo)
                {
                    if (TargetName == ((BuildingInfo)Parent).Name)
                        further = true;
                    target = HtmlCompiler.GetPlural(TargetName);
                }
                else target = HtmlCompiler.GetPlural(TargetName);

                if (this.UsageType == UsageTypes.Add)
                    amount = ((double)Math.Abs((mul * Amount))).ToString();                    
                else
                    amount = ((double)(mul * Math.Abs((this.Amount - 1) * 100))).ToString() + "%";

                return action + " the " + this.ModifierTranslation + " of " + (further ? "further " : "") + target + " by " + amount + (ModifierTranslation == "build time" ? " seconds" : "") + (stacks ? "" : "(do not stacks)");

            }
            return base.Compile();
        }
    }
	public class WeaponModifier:ModifierInfo
	{

		public WeaponModifier(string mod, string modClass, string hardpoint):base(mod,modClass,"default_weapon_modifier_hardpoint","weapon",false)
		{
            HardPoint = System.Convert.ToInt32(Regex.Replace(hardpoint, "\b0", ""), LuaParser.NumberFormat);
            ApplicationType = ApplicationTypes.ApplyToEntityType;
            UsageType = UsageTypes.Enable;
            Amount = 1;
		}
		

		public override string Compile()
		{	
			string output ="";
			WeaponInfo oldWeapon = null;
			WeaponInfo newWeapon = null;
            BuildableInfo entity = null;


            if (ApplicationType == ApplicationTypes.ApplyToEntity)
            {
                if (this.Parent.Parent is BuildableInfo)
                    entity = ((BuildableInfo)Parent.Parent);

            }
            else entity = DataDumper.Units[Target] as BuildableInfo;
            try
            {
                if (entity != null)
                {
                    WeaponHardPointInfo whpInfo = entity.WeaponHardPoints[HardPoint] as WeaponHardPointInfo;
                    oldWeapon = (WeaponInfo)whpInfo.Weapons[whpInfo.UpgradeValue];
                    whpInfo.UpgradeValue++;
                    newWeapon = (WeaponInfo)whpInfo.Weapons[whpInfo.UpgradeValue];
                    if (!oldWeapon.IsDummyWeapon())
                    {
                        if (ApplicationType == ApplicationTypes.ApplyToEntityType)
                            output += "Upgrades " + entity.Name + "'s " + Translation.Translate(oldWeapon.Name) + " to "  + Translation.Translate(newWeapon.Name);
                        else if (ApplicationType == ApplicationTypes.ApplyToEntity)
                            output += "Upgrades " + Translation.Translate(oldWeapon.Name) + " to "+ Translation.Translate(newWeapon.Name);
                    }
                    else output += "Equips " + entity.Name + " with " + Translation.Translate(newWeapon.Name);
                        
                    
                }
                
            }
            catch {  }
			return output;	
		}
	}

    public class EnableWeaponModifier : ModifierInfo
    {
        
        public EnableWeaponModifier(string mod, string modClass, string hardpoint): base(mod, modClass, "enable_hardpoint", "weapon", false)
        {
            HardPoint = System.Convert.ToInt32(Regex.Replace(hardpoint, "\b0", ""), LuaParser.NumberFormat);
            UsageType = UsageTypes.Enable;
            Amount = 1;
        }
       

        public override string Compile()
        {
            string output = "";
            string Weapon = "";
            UnitInfo unit = null;

            if (Target != null)
                unit = DataDumper.Units[Target] as UnitInfo;
            if (ApplicationType == ApplicationTypes.ApplyToEntity)
            {
                if (Parent is UnitInfo)
                    unit = Parent as UnitInfo;
                else if (this.Parent.Parent is UnitInfo)
                    unit = this.Parent.Parent as UnitInfo;
            }
            try
            {
                if (unit != null)
                {
                    WeaponHardPointInfo whpInfo = unit.WeaponHardPoints[HardPoint] as WeaponHardPointInfo;

                    Weapon = ((WeaponInfo)whpInfo.Weapons[0]).Name;

                    output += (Amount == -1) ? "Disables " : "Enables ";

                    if (ApplicationType != ApplicationTypes.ApplyToEntity)
                        output += unit.Name + "'s ";
                    output += Translation.Translate(Weapon);

                }                
                else
                {
                    output += (Amount == -1) ? "Disables " : "Enables ";
                    output += "the main weapon.";
                }
            }
            catch { }
            return output;
        }
    }

    public class EnableChargeModifier : ModifierInfo
    {

        public EnableChargeModifier(string mod, string modClass): base(mod, modClass, "enable_charge_modifiers", "charge modifiers", false)
        {
            UsageType = UsageTypes.Enable;
        }
    }

    public class EnableLeapModifier : ModifierInfo
    {

        public EnableLeapModifier(string mod, string modClass): base(mod, modClass, "enable_leap_modifier", "Melee Leap", false)
        {
            UsageType = UsageTypes.Enable;
            ApplicationType = ApplicationTypes.ApplyToPlayer;
        }
        public override string Compile()
        {
            return "Enables Leap in melee attacks";
        }
    }

	public class HealthDegenerationModifier:ModifierInfo
	{
		public HealthDegenerationModifier(string mod, string modClass):base(mod,modClass,"helth_degeneration_modifier","hits degeneration",false)
		{
			UsageType = UsageTypes.Add;
		}
		public override string Compile()
		{
            if (Parent is SkillInfo)
                if (Parent.Parent is UnitInfo || Parent.Parent is SquadInfo)
                    return "The "+((BuildableInfo)Parent.Parent).Name + " loses " + Amount + " hit points every seconds";
            
            return base.Compile();
		}

	}
	public class PopulationCapModifier:ModifierInfo
	{
		public PopulationCapModifier(string mod, string modClass):base(mod,modClass,"population_cap_player_modifier","population",false)
		{
			UsageType = UsageTypes.Add;
			ApplicationType = ApplicationTypes.ApplyToPlayer;
		}
		public override string GetModTranslation()
		{
			if (Parent !=null)
			{
				if (this.Parent.Race == "guard") 
					return "FSR";
				else if (this.Parent.Race == "ork") 
					return "Ork Resource";
				else if (Parent.Parent != null)
					if (this.Parent.Parent.Race == "guard") 
						return "FSR";
					else if (this.Parent.Parent.Race == "ork") 
						return "Ork Resource";
			}
			
			return "Population";

		}
		
	}
	public class PopulationGrowRateModifier:ModifierInfo
	{
		public PopulationGrowRateModifier(string mod, string modClass):base(mod,modClass,"population_growth_rate_player_modifier","",true)
		{
			UsageType = UsageTypes.Add;
			ApplicationType = ApplicationTypes.ApplyToPlayer;
		}
		public override string GetModTranslation()
		{
			if (Parent !=null)
			{
				if (this.Parent.Race == "guard") 
					return "FSR grow rate";
				else if (this.Parent.Race == "ork") 
					return "Ork Resource rate";
				else if (Parent.Parent != null)
					if (this.Parent.Parent.Race == "guard") 
						return "FSR grow rate";
					else if (this.Parent.Parent.Race == "ork") 
						return "Ork Resource rate";
			}
			
			return "Population grow rate";

		}
		
	}
	public class EnableAbilitiesModifier:ModifierInfo
	{
		public EnableAbilitiesModifier(string mod, string modClass):base(mod,modClass,"enable_abilities","",false)
		{
			UsageType = UsageTypes.Enable;
		}

		public override string Compile()
		{
			string output ="";
			
			if (Amount < 0)
				output += "Disables all abilities "+GetTarget();
			
			if (LifeTime > 0)
				output += " for "+LifeTime.ToString()+" seconds.";
			
			return output;
		}

	}
	public class EnableMoraleDamageModifier:ModifierInfo
	{
		public EnableMoraleDamageModifier(string mod, string modClass):base(mod,modClass,"enable__squad_morale_damage","",false)
		{
			UsageType = UsageTypes.Enable;
		}

		public override string Compile()
		{
			string output ="";
			if (Parent is SkillInfo)
			{
				if (Amount > 0)
					output += "Disables morale immunity "+ Regex.Replace(GetTarget(),"of","for");
				else output += "Enables morale immunity "+ Regex.Replace(GetTarget(),"of","for");
			
				if (LifeTime > 0)
					output += " for "+LifeTime.ToString()+" seconds.";

				output+="<li>Restores morale "+ Regex.Replace(GetTarget(),"of","for")+"</li>";
			}
			return output;
		}

	}	
	public class EnableInfiltrationModifier:ModifierInfo
	{
		public EnableInfiltrationModifier(string mod, string modClass):base(mod,modClass,"enable_infiltration","",false)
		{
			UsageType = UsageTypes.Enable;
            Amount = 1;
		}

		public override string Compile()
		{
			string output ="";
			if (Parent is SkillInfo)
			{
				if (Amount > 0)
					output += "Enables infiltration "+GetTarget();
				else output += "Disables infiltration "+GetTarget();
			
				if (LifeTime > 0)
					output += " for "+LifeTime.ToString()+" seconds.";
			}
			return output;
		}

	}

    public class EnableArmorModifier : ModifierInfo
    {
        public EnableArmorModifier(string mod, string modClass):base(mod,modClass,"enable_armor","",false)
		{
			UsageType = UsageTypes.Enable;
            ApplicationType = ApplicationTypes.ApplyToEntity;
		}

		public override string Compile()
		{
            
            string output ="";
                        
            if (this.Parent.Parent is BuildableInfo)
			{
                
                BuildableInfo Bld = Parent.Parent as BuildableInfo;
                if (Bld.ArmorType > ArmorTypes.unknown && Bld.ArmorType < ArmorTypes.tp_building_high)
                { 
                    if (ApplicationType == ApplicationTypes.ApplyToEntityType)
                        output = "Changes the armor of the " + Bld.Name + "to the secondary armor type";
                    else if (ApplicationType == ApplicationTypes.ApplyToEntity)
                        output = "Changes the armor to the secondary armor type";
                }
                
            }
            
			return output;
		}
    }

    public class WeaponDamageModifier : ModifierInfo
    {
        public WeaponDamageModifier(string mod, string modClass, string modType, string translation ):base(mod,modClass,modType,translation,false)
        {            
        }

        public override string TargetName
        {
            get { return Translation.TransOwner(Target); }
        }

        public override string Compile()
        {
            string output="";
            string action = "";
            string amount = ""; 

            if (UsageType == UsageTypes.Add)
            {
                if (Amount >= 0)
                    action = "Increases the damage of";
                else
                    action = "Decreases the damage of";
                amount = Amount.ToString();
            }
            else
            {
                if (Amount >= 1)
                    action = "Increases the damage of";
                else
                    action = "Decreases the damage of";
                amount = Math.Abs(1 - Amount) * 100 + "%";
            }

            output = action + " " + TargetName + " by " + amount;
            return base.Compile();
        }

    }
}