using System;
using System.Collections.Generic;
using System.Text;

namespace StatsCompiler
{
    public class PossessInfo : ExtensionInfo
    {
        private string m_Replacement;
        public string Replacement
        {
            get { return m_Replacement; }
            set { m_Replacement = value; }
        }
        public int Duration;        
        public PossessInfo()
        {
        }

        public override void Compile(ref int index, ref string html)
        {
            if (Name != null && Name != "")
                HtmlCompiler.HtmlInsert(ref index, ref html, "<h4>" + Name + "</h4>");
            else HtmlCompiler.HtmlInsert(ref index, ref html, "<h4>" + Translation.Translate(Replacement) + "</h4>");

            HtmlCompiler.CompileToolTips(ref index, ref html, this);
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<table class=""box"">");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>");
            if (Name != null && Name != "")
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<div class=""boxHeader skillHeader"">" + Name + "</div>");
            else HtmlCompiler.HtmlInsert(ref index, ref html, @"<div class=""boxHeader skillHeader"">" + Translation.Translate(Replacement) + "</div>");

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
            HtmlCompiler.HtmlInsert(ref index, ref html, "Possession");
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

            #region Possession
            HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Effects</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td><ul class=""innerInfoList"">");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<li>The "+((BuildableInfo)Parent).Name+" is replaced by " + Translation.Translate(Replacement)+"</li>");
            if (Duration != 0)
                HtmlCompiler.HtmlInsert(ref index, ref html, @"<li>Automatically starts after " + Duration + " seconds</li>");
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
