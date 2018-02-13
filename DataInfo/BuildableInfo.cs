using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace StatsCompiler
{
    public class BuildableInfo:BaseInfo,IComparable
	{
		public string Name;
        public string LuaName;
		public string FileName;
		
		public int PowerCost;
		public int RequisitionCost;
		public int PopCost;                 
		public int UI;
		//public bool Allowed;                // Specify if this info should be compiled or not
        public Hashtable Skills;            // Contains a list of the skills available for this object
        public Hashtable Extensions;        // Contains a list of the Extensions skill available for this object
        private string m_Icon;
        public string Icon   // The path(relative) of the icon for this resource
        {
            get 
            {
                if (m_Icon != null)
                {
                    Match m = Regex.Match(m_Icon, @"_icons/$");
                    if (m.Success)
                        return null;
                    m = Regex.Match(m_Icon, @"(?<race>.*)_icons/(?<icon>.+)");
                    if (m.Success) 
                        return m_Icon;    
                    else
                    {
                        m = Regex.Match(m_Icon, @"(?!.*_icons/)(?<icon>.+)");
                        if (m.Success && this.RootParent is BuildableInfo)    
                            return ((BuildableInfo)RootParent).Race + "_icons/" + m.Groups["icon"].Value;
                    }
                }
                return "";
            }
            set 
            {
                if (value != null)
                    m_Icon = Regex.Replace(value, @"\\\\", "/");
                else m_Icon = "";
            }
        }
        public int TransportSlots;          
        public Hashtable WeaponHardPoints;
        private ArmorTypes m_ArmorType;
        public ArmorTypes ArmorType
        {
            get { return m_ArmorType; }
            set { m_ArmorType = value; }
        }
		public BuildableInfo()
		{
			Name="";

			WeaponHardPoints = new Hashtable();
			Skills = new Hashtable();
            Extensions = new Hashtable();
		}

		public virtual int CompareTo(object o)
		{
			if (! (o is BuildableInfo))
				return -1;
            try
            {
                BuildableInfo toCompare = o as BuildableInfo;

                if (Race.CompareTo(toCompare.Race) < 0)
                    return -1;
                if (Race.CompareTo(toCompare.Race) > 0)
                    return 1;
                return Name.CompareTo(toCompare.Name);
            }
            catch {  };
            return -1;
		}
	}
}