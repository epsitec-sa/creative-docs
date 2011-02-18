//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// The <c>GridLength</c> structure represents the length of elements that
	/// support absolute, relative and automatic values.
	/// </summary>
	[SerializationConverter (typeof (GridLength.SerializationConverter))]
	public struct GridLength : System.IEquatable<GridLength>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:GridLength"/> structure.
		/// </summary>
		/// <param name="value">The absolute value.</param>
		public GridLength(double value)
		{
			this.value = value;
			this.gridUnitType = GridUnitType.Absolute;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GridLength"/> structure.
		/// </summary>
		/// <param name="value">The absolute or relative value.</param>
		/// <param name="gridUnitType">The grid unit type.</param>
		public GridLength(double value, GridUnitType gridUnitType)
		{
			this.value = value;
			this.gridUnitType = gridUnitType;
		}

		public static readonly GridLength		Auto = new GridLength (0, GridUnitType.Auto);

		/// <summary>
		/// Gets a value indicating whether this length is absolute.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this length is absolute; otherwise, <c>false</c>.
		/// </value>
		public bool								IsAbsolute
		{
			get
			{
				return this.gridUnitType == GridUnitType.Absolute;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this length is automatic.
		/// </summary>
		/// <value><c>true</c> if this length is automatic; otherwise, <c>false</c>.</value>
		public bool								IsAuto
		{
			get
			{
				return this.gridUnitType == GridUnitType.Auto;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this length is proportional.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this length is proportional; otherwise, <c>false</c>.
		/// </value>
		public bool								IsProportional
		{
			get
			{
				return this.gridUnitType == GridUnitType.Proportional;
			}
		}

		/// <summary>
		/// Gets the grid unit type of the length.
		/// </summary>
		/// <value>The grid unit type of the length.</value>
		public GridUnitType						GridUnitType
		{
			get
			{
				return this.gridUnitType;
			}
		}

		/// <summary>
		/// Gets the value of the length.
		/// </summary>
		/// <value>The value of the length.</value>
		public double							Value
		{
			get
			{
				return this.value;
			}
		}
		
		#region SerializationConverter Class

		public class SerializationConverter : ISerializationConverter
		{
			#region ISerializationConverter Members

			public string ConvertToString(object value, IContextResolver context)
			{
				GridLength length = (GridLength) value;
				
				switch (length.GridUnitType)
				{
					case GridUnitType.Auto:
						return "Auto";
					
					case GridUnitType.Absolute:
						return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}", length.Value);
					
					case GridUnitType.Proportional:
						if (length.Value == 1)
						{
							return "*";
						}
						else
						{
							return string.Format (System.Globalization.CultureInfo.InvariantCulture, "*{0}", length.Value);
						}
				}

				throw new System.InvalidOperationException ("Cannot convert GridLength");
			}

			public object ConvertFromString(string value, IContextResolver context)
			{
				if (value == "Auto")
				{
					return GridLength.Auto;
				}
				if (value.StartsWith ("*"))
				{
					double num;
					
					if (value.Length > 1)
					{
						num = System.Double.Parse (value.Substring (1), System.Globalization.CultureInfo.InvariantCulture);
					}
					else
					{
						num = 1;
					}
					return new GridLength (num, GridUnitType.Proportional);
				}
				
				return new GridLength (System.Double.Parse (value, System.Globalization.CultureInfo.InvariantCulture));
			}

			#endregion
		}

		#endregion

		#region IEquatable<GridLength> Members

		public bool Equals(GridLength other)
		{
			return (this.value == other.value)
				&& (this.gridUnitType == other.gridUnitType);
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj is GridLength)
			{
				return this.Equals ((GridLength) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.value.GetHashCode () ^ this.gridUnitType.GetHashCode ();
		}

		public static bool operator==(GridLength a, GridLength b)
		{
			return a.Equals (b);
		}

		public static bool operator!=(GridLength a, GridLength b)
		{
			return !a.Equals (b);
		}

		private double							value;
		private GridUnitType					gridUnitType;
	}
}
