using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;



namespace StatsCompiler
{
   
	public class HtmlCompiler
	{
        public static Hashtable RacesTable;
                
        public static string[,] Races=new string[,]
		{
			{"chaos","Chaos"},
			{"eldar","Eldar"},			
			{"guard","Imperial Guard"},			
			{"space_marines","Space Marines"},			
			{"necrons","Necron"},
			{"orks","Ork"},
			{"tau","Tau Empire"},
            {"sisters","Sister of Battle"},
            {"dark_eldar","Dark Eldar"}
		};
		public static string[] SubTypes=new string[]
			{
				"units",
				//"skills",
				"researches",
				//"addons",
				"buildings"
			};
        public static string GetPlural(string s)
        {
            string plural = s;
            string brackets = "";

            Match m = Regex.Match(s, @"(\s?\(.*\))");
            if (m.Success)
            {
                brackets = m.Groups[1].Value;
                plural = s.Remove(plural.Length - brackets.Length, brackets.Length);
            }
            
            if (Regex.Match(plural,"[bcdfghjklmnpqrstvwxz]y$").Success)
                plural = Regex.Replace(plural,"$[bcdfghjklmnpqrstvwxz]y$","ies");
            else if (Regex.Match(plural,"[sxzo]$|sh$|ch$").Success)
                plural = plural+"es";
            else plural += "s";

            return plural + brackets;
        }

        public static string Getarticle(string s)
        {
            if (Regex.Match(s, @"^[a,e,i,o,u,y)]").Success)
                return "an";
            else return "a";
        }
        
        public static void CopyTree(string origin, string target)
		{
			foreach(string file in Directory.GetFiles(origin))
			{		
				string fileName = file.Remove(0,origin.Length+1);
				
				File.Copy(file,Path.Combine(target,fileName));			
			}
			foreach(string dir in Directory.GetDirectories(origin))
			{
				string dirName = dir.Remove(0,origin.Length+1);
				Directory.CreateDirectory(Path.Combine(target,dirName));

				CopyTree(dir,Path.Combine(target,dirName));
			}
        }

        public static void DelTree(string dir)
		{
            foreach (string file in Directory.GetFiles(dir))
                File.Delete(file);
            foreach (string folder in Directory.GetDirectories(dir))
				DelTree(folder);			
			Directory.Delete(dir);
		}
		
		// Create the Html Output
		public static void CompileHtml()
		{
            MainForm.Log("Html creation started...");
            DateTime start = DateTime.Now;
            string contentPath = Path.Combine(DataPath.OutputPath, "Content");
            
				
			if (!Directory.Exists(DataPath.OutputPath))
			    Directory.CreateDirectory(DataPath.OutputPath);
            if (Directory.Exists(contentPath))
                DelTree(contentPath);
            File.Copy(Path.Combine(DataPath.TemplatePath,"style.css"),Path.Combine(DataPath.OutputPath,"style.css"),true);
			
			string template;
			StreamReader reader = new StreamReader(File.OpenRead(Path.Combine(DataPath.TemplatePath,"template.htm")),System.Text.Encoding.UTF8);			
			template = reader.ReadToEnd();
			reader.Close();

			CreateIndex(template);
			CreateRacesIndexes(template);
            int Amount = 0; 
            Amount += DataDumper.Buildings.Count;
            Amount += (DataDumper.Squads.Count*2);

            Amount += DataDumper.Researches.Count;
            MainForm.BarSetMax(Bars.Html, Amount);
            CompileBuildings(template);
            CompileSquads(template);
            CompileResearches(template);

            CompileDpsChart(template);

            TimeSpan elapsed = DateTime.Now - start;
            MainForm.Log("Html compiled in " + Math.Round(elapsed.TotalSeconds,2) + " seconds.");

		}
	
		public static Hashtable CreateLinkList(ArrayList list)
		{
			Hashtable linkTable = new Hashtable();
			for (int i=0;i<9;i++)
                linkTable.Add(Races[i, 0], "<li><a href=\"../../" + Races[i, 0] + ".htm" + "\">Back</a></li>");
			foreach(BuildableInfo info in list)
			{
				string race = info.Race;
				if ( race != "none" )
				{
                    linkTable[race] = (string)linkTable[race] + "<li><a href=\"" + info.FileName + ".htm" + "\">" + Regex.Replace(info.Name, "Add-on|Addon", "") + "</a></li>";		
				}
			}
			return linkTable;
		}
		
		
		public static string CompileRequirements(Hashtable requirements)
		{
			string output="";
			if (requirements!= null)
			{		
				int count=0;
				ArrayList l = new ArrayList(requirements.Values);
				l.Sort();
				foreach(RequirementInfo reqInfo in l)
				{
					if (reqInfo != null)
						output+= reqInfo.Format();
					count++;
					if (count < requirements.Count)
						output+= " &nbsp;-&nbsp; ";
					
				}	
			}
			return output;
		}		

		public static string CompileModifiers(Hashtable modifiers)
		{	
			string output ="";
			if (modifiers != null)
			{
				ArrayList l = new ArrayList(modifiers.Values);
				l.Sort();

				foreach(ModifierInfo modInfo in l)
				{
                    if (modInfo != null && modInfo.IsValid)
                    {
                        string minfo = modInfo.Compile();
                        if (minfo.Length > 0)
                            output += "<li>" + minfo + "</li>";
                    }
				}                
			}
			return output;
		}

        public static Hashtable linkTable = new Hashtable();
        public static ArrayList ResearchesList = new ArrayList();

        
     
        public static void CompileResearches(string template)
        { 
            ArrayList list = DataDumper.Researches;
            list.Sort();
            linkTable = CreateLinkList(list);

            CreateFirstPage(template, linkTable, "/researches/researches.htm");

            foreach (ResearchInfo resInfo in list)
            {
                
                MainForm.BarInc(Bars.Html);
                string race = resInfo.Race;
                if (race == "none")
                    continue;
                
                string file = string.Copy(template);
                file = Regex.Replace(file, @"href=""style.css""", @"href=""../../../style.css""");
                string dir = Path.Combine(DataPath.OutputPath, "content/"+race + @"\researches");

                StreamWriter writer = new StreamWriter(File.Create(Path.Combine(dir, resInfo.FileName + ".htm")));
                Match m = Regex.Match(file, @"</div>[^<div>]*<div\sid=""content"">");
                if (m.Success)
                {
                    int index = m.Index;
                    HtmlInsert(ref index, ref file, @"<ul id=""menu"">");
                    HtmlInsert(ref index, ref file, (string)linkTable[race]);
                    HtmlInsert(ref index, ref file, "</ul>");
                }
                m = Regex.Match(file, @"</div>[^<div>]*<div\sid=""footer"">");
                // Research Table
                if (m.Success)
                {
                    int index = m.Index;
                    HtmlInsert(ref index, ref file, "<h2>" + resInfo.Name + "</h2>");
                    HtmlInsert(ref index, ref file, "<hr>");

                    CompileToolTips(ref index, ref file, resInfo);
                    
                    HtmlInsert(ref index, ref file, @"<table class=""box"">");
                    HtmlInsert(ref index, ref file, @"<tr>");
                    HtmlInsert(ref index, ref file, @"<td>");
                    HtmlInsert(ref index, ref file, @"<div class=""boxHeader unitHeader"">" + resInfo.Name + "</div>");

                    HtmlInsert(ref index, ref file, @"<img class=""icon"" src=""../../../images/" + resInfo.Icon + @".png"">");


                    HtmlInsert(ref index, ref file, @"<div class=""innerInfo"">");
                    HtmlInsert(ref index, ref file, @"<table class=""innerInfoTable"">"); 
                    #region COSTS
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, "<td>Cost</td>");
                    HtmlInsert(ref index, ref file, @"<td>");
                    HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_requisition.gif""><span class=""resourceSpan"">" + resInfo.RequisitionCost + @"</span>");
                    HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_power.gif""><span class=""resourceSpan"">" + resInfo.PowerCost + @"</span>");
                    HtmlInsert(ref index, ref file, "</td>");
                    HtmlInsert(ref index, ref file, "</tr>");
                    #endregion

                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, "<td>Time</td>");
                    HtmlInsert(ref index, ref file, @"<td>" + resInfo.ResearchTime + " seconds</td>");
                    HtmlInsert(ref index, ref file, "</tr>");

                    #region REQUIREMENTS
                    if (resInfo.Requirements != null && resInfo.Requirements.Count > 0)
                    {
                        HtmlInsert(ref index, ref file, "<tr>");
                        HtmlInsert(ref index, ref file, "<td>Requirements</td>");
                        HtmlInsert(ref index, ref file, "<td>");
                        HtmlInsert(ref index, ref file, CompileRequirements(resInfo.Requirements));
                        HtmlInsert(ref index, ref file, "</td>");
                        HtmlInsert(ref index, ref file, "</tr>");
                    }
                    #endregion

                    #region MODIFIERS
                    string mod = CompileModifiers(resInfo.Modifiers);
                    if (mod != "")
                    {
                        HtmlInsert(ref index, ref file, "<tr>");
                        HtmlInsert(ref index, ref file, "<td>Effects</td>");
                        HtmlInsert(ref index, ref file, @"<td><ul class=""innerInfoList"">");
                        HtmlInsert(ref index, ref file, mod);
                        HtmlInsert(ref index, ref file, "</ul></td>");
                        HtmlInsert(ref index, ref file, "</tr>");
                    }

                    #endregion
                    HtmlInsert(ref index, ref file, @"</table>");
                    HtmlInsert(ref index, ref file, @"</div>");
                    HtmlInsert(ref index, ref file, @"</td></tr>");
                    HtmlInsert(ref index, ref file, @"</table>");
                }
                file = file.Replace("<nl>", Environment.NewLine);
                writer.Write(file);
                writer.Close();
            }
        }
		public static void CompileSquads(string template)
		{
            
            Hashtable linkTable = new Hashtable();
			ArrayList list = DataDumper.Squads;

            linkTable = CreateLinkList(list);
			
			CreateFirstPage(template,linkTable,"/units/units.htm");
			foreach(SquadInfo squadInfo in list)
			{
                MainForm.BarInc(Bars.Html);
                string race = squadInfo.Race;
				if (race == "none" || squadInfo.Unit==null)
					continue;
				string file = string.Copy(template);
                file = Regex.Replace(file, "href=\"style.css\"", "href=\"../../../style.css\"");	
				string dir = Path.Combine(DataPath.OutputPath,@"content\"+race+@"\units");
				StreamWriter writer = new StreamWriter(File.Create(Path.Combine(dir,squadInfo.FileName+".htm")));
				Match m = Regex.Match(file,@"</div>[^<div>]*<div\sid=""content"">");
				if (m.Success)
				{
					int index = m.Index;
					HtmlInsert(ref index,ref file,@"<ul id=""menu"">");
					HtmlInsert(ref index,ref file,(string)linkTable[race]);	
					HtmlInsert(ref index,ref file,"</ul>");									
				}
				m = Regex.Match(file,@"</div>[^<div>]*<div\sid=""footer"">");
				if (m.Success)
				{
					int index = m.Index;
					HtmlInsert(ref index,ref file,@"<h2 class=""pageTitle"">"+squadInfo.Name+"</h2>");
					HtmlInsert(ref index,ref file,"<hr>");
					
					CompileToolTips(ref index,ref file,squadInfo); // TODO: SHOULD BE ADDED ?					
					
			   #region SQUAD TABLE
					HtmlInsert(ref index,ref file,@"<table class=""box"">");		
                    HtmlInsert(ref index,ref file,@"<tr>");
                    HtmlInsert(ref index,ref file,@"<td>");
                    HtmlInsert(ref index, ref file, @"<div class=""boxHeader unitHeader"">" + squadInfo.Name + "</div>");

                    HtmlInsert(ref index, ref file, @"<img class=""icon"" src=""../../../images/" + squadInfo.Unit.Icon + @".png"">");
                   

                    HtmlInsert(ref index, ref file, @"<div class=""innerInfo"">");
                    HtmlInsert(ref index, ref file, @"<table class=""innerInfoTable"">");        
                #region COSTS					
					HtmlInsert(ref index,ref file,"<tr>");
					HtmlInsert(ref index,ref file,"<td>Cost</td>");
					HtmlInsert(ref index,ref file,"<td>");
					int totalReq=0; 
					int totalPow=0; 
					
					totalReq= squadInfo.Unit.RequisitionCost * squadInfo.StartingSquadSize;
					totalPow= squadInfo.Unit.PowerCost * squadInfo.StartingSquadSize;
					
					if (squadInfo.Race != "necrons")
                        HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_requisition.gif""><span class=""resourceSpan"">" + totalReq + @"</span>");

                    HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_power.gif""><span class=""resourceSpan"">" + totalPow + @"</span>");
					
					if (squadInfo.SquadCapUsage > 0)
                        HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_cap_infantry.gif""><span class=""resourceSpan"">" + squadInfo.SquadCapUsage + @"</span>");
					else if (squadInfo.Race == "orks"  )
					{
						int totalPop = squadInfo.Unit.PopCost * squadInfo.StartingSquadSize;
						if (totalPop > 0 )
                            HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_orksquadcap.gif""><span class=""resourceSpan"">" + totalPop + @"</span>");
					}
					else if (squadInfo.Race == "guard" && squadInfo.Unit.PopCost>0 )
                        HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_FSR.gif""><span class=""resourceSpan"">" + squadInfo.Unit.PopCost + @"</span>");
					if (squadInfo.SupportCapUsage > 0)
						if (squadInfo.Race != "guard")
                            HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_cap_vehicle.gif""><span class=""resourceSpan"">" + squadInfo.SupportCapUsage + @"</span>");
                        else HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_cap_infantry.gif""><span class=""resourceSpan"">" + squadInfo.SupportCapUsage + @"</span>");
					HtmlInsert(ref index,ref file,"</td>");
					HtmlInsert(ref index,ref file,"</tr>");
				#endregion
				
				#region BUILD-TIME	
					HtmlInsert(ref index,ref file,"<tr>");
					HtmlInsert(ref index,ref file,"<td>Build Time</td>");
					HtmlInsert(ref index,ref file,"<td>");
					HtmlInsert(ref index,ref file,((int)(squadInfo.Unit.BuildTime*squadInfo.StartingSquadSize)).ToString()+" seconds.");
					HtmlInsert(ref index,ref file,"</td>");
					HtmlInsert(ref index,ref file,"</tr>");
				#endregion
					
				#region SQUAD SIZE
					if (squadInfo.MaxSquadSize>1)
					{
						HtmlInsert(ref index,ref file,"<tr>");
						HtmlInsert(ref index,ref file,"<td>Squad size</td>");
						HtmlInsert(ref index,ref file,"<td>");
						HtmlInsert(ref index,ref file,squadInfo.StartingSquadSize+"/"+squadInfo.MaxSquadSize);
						HtmlInsert(ref index,ref file,"</td>");
						HtmlInsert(ref index,ref file,"</tr>");
                    }
                #endregion

                #region REINFORCEMENT

                    if (squadInfo.MaxSquadSize > 1)
                    {
                        HtmlInsert(ref index, ref file, "<tr>");
                        HtmlInsert(ref index, ref file, "<td>Reinforce cost</td>");
                        HtmlInsert(ref index, ref file, "<td>");

                        if (squadInfo.Race != "necrons")
                            HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_requisition.gif""><span class=""resourceSpan"">" + squadInfo.RequisitionCost + @"</span>");

                        HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_power.gif""><span class=""resourceSpan"">" + squadInfo.PowerCost + @"</span>");

                        if (squadInfo.Race == "orks")
                            HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_orksquadcap.gif""><span class=""resourceSpan"">" + squadInfo.Unit.PopCost + @"</span>");

                        HtmlInsert(ref index, ref file, "</td>");
                        HtmlInsert(ref index, ref file, "</tr>");

                        HtmlInsert(ref index, ref file, "<tr>");
                        HtmlInsert(ref index, ref file, "<td>Reinforce time</td>");
                        HtmlInsert(ref index, ref file, "<td>");
                        string time = squadInfo.ReinforceTime.ToString()+ " seconds";
                        if (squadInfo.InCombatTimeMultiplier > 0)
                            time += " ( "+ ((double)squadInfo.ReinforceTime * squadInfo.InCombatTimeMultiplier)+" seconds in combat )";
                        HtmlInsert(ref index, ref file, time);
                        HtmlInsert(ref index, ref file, "</td>");
                        HtmlInsert(ref index, ref file, "</tr>");

                        if (squadInfo.MaxWeapons > 0)
                        {
                            HtmlInsert(ref index, ref file, "<tr>");
                            HtmlInsert(ref index, ref file, "<td>Max Weapons</td>");
                            HtmlInsert(ref index, ref file, "<td>");
                            HtmlInsert(ref index, ref file, squadInfo.MaxWeapons.ToString());
                            HtmlInsert(ref index, ref file, "</td>");
                            HtmlInsert(ref index, ref file, "</tr>");
                        }
                    }
                #endregion

				#region HITS-MORALE
					HtmlInsert(ref index,ref file,"<tr>");
					HtmlInsert(ref index,ref file,"<td>Hit Points</td>");
					HtmlInsert(ref index,ref file,@"<td class=""hits"">");
					if (squadInfo.Unit.HitsRegen != 0)
						HtmlInsert(ref index,ref file,squadInfo.Unit.HitPoints.ToString()+"/"+squadInfo.Unit.HitsRegen.ToString());
					else HtmlInsert(ref index,ref file,squadInfo.Unit.HitPoints.ToString());
					HtmlInsert(ref index,ref file,"</td>");
					HtmlInsert(ref index,ref file,"</tr>");

					if (squadInfo.Morale>0)
					{
						HtmlInsert(ref index,ref file,"<tr>");
						HtmlInsert(ref index,ref file,"<td>Morale</td>");
						HtmlInsert(ref index,ref file,@"<td class=""morale"">");				
						HtmlInsert(ref index,ref file,squadInfo.Morale.ToString()+"/"+(squadInfo.MoraleRegen).ToString());						
						HtmlInsert(ref index,ref file,"</td>");
						HtmlInsert(ref index,ref file,"</tr>");
					}
				#endregion
					
				#region ARMOR 
					HtmlInsert(ref index,ref file,"<tr>");
					HtmlInsert(ref index,ref file,"<td>Armor Type</td>");
					HtmlInsert(ref index,ref file,@"<td>");
					HtmlInsert(ref index,ref file,Translation.Translate(squadInfo.Unit.ArmorType.ToString()));
					HtmlInsert(ref index,ref file,"</td>");
					HtmlInsert(ref index,ref file,"</tr>");
				#endregion

                #region Resurrection
                    if (squadInfo.Unit.RessurectChance > 0)
                    {
                        HtmlInsert(ref index, ref file, "<tr>");
                        HtmlInsert(ref index, ref file, "<td>Resurrection</td>");
                        HtmlInsert(ref index, ref file, @"<td><ul class=""innerInfoList"">");
                        HtmlInsert(ref index, ref file, "<li>Chance: "+squadInfo.Unit.RessurectChance*100+"%</li>"); 
                        HtmlInsert(ref index, ref file, "<li>Hit Points: " + squadInfo.Unit.ResurrectHps * 100 + "%</li>");
                        HtmlInsert(ref index, ref file, "</ul></td>");
                        HtmlInsert(ref index, ref file, "</tr>");
                    }
                #endregion
    	
				#region MASS
					HtmlInsert(ref index,ref file,"<tr>");
					HtmlInsert(ref index,ref file,"<td>Mass</td>");
					HtmlInsert(ref index,ref file,@"<td>");
					HtmlInsert(ref index,ref file,squadInfo.Unit.Mass.ToString());
					HtmlInsert(ref index,ref file,"</td>");
					HtmlInsert(ref index,ref file,"</tr>");
				#endregion
					
				#region SPEED
					HtmlInsert(ref index,ref file,"<tr>");
					HtmlInsert(ref index,ref file,"<td>Speed</td>");
					HtmlInsert(ref index,ref file,@"<td>");
					HtmlInsert(ref index,ref file,squadInfo.Unit.Speed.ToString());
                    if (squadInfo.Unit.CanFly)
                        HtmlInsert(ref index, ref file, " (Air Unit)");
					HtmlInsert(ref index,ref file,"</td>");
					HtmlInsert(ref index,ref file,"</tr>");
				#endregion

				#region SIGHT
					HtmlInsert(ref index,ref file,"<tr>");
					HtmlInsert(ref index,ref file,"<td>Sight Radius</td>");
					HtmlInsert(ref index,ref file,@"<td>");
					string detect = "";
					if (squadInfo.Unit.KeenSight>0)
						detect= "/"+squadInfo.Unit.KeenSight;
					HtmlInsert(ref index,ref file,squadInfo.Unit.Sight.ToString() + detect);
					HtmlInsert(ref index,ref file,"</td>");
					HtmlInsert(ref index,ref file,"</tr>");
				#endregion
					
				#region CHARGE RANGE
					if (squadInfo.Unit.ChargeRange > 0)
					{
						HtmlInsert(ref index,ref file,"<tr>");
						HtmlInsert(ref index,ref file,"<td>Melee Charge Range</td>");
						HtmlInsert(ref index,ref file,@"<td>");
						HtmlInsert(ref index,ref file,squadInfo.Unit.ChargeRange.ToString());
						HtmlInsert(ref index,ref file,"</td>");
						HtmlInsert(ref index,ref file,"</tr>");
					}
				#endregion

                #region TRANSPORT
                if (squadInfo.Unit.TransportSlots > 0)
                {
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, "<td>Transport slots</td>");
                    HtmlInsert(ref index, ref file, @"<td>");
                    HtmlInsert(ref index, ref file, squadInfo.Unit.TransportSlots.ToString());
                    HtmlInsert(ref index, ref file, "</td>");
                    HtmlInsert(ref index, ref file, "</tr>");
                }
                #endregion

                #region INFILTRATION
                    if (squadInfo.Infiltration.InfiltrationType != InfiltrationTypes.None)
                    {
                        HtmlInsert(ref index, ref file, "<tr>");
                        HtmlInsert(ref index, ref file, "<td>Infiltration</td>");
                        HtmlInsert(ref index, ref file, @"<td>");
                        string infilString = "In cover";
                        if (squadInfo.Infiltration.InfiltrationType == InfiltrationTypes.Permanent)
                            infilString = "Permanent";
                        if (squadInfo.Infiltration.Requirements.Count > 0)
                            infilString += " ( Requires: " + CompileRequirements(squadInfo.Infiltration.Requirements) + " )";
                        HtmlInsert(ref index, ref file,infilString );
                        HtmlInsert(ref index, ref file, "</td>");
                        HtmlInsert(ref index, ref file, "</tr>");
                    }
                #endregion

                #region JUMPS
                    if (squadInfo.Jumps.JumpType != JumpTypes.None)
                    {
                        HtmlInsert(ref index, ref file, "<tr>");
                        
                        string jumptype = (squadInfo.Jumps.JumpType == JumpTypes.Teleport)?"Teleport":"Jump";
                        if (squadInfo.Jumps.Jumps > 1)
                            jumptype += "s";
                        HtmlInsert(ref index, ref file, "<td>"+jumptype+"</td>");
                        HtmlInsert(ref index, ref file, @"<td>");
                        string jumpstring = "";
                        jumpstring += squadInfo.Jumps.Jumps + " " + jumptype + " &nbsp;-&nbsp; range " + squadInfo.Jumps.JumpRange + " &nbsp;-&nbsp; recharge " + squadInfo.Jumps.JumpRecharge + " seconds";
                        if (squadInfo.Jumps.Requirements.Count > 0)
                            jumpstring += " ( Requires: " + CompileRequirements(squadInfo.Jumps.Requirements) + " )";

                        HtmlInsert(ref index, ref file, jumpstring);
                        HtmlInsert(ref index, ref file, "</td>");
                        HtmlInsert(ref index, ref file, "</tr>");
                    }
                #endregion

                    #region MOB VALUE
                    if (squadInfo.Unit.MobValue > 0)
                    {
                        HtmlInsert(ref index, ref file, "<tr>");
                        HtmlInsert(ref index, ref file, "<td>Mob Value</td>");
                        HtmlInsert(ref index, ref file, "<td>");
                        HtmlInsert(ref index, ref file, squadInfo.Unit.MobValue.ToString());
                        HtmlInsert(ref index, ref file, "</td>");
                        HtmlInsert(ref index, ref file, "</tr>");
                    }
                    #endregion
                    #region REQUIREMENTS
                    if (squadInfo.Requirements != null && squadInfo.Requirements.Count>0)
					{
						HtmlInsert(ref index,ref file,"<tr>");
						HtmlInsert(ref index,ref file,"<td>Requirements</td>");
						HtmlInsert(ref index,ref file,"<td>");
						HtmlInsert(ref index,ref file,CompileRequirements(squadInfo.Requirements));
						HtmlInsert(ref index,ref file,"</td>");
						HtmlInsert(ref index,ref file,"</tr>");
					}
					#endregion

                #region IN-COMBAT MODIFIERS
                    string htmlMod = "";
					htmlMod += CompileModifiers(squadInfo.Unit.InCombatModifiers);

					if ( htmlMod.Length > 0 )
					{
                        HtmlInsert(ref index,ref file,"<tr>");
						HtmlInsert(ref index,ref file,"<td>Melee modifiers</td>");
						HtmlInsert(ref index,ref file,"<td>");
						HtmlInsert(ref index,ref file,@"<ul class=""innerInfoList"">"+htmlMod+"</ul>");
						HtmlInsert(ref index,ref file,"</td>");
						HtmlInsert(ref index,ref file,"</tr>");
					}
                #endregion
                #region MODIFIERS

                    htmlMod = CompileModifiers(squadInfo.Modifiers);
					htmlMod += CompileModifiers(squadInfo.Unit.Modifiers);

					if ( htmlMod.Length > 0 )
					{
                        HtmlInsert(ref index,ref file,"<tr>");
						HtmlInsert(ref index,ref file,"<td>Effects</td>");
						HtmlInsert(ref index,ref file,"<td>");
						HtmlInsert(ref index,ref file,@"<ul class=""innerInfoList"">"+htmlMod+"</ul>");
						HtmlInsert(ref index,ref file,"</td>");
						HtmlInsert(ref index,ref file,"</tr>");
					}
					#endregion
                    
                    HtmlInsert(ref index, ref file, @"</table>");                    
                    HtmlInsert(ref index, ref file, @"</div>");
                    HtmlInsert(ref index, ref file, @"</td></tr>");
                    HtmlInsert(ref index, ref file, @"</table>");
			    #endregion
					
					
                    UnitInfo entrenchedUnit = null;
                    if (squadInfo.Unit.Extensions.ContainsKey("entrench"))
                        entrenchedUnit = ((EntrenchInfo)squadInfo.Unit.Extensions["entrench"]).EntrenchedUnit;

				#region SKILLS-EXTENSIONS
                    
                    if (squadInfo.Unit.Skills.Count > 0 || squadInfo.Unit.Extensions.Count>0 || squadInfo.Extensions.Count>0)
                    {
                        HtmlInsert(ref index, ref file, "<br>");
                        HtmlInsert(ref index, ref file, "<h3>Squad Abilities</h3><hr>");
                        foreach (SkillInfo skill in squadInfo.Unit.Skills.Values)
                        {
                            CompileSkills(ref index, ref file, skill);
                        }
                        foreach (ExtensionInfo ext in squadInfo.Extensions.Values)
                            ext.Compile(ref index, ref file); 
                        foreach (ExtensionInfo ext in squadInfo.Unit.Extensions.Values)
                            ext.Compile(ref index, ref file); 
                    }

                    if (entrenchedUnit != null)
                    {
                        if (entrenchedUnit.Skills.Count > 0 )
                        {
                            HtmlInsert(ref index, ref file, "<h3>Entrenched Abilities</h3><hr>");
                            foreach (SkillInfo skill in entrenchedUnit.Skills.Values)
                                CompileSkills(ref index, ref file, skill); 
                        }
                    }
						
				#endregion     
                        
                    
                    #region  SQUAD WEAPONS
                    int weaponCount = 0;
                    weaponCount += squadInfo.Unit.WeaponHardPoints.Count;
                    if (entrenchedUnit != null)
                        weaponCount += entrenchedUnit.WeaponHardPoints.Count;
                    if (entrenchedUnit != null)
                        weaponCount += entrenchedUnit.WeaponHardPoints.Count;
                    
                    if (weaponCount > 0 )
                    {
                        bool header = false;
                        foreach (WeaponHardPointInfo whpInfo in squadInfo.Unit.WeaponHardPoints.Values)
                        {
                            foreach (WeaponInfo weapon in whpInfo.Weapons)
                                if (weapon.WeaponIndex == 1 && !weapon.IsDummyWeapon())
                                {
                                    if (!header)
                                    {
                                        HtmlInsert(ref index, ref file, "<br>");
                                        HtmlInsert(ref index, ref file, "<h3>Squad Weapons</h3><hr>");
                                        header = true;
                                    }
                                    CompileWeapon(ref index, ref file, weapon);
                                }
                        }
                        if (entrenchedUnit != null)
                        {
                            foreach (WeaponHardPointInfo whpInfo in entrenchedUnit.WeaponHardPoints.Values)
                            {
                                foreach (WeaponInfo weapon in whpInfo.Weapons)
                                    if (weapon.WeaponIndex == 1 && !weapon.IsDummyWeapon())
                                    {
                                        if (!header)
                                        {
                                            HtmlInsert(ref index, ref file, "<br>");
                                            HtmlInsert(ref index, ref file, "<h3>Squad Weapons</h3><hr>");
                                            header = true;
                                        }
                                        CompileWeapon(ref index, ref file, weapon);
                                    }
                            }
                        }
                    }
				#endregion
                    HtmlInsert(ref index, ref file, "<br/>");
                #region  WEAPONS UPGRADES
                    if (weaponCount > 0)
                    {
                        bool upgrades = false;
                        foreach (WeaponHardPointInfo whpInfo in squadInfo.Unit.WeaponHardPoints.Values)
                        {
                            if (!upgrades)
                            {
                                foreach (WeaponInfo weapon in whpInfo.Weapons)
                                    if (weapon.WeaponIndex > 1 && !weapon.IsDummyWeapon())
                                    {
                                        upgrades = true;
                                        break;
                                    }
                            }
                            else break;
                        }
                        if (entrenchedUnit != null)
                        {
                            foreach (WeaponHardPointInfo whpInfo in entrenchedUnit.WeaponHardPoints.Values)
                            {
                                if (!upgrades)
                                {
                                    foreach (WeaponInfo weapon in whpInfo.Weapons)
                                        if (weapon.WeaponIndex > 1 && !weapon.IsDummyWeapon())
                                        {
                                            upgrades = true;
                                            break;
                                        }
                                }
                                else break;
                            }
                        }
                        if (upgrades)
                        {
                            HtmlInsert(ref index, ref file, "<br>");
                            HtmlInsert(ref index, ref file, "<h3>Weapon Upgrades</h3><hr>");
                            foreach (WeaponHardPointInfo whpInfo in squadInfo.Unit.WeaponHardPoints.Values)
                            {
                                foreach (WeaponInfo weapon in whpInfo.Weapons)
                                    if (weapon.WeaponIndex > 1)
                                        CompileWeapon(ref index, ref file, weapon);
                            }
                            if (entrenchedUnit != null)
                            {
                                foreach (WeaponHardPointInfo whpInfo in entrenchedUnit.WeaponHardPoints.Values)
                                {
                                    foreach (WeaponInfo weapon in whpInfo.Weapons)
                                        if (weapon.WeaponIndex > 1)
                                            CompileWeapon(ref index, ref file, weapon);
                                }
                            }
                        }
                    }
                #endregion
					
					
				
				#region LEADERS
					if (squadInfo.Leaders !=null)
					{
                        HtmlInsert(ref index, ref file, "<br>\n\f");
                        HtmlInsert(ref index, ref file, "<h3>Squad Leaders</h3><hr>");
                        
                        foreach (UnitInfo leaderInfo in squadInfo.Leaders)
						{	
							HtmlInsert(ref index,ref file,"<h4>"+leaderInfo.Name+"</h4>");
							CompileToolTips(ref index,ref file,leaderInfo); //TODO SHOULD BE ADDED?

                            HtmlInsert(ref index, ref file, @"<table class=""box"">");
                            HtmlInsert(ref index, ref file, @"<tr>");
                            HtmlInsert(ref index, ref file, @"<td>");
                            HtmlInsert(ref index, ref file, @"<div class=""boxHeader unitHeader"">" + leaderInfo.Name + "</div>");

                            HtmlInsert(ref index, ref file, @"<img class=""icon"" src=""../../../images/" + leaderInfo.Icon + @".png"">");


                            HtmlInsert(ref index, ref file, @"<div class=""innerInfo"">");
                            HtmlInsert(ref index, ref file, @"<table class=""innerInfoTable"">");
                            #region COSTS					
							HtmlInsert(ref index,ref file,"<tr>");
							HtmlInsert(ref index,ref file,"<td>Cost</td>");
							HtmlInsert(ref index,ref file,"<td>");
											
							if (squadInfo.Race != "necrons")
                                HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_requisition.gif""><span class=""resourceSpan"">" + leaderInfo.RequisitionCost + @"</span>");

                            HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_power.gif""><span class=""resourceSpan"">" + leaderInfo.PowerCost + @"</span>");
					
							if (squadInfo.Race == "orks"  )
                                HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_orksquadcap.gif""><span class=""resourceSpan"">" + leaderInfo.PopCost + @"</span>");

						#endregion
					
						#region BUILD TIME		
							HtmlInsert(ref index,ref file,"<tr>");
							HtmlInsert(ref index,ref file,"<td>Build Time</td>");
							HtmlInsert(ref index,ref file,"<td>");
							HtmlInsert(ref index,ref file,leaderInfo.BuildTime.ToString()+" seconds.");
							HtmlInsert(ref index,ref file,"</td>");
							HtmlInsert(ref index,ref file,"</tr>");
						#endregion
							
						#region HITS-MORALE
							HtmlInsert(ref index,ref file,"<tr>");
							HtmlInsert(ref index,ref file,"<td>Hit Points</td>");
							HtmlInsert(ref index,ref file,@"<td class=""hits"">");
							if (squadInfo.Unit.HitsRegen > 0)
								HtmlInsert(ref index,ref file,leaderInfo.HitPoints.ToString()+"/"+leaderInfo.HitsRegen.ToString());
							else HtmlInsert(ref index,ref file,leaderInfo.HitPoints.ToString());
							HtmlInsert(ref index,ref file,"</td>");
							HtmlInsert(ref index,ref file,"</tr>");
							

							if (leaderInfo.MoraleRegen > 0 || leaderInfo.Morale >0)
							{
								HtmlInsert(ref index,ref file,"<tr>");
								HtmlInsert(ref index,ref file,"<td>Squad Morale</td>");
								HtmlInsert(ref index,ref file,@"<td class=""morale"">");
								if (leaderInfo.Morale > 0 )
								{
									HtmlInsert(ref index,ref file,"+"+leaderInfo.Morale);
									if (leaderInfo.MoraleRegen > 0)
										HtmlInsert(ref index,ref file,"/");
								}
								if (leaderInfo.MoraleRegen > 0 )
									HtmlInsert(ref index,ref file,"+"+leaderInfo.MoraleRegen);
								HtmlInsert(ref index,ref file,"</td>");
								HtmlInsert(ref index,ref file,"</tr>");
							}
						#endregion
							
						#region ARMOR
							HtmlInsert(ref index,ref file,"<tr>");
							HtmlInsert(ref index,ref file,"<td>Armor Type</td>");
							HtmlInsert(ref index,ref file,@"<td>");
							HtmlInsert(ref index,ref file,Translation.Translate(leaderInfo.ArmorType.ToString()));
							HtmlInsert(ref index,ref file,"</td>");
							HtmlInsert(ref index,ref file,"</tr>");
						#endregion
							
						#region SIGHT
							HtmlInsert(ref index,ref file,"<tr>");
							HtmlInsert(ref index,ref file,"<td>Sight Radius</td>");
							HtmlInsert(ref index,ref file,@"<td>");
							detect = "";
							if (leaderInfo.KeenSight>0)
								detect= "/"+leaderInfo.KeenSight;
							HtmlInsert(ref index,ref file,leaderInfo.Sight.ToString() + detect);
							HtmlInsert(ref index,ref file,"</td>");
							HtmlInsert(ref index,ref file,"</tr>");
						#endregion
							
						#region CHARGE
							if (leaderInfo.ChargeRange > 0)
							{
								HtmlInsert(ref index,ref file,"<tr>");
								HtmlInsert(ref index,ref file,"<td>Melee Charge Range</td>");
								HtmlInsert(ref index,ref file,@"<td>");
								HtmlInsert(ref index,ref file,leaderInfo.ChargeRange.ToString());
								HtmlInsert(ref index,ref file,"</td>");
								HtmlInsert(ref index,ref file,"</tr>");
							}
						#endregion

                        #region MOB VALUE
                            if (leaderInfo.MobValue > 0)
                            {
                                HtmlInsert(ref index, ref file, "<tr>");
                                HtmlInsert(ref index, ref file, "<td>Mob Value</td>");
                                HtmlInsert(ref index, ref file, "<td>");
                                HtmlInsert(ref index, ref file, leaderInfo.MobValue.ToString());
                                HtmlInsert(ref index, ref file, "</td>");
                                HtmlInsert(ref index, ref file, "</tr>");
                            }
                            #endregion

                        #region IN-COMBAT MODIFIERS


                            if (leaderInfo.InCombatModifiers != null && leaderInfo.InCombatModifiers.Count > 0)
                            {
                                HtmlInsert(ref index, ref file, "<tr>");
                                HtmlInsert(ref index, ref file, "<td>Melee modifiers</td>");
                                HtmlInsert(ref index, ref file, "<td>");
                                HtmlInsert(ref index, ref file, @"<ul class=""innerInfoList"">" + CompileModifiers(leaderInfo.InCombatModifiers) + "</ul>");
                                HtmlInsert(ref index, ref file, "</td>");
                                HtmlInsert(ref index, ref file, "</tr>");
                            }
                            #endregion
	
						#region MODIFIERS
                            string mods = CompileModifiers(leaderInfo.Modifiers);
                            if (mods != "")
							{
								HtmlInsert(ref index,ref file,"<tr>");
								HtmlInsert(ref index,ref file,"<td>Effects</td>");
								HtmlInsert(ref index,ref file,@"<td><ul class=""innerInfoList"">");
								HtmlInsert(ref index,ref file,mods);
								HtmlInsert(ref index,ref file,"</ul></td>");
								HtmlInsert(ref index,ref file,"</tr>");
							}
							#endregion

       
                        HtmlInsert(ref index, ref file, @"</table>");                    
                    HtmlInsert(ref index, ref file, @"</div>");
                    HtmlInsert(ref index, ref file, @"</td></tr>");
                    HtmlInsert(ref index, ref file, @"</table>");
							
						#region LEADER SKILLS                          
                            if (leaderInfo.Skills.Count > 0 || leaderInfo.Extensions.Count > 0)
                            {
                                HtmlInsert(ref index, ref file, "<br>\n\f");
                                HtmlInsert(ref index, ref file, "<h3>Leader Abilities</h3><hr>");
                                foreach (SkillInfo skill in leaderInfo.Skills.Values)
                                    CompileSkills(ref index, ref file, skill);
                                foreach (ExtensionInfo ext in leaderInfo.Extensions.Values)
                                    ext.Compile(ref index, ref file);
                            }
                        #endregion

                        #region  Leader WEAPONS
                            if (leaderInfo.WeaponHardPoints.Count > 0)
                            {
                                bool header = false;
                                foreach (WeaponHardPointInfo whpInfo in leaderInfo.WeaponHardPoints.Values)
                                {
                                    if (whpInfo.Weapons.Count > 0 && ((WeaponInfo)whpInfo.Weapons[0]).WeaponIndex == 1 && !((WeaponInfo)whpInfo.Weapons[0]).IsDummyWeapon())
                                    {
                                        if (!header)
                                        {
                                            HtmlInsert(ref index, ref file, "<br>\n\f");
                                            HtmlInsert(ref index, ref file, "<h3>Leader Weapons</h3><hr>");
                                            header = true;
                                        }
                                        CompileWeapon(ref index, ref file, (WeaponInfo)whpInfo.Weapons[0]);
                                    }
                                }
                            }
                            #endregion
                            HtmlInsert(ref index, ref file, "<br/>");
                        #region  WEAPONS UPGRADES
                            if (leaderInfo.WeaponHardPoints.Count > 0)
                            {
                                bool upgrades = false;
                                foreach (WeaponHardPointInfo whpInfo in leaderInfo.WeaponHardPoints.Values)
                                {
                                    if (!upgrades)
                                    {
                                        foreach (WeaponInfo weapon in whpInfo.Weapons)
                                            if (weapon.WeaponIndex > 1)
                                            {
                                                upgrades = true;
                                                break;
                                            }
                                    }
                                    else break;
                                }
                                if (upgrades)
                                {
                                    HtmlInsert(ref index, ref file, "<br>\n\f");
                                    HtmlInsert(ref index, ref file, "<h3>Leader Weapons Upgrades</h3><hr>");
                                    foreach (WeaponHardPointInfo whpInfo in leaderInfo.WeaponHardPoints.Values)
                                    {
                                        foreach (WeaponInfo weapon in whpInfo.Weapons)
                                            if (weapon.WeaponIndex > 1 && !weapon.IsDummyWeapon())
                                                CompileWeapon(ref index, ref file, weapon);
                                    }
                                }
                            }
                            #endregion

							
					#endregion
						}
					}
					
				}
                file=file.Replace("<nl>", Environment.NewLine);
				writer.Write(file);
				writer.Close();
			}

			
		}						
		public static void CompileBuildings(string template)
		{
			Hashtable linkTable = new Hashtable();
			ArrayList list = DataDumper.Buildings;
			linkTable = CreateLinkList(list);
			
			CreateFirstPage(template,linkTable,"/buildings/buildings.htm");
			foreach(BuildingInfo buildInfo in list)
			{
                MainForm.BarInc(Bars.Html);
                string race = buildInfo.Race;
				if (race == "none")
					continue;
				string file = string.Copy(template);	
				file = Regex.Replace(file,@"href=""style.css""",@"href=""../../../style.css""");	
                string dir = Path.Combine(DataPath.OutputPath, @"content\" + race + @"\buildings");
                StreamWriter writer = new StreamWriter(File.Create(Path.Combine(dir,buildInfo.FileName+".htm")));
				Match m = Regex.Match(file,@"</div>[^<div>]*<div\sid=""content"">");
				if (m.Success)
				{
					int index = m.Index;
					HtmlInsert(ref index,ref file,@"<ul id=""menu"">");
					HtmlInsert(ref index,ref file,(string)linkTable[race]);	
					HtmlInsert(ref index,ref file,"</ul>");									
				}
				m = Regex.Match(file,@"</div>[^<div>]*<div\sid=""footer"">");
				if (m.Success)
				{
					int index = m.Index;
					HtmlInsert(ref index,ref file,@"<h2 class=""pageTitle"">"+buildInfo.Name+"</h2>");
					HtmlInsert(ref index,ref file,"<hr>");
					
					CompileToolTips(ref index,ref file,buildInfo);				
					
				#region BUILDING TABLE
                    HtmlInsert(ref index, ref file, @"<table class=""box"">");
                    HtmlInsert(ref index, ref file, @"<tr>");
                    HtmlInsert(ref index, ref file, @"<td>");
                    HtmlInsert(ref index, ref file, @"<div class=""boxHeader buildingHeader"">" + buildInfo.Name + "</div>");

                    HtmlInsert(ref index, ref file, @"<img class=""icon"" src=""../../../images/" + buildInfo.Icon + @".png"">");


                    HtmlInsert(ref index, ref file, @"<div class=""innerInfo"">");
                    HtmlInsert(ref index, ref file, @"<table class=""innerInfoTable"">");
										
					#region COSTS					
					HtmlInsert(ref index,ref file,"<tr>");
					HtmlInsert(ref index,ref file,"<td>Cost</td>");
					HtmlInsert(ref index,ref file,"<td>");
					
					
					if (buildInfo.Race != "necrons")
                        HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_requisition.gif""><span class=""resourceSpan"">" + buildInfo.RequisitionCost + @"</span>");

                    HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_power.gif""><span class=""resourceSpan"">" + buildInfo.PowerCost + @"</span>");					
					#endregion

                    #region BUILD TIME
                    HtmlInsert(ref index,ref file,"<tr>");
					HtmlInsert(ref index,ref file,"<td>Build Time</td>");
					HtmlInsert(ref index,ref file,"<td>");
					HtmlInsert(ref index,ref file,((int)(buildInfo.BuildTime)).ToString()+" seconds.");
					HtmlInsert(ref index,ref file,"</td>");
					HtmlInsert(ref index,ref file,"</tr>");
                    #endregion

                    
                    HtmlInsert(ref index,ref file,"<tr>");
					HtmlInsert(ref index,ref file,"<td>Hit Points</td>");
					HtmlInsert(ref index,ref file,@"<td class=""hits"">");
					HtmlInsert(ref index,ref file,buildInfo.HitPoints.ToString());
                    
                    HtmlInsert(ref index,ref file,"</td>");
					HtmlInsert(ref index,ref file,"</tr>");
					HtmlInsert(ref index,ref file,"</td>");

					HtmlInsert(ref index,ref file,"</tr>");
                    HtmlInsert(ref index,ref file,"<tr>");
					HtmlInsert(ref index,ref file,"<td>Armor Type</td>");
					HtmlInsert(ref index,ref file,@"<td>");
					HtmlInsert(ref index,ref file,Translation.Translate(buildInfo.ArmorType.ToString()));
					HtmlInsert(ref index,ref file,"</td>");
					HtmlInsert(ref index,ref file,"</tr>");					
					
					HtmlInsert(ref index,ref file,"<tr>");
					HtmlInsert(ref index,ref file,"<td>Sight Radius</td>");
					HtmlInsert(ref index,ref file,@"<td>");
					HtmlInsert(ref index,ref file,buildInfo.Sight.ToString());
					HtmlInsert(ref index,ref file,"</td>");
					HtmlInsert(ref index,ref file,"</tr>");

                    // Resource income                    
                    if (buildInfo.PowerIncome > 0 || buildInfo.RequisitionIncome > 0 || buildInfo.RequisitionModifier > 0)
                    {
                        HtmlInsert(ref index, ref file, "<tr>");
                        HtmlInsert(ref index, ref file, "<td>Resource Income</td>");
                        HtmlInsert(ref index, ref file, @"<td>");
                        if (buildInfo.RequisitionIncome > 0 )
                            HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_requisition.gif""><span class=""resourceSpan"">+" + buildInfo.RequisitionIncome + @"</span>");
                        if (buildInfo.RequisitionModifier > 0)
                            HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_requisition.gif""><span class=""resourceSpan"">+" + buildInfo.RequisitionModifier + @"</span>");
                        if (buildInfo.PowerIncome > 0)
                            HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_power.gif""><span class=""resourceSpan"">+" + buildInfo.PowerIncome + @"</span>");
 
                        HtmlInsert(ref index, ref file, "</td>");
                        HtmlInsert(ref index, ref file, "</tr>");
                    }
                    if (buildInfo.TransportSlots > 0)
                    {
                        HtmlInsert(ref index, ref file, "<tr>");
                        HtmlInsert(ref index, ref file, "<td>Unit slots</td>");
                        HtmlInsert(ref index, ref file, @"<td>");
                        HtmlInsert(ref index, ref file, buildInfo.TransportSlots.ToString());
                        HtmlInsert(ref index, ref file, "</td>");
                        HtmlInsert(ref index, ref file, "</tr>");
                    }
					
                    #region REQUIREMENTS
					if (buildInfo.Requirements != null && buildInfo.Requirements.Count>0)
					{
                        HtmlInsert(ref index,ref file,"<tr>");
						HtmlInsert(ref index,ref file,"<td>Requirements</td>");
						HtmlInsert(ref index,ref file,"<td>");
						HtmlInsert(ref index,ref file,CompileRequirements(buildInfo.Requirements));
						HtmlInsert(ref index,ref file,"</td>");
						HtmlInsert(ref index,ref file,"</tr>");
					}
					#endregion
					
					#region MODIFIERS

                    string htmlMod = CompileModifiers(buildInfo.Modifiers);

                    if (htmlMod.Length > 0)
					{
                        
                        HtmlInsert(ref index,ref file,"<tr>");
						HtmlInsert(ref index,ref file,"<td>Effects</td>");
						HtmlInsert(ref index,ref file,@"<td><ul class=""innerInfoList"">");
						HtmlInsert(ref index,ref file,htmlMod);
						HtmlInsert(ref index,ref file,"</ul></td>");
						HtmlInsert(ref index,ref file,"</tr>");
					}
					
					#endregion

                    #region RESEARCHES
                    if (buildInfo.Researches != null && buildInfo.Researches.Count > 0)
                    {
                        HtmlInsert(ref index, ref file, "<tr>");
                        HtmlInsert(ref index, ref file, "<td>Researches</td>");
                        HtmlInsert(ref index, ref file, "<td>");
                        HtmlInsert(ref index, ref file, @"<ul class=""innerInfoList"">");
                                                
                        foreach (string s in buildInfo.Researches)
                        { 
                            HtmlInsert(ref index, ref file, @"<li><a href=""" + "../researches/" + s + @".htm"">" + Translation.Translate(s) + @"</li>");
                        }
                        HtmlInsert(ref index, ref file, "</ul>");
                        HtmlInsert(ref index, ref file, "</td>");
                        HtmlInsert(ref index, ref file, "</tr>");
                    }
                    #endregion

					#region BUILDABLE UNITS
					if (buildInfo.BuildUnits != null && buildInfo.BuildUnits.Count > 0)
					{
						HtmlInsert(ref index,ref file,"<tr>");
						HtmlInsert(ref index,ref file,"<td>Buildable Units</td>");
						HtmlInsert(ref index,ref file,"<td>");
                        HtmlInsert(ref index, ref file, @"<ul class=""innerInfoList"">");
						foreach(string s in buildInfo.BuildUnits)
                            HtmlInsert(ref index, ref file, @"<li><a href=""" + "../units/" + s + @".htm"">"+Translation.Translate(s)+@"</li>");					
						HtmlInsert(ref index,ref file,"</ul>");
						HtmlInsert(ref index,ref file,"</td>");
						HtmlInsert(ref index,ref file,"</tr>");
					}
					#endregion
        
                    HtmlInsert(ref index, ref file, @"</table>");                    
                    HtmlInsert(ref index, ref file, @"</div>");
                    HtmlInsert(ref index, ref file, @"</td></tr>");
                    HtmlInsert(ref index, ref file, @"</table>");
				#endregion

                    HtmlInsert(ref index, ref file, "<br/>");
                    
                #region ADD-ONS
                    if (buildInfo.Addons.Count > 0)
                    {
                        HtmlInsert(ref index, ref file, "<h3>Building's Add-Ons</h3><hr>");
                        foreach (ResearchInfo addon in buildInfo.Addons)
                        {

                            HtmlInsert(ref index, ref file, "<h4>" + addon.Name + "</h4>");

                            CompileToolTips(ref index, ref file, addon);


                            HtmlInsert(ref index, ref file, @"<table class=""box"">");
                            HtmlInsert(ref index, ref file, @"<tr>");
                            HtmlInsert(ref index, ref file, @"<td>");
                            HtmlInsert(ref index, ref file, @"<div class=""boxHeader resHeader"">" +addon.Name + "</div>");

                            HtmlInsert(ref index, ref file, @"<img class=""icon"" src=""../../../images/" + addon.Icon + @".png"">");


                            HtmlInsert(ref index, ref file, @"<div class=""innerInfo"">");
                            HtmlInsert(ref index, ref file, @"<table class=""innerInfoTable"">");

                            #region COSTS
                            HtmlInsert(ref index, ref file, "<tr>");
                            HtmlInsert(ref index, ref file, "<td>Cost</td>");
                            HtmlInsert(ref index, ref file, @"<td>");
                            HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_requisition.gif""><span class=""resourceSpan"">" + addon.RequisitionCost + @"</span>");
                            HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_power.gif""><span class=""resourceSpan"">" + addon.PowerCost + @"</span>");
                            HtmlInsert(ref index, ref file, "</td>");
                            HtmlInsert(ref index, ref file, "</tr>");
                            #endregion

                            HtmlInsert(ref index, ref file, "<tr>");
                            HtmlInsert(ref index, ref file, "<td>Time</td>");
                            HtmlInsert(ref index, ref file, @"<td>" + addon.ResearchTime + " seconds</td>");
                            HtmlInsert(ref index, ref file, "</tr>");

                            HtmlInsert(ref index, ref file, "</td>");
                            HtmlInsert(ref index, ref file, "</tr>");
                            #region REQUIREMENTS
                            if (addon.Requirements != null && addon.Requirements.Count > 0)
                            {
                                HtmlInsert(ref index, ref file, "<tr>");
                                HtmlInsert(ref index, ref file, "<td>Requirements</td>");
                                HtmlInsert(ref index, ref file, "<td>");
                                HtmlInsert(ref index, ref file, CompileRequirements(addon.Requirements));
                                HtmlInsert(ref index, ref file, "</td>");
                                HtmlInsert(ref index, ref file, "</tr>");
                            }
                            #endregion

                            #region MODIFIERS
                            if (addon.Modifiers != null && addon.Modifiers.Count > 0)
                            {
                                HtmlInsert(ref index, ref file, "<tr>");
                                HtmlInsert(ref index, ref file, "<td>Effects</td>");
                                HtmlInsert(ref index, ref file, @"<td><ul class=""innerInfoList"">");
                                HtmlInsert(ref index, ref file, CompileModifiers(addon.Modifiers));
                                HtmlInsert(ref index, ref file, "</ul></td>");
                                HtmlInsert(ref index, ref file, "</tr>");
                            }

                            #endregion
                            HtmlInsert(ref index, ref file, @"</table>");
                            HtmlInsert(ref index, ref file, @"</div>");
                            HtmlInsert(ref index, ref file, @"</td></tr>");
                            HtmlInsert(ref index, ref file, @"</table>");
                            HtmlInsert(ref index, ref file, "<br/>");
                        }
                #endregion

                    }

                    
                    #region SKILLS-EXTENSIONS

                    if (buildInfo.Skills.Count > 0 || buildInfo.Extensions.Count > 0)
                    {
                        HtmlInsert(ref index, ref file, "<h3>Buildings Abilities</h3><hr>");
                        foreach (SkillInfo skill in buildInfo.Skills.Values)
                            CompileSkills(ref index, ref file, skill);
                        foreach (ExtensionInfo ext in buildInfo.Extensions.Values)
                            ext.Compile(ref index, ref file);
                        
                    }

                    #endregion
                    #region  BUIlDING WEAPONS
                    if (buildInfo.WeaponHardPoints.Count > 0)
                    {
                        bool header = false;
                        foreach (WeaponHardPointInfo whpInfo in buildInfo.WeaponHardPoints.Values)
                        {
                            foreach (WeaponInfo weapon in whpInfo.Weapons)
                                if (weapon.WeaponIndex == 1 && !weapon.IsDummyWeapon())
                                {
                                    if (!header)
                                    {
                                        HtmlInsert(ref index, ref file, "<h3>Building Weapons</h3><hr>");
                                        header = true;
                                    }               
                                    CompileWeapon(ref index, ref file, weapon);
                                }

                        }
                    }
                     #endregion
                    HtmlInsert(ref index, ref file, "<br/>");
                    #region  WEAPONS UPGRADES
                    if (buildInfo.WeaponHardPoints.Count > 0)
                    {
                        bool upgrades = false;
                        foreach (WeaponHardPointInfo whpInfo in buildInfo.WeaponHardPoints.Values)
                        {
                            if (!upgrades)
                            {
                                foreach (WeaponInfo weapon in whpInfo.Weapons)
                                    if (weapon.WeaponIndex > 1)
                                    {
                                        upgrades = true;
                                        break;
                                    }
                            }
                            else break;
                        }
                        if (upgrades)
                        {
                            HtmlInsert(ref index, ref file, "<h3>Weapon Upgrades</h3><hr>");
                            foreach (WeaponHardPointInfo whpInfo in buildInfo.WeaponHardPoints.Values)
                            {
                                foreach (WeaponInfo weapon in whpInfo.Weapons)
                                    if (weapon.WeaponIndex > 1 && !weapon.IsDummyWeapon())
                                        CompileWeapon(ref index, ref file, weapon);
                            }
                        }
                    }
                    #endregion
                    
                  
					
					HtmlInsert(ref index,ref file,@"<br>");
				}
                file=file.Replace("<nl>", Environment.NewLine);
				writer.Write(file);
				writer.Close();
			}

			
		}

        public static void CompileSummons(ref int index, ref string file, SkillInfo skillInfo)
        {
            HtmlInsert(ref index, ref file, "<h4>" + Translation.Translate(skillInfo.Name) + "</h4>");
            CompileToolTips(ref index, ref file, skillInfo); // TODO: SHOULD BE ADDED ?					

            HtmlInsert(ref index, ref file, @"<table class=""box"">");
            HtmlInsert(ref index, ref file, @"<tr>");
            HtmlInsert(ref index, ref file, @"<td>");
            HtmlInsert(ref index, ref file, @"<div class=""boxHeader skillHeader"">" + skillInfo.Name + "</div>");
            if (skillInfo.Icon == null)
                HtmlInsert(ref index, ref file, @"<img class=""icon"" src=""../../../images/general/PassiveAbility_icon.jpg"">");
            else HtmlInsert(ref index, ref file, @"<img class=""icon"" src=""../../../images/" + skillInfo.Icon + @".png"">");


            HtmlInsert(ref index, ref file, @"<div class=""innerInfo"">");
            HtmlInsert(ref index, ref file, @"<table class=""innerInfoTable"">");

            #region SKILL INFO TABLE


            #region COSTS
            if (skillInfo.PowerCost > 0 || skillInfo.RequisitionCost > 0)
            {
                HtmlInsert(ref index, ref file, "<tr>");
                HtmlInsert(ref index, ref file, "<td>Cost</td>");
                HtmlInsert(ref index, ref file, @"<td>");
                HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_requisition.gif""><span class=""resourceSpan"">" + skillInfo.RequisitionCost + @"</span>");
                HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_power.gif""><span class=""resourceSpan"">" + skillInfo.PowerCost + @"</span>");
                HtmlInsert(ref index, ref file, "</td>");
                HtmlInsert(ref index, ref file, "</tr>");
            }
            #endregion

            #region APPLICATION-TYPE
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, "<td>Application Type</td>");
            HtmlInsert(ref index, ref file, @"<td>");
            HtmlInsert(ref index, ref file, "Summon ability");
            HtmlInsert(ref index, ref file, @"</td>");
            HtmlInsert(ref index, ref file, "</tr>");

            #endregion            

            #region COOLDOWN
            if (skillInfo.RechargeTime > 0)
            {
                HtmlInsert(ref index, ref file, "<tr>");
                HtmlInsert(ref index, ref file, "<td>Recharge time</td>");
                HtmlInsert(ref index, ref file, @"<td>" + skillInfo.RechargeTime + " seconds.</td>");
                HtmlInsert(ref index, ref file, "</tr>");
            }
            #endregion

            #region EFFECTS
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, "<td>Effects</td>");
            HtmlInsert(ref index, ref file, "<td>Summons a "+Translation.Translate(skillInfo.Spawn.Name)+"</td>");
            HtmlInsert(ref index, ref file, "</tr>");
            #endregion

            #region SUMMON STATS
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, "<td>Hit Points</td>");
            HtmlInsert(ref index, ref file, @"<td class=""hits"">");
            if (skillInfo.Spawn.HitsRegen != 0)
                HtmlInsert(ref index, ref file, skillInfo.Spawn.HitPoints.ToString() + "/" + skillInfo.Spawn.HitsRegen.ToString());
            else HtmlInsert(ref index, ref file, skillInfo.Spawn.HitPoints.ToString());
            HtmlInsert(ref index, ref file, "</td>");
            HtmlInsert(ref index, ref file, "</tr>");

            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, "<td>Armor Type</td>");
            HtmlInsert(ref index, ref file, "<td>");
            HtmlInsert(ref index, ref file, Translation.Translate(skillInfo.Spawn.ArmorType.ToString()));
            HtmlInsert(ref index, ref file, "</td>");
            HtmlInsert(ref index, ref file, "</tr>");

            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, "<td>Sight</td>");
            HtmlInsert(ref index, ref file, "<td>");
            HtmlInsert(ref index, ref file, skillInfo.Spawn.Sight+"/"+skillInfo.Spawn.KeenSight);
            HtmlInsert(ref index, ref file, "</td>");
            HtmlInsert(ref index, ref file, "</tr>");
            #endregion

            #region ABILITIES
            foreach (SkillInfo skill in skillInfo.Spawn.Skills.Values)
            {

                HtmlInsert(ref index, ref file, "<tr>");
                HtmlInsert(ref index, ref file, "<td>" + Translation.Translate(skill.Name) + @"</td>");
                

                #region Requirements
                bool req = (skill.Requirements != null && skill.Requirements.Count > 0);
                if (req)
                    HtmlInsert(ref index, ref file, "<td>Requires: " + CompileRequirements(skill.Requirements) + @"</td></tr>");  
          
                #endregion

                #region target-TYPE
                if (skill.Modifiers.Count > 0 && ((skill.Filter != null && skill.Filter.Count > 0) || skill.AOEFilter != ""))
                {
                    if (req)
                        HtmlInsert(ref index, ref file, "<tr><td></td>");
                    
                    HtmlInsert(ref index, ref file, @"<td>Target types: ");
                    string trg = skill.FormatFilter();
                    if (trg != "")
                        HtmlInsert(ref index, ref file, skill.FormatAlliance() + trg);
                    else if (skill.Parent is UnitInfo)
                    {
                        if (((UnitInfo)skill.Parent).IsIndipendant())
                            HtmlInsert(ref index, ref file, @"Self");
                        else HtmlInsert(ref index, ref file, @"All squad members");
                    }
                    else HtmlInsert(ref index, ref file, @"TO DO");

                    HtmlInsert(ref index, ref file, @"</td>");
                    HtmlInsert(ref index, ref file, "</tr>");
                    if (!req)
                        req = true;
                }
                #endregion

                #region AREA OF EFFECT
                if (skill.Radius > 0)
                {
                    if (req)
                        HtmlInsert(ref index, ref file, "<tr><td></td>");                   
                    HtmlInsert(ref index, ref file, @"<td>Area of Effect: " + skill.Radius + "</td>");
                   
                    HtmlInsert(ref index, ref file, "</tr>");
                    if (!req)
                        req = true;
                }
                
                #endregion

                #region MODIFIERS
                string mod = "";
                mod += CompileModifiers(skill.Modifiers);

                if (mod != "")
                {
                    if (req)
                        HtmlInsert(ref index, ref file, "<tr><td></td>");                   
                    HtmlInsert(ref index, ref file, @"<td><ul class=""innerInfoList"">");
                    HtmlInsert(ref index, ref file, mod);
                    HtmlInsert(ref index, ref file, "</ul></td>");                   
                    HtmlInsert(ref index, ref file, "</tr>");
                    if (!req)
                        req = true;
                }
                #endregion

            }
            #endregion

            #endregion

            HtmlInsert(ref index, ref file, @"</table>");
            HtmlInsert(ref index, ref file, @"</div>");
            HtmlInsert(ref index, ref file, @"</td></tr>");
            HtmlInsert(ref index, ref file, @"</table>");
            HtmlInsert(ref index, ref file, @"<br/>");
        }

        public static void CompileSkillInfoTable(ref int index, ref string file, SkillInfo skillInfo, int dept)
        {   
            if (skillInfo.IsValid)
            {
                HtmlInsert(ref index, ref file, @"<table class=""innerInfoTable"">");

                #region SKILL INFO TABLE

                if (dept > 0 )
                    HtmlInsert(ref index, ref file, @"<tr><td colspan=""2"">Child ability</td></tr>");

                #region COSTS
                if (skillInfo.PowerCost > 0 || skillInfo.RequisitionCost > 0)
                {
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, "<td>Cost</td>");
                    HtmlInsert(ref index, ref file, @"<td>");
                    HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_requisition.gif""><span class=""resourceSpan"">" + skillInfo.RequisitionCost + @"</span>");
                    HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_power.gif""><span class=""resourceSpan"">" + skillInfo.PowerCost + @"</span>");
                    HtmlInsert(ref index, ref file, "</td>");
                    HtmlInsert(ref index, ref file, "</tr>");
                }
                #endregion

                #region APPLICATION-TYPE
                if (dept == 0)
                {
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, "<td>Application Type</td>");
                    HtmlInsert(ref index, ref file, @"<td>");
                    HtmlInsert(ref index, ref file, skillInfo.GetActivationType());
                    HtmlInsert(ref index, ref file, @"</td>");
                    HtmlInsert(ref index, ref file, "</tr>");
                }
                #endregion

                #region target-TYPE

                HtmlInsert(ref index, ref file, "<tr>");
                HtmlInsert(ref index, ref file, "<td>Target types</td>");
                HtmlInsert(ref index, ref file, @"<td>");               

                if ( (skillInfo.MinDamage!=0 || skillInfo.MoraleDamage!=0 || skillInfo.Modifiers.Count > 0) && ( (skillInfo.Filter != null && skillInfo.Filter.Count > 0) || skillInfo.AOEFilter != ""))
                {
                    
                    string trg = skillInfo.FormatFilter();
                    if (trg != "")
                        HtmlInsert(ref index, ref file, skillInfo.FormatAlliance() + trg);
                    else if (skillInfo.Parent is UnitInfo)
                    {
                        if (((UnitInfo)skillInfo.Parent).IsIndipendant())
                            HtmlInsert(ref index, ref file, @"Self");
                        else HtmlInsert(ref index, ref file, @"All squad members");
                    }
                    else if (skillInfo.Parent is BuildingInfo)
                        HtmlInsert(ref index, ref file, @"Self");
                    else HtmlInsert(ref index, ref file, @"--");
                }
                else HtmlInsert(ref index, ref file, @"Self");
                HtmlInsert(ref index, ref file, @"</td>");
                HtmlInsert(ref index, ref file, "</tr>");


                #endregion

                #region AREA OF EFFECT - RANGE
                if (dept == 0 && skillInfo.Range > 0 && skillInfo.GetActivationType() == "Target Ability")
                {
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, "<td>Range</td>");
                    if (!skillInfo.IsChild)
                        HtmlInsert(ref index, ref file, @"<td>" + skillInfo.Range + "</td>");
                    else HtmlInsert(ref index, ref file, @"<td>" + skillInfo.ParentSkill.Range + "</td>");
                    HtmlInsert(ref index, ref file, "</tr>");
                }
                if (skillInfo.Radius > 0)
                {
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, "<td>Area of Effect</td>");
                    HtmlInsert(ref index, ref file, @"<td>" + skillInfo.Radius + "</td>");
                    HtmlInsert(ref index, ref file, "</tr>");
                }
                
                #endregion

                #region DURATION AND RECHARGE TIME
                if (skillInfo.Duration > 0 && skillInfo.GetActivationType() != "Passive Ability")
                {
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, "<td>Duration</td>");

                    HtmlInsert(ref index, ref file, @"<td>" + skillInfo.Duration.ToString() + " seconds.</td>");
                    HtmlInsert(ref index, ref file, "</tr>");
                }
                if (dept ==0 && skillInfo.RootParentSkill.RechargeTime>0)
                {
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, "<td>Recharge time</td>");
                    if (skillInfo.GlobalTimer)
                        HtmlInsert(ref index, ref file, @"<td>" + skillInfo.RootParentSkill.RechargeTime.ToString() + " seconds (global)</td>");
                    else HtmlInsert(ref index, ref file, @"<td>" + skillInfo.RootParentSkill.RechargeTime.ToString() + " seconds</td>");

                    HtmlInsert(ref index, ref file, "</tr>");
                }
                #endregion

                #region REQUIREMENTS
                if (dept == 0 && skillInfo.Requirements != null && skillInfo.Requirements.Count > 0)
                {
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, "<td>Requirements</td>");
                    HtmlInsert(ref index, ref file, "<td>");
                    HtmlInsert(ref index, ref file, CompileRequirements(skillInfo.Requirements));
                    HtmlInsert(ref index, ref file, "</td>");
                    HtmlInsert(ref index, ref file, "</tr>");
                }
                #endregion

                #region MODIFIERS
                string mod = "";
                mod += CompileModifiers(skillInfo.Modifiers);
                
                if (DataDumper.RepeatedAbilities.ContainsKey(skillInfo.LuaName))
                {
                    string[] args = (string[])DataDumper.RepeatedAbilities[skillInfo.LuaName];
                    mod += "<li>Strikes up to "+args[1]+" times</li>";
                }

                if (mod != "")
                {
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, "<td>Effects</td>");

                    HtmlInsert(ref index, ref file, "<td>");


                    HtmlInsert(ref index, ref file, @"<ul class=""innerInfoList"">");
                    HtmlInsert(ref index, ref file, mod);
                    HtmlInsert(ref index, ref file, "</ul>");

                    HtmlInsert(ref index, ref file, "</td>");
                    HtmlInsert(ref index, ref file, "</tr>");
                }

                #endregion

                #endregion
                HtmlInsert(ref index, ref file, @"</table>");
                bool log = skillInfo.LuaName == "guard_strafing_run";

                #region DAMAGE
                if (skillInfo.IsValidDamage())
                {
                    HtmlInsert(ref index, ref file, @"<table class=""dpsTable skillDpsTable"">");
                    HtmlInsert(ref index, ref file, "<tr>");
                    bool overtime = skillInfo.Refresh < skillInfo.Duration && !skillInfo.TargetGround;
                    if (overtime)
                        HtmlInsert(ref index, ref file, @"<th colspan=""8"">Total Damage</th>");
                    else if (DataDumper.RepeatedAbilities.ContainsKey(skillInfo.LuaName))
                        HtmlInsert(ref index, ref file, @"<th colspan=""8"">"+((string[])DataDumper.RepeatedAbilities[skillInfo.LuaName])[0]+"</th>");
                    else HtmlInsert(ref index, ref file, @"<th colspan=""8"">Damage values</th>");
                    
                    
                    HtmlInsert(ref index, ref file, "</tr>");

                    #region DpsTables
                    double dps = 0.0;
                    HtmlInsert(ref index, ref file, "<tr>");

                    #region INFANTRY
                    HtmlInsert(ref index, ref file, "<td>");
                    HtmlInsert(ref index, ref file, @"<table class =""dpsCell"">");
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, @"<th colspan=""3"" class=""armorClass"">Infantry</th>");
                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, @"<td class=""armorClass"">Low</td>");
                    HtmlInsert(ref index, ref file, @"<td class=""armorClass"">Med</td>");
                    HtmlInsert(ref index, ref file, @"<td class=""armorClass"">High</td>");
                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "<tr>");
                    dps = GetDps(skillInfo, ArmorTypes.tp_infantry_low);
                    if (overtime)
                        dps = dps * ((int)skillInfo.Duration / skillInfo.Refresh);
                    HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + GetDamageColor(dps) + @""">" + dps + "</td>");
                    dps = GetDps(skillInfo, ArmorTypes.tp_infantry_med);
                    if (overtime)
                        dps = dps * ((int)skillInfo.Duration / skillInfo.Refresh);
                    HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + GetDamageColor(dps) + @""">" + dps + "</td>");
                    dps = GetDps(skillInfo, ArmorTypes.tp_infantry_high);
                    if (overtime)
                        dps = dps * ((int)skillInfo.Duration / skillInfo.Refresh);
                    HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + GetDamageColor(dps) + @""">" + dps + "</td>");
                    HtmlInsert(ref index, ref file, "</tr>");

                    HtmlInsert(ref index, ref file, "</table>");
                    HtmlInsert(ref index, ref file, "</td>");
                    #endregion
                    #region HEAVY INFANTRY
                    HtmlInsert(ref index, ref file, "<td>");
                    HtmlInsert(ref index, ref file, @"<table class =""dpsCell"">");
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, @"<th colspan=""3"" class=""armorClass"">Heavy Infantry</th>");
                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, @"<td class=""armorClass"">Low</td>");
                    HtmlInsert(ref index, ref file, @"<td class=""armorClass"">Med</td>");
                    HtmlInsert(ref index, ref file, @"<td class=""armorClass"">High</td>");
                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "<tr>");
                    dps = GetDps(skillInfo, ArmorTypes.tp_infantry_heavy_low);
                    if (overtime)
                        dps = dps * ((int)skillInfo.Duration / skillInfo.Refresh);
                    HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + GetDamageColor(dps) + @""">" + dps + "</td>");
                    dps = GetDps(skillInfo, ArmorTypes.tp_infantry_heavy_med);
                    if (overtime)
                        dps = dps * ((int)skillInfo.Duration / skillInfo.Refresh);
                    HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + GetDamageColor(dps) + @""">" + dps + "</td>");
                    dps = GetDps(skillInfo, ArmorTypes.tp_infantry_heavy_high);
                    if (overtime)
                        dps = dps * ((int)skillInfo.Duration / skillInfo.Refresh);
                    HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + GetDamageColor(dps) + @""">" + dps + "</td>");
                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "</table>");
                    HtmlInsert(ref index, ref file, "</td>"); ;
                    #endregion
                    #region COMMANDERS

                    HtmlInsert(ref index, ref file, "<td>");
                    HtmlInsert(ref index, ref file, @"<table class =""dpsCell"">");
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, @"<th class=""armorClass"">Comm</th>");
                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, @"<td class=""armorClass"">&nbsp;</td>");
                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "<tr>");
                    dps = GetDps(skillInfo, ArmorTypes.tp_commander);
                    if (overtime)
                        dps = dps * ((int)skillInfo.Duration / skillInfo.Refresh);
                    HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + GetDamageColor(dps) + @""">" + dps + "</td>");
                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "</table>");
                    HtmlInsert(ref index, ref file, "</td>");

                    #endregion
                    #region VEHICLES
                    HtmlInsert(ref index, ref file, "<td>");
                    HtmlInsert(ref index, ref file, @"<table class =""dpsCell"">");
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, @"<th colspan=""4"" class=""armorClass"">Vehicles</th>");
                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, @"<td class=""armorClass"">U.Low</td>");
                    HtmlInsert(ref index, ref file, @"<td class=""armorClass"">Low</td>");
                    HtmlInsert(ref index, ref file, @"<td class=""armorClass"">Med</td>");
                    HtmlInsert(ref index, ref file, @"<td class=""armorClass"">High</td>");
                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "<tr>");
                    dps = GetDps(skillInfo, ArmorTypes.tp_air_low);
                    if (overtime)
                        dps = dps * ((int)skillInfo.Duration / skillInfo.Refresh);
                    HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + GetDamageColor(dps) + @""">" + dps + "</td>");
                    dps = GetDps(skillInfo, ArmorTypes.tp_vehicle_low);
                    if (overtime)
                        dps = dps * ((int)skillInfo.Duration / skillInfo.Refresh);
                    HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + GetDamageColor(dps) + @""">" + dps + "</td>");
                    dps = GetDps(skillInfo, ArmorTypes.tp_vehicle_med);
                    if (overtime)
                        dps = dps * ((int)skillInfo.Duration / skillInfo.Refresh);
                    HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + GetDamageColor(dps) + @""">" + dps + "</td>");
                    dps = GetDps(skillInfo, ArmorTypes.tp_vehicle_high);
                    if (overtime)
                        dps = dps * ((int)skillInfo.Duration / skillInfo.Refresh);
                    HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + GetDamageColor(dps) + @""">" + dps + "</td>");

                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "</table>");
                    HtmlInsert(ref index, ref file, "</td>"); ;
                    #endregion
                    #region BUILDINGS
                    HtmlInsert(ref index, ref file, "<td>");
                    HtmlInsert(ref index, ref file, @"<table class =""dpsCell"">");
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, @"<th colspan=""3"" class=""armorClass"">Buildings</th>");
                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, @"<td class=""armorClass"">Low</td>");
                    HtmlInsert(ref index, ref file, @"<td class=""armorClass"">Med</td>");
                    HtmlInsert(ref index, ref file, @"<td class=""armorClass"">High</td>");
                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "<tr>");
                    dps = GetDps(skillInfo, ArmorTypes.tp_building_low);
                    if (overtime)
                        dps = dps * ((int)skillInfo.Duration / skillInfo.Refresh);
                    HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + GetDamageColor(dps) + @""">" + dps + "</td>");
                    dps = GetDps(skillInfo, ArmorTypes.tp_building_med);
                    if (overtime)
                        dps = dps * ((int)skillInfo.Duration / skillInfo.Refresh);
                    HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + GetDamageColor(dps) + @""">" + dps + "</td>");
                    dps = GetDps(skillInfo, ArmorTypes.tp_building_high);
                    if (overtime)
                        dps = dps * ((int)skillInfo.Duration / skillInfo.Refresh);
                    HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + GetDamageColor(dps) + @""">" + dps + "</td>");

                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "</table>");
                    HtmlInsert(ref index, ref file, "</td>"); ;
                    #endregion
                    #region MONSTERS
                    HtmlInsert(ref index, ref file, "<td>");
                    HtmlInsert(ref index, ref file, @"<table class =""dpsCell"">");
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, @"<th colspan=""3"" class=""armorClass"">Monsters</th>");
                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, @"<td class=""armorClass"">Low</td>");
                    HtmlInsert(ref index, ref file, @"<td class=""armorClass"">Med</td>");
                    HtmlInsert(ref index, ref file, @"<td class=""armorClass"">High</td>");
                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "<tr>");
                    dps = GetDps(skillInfo, ArmorTypes.tp_monster_low);
                    if (overtime)
                        dps = dps * ((int)skillInfo.Duration / skillInfo.Refresh);
                    HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + GetDamageColor(dps) + @""">" + dps + "</td>");
                    dps = GetDps(skillInfo, ArmorTypes.tp_monster_med);
                    if (overtime)
                        dps = dps * ((int)skillInfo.Duration / skillInfo.Refresh);
                    HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + GetDamageColor(dps) + @""">" + dps + "</td>");
                    dps = GetDps(skillInfo, ArmorTypes.tp_monster_high);
                    if (overtime)
                        dps = dps * ((int)skillInfo.Duration / skillInfo.Refresh);
                    HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + GetDamageColor(dps) + @""">" + dps + "</td>");

                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "</table>");
                    HtmlInsert(ref index, ref file, "</td>"); ;
                    #endregion
                    #region MORALE
                    HtmlInsert(ref index, ref file, "<td>");
                    HtmlInsert(ref index, ref file, @"<table class =""dpsCell"">");
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, @"<th class=""armorClass"">Morale</th>");
                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "<tr>");
                    HtmlInsert(ref index, ref file, @"<td class=""armorClass"">&nbsp;</td>");
                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "<tr>");
                    dps = GetMoraleDps(skillInfo);
                    if (overtime)
                        dps = dps * ((int)skillInfo.Duration / skillInfo.Refresh);
                    HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + GetMoraleColor(dps) + @""">" + dps + "</td>");
                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "</table>");
                    HtmlInsert(ref index, ref file, "</td>");
                    #endregion 

                    HtmlInsert(ref index, ref file, "</tr>");
                    HtmlInsert(ref index, ref file, "</table>");

                    #endregion
                }
                #endregion
                
                dept++;
            }
            
            if (skillInfo.ChildSkill != null)
            {
                if (skillInfo.ChildSkill.IsValid && dept > 0)
                    HtmlInsert(ref index, ref file, @"<br/>");
                CompileSkillInfoTable(ref index, ref file, skillInfo.ChildSkill,dept);
            } 
        }

		
		public static void CompileSkills(ref int index, ref string file, SkillInfo skillInfo)
		{
            
            if (skillInfo.Spawn != null)
            {
                CompileSummons(ref index, ref file, skillInfo);
                return;
            }
            if (!skillInfo.IsNeeded)
                return;
            
            HtmlInsert(ref index, ref file, "<h4>" + skillInfo.Name + "</h4>");
            CompileToolTips(ref index, ref file, skillInfo); // TODO: SHOULD BE ADDED ?					
            
            HtmlInsert(ref index, ref file, @"<table class=""box"">");
            HtmlInsert(ref index, ref file, @"<tr>");
            HtmlInsert(ref index, ref file, @"<td>");
            HtmlInsert(ref index, ref file, @"<div class=""boxHeader skillHeader"">" + skillInfo.Name + "</div>");
            if (skillInfo.Icon == null || skillInfo.Icon=="")
                HtmlInsert(ref index, ref file, @"<img class=""icon"" src=""../../../images/general/PassiveAbility_icon.jpg"">");
            else HtmlInsert(ref index, ref file, @"<img class=""icon"" src=""../../../images/" + skillInfo.Icon + @".png"">");

            HtmlInsert(ref index, ref file, @"<div class=""innerInfo"">");
            
            CompileSkillInfoTable(ref index, ref file, skillInfo,0);
                                 
            HtmlInsert(ref index, ref file, @"</div>");
            HtmlInsert(ref index, ref file, @"</td></tr>");
            HtmlInsert(ref index, ref file, @"</table>");
            HtmlInsert(ref index, ref file, @"<br/>");
		}


        public static void CompileFirstCol(ref int index, ref string file, string unitName, ArrayList weaponNames)
        {
            HtmlInsert(ref index, ref file, "<td>");
            HtmlInsert(ref index, ref file, @"<table class=""dpsChart firstCol"">");
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, @"<th rowspan=""2"" class=""firstCol""><div>"+unitName+"</div></th>");
            HtmlInsert(ref index, ref file, @"<th>&nbsp;</th>");
            HtmlInsert(ref index, ref file, "</tr>");
            HtmlInsert(ref index, ref file, "<tr><th>&nbsp;</th></tr>");
            for (int i = 0; i < weaponNames.Count; i++)
            {
                HtmlInsert(ref index, ref file, "<tr>");
                HtmlInsert(ref index, ref file, @"<td colspan=""2"" class=""firstCol""><span>"+Translation.Translate((string)weaponNames[i])+@"</span></td>");
                HtmlInsert(ref index, ref file, "</tr>");
            }
            HtmlInsert(ref index, ref file, "</table>");
            HtmlInsert(ref index, ref file, "</td>");
        }

        public static void CompileValuesTable(ref int index, ref string file,string header, string[] subHeaders, ArrayList[] values, bool morale)
        {
            string cssClass ="";
            int colSpan = 0;
            if (subHeaders != null)
            {
                if (subHeaders.Length == 1)
                    cssClass = "singleCol";
                else if (subHeaders.Length == 3)
                {
                    cssClass = "threeCol";
                    colSpan = 3;
                }
                else if (subHeaders.Length == 4)
                {
                    cssClass = "fourCol";
                    colSpan = 4;
                }
            }
            HtmlInsert(ref index, ref file, "<td>");
            HtmlInsert(ref index, ref file, @"<table class=""dpsChart "+cssClass+@""">");
            HtmlInsert(ref index, ref file, "<tr>");
            
            if (colSpan > 0)
                HtmlInsert(ref index, ref file, @"<th colspan="""+colSpan+@""">"+header+@"</th>");
            else HtmlInsert(ref index, ref file, @"<th>" + header + @"</th>");
            HtmlInsert(ref index, ref file, "</tr>");
            HtmlInsert(ref index, ref file, "<tr>");
            if (colSpan == 0)
                HtmlInsert(ref index, ref file, @"<th>&nbsp;</th>");
            else            
                for (int i=0;i<subHeaders.Length;i++)
                    HtmlInsert(ref index, ref file, @"<th>"+subHeaders[i]+"</th>");
            HtmlInsert(ref index, ref file, "</tr>");            
            

            #region Data Row
            int depth = values[0].Count;
            
                for (int i = 0; i < depth; i++)
                {
                    HtmlInsert(ref index, ref file, "<tr>");
                    for (int j = 0; j < subHeaders.Length; j++)
                    {
                        double dps = (double)values[j][i];
                        string colorClass = morale ? GetMoraleColor(dps) : GetDamageColor(dps);
                        HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + colorClass + @""">" + dps + "</td>");
                    }
                    HtmlInsert(ref index, ref file, "</tr>");
                }
            
            
            #endregion
            HtmlInsert(ref index, ref file, "</table>");
            HtmlInsert(ref index, ref file, "</td>");
        }

		public static void CompileWeapon(ref int index, ref string file, WeaponInfo weaponInfo)
		{
            bool develop = true;
            HtmlInsert(ref index,ref file,"<h4>"+Translation.Translate(weaponInfo.Name)+"</h4>");
			HtmlInsert(ref index,ref file,@"<table class=""weaponTable"">");
			HtmlInsert(ref index,ref file,@"<tr><th colspan=""8"" class =""boxHeader weaponHeader"">"+Translation.Translate(weaponInfo.Name)+"</th></tr>");

        #region Upper Part
            
            HtmlInsert(ref index, ref file, "<tr>");
            
            #region Icon
            if (weaponInfo.Icon == "space_marine_icons/upgrade" || weaponInfo.Icon == "")
            {
                if (weaponInfo.MaxRange > 0)
                    HtmlInsert(ref index, ref file, @"<td><img class=""icon"" src=""../../../images/general/default_ranged.jpg""></td>");
                else HtmlInsert(ref index, ref file, @"<td><img class=""icon"" src=""../../../images/general/default_melee.jpg""></td>");
            }
            else HtmlInsert(ref index, ref file, @"<td><img class=""icon"" src=""../../../images/" + weaponInfo.Icon + @".png""></td>");
            #endregion

            HtmlInsert(ref index, ref file, "<td>");
            HtmlInsert(ref index, ref file, @"<table class=""dpsTable"">");
            HtmlInsert(ref index, ref file, "<tr>");
            if (weaponInfo.SetupTime >= 0)
                HtmlInsert(ref index, ref file, @"<th colspan=""7"">Damage per second values</th>");
            else HtmlInsert(ref index, ref file, @"<th colspan=""7"">Explosion damage</th>");
            
            
            HtmlInsert(ref index, ref file, "</tr>");
            if (develop)
                HtmlInsert(ref index, ref file, @"<tr><th colspan=""7"">" + weaponInfo.MinDamage + " - " + weaponInfo.MaxDamage + "</th></tr>");
            #region DpsTables
            double dps = 0.0;
            double ap = 0.0;
            HtmlInsert(ref index, ref file, "<tr>");
            
            #region INFANTRY
            HtmlInsert(ref index, ref file, "<td>");
            HtmlInsert(ref index, ref file, @"<table class =""dpsCell"">");
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, @"<th colspan=""3"" class=""armorClass"">Infantry</th>");
            HtmlInsert(ref index, ref file, "</tr>");
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, @"<td class=""armorClass"">Low</td>");
            HtmlInsert(ref index, ref file, @"<td class=""armorClass"">Med</td>");
            HtmlInsert(ref index, ref file, @"<td class=""armorClass"">High</td>");
            HtmlInsert(ref index, ref file, "</tr>");
            if (develop)
            {
                HtmlInsert(ref index, ref file, "<tr>");
                ap = GetPiercingValue(weaponInfo, ArmorTypes.tp_infantry_low);
                HtmlInsert(ref index, ref file, @"<td>" + ap + "</td>");
                ap = GetPiercingValue(weaponInfo, ArmorTypes.tp_infantry_med);
                HtmlInsert(ref index, ref file, @"<td>" + ap + "</td>");
                ap = GetPiercingValue(weaponInfo, ArmorTypes.tp_infantry_high);
                HtmlInsert(ref index, ref file, @"<td>" + ap + "</td>");
                HtmlInsert(ref index, ref file, "</tr>");
            }
            HtmlInsert(ref index, ref file, "<tr>");
            dps = GetDps(weaponInfo, ArmorTypes.tp_infantry_low);
            HtmlInsert(ref index, ref file, @"<td class=""dpsValue "+GetDamageColor(dps)+ @""">" + dps + "</td>");
            dps = GetDps(weaponInfo, ArmorTypes.tp_infantry_med);
            HtmlInsert(ref index, ref file, @"<td class=""dpsValue "+GetDamageColor(dps)+ @""">" + dps + "</td>");
            dps = GetDps(weaponInfo, ArmorTypes.tp_infantry_high);
            HtmlInsert(ref index, ref file, @"<td class=""dpsValue "+GetDamageColor(dps)+ @""">" + dps + "</td>");
            HtmlInsert(ref index, ref file, "</tr>");

            HtmlInsert(ref index, ref file, "</table>");
            HtmlInsert(ref index, ref file, "</td>");
            #endregion  
            #region HEAVY INFANTRY
            HtmlInsert(ref index, ref file, "<td>");
            HtmlInsert(ref index, ref file, @"<table class =""dpsCell"">");
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, @"<th colspan=""3"" class=""armorClass"">Heavy Infantry</th>");
            HtmlInsert(ref index, ref file, "</tr>");
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, @"<td class=""armorClass"">Low</td>");
            HtmlInsert(ref index, ref file, @"<td class=""armorClass"">Med</td>");
            HtmlInsert(ref index, ref file, @"<td class=""armorClass"">High</td>");
            HtmlInsert(ref index, ref file, "</tr>");
            if (develop)
            {
                HtmlInsert(ref index, ref file, "<tr>");
                ap = GetPiercingValue(weaponInfo, ArmorTypes.tp_infantry_heavy_low);
                HtmlInsert(ref index, ref file, @"<td>" + ap + "</td>");
                ap = GetPiercingValue(weaponInfo, ArmorTypes.tp_infantry_heavy_med);
                HtmlInsert(ref index, ref file, @"<td>" + ap + "</td>");
                ap = GetPiercingValue(weaponInfo, ArmorTypes.tp_infantry_heavy_high);
                HtmlInsert(ref index, ref file, @"<td>" + ap + "</td>");
                HtmlInsert(ref index, ref file, "</tr>");
            }
            HtmlInsert(ref index, ref file, "<tr>");
            dps = GetDps(weaponInfo, ArmorTypes.tp_infantry_heavy_low);
            HtmlInsert(ref index, ref file, @"<td class=""dpsValue "+GetDamageColor(dps)+ @""">" + dps + "</td>");
            dps = GetDps(weaponInfo, ArmorTypes.tp_infantry_heavy_med);
            HtmlInsert(ref index, ref file, @"<td class=""dpsValue "+GetDamageColor(dps)+ @""">" + dps + "</td>");
            dps = GetDps(weaponInfo, ArmorTypes.tp_infantry_heavy_high);
            HtmlInsert(ref index, ref file, @"<td class=""dpsValue "+GetDamageColor(dps)+ @""">" + dps + "</td>");
            HtmlInsert(ref index, ref file, "</tr>");
            HtmlInsert(ref index, ref file, "</table>");
            HtmlInsert(ref index, ref file, "</td>"); ;
            #endregion
            #region COMMANDERS
          
            HtmlInsert(ref index, ref file, "<td>");
            HtmlInsert(ref index, ref file, @"<table class =""dpsCell"">");
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, @"<th class=""armorClass"">Comm</th>");
            HtmlInsert(ref index, ref file, "</tr>");
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, @"<td class=""armorClass"">&nbsp;</td>"); 
            HtmlInsert(ref index, ref file, "</tr>");
            if (develop)
            {
                HtmlInsert(ref index, ref file, "<tr>");
                ap = GetPiercingValue(weaponInfo, ArmorTypes.tp_commander);
                HtmlInsert(ref index, ref file, @"<td>" + ap + "</td>");               
                HtmlInsert(ref index, ref file, "</tr>");
            }
            HtmlInsert(ref index, ref file, "<tr>");
            dps = GetDps(weaponInfo, ArmorTypes.tp_commander);
            HtmlInsert(ref index, ref file, @"<td class=""dpsValue "+GetDamageColor(dps)+ @""">" + dps + "</td>");
            HtmlInsert(ref index, ref file, "</tr>");
            HtmlInsert(ref index, ref file, "</table>");
            HtmlInsert(ref index, ref file, "</td>");
           
            #endregion
            #region VEHICLES
            HtmlInsert(ref index, ref file, "<td>");
            HtmlInsert(ref index, ref file, @"<table class =""dpsCell"">");
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, @"<th colspan=""4"" class=""armorClass"">Vehicles</th>");
            HtmlInsert(ref index, ref file, "</tr>");
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, @"<td class=""armorClass"">U.Low</td>");
            HtmlInsert(ref index, ref file, @"<td class=""armorClass"">Low</td>");
            HtmlInsert(ref index, ref file, @"<td class=""armorClass"">Med</td>");
            HtmlInsert(ref index, ref file, @"<td class=""armorClass"">High</td>");
            HtmlInsert(ref index, ref file, "</tr>");
            if (develop)
            {
                HtmlInsert(ref index, ref file, "<tr>");
                ap = GetPiercingValue(weaponInfo, ArmorTypes.tp_air_low);
                HtmlInsert(ref index, ref file, @"<td>" + ap + "</td>");
                ap = GetPiercingValue(weaponInfo, ArmorTypes.tp_vehicle_low);
                HtmlInsert(ref index, ref file, @"<td>" + ap + "</td>");
                ap = GetPiercingValue(weaponInfo, ArmorTypes.tp_vehicle_med);
                HtmlInsert(ref index, ref file, @"<td>" + ap + "</td>");
                ap = GetPiercingValue(weaponInfo, ArmorTypes.tp_vehicle_high);
                HtmlInsert(ref index, ref file, @"<td>" + ap + "</td>");
                HtmlInsert(ref index, ref file, "</tr>");
            }
            HtmlInsert(ref index, ref file, "<tr>");
            dps = GetDps(weaponInfo, ArmorTypes.tp_air_low);
            HtmlInsert(ref index, ref file, @"<td class=""dpsValue "+GetDamageColor(dps)+ @""">" + dps + "</td>");
            dps = GetDps(weaponInfo, ArmorTypes.tp_vehicle_low);
            HtmlInsert(ref index, ref file, @"<td class=""dpsValue "+GetDamageColor(dps)+ @""">" + dps + "</td>");
            dps = GetDps(weaponInfo, ArmorTypes.tp_vehicle_med);
            HtmlInsert(ref index, ref file, @"<td class=""dpsValue "+GetDamageColor(dps)+ @""">" + dps + "</td>");
            dps = GetDps(weaponInfo, ArmorTypes.tp_vehicle_high);
            HtmlInsert(ref index, ref file, @"<td class=""dpsValue "+GetDamageColor(dps)+ @""">" + dps + "</td>");

            HtmlInsert(ref index, ref file, "</tr>");
            HtmlInsert(ref index, ref file, "</table>");
            HtmlInsert(ref index, ref file, "</td>"); ;
            #endregion
            #region AIRCRAFTS

            HtmlInsert(ref index, ref file, "<td>");
            HtmlInsert(ref index, ref file, @"<table class =""dpsCell"">");
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, @"<th class=""armorClass"">Air</th>");
            HtmlInsert(ref index, ref file, "</tr>");
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, @"<td class=""armorClass"">&nbsp;</td>");
            HtmlInsert(ref index, ref file, "</tr>");
            if (develop)
            {
                HtmlInsert(ref index, ref file, "<tr>");
                ap = GetPiercingValue(weaponInfo, ArmorTypes.tp_air_med);
                HtmlInsert(ref index, ref file, @"<td>" + ap + "</td>");
                HtmlInsert(ref index, ref file, "</tr>");
            }
            HtmlInsert(ref index, ref file, "<tr>");
            dps = GetDps(weaponInfo, ArmorTypes.tp_air_med);
            HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + GetDamageColor(dps) + @""">" + dps + "</td>");
            HtmlInsert(ref index, ref file, "</tr>");
            HtmlInsert(ref index, ref file, "</table>");
            HtmlInsert(ref index, ref file, "</td>");

            #endregion
            #region BUILDINGS
            HtmlInsert(ref index, ref file, "<td>");
            HtmlInsert(ref index, ref file, @"<table class =""dpsCell"">");
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, @"<th colspan=""3"" class=""armorClass"">Buildings</th>");
            HtmlInsert(ref index, ref file, "</tr>");
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, @"<td class=""armorClass"">Low</td>");
            HtmlInsert(ref index, ref file, @"<td class=""armorClass"">Med</td>");
            HtmlInsert(ref index, ref file, @"<td class=""armorClass"">High</td>");
            HtmlInsert(ref index, ref file, "</tr>");
            if (develop)
            {
                HtmlInsert(ref index, ref file, "<tr>");
                ap = GetPiercingValue(weaponInfo, ArmorTypes.tp_building_low);
                HtmlInsert(ref index, ref file, @"<td>" + ap + "</td>");
                ap = GetPiercingValue(weaponInfo, ArmorTypes.tp_building_med);
                HtmlInsert(ref index, ref file, @"<td>" + ap + "</td>");
                ap = GetPiercingValue(weaponInfo, ArmorTypes.tp_building_high);
                HtmlInsert(ref index, ref file, @"<td>" + ap + "</td>");
                HtmlInsert(ref index, ref file, "</tr>");
            }
            HtmlInsert(ref index, ref file, "<tr>");
            dps = GetDps(weaponInfo, ArmorTypes.tp_building_low);
            HtmlInsert(ref index, ref file, @"<td class=""dpsValue "+GetDamageColor(dps)+ @""">" + dps + "</td>");
            dps = GetDps(weaponInfo, ArmorTypes.tp_building_med);
            HtmlInsert(ref index, ref file, @"<td class=""dpsValue "+GetDamageColor(dps)+ @""">" + dps + "</td>");
            dps = GetDps(weaponInfo, ArmorTypes.tp_building_high);
            HtmlInsert(ref index, ref file, @"<td class=""dpsValue "+GetDamageColor(dps)+ @""">" + dps + "</td>");

            HtmlInsert(ref index, ref file, "</tr>");
            HtmlInsert(ref index, ref file, "</table>");
            HtmlInsert(ref index, ref file, "</td>"); ;
            #endregion
            #region MONSTERS
            HtmlInsert(ref index, ref file, "<td>");
            HtmlInsert(ref index, ref file, @"<table class =""dpsCell"">");
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, @"<th colspan=""3"" class=""armorClass"">Monsters</th>");
            HtmlInsert(ref index, ref file, "</tr>");
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, @"<td class=""armorClass"">Low</td>");
            HtmlInsert(ref index, ref file, @"<td class=""armorClass"">Med</td>");
            HtmlInsert(ref index, ref file, @"<td class=""armorClass"">High</td>");
            HtmlInsert(ref index, ref file, "</tr>");
            if (develop)
            {
                HtmlInsert(ref index, ref file, "<tr>");
                ap = GetPiercingValue(weaponInfo, ArmorTypes.tp_monster_low);
                HtmlInsert(ref index, ref file, @"<td>" + ap + "</td>");
                ap = GetPiercingValue(weaponInfo, ArmorTypes.tp_monster_med);
                HtmlInsert(ref index, ref file, @"<td>" + ap + "</td>");
                ap = GetPiercingValue(weaponInfo, ArmorTypes.tp_monster_high);
                HtmlInsert(ref index, ref file, @"<td>" + ap + "</td>");
                HtmlInsert(ref index, ref file, "</tr>");
            }
            HtmlInsert(ref index, ref file, "<tr>");
            dps = GetDps(weaponInfo, ArmorTypes.tp_monster_low);
            HtmlInsert(ref index, ref file, @"<td class=""dpsValue "+GetDamageColor(dps)+ @""">" + dps + "</td>");
            dps = GetDps(weaponInfo, ArmorTypes.tp_monster_med);
            HtmlInsert(ref index, ref file, @"<td class=""dpsValue "+GetDamageColor(dps)+ @""">" + dps + "</td>");
            dps = GetDps(weaponInfo, ArmorTypes.tp_monster_high);
            HtmlInsert(ref index, ref file, @"<td class=""dpsValue "+GetDamageColor(dps)+ @""">" + dps + "</td>");

            HtmlInsert(ref index, ref file, "</tr>");
            HtmlInsert(ref index, ref file, "</table>");
            HtmlInsert(ref index, ref file, "</td>"); ;
            #endregion
            #region MORALE
            HtmlInsert(ref index, ref file, "<td>");
            HtmlInsert(ref index, ref file, @"<table class =""dpsCell"">");
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, @"<th class=""armorClass"">Morale</th>");
            HtmlInsert(ref index, ref file, "</tr>");
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, @"<td class=""armorClass"">&nbsp;</td>");
            HtmlInsert(ref index, ref file, "</tr>");
            if (develop)
            {
                HtmlInsert(ref index, ref file, "<tr>");
                ap = weaponInfo.MoraleDamage;
                HtmlInsert(ref index, ref file, @"<td>" + ap + "</td>");
                
                HtmlInsert(ref index, ref file, "</tr>");
            }
            HtmlInsert(ref index, ref file, "<tr>");
            dps = GetMoraleDps(weaponInfo);
            HtmlInsert(ref index, ref file, @"<td class=""dpsValue " + GetMoraleColor(dps) + @""">" + dps + "</td>");
            HtmlInsert(ref index, ref file, "</tr>");
            HtmlInsert(ref index, ref file, "</table>");
            HtmlInsert(ref index, ref file, "</td>");
            #endregion

            HtmlInsert(ref index, ref file, "</tr>");
            HtmlInsert(ref index, ref file, "</table>");
            HtmlInsert(ref index, ref file, "</td>");
            HtmlInsert(ref index, ref file, "</tr>");
            #endregion
            
        #endregion
            
        #region Lower part
           
            HtmlInsert(ref index, ref file, "<tr>");
            HtmlInsert(ref index, ref file, @"<td colspan=""2"" align=""center"">");
            HtmlInsert(ref index, ref file, @"<table class=""weaponInfoTable"">");
            HtmlInsert(ref index, ref file, @"<colgroup class=""left"" span=""2""></colgroup>");
            HtmlInsert(ref index, ref file, @"<colgroup class=""middle"" span=""2""></colgroup>");
            HtmlInsert(ref index, ref file, @"<colgroup class=""right"" span=""2""></colgroup>");
         
        
            HtmlInsert(ref index, ref file, @"<tr>");
            HtmlInsert(ref index, ref file, @"<td>Cost</td>");
            HtmlInsert(ref index, ref file, @"<td>");
            if (weaponInfo.RequisitionCost == 0 && weaponInfo.PowerCost == 0 && weaponInfo.BuildTime == 0)
                HtmlInsert(ref index, ref file, @"--");
            else
            {
                HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_requisition.gif""><span class=""cost"">"+weaponInfo.RequisitionCost+@"</span>");
                HtmlInsert(ref index, ref file, @"<img class=""resourceIcon"" src=""../../../images/resources/Resource_power.gif""><span class=""cost"">"+weaponInfo.PowerCost+"</span>");
            }
    
            HtmlInsert(ref index, ref file, @"</td>");
            HtmlInsert(ref index, ref file, @"<td>Range</td>");
            HtmlInsert(ref index, ref file, @"<td>");
            if (weaponInfo.MinRange>0)
                HtmlInsert(ref index, ref file, weaponInfo.MinRange+" - ");
            HtmlInsert(ref index, ref file, weaponInfo.MaxRange.ToString());
            if (weaponInfo.CanHitGround)
            {
                if (weaponInfo.CanHitAir)
                    HtmlInsert(ref index, ref file, " - Ground/Air ");
                else HtmlInsert(ref index, ref file, " - Ground only ");
            }
            else HtmlInsert(ref index, ref file, " - Air only ");
                HtmlInsert(ref index, ref file, @"</td>");
            HtmlInsert(ref index, ref file, @"<td>Setup time</td>");
            if (weaponInfo.SetupTime >0)
                HtmlInsert(ref index, ref file, @"<td>"+weaponInfo.SetupTime.ToString()+" seconds.</td>");
            else HtmlInsert(ref index, ref file, @"<td>--</td>");

            HtmlInsert(ref index, ref file, @"</tr>");

            HtmlInsert(ref index, ref file, @"<tr>");
            HtmlInsert(ref index, ref file, @"<td>Build time</td>");
            if (weaponInfo.BuildTime >0)
                HtmlInsert(ref index, ref file, @"<td>"+weaponInfo.BuildTime.ToString()+" seconds.</td>");
            else HtmlInsert(ref index, ref file, @"<td>--</td>");
            HtmlInsert(ref index, ref file, @"<td>Accuracy</td>");
            HtmlInsert(ref index, ref file, @"<td>"+(weaponInfo.Accuracy*100)+"%</td>");
            HtmlInsert(ref index, ref file, @"<td>Reload time</td>");
            HtmlInsert(ref index, ref file, @"<td>"+weaponInfo.ReloadTime+" seconds.</td>");
            HtmlInsert(ref index, ref file, @"</tr>");

            HtmlInsert(ref index, ref file, @"<tr>");
            HtmlInsert(ref index, ref file, @"<td>Area of Effect</td>");
            HtmlInsert(ref index, ref file, @"<td>"+weaponInfo.AOERadius+"</td>");
            HtmlInsert(ref index, ref file, @"<td>FOTM accuracy</td>");
            if (!(weaponInfo.Parent is BuildingInfo) )
                HtmlInsert(ref index, ref file, @"<td>"+Math.Max(0,weaponInfo.Accuracy-weaponInfo.AccuracyReduction)*100+"%</td>");
            else HtmlInsert(ref index, ref file, @"<td>--</td>");
            HtmlInsert(ref index, ref file, @"<td>Throw force</td>");
            if(weaponInfo.MinForce ==0 && weaponInfo.MaxForce==0)
                HtmlInsert(ref index, ref file, @"<td>--</td>");
            else HtmlInsert(ref index, ref file, @"<td>"+weaponInfo.MinForce+" - "+weaponInfo.MaxForce+"</td>");
            HtmlInsert(ref index, ref file, @"</tr>");
            
            #region Requirements
            if (weaponInfo.Requirements != null && weaponInfo.Requirements.Count > 0)
            {
                HtmlInsert(ref index, ref file, @"<tr>");
                HtmlInsert(ref index, ref file, @"<td>Requirements</td>");
                HtmlInsert(ref index, ref file, @"<td colspan=""5"">");
                HtmlInsert(ref index, ref file, CompileRequirements(weaponInfo.Requirements));
                HtmlInsert(ref index, ref file, @"</td>");
                HtmlInsert(ref index, ref file, @"</tr>");
            }
            #endregion

            #region Upgrade Infos
            if (weaponInfo.Parent is BuildableInfo && weaponInfo.WeaponIndex > 1)
            {
                HtmlInsert(ref index, ref file, @"<tr>");
                HtmlInsert(ref index, ref file, @"<td>Upgrade Type</td>");
                HtmlInsert(ref index, ref file, @"<td colspan=""5"">");

                BuildableInfo parentInfo = weaponInfo.Parent as BuildableInfo;

                WeaponHardPointInfo whpInfo = parentInfo.WeaponHardPoints[weaponInfo.HardPoint] as WeaponHardPointInfo;

                
                WeaponInfo oldWeapon = null;
                WeaponUpgradeTypes upgradeType = WeaponUpgradeTypes.New;

                if (weaponInfo.BuildTime == 0) // An upgrade
                {
                    foreach (WeaponInfo whInfo in whpInfo.Weapons)
                        if (whInfo.WeaponIndex == weaponInfo.WeaponIndex - 1 && !whInfo.IsDummyWeapon())
                        {
                            oldWeapon = whInfo;
                            upgradeType = WeaponUpgradeTypes.Upgrade;
                            break;
                        }
                }
                else if (whpInfo.Weapons[0] != null && whpInfo.Weapons[0] != weaponInfo && ((WeaponInfo)whpInfo.Weapons[0]).WeaponIndex == 1 && !((WeaponInfo)whpInfo.Weapons[0]).IsDummyWeapon())
                {
                    upgradeType = WeaponUpgradeTypes.Replacement;
                    oldWeapon = whpInfo.Weapons[0] as WeaponInfo;
                }
                
                switch(upgradeType)
                {
                    case WeaponUpgradeTypes.New:
                        HtmlInsert(ref index, ref file,"Additional Weapon");
                        break;
                    case WeaponUpgradeTypes.Upgrade:
                        HtmlInsert(ref index, ref file, "Upgrades "+Translation.Translate(oldWeapon.Name));
                        break;
                    case WeaponUpgradeTypes.Replacement:
                        HtmlInsert(ref index, ref file, "Replaces " + Translation.Translate(oldWeapon.Name));
                        break;
                }
                

                HtmlInsert(ref index, ref file, @"</td>");
                HtmlInsert(ref index, ref file, @"</tr>");
            }
            #endregion
            
            HtmlInsert(ref index, ref file, "</table>");
            HtmlInsert(ref index, ref file, "</td>");
            HtmlInsert(ref index, ref file, "</tr>");

        #endregion
            HtmlInsert(ref index,ref file,@"</table><br>");
           
		}

        public static void CompileDpsChart(string template)
        {
            ArrayList RacesList = new ArrayList();
            ArrayList list = DataDumper.Squads;
            string file = string.Copy(template);
            StreamWriter writer = new StreamWriter(File.Create(Path.Combine(DataPath.OutputPath, "content/dpsChart.htm")));
            file = Regex.Replace(file, "href=\"style.css\"", "href=\"../style.css\"");
                                     
            file = Regex.Replace(file, @"<div id=""navigation""></div>","");
            file = Regex.Replace(file, @"<div id=""content""></div>", "<div></div>");
            //Match m = Regex.Match(file, @"</div>[^<div>]*<div\sid=""content"">");
            
		    Match m = Regex.Match(file,@"</div>[^<div>]*<div\sid=""footer"">");
            if (m.Success)
            {
                int index = m.Index;               

                HtmlInsert(ref index, ref file, @"<h2 class=""pageTitle"">Dps Chart</h2>");
                HtmlInsert(ref index, ref file, "<hr>");

                HtmlInsert(ref index, ref file, @"<table class=""tableContainer"">");
                
                foreach (SquadInfo squadInfo in list)
                {
                    if (!RacesList.Contains(squadInfo.Race))
                    {
                        RacesList.Add(squadInfo.Race);
                        string raceName = "";
                        for (int i = 0; i < 9; i++)
                            if (Races[i, 0] == squadInfo.Race)
                                raceName = Races[i, 1];
                        HtmlInsert(ref index, ref file, @"<tr><td colspan=""8""><h3>"+raceName+"</h3></td></tr>");
                    }

                    ArrayList weaponNames=new ArrayList();
                    ArrayList infLow = new ArrayList();
                    ArrayList infMed = new ArrayList();
                    ArrayList infHigh = new ArrayList();
                    ArrayList hInfLow = new ArrayList();
                    ArrayList hInfMed = new ArrayList();
                    ArrayList hInfHigh = new ArrayList();
                    ArrayList comm = new ArrayList();
                    ArrayList vehULow = new ArrayList();
                    ArrayList vehLow = new ArrayList();
                    ArrayList vehMed = new ArrayList();
                    ArrayList vehHigh = new ArrayList();
                    
                    //ArrayList airMed = new ArrayList();
                    
                    ArrayList monsLow = new ArrayList();
                    ArrayList monsMed = new ArrayList();
                    ArrayList monsHigh = new ArrayList();
                    ArrayList bldLow = new ArrayList();
                    ArrayList bldMed = new ArrayList();
                    ArrayList bldHigh = new ArrayList();
                    ArrayList morale = new ArrayList();

                    MainForm.BarInc(Bars.Html);
                    UnitInfo entrenchedUnit = null;
                    
                    if (squadInfo.Unit.Extensions.ContainsKey("entrench"))
                        entrenchedUnit = ((EntrenchInfo)squadInfo.Unit.Extensions["entrench"]).EntrenchedUnit;
                    
                    #region  SQUAD WEAPONS
                    
                    int weaponCount = 0;
                    weaponCount += squadInfo.Unit.WeaponHardPoints.Count;
                    if (entrenchedUnit != null)
                        weaponCount += entrenchedUnit.WeaponHardPoints.Count;
                    if (entrenchedUnit != null)
                        weaponCount += entrenchedUnit.WeaponHardPoints.Count;

                    if (weaponCount > 0)
                    {                        
                        
                        foreach (WeaponHardPointInfo whpInfo in squadInfo.Unit.WeaponHardPoints.Values)
                        {
                            foreach (WeaponInfo weapon in whpInfo.Weapons)
                                if (!weapon.IsDummyWeapon())
                                {
                                    #region lists
                                    weaponNames.Add( weapon.Name);
                                    double dps = 0.0;
                                    dps = GetDps(weapon, ArmorTypes.tp_infantry_low);
                                    infLow.Add(dps);
                                    dps = GetDps(weapon, ArmorTypes.tp_infantry_med);
                                    infMed.Add(dps);
                                    dps = GetDps(weapon, ArmorTypes.tp_infantry_high);
                                    infHigh.Add(dps);
                                    dps = GetDps(weapon, ArmorTypes.tp_infantry_heavy_low);
                                    hInfLow.Add(dps);
                                    dps = GetDps(weapon, ArmorTypes.tp_infantry_heavy_med);
                                    hInfMed.Add(dps);
                                    dps = GetDps(weapon, ArmorTypes.tp_infantry_heavy_high);
                                    hInfHigh.Add(dps);
                                    dps = GetDps(weapon, ArmorTypes.tp_commander);
                                    comm.Add(dps);
                                    dps = GetDps(weapon, ArmorTypes.tp_air_low);
                                    vehULow.Add(dps);
                                    dps = GetDps(weapon, ArmorTypes.tp_vehicle_low);
                                    vehLow.Add(dps);
                                    dps = GetDps(weapon, ArmorTypes.tp_vehicle_med);
                                    vehMed.Add(dps);
                                    dps = GetDps(weapon, ArmorTypes.tp_vehicle_high);
                                    vehHigh.Add(dps);
                                    //dps = GetDps(weapon, ArmorTypes.tp_air_med);
                                    //airMed.Add(dps);
                                    dps = GetDps(weapon, ArmorTypes.tp_building_low);
                                    bldLow.Add(dps);
                                    dps = GetDps(weapon, ArmorTypes.tp_building_med);
                                    bldMed.Add(dps);
                                    dps = GetDps(weapon, ArmorTypes.tp_building_high);
                                    bldHigh.Add(dps);
                                    dps = GetDps(weapon, ArmorTypes.tp_monster_low);
                                    monsLow.Add(dps);
                                    dps = GetDps(weapon, ArmorTypes.tp_monster_med);
                                    monsMed.Add(dps);
                                    dps = GetDps(weapon, ArmorTypes.tp_monster_high);
                                    monsHigh.Add(dps);
                                    dps = GetMoraleDps(weapon);
                                    morale.Add(dps);
                                    #endregion
                                }
                                    
                                
                        }
                        if (entrenchedUnit != null)
                        {
                            foreach (WeaponHardPointInfo whpInfo in entrenchedUnit.WeaponHardPoints.Values)
                            {
                                foreach (WeaponInfo weapon in whpInfo.Weapons)
                                    if (!weapon.IsDummyWeapon())
                                    {
                                        #region lists
                                        weaponNames.Add(weapon.Name);
                                        double dps = 0.0;
                                        dps = GetDps(weapon, ArmorTypes.tp_infantry_low);
                                        infLow.Add(dps);
                                        dps = GetDps(weapon, ArmorTypes.tp_infantry_med);
                                        infMed.Add(dps);
                                        dps = GetDps(weapon, ArmorTypes.tp_infantry_high);
                                        infHigh.Add(dps);
                                        dps = GetDps(weapon, ArmorTypes.tp_infantry_heavy_low);
                                        hInfLow.Add(dps);
                                        dps = GetDps(weapon, ArmorTypes.tp_infantry_heavy_med);
                                        hInfMed.Add(dps);
                                        dps = GetDps(weapon, ArmorTypes.tp_infantry_heavy_high);
                                        hInfHigh.Add(dps);
                                        dps = GetDps(weapon, ArmorTypes.tp_commander);
                                        comm.Add(dps);
                                        dps = GetDps(weapon, ArmorTypes.tp_air_low);
                                        vehULow.Add(dps);
                                        dps = GetDps(weapon, ArmorTypes.tp_vehicle_low);
                                        vehLow.Add(dps);
                                        dps = GetDps(weapon, ArmorTypes.tp_vehicle_med);
                                        vehMed.Add(dps);
                                        dps = GetDps(weapon, ArmorTypes.tp_vehicle_high);
                                        vehHigh.Add(dps);
                                        //dps = GetDps(weapon, ArmorTypes.tp_air_med);
                                        //airMed.Add(dps);
                                        dps = GetDps(weapon, ArmorTypes.tp_building_low);
                                        bldLow.Add(dps);
                                        dps = GetDps(weapon, ArmorTypes.tp_building_med);
                                        bldMed.Add(dps);
                                        dps = GetDps(weapon, ArmorTypes.tp_building_high);
                                        bldHigh.Add(dps);
                                        dps = GetDps(weapon, ArmorTypes.tp_monster_low);
                                        monsLow.Add(dps);
                                        dps = GetDps(weapon, ArmorTypes.tp_monster_med);
                                        monsMed.Add(dps);
                                        dps = GetDps(weapon, ArmorTypes.tp_monster_high);
                                        monsHigh.Add(dps);
                                        dps = GetMoraleDps(weapon);
                                        morale.Add(dps);
                                        #endregion
                                    }
                                    
                            }
                        }
                        HtmlInsert(ref index, ref file, "<tr>");

                        CompileFirstCol(ref index, ref file, squadInfo.Unit.Name, weaponNames);
                        CompileValuesTable(ref index, ref file, "Infantry", new string[] { "low", "Med", "High" }, new ArrayList[] { infLow, infMed, infHigh }, false);
                        CompileValuesTable(ref index, ref file, "Heavy Infantry", new string[] { "low", "Med", "High" }, new ArrayList[] { hInfLow, hInfMed, hInfHigh }, false);
                        CompileValuesTable(ref index, ref file, "Comm", new string[] { "Comm" }, new ArrayList[] { comm }, false);
                        CompileValuesTable(ref index, ref file, "Vehicles", new string[] { "U.Low", "low", "Med", "High" }, new ArrayList[] { vehULow, vehLow, vehMed, vehHigh }, false);
                        //CompileValuesTable(ref index, ref file, "Air", new string[] { "Air" }, new ArrayList[] { airMed }, false);
                        CompileValuesTable(ref index, ref file, "Monsters", new string[] { "low", "Med", "High" }, new ArrayList[] { monsLow, monsMed, monsHigh }, false);
                        CompileValuesTable(ref index, ref file, "Buildings", new string[] { "low", "Med", "High" }, new ArrayList[] { bldLow, bldMed, bldHigh }, false);
                        CompileValuesTable(ref index, ref file, "Morale", new string[] { "morale" }, new ArrayList[] { morale }, true);

                        HtmlInsert(ref index, ref file, "</tr>");

                        weaponNames.Clear();
                        infLow.Clear();
                        infMed.Clear();
                        infHigh.Clear();
                        hInfLow.Clear();
                        hInfMed.Clear();
                        hInfHigh.Clear();
                        comm.Clear();
                        vehULow.Clear();
                        vehLow.Clear();
                        vehMed.Clear();
                        vehHigh.Clear();
                        //airMed.Clear();
                        monsLow.Clear();
                        monsMed.Clear();
                        monsHigh.Clear();
                        bldLow.Clear();
                        bldMed.Clear();
                        bldHigh.Clear();
                        morale.Clear();
                    }
                    
                    

                    #endregion
                    

                    #region  Leader WEAPONS
                    if (squadInfo.Leaders != null)
                    {
                        foreach (UnitInfo leaderInfo in squadInfo.Leaders)
                        {                            
                            
                            if (leaderInfo.WeaponHardPoints.Count > 0)
                            {

                                foreach (WeaponHardPointInfo whpInfo in leaderInfo.WeaponHardPoints.Values)
                                {
                                    if (whpInfo.Weapons.Count > 0)
                                    {
                                        foreach (WeaponInfo weapon in whpInfo.Weapons)
                                            if (!weapon.IsDummyWeapon())
                                            {
                                                weaponNames.Add(weapon.Name);
                                                double dps = 0.0;
                                                dps = GetDps(weapon, ArmorTypes.tp_infantry_low);
                                                infLow.Add(dps);
                                                dps = GetDps(weapon, ArmorTypes.tp_infantry_med);
                                                infMed.Add(dps);
                                                dps = GetDps(weapon, ArmorTypes.tp_infantry_high);
                                                infHigh.Add(dps);
                                                dps = GetDps(weapon, ArmorTypes.tp_infantry_heavy_low);
                                                hInfLow.Add(dps);
                                                dps = GetDps(weapon, ArmorTypes.tp_infantry_heavy_med);
                                                hInfMed.Add(dps);
                                                dps = GetDps(weapon, ArmorTypes.tp_infantry_heavy_high);
                                                hInfHigh.Add(dps);
                                                dps = GetDps(weapon, ArmorTypes.tp_commander);
                                                comm.Add(dps);
                                                dps = GetDps(weapon, ArmorTypes.tp_air_low);
                                                vehULow.Add(dps);
                                                dps = GetDps(weapon, ArmorTypes.tp_vehicle_low);
                                                vehLow.Add(dps);
                                                dps = GetDps(weapon, ArmorTypes.tp_vehicle_med);
                                                vehMed.Add(dps);
                                                //dps = GetDps(weapon, ArmorTypes.tp_air_med);
                                                //airMed.Add(dps);
                                                dps = GetDps(weapon, ArmorTypes.tp_vehicle_high);
                                                vehHigh.Add(dps);
                                                dps = GetDps(weapon, ArmorTypes.tp_building_low);
                                                bldLow.Add(dps);
                                                dps = GetDps(weapon, ArmorTypes.tp_building_med);
                                                bldMed.Add(dps);
                                                dps = GetDps(weapon, ArmorTypes.tp_building_high);
                                                bldHigh.Add(dps);
                                                dps = GetDps(weapon, ArmorTypes.tp_monster_low);
                                                monsLow.Add(dps);
                                                dps = GetDps(weapon, ArmorTypes.tp_monster_med);
                                                monsMed.Add(dps);
                                                dps = GetDps(weapon, ArmorTypes.tp_monster_high);
                                                monsHigh.Add(dps);
                                                dps = GetMoraleDps(weapon);
                                                morale.Add(dps);
                                            }
                                    }
                                    
                                }
                                HtmlInsert(ref index, ref file, "<tr>");
                               
                                    CompileFirstCol(ref index, ref file, leaderInfo.Name, weaponNames);
                                    CompileValuesTable(ref index, ref file, "Infantry", new string[] { "low", "Med", "High" }, new ArrayList[] { infLow, infMed, infHigh }, false);
                                    CompileValuesTable(ref index, ref file, "Heavy Infantry", new string[] { "low", "Med", "High" }, new ArrayList[] { hInfLow, hInfMed, hInfHigh }, false);
                                    CompileValuesTable(ref index, ref file, "Comm", new string[] { "Comm" }, new ArrayList[] { comm }, false);
                                    CompileValuesTable(ref index, ref file, "Vehicles", new string[] { "U.Low", "low", "Med", "High" }, new ArrayList[] { vehULow, vehLow, vehMed, vehHigh }, false);
                                    //CompileValuesTable(ref index, ref file, "Air", new string[] { "Air" }, new ArrayList[] { airMed }, false);
                                    CompileValuesTable(ref index, ref file, "Monsters", new string[] { "low", "Med", "High" }, new ArrayList[] { monsLow, monsMed, monsHigh }, false);
                                    CompileValuesTable(ref index, ref file, "Buildings", new string[] { "low", "Med", "High" }, new ArrayList[] { bldLow, bldMed, bldHigh }, false);
                                    CompileValuesTable(ref index, ref file, "Morale", new string[] { "morale" }, new ArrayList[] { morale }, true);
                               
                                
                                HtmlInsert(ref index, ref file, "</tr>");

                                weaponNames.Clear();
                                infLow.Clear();
                                infMed.Clear();
                                infHigh.Clear();
                                hInfLow.Clear();
                                hInfMed.Clear();
                                hInfHigh.Clear();
                                comm.Clear();
                                vehULow.Clear();
                                vehLow.Clear();
                                vehMed.Clear();
                                vehHigh.Clear();
                                //airMed.Clear();
                                monsLow.Clear();
                                monsMed.Clear();
                                monsHigh.Clear();
                                bldLow.Clear();
                                bldMed.Clear();
                                bldHigh.Clear();
                                morale.Clear();
                            }
                                                        
                        }
                    }
                    #endregion


                }
                HtmlInsert(ref index, ref file, "</table>");
            }
            file = file.Replace("<nl>", Environment.NewLine);
            writer.Write(file);
            writer.Close();
        }
        
        public static string GetDamageColor(double dps)
        {
            if (dps >= 80)
                return "extremelyHighDamage";
            if (dps >= 40)
                return "veryHighDamage";
            if (dps >= 20)
                return "highDamage";
            if (dps >= 10)
                return "mediumDamage";
            if (dps >= 4)
                return "lowDamage";            
            return "veryLowDamage";
        }
        public static string GetMoraleColor(double dps)
        {
            if (dps > 20)
                return "highMoraleDamage";
            if (dps > 5)
                return "mediumMoraleDamage";
            return "lowMoraleDamage";
        }
        private static double GetPiercingValue(WeaponInfo info, ArmorTypes armorType )
        {
            foreach (ArmorPiercing ap in info.ArmorPiercingValues.Values)
            {
                if (ap.ArmorType == armorType)    
                    return ap.PiercingValue;    
            }
            return 0.0;
        }
        private static double GetPiercingValue(SkillInfo info, ArmorTypes armorType)
        {
            
            double value = 0.0;
            ArrayList filter = new ArrayList(info.Filter.Values);
            if (filter.Contains(armorType))
                value = info.BasePiercing;
            foreach (ArmorPiercing ap in info.ArmorPiercingValues.Values)
            {
                if (ap.ArmorType == armorType)
                {
                    value = ap.PiercingValue;
                    break;
                }
            }
           
            return value ;
        }
        public static double GetDps(SkillInfo info, ArmorTypes armorType)
		{
            if (info.Refresh < info.Duration)
                return GetDps(info.MinDamage, info.MaxDamage, info.MinDamageValue, GetPiercingValue(info, armorType), 1, info.Refresh,0);
            return GetDps(info.MinDamage, info.MaxDamage, info.MinDamageValue, GetPiercingValue(info, armorType), 1, 1,0);
            
        }
        public static double GetDps(WeaponInfo info, ArmorTypes armorType)
		{
            return GetDps(info.MinDamage, info.MaxDamage, info.MinDamageValue, GetPiercingValue(info, armorType), info.Accuracy, info.ReloadTime,info.SetupTime);			
		}

		public static double GetDps( double minDamage,double maxDamage,double minDamageValue,double piercing,double accuracy,double reloadTime, double setupTime)
		{
			double damage = (minDamage + maxDamage) / 2 * (piercing/100);

            damage = (Math.Max(damage, minDamageValue));

			damage *= accuracy;
            
            if (setupTime >= 0 || reloadTime==0)
			    damage /= reloadTime;           

            damage =Math.Round(damage,2);
            
            
			return damage;
		}
		public static double GetMoraleDps(SkillInfo info)
		{
			
            return GetMoraleDps(info.MoraleDamage, 1, 1);
		}
		public static double GetMoraleDps(WeaponInfo info)
		{
			return GetMoraleDps(info.MoraleDamage,info.Accuracy,info.ReloadTime);
		}
		public static double GetMoraleDps(double moraleDamage,double accuracy,double reloadTime)
		{
			double MoraleDamage = Math.Round(moraleDamage * accuracy * 0.5 / reloadTime,2);
            return MoraleDamage;
		}
						
		public static void CompileToolTips(ref int index,ref string file,BaseInfo info)
		{
			if (info.ToolTipsInfo!=null)
			{
                ArrayList entries = new ArrayList(info.ToolTipsInfo.Keys);
                entries.Sort();

                if (entries.Count > 0)
                {
                    HtmlInsert(ref index, ref file, @"<div class=""tooltips"">");
                    foreach (string key in entries)
                    {
                        string tips = Translation.Translate((int)info.ToolTipsInfo[key]);
                        if (tips != "")
                            HtmlInsert(ref index, ref file, "<span class=\"Description\">" + tips + "</span><br/>");

                    }
                    HtmlInsert(ref index, ref file, "</div>");

                }
               // HtmlInsert(ref index, ref file, "<br/>");
			}
		}	
			
		public static void HtmlInsert(ref int index,ref string html,string s)
		{
            string str = s+"<nl>";
           
            html=html.Insert(index,str);
			index+=str.Length;
		}

        public static void HtmlRemove(ref int index, ref string html, string toRemove)
        {
             html = html.Remove(index, toRemove.Length);
             index -= toRemove.Length;
        }

        public static void CreateFirstPage(string template, Hashtable linkTable, string subPath)
        {
            for (int i = 0; i < 9; i++)
            {
                string file = string.Copy(template);
                StreamWriter writer = new StreamWriter(File.Create(Path.Combine(DataPath.OutputPath, "content/" + Races[i, 0] + subPath)));
                file = Regex.Replace(file, "href=\"style.css\"", "href=\"../../../style.css\"");
                Match m = Regex.Match(file, @"</div>[^<div>]*<div\sid=""content"">");
               
                if (m.Success)
                {
                    int index = m.Index;
                    HtmlInsert(ref index, ref file, "<ul id=\"menu\">");
                  
                    HtmlInsert(ref index, ref file, (string)linkTable[Races[i, 0]]);
                    HtmlInsert(ref index, ref file, "</ul>");                    
                }
                
                file = file.Replace("<nl>", Environment.NewLine);
                writer.Write(file);
                writer.Close();
            }
        }

		
		public static void CreateIndex(string template)
		{
			StreamWriter writer = new StreamWriter(File.Create(Path.Combine(DataPath.OutputPath,"index.htm"))); 
			string file = string.Copy(template);
            file = Regex.Replace(file, @"href=""style.css""", @"href=""style.css""");
            Match m = Regex.Match(file, @"</div>[^<div>]*<div\sid=""content"">");
            
            if (m.Success)
			{
				int index = m.Index;
				HtmlInsert(ref index,ref file,"<ul id=\"menu\">");
				for (int i=0; i<9; i++)
                    HtmlInsert(ref index, ref file, "<li><a href=\"content/" + Races[i, 0] + ".htm\">" + Races[i, 1] + "</a></li>");
                HtmlInsert(ref index, ref file, "<li>&nbsp;</li>");
                HtmlInsert(ref index, ref file, "<li><a href=\"content/dpsChart.htm\">Dps Chart</a></li>");
                HtmlInsert(ref index, ref file, "</ul>");
			}
            file=file.Replace("<nl>", Environment.NewLine);
            writer.Write(file);			
			writer.Close();
	
		}

		public static void CreateRacesIndexes(string template)
		{			
			for (int i=0; i<9; i++)
			{
				string dir = Path.Combine(DataPath.OutputPath,"content/"+Races[i,0]);
			
                Directory.CreateDirectory(dir);
								
				StreamWriter writer = new StreamWriter(File.Create(Path.Combine(DataPath.OutputPath,"content/"+Races[i,0]+".htm"))); 
				string file = string.Copy(template);
                file = Regex.Replace(file, @"href=""style.css""", @"href=""../style.css""");
                Match m = Regex.Match(file,@"</div>[^<div>]*<div\sid=""content"">");
				if (m.Success)
				{
					int index = m.Index;
                    HtmlInsert(ref index, ref file, "<ul id=\"menu\">");
                    HtmlInsert(ref index, ref file, "<li><a href=\"../index.htm\">Back</a></li>");
                    HtmlInsert(ref index, ref file, "<li><a href=\"" + Races[i, 0] + "/buildings/buildings.htm" + "\">Buildings</a></li>");
                    HtmlInsert(ref index, ref file, "<li><a href=\"" + Races[i, 0] + "/units/units.htm" + "\">Units</a></li>");
                    HtmlInsert(ref index, ref file, "<li><a href=\""+Races[i, 0] + "/researches/researches.htm" + "\">Researches</a></li>");
					HtmlInsert(ref index,ref file,"</ul>");					
				}
                m = Regex.Match(file, "</div>[^<div>]*<div\\sid=\"footer\">");
				if (m.Success)
				{
					int index = m.Index;
                    HtmlInsert(ref index, ref file, @"<h1 class=""title"">" + Races[i, 1] + "</h1>");
                    HtmlInsert(ref index, ref file, @"<hr>");
					HtmlInsert(ref index,ref file,@"<div id=""raceManifest"">"+GetRaceManifest(Races[i,0])+@"</div>");
				}
                file=file.Replace("<nl>", Environment.NewLine);
                writer.Write(file);
				writer.Close();
				for (int j=0; j<SubTypes.Length; j++)
					Directory.CreateDirectory(Path.Combine(dir,SubTypes[j]));
			}
		}
	
		public static string GetRaceManifest(string race)
		{
			
			StreamReader reader = new StreamReader(Path.Combine(DataPath.TemplatePath,"racemanifests.txt"));
			string manifests = reader.ReadToEnd();
			reader.Close();			
			string pattern = "<"+race+@">(?<manifest>.*)</"+race+">";
			Match m = Regex.Match(manifests,pattern);
	
			if (m.Success)
			{
				Group grp = m.Groups["manifest"];
				if (grp.Success)
					return grp.Value;
			}
			return "";
		}
	}
}
