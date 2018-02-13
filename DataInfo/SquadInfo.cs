using System;
using System.Collections;
using System.Text;

namespace StatsCompiler
{
    public class SquadInfo : BuildableInfo
    {
        private bool m_CanBeAttached;
        public int ReinforceTime;
        public double InCombatTimeMultiplier;
        public int ReinforcePop;
        public int SquadCapUsage;
        public int SupportCapUsage;
        public int HardCap;
        public int StartingSquadSize;
        public int MaxSquadSize;
        public int MaxWeapons;
        public int MaxLeaders;
        public int Morale;
        public double MoraleRegen;
        public UnitInfo Unit;
        public ArrayList Leaders;

        public InfiltrationInfo Infiltration;
        public JumpInfo Jumps;

        public bool CanBeAttached
        {
            get { return m_CanBeAttached; }
            set { m_CanBeAttached = value; }
        }

        public bool IsIndipendant()
        {
            if (CanBeAttached || MaxSquadSize > 1)
                return false;
            return true;
        }

        public SquadInfo(): base()
        {
            Infiltration = new InfiltrationInfo();
            Jumps = new JumpInfo();
        }

        public override int CompareTo(object o)
        {
            if (!(o is SquadInfo))
                return -1;

            SquadInfo toCompare = o as SquadInfo;

            if (Race.CompareTo(toCompare.Race) < 0)
                return -1;
            if (Race.CompareTo(toCompare.Race) > 0)
                return 1;
            if (Unit == null)
                return -1;
            else if (toCompare.Unit == null)
                return 1;
            else
            {
                int comp = Unit.ArmorType.CompareTo(toCompare.Unit.ArmorType);
                if (comp != 0)
                    return comp;
            }
            return Name.CompareTo(toCompare.Name);
        }
    }
}
