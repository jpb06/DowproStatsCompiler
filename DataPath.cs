using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace StatsCompiler
{
	public class DataPath
	{
		private static string m_OriginalLuasPath="";
		private static string m_ModLuasPath="";
		private static string m_TemplatePath="";
		private static string m_OutputPath="";
        private static string m_W40kUcsPath = "";
        private static string m_Dxp2UcsPath = "";
        private static string m_ModUcsPath = "";

		
		public static string OriginalLuasPath
		{
			get{return m_OriginalLuasPath;}
			set{m_OriginalLuasPath=value;}
		}
		
		public static string TemplatePath
		{
			get{return m_TemplatePath;}
			set{m_TemplatePath=value;}
		}
		public static string ModLuasPath
		{
			get{return m_ModLuasPath;}
			set{m_ModLuasPath=value;}
		}

		public static string OutputPath
		{
			get{return m_OutputPath;}
			set{m_OutputPath=value;}
		}
        public static string W40kUcsPath
        {
            get { return m_W40kUcsPath; }
            set { m_W40kUcsPath = value; }
        }
        public static string Dxp2UcsPath
        {
            get { return m_Dxp2UcsPath; }
            set { m_Dxp2UcsPath = value; }
        }
        public static string ModUcsPath
        {
            get { return m_ModUcsPath; }
            set { m_ModUcsPath = value; }
        }
		
		public static void LoadPaths()
		{
			try
			{
                StreamReader reader = new StreamReader(File.OpenRead("Config/StatsDump.Ini"), System.Text.Encoding.ASCII);
                string content = reader.ReadToEnd();
                
                Match m = Regex.Match(content, @"OriginalLuaPath\s*=\s*""(?<OriginalLuaPath>.*)""");
                if (m.Success)
                    m_OriginalLuasPath=m.Groups["OriginalLuaPath"].Value;
                                         
                m = Regex.Match(content, @"ModLuaPath\s*=\s*""(?<ModLuaPath>.*)""");
                if (m.Success)
                    m_ModLuasPath = m.Groups["ModLuaPath"].Value;
                
                m = Regex.Match(content, @"OutputPath\s*=\s*""(?<OutputPath>.*)""");
                if (m.Success)
                    m_OutputPath = m.Groups["OutputPath"].Value;

                m = Regex.Match(content, @"W40kUcsPath\s*=\s*""(?<W40kUcsPath>.*)""");
                if (m.Success)
                    m_W40kUcsPath = m.Groups["W40kUcsPath"].Value;

                m = Regex.Match(content, @"Dxp2UcsPath\s*=\s*""(?<Dxp2UcsPath>.*)""");
                if (m.Success)
                    m_Dxp2UcsPath = m.Groups["Dxp2UcsPath"].Value;

                m = Regex.Match(content, @"ModUcsPath\s*=\s*""(?<ModUcsPath>.*)""");
                if (m.Success)
                    m_ModUcsPath = m.Groups["ModUcsPath"].Value;

                reader.Close();

                m_TemplatePath = Application.StartupPath + @"\\Template";
			}
			catch{};
		}

        public static string GetCategoryPath(string lua, InfoTypes infoType, string race)
        {
            string prefix = "";
            switch (infoType)
            {
                case InfoTypes.Unit:
                    prefix = "sbps\\races\\" + race+"\\";
                    break;
                case InfoTypes.Building:
                    prefix = "ebps\\races\\" + race + "\\structures\\";
                    break;
                case InfoTypes.Research:
                    prefix = "research\\";
                    break;
            }
            
            return Path.Combine(prefix,lua);
        }

        public static string GetPath(string lua)
        {  
            string W40KPath = DataPath.OriginalLuasPath + @"\W40K\attrib\";
            string DXP2Path = DataPath.OriginalLuasPath + @"\DXP2\attrib\";
            string ModPath = DataPath.ModLuasPath + @"\attrib\";

            if (File.Exists(Path.Combine(ModPath, lua)))   
                return Path.Combine(ModPath, lua);
            else if (File.Exists(Path.Combine(DXP2Path, lua)))
                return Path.Combine(DXP2Path, lua);
            else if (File.Exists(Path.Combine(W40KPath, lua)))
                return Path.Combine(W40KPath, lua);

            return "";
  
        }

    }
}