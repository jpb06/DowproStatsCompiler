using System;
using System.Collections;
using System.Text;

namespace StatsCompiler
{
    
    public class BaseInfo
    {
        private string m_Race;

        public string Race
        {
            get { return m_Race; }
            set { m_Race = value; }
        }
        
        private BaseInfo m_Parent;
        public BaseInfo Parent
        {
            get { return m_Parent; }
            set { m_Parent = value; }
        }
        public BaseInfo RootParent
        {
            get
            {
                if (m_Parent != null)
                    return m_Parent.RootParent;
                else return this;
            }
        }
        
        public Hashtable Requirements;      // Contains all requirements for this object
        public Hashtable Modifiers;         // Contains All modifiers for this object
        public Hashtable ToolTipsInfo;      // Contains the tooltips for this object 		

        public BaseInfo()
        {
            Requirements = new Hashtable();
            Modifiers = new Hashtable();
            ToolTipsInfo = new Hashtable();
        }
    }
}
