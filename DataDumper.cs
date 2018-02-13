using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;

namespace StatsCompiler
{	
    public enum InfoTypes
	{
		Unit,
		Building,
		Skill,
		Research,
		Addon,
		None
	}
	
	public class LuaInfo:IComparable
	{
		public string Path;
		public string Race;
		public string Name;
		public InfoTypes Type;
		
		public LuaInfo(string path,InfoTypes type, string race)
		{
			Path=path;
			Type = type;
			
			Race = race;
		}
		
		public int CompareTypes(InfoTypes type)
		{
			if (Type < type)
				return -1;
			if (Type == type)
				return 0;
			return 1;
		}

		public int CompareTo(object o)
		{
			LuaInfo toCompare = o as LuaInfo;
			if (Race.CompareTo(toCompare.Race) < 0)
				return -1;
			if (Race.CompareTo(toCompare.Race) > 0)
				return 1;
			int result = (CompareTypes(toCompare.Type));
			
			if (result==0)
				return Path.CompareTo(toCompare.Path);
			
			return result;
		}
	}
	
	/// <summary>
	/// This Class contains function to extract data from .lua files
	/// </summary>
	    
    public class DataDumper
	{
		public static Hashtable FilesTable;
		
		public static ArrayList Researches	= new ArrayList();
		public static ArrayList Addons		= new ArrayList();
		public static ArrayList Skills		= new ArrayList();
		public static ArrayList Squads		= new ArrayList();
		public static ArrayList Buildings	= new ArrayList();

        public static ArrayList UnnecessaryAbilities = new ArrayList();
        public static Hashtable RepeatedAbilities = new Hashtable();

		public static Hashtable Units = new Hashtable();

        
		// Writes the files list
		public static void WriteFilesList()
		{
			// Writes the filesList
			ArrayList list = new ArrayList(FilesTable.Keys);
			list.Sort();
			StreamWriter writer = new StreamWriter(File.Create(Path.Combine(Directory.GetCurrentDirectory(),"Config/FilesIndex.def")));
			foreach (string k in list)
			{
                LuaInfo lInfo = FilesTable[k] as LuaInfo;
                if (lInfo.Type!= InfoTypes.Research)
                    writer.WriteLine("<" + lInfo.Type.ToString() + "> <" + lInfo.Race + "> <" + k + ">");
			}
			writer.Close();
		}
        
        public static void LoadAbilitiesConfig()
        {                                                                   
            string file = Path.Combine(Directory.GetCurrentDirectory(), "Config/UnnecessaryAbilities.def");
            if (File.Exists(file))
            {
                StreamReader reader = new StreamReader(File.OpenRead(file));
                while (reader.Peek() > -1)
                {
                    string line = reader.ReadLine();
                    UnnecessaryAbilities.Add(line);
                }
            }
            file = Path.Combine(Directory.GetCurrentDirectory(), "Config/RepeatedAbilities.def");
            if (File.Exists(file))
            {
                StreamReader reader = new StreamReader(File.OpenRead(file));

                while (reader.Peek() > -1)
                {
                    string s = reader.ReadLine();
                    Match m = Regex.Match(s, @"(?<key>.*)\s-\s""(?<description>.*)""\s-\s""(?<amount>.*)""");
                    if (m.Success)
                    {
                        string key = m.Groups["key"].Value;
                        string description = m.Groups["description"].Value;
                        string amount = m.Groups["amount"].Value;
                        
                        if (!RepeatedAbilities.ContainsKey(key))
                            RepeatedAbilities.Add(key, new string[] { description, amount });
                    }
                }
                reader.Close();
            }
        }
		
        // Load File Index from previous saved index
		public static bool LoadFilesList()
		{
            FilesTable = new Hashtable();
           
            string file= Path.Combine(Directory.GetCurrentDirectory(),"Config/FilesIndex.def");
			if (File.Exists( file ) )
			{
				
				StreamReader reader = new StreamReader(File.OpenRead(file));
				while (reader.Peek() > -1)
				{
					string line = reader.ReadLine();

                    Match m = Regex.Match(line, @"<(?<type>.*)\>\s\<(?<race>.*)\>\s\<(?<file>.*)\>");
                    if (m.Success)
                    {
                        Group typeGr = m.Groups["type"];
                        Group raceGr = m.Groups["race"];
                        Group fileGr = m.Groups["file"];
   
                        string lua = fileGr.Value;
                        
                        InfoTypes infoType = GetInfoType(typeGr.Value);
                        if (!FilesTable.Contains(lua))
                        {
                            try
                            {
                                string path = DataPath.GetPath(DataPath.GetCategoryPath(lua, infoType, raceGr.Value));
                                FilesTable.Add(/*Regex.Replace(file, @"^.*\\", "")*/lua, new LuaInfo(path, infoType, raceGr.Value));                                
                            }
                            catch (Exception e)
                            {
                            }
                            
                        }
                    }
                    
				}
				reader.Close();
				return true;
			}
			return false;
            
		}

		public static void Dump()
		{
            // Clear Resources            
            Addons.Clear();
            Skills.Clear();
            Squads.Clear();
            Buildings.Clear();
            Units.Clear();
            Researches.Clear();
            MainForm.Log("Lua processing started...");
            DateTime start = DateTime.Now;
            

            Hashtable temp = (Hashtable)FilesTable.Clone();

            foreach (string lua in temp.Keys)
            {
                LuaInfo lInfo = temp[lua] as LuaInfo;
                if (lInfo.Type == InfoTypes.Building)
                {
                    StreamReader file = new StreamReader(File.OpenRead(lInfo.Path), System.Text.Encoding.ASCII);
                    file.BaseStream.Seek(0, SeekOrigin.Begin);

                    string s = file.ReadToEnd();

                    file.Close();

                    MatchCollection mccR = Regex.Matches(s, @"GameData\[""research_ext""\]\[""research_table""\]\[""research_([0-9]?[0-9]?)""\]\s=\s""(.*\\\\)?((?<research>.*)\.lua|(?<research>.*))""");
                    if (mccR.Count > 0)
                    {                        
                        foreach (Match mt in mccR)
                        {
                            Group research = mt.Groups["research"];

                            if (research.Success && research.Value!="")
                            {
                                string res = research.Value;
                                if (!res.EndsWith(".lua"))
                                    res += ".lua";
                                res = Path.Combine("research", res);
                                string path = DataPath.GetPath(res);
                               
                                string race = GetRace(lInfo.Path);
                                LuaInfo resLua = new LuaInfo(path, InfoTypes.Research, race);
                                
                                if (!FilesTable.Contains(res))
                                    FilesTable.Add(res, resLua);
                            }
                        }

                    }
                }
            }
            int count = FilesTable.Count;
            MainForm.BarSetMax(Bars.Data,count);
            // Collecting resources from lua files.
            foreach (string lua in FilesTable.Keys)
            {
                LuaInfo lInfo = FilesTable[lua] as LuaInfo;

                switch (lInfo.Type)
                {
                    case InfoTypes.Research:
                        ResearchInfo rInfo = new ResearchInfo();                        
                        rInfo.Race = lInfo.Race;
                        rInfo.FileName = Regex.Replace(lua, @"\.lua|\.nil|(.*)\\", "");
                        
                        try
                        {                          
                            LuaParser.ParseResearch(lInfo.Path, rInfo);                        
                        }
                        catch (Exception e)
                        {
                        }
                        
                        MainForm.BarInc(Bars.Data);
                        Researches.Add(rInfo);
                        break;
                    case InfoTypes.Unit:
                        
                        SquadInfo sInfo = new SquadInfo();                        
                        sInfo.Race = lInfo.Race;
                        sInfo.FileName = Regex.Replace(lua, @"\.lua|\.nil", "");
                        LuaParser.ParseSquad(lInfo.Path, sInfo);
                        MainForm.BarInc(Bars.Data);
                        if (sInfo.Unit!=null)
                            Squads.Add(sInfo);
                        break;
                    case InfoTypes.Building:
                        BuildingInfo bInfo = new BuildingInfo();
                        bInfo.Race = lInfo.Race;
                        bInfo.FileName = Regex.Replace(lua, @"\.lua|\.nil", "");
                        LuaParser.ParseBuilding(lInfo.Path, bInfo);
                        MainForm.BarInc(Bars.Data);
                        Buildings.Add(bInfo);
                        break;
                }

            }
			Researches.Sort();
			Squads.Sort();
			Buildings.Sort();

            TimeSpan elapsed = DateTime.Now - start;
            MainForm.Log("Lua processing done in " + Math.Round(elapsed.TotalSeconds, 2) + " seconds.");
            
            
    }

		public static InfoTypes GetInfoType(string input)
		{
			switch (input)
            {
                case "Addon":
                    return InfoTypes.Addon;            
                case "Unit":   
                case "Squads":
                    return InfoTypes.Unit;        
                case "Research":   
                case "Researches":
                    return InfoTypes.Research;          
                case "Building":
                case "Buildings":
                    return InfoTypes.Building;               
            }
           /* if (input == "Addon")
				return InfoTypes.Addon;
			if (input == "Unit")
				return InfoTypes.Unit;
			else if (input == "Research")
				return InfoTypes.Research;
			if (input == "Skill")
				return InfoTypes.Skill;
			if (input == "Building")
				return InfoTypes.Building;
            */ 
			return InfoTypes.Unit;
		}
		
		// Creates Files Index from Scratch
		public static void CreateFilesList()
		{
			// Contains all SubPaths that program must scan
			string[] SquadsToLoad= new string[]
				{
					@"\sbps\races\chaos",
					@"\sbps\races\eldar",
					@"\sbps\races\guard",
					@"\sbps\races\necrons",
					@"\sbps\races\orks",
					@"\sbps\races\space_marines",
					@"\sbps\races\tau",
                    // Soulstorm Squads
                    @"\sbps\races\dark_eldar",
                    @"\sbps\races\sisters"
				};
			string[] BuildingsToLoad= new string[]
				{
					@"\ebps\races\chaos\structures",
					@"\ebps\races\eldar\structures",
					@"\ebps\races\guard\structures",
					@"\ebps\races\necrons\structures",
					@"\ebps\races\orks\structures",
					@"\ebps\races\space_marines\structures",
					@"\ebps\races\tau\structures",
                    // Soulstorm Structures
                    @"\ebps\races\dark_eldar\structures",
                    @"\ebps\races\sisters\structures"
				};
			
			string[] Races=new string[]
			{
				"chaos",
				"eldar",
				"guard",
				"necrons",
				"orks",
				"space_marines",
				"tau",
                // Soulstorm races
                "dark_eldar",
                "sisters"
			};

			string W40KPath		=	DataPath.OriginalLuasPath + "\\W40K\\Attrib";
			string DXP2Path		=	DataPath.OriginalLuasPath + "\\DXP2\\Attrib";
			string ModPath		=	DataPath.ModLuasPath + "\\Attrib";

			
			FilesTable = new Hashtable();
			
			// Load Researches
			if (Directory.Exists(W40KPath+@"\research"))
				LoadFiles( W40KPath+@"\research","Research" );
			if (Directory.Exists(DXP2Path+@"\research"))
				LoadFiles( DXP2Path+@"\research","Research"  );
			if (Directory.Exists(ModPath+@"\research"))
				LoadFiles( ModPath+@"\research","Research" );

            int racesCount = 9;
            

            // Load Squads
			for(int i=0; i<racesCount; i++)
			{
				if (Directory.Exists(W40KPath+SquadsToLoad[i]))
					LoadFiles( W40KPath+SquadsToLoad[i],"Unit",Races[i] );
				if (Directory.Exists(DXP2Path+SquadsToLoad[i]))
					LoadFiles( DXP2Path+SquadsToLoad[i],"Unit",Races[i] );
				if (Directory.Exists(ModPath+SquadsToLoad[i]))
					LoadFiles( ModPath+SquadsToLoad[i],"Unit",Races[i]);	
			}
			// Load Buildings
			for (int i=0; i<racesCount; i++)
			{
				if (Directory.Exists(W40KPath+SquadsToLoad[i]))
					LoadFiles( W40KPath+BuildingsToLoad[i],"Building",Races[i] );
				if (Directory.Exists(DXP2Path+SquadsToLoad[i]))
					LoadFiles( DXP2Path+BuildingsToLoad[i],"Building",Races[i] );
				if (Directory.Exists(ModPath+SquadsToLoad[i]))
					LoadFiles( ModPath+BuildingsToLoad[i],"Building",Races[i]);
			}
			
		}

        public static string GetRaceFromLua(string luaName)
        {
            if (luaName == "")
                return "none";
            string lua="";
            try
            {
                
                StreamReader file = new StreamReader(File.OpenRead(DataPath.GetPath(Path.Combine("research",luaName))), System.Text.Encoding.ASCII);
                file.BaseStream.Seek(0, SeekOrigin.Begin);

                lua = file.ReadToEnd();

                file.Close();
            }
            catch (Exception e)
            { 
                MainForm.Log("Unable to open file: " + luaName+ " Exception: "+e.Message); 
                return "none"; 
            }

            Match m = Regex.Match(lua, @"GameData\s=\sInherit\(\[\[(?<parent>.*)\]\]\)");

            if (!m.Success) 
                return "none";              

            string parent = m.Groups["parent"].Value; 

            Match mc = Regex.Match(parent, @"research\\(?<race>.*)_research\.nil");
            if (mc.Success)
            {
                return GetRace(mc.Groups["race"].Value);
            }
            try
            {
                return GetRaceFromLua(DataPath.GetPath(parent));
            }
            catch (Exception e)
            {
                return "none";
            }
        }

		public static string GetRace(string luaName)
		{
			if (Regex.Match(luaName,"chaos").Success)
				return "chaos";
			if (Regex.Match(luaName,"eldar").Success)
				return "eldar";
			if (Regex.Match(luaName,"guard").Success)
				return "guard";
			if (Regex.Match(luaName,"marine").Success)
				return "space_marines";
			if (Regex.Match(luaName,"necron").Success)
				return "necrons";
			if (Regex.Match(luaName,"ork").Success)
				return "orks";
			if (Regex.Match(luaName,"tau").Success)
				return "tau";
            if (Regex.Match(luaName, "sister").Success)
                return "sisters";
            if (Regex.Match(luaName, "dark").Success)
                return "dark_eldar";
            string output = GetRaceFromLua(luaName);
            return output;
		}
		public static void LoadFiles(string folder, string type)
		{
			LoadFiles(folder,type,null);
		}
		
		public static void LoadFiles(string folder,string type, string race)
		{
			
			string[] files	 = Directory.GetFiles(folder);
			foreach (string f in files)
			{		
				string Race = race;
				if (f.EndsWith(".lua") || f.EndsWith(".nil"))
				{
					string fileName = f.Remove(0,folder.Length+1);																		
					Match m = Regex.Match(fileName,@"(single_player_only)|(sp_)|(_sp\.)|(_sp_)|(_nis\.)|(exarch_council)|(.nil)");

					if (Race == null || Race == "")
						Race=GetRace(fileName);
					if (FilesTable.ContainsKey(fileName))
						FilesTable.Remove(fileName);
					FilesTable.Add(fileName,new LuaInfo(f,GetInfoType(type),Race));
				}
			}
			
		}
		
	}
}