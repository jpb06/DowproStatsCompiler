using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace StatsCompiler
{
    public class EntrenchInfo : ExtensionInfo
    {       
        public UnitInfo EntrenchedUnit;

        public EntrenchInfo()
        {            
        }

        public string GetEntrenchInfo()
        {
            string output = "";
            UnitInfo original = this.Parent as UnitInfo;
            if (original != null && EntrenchedUnit!=null)
            {
                if (EntrenchedUnit.ArmorType != original.ArmorType)
                    output += "<li>Change the armor type to " + Translation.Translate(EntrenchedUnit.ArmorType.ToString())+"</li>";
                if (EntrenchedUnit.Mass > original.Mass)    
                    output += "<li>Increases the mass by "+(EntrenchedUnit.Mass - original.Mass)+"</li>";
                
                if (EntrenchedUnit.Sight> original.Sight)
                    output += "<li>Increases the Sight Radius by " + (EntrenchedUnit.Sight - original.Sight) + "</li>";
                foreach (WeaponHardPointInfo whp in original.WeaponHardPoints.Values)
                {
                    foreach(WeaponInfo wInfo in whp.Weapons)
                    if (!wInfo.IsDummyWeapon())
                        output += "<li>Disables "+Translation.Translate(wInfo.Name)+"</li>";
                }
                foreach (WeaponHardPointInfo whp in EntrenchedUnit.WeaponHardPoints.Values)
                {
                    foreach (WeaponInfo wInfo in whp.Weapons)
                        if (!wInfo.IsDummyWeapon())
                            output += "<li>Enables " + Translation.Translate(wInfo.Name) + "</li>";
                }               
            }
            return output;
        }

        public override void Compile(ref int index, ref string html)
        {
            HtmlCompiler.HtmlInsert(ref index, ref html, "<h4>Entrench</h4>");
            HtmlCompiler.CompileToolTips(ref index, ref html, this);
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<table class=""box"">");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<div class=""boxHeader skillHeader"">Entrench</div>");
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
            HtmlCompiler.HtmlInsert(ref index, ref html, "Toggled Ability");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");

            #endregion

            #region target-TYPE
            HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Target types</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "Self");
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

            #region Modifiers

            string mod = "<li>This unit becomes immobile</li>";
            mod += HtmlCompiler.CompileModifiers(Modifiers)+GetEntrenchInfo();


            
            HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Effects</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td><ul class=""innerInfoList"">");
            HtmlCompiler.HtmlInsert(ref index, ref html, mod);
            HtmlCompiler.HtmlInsert(ref index, ref html, "</ul></td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
            
                      
            #endregion

            #endregion
            HtmlCompiler.HtmlInsert(ref index, ref html, @"</table></td></tr>");           

            HtmlCompiler.HtmlInsert(ref index, ref html, "</table><br>");
        }

    }

}
