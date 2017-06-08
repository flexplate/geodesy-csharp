using System;

namespace geodesy
{
	public class Vector3D
	{
		public Vector3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }

		/// <summary>
		/// Length (magnitude or norm) of ‘this’ vector
		/// </summary>
		public double Length
		{
			get { return Math.Sqrt(X * X + Y * Y + Z * Z); }
		}

		#region Basic arithmetic operators

		public static Vector3D operator +(Vector3D v1, Vector3D v2)
		{
			return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
		}

		public static Vector3D operator -(Vector3D v1, Vector3D v2)
		{
			return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
		}

		public static Vector3D operator *(Vector3D v1, Vector3D v2)
		{
			return new Vector3D(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
		}

		public static Vector3D operator /(Vector3D v1, Vector3D v2)
		{
			return new Vector3D(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z);
		}
		#endregion

		#region Products

		/// <summary>
		/// Multiplies this vector by the supplied vector using dot (scalar) product.
		/// </summary>
		/// <param name="v">Vector to be dotted with this vector.</param>
		/// <returns>Dot product of ‘this’ and v.</returns>
		public double Dot(Vector3D v)
		{
			return X * v.X + Y * v.Y + Z * v.Z;
		}

		/// <summary>
		/// Multiplies this vector by the supplied vector using cross (vector) product.
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public Vector3D Cross(Vector3D v)
		{
			var x = Y * v.Z - Z * v.Y;
			var y = Z * v.X - X * v.Z;
			var z = X * v.Y - Y * v.X;

			return new Vector3D(x, y, z);
		}

		/// <summary>
		/// Negates a vector to point in the opposite direction.
		/// </summary>
		/// <returns>Negated vector.</returns>
		public Vector3D Negate()
		{
			return new Vector3D(-X, -Y, -Z);
		}

		/// <summary>
		/// Normalizes a vector to its unit vector.
		/// </summary>
		/// <remarks>If the vector is already unit or is zero magnitude, this is a no-op.</remarks>
		/// <returns>Normalised version of this vector</returns>
		public Vector3D Unit()
		{
			if (Length == 1 || Length == 0) { return this; }

			var NormX = X / Length;
			var NormY = X / Length;
			var NormZ = X / Length;
			return new Vector3D(NormX, NormY, NormZ);
		}
		#endregion

		public string ToString(int precision =3)
		{
			string NumberFormat = new string('0', precision);
			return string.Format(@"[{0},{1},{2}]", X.ToString(NumberFormat), Y.ToString(NumberFormat), Z.ToString(NumberFormat));
		}

		/// <summary>
		/// Applies Helmert transform to ‘this’ point using transform parameters t.
		/// </summary>
		/// <param name="transform">Transform to apply to this point.</param>
		public Vector3D ApplyTransform(double[] transform)
		{
			// this point
			var X1 = X;
			var Y1 = Y;
			var Z1 = Z;

			// transform parameters
			var Tx = transform[0];                      // x-shift
			var Ty = transform[1];                      // y-shift
			var Tz = transform[2];                      // z-shift
			var S1 = transform[3] / 1e6 + 1;            // scale: normalise parts-per-million to (s+1)
			var Rx = (transform[4] / 3600).ToRadians(); // x-rotation: normalise arcseconds to radians
			var Ry = (transform[5] / 3600).ToRadians(); // y-rotation: normalise arcseconds to radians
			var Rz = (transform[6] / 3600).ToRadians(); // z-rotation: normalise arcseconds to radians

			// apply transform
			var X2 = Tx + X1 * S1 - Y1 * Rz + Z1 * Ry;
			var Y2 = Ty + X1 * Rz + Y1 * S1 - Z1 * Rx;
			var Z2 = Tz - X1 * Ry + Y1 * Rx + Z1 * S1;

			return new Vector3D(X2, Y2, Z2);
		}

		/// <summary>
		/// Converts ‘this’ (geocentric) cartesian (x/y/z) point to (ellipsoidal geodetic) latitude/longitude coordinates on specified datum.
		/// Uses Bowring’s (1985) formulation for μm precision in concise form.
		/// </summary>
		/// <param name="toDatum">Datum to use when converting point.</param>
		public LatLonEllipsoidal ToLatLonE(Datum toDatum)
		{
			var Major = toDatum.Ellipsoid.Major;
			var Minor = toDatum.Ellipsoid.Minor;
			var Flattening = toDatum.Ellipsoid.Flattening;

			var E2 = 2 * Flattening - Flattening * Flattening;   // 1st eccentricity squared ≡ (a²-b²)/a²
			var ε2 = E2 / (1 - E2); // 2nd eccentricity squared ≡ (a²-b²)/b²
			var P = Math.Sqrt(X * X + Y * Y); // distance from minor axis
			var R = Math.Sqrt(P * P + Z * Z); // polar radius

			// parametric latitude (Bowring eqn 17, replacing tanβ = z·a / p·b)
			var Tanβ = (Minor * Z) / (Major * P) * (1 + ε2 * Minor / R);
			var Sinβ = Tanβ / Math.Sqrt(1 + Tanβ * Tanβ);
			var Cosβ = Sinβ / Tanβ;

			// geodetic latitude (Bowring eqn 18: tanφ = z+ε²bsin³β / p−e²cos³β)
			var φ = Math.Atan2(Z + ε2 * Minor * Sinβ * Sinβ * Sinβ, P - E2 * Major * Cosβ * Cosβ * Cosβ);

			// longitude
			var λ = Math.Atan2(Y, X);

			// height above ellipsoid (Bowring eqn 7) [not currently used]
			var Sinφ = Math.Sin(φ);
			var Cosφ = Math.Cos(φ);
			var V = Major / Math.Sqrt(1 - E2 * Sinφ * Sinφ); // length of the normal terminated by the minor axis
			var H = P * Cosφ + Z * Sinφ - (Major * Major / V);

			var point = new LatLonEllipsoidal(φ.ToDegrees(), λ.ToDegrees(), toDatum);

			return point;
		}

		/// <summary>
		/// Calculates the angle between ‘this’ vector and supplied vector.
		/// </summary>
		/// <param name="inputVector">Vector to calculate angle.</param>
		/// <param name="normal">Plane normal: if supplied, angle is -π..+π, signed +ve if this->v is clockwise looking along n, -ve in opposite direction(if not supplied, angle is always 0..π).</param>
		/// <returns>Angle (in radians) between this vector and inputVector.</returns>
		public double AngleTo(Vector3D inputVector, Vector3D normal = null)
		{
			var Sign = normal == null ? 1 : Math.Sign(this.Cross(inputVector).Dot(normal));
			var Sinθ = this.Cross(inputVector).Length * Sign;
			var Cosθ = this.Dot(inputVector);
			return Math.Atan2(Sinθ, Cosθ);
		}

		/// <summary>
		/// Rotates this point around an axis by a specified angle.
		/// </summary>
		/// <param name="axis">The axis being rotated around.</param>
		/// <param name="theta">The angle of rotation (in radians).</param>
		/// <returns>The rotated point.</returns>
		public Vector3D RotateAround(Vector3D axis, double theta)
		{
			// en.wikipedia.org/wiki/Rotation_matrix#Rotation_matrix_from_axis_and_angle
			// en.wikipedia.org/wiki/Quaternions_and_spatial_rotation#Quaternion-derived_rotation_matrix
			Vector3D P1 = this.Unit();
			double[] P = { P1.X, P1.Y, P1.Z }; // the point being rotated
			Vector3D a = axis.Unit();          // the axis being rotated around
			double s = Math.Sin(theta);
			double c = Math.Cos(theta);

			// quaternion-derived rotation matrix
			double[][] q = {
				new double[] { a.X * a.X * (1 - c) + c, a.X * a.Y * (1 - c) - a.Z * s, a.X * a.Z * (1 - c) + a.Y * s },
				new double[] { a.Y * a.X * (1 - c) + a.Z * s, a.Y * a.Y * (1 - c) + c, a.Y * a.Z * (1 - c) - a.X * s },
				new double[] { a.Z * a.X * (1 - c) - a.Y * s, a.Z * a.Y * (1 - c) + a.X * s, a.Z * a.Z * (1 - c) + c}
		    };

			// multiply q × p
			double[] QP = { 0, 0, 0 };
			for (var i = 0; i < 3; i++)
			{
				for (var j = 0; j < 3; j++)
				{
					QP[i] += q[i][j] * P[j];
				}
			}
			var p2 = new Vector3D(QP[0], QP[1], QP[2]);
			return p2;
		}
	}
}
