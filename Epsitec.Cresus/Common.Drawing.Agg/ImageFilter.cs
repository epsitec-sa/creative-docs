//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	[System.Serializable]

	/// <summary>
	/// The <c>ImageFilter</c> structure defines the image filtering setting.
	/// </summary>
	public struct ImageFilter : System.IEquatable<ImageFilter>
	{
		public ImageFilter(ImageFilteringMode mode)
		{
			this.mode = mode;
			this.radius = 1.0;
		}

		public ImageFilter(ImageFilteringMode mode, double radius)
		{
			this.mode = mode;
			this.radius = radius;
		}

		public ImageFilteringMode Mode
		{
			get
			{
				return this.mode;
			}
		}

		public double Radius
		{
			//	Le rayon est effectif pour les modes Sync, Lanczos et Blackman.
			get
			{
				return this.radius;
			}
		}

		public bool Active
		{
			get
			{
				return this.mode != ImageFilteringMode.None;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is ImageFilter)
			{
				return this == (ImageFilter) obj;
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.mode.GetHashCode () ^ this.radius.GetHashCode ();
		}

		#region IEquatable<ImageFilter> Members

		public bool Equals(ImageFilter other)
		{
			return this == other;
		}

		#endregion

		public static bool operator==(ImageFilter a, ImageFilter b)
		{
			return (a.mode == b.mode) && (a.radius == b.radius);
		}

		public static bool operator!=(ImageFilter a, ImageFilter b)
		{
			return (a.mode != b.mode) || (a.radius != b.radius);
		}

		private ImageFilteringMode mode;
		private double radius;
	}
}
