using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace StatsCompiler
{
    public class FearInfo : ExtensionInfo
    {
        public double RechargeTime;
        public double Duration;
        public double Radius;
        public WeaponInfo Weapon;

        public FearInfo()
        {
            Weapon = new WeaponInfo();
        }

        public string FormatFilter()
        {
            string output = "";
            string temp = "";

            int count = 0;
            foreach (ArmorTypes ar in Filter)
            {
                if (ar >= ArmorTypes.tp_infantry_low && ar <= ArmorTypes.tp_infantry_heavy_high)
                {
                    count++;
                    temp += Translation.Translate(ar.ToString()) + "*";
                }
            }
            if (count >= 6)
                output += "Infantry*";
            else if (count > 0)
                output += temp;
            temp = "";
            count = 0;
            foreach (ArmorTypes ar in Filter)
            {
                if (ar >= ArmorTypes.tp_monster_low && ar <= ArmorTypes.tp_monster_high)
                {
                    count++;
                    temp += Translation.Translate(ar.ToString()) + "*";
                }
            }
            if (count == 3)
                output += "Monsters*";
            else if (count > 0)
                output += temp;
            temp = "";
            count = 0;
            foreach (ArmorTypes ar in Filter)
            {
                if (ar == ArmorTypes.tp_air_low || (ar >= ArmorTypes.tp_vehicle_low && ar <= ArmorTypes.tp_vehicle_high))
                {
                    count++;
                    temp += Translation.Translate(ar.ToString()) + "*";
                }
            }
            if (count == 4)
                output += "Vehicles*";
            else if (count > 0)
                output += temp;
            temp = "";
            count = 0;
            foreach (ArmorTypes ar in Filter)
            {
                if (ar >= ArmorTypes.tp_building_low && ar <= ArmorTypes.tp_building_high)
                {
                    count++;
                    temp += Translation.Translate(ar.ToString()) + "*";
                }
            }
            if (count == 3)
                output += "Buildings*";
            else if (count > 0)
                output += temp;

            if (Filter.Contains(ArmorTypes.tp_commander))
                output += "Commanders*";

            output = Regex.Replace(output, @"\*(?<word>[^\*]*)\*$", @" and ${word}");
            output = Regex.Replace(output, @"\*$", @"");
            output = Regex.Replace(output, @"\*", @", ");


            if (this.Radius > 0)
            {
                output += " in the surrounding area.";
            }
           
            return output;
        }
        public string FormatAlliance()
        {
            if (AOEFilter == "tp_area_filter_allied")
                return "Allied ";
            if (AOEFilter == "tp_area_filter_enemy")
                return "Enemy ";
            if (AOEFilter == "tp_area_filter_own")
                return "Friendly ";
            return "";
        }
        public override void Compile(ref int index, ref string html)
        {
            HtmlCompiler.HtmlInsert(ref index, ref html, "<h4>" + Name + "</h4>");
            HtmlCompiler.CompileToolTips(ref index, ref html, this);
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<table class=""box"">");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<div class=""boxHeader skillHeader"">" + Name + "</div>");
            if (Icon == null)
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<img class=""icon"" src=""../../../images/general/PassiveAbility_icon.jpg"">");
            else HtmlCompiler.HtmlInsert(ref index, ref html, @"<img class=""icon"" src=""../../../images/" + Icon + @".png"">");


            HtmlCompiler.HtmlInsert(ref index, ref html, @"<div class=""innerInfo"">");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<table class=""innerInfoTable"">");


            #region SKILL INFO TABLE

            #region APPLICATION-TYPE

            HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Application Type</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "Activated Ability");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");

            #endregion

            #region target-TYPE
            
            HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Target types</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, FormatAlliance()+" "+FormatFilter() );
            HtmlCompiler.HtmlInsert(ref index, ref html, @"</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
            
            #endregion
            
            #region REQUIREMENTS

            if (Requirements != null && Requirements.Count > 0)
            {
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Requirements</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, HtmlCompiler.CompileRequirements(Requirements));
                HtmlCompiler.HtmlInsert(ref index, ref html, "</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
            }

            #endregion

            #region MODIFIERS
            
            string mod = "";
            mod += HtmlCompiler.CompileModifiers(Modifiers);

            
            if (mod != "")
            {
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Effects</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td><ul class=""innerInfoList"">");
                HtmlCompiler.HtmlInsert(ref index, ref html, mod);
                HtmlCompiler.HtmlInsert(ref index, ref html, "</ul></td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
            }
            
            #endregion

            #region AREA OF EFFECT - RANGE
            
            if (Radius > 0)
            {
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Area of Effect</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>" + Radius + "</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
            }
            
            #endregion

            #region DURATION AND RECHARGE TIME
            
            
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Duration</td>");

                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>" + Duration.ToString() + " seconds.</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
            
            
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Recharge time</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>" + RechargeTime.ToString() + " seconds.</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
            
            
            #endregion


            #endregion

            HtmlCompiler.HtmlInsert(ref index, ref html, @"</table>");

            #region DAMAGE
            
            if (Weapon.MoraleDamage > 0)
            {
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<table class=""dpsTable skillDpsTable"">");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<th colspan=""8"">Damage values</th>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");

                #region DpsTables
                double dps = 0.0;
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");

                #region INFANTRY
                HtmlCompiler.HtmlInsert(ref index, ref html, "<td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<table class =""dpsCell"">");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<th colspan=""3"" class=""armorClass"">Infantry</th>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""armorClass"">Low</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""armorClass"">Med</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""armorClass"">High</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                dps = 0;
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""dpsValue " + HtmlCompiler.GetDamageColor(dps) + @""">" + dps + "</td>");

                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""dpsValue " + HtmlCompiler.GetDamageColor(dps) + @""">" + dps + "</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""dpsValue " + HtmlCompiler.GetDamageColor(dps) + @""">" + dps + "</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");

                HtmlCompiler.HtmlInsert(ref index, ref html, "</table>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</td>");
                #endregion
                #region HEAVY INFANTRY
                HtmlCompiler.HtmlInsert(ref index, ref html, "<td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<table class =""dpsCell"">");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<th colspan=""3"" class=""armorClass"">Heavy Infantry</th>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""armorClass"">Low</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""armorClass"">Med</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""armorClass"">High</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""dpsValue " + HtmlCompiler.GetDamageColor(dps) + @""">" + dps + "</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""dpsValue " + HtmlCompiler.GetDamageColor(dps) + @""">" + dps + "</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""dpsValue " + HtmlCompiler.GetDamageColor(dps) + @""">" + dps + "</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</table>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</td>"); ;
                #endregion
                #region COMMANDERS

                HtmlCompiler.HtmlInsert(ref index, ref html, "<td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<table class =""dpsCell"">");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<th class=""armorClass"">Comm</th>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""armorClass"">&nbsp</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""dpsValue " + HtmlCompiler.GetDamageColor(dps) + @""">" + dps + "</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</table>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</td>");

                #endregion
                #region VEHICLES
                HtmlCompiler.HtmlInsert(ref index, ref html, "<td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<table class =""dpsCell"">");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<th colspan=""4"" class=""armorClass"">Vehicles</th>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""armorClass"">U.Low</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""armorClass"">Low</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""armorClass"">Med</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""armorClass"">High</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""dpsValue " + HtmlCompiler.GetDamageColor(dps) + @""">" + dps + "</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""dpsValue " + HtmlCompiler.GetDamageColor(dps) + @""">" + dps + "</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""dpsValue " + HtmlCompiler.GetDamageColor(dps) + @""">" + dps + "</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""dpsValue " + HtmlCompiler.GetDamageColor(dps) + @""">" + dps + "</td>");

                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</table>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</td>"); ;
                #endregion
                #region BUILDINGS
                HtmlCompiler.HtmlInsert(ref index, ref html, "<td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<table class =""dpsCell"">");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<th colspan=""3"" class=""armorClass"">Buildings</th>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""armorClass"">Low</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""armorClass"">Med</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""armorClass"">High</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""dpsValue " + HtmlCompiler.GetDamageColor(dps) + @""">" + dps + "</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""dpsValue " + HtmlCompiler.GetDamageColor(dps) + @""">" + dps + "</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""dpsValue " + HtmlCompiler.GetDamageColor(dps) + @""">" + dps + "</td>");

                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</table>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</td>"); ;
                #endregion
                #region MONSTERS
                HtmlCompiler.HtmlInsert(ref index, ref html, "<td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<table class =""dpsCell"">");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<th colspan=""3"" class=""armorClass"">Monsters</th>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""armorClass"">Low</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""armorClass"">Med</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""armorClass"">High</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""dpsValue " + HtmlCompiler.GetDamageColor(dps) + @""">" + dps + "</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""dpsValue " + HtmlCompiler.GetDamageColor(dps) + @""">" + dps + "</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""dpsValue " + HtmlCompiler.GetDamageColor(dps) + @""">" + dps + "</td>");

                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</table>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</td>"); ;
                #endregion
                #region MORALE
                HtmlCompiler.HtmlInsert(ref index, ref html, "<td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<table class =""dpsCell"">");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<th class=""armorClass"">Morale</th>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""armorClass"">&nbsp</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                dps = this.Weapon.MoraleDamage;
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""dpsValue " + HtmlCompiler.GetMoraleColor(dps) + @""">" + dps + "</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</table>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</td>");
                #endregion
                /*
                #region Throw FORCE
                HtmlCompiler.HtmlInsert(ref index, ref html, "<td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<table class =""dpsCell"">");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<th class=""armorClass"">Throw</th>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""armorClass"">force</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<td class=""dpsValue"">--</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</table>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</td>");
                #endregion
                */
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</table>");                
                #endregion
            }
            
            #endregion
            
            HtmlCompiler.HtmlInsert(ref index, ref html, @"</div>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"</td></tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"</table>");
            
        }
    }

}
