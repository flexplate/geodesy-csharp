using System;
using System.Linq;

namespace geodesy
{
	public class LatLon
	{
		public double Latitude { get; set; }
		public double Longitude { get; set; }

		public LatLon(double latitude, double longitude)
		{
			Latitude = latitude;
			Longitude = longitude;
		}

		public string ToString(string format, int decimals)
		{
			format = format.ToLower();
			if (new string[] { "d", "dm", "dms" }.Contains(format))
			{
				if ((decimals >= 0) && (decimals % 2 == 0) && decimals <= 4)
				{
					return DMS.ToLat(Latitude, format, decimals) + ", " + DMS.ToLon(Longitude, format, decimals);
				}
				else
				{
					throw new ArgumentException("Decimals must be 0, 2 or 4");
				}
			}
			else
			{
				throw new ArgumentException("Format must be d|dm|dms");
			}
		}

		#region Overriding equality operators
		public override bool Equals(object obj)
		{
			var NewObj = obj as LatLon;
			if (null != NewObj)
			{
				return Latitude == NewObj.Latitude
					&& Longitude == NewObj.Longitude;
			}
			else
			{
				return base.Equals(obj);
			}
		}
		public override int GetHashCode()
		{
			unchecked // Overflow is fine, just wrap
			{
				int hash = (int)2166136261;
				// Suitable nullity checks etc, of course :)
				hash = hash * 16777619 + Latitude.GetHashCode();
				hash = hash * 16777619 + Longitude.GetHashCode();
				return hash;
			}
		}

		public static bool operator ==(LatLon l1, LatLon l2)
		{
			if (l1.Latitude != l2.Latitude || l1.Longitude != l2.Longitude) { return false; }
			return true;
		}
		public static bool operator !=(LatLon l1, LatLon l2)
		{
			if (l1.Latitude != l2.Latitude || l1.Longitude == l2.Longitude) { return true; }
			return false;
		}
		#endregion

	}
}