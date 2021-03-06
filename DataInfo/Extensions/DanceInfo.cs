using System;
using System.Collections.Generic;
using System.Text;

namespace StatsCompiler
{
    public class DanceInfo : ExtensionInfo
    {
        public double Duration;
        public double Recharge;

        public DanceInfo()
        {
            Name = "Dance of Death";    
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
            HtmlCompiler.HtmlInsert(ref index, ref html, "Self");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
            #endregion
            #region target-TYPE
            HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Duration</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, Duration+ " seconds");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
            #endregion
            #region target-TYPE
            HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Recharge Time</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, Recharge+" seconds");
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

            #region EFFECTS
            HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Effects</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td><ul class=""innerInfoList"">");
            HtmlCompiler.HtmlInsert(ref index, ref html, "<li>The unit starts to leap and dance among enemy troops</li>");
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

