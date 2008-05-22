//	Copyright � 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using System.Collections.Generic;

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	[System.ComponentModel.TypeConverter (typeof (WindowPlacement.Converter))]

	public struct WindowPlacement : System.IEquatable<WindowPlacement>
	{
		public WindowPlacement(Drawing.Rectangle bounds, bool isFullScreen, bool isMinimized)
		{
			this.bounds = bounds;
			this.isFullScreen = isFullScreen;
			this.isMinimized = isMinimized;
		}


		public Drawing.Rectangle				Bounds
		{
			get
			{
				return this.bounds;
			}
		}

		public bool								IsFullScreen
		{
			get
			{
				return this.isFullScreen;
			}
		}

		public bool								IsMinimized
		{
			get
			{
				return this.isMinimized;
			}
		}

		
		#region IEquatable<WindowPlacement> Members

		public bool Equals(WindowPlacement other)
		{
			return this.bounds == other.bounds
				&& this.isFullScreen == other.isFullScreen
				&& this.IsMinimized == other.isMinimized;
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj is WindowPlacement)
			{
				return this.Equals ((WindowPlacement) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.bounds.GetHashCode ();
		}

		public override string ToString()
		{
			return string.Concat (this.bounds.ToString (), ";", this.isFullScreen ? "F" : "", this.isMinimized ? "M" : "");
		}

		public static WindowPlacement Parse(string value)
		{
			int pos = value.LastIndexOf (';');

			if (pos < 0)
			{
				throw new System.FormatException ();
			}

			string rect = value.Substring (0, pos);
			string attr = value.Substring (pos+1);

			return new WindowPlacement (Drawing.Rectangle.Parse (rect), attr.Contains ("F"), attr.Contains ("M"));
		}

		#region Converter Class
		
		public class Converter : Types.AbstractStringConverter
		{
			public override object ParseString(string value, System.Globalization.CultureInfo culture)
			{
				return WindowPlacement.Parse (value);
			}

			public override string ToString(object value, System.Globalization.CultureInfo culture)
			{
				WindowPlacement placement = (WindowPlacement) value;
				return placement.ToString ();
			}
		}
		
		#endregion
		
		private readonly Drawing.Rectangle		bounds;
		private readonly bool					isFullScreen;
		private readonly bool					isMinimized;
	}
}
