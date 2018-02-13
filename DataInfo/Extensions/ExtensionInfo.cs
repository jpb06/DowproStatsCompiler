using System;
using System.Collections;
using System.Text;

namespace StatsCompiler
{
    public class ExtensionInfo : BaseInfo
    {
        private string m_Icon;

        public string Icon
        {
            get { return m_Icon; }
            set { m_Icon = value; }
        }

        private string m_Name;

        public string Name  
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public ArrayList Filter;
        public string AOEFilter;

        public ExtensionInfo()
        {
            Filter = new ArrayList();
        }

        public virtual void Compile(ref int index, ref string html)
        {
        }
    }

}
