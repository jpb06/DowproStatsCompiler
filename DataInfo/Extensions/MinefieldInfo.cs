using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace StatsCompiler
{
    public class MinefieldInfo : ExtensionInfo
    {
        public double ExplosionRecharge;
        public int maxExplosions;
        public double Radius;
        public WeaponInfo Weapon;

        public MinefieldInfo()
        {
            Weapon = new WeaponInfo();
        }
        public override void Compile(ref int index, ref string html)
        {
            HtmlCompiler.CompileWeapon(ref index,ref html, Weapon);
        }
    }

}

