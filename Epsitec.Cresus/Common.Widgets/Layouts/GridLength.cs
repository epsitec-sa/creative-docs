//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets.Layouts
{
	[SerializationConverter (typeof (GridLength.SerializationConverter))]
	public struct GridLength
	{
		public GridLength(double value)
		{
			this.value = value;
			this.gridUnitType = GridUnitType.Absolute;
		}

		public GridLength(double value, GridUnitType gridUnitType)
		{
			this.value = value;
			this.gridUnitType = gridUnitType;
		}

		public static readonly GridLength		Auto = new GridLength (0, GridUnitType.Auto);
		
		public bool								IsAbsolute
		{
			get
			{
				return this.gridUnitType == GridUnitType.Absolute;
			}
		}

		public bool								IsAuto
		{
			get
			{
				return this.gridUnitType == GridUnitType.Auto;
			}
		}

		public bool								IsProportional
		{
			get
			{
				return this.gridUnitType == GridUnitType.Proportional;
			}
		}
		
		public GridUnitType						GridUnitType
		{
			get
			{
				return this.gridUnitType;
			}
		}

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

		private double							value;
		private GridUnitType					gridUnitType;
	}
}
