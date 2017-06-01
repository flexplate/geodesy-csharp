using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace geodesy_csharp
{
    public static class DMS
    {
        public static double ParseDMS(string dmsStr)
        {
            // check for signed decimal degrees without NSEW, if so return it directly
            double testDms;
            if (double.TryParse(dmsStr,out testDms)) { return testDms; }

            // strip off any sign or compass dir'n & split out separate d/m/s
            var Rgx= new Regex("^ -");
            dmsStr = dmsStr.Trim();
            dmsStr = Rgx.Replace(dmsStr, "");
            Rgx = new Regex("[NSEWnsew]$");
            dmsStr = Rgx.Replace(dmsStr, "");
            var dms  = new List<string>(Regex.Split(dmsStr,"[^ 0 - 9.,] +"));
            if (dms[dms.Count - 1] == "") { dms.RemoveAt(dms.Count - 1); }  // from trailing symbol
            if (dms.Count == 0) { throw new ArgumentOutOfRangeException(); }

            // and convert to decimal degrees...
            double deg;
            switch (dms.Count)
            {
                case 3:  // interpret 3-part result as d/m/s
                    deg = double.Parse(dms[0]) / 1 + double.Parse(dms[1]) / 60 + double.Parse(dms[2]) / 3600;
                    break;
                case 2:  // interpret 2-part result as d/m
                    deg = double.Parse(dms[0]) / 1 + double.Parse(dms[1]) / 60;
                    break;
                case 1:  // just d (possibly decimal) or non-separated dddmmss
                    deg = double.Parse(dms[0]);
                    // check for fixed-width unseparated format eg 0033709W
                    //if (/[NS]/i.test(dmsStr)) deg = '0' + deg;  // - normalise N/S to 3-digit degrees
                    //if (/[0-9]{7}/.test(deg)) deg = deg.slice(0,3)/1 + deg.slice(3,5)/60 + deg.slice(5)/3600;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (Regex.IsMatch(dmsStr, "^ -|[WSws]$"))// take '-', west and south as -ve
            {
                deg = -deg;
            }
            return deg;
        }
    }
}
