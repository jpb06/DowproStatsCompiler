using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace StatsCompiler
{
	public class Translation 
	{
		public static Hashtable TranslationTable=new Hashtable();
		public static Hashtable W40kUcs=new Hashtable();
		public static Hashtable Dxp2Ucs=new Hashtable();
		public static Hashtable ModUcs=new Hashtable();

		public static void LoadTranslationTable()
		{
			
			StreamReader reader = new StreamReader("Config/TranslationTable.def");
			
			while(reader.Peek() > -1)
			{
				string s = reader.ReadLine();
				Match m = Regex.Match(s,@"""(?<key>.*)""\s*=\s*""(?<translation>.*)""(\s\[(?<owner>.*)\])?");
                if (m.Success)
                {
                    string key = m.Groups["key"].Value;
                    string translation = m.Groups["translation"].Value;
                    string owner = "";
                    if (m.Groups["owner"].Success)
                        owner = m.Groups["owner"].Value;
                    if (!TranslationTable.Contains(key))
                        TranslationTable.Add(key, new string[] { translation, owner });
                }
			}
			reader.Close();
		}
		public static void WriteTable()
		{
			StreamWriter writer = new StreamWriter("Config/TranslationTable.def");
			ArrayList list = new ArrayList(TranslationTable.Keys);
			list.Sort();
            foreach (string key in list)
            {
                string[] trans = (string[])TranslationTable[key];
                string output = @"""" + key + @""" = """ + trans[0] + @"""";
                if (trans[1] != "")
                    output += " [" + trans[1] + "]"; 
                writer.WriteLine(output);
            }
			writer.Close();            
		}
		public static void LoadUcs(string path, Hashtable table)
		{
			StreamReader reader = new StreamReader(path);
			
			while(reader.Peek() > -1)
			{
				string s = reader.ReadLine();
				Match m = Regex.Match(s,@"([0-9]+)\s+(.*)");
				if (m.Success)
				{
					Group index = m.Groups[1];
					Group str = m.Groups[2];
                    int tIndex = System.Convert.ToInt32(index.Value, LuaParser.NumberFormat);
					table.Add(tIndex,str.Value);
				}
			}
			reader.Close();
		}

		public static void Initialize()
		{
            try
            {
                LoadTranslationTable();
                LoadUcs(DataPath.W40kUcsPath, W40kUcs);
                LoadUcs(DataPath.Dxp2UcsPath, Dxp2Ucs);
                LoadUcs(DataPath.ModUcsPath, ModUcs);
            }
            catch { };
		}
		public static string Translate(int index, string lua)
		{
			string res = Translate(index);
			string toTranslate = Regex.Replace(lua,@"(\.lua|.\nil)","");
			if (res != "")
			{
				if (!TranslationTable.Contains(toTranslate))
                    TranslationTable.Add(toTranslate, new string[] { res, "" });
                else return ((string[])TranslationTable[toTranslate])[0];

                return res;
			}
			return Translate(lua);
		}
		public static string Translate(int index)
		{
			Hashtable table= W40kUcs;			
			if (index < 550000)
				table = W40kUcs;
			else if (index < 3700101)
				table = Dxp2Ucs;
			else table = ModUcs;

			if (table.ContainsKey(index))
				return (string)table[index];

			return "";			
		}
		public static string Translate(string key)
		{
			string Key="";
			if (key != null)
			{
				Key = Regex.Replace(key,"(.lua)|(.nil)","");

                if (TranslationTable.ContainsKey(Key))
                {
                    return ((string[])TranslationTable[Key])[0];
                }
                //else TranslationTable.Add(Key,"*** "+Key+" ***");

			}
			return Key;
		}

        public static string TransOwner(string key)
        {
            string Key = "";
            if (key != null)
            {
                Key = Regex.Replace(key, "(.lua)|(.nil)", "");

                if (TranslationTable.ContainsKey(Key))
                {
                    string output = "";
                    output += ((string[])TranslationTable[Key])[1];
                    output += " " + ((string[])TranslationTable[Key])[0];
                    return output;
                }
                //else TranslationTable.Add(Key,"*** "+Key+" ***");

            }
            return Key;
        }
	}
}