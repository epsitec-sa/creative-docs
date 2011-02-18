//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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

		/// <summary>
		/// Gets the radius for the <c>Sinc</c>, <c>Lanczos</c> and <c>Blackman</c> filtering
		/// modes.
		/// </summary>
		/// <value>The radius.</value>
		public double Radius
		{
			//	Le rayon est effectif pour les modes Sinc (c'est la fonction "sin(x)/x"), Lanczos et Blackman.
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
