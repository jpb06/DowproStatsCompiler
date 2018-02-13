using System;
using System.Collections.Generic;
using System.Text;

namespace StatsCompiler
{
    public class PossessEnemyInfo : ExtensionInfo
    {
        public int Range;
        public int Duration;
        public PossessEnemyInfo()
        {
        }

        public override void Compile(ref int index, ref string html)
        {
           
            HtmlCompiler.HtmlInsert(ref index, ref html, "<h4>Possesion</h4>");

            HtmlCompiler.CompileToolTips(ref index, ref html, this);
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<table class=""box"">");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>");
            
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<div class=""boxHeader skillHeader"">Possession</div>");


            HtmlCompiler.HtmlInsert(ref index, ref html, @"<img class=""icon"" src=""../../../images/necron_icons/necron_possess_enemy_icon.png"">");
          
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<div class=""innerInfo"">");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<table class=""innerInfoTable"">");


            #region SKILL INFO TABLE

            #region APPLICATION-TYPE

            HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Application Type</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "Possession");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");

            #endregion

            #region target-TYPE
            HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Target types</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "Enemy Vehicles Low, Vehicles Medium and Vehicles High");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
            #endregion

            #region RANGE
            HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Range</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, Range.ToString());
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

            #region Possession
            HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Effects</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td><ul class=""innerInfoList"">");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<li>Tries to take control of the targeted unit</li>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<li>It needs "+Duration+" seconds to complete the possession</li>");            
            HtmlCompiler.HtmlInsert(ref index, ref html, "</ul></td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
            #endregion

            #endregion

            HtmlCompiler.HtmlInsert(ref index, ref html, @"</table>");

            HtmlCompiler.HtmlInsert(ref index, ref html, @"</div>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"</td></tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"</table>");
        }
    }

}
