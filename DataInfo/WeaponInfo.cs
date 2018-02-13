using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace StatsCompiler
{
    public class WeaponHardPointInfo
    {
        public ArrayList Weapons;
        public int UpgradeValue;
        public WeaponHardPointInfo()
        {
            Weapons = new ArrayList();
        }
    }

    public enum ArmorTypes
    {
        unknown=0,
        tp_builder,
        tp_infantry_low,
        tp_infantry_med,
        tp_infantry_high,
        tp_infantry_heavy_low,
        tp_infantry_heavy_med,
        tp_infantry_heavy_high,
        tp_monster_low,
        tp_monster_med,
        tp_monster_high,
        tp_vehicle_low,
        tp_vehicle_med,
        tp_vehicle_high,
        tp_air_low,
        tp_air_med,
        tp_air_high,
        tp_commander,
        tp_building_low,
        tp_building_med,
        tp_building_high     
    }

    public enum WeaponUpgradeTypes
    {
        New,
        Upgrade,
        Replacement
    }

    public class ArmorPiercing
    {
        public int Entry;
        public ArmorTypes ArmorType;
        public double PiercingValue;
        public ArmorPiercing()
        {
        }
    }

	public class WeaponInfo : BuildableInfo
	{
		public double Accuracy;
		public double AccuracyReduction;
		public double MaxDamage;
		public double MinDamage;
		public double MinDamageValue;
		public double MoraleDamage;
		public double MinRange;
		public double MaxRange;
		public double ReloadTime;
		public double SetupTime;
		public int BuildTime;
		public int WeaponIndex;
		public int HardPoint;
		public double AOERadius;
        public double MinForce;
        public double MaxForce;
        public double BasePiercing;

        public bool CanHitAir;
        public bool CanHitGround;
		
        public Hashtable ArmorPiercingValues;
        
        public bool ShowCost;
		
		public WeaponInfo():base()
		{
            ArmorPiercingValues = new Hashtable();
            ShowCost = true;
            CanHitGround = true;
            CanHitAir = false;
		}

		public override int CompareTo(object o)
		{
			if (! (o is WeaponInfo))
				return -1;
			
			WeaponInfo toCompare = o as WeaponInfo;
			
			int compIndex = WeaponIndex.CompareTo(toCompare.WeaponIndex);
			int compRange = MaxRange.CompareTo(toCompare.MaxRange);
			if (compIndex == 0)
				if (compRange == 0)
					return Name.CompareTo(toCompare.Name);
				else return compRange;	
			else return compIndex;
		}

        public bool IsDummyWeapon()
        {
            return this.Name.Contains("dummy");
        }

        public bool IsValid()
        {
            if (!IsDummyWeapon())
            {
                if (ReloadTime == 0)
                    return false;
                
                if (ArmorPiercingValues.Count == 0)
                    return false;
                foreach (ArmorPiercing ap in this.ArmorPiercingValues.Values)
                {                    
                    if (!Double.IsNaN(ap.PiercingValue) && ap.PiercingValue != 0)
                        return true;
                }
                if ( !Double.IsNaN(MoraleDamage) && MoraleDamage != 0)
                    return true;
                return false;
            }
            return true;
        }

        public static WeaponInfo GetWeapon(BuildableInfo info, int hardpoint, int index)
        {
            WeaponHardPointInfo whp = null;
            
            whp = info.WeaponHardPoints[hardpoint] as WeaponHardPointInfo;
            if (whp == null)
                return null;
            foreach (WeaponInfo wpInfo in whp.Weapons)
            {
                if (wpInfo.WeaponIndex == index)
                    return wpInfo;
            }
            return null;

        }
	}
}