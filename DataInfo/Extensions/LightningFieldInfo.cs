using System;
using System.Collections.Generic;
using System.Text;

namespace StatsCompiler
{
    public class LightningFieldInfo : ExtensionInfo
    {
        public int Radius;
               
        public double DischargeDamageRatio; // Multiplier for the discharged damage

        public double MaxCharge; // Maximum points that can be charged
        public double RechargeMinFraction; // Minimum fraction of charge needed to discharge
        public double ReflectedDamageRatio; //Multiplier for the reflected damage
        public double RechargeImpactRatio;  // Multiplier for the charge gained from melee attacks


        public LightningFieldInfo()
        {
        }

        public override void Compile(ref int index, ref string html)
        {

            HtmlCompiler.HtmlInsert(ref index, ref html, "<h4>Lightning Field</h4>");

            HtmlCompiler.CompileToolTips(ref index, ref html, this);
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<table class=""box"">");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>");

            HtmlCompiler.HtmlInsert(ref index, ref html, @"<div class=""boxHeader skillHeader"">Lightning Field</div>");

            HtmlCompiler.HtmlInsert(ref index, ref html, @"<img class=""icon"" src=""../../../images/necron_icons/necron_lightning_field_icon.png"">");

            HtmlCompiler.HtmlInsert(ref index, ref html, @"<div class=""innerInfo"">");
           


            #region SKILL INFO TABLE
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<table class=""innerInfoTable"">");
            #region APPLICATION-TYPE

            HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Application Type</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "Passive");
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

            #region EFFECTS
            HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Charge</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td><ul class=""innerInfoList"">");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<li>Damage taken by the unit will charge the Lightning Field</li>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<li>Can store up to 900 points of damage</li>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<li>"+ReflectedDamageRatio*100+"% of the melee damage taken by the unit is reflected back to the enemy</li>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<li>"+RechargeImpactRatio*100+"% of the melee damage inflicted by the unit charges the Lightning Field</li>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "</ul></td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
            #endregion

            HtmlCompiler.HtmlInsert(ref index, ref html, @"</table>");
            #endregion

            

            #region DISCHARGE
            
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<table class=""innerInfoTable"">");
            
            HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Discharge</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "&nbsp;");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
            #region APPLICATION-TYPE

            HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Application Type</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "Target Ability");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");

            #endregion

            #region target-TYPE
            HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Target types</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "Enemy Infantries Vehicles and Buildings in the area of effect");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
            #endregion

            #region AOE
            HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Area of Effect</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, Radius.ToString());
            HtmlCompiler.HtmlInsert(ref index, ref html, @"</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
            #endregion

            #region REQUIREMENTS

            if (Requirements != null && Requirements.Count > 0)
            {
                HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Requirements</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "<td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "At least "+MaxCharge*RechargeMinFraction+" of charged damage points");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</td>");
                HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
            }

            #endregion

            #region EFFECTS
            HtmlCompiler.HtmlInsert(ref index, ref html, "<tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "<td>Charge</td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<td><ul class=""innerInfoList"">");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<li>Discharge the Lightning Field</li>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"<li>Deals a damage equal to "+DischargeDamageRatio*100+"% of the charged points to each target in the area of effect.</li>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "</ul></td>");
            HtmlCompiler.HtmlInsert(ref index, ref html, "</tr>");
            #endregion

            HtmlCompiler.HtmlInsert(ref index, ref html, @"</table>");            
            
            #endregion

            HtmlCompiler.HtmlInsert(ref index, ref html, @"</div>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"</td></tr>");
            HtmlCompiler.HtmlInsert(ref index, ref html, @"</table>");
        }
    }

}
