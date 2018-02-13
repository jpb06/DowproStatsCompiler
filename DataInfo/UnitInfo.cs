using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace StatsCompiler
{
	public class UnitInfo : BuildableInfo
	{
        private int m_HitPoints;
        private double m_Sight;
        private double m_KeenSight;
        private double m_Mass;
        private double m_Speed;
        private double m_BuildTime;
        private double m_HitsRegen;
        private double m_MoraleRegen;
        private double m_Morale;
        private double m_ChargeRange;
        private double m_ResurrectChance;
        private double m_ResurrectHps;
        private bool m_CanFly;
               
        public int HitPoints
        {
            get {return m_HitPoints ;}
            set {m_HitPoints=value ;}
        }

        public double RessurectChance
        {
            get { return m_ResurrectChance; }
            set { m_ResurrectChance = value; }
        }
        public double ResurrectHps
        {
            get { return m_ResurrectHps; }
            set { m_ResurrectHps = value; }
        }
		public double Sight
        {
            get {return m_Sight ;}
            set {m_Sight=value ;}
        }
		public double KeenSight
        {
            get {return m_KeenSight ;}
            set {m_KeenSight=value ;}
        }
		public double Mass
        {
            get {return m_Mass ;}
            set {m_Mass=value ;}
        }
		public double Speed
        {
            get {return m_Speed ;}
            set {m_Speed=value ;}
        }
		public double BuildTime
        {
            get {return m_BuildTime ;}
            set {m_BuildTime=value ;}
        }
		public double HitsRegen
        {
            get {return m_HitsRegen ;}
            set {m_HitsRegen=value ;}
        }
		public double MoraleRegen
        {
            get {return m_MoraleRegen ;}
            set {m_MoraleRegen = value ;}
        }
		public double Morale
        {
            get {return m_Morale;}
            set {m_Morale = value ;}
        }
		public double ChargeRange
        {
            get {return m_ChargeRange;}
            set {m_ChargeRange = value ;}
        }

        public bool CanFly
        {
            get { return m_CanFly; }
            set { m_CanFly = value; }
        }

        public int MobValue;

        public virtual bool IsIndipendant()
        {
            if (Parent != null && Parent is SquadInfo)
                return ((SquadInfo)Parent).IsIndipendant();            
            return true;
        }

        public Hashtable InCombatModifiers;


		public UnitInfo():base()
		{
            InCombatModifiers = new Hashtable();
		}
	}		
}